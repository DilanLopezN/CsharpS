using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Simjob.Framework.Application.Controllers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Simjob.Framework.Services.Api.Controllers
{
  public class CepValidacaoController : BaseController
  {
    private readonly IRepository<SourceContext, Source> _sourceRepository;
    protected readonly IMongoCollection<Source> _sourceCollection;
    protected readonly SourceContext SourceContext;

    public CepValidacaoController(
        IMediatorHandler bus,
        INotificationHandler<DomainNotification> notifications,
        IRepository<SourceContext, Source> sourceRepository,
        SourceContext sourceContext)
        : base(bus, notifications)
    {
      _sourceRepository = sourceRepository;
      SourceContext = sourceContext;
      _sourceCollection = sourceContext.GetUserCollection();
    }

    /// <summary>
    /// Corrige a hierarquia completa de um CEP consultando ViaCEP e atualizando no banco
    /// </summary>
    /// <param name="cep">CEP a ser corrigido (com ou sem formatação)</param>
    /// <returns>Resultado da correção com lista de alterações realizadas</returns>
    [Authorize]
    [HttpPost("corrigir-hierarquia/{cep}")]
    public async Task<IActionResult> CorrigirHierarquia(string cep)
    {
      try
      {
        // Buscar Source ativo
        var filterActive = Builders<Source>.Filter.Eq(u => u.Active, true);
        var filterIsDeleted = Builders<Source>.Filter.Eq(u => u.IsDeleted, false);
        var source = _sourceCollection.Find(filterIsDeleted & filterActive).FirstOrDefault();

        if (source == null)
        {
          return BadRequest(new { error = "Nenhuma fonte de dados ativa encontrada." });
        }

        // Limpar e formatar CEP
        var cepLimpo = cep.Replace(".", "").Replace("-", "").Trim();
        if (cepLimpo.Length != 8)
        {
          return BadRequest(new { error = "CEP inválido. Formato esperado: 00000000 ou 00000-000" });
        }

        var cepFormatado = cepLimpo.Insert(5, "-");

        // 1. CONSULTAR VIACEP
        var viaCepData = await ConsultarViaCEP(cepLimpo);
        if (viaCepData == null)
        {
          return NotFound(new { error = "CEP não encontrado no ViaCEP" });
        }

        // 2. BUSCAR HIERARQUIA ATUAL NO BANCO
        var hierarquiaAtual = await BuscarHierarquiaBanco(cepFormatado, source);

        // 3. CORRIGIR HIERARQUIA
        var resultado = await CorrigirHierarquiaCompleta(cepFormatado, hierarquiaAtual, viaCepData, source);

        return Ok(new
        {
          cep = cepLimpo,
          status = resultado.correcoes.Count > 0 ? "Hierarquia corrigida com sucesso" : "Hierarquia já estava correta",
          tinha_erro = resultado.correcoes.Count > 0,
          total_correcoes = resultado.correcoes.Count,
          correcoes_realizadas = resultado.correcoes,
          hierarquia_corrigida = resultado.hierarquia,
          dados_viacep = viaCepData
        });
      }
      catch (Exception ex)
      {
        return BadRequest(new { error = $"Erro ao corrigir hierarquia: {ex.Message}" });
      }
    }

    /// <summary>
    /// Valida a hierarquia completa de um CEP comparando com ViaCEP
    /// APENAS CONSULTA - NÃO FAZ CORREÇÕES NO BANCO
    /// </summary>
    /// <param name="cep">CEP a ser validado (com ou sem formatação)</param>
    /// <returns>Resultado da validação com detalhes de inconsistências</returns>
    [Authorize]
    [HttpGet("validar-hierarquia/{cep}")]
    public async Task<IActionResult> ValidarHierarquia(string cep)
    {
      try
      {
        // Buscar Source ativo
        var filterActive = Builders<Source>.Filter.Eq(u => u.Active, true);
        var filterIsDeleted = Builders<Source>.Filter.Eq(u => u.IsDeleted, false);
        var source = _sourceCollection.Find(filterIsDeleted & filterActive).FirstOrDefault();

        if (source == null)
        {
          return BadRequest(new { error = "Nenhuma fonte de dados ativa encontrada." });
        }

        // Limpar e formatar CEP
        var cepLimpo = cep.Replace(".", "").Replace("-", "").Trim();
        if (cepLimpo.Length != 8)
        {
          return BadRequest(new { error = "CEP inválido. Formato esperado: 00000000 ou 00000-000" });
        }

        var cepFormatado = cepLimpo.Insert(5, "-");

        // 1. CONSULTAR VIACEP
        var viaCepData = await ConsultarViaCEP(cepLimpo);
        if (viaCepData == null)
        {
          return NotFound(new { error = "CEP não encontrado no ViaCEP" });
        }

        // 2. BUSCAR HIERARQUIA NO BANCO SQL SERVER
        var hierarquiaBanco = await BuscarHierarquiaBanco(cepFormatado, source);

        if (hierarquiaBanco == null)
        {
          return Ok(new
          {
            cep = cepLimpo,
            status = "CEP não cadastrado no banco",
            valido = false,
            tem_erros = true,
            erros_hierarquia = new[] { "CEP não existe no banco de dados" },
            dados_viacep = viaCepData,
            sugestao = "Use o endpoint /api/Localidade/corrigir-cep para cadastrar e corrigir automaticamente"
          });
        }

        // 3. VALIDAR HIERARQUIA
        var errosHierarquia = ValidarHierarquiaCompleta(hierarquiaBanco, viaCepData);

        // 4. RETORNAR RESULTADO
        var temErros = errosHierarquia.Count > 0;

        return Ok(new
        {
          cep = cepLimpo,
          status = temErros ? "Inconsistências encontradas na hierarquia" : "Hierarquia válida",
          valido = !temErros,
          tem_erros = temErros,
          total_erros = errosHierarquia.Count,
          erros_hierarquia = errosHierarquia,
          hierarquia_banco = hierarquiaBanco,
          dados_viacep = viaCepData,
          sugestao = temErros ? $"Use /api/Localidade/corrigir-cep/{cep} para corrigir automaticamente" : null
        });
      }
      catch (Exception ex)
      {
        return BadRequest(new { error = $"Erro ao validar hierarquia: {ex.Message}" });
      }
    }

    /// <summary>
    /// Busca hierarquia completa no SQL Server usando T_LOCALIDADE
    /// </summary>
    private async Task<Dictionary<string, object>> BuscarHierarquiaBanco(string cep, Source source)
    {
      var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};";

      using (var connection = new SqlConnection(connectionString))
      {
        await connection.OpenAsync();

        var query = @"
                    SELECT
                        -- CEP
                        cep.cd_localidade AS cep_cd_localidade,
                        cep.no_localidade AS cep_no_localidade,
                        cep.dc_num_cep AS cep_dc_num_cep,
                        cep.cd_tipo_logradouro AS cep_cd_tipo_logradouro,

                        -- BAIRRO
                        bairro.cd_localidade AS bairro_cd_localidade,
                        bairro.no_localidade AS bairro_no_localidade,

                        -- CIDADE
                        cidade.cd_localidade AS cidade_cd_localidade,
                        cidade.no_localidade AS cidade_no_localidade,

                        -- ESTADO
                        estado.cd_localidade AS estado_cd_localidade,
                        estado.no_localidade AS estado_no_localidade,
                        est.sg_estado AS estado_sg_estado,

                        -- PAÍS
                        pais.cd_localidade AS pais_cd_localidade,
                        pais.no_localidade AS pais_no_localidade
                    FROM T_LOCALIDADE cep
                    LEFT JOIN T_LOCALIDADE bairro ON cep.cd_loc_relacionada = bairro.cd_localidade
                    LEFT JOIN T_LOCALIDADE cidade ON bairro.cd_loc_relacionada = cidade.cd_localidade
                    LEFT JOIN T_LOCALIDADE estado ON cidade.cd_loc_relacionada = estado.cd_localidade
                    LEFT JOIN T_ESTADO est ON estado.cd_localidade = est.cd_localidade_estado
                    LEFT JOIN T_LOCALIDADE pais ON estado.cd_loc_relacionada = pais.cd_localidade
                    WHERE cep.dc_num_cep = @cep";

        using (var command = new SqlCommand(query, connection))
        {
          command.Parameters.AddWithValue("@cep", cep);

          using (var reader = await command.ExecuteReaderAsync())
          {
            if (await reader.ReadAsync())
            {
              var resultado = new Dictionary<string, object>();

              resultado["cep"] = new
              {
                cd_localidade = reader["cep_cd_localidade"],
                no_localidade = reader["cep_no_localidade"],
                dc_num_cep = reader["cep_dc_num_cep"],
                cd_tipo_logradouro = reader["cep_cd_tipo_logradouro"] == DBNull.Value ? null : reader["cep_cd_tipo_logradouro"]
              };

              if (reader["bairro_cd_localidade"] != DBNull.Value)
              {
                resultado["bairro"] = new
                {
                  cd_localidade = reader["bairro_cd_localidade"],
                  no_localidade = reader["bairro_no_localidade"]
                };
              }

              if (reader["cidade_cd_localidade"] != DBNull.Value)
              {
                resultado["cidade"] = new
                {
                  cd_localidade = reader["cidade_cd_localidade"],
                  no_localidade = reader["cidade_no_localidade"]
                };
              }

              if (reader["estado_cd_localidade"] != DBNull.Value)
              {
                resultado["estado"] = new
                {
                  cd_localidade = reader["estado_cd_localidade"],
                  no_localidade = reader["estado_no_localidade"],
                  sg_estado = reader["estado_sg_estado"] != DBNull.Value ? reader["estado_sg_estado"] : null
                };
              }

              if (reader["pais_cd_localidade"] != DBNull.Value)
              {
                resultado["pais"] = new
                {
                  cd_localidade = reader["pais_cd_localidade"],
                  no_localidade = reader["pais_no_localidade"]
                };
              }

              return resultado;
            }
          }
        }
      }

      return null;
    }

    /// <summary>
    /// Valida hierarquia comparando banco SQL Server com ViaCEP
    /// </summary>
    private List<string> ValidarHierarquiaCompleta(Dictionary<string, object> hierarquiaBanco, Dictionary<string, object> viaCepData)
    {
      var erros = new List<string>();

      // Extrair dados do CEP
      dynamic cepBanco = hierarquiaBanco.ContainsKey("cep") ? hierarquiaBanco["cep"] : null;
      string logradouroBanco = cepBanco?.no_localidade?.ToString();
      string logradouroViaCep = viaCepData.ContainsKey("logradouro") ? viaCepData["logradouro"]?.ToString() : "";

      // VALIDAR LOGRADOURO
      if (!string.IsNullOrEmpty(logradouroViaCep) && !string.IsNullOrEmpty(logradouroBanco))
      {
        if (!NormalizarTexto(logradouroBanco).Contains(NormalizarTexto(logradouroViaCep)) &&
            !NormalizarTexto(logradouroViaCep).Contains(NormalizarTexto(logradouroBanco)))
        {
          erros.Add($"LOGRADOURO divergente: Banco='{logradouroBanco}' vs ViaCEP='{logradouroViaCep}'");
        }
      }

      // VALIDAR BAIRRO
      if (hierarquiaBanco.ContainsKey("bairro"))
      {
        dynamic bairroBanco = hierarquiaBanco["bairro"];
        string bairroNome = bairroBanco?.no_localidade?.ToString();
        string bairroViaCep = viaCepData.ContainsKey("bairro") ? viaCepData["bairro"]?.ToString() : "";

        if (!string.IsNullOrEmpty(bairroViaCep) && !string.IsNullOrEmpty(bairroNome))
        {
          if (!string.Equals(NormalizarTexto(bairroNome), NormalizarTexto(bairroViaCep), StringComparison.OrdinalIgnoreCase))
          {
            erros.Add($"BAIRRO divergente: Banco='{bairroNome}' vs ViaCEP='{bairroViaCep}'");
          }
        }
      }
      else
      {
        erros.Add("CEP não vinculado a um Bairro");
      }

      // VALIDAR CIDADE
      if (hierarquiaBanco.ContainsKey("cidade"))
      {
        dynamic cidadeBanco = hierarquiaBanco["cidade"];
        string cidadeNome = cidadeBanco?.no_localidade?.ToString();
        string cidadeViaCep = viaCepData.ContainsKey("localidade") ? viaCepData["localidade"]?.ToString() : "";

        if (!string.Equals(NormalizarTexto(cidadeNome), NormalizarTexto(cidadeViaCep), StringComparison.OrdinalIgnoreCase))
        {
          erros.Add($"CIDADE divergente: Banco='{cidadeNome}' vs ViaCEP='{cidadeViaCep}'");
        }
      }
      else
      {
        erros.Add("Bairro não vinculado a uma Cidade");
      }

      // VALIDAR ESTADO
      if (hierarquiaBanco.ContainsKey("estado"))
      {
        dynamic estadoBanco = hierarquiaBanco["estado"];
        string estadoSigla = estadoBanco?.sg_estado?.ToString();
        string ufViaCep = viaCepData.ContainsKey("uf") ? viaCepData["uf"]?.ToString() : "";

        // Compara pela SIGLA (SP, RJ, MG, etc)
        if (!string.Equals(estadoSigla, ufViaCep, StringComparison.OrdinalIgnoreCase))
        {
          erros.Add($"ESTADO/UF divergente: Banco='{estadoSigla}' vs ViaCEP='{ufViaCep}'");
        }
      }
      else
      {
        erros.Add("Cidade não vinculada a um Estado");
      }

      // VALIDAR PAÍS
      if (hierarquiaBanco.ContainsKey("pais"))
      {
        dynamic paisBanco = hierarquiaBanco["pais"];
        string paisNome = paisBanco?.no_localidade?.ToString();

        if (!string.Equals(paisNome, "Brasil", StringComparison.OrdinalIgnoreCase))
        {
          erros.Add($"PAÍS divergente: esperado 'Brasil', encontrado '{paisNome}'");
        }
      }
      else
      {
        erros.Add("Estado não vinculado a um País");
      }

      return erros;
    }

    /// <summary>
    /// Consulta a API ViaCEP
    /// </summary>
    private async Task<Dictionary<string, object>> ConsultarViaCEP(string cep)
    {
      try
      {
        using (var httpClient = new HttpClient())
        {
          var url = $"https://viacep.com.br/ws/{cep}/json/";
          var response = await httpClient.GetAsync(url);

          if (!response.IsSuccessStatusCode)
            return null;

          var json = await response.Content.ReadAsStringAsync();

          // Deserializar usando Newtonsoft.Json para evitar problema de valueKind
          var dados = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

          if (dados != null && dados.ContainsKey("erro"))
            return null;

          return dados;
        }
      }
      catch
      {
        return null;
      }
    }

    /// <summary>
    /// Normaliza texto removendo acentos e convertendo para maiúsculas
    /// </summary>
    private string NormalizarTexto(string texto)
    {
      if (string.IsNullOrWhiteSpace(texto))
        return string.Empty;

      return texto.Trim()
          .ToUpperInvariant()
          .Replace("Á", "A").Replace("À", "A").Replace("Ã", "A").Replace("Â", "A")
          .Replace("É", "E").Replace("È", "E").Replace("Ê", "E")
          .Replace("Í", "I").Replace("Ì", "I").Replace("Î", "I")
          .Replace("Ó", "O").Replace("Ò", "O").Replace("Õ", "O").Replace("Ô", "O")
          .Replace("Ú", "U").Replace("Ù", "U").Replace("Û", "U")
          .Replace("Ç", "C");
    }

    /// <summary>
    /// Corrige a hierarquia completa do CEP no banco
    /// </summary>
    private async Task<(List<string> correcoes, Dictionary<string, object> hierarquia)> CorrigirHierarquiaCompleta(
        string cep,
        Dictionary<string, object> hierarquiaAtual,
        Dictionary<string, object> viaCepData,
        Source source)
    {
      var correcoes = new List<string>();
      var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};";

      using (var connection = new SqlConnection(connectionString))
      {
        await connection.OpenAsync();
        using (var transaction = connection.BeginTransaction())
        {
          try
          {
            // 1. GARANTIR PAÍS (Brasil)
            var cd_pais = await GarantirPais(connection, transaction, correcoes);

            // 2. GARANTIR ESTADO
            var ufViaCep = viaCepData.ContainsKey("uf") ? viaCepData["uf"]?.ToString() : "";
            var cd_estado = await GarantirEstado(connection, transaction, ufViaCep, cd_pais, hierarquiaAtual, correcoes);

            // 3. GARANTIR CIDADE
            var cidadeViaCep = viaCepData.ContainsKey("localidade") ? viaCepData["localidade"]?.ToString() : "";
            var cd_cidade = await GarantirCidade(connection, transaction, cidadeViaCep, cd_estado, hierarquiaAtual, correcoes);

            // 4. GARANTIR BAIRRO
            var bairroViaCep = viaCepData.ContainsKey("bairro") ? viaCepData["bairro"]?.ToString() : "";
            var cd_bairro = await GarantirBairro(connection, transaction, bairroViaCep, cd_cidade, hierarquiaAtual, correcoes);

            // 5. GARANTIR/ATUALIZAR CEP (LOGRADOURO)
            var logradouroViaCep = viaCepData.ContainsKey("logradouro") ? viaCepData["logradouro"]?.ToString() : "";
            var cd_cep = await GarantirCep(connection, transaction, cep, logradouroViaCep, cd_bairro, hierarquiaAtual, correcoes);

            transaction.Commit();

            // Buscar hierarquia atualizada
            var hierarquiaCorrigida = await BuscarHierarquiaBanco(cep, source);

            return (correcoes, hierarquiaCorrigida);
          }
          catch (Exception ex)
          {
            transaction.Rollback();
            throw new Exception($"Erro ao corrigir hierarquia: {ex.Message}", ex);
          }
        }
      }
    }

    /// <summary>
    /// Garante que o país Brasil existe
    /// </summary>
    private async Task<int> GarantirPais(SqlConnection connection, SqlTransaction transaction, List<string> correcoes)
    {
      var query = "SELECT cd_localidade FROM T_LOCALIDADE WHERE cd_tipo_localidade = 1 AND no_localidade = 'Brasil'";

      using (var cmd = new SqlCommand(query, connection, transaction))
      {
        var result = await cmd.ExecuteScalarAsync();
        if (result != null)
        {
          return Convert.ToInt32(result);
        }
      }

      // Inserir Brasil
      var insertQuery = "INSERT INTO T_LOCALIDADE (no_localidade, cd_tipo_localidade, id_exportado) VALUES ('Brasil', 1, 0); SELECT SCOPE_IDENTITY();";
      using (var cmd = new SqlCommand(insertQuery, connection, transaction))
      {
        var cd_pais = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        correcoes.Add($"País 'Brasil' criado (cd_localidade: {cd_pais})");
        return cd_pais;
      }
    }

    /// <summary>
    /// Garante que o estado existe e está correto
    /// </summary>
    private async Task<int> GarantirEstado(SqlConnection connection, SqlTransaction transaction, string uf, int cd_pais,
        Dictionary<string, object> hierarquiaAtual, List<string> correcoes)
    {
      // Buscar estado pela sigla na T_ESTADO
      var query = @"SELECT e.cd_localidade_estado
                         FROM T_ESTADO e
                         WHERE e.sg_estado = @uf";

      using (var cmd = new SqlCommand(query, connection, transaction))
      {
        cmd.Parameters.AddWithValue("@uf", uf);
        var result = await cmd.ExecuteScalarAsync();

        if (result != null)
        {
          var cd_estado = Convert.ToInt32(result);

          // Verificar se precisa atualizar a hierarquia
          if (hierarquiaAtual != null && hierarquiaAtual.ContainsKey("estado"))
          {
            dynamic estadoAtual = hierarquiaAtual["estado"];
            int cd_estado_atual = Convert.ToInt32(estadoAtual.cd_localidade);
            string sg_atual = estadoAtual.sg_estado?.ToString();

            if (cd_estado_atual != cd_estado || !string.Equals(sg_atual, uf, StringComparison.OrdinalIgnoreCase))
            {
              correcoes.Add($"Estado corrigido: {sg_atual} → {uf}");
            }
          }

          return cd_estado;
        }
      }

      // Estado não existe - criar
      var nomeEstado = ObterNomeEstado(uf);
      var insertLocalidade = @"INSERT INTO T_LOCALIDADE (no_localidade, cd_tipo_localidade, cd_loc_relacionada, id_exportado)
                                    VALUES (@nome, 2, @cd_pais, 0); SELECT SCOPE_IDENTITY();";

      int cd_localidade;
      using (var cmd = new SqlCommand(insertLocalidade, connection, transaction))
      {
        cmd.Parameters.AddWithValue("@nome", nomeEstado);
        cmd.Parameters.AddWithValue("@cd_pais", cd_pais);
        cd_localidade = Convert.ToInt32(await cmd.ExecuteScalarAsync());
      }

      // Inserir na T_ESTADO
      var insertEstado = "INSERT INTO T_ESTADO (cd_localidade_estado, sg_estado) VALUES (@cd_localidade, @uf)";
      using (var cmd = new SqlCommand(insertEstado, connection, transaction))
      {
        cmd.Parameters.AddWithValue("@cd_localidade", cd_localidade);
        cmd.Parameters.AddWithValue("@uf", uf);
        await cmd.ExecuteNonQueryAsync();
      }

      correcoes.Add($"Estado '{nomeEstado}' ({uf}) criado (cd_localidade: {cd_localidade})");
      return cd_localidade;
    }

    /// <summary>
    /// Garante que a cidade existe e está correta
    /// </summary>
    private async Task<int> GarantirCidade(SqlConnection connection, SqlTransaction transaction, string cidade, int cd_estado,
        Dictionary<string, object> hierarquiaAtual, List<string> correcoes)
    {
      var query = @"SELECT cd_localidade FROM T_LOCALIDADE
                         WHERE cd_tipo_localidade = 3
                         AND no_localidade = @cidade
                         AND cd_loc_relacionada = @cd_estado";

      using (var cmd = new SqlCommand(query, connection, transaction))
      {
        cmd.Parameters.AddWithValue("@cidade", cidade);
        cmd.Parameters.AddWithValue("@cd_estado", cd_estado);
        var result = await cmd.ExecuteScalarAsync();

        if (result != null)
        {
          var cd_cidade = Convert.ToInt32(result);

          // Verificar se precisa atualizar
          if (hierarquiaAtual != null && hierarquiaAtual.ContainsKey("cidade"))
          {
            dynamic cidadeAtual = hierarquiaAtual["cidade"];
            string nomeAtual = cidadeAtual.no_localidade?.ToString();

            if (!string.Equals(NormalizarTexto(nomeAtual), NormalizarTexto(cidade), StringComparison.OrdinalIgnoreCase))
            {
              correcoes.Add($"Cidade corrigida: {nomeAtual} → {cidade}");
            }
          }

          return cd_cidade;
        }
      }

      // Cidade não existe - criar
      var insertQuery = @"INSERT INTO T_LOCALIDADE (no_localidade, cd_tipo_localidade, cd_loc_relacionada, id_exportado)
                               VALUES (@cidade, 3, @cd_estado, 0); SELECT SCOPE_IDENTITY();";

      using (var cmd = new SqlCommand(insertQuery, connection, transaction))
      {
        cmd.Parameters.AddWithValue("@cidade", cidade);
        cmd.Parameters.AddWithValue("@cd_estado", cd_estado);
        var cd_cidade = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        correcoes.Add($"Cidade '{cidade}' criada (cd_localidade: {cd_cidade})");
        return cd_cidade;
      }
    }

    /// <summary>
    /// Garante que o bairro existe e está correto
    /// </summary>
    private async Task<int> GarantirBairro(SqlConnection connection, SqlTransaction transaction, string bairro, int cd_cidade,
        Dictionary<string, object> hierarquiaAtual, List<string> correcoes)
    {
      if (string.IsNullOrWhiteSpace(bairro))
      {
        bairro = "Centro"; // Bairro padrão se não informado
      }

      var query = @"SELECT cd_localidade FROM T_LOCALIDADE
                         WHERE cd_tipo_localidade = 4
                         AND no_localidade = @bairro
                         AND cd_loc_relacionada = @cd_cidade";

      using (var cmd = new SqlCommand(query, connection, transaction))
      {
        cmd.Parameters.AddWithValue("@bairro", bairro);
        cmd.Parameters.AddWithValue("@cd_cidade", cd_cidade);
        var result = await cmd.ExecuteScalarAsync();

        if (result != null)
        {
          var cd_bairro = Convert.ToInt32(result);

          // Verificar se precisa atualizar
          if (hierarquiaAtual != null && hierarquiaAtual.ContainsKey("bairro"))
          {
            dynamic bairroAtual = hierarquiaAtual["bairro"];
            string nomeAtual = bairroAtual.no_localidade?.ToString();

            if (!string.Equals(NormalizarTexto(nomeAtual), NormalizarTexto(bairro), StringComparison.OrdinalIgnoreCase))
            {
              correcoes.Add($"Bairro corrigido: {nomeAtual} → {bairro}");
            }
          }

          return cd_bairro;
        }
      }

      // Bairro não existe - criar
      var insertQuery = @"INSERT INTO T_LOCALIDADE (no_localidade, cd_tipo_localidade, cd_loc_relacionada, id_exportado)
                               VALUES (@bairro, 4, @cd_cidade, 0); SELECT SCOPE_IDENTITY();";

      using (var cmd = new SqlCommand(insertQuery, connection, transaction))
      {
        cmd.Parameters.AddWithValue("@bairro", bairro);
        cmd.Parameters.AddWithValue("@cd_cidade", cd_cidade);
        var cd_bairro = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        correcoes.Add($"Bairro '{bairro}' criado (cd_localidade: {cd_bairro})");
        return cd_bairro;
      }
    }

    /// <summary>
    /// Garante que o CEP (logradouro) existe e está correto
    /// </summary>
    private async Task<int> GarantirCep(SqlConnection connection, SqlTransaction transaction, string cep, string logradouro,
        int cd_bairro, Dictionary<string, object> hierarquiaAtual, List<string> correcoes)
    {
      if (string.IsNullOrWhiteSpace(logradouro))
      {
        logradouro = $"CEP {cep}"; // Logradouro padrão
      }

      // Verificar se CEP já existe
      var query = "SELECT cd_localidade FROM T_LOCALIDADE WHERE dc_num_cep = @cep";

      using (var cmd = new SqlCommand(query, connection, transaction))
      {
        cmd.Parameters.AddWithValue("@cep", cep);
        var result = await cmd.ExecuteScalarAsync();

        if (result != null)
        {
          var cd_localidade = Convert.ToInt32(result);

          // Atualizar logradouro e bairro se necessário
          var updateQuery = @"UPDATE T_LOCALIDADE
                                       SET no_localidade = @logradouro,
                                           cd_loc_relacionada = @cd_bairro,
                                           cd_tipo_localidade = 6
                                       WHERE cd_localidade = @cd_localidade";

          using (var updateCmd = new SqlCommand(updateQuery, connection, transaction))
          {
            updateCmd.Parameters.AddWithValue("@logradouro", logradouro);
            updateCmd.Parameters.AddWithValue("@cd_bairro", cd_bairro);
            updateCmd.Parameters.AddWithValue("@cd_localidade", cd_localidade);
            await updateCmd.ExecuteNonQueryAsync();
          }

          if (hierarquiaAtual != null && hierarquiaAtual.ContainsKey("cep"))
          {
            dynamic cepAtual = hierarquiaAtual["cep"];
            string logradouroAtual = cepAtual.no_localidade?.ToString();

            if (!string.Equals(NormalizarTexto(logradouroAtual), NormalizarTexto(logradouro), StringComparison.OrdinalIgnoreCase))
            {
              correcoes.Add($"Logradouro do CEP atualizado: {logradouroAtual} → {logradouro}");
            }
            else
            {
              correcoes.Add($"Hierarquia do CEP corrigida para bairro correto");
            }
          }

          return cd_localidade;
        }
      }

      // CEP não existe - criar
      var insertQuery = @"INSERT INTO T_LOCALIDADE (no_localidade, cd_tipo_localidade, cd_loc_relacionada, dc_num_cep, id_exportado)
                               VALUES (@logradouro, 6, @cd_bairro, @cep, 0); SELECT SCOPE_IDENTITY();";

      using (var cmd = new SqlCommand(insertQuery, connection, transaction))
      {
        cmd.Parameters.AddWithValue("@logradouro", logradouro);
        cmd.Parameters.AddWithValue("@cd_bairro", cd_bairro);
        cmd.Parameters.AddWithValue("@cep", cep);
        var cd_localidade = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        correcoes.Add($"CEP '{cep}' criado com logradouro '{logradouro}' (cd_localidade: {cd_localidade})");
        return cd_localidade;
      }
    }

    /// <summary>
    /// Retorna o nome completo do estado pela sigla
    /// </summary>
    private string ObterNomeEstado(string uf)
    {
      var estados = new Dictionary<string, string>
            {
                { "AC", "Acre" }, { "AL", "Alagoas" }, { "AP", "Amapá" }, { "AM", "Amazonas" },
                { "BA", "Bahia" }, { "CE", "Ceará" }, { "DF", "Distrito Federal" }, { "ES", "Espírito Santo" },
                { "GO", "Goiás" }, { "MA", "Maranhão" }, { "MT", "Mato Grosso" }, { "MS", "Mato Grosso do Sul" },
                { "MG", "Minas Gerais" }, { "PA", "Pará" }, { "PB", "Paraíba" }, { "PR", "Paraná" },
                { "PE", "Pernambuco" }, { "PI", "Piauí" }, { "RJ", "Rio de Janeiro" }, { "RN", "Rio Grande do Norte" },
                { "RS", "Rio Grande do Sul" }, { "RO", "Rondônia" }, { "RR", "Roraima" }, { "SC", "Santa Catarina" },
                { "SP", "São Paulo" }, { "SE", "Sergipe" }, { "TO", "Tocantins" }
            };

      return estados.ContainsKey(uf.ToUpper()) ? estados[uf.ToUpper()] : uf;
    }
  }
}
