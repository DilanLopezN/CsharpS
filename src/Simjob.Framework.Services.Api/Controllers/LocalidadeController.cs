using MediatR;
using Microsoft.AspNetCore.Mvc;
using Simjob.Framework.Application.Controllers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Services.Api.Services;
using System.Threading.Tasks;
using Simjob.Framework.Infra.Identity.Entities;
using Newtonsoft.Json;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Schemas.Interfaces;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Schemas.Entities;
using Microsoft.AspNetCore.Authorization;



namespace Simjob.Framework.Services.Api.Controllers
{
  [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public class LocalidadeController : BaseController
  {
    private readonly IRepository<SourceContext, Source> _sourceRepository;
    private readonly ISchemaBuilder _schemaBuilder;
    private readonly IRepository<MongoDbContext, Schema> _schemaRepository;
    public LocalidadeController(
        IMediatorHandler bus,
        INotificationHandler<DomainNotification> notifications)
        : base(bus, notifications)
    {


    }


    /// <summary>
    /// Consulta ViaCEP e corrige automaticamente a hierarquia do CEP no banco
    /// </summary>
    /// <param name="cep">CEP a ser consultado (com ou sem formatação)</param>
    /// <param name="cd_tipo_logradouro">Código do tipo de logradouro (opcional)</param>
    /// <returns>Dados corrigidos do CEP com lista de correções realizadas</returns>
    [HttpGet("corrigir-cep/{cep}")]
    public async Task<IActionResult> CorrigirCEP(
            string cep,
            [FromQuery] int? cd_tipo_logradouro = null)
    {
      var fonteDados = ValidarFonteDados();
      if (!fonteDados.success)
      {
        return BadRequest(new { error = fonteDados.error });
      }

      var resultado = await LocalidadeService.ConsultarECorrigirCEP(
          cep,
          fonteDados.source,
          cd_tipo_logradouro);

      if (!resultado.success)
      {
        return BadRequest(new { error = resultado.error });
      }

      return Ok(new
      {
        success = true,
        message = resultado.data.tinha_erro
              ? "CEP consultado e hierarquia corrigida com sucesso"
              : "CEP consultado - hierarquia já estava correta",
        data = resultado.data
      });
    }

    private (Source source, bool success, string error) ValidarFonteDados()
    {
      var schemaName = "T_LOCALIDADE";

      // Remove prefixo T_ se existir
      if (schemaName.Contains("T_"))
        schemaName = schemaName.Replace("T_", "").Replace("_", "");

      // Busca o schema no repositório
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);

      if (schema == null)
      {
        return (null, false, "Schema não encontrado.");
      }

      // Desserializa o schema
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);

      // Busca a fonte de dados
      var source = _sourceRepository.GetByField("description", schemaModel.Source);

      // Valida se a fonte está ativa
      if (source == null || source.Active != true)
      {
        return (null, false, "Fonte de dados não configurada ou inativa.");
      }

      return (source, true, null);
    }


    /// <summary>
    /// Consulta apenas o ViaCEP sem fazer correções no banco
    /// </summary>
    /// <param name="cep">CEP a ser consultado</param>
    [HttpGet("consultar-viacep/{cep}")]
    public async Task<IActionResult> ConsultarViaCEP(string cep)
    {
      try
      {
        var cepLimpo = cep.Replace(".", "").Replace("-", "").Trim();
        if (cepLimpo.Length != 8)
        {
          return BadRequest(new { error = "CEP inválido" });
        }

        using (var httpClient = new System.Net.Http.HttpClient())
        {
          var url = $"https://viacep.com.br/ws/{cepLimpo}/json/";
          var response = await httpClient.GetAsync(url);

          if (!response.IsSuccessStatusCode)
          {
            return BadRequest(new { error = "Erro ao consultar ViaCEP" });
          }

          var json = await response.Content.ReadAsStringAsync();
          return Ok(new
          {
            success = true,
            data = System.Text.Json.JsonSerializer.Deserialize<object>(json)
          });
        }
      }
      catch (System.Exception ex)
      {
        return BadRequest(new { error = $"Erro: {ex.Message}" });
      }
    }

    /// <summary>
    /// Busca a hierarquia atual de um CEP no banco (sem correções)
    /// </summary>
    /// <param name="cep">CEP a ser consultado</param>
    [HttpGet("hierarquia-atual/{cep}")]
    public async Task<IActionResult> BuscarHierarquiaAtual(string cep)
    {
      var fonteDados = ValidarFonteDados();
      if (!fonteDados.success)
      {
        return BadRequest(new { error = fonteDados.error });
      }

      try
      {
        var cepFormatado = cep.Replace(".", "").Replace("-", "").Insert(5, "-");

        var filtrosCep = new System.Collections.Generic.List<(string campo, object valor)>
                {
                    ("dc_num_cep", cepFormatado)
                };

        var cepExists = await SQLServerService.GetFirstByFields(
            fonteDados.source,
            "T_LOCALIDADE",
            filtrosCep);

        if (cepExists == null)
        {
          return NotFound(new
          {
            error = "CEP não encontrado no banco de dados",
            sugestao = $"Use o endpoint /corrigir-cep/{cep} para buscar no ViaCEP e cadastrar"
          });
        }

        // Montar hierarquia completa
        dynamic hierarquia = new
        {
          cep = new
          {
            cd_localidade = cepExists["cd_localidade"],
            no_localidade = cepExists["no_localidade"],
            dc_num_cep = cepExists["dc_num_cep"],
            cd_tipo_logradouro = cepExists["cd_tipo_logradouro"]
          },
          bairro = (object)null,
          cidade = (object)null,
          estado = (object)null,
          pais = (object)null
        };

        // Buscar Bairro
        if (cepExists["cd_loc_relacionada"] != null && cepExists["cd_loc_relacionada"] != System.DBNull.Value)
        {
          var filtroBairro = new System.Collections.Generic.List<(string campo, object valor)>
                    {
                        ("cd_localidade", cepExists["cd_loc_relacionada"].ToString())
                    };
          var bairroExists = await SQLServerService.GetFirstByFields(fonteDados.source, "T_LOCALIDADE", filtroBairro);

          if (bairroExists != null)
          {
            hierarquia = new
            {
              cep = hierarquia.cep,
              bairro = new
              {
                cd_localidade = bairroExists["cd_localidade"],
                no_localidade = bairroExists["no_localidade"]
              },
              cidade = hierarquia.cidade,
              estado = hierarquia.estado,
              pais = hierarquia.pais
            };

            // Buscar Cidade
            if (bairroExists["cd_loc_relacionada"] != null && bairroExists["cd_loc_relacionada"] != System.DBNull.Value)
            {
              var filtroCidade = new System.Collections.Generic.List<(string campo, object valor)>
                            {
                                ("cd_localidade", bairroExists["cd_loc_relacionada"].ToString())
                            };
              var cidadeExists = await SQLServerService.GetFirstByFields(fonteDados.source, "T_LOCALIDADE", filtroCidade);

              if (cidadeExists != null)
              {
                hierarquia = new
                {
                  cep = hierarquia.cep,
                  bairro = hierarquia.bairro,
                  cidade = new
                  {
                    cd_localidade = cidadeExists["cd_localidade"],
                    no_localidade = cidadeExists["no_localidade"]
                  },
                  estado = hierarquia.estado,
                  pais = hierarquia.pais
                };

                // Buscar Estado
                if (cidadeExists["cd_loc_relacionada"] != null && cidadeExists["cd_loc_relacionada"] != System.DBNull.Value)
                {
                  var filtroEstado = new System.Collections.Generic.List<(string campo, object valor)>
                                    {
                                        ("cd_localidade", cidadeExists["cd_loc_relacionada"].ToString())
                                    };
                  var estadoExists = await SQLServerService.GetFirstByFields(fonteDados.source, "T_LOCALIDADE", filtroEstado);

                  if (estadoExists != null)
                  {
                    hierarquia = new
                    {
                      cep = hierarquia.cep,
                      bairro = hierarquia.bairro,
                      cidade = hierarquia.cidade,
                      estado = new
                      {
                        cd_localidade = estadoExists["cd_localidade"],
                        no_localidade = estadoExists["no_localidade"]
                      },
                      pais = hierarquia.pais
                    };

                    // Buscar País
                    if (estadoExists["cd_loc_relacionada"] != null && estadoExists["cd_loc_relacionada"] != System.DBNull.Value)
                    {
                      var filtroPais = new System.Collections.Generic.List<(string campo, object valor)>
                                            {
                                                ("cd_localidade", estadoExists["cd_loc_relacionada"].ToString())
                                            };
                      var paisExists = await SQLServerService.GetFirstByFields(fonteDados.source, "T_LOCALIDADE", filtroPais);

                      if (paisExists != null)
                      {
                        hierarquia = new
                        {
                          cep = hierarquia.cep,
                          bairro = hierarquia.bairro,
                          cidade = hierarquia.cidade,
                          estado = hierarquia.estado,
                          pais = new
                          {
                            cd_localidade = paisExists["cd_localidade"],
                            no_localidade = paisExists["no_localidade"]
                          }
                        };
                      }
                    }
                  }
                }
              }
            }
          }
        }

        return Ok(new
        {
          success = true,
          data = hierarquia,
          observacao = "Esta é a hierarquia atual no banco. Para corrigir inconsistências use /corrigir-cep"
        });
      }
      catch (System.Exception ex)
      {
        return BadRequest(new { error = $"Erro ao buscar hierarquia: {ex.Message}" });
      }
    }

    /// <summary>
    /// Corrige todos os CEPs de uma lista específica
    /// </summary>
    /// <param name="request">Lista de CEPs para corrigir</param>
    [HttpPost("corrigir-ceps-em-lote")]
    public async Task<IActionResult> CorrigirCEPsEmLote([FromBody] CorrigirCEPsEmLoteRequest request)
    {
      var fonteDados = ValidarFonteDados();
      if (!fonteDados.success)
      {
        return BadRequest(new { error = fonteDados.error });
      }

      if (request?.ceps == null || request.ceps.Count == 0)
      {
        return BadRequest(new { error = "Lista de CEPs não informada" });
      }

      var resultados = new System.Collections.Generic.List<object>();
      var sucessos = 0;
      var erros = 0;
      var ja_corretos = 0;

      foreach (var cep in request.ceps)
      {
        var resultado = await LocalidadeService.ConsultarECorrigirCEP(
            cep,
            fonteDados.source,
            null);

        if (resultado.success)
        {
          if (resultado.data.tinha_erro)
          {
            sucessos++;
            resultados.Add(new
            {
              cep = cep,
              status = "corrigido",
              correcoes = resultado.data.correcoes_realizadas
            });
          }
          else
          {
            ja_corretos++;
            resultados.Add(new
            {
              cep = cep,
              status = "ja_correto"
            });
          }
        }
        else
        {
          erros++;
          resultados.Add(new
          {
            cep = cep,
            status = "erro",
            mensagem = resultado.error
          });
        }
      }

      return Ok(new
      {
        success = true,
        resumo = new
        {
          total = request.ceps.Count,
          corrigidos = sucessos,
          ja_corretos = ja_corretos,
          erros = erros
        },
        resultados = resultados
      });
    }

    /// <summary>
    /// Valida a consistência da hierarquia de um CEP sem fazer correções
    /// Retorna true se estiver correto, false se houver inconsistências
    /// </summary>
    [HttpGet("validar-hierarquia/{cep}")]
    public async Task<IActionResult> ValidarHierarquia(string cep)
    {
      var fonteDados = ValidarFonteDados();
      if (!fonteDados.success)
      {
        return BadRequest(new { error = fonteDados.error });
      }

      try
      {
        // Consulta ViaCEP
        var cepLimpo = cep.Replace(".", "").Replace("-", "").Trim();
        using (var httpClient = new System.Net.Http.HttpClient())
        {
          var url = $"https://viacep.com.br/ws/{cepLimpo}/json/";
          var response = await httpClient.GetAsync(url);

          if (!response.IsSuccessStatusCode)
          {
            return BadRequest(new { error = "Erro ao consultar ViaCEP" });
          }

          var json = await response.Content.ReadAsStringAsync();
          var viaCepData = System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.Dictionary<string, object>>(json);

          if (viaCepData.ContainsKey("erro"))
          {
            return NotFound(new { error = "CEP não encontrado no ViaCEP" });
          }

          // Busca hierarquia atual no banco
          var cepFormatado = cepLimpo.Insert(5, "-");
          var filtrosCep = new System.Collections.Generic.List<(string campo, object valor)>
                    {
                        ("dc_num_cep", cepFormatado)
                    };

          var cepExists = await SQLServerService.GetFirstByFields(fonteDados.source, "T_LOCALIDADE", filtrosCep);

          if (cepExists == null)
          {
            return Ok(new
            {
              success = true,
              valido = false,
              motivo = "CEP não cadastrado no banco de dados",
              dados_viacep = viaCepData
            });
          }

          var inconsistencias = new System.Collections.Generic.List<string>();

          // Valida cada nível da hierarquia
          var bairroExists = (object)null;
          var cidadeExists = (object)null;
          var estadoExists = (object)null;

          if (cepExists["cd_loc_relacionada"] != null && cepExists["cd_loc_relacionada"] != System.DBNull.Value)
          {
            var filtroBairro = new System.Collections.Generic.List<(string campo, object valor)>
                        {
                            ("cd_localidade", cepExists["cd_loc_relacionada"].ToString())
                        };
            bairroExists = await SQLServerService.GetFirstByFields(fonteDados.source, "T_LOCALIDADE", filtroBairro);

            if (bairroExists != null)
            {
              var bairroDict = (System.Collections.Generic.Dictionary<string, object>)bairroExists;
              var bairroViaCep = viaCepData.ContainsKey("bairro") ? viaCepData["bairro"]?.ToString() : "";
              var bairroBanco = bairroDict["no_localidade"]?.ToString();

              if (!string.Equals(bairroBanco, bairroViaCep, System.StringComparison.OrdinalIgnoreCase))
              {
                inconsistencias.Add($"Bairro divergente: Banco='{bairroBanco}' vs ViaCEP='{bairroViaCep}'");
              }

              if (bairroDict["cd_loc_relacionada"] != null && bairroDict["cd_loc_relacionada"] != System.DBNull.Value)
              {
                var filtroCidade = new System.Collections.Generic.List<(string campo, object valor)>
                                {
                                    ("cd_localidade", bairroDict["cd_loc_relacionada"].ToString())
                                };
                cidadeExists = await SQLServerService.GetFirstByFields(fonteDados.source, "T_LOCALIDADE", filtroCidade);

                if (cidadeExists != null)
                {
                  var cidadeDict = (System.Collections.Generic.Dictionary<string, object>)cidadeExists;
                  var cidadeViaCep = viaCepData.ContainsKey("localidade") ? viaCepData["localidade"]?.ToString() : "";
                  var cidadeBanco = cidadeDict["no_localidade"]?.ToString();

                  if (!string.Equals(cidadeBanco, cidadeViaCep, System.StringComparison.OrdinalIgnoreCase))
                  {
                    inconsistencias.Add($"Cidade divergente: Banco='{cidadeBanco}' vs ViaCEP='{cidadeViaCep}'");
                  }

                  if (cidadeDict["cd_loc_relacionada"] != null && cidadeDict["cd_loc_relacionada"] != System.DBNull.Value)
                  {
                    var filtroEstado = new System.Collections.Generic.List<(string campo, object valor)>
                                        {
                                            ("cd_localidade", cidadeDict["cd_loc_relacionada"].ToString())
                                        };
                    estadoExists = await SQLServerService.GetFirstByFields(fonteDados.source, "T_LOCALIDADE", filtroEstado);

                    if (estadoExists != null)
                    {
                      var estadoDict = (System.Collections.Generic.Dictionary<string, object>)estadoExists;
                      var ufViaCep = viaCepData.ContainsKey("uf") ? viaCepData["uf"]?.ToString() : "";
                      var estadoBanco = estadoDict["no_localidade"]?.ToString();

                      if (!string.Equals(estadoBanco, ufViaCep, System.StringComparison.OrdinalIgnoreCase))
                      {
                        inconsistencias.Add($"Estado divergente: Banco='{estadoBanco}' vs ViaCEP='{ufViaCep}'");
                      }
                    }
                    else
                    {
                      inconsistencias.Add("Estado não encontrado na hierarquia");
                    }
                  }
                  else
                  {
                    inconsistencias.Add("Cidade não vinculada a um Estado");
                  }
                }
                else
                {
                  inconsistencias.Add("Cidade não encontrada na hierarquia");
                }
              }
              else
              {
                inconsistencias.Add("Bairro não vinculado a uma Cidade");
              }
            }
            else
            {
              inconsistencias.Add("Bairro não encontrado na hierarquia");
            }
          }
          else
          {
            inconsistencias.Add("CEP não vinculado a um Bairro");
          }

          return Ok(new
          {
            success = true,
            valido = inconsistencias.Count == 0,
            inconsistencias = inconsistencias,
            dados_banco = new
            {
              cep = cepExists["no_localidade"],
              bairro = bairroExists != null ? ((System.Collections.Generic.Dictionary<string, object>)bairroExists)["no_localidade"] : null,
              cidade = cidadeExists != null ? ((System.Collections.Generic.Dictionary<string, object>)cidadeExists)["no_localidade"] : null,
              estado = estadoExists != null ? ((System.Collections.Generic.Dictionary<string, object>)estadoExists)["no_localidade"] : null
            },
            dados_viacep = viaCepData,
            sugestao = inconsistencias.Count > 0 ? $"Use /corrigir-cep/{cep} para corrigir automaticamente" : null
          });
        }
      }
      catch (System.Exception ex)
      {
        return BadRequest(new { error = $"Erro ao validar: {ex.Message}" });
      }
    }
  }

  #region Models

  public class CorrigirCEPsEmLoteRequest
  {
    public System.Collections.Generic.List<string> ceps { get; set; }
  }

  #endregion
}
