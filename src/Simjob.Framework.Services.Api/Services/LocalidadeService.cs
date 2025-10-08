using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Simjob.Framework.Services.Api.Models;
using Simjob.Framework.Infra.Identity.Entities;
namespace Simjob.Framework.Services.Api.Services
{
  /// <summary>
  /// Serviço centralizado para gerenciar localidades e corrigir hierarquia de CEPs
  /// </summary>
  public static class LocalidadeService
  {
    private static readonly HttpClient _httpClient = new HttpClient();

    /// <summary>
    /// Consulta ViaCEP e corrige a hierarquia no banco de dados
    /// </summary>
    public static async Task<(bool success, InfosCEPCorrigido data, string error)> ConsultarECorrigirCEP(
        string cep,
        Source source,
        int? cd_tipo_logradouro = null)
    {
      try
      {
        if (string.IsNullOrEmpty(cep))
          return (false, null, "CEP não informado");

        // Formatar CEP
        var cepLimpo = cep.Replace(".", "").Replace("-", "").Trim();
        if (cepLimpo.Length != 8)
          return (false, null, "CEP inválido");

        var cepFormatado = cepLimpo.Insert(5, "-");

        // 1. CONSULTAR VIACEP
        var dadosViaCep = await ConsultarViaCEP(cepLimpo);
        if (!dadosViaCep.success)
          return (false, null, dadosViaCep.error);

        // 2. VERIFICAR HIERARQUIA NO BANCO
        var hierarquiaAtual = await BuscarHierarquiaAtual(cepFormatado, source);

        // 3. VALIDAR E CORRIGIR SE NECESSÁRIO
        var hierarquiaCorreta = await ValidarECorrigirHierarquia(
            dadosViaCep.data,
            hierarquiaAtual,
            cepFormatado,
            cd_tipo_logradouro,
            source);

        if (!hierarquiaCorreta.success)
          return (false, null, hierarquiaCorreta.error);

        return (true, hierarquiaCorreta.data, null);
      }
      catch (Exception ex)
      {
        return (false, null, $"Erro ao processar CEP: {ex.Message}");
      }
    }

    /// <summary>
    /// Consulta a API ViaCEP
    /// </summary>
    private static async Task<(bool success, ViaCEPResponse data, string error)> ConsultarViaCEP(string cep)
    {
      try
      {
        var url = $"https://viacep.com.br/ws/{cep}/json/";
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
          return (false, null, "Erro ao consultar ViaCEP");

        var json = await response.Content.ReadAsStringAsync();
        var dados = JsonSerializer.Deserialize<ViaCEPResponse>(json, new JsonSerializerOptions
        {
          PropertyNameCaseInsensitive = true
        });

        if (dados.erro)
          return (false, null, "CEP não encontrado na base do ViaCEP");

        return (true, dados, null);
      }
      catch (Exception ex)
      {
        return (false, null, $"Erro ao consultar ViaCEP: {ex.Message}");
      }
    }

    /// <summary>
    /// Busca a hierarquia atual do CEP no banco
    /// </summary>
    private static async Task<HierarquiaCEP> BuscarHierarquiaAtual(string cep, Source source)
    {
      var hierarquia = new HierarquiaCEP();

      var filtrosCep = new List<(string campo, object valor)> { ("dc_num_cep", cep) };
      var cepExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtrosCep);

      if (cepExists != null)
      {
        hierarquia.cep_cd_localidade = (int)cepExists["cd_localidade"];
        hierarquia.cep_nome = cepExists["no_localidade"]?.ToString();
        hierarquia.cep_cd_tipo_logradouro = cepExists["cd_tipo_logradouro"] != null ? (int?)cepExists["cd_tipo_logradouro"] : null;

        // Buscar Bairro
        if (cepExists["cd_loc_relacionada"] != null && cepExists["cd_loc_relacionada"] != DBNull.Value)
        {
          var filtroBairro = new List<(string campo, object valor)> { ("cd_localidade", cepExists["cd_loc_relacionada"].ToString()) };
          var bairroExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroBairro);

          if (bairroExists != null)
          {
            hierarquia.bairro_cd_localidade = (int)bairroExists["cd_localidade"];
            hierarquia.bairro_nome = bairroExists["no_localidade"]?.ToString();

            // Buscar Cidade
            if (bairroExists["cd_loc_relacionada"] != null && bairroExists["cd_loc_relacionada"] != DBNull.Value)
            {
              var filtroCidade = new List<(string campo, object valor)> { ("cd_localidade", bairroExists["cd_loc_relacionada"].ToString()) };
              var cidadeExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroCidade);

              if (cidadeExists != null)
              {
                hierarquia.cidade_cd_localidade = (int)cidadeExists["cd_localidade"];
                hierarquia.cidade_nome = cidadeExists["no_localidade"]?.ToString();

                // Buscar Estado
                if (cidadeExists["cd_loc_relacionada"] != null && cidadeExists["cd_loc_relacionada"] != DBNull.Value)
                {
                  var filtroEstado = new List<(string campo, object valor)> { ("cd_localidade", cidadeExists["cd_loc_relacionada"].ToString()) };
                  var estadoExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroEstado);

                  if (estadoExists != null)
                  {
                    hierarquia.estado_cd_localidade = (int)estadoExists["cd_localidade"];
                    hierarquia.estado_nome = estadoExists["no_localidade"]?.ToString();

                    // Buscar País
                    if (estadoExists["cd_loc_relacionada"] != null && estadoExists["cd_loc_relacionada"] != DBNull.Value)
                    {
                      var filtroPais = new List<(string campo, object valor)> { ("cd_localidade", estadoExists["cd_loc_relacionada"].ToString()) };
                      var paisExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroPais);

                      if (paisExists != null)
                      {
                        hierarquia.pais_cd_localidade = (int)paisExists["cd_localidade"];
                        hierarquia.pais_nome = paisExists["no_localidade"]?.ToString();
                      }
                    }
                  }
                }
              }
            }
          }
        }
      }

      return hierarquia;
    }

    /// <summary>
    /// Valida e corrige a hierarquia comparando com dados do ViaCEP
    /// </summary>
    private static async Task<(bool success, InfosCEPCorrigido data, string error)> ValidarECorrigirHierarquia(
        ViaCEPResponse viaCep,
        HierarquiaCEP hierarquiaAtual,
        string cep,
        int? cd_tipo_logradouro,
        Source source)
    {
      var correcoesRealizadas = new List<string>();

      // 1. GARANTIR PAÍS (Brasil)
      var cd_pais = await GarantirPais(source);
      if (hierarquiaAtual.pais_cd_localidade == null || hierarquiaAtual.pais_cd_localidade != cd_pais)
      {
        correcoesRealizadas.Add($"País corrigido para Brasil (cd: {cd_pais})");
      }

      // 2. GARANTIR ESTADO
      var cd_estado = await GarantirEstado(viaCep.uf, cd_pais, source);
      if (hierarquiaAtual.estado_cd_localidade == null || hierarquiaAtual.estado_nome?.ToUpper() != viaCep.uf?.ToUpper())
      {
        correcoesRealizadas.Add($"Estado corrigido: {hierarquiaAtual.estado_nome} → {viaCep.uf}");
      }

      // 3. GARANTIR CIDADE
      var cd_cidade = await GarantirCidade(viaCep.localidade, cd_estado, source);
      if (hierarquiaAtual.cidade_cd_localidade == null ||
          !hierarquiaAtual.cidade_nome?.Equals(viaCep.localidade, StringComparison.OrdinalIgnoreCase) == true)
      {
        correcoesRealizadas.Add($"Cidade corrigida: {hierarquiaAtual.cidade_nome} → {viaCep.localidade}");
      }

      // 4. GARANTIR BAIRRO
      var cd_bairro = await GarantirBairro(viaCep.bairro, cd_cidade, source);
      if (hierarquiaAtual.bairro_cd_localidade == null ||
          !hierarquiaAtual.bairro_nome?.Equals(viaCep.bairro, StringComparison.OrdinalIgnoreCase) == true)
      {
        correcoesRealizadas.Add($"Bairro corrigido: {hierarquiaAtual.bairro_nome} → {viaCep.bairro}");
      }

      // 5. GARANTIR/ATUALIZAR LOGRADOURO (CEP)
      var cd_logradouro = await GarantirLogradouro(
          viaCep.logradouro,
          cep,
          cd_bairro,
          cd_tipo_logradouro,
          hierarquiaAtual.cep_cd_localidade,
          source);

      if (hierarquiaAtual.cep_cd_localidade == null)
      {
        correcoesRealizadas.Add($"CEP {cep} criado no banco de dados");
      }
      else if (!hierarquiaAtual.cep_nome?.Equals(viaCep.logradouro, StringComparison.OrdinalIgnoreCase) == true)
      {
        correcoesRealizadas.Add($"Logradouro corrigido: {hierarquiaAtual.cep_nome} → {viaCep.logradouro}");
      }

      // Montar resposta corrigida
      var resultado = new InfosCEPCorrigido
      {
        cd_pais = cd_pais.ToString(),
        cd_estado = cd_estado.ToString(),
        cd_cidade = cd_cidade.ToString(),
        cd_bairro = cd_bairro.ToString(),
        cd_logradouro = cd_logradouro.ToString(),
        cd_tipo_logradouro = cd_tipo_logradouro?.ToString(),
        dc_num_cep = cep,
        no_pais = "Brasil",
        no_estado = viaCep.uf,
        no_cidade = viaCep.localidade,
        no_bairro = viaCep.bairro,
        no_logradouro = viaCep.logradouro,
        correcoes_realizadas = correcoesRealizadas,
        tinha_erro = correcoesRealizadas.Count > 0
      };

      return (true, resultado, null);
    }

    /// <summary>
    /// Garante que o país Brasil existe
    /// </summary>
    private static async Task<int> GarantirPais(Source source)
    {
      var filtro = new List<(string campo, object valor)>
            {
                ("no_localidade", "Brasil"),
                ("cd_tipo_localidade", 1) // Tipo 1 = País
            };

      var paisExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtro);

      if (paisExists != null)
        return (int)paisExists["cd_localidade"];

      // Criar país
      var paisDict = new Dictionary<string, object>
      {
        ["no_localidade"] = "Brasil",
        ["cd_tipo_localidade"] = 1,
        ["cd_loc_relacionada"] = DBNull.Value,
        ["id_exportado"] = 0
      };

      var insert = await SQLServerService.InsertWithResult("T_LOCALIDADE", paisDict, source);
      if (insert.success && insert.inserted != null && insert.inserted.ContainsKey("cd_localidade"))
      {
        return (int)insert.inserted["cd_localidade"];
      }

      // Se falhar, tentar buscar novamente (pode ter sido criado por outro processo)
      paisExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtro);
      if (paisExists != null)
        return (int)paisExists["cd_localidade"];

      return 0;
    }

    /// <summary>
    /// Garante que o estado existe e está vinculado ao país correto
    /// </summary>
    private static async Task<int> GarantirEstado(string uf, int cd_pais, Source source)
    {
      var filtro = new List<(string campo, object valor)>
            {
                ("no_localidade", uf),
                ("cd_tipo_localidade", 2) // Tipo 2 = Estado
            };

      var estadoExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtro);

      if (estadoExists != null)
      {
        // Verificar se está vinculado ao país correto
        if (estadoExists["cd_loc_relacionada"] == null ||
            estadoExists["cd_loc_relacionada"] == DBNull.Value ||
            (int)estadoExists["cd_loc_relacionada"] != cd_pais)
        {
          // Corrigir vínculo
          var update = new Dictionary<string, object>
          {
            ["cd_loc_relacionada"] = cd_pais
          };
          await SQLServerService.Update("T_LOCALIDADE", update, source, "cd_localidade", (int)estadoExists["cd_localidade"]);
        }
        return (int)estadoExists["cd_localidade"];
      }

      // Criar estado
      var estadoDict = new Dictionary<string, object>
      {
        ["no_localidade"] = uf,
        ["cd_tipo_localidade"] = 2,
        ["cd_loc_relacionada"] = cd_pais,
        ["id_exportado"] = 0
      };

      var insert = await SQLServerService.InsertWithResult("T_LOCALIDADE", estadoDict, source);
      if (insert.success && insert.inserted != null && insert.inserted.ContainsKey("cd_localidade"))
      {
        return (int)insert.inserted["cd_localidade"];
      }
      return 0;
    }

    /// <summary>
    /// Garante que a cidade existe e está vinculada ao estado correto
    /// </summary>
    private static async Task<int> GarantirCidade(string nomeCidade, int cd_estado, Source source)
    {
      var filtro = new List<(string campo, object valor)>
            {
                ("no_localidade", nomeCidade),
                ("cd_tipo_localidade", 3) // Tipo 3 = Cidade
            };

      var cidadeExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtro);

      if (cidadeExists != null)
      {
        // Verificar se está vinculada ao estado correto
        if (cidadeExists["cd_loc_relacionada"] == null ||
            cidadeExists["cd_loc_relacionada"] == DBNull.Value ||
            (int)cidadeExists["cd_loc_relacionada"] != cd_estado)
        {
          // Corrigir vínculo
          var update = new Dictionary<string, object>
          {
            ["cd_loc_relacionada"] = cd_estado
          };
          await SQLServerService.Update("T_LOCALIDADE", update, source, "cd_localidade", (int)cidadeExists["cd_localidade"]);
        }
        return (int)cidadeExists["cd_localidade"];
      }

      // Criar cidade
      var cidadeDict = new Dictionary<string, object>
      {
        ["no_localidade"] = nomeCidade,
        ["cd_tipo_localidade"] = 3,
        ["cd_loc_relacionada"] = cd_estado,
        ["id_exportado"] = 0
      };

      var insert = await SQLServerService.InsertWithResult("T_LOCALIDADE", cidadeDict, source);
      if (insert.success && insert.inserted != null && insert.inserted.ContainsKey("cd_localidade"))
      {
        return (int)insert.inserted["cd_localidade"];
      }
      return 0;
    }

    /// <summary>
    /// Garante que o bairro existe e está vinculado à cidade correta
    /// </summary>
    private static async Task<int> GarantirBairro(string nomeBairro, int cd_cidade, Source source)
    {
      var filtro = new List<(string campo, object valor)>
            {
                ("no_localidade", nomeBairro),
                ("cd_tipo_localidade", 4) // Tipo 4 = Bairro
            };

      var bairroExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtro);

      if (bairroExists != null)
      {
        // Verificar se está vinculado à cidade correta
        if (bairroExists["cd_loc_relacionada"] == null ||
            bairroExists["cd_loc_relacionada"] == DBNull.Value ||
            (int)bairroExists["cd_loc_relacionada"] != cd_cidade)
        {
          // Corrigir vínculo
          var update = new Dictionary<string, object>
          {
            ["cd_loc_relacionada"] = cd_cidade
          };
          await SQLServerService.Update("T_LOCALIDADE", update, source, "cd_localidade", (int)bairroExists["cd_localidade"]);
        }
        return (int)bairroExists["cd_localidade"];
      }

      // Criar bairro
      var bairroDict = new Dictionary<string, object>
      {
        ["no_localidade"] = nomeBairro,
        ["cd_tipo_localidade"] = 4,
        ["cd_loc_relacionada"] = cd_cidade,
        ["id_exportado"] = 0
      };

      var insert = await SQLServerService.InsertWithResult("T_LOCALIDADE", bairroDict, source);
      if (insert.success && insert.inserted != null && insert.inserted.ContainsKey("cd_localidade"))
      {
        return (int)insert.inserted["cd_localidade"];
      }
      return 0;
    }

    /// <summary>
    /// Garante que o logradouro/CEP existe e está vinculado ao bairro correto
    /// </summary>
    private static async Task<int> GarantirLogradouro(
        string nomeLogradouro,
        string cep,
        int cd_bairro,
        int? cd_tipo_logradouro,
        int? cd_localidade_atual,
        Source source)
    {
      var filtro = new List<(string campo, object valor)>
            {
                ("dc_num_cep", cep)
            };

      var logradouroExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtro);

      if (logradouroExists != null)
      {
        // Atualizar se necessário
        var precisaAtualizar = false;
        var update = new Dictionary<string, object>();

        if (logradouroExists["cd_loc_relacionada"] == null ||
            logradouroExists["cd_loc_relacionada"] == DBNull.Value ||
            (int)logradouroExists["cd_loc_relacionada"] != cd_bairro)
        {
          update["cd_loc_relacionada"] = cd_bairro;
          precisaAtualizar = true;
        }

        if (!logradouroExists["no_localidade"]?.ToString().Equals(nomeLogradouro, StringComparison.OrdinalIgnoreCase) == true)
        {
          update["no_localidade"] = nomeLogradouro;
          precisaAtualizar = true;
        }

        if (cd_tipo_logradouro != null &&
            (logradouroExists["cd_tipo_logradouro"] == null ||
             logradouroExists["cd_tipo_logradouro"] == DBNull.Value ||
             (int)logradouroExists["cd_tipo_logradouro"] != cd_tipo_logradouro))
        {
          update["cd_tipo_logradouro"] = cd_tipo_logradouro;
          precisaAtualizar = true;
        }

        if (precisaAtualizar)
        {
          await SQLServerService.Update("T_LOCALIDADE", update, source, "cd_localidade", (int)logradouroExists["cd_localidade"]);
        }

        return (int)logradouroExists["cd_localidade"];
      }

      // Criar logradouro
      var logradouroDict = new Dictionary<string, object>
      {
        ["no_localidade"] = nomeLogradouro,
        ["cd_tipo_localidade"] = 5, // Tipo 5 = Logradouro
        ["cd_loc_relacionada"] = cd_bairro,
        ["dc_num_cep"] = cep,
        ["cd_tipo_logradouro"] = cd_tipo_logradouro ?? (object)DBNull.Value,
        ["id_exportado"] = 0
      };

      var insert = await SQLServerService.InsertWithResult("T_LOCALIDADE", logradouroDict, source);
      if (insert.success && insert.inserted != null && insert.inserted.ContainsKey("cd_localidade"))
      {
        return (int)insert.inserted["cd_localidade"];
      }
      return 0;
    }
  }

  #region Models

  public class ViaCEPResponse
  {
    public string cep { get; set; }
    public string logradouro { get; set; }
    public string complemento { get; set; }
    public string bairro { get; set; }
    public string localidade { get; set; }
    public string uf { get; set; }
    public string ibge { get; set; }
    public string gia { get; set; }
    public string ddd { get; set; }
    public string siafi { get; set; }
    public bool erro { get; set; }
  }

  public class HierarquiaCEP
  {
    public int? pais_cd_localidade { get; set; }
    public string pais_nome { get; set; }
    public int? estado_cd_localidade { get; set; }
    public string estado_nome { get; set; }
    public int? cidade_cd_localidade { get; set; }
    public string cidade_nome { get; set; }
    public int? bairro_cd_localidade { get; set; }
    public string bairro_nome { get; set; }
    public int? cep_cd_localidade { get; set; }
    public string cep_nome { get; set; }
    public int? cep_cd_tipo_logradouro { get; set; }
  }

  public class InfosCEPCorrigido
  {
    public string cd_pais { get; set; }
    public string cd_estado { get; set; }
    public string cd_cidade { get; set; }
    public string cd_bairro { get; set; }
    public string cd_logradouro { get; set; }
    public string cd_tipo_logradouro { get; set; }
    public string dc_num_cep { get; set; }
    public string no_pais { get; set; }
    public string no_estado { get; set; }
    public string no_cidade { get; set; }
    public string no_bairro { get; set; }
    public string no_logradouro { get; set; }
    public List<string> correcoes_realizadas { get; set; }
    public bool tinha_erro { get; set; }
  }

  #endregion
}
