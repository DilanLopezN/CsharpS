using Amazon.Runtime.Internal.Transform;
using Azure.Core;
using DotLiquid.Tags;
using DotLiquid.Util;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Core.Configuration;
using Newtonsoft.Json;
using Simjob.Framework.Application.Controllers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Core.Utils;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Infra.Schemas.Entities;
using Simjob.Framework.Services.Api.Enums;
using Simjob.Framework.Services.Api.Models.Matricula;
using Simjob.Framework.Services.Api.Modules.TurmaModule.Services;
using Simjob.Framework.Services.Api.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xceed.Words.NET;

namespace Simjob.Framework.Services.Api.Controllers
{
  public class MatriculaController : BaseController
  {
    private readonly IRepository<SourceContext, Source> _sourceRepository;
    private readonly IRepository<MongoDbContext, Schema> _schemaRepository;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly SimulacaoBaixaService _simulacaoBaixaService;
    private readonly ILogger<MatriculaController> _logger;
    private readonly MatriculaService _matriculaService;
    private readonly IUserService _userService;
    private readonly IGroupService _groupService;

    public MatriculaController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, IRepository<SourceContext, Source> sourceRepository, IRepository<MongoDbContext, Schema> schemaRepository, IWebHostEnvironment webHostEnvironment, MatriculaService matriculaService, IUserService userService, IGroupService groupService, ILogger<MatriculaController> logger) : base(bus, notifications)
    {
      _sourceRepository = sourceRepository;
      _schemaRepository = schemaRepository;
      _webHostEnvironment = webHostEnvironment;
      _simulacaoBaixaService = new SimulacaoBaixaService();
      _logger = logger;
      _matriculaService = matriculaService;
      _userService = userService;
      _groupService = groupService;
    }

    [Authorize]
    [HttpGet()]
    public async Task<IActionResult> GetAll(string value, SearchModeEnum mode, int? page, int? limit, string sortField, bool sortDesc = false, string ids = "", string searchFields = null, string? cd_empresa = null, DateTime? dataInicio = null, DateTime? dataMatriculaInicio = null, DateTime? dataMatriculaFim = null)
    {
      if (cd_empresa == null) return BadRequest("campo cd_empresa não informado");
      var schemaName = "T_Pessoa";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);
      if (source != null && source.Active != null && source.Active == true)
      {
        var matriculaResult = await SQLServerService.GetListFiltroData("vi_contrato", page, limit, sortField, sortDesc, ids, "cd_contrato", searchFields, value, source, mode, "cd_pessoa_escola", cd_empresa, "dt_inicial_contrato", "dt_inicial_contrato", dataInicio, dataInicio, "dt_matricula_contrato", dataMatriculaInicio, dataMatriculaFim);
        if (matriculaResult.success)
        {
          var matriculas = matriculaResult.data;

          var retorno = new
          {
            data = matriculas,
            matriculaResult.total,
            page,
            limit,
            pages = limit != null ? (int)Math.Ceiling((double)matriculaResult.total / limit.Value) : 0
          };

          return ResponseDefault(retorno);
        }
        return BadRequest(new
        {
          sucess = false,
          error = matriculaResult.error
        });
      }
      return BadRequest(new
      {
        error = "Fonte de dados não configurada ou inativa."
      });
    }

    [Authorize]
    [HttpGet()]
    [Route("aditamento")]
    public async Task<IActionResult> GetAllAditamento(string value, SearchModeEnum mode, int? page, int? limit, string sortField, bool sortDesc = false, string ids = "", string searchFields = null, string? cd_empresa = null, DateTime? dataInicio = null, DateTime? dataFim = null)
    {
      if (cd_empresa == null) return BadRequest("campo cd_empresa não informado");
      var schemaName = "T_Pessoa";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);
      if (source != null && source.Active != null && source.Active == true)
      {
        var aditamentoResult = await SQLServerService.GetListFiltroData("v_aditamento", page, limit, sortField, sortDesc, ids, null, searchFields, value, source, mode, "cd_pessoa_escola", cd_empresa, "dt_aditamento", "dt_aditamento", dataInicio, dataFim, null, null, null);
        if (aditamentoResult.success)
        {
          var matriculas = aditamentoResult.data;

          var retorno = new
          {
            data = matriculas,
            aditamentoResult.total,
            page,
            limit,
            pages = limit != null ? (int)Math.Ceiling((double)aditamentoResult.total / limit.Value) : 0
          };

          return ResponseDefault(retorno);
        }

      }
      return BadRequest(new
      {
        error = "Fonte de dados não configurada ou inativa."
      });
    }

    /// <summary>
    /// Obtém o histórico de aditamentos.
    /// </summary>
    /// <returns>
    /// Uma lista contendo o histórico de aditamentos.
    /// </returns>
    /// <response code="200">Retorna a lista de aditamentos com sucesso.</response>
    [Authorize]
    [HttpGet()]
    [Route("aditamento-historico")]
    public async Task<IActionResult> GetAllAditamentoHistorico(string value, SearchModeEnum mode, int? page, int? limit, string sortField, bool sortDesc = false, string ids = "", string searchFields = null, string? cd_empresa = null, DateTime? dataInicio = null, DateTime? dataFim = null)
    {
      if (cd_empresa == null) return BadRequest("campo cd_empresa não informado");
      var schemaName = "T_Pessoa";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);
      if (source != null && source.Active != null && source.Active == true)
      {
        var matriculaResult = await SQLServerService.GetListFiltroData("v_aditamento_historico", page, limit, sortField, sortDesc, ids, null, searchFields, value, source, mode, "cd_pessoa_escola", cd_empresa, "dt_aditamento_historico", "dt_aditamento_historico", dataInicio, dataFim, null, null, null);
        if (matriculaResult.success)
        {
          var matriculas = matriculaResult.data;

          var retorno = new
          {
            data = matriculas,
            matriculaResult.total,
            page,
            limit,
            pages = limit != null ? (int)Math.Ceiling((double)matriculaResult.total / limit.Value) : 0
          };

          return ResponseDefault(retorno);
        }

      }
      return BadRequest(new
      {
        error = "Fonte de dados não configurada ou inativa."
      });
    }



    [Authorize]
    [HttpGet()]
    [Route("{cd_contrato}")]
    public async Task<IActionResult> GetById(int cd_contrato)
    {
      var schemaName = "T_Pessoa";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);
      if (source != null && source.Active != null && source.Active == true)
      {
        var filtrosContrato = new List<(string campo, object valor)> { new("cd_contrato", cd_contrato) };
        var matriculaExists = await SQLServerService.GetFirstByFields(source, "vi_contrato_id", filtrosContrato);
        if (matriculaExists == null) return NotFound("contrato");

        var gridTurma_result = await SQLServerService.GetList("vi_contrato_grid_turma", null, "[cd_contrato]", $"[{cd_contrato}]", source, SearchModeEnum.Equals);
        var gridTurma = gridTurma_result.data;

        // MODIFICADO: Buscar todos os descontos do contrato (incluindo múltiplos descontos por aditamento)
        var gridDesconto_result = await SQLServerService.GetList("T_DESCONTO_CONTRATO", null, "[cd_contrato]", $"[{cd_contrato}]", source, SearchModeEnum.Equals);
        List<Dictionary<string, object>>? gridDesconto = null;
        if (gridDesconto_result.success)
        {
          gridDesconto = gridDesconto_result.data;
        }

        var gridCheque_result = await SQLServerService.GetList("T_CHEQUE", null, "[cd_contrato]", $"[{cd_contrato}]", source, SearchModeEnum.Equals);
        List<Dictionary<string, object>>? gridCheque = null;
        if (gridCheque_result.success)
        {
          gridCheque = gridCheque_result.data;
        }

        var gridTaxa_result = await SQLServerService.GetList("V_TAXA_MATRICULA", null, "[cd_contrato]", $"[{cd_contrato}]", source, SearchModeEnum.Equals);
        List<Dictionary<string, object>>? gridTaxa = null;
        if (gridTaxa_result.success)
        {
          gridTaxa = gridTaxa_result.data;
        }

        // Obter cd_pessoa_escola para filtrar títulos da escola correta (sistema multi-tenant)
        var cd_pessoa_escola = matriculaExists["cd_pessoa_escola"];

        // Buscar títulos do contrato filtrando por escola
        var titulos_result = await SQLServerService.GetList("vi_contrato_titulos", null, "[cd_origem],[cd_pessoa_escola]", $"[{cd_contrato}],[{cd_pessoa_escola}]", source, SearchModeEnum.Equals);

        // Buscar também títulos dos aditamentos
        var aditamentos_temp = await SQLServerService.GetList("T_ADITAMENTO", null, "[cd_contrato]", $"[{cd_contrato}]", source, SearchModeEnum.Equals);
        if (aditamentos_temp.success && aditamentos_temp.data != null && aditamentos_temp.data.Any())
        {
          var cd_aditamentos_list = string.Join(",", aditamentos_temp.data.Select(x => x["cd_aditamento"]));

          // Buscar os títulos vinculados aos aditamentos através da tabela de relacionamento
          var titulo_aditamento_result = await SQLServerService.GetList("T_TITULO_ADITAMENTO", cd_aditamentos_list, "cd_aditamento", null, source);

          if (titulo_aditamento_result.success && titulo_aditamento_result.data != null && titulo_aditamento_result.data.Any())
          {
            // Obter os IDs dos títulos vinculados aos aditamentos
            var cd_titulos_list = string.Join(",", titulo_aditamento_result.data.Select(x => x["cd_titulo"]));

            // Buscar os títulos usando os IDs corretos
            var titulos_dos_aditamentos = await SQLServerService.GetList("vi_contrato_titulos", cd_titulos_list, "cd_titulo", null, source);

            if (titulos_dos_aditamentos.success && titulos_dos_aditamentos.data != null && titulos_dos_aditamentos.data.Any())
            {
              // Adicionar títulos dos aditamentos à lista de títulos do contrato
              if (titulos_result.success && titulos_result.data != null)
              {
                // Deduplica títulos usando cd_titulo como chave única
                var existingCdTitulos = titulos_result.data
                  .Select(t => t["cd_titulo"]?.ToString())
                  .ToHashSet();

                var novosTitulos = titulos_dos_aditamentos.data
                  .Where(t => !existingCdTitulos.Contains(t["cd_titulo"]?.ToString()))
                  .ToList();

                titulos_result.data.AddRange(novosTitulos);
              }
              else
              {
                titulos_result = titulos_dos_aditamentos;
              }
            }
          }
        }

        var baixas_result = await SQLServerService.GetList("vi_contrato_titulos_baixas", null, "[cd_contrato]", $"[{cd_contrato}]", source, SearchModeEnum.Equals);
        List<Dictionary<string, object>>? gridTituloTaxa = null;
        List<Dictionary<string, object>>? gridTituloMensalidade = null;
        List<Dictionary<string, object>>? gridTituloMaterial = null;
        if (titulos_result.success)
        {
          //taxa
          var dc_tipos_taxas = new List<string> { "TX", "TM", "TA" };
          var titulosTaxa = titulos_result.data.Where(x => dc_tipos_taxas.Contains(x["dc_tipo_titulo"]?.ToString() ?? "")).ToList();
          if (baixas_result.success)
          {
            foreach (var titulo in titulosTaxa)
            {
              var baixas = baixas_result.data.Where(x => (x["cd_titulo"].ToString() ?? "") == (titulo["cd_titulo"]?.ToString() ?? "")).ToList();
              titulo.Add("gridBaixa", baixas);
            }
          }
          gridTituloTaxa = titulosTaxa;

          //mensalidade
          var dc_tipos_mensalidade = new List<string> { "ME", "MM", "MA" };
          var titulosMensalidade = titulos_result.data.Where(x => dc_tipos_mensalidade.Contains(x["dc_tipo_titulo"]?.ToString() ?? "")).ToList();
          if (baixas_result.success)
          {
            foreach (var titulo in titulosMensalidade)
            {
              var baixas = baixas_result.data.Where(x => (x["cd_titulo"].ToString() ?? "") == (titulo["cd_titulo"]?.ToString() ?? ""));
              titulo.Add("gridBaixa", baixas);
            }
          }
          gridTituloMensalidade = titulosMensalidade;

          //material
          var dc_tipos_material = new List<string> { "AD", "AA", "MT" };
          var titulosMaterial = titulos_result.data.Where(x => dc_tipos_material.Contains(x["dc_tipo_titulo"]?.ToString() ?? "")).ToList();
          if (baixas_result.success)
          {
            foreach (var titulo in titulosMaterial)
            {
              var baixas = baixas_result.data.Where(x => (x["cd_titulo"].ToString() ?? "") == (titulo["cd_titulo"]?.ToString() ?? ""));
              titulo.Add("gridBaixa", baixas);
            }
          }
          gridTituloMaterial = titulosMaterial;
        }

        //curso
        var cursoContrato_result = await SQLServerService.GetList("vi_contrato_curso", null, "[cd_contrato]", $"[{cd_contrato}]", source, SearchModeEnum.Equals);
        List<Dictionary<string, object>>? cursoContrato = null;
        if (cursoContrato_result.success)
        {
          cursoContrato = cursoContrato_result.data;
        }

        //aditamento
        var aditamentos_result = await SQLServerService.GetList("T_ADITAMENTO", null, "[cd_contrato]", $"[{cd_contrato}]", source, SearchModeEnum.Equals);
        List<Dictionary<string, object>>? aditamentos = null;
        //if (aditamentos_result.success)
        //{
        //  aditamentos = aditamentos_result.data.Where(x => x["id_status_renegociacao"] != null).ToList();
        //}
        aditamentos = aditamentos_result.data.ToList();
        var ultimo_aditamento = aditamentos.Count > 0 ? aditamentos.OrderByDescending(x => x["cd_aditamento"]).First() : null;
        var cd_aditamentos = aditamentos.Select(x => x["cd_aditamento"]);
        List<Dictionary<string, object>>? titulos_aditamentos = null;
        var titulos_aditamento_result = await SQLServerService.GetList("T_TITULO_ADITAMENTO", string.Join(",", cd_aditamentos), "cd_aditamento", null, source);
        if (titulos_aditamento_result.success)
        {
          titulos_aditamentos = titulos_aditamento_result.data;
        }
        var cd_aluno = matriculaExists["cd_aluno"];
        var aluno = await SQLServerService.GetFirstByFields(source, "T_ALUNO", new List<(string campo, object valor)> { new("cd_aluno", matriculaExists["cd_aluno"]) });
        var cd_pessoa_aluno = aluno["cd_pessoa_aluno"];
        var movimento = await SQLServerService.GetFirstByFields(source, "T_MOVIMENTO", new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa_aluno) });

        var titulosComBaixa = await SQLServerService.GetFirstByFields(source, "T_TITULO", new List<(string campo, object valor)> { ("cd_origem_titulo", cd_contrato), ("id_status_titulo", 2) });
        var titulosComCnab = await SQLServerService.GetFirstByFields(source, "T_TITULO", new List<(string campo, object valor)> { ("cd_origem_titulo", cd_contrato), ("id_status_cnab", 2) });

        var cd_cursos = string.Join(",", cursoContrato.Select(x => x["cd_curso"]));
        //v_movimento_aluno_curso
        var item_movimento = await SQLServerService.GetList("v_movimento_aluno_curso", null, "[cd_aluno],[cd_curso]", $"[{cd_aluno}],[{cd_cursos}]", source, SearchModeEnum.Equals);

        var titulos = new List<Dictionary<string, object>>();

        //adiciona titulos do aditamento
        foreach (var ad in aditamentos)
        {
          var titulos_ad = titulos_aditamentos.Where(x => x["cd_aditamento"].ToString() == ad["cd_aditamento"].ToString()).ToList();
          ad.Add("titulos", titulos_ad.Select(x => x["cd_titulo"]));
        }
        //adicionar e compor outros objetos
        var contrato = new Dictionary<string, object>
        {
          ["cd_contrato"] = matriculaExists["cd_contrato"],
          ["cd_pessoa_escola"] = matriculaExists["cd_pessoa_escola"],
          ["cd_aluno"] = matriculaExists["cd_aluno"],
          ["no_aluno"] = matriculaExists["no_aluno"],
          ["id_tipo_matricula"] = matriculaExists["id_tipo_matricula"],
          ["dt_matricula_contrato"] = matriculaExists["dt_matricula_contrato"],
          ["dt_inicial_contrato"] = matriculaExists["dt_inicial_contrato"],
          ["dt_final_contrato"] = matriculaExists["dt_final_contrato"],
          ["nm_contrato"] = matriculaExists["nm_contrato"],
          ["nm_matricula_contrato"] = matriculaExists["nm_matricula_contrato"],
          ["cd_ano_escolar"] = matriculaExists["cd_ano_escolar"],
          ["id_tipo_contrato"] = matriculaExists["id_tipo_contrato"],
          ["cd_usuario"] = matriculaExists["cd_usuario"],
          ["id_transferencia"] = matriculaExists["id_transferencia"],
          ["id_retorno"] = matriculaExists["id_retorno"],
          ["id_contrato_aula"] = matriculaExists["id_contrato_aula"],
          ["id_divida_primeira_parcela"] = matriculaExists["id_divida_primeira_parcela"],
          ["id_ajuste_manual"] = matriculaExists["id_ajuste_manual"],
          ["id_nf_servico"] = matriculaExists["id_nf_servico"],
          ["cd_produto_atual"] = matriculaExists["cd_produto_atual"],
          ["cd_curso_atual"] = matriculaExists["cd_curso_atual"],
          ["cd_regime_atual"] = matriculaExists["cd_regime_atual"],
          ["cd_duracao_atual"] = matriculaExists["cd_duracao_atual"],
          ["vl_curso_contrato"] = matriculaExists["vl_curso_contrato"],
          ["nm_parcelas_mensalidade"] = matriculaExists["nm_parcelas_mensalidade"],
          ["vl_parcela_contrato"] = matriculaExists["vl_parcela_contrato"],
          ["dt_vencimento_parcela_1"] = matriculaExists["dt_vencimento_parcela_1"],
          ["dt_vencimento_parcela_1_material"] = matriculaExists["dt_vencimento_parcela_1_material"],
          ["cd_tipo_financeiro"] = matriculaExists["cd_tipo_financeiro"],
          ["pc_desconto_bolsa"] = matriculaExists["pc_desconto_bolsa"],
          ["pc_desconto_contrato"] = matriculaExists["pc_desconto_contrato"],
          ["vl_parcela_liquida"] = matriculaExists["vl_parcela_liquida"],
          ["vl_liquido_contrato"] = matriculaExists["vl_liquido_contrato"],
          ["tx_obs_contrato"] = matriculaExists["tx_obs_contrato"],
          ["cd_nome_contrato"] = matriculaExists["cd_nome_contrato"],
          ["cd_pessoa_responsavel"] = matriculaExists["cd_pessoa_responsavel"],
          ["cd_pessoa_aluno"] = matriculaExists["cd_pessoa_aluno"],
          ["pc_responsavel_contrato"] = matriculaExists["pc_responsavel_contrato"],
          ["vl_matricula_contrato"] = matriculaExists["vl_matricula_contrato"],
          ["no_pessoa_responsavel"] = matriculaExists["no_pessoa_responsavel"],

          ["cd_plano_conta_mat"] = matriculaExists["cd_plano_conta_mat"],
          ["no_subgrupo_conta_mat"] = matriculaExists["no_subgrupo_conta_mat"],
          ["nm_mes_curso_inicial"] = matriculaExists["nm_mes_curso_inicial"],
          ["nm_ano_curso_inicial"] = matriculaExists["nm_ano_curso_inicial"],
          ["nm_mes_curso_final"] = matriculaExists["nm_mes_curso_final"],
          ["nm_ano_curso_final"] = matriculaExists["nm_ano_curso_final"],
          ["opcao_venda"] = matriculaExists["opcao_venda"],
          ["nm_parcelas_material"] = matriculaExists["nm_parcelas_material"],
          ["vl_parcela_material"] = matriculaExists["vl_parcela_material"],
          ["vl_material_contrato"] = matriculaExists["vl_material_contrato"],
          ["vl_parcela_liq_material"] = matriculaExists["vl_parcela_liq_material"],
          ["pc_bolsa_material"] = matriculaExists["pc_bolsa_material"],
          ["pc_desconto_material"] = matriculaExists["pc_desconto_material"],
          ["cd_pessoa_responsavel_material"] = matriculaExists["cd_pessoa_responsavel_material"],
          ["pc_responsavel_material"] = matriculaExists["pc_responsavel_material"],
          ["no_pessoa_responsavel_material"] = matriculaExists["no_pessoa_responsavel_material"],
          ["cd_tipo_financeiro_material"] = matriculaExists["cd_tipo_financeiro_material"],
          ["vl_liquido_material"] = matriculaExists["vl_liquido_material"],
          ["vl_desconto_material"] = matriculaExists["vl_desconto_material"],
          ["id_tipo_data_inicio"] = matriculaExists["id_tipo_data_inicio"],
          ["dt_inicio_aditamento"] = matriculaExists["dt_inicio_aditamento"],
          ["nm_dia_vcto_desconto"] = matriculaExists["nm_dia_vcto_desconto"],
          ["nm_previsao_inicial"] = matriculaExists["nm_previsao_inicial"],
          ["vl_aula_hora"] = matriculaExists["vl_aula_hora"],
          ["nm_arquivo_digitalizado"] = matriculaExists["nm_arquivo_digitalizado"],
          ["no_tipo_financeiro"] = matriculaExists["no_tipo_financeiro"],
          ["no_tipo_financeiro_material"] = matriculaExists["no_tipo_financeiro_material"],
          ["no_curso_atual"] = matriculaExists["no_curso_atual"],
          ["id_status_contrato"] = matriculaExists["id_status_contrato"],
          ["cd_fila_matricula"] = matriculaExists["cd_fila_matricula"],
          ["gridTurma"] = gridTurma,
          ["gridDesconto"] = gridDesconto,
          ["cheque"] = gridDesconto,
          ["gridTaxa"] = gridTaxa,
          ["gridTituloTaxa"] = gridTituloTaxa,
          ["gridTituloMensalidade"] = gridTituloMensalidade,
          ["gridTituloMaterial"] = gridTituloMaterial,
          ["cursoContrato"] = cursoContrato,
          ["aditamentos"] = aditamentos,
          ["possui_material"] = movimento == null ? false : true,

          ["possui_titulo_baixado"] = titulosComBaixa == null ? false : true,
          ["possui_cnab"] = titulosComCnab == null ? false : true,
          ["item_movimento"] = item_movimento.data,

          ["cd_nome_contrato"] = ultimo_aditamento?["cd_nome_contrato"],
          ["dt_inicio_aditamento"] = ultimo_aditamento?["dt_inicio_aditamento"],
          ["id_tipo_data_inicio"] = ultimo_aditamento?["id_tipo_data_inicio"],
          ["nm_previsao_inicial"] = ultimo_aditamento?["nm_previsao_inicial"],
          ["nm_dia_vcto_desconto"] = ultimo_aditamento?["nm_dia_vcto_desconto"]
        };


        return ResponseDefault(contrato);
      }
      return BadRequest(new
      {
        error = "Fonte de dados não configurada ou inativa."
      });
    }

    [Authorize]
    [HttpGet()]
    [Route("aditamento/{cd_aditamento}")]
    public async Task<IActionResult> GetAditamentoId(int cd_aditamento)
    {
      var schemaName = "T_Pessoa";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);
      if (source != null && source.Active != null && source.Active == true)
      {
        var aditamentoExists = await SQLServerService.GetFirstByFields(source, "T_ADITAMENTO", new List<(string campo, object valor)> { new("cd_aditamento", cd_aditamento) });
        if (aditamentoExists == null) return NotFound("aditamento");

        var t_aditamento_bolsa = await SQLServerService.GetFirstByFields(source, "T_ADITAMENTO_BOLSA", new List<(string campo, object valor)> { new("cd_aditamento", cd_aditamento) });
        aditamentoExists.Add("bolsa", t_aditamento_bolsa);

        // MODIFICADO: Buscar TODOS os descontos do aditamento (suporte a múltiplos descontos)
        var t_descontos_result = await SQLServerService.GetList("T_DESCONTO_CONTRATO", null, "[cd_aditamento]", $"[{cd_aditamento}]", source, SearchModeEnum.Equals);

        // Se houver múltiplos descontos, retornar array; senão, manter retrocompatibilidade
        if (t_descontos_result.success && t_descontos_result.data != null && t_descontos_result.data.Any())
        {
          if (t_descontos_result.data.Count > 1)
          {
            // Múltiplos descontos: retornar array
            aditamentoExists.Add("descontos", t_descontos_result.data);
            // Manter campo "desconto" com o primeiro para retrocompatibilidade
            aditamentoExists.Add("desconto", t_descontos_result.data.First());
          }
          else
          {
            // Desconto único: manter formato legado
            aditamentoExists.Add("desconto", t_descontos_result.data.First());
            aditamentoExists.Add("descontos", t_descontos_result.data);
          }
        }
        else
        {
          // Sem descontos
          aditamentoExists.Add("desconto", null);
          aditamentoExists.Add("descontos", new List<Dictionary<string, object>>());
        }

        var t_contrato = await SQLServerService.GetFirstByFields(source, "vi_contrato", new List<(string campo, object valor)> { new("cd_contrato", aditamentoExists["cd_contrato"]) });
        aditamentoExists.Add("cd_aluno", t_contrato["cd_aluno"]);
        aditamentoExists.Add("no_aluno", t_contrato["no_pessoa"]);
        return ResponseDefault(aditamentoExists);

      }
      return BadRequest(new
      {
        error = "Fonte de dados não configurada ou inativa."
      });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Post(MatriculaInputModel model)
    {
      if (!ValidadorHelper.ValidarCamposCd(model, out var erros))
      {
        var sb = new StringBuilder();
        sb.Append("Campos inválidos: ");
        sb.Append(string.Join(", ", erros));
        return BadRequest(sb.ToString());
      }
      var schemaName = "T_Pessoa";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);
      if (source != null && source.Active != null && source.Active == true)
      {


        //validações iniciais
        if (model.cd_pessoa_escola == null) return BadRequest("Escola não informada");

        if (model.id_tipo_contrato == 2 && !model.Turmas.Any()) return BadRequest("Tipo de contrato preenchido como B2C então Turma é obrigatorio");
        if (model.id_tipo_contrato == 2 && model.Turmas.Count() > 1) return BadRequest("Tipo de contrato preenchido como B2C então não permite que seja uma matrícula múltipla");

        if (model.dt_vencimento_parcela_1 == null) return BadRequest("Data de vencimento da primeira parcela deve ser informada");

        if (model.opcao_venda == "3" && model.dt_vencimento_parcela_1_material == null) return BadRequest("Data de vencimento da primeira parcela de material deve ser informada");

        if (model.cd_aluno == null) return BadRequest("aluno não informado");
        var cd_alunoExists = await SQLServerService.GetFirstByFields(source, "T_ALUNO", new List<(string campo, object valor)> { new("cd_aluno", model.cd_aluno) });
        if (cd_alunoExists == null) return NotFound("aluno não encontrado!");
        var cd_pessoa_aluno = cd_alunoExists["cd_pessoa_aluno"];


        //validações parametros

        var filtroParametro = new List<(string campo, object valor)> { new("cd_pessoa_escola", model.cd_pessoa_escola) };
        var parametroExists = await SQLServerService.GetFirstByFields(source, "T_PARAMETRO", filtroParametro);
        if (parametroExists == null) return NotFound("parametros não encontratos para esta escola");
        var nm_nf_mercantil = (bool)parametroExists["id_emitir_nf_mercantil"] == true ? int.Parse(parametroExists["nm_nf_mercantil"].ToString()) : int.Parse(parametroExists["nm_nf_material"].ToString());
        var id_nro_contrato_automatico = (bool)parametroExists["id_nro_contrato_automatico"];
        var id_tipo_numero_contrato = parametroExists["id_tipo_numero_contrato"]?.ToString() ?? "0";

        if (model.nm_matricula_contrato > 0 && id_tipo_numero_contrato == "2") return BadRequest("Numeração das matriculas está programada para ser automática, portanto não  deve ser informada(nm_matricula_contrato)");

        var responsavel = model.cd_pessoa_responsavel;
        if (string.IsNullOrEmpty(responsavel))
        {
          responsavel = cd_pessoa_aluno?.ToString() ?? "0";
        }

        // validação e cadastro de relacionamento

        // ===== VALIDAÇÕES DE MATRÍCULA DUPLICADA =====
        // Implementando a lógica do sgf1-prod para evitar matrículas duplicadas no mesmo período/produto
// ===== VALIDAÇÃO DE VALOR MATERIAL INCLUSO =====
        if (model.CursoContrato != null && model.CursoContrato.Any())
{
  foreach (var curso in model.CursoContrato)
  {
    if (curso.id_valor_incluso && curso.vl_material_curso.HasValue && curso.vl_material_curso.Value > 0)
    {
      var valorContrato = curso.vl_curso_total;
      var valorMaterial = curso.vl_material_curso.Value;
      
      if (valorMaterial > valorContrato)
      {
        return BadRequest($"Valor do material (R$ {valorMaterial:N2}) do tipo 'Incluso' não pode exceder o valor do contrato (R$ {valorContrato:N2}) para o curso {curso.cd_curso}");
      }
    }
  }
}
        try
        {
          await ValidarMatriculaDuplicada(model, source);
        }
        catch (Exception e)
        {
          return BadRequest(e.Message);
        }

        //obtem e atualizar ultimo nm_contrato e matricula
        var nm_contrato_p = parametroExists["nm_ultimo_contrato"] != null ? parametroExists["nm_ultimo_contrato"].ToString() : "0";
        var nm_matricula_p = parametroExists["nm_ultimo_matricula"] != null ? parametroExists["nm_ultimo_matricula"].ToString() : "0";
        var cd_plano_conta_mat = parametroExists["cd_plano_conta_mat"] != null ? parametroExists["cd_plano_conta_mat"].ToString() : "0";
        var cd_plano_conta_tax = parametroExists["cd_plano_conta_tax"] != null ? parametroExists["cd_plano_conta_tax"].ToString() : "0";
        var cd_plano_conta_mtr = parametroExists["cd_plano_conta_material"] != null ? parametroExists["cd_plano_conta_material"].ToString() : "0";

        var nm_contrato = model.nm_contrato;
        var nm_matricula = model.nm_matricula_contrato;
        if (id_nro_contrato_automatico) nm_contrato = int.Parse(nm_contrato_p) + 1;
        if (id_tipo_numero_contrato == "1") nm_matricula = nm_contrato;
        else if (id_tipo_numero_contrato == "2") nm_matricula = int.Parse(nm_matricula_p) + 1;

        var filtroContrato = new List<(string campo, object valor)> { new("cd_pessoa_escola", model.cd_pessoa_escola), new("nm_contrato", nm_contrato) };
        var contratoExists = await SQLServerService.GetFirstByFields(source, "T_CONTRATO", filtroContrato);
        if (contratoExists != null && int.Parse(contratoExists["nm_contrato"].ToString()) > 0) return BadRequest("Contrato com este número já cadastrado para esta escola");

        if (nm_contrato != model.nm_contrato || nm_matricula != model.nm_matricula_contrato)
        {
          var parametroUpdate = new Dictionary<string, object>
                    {
                        { "nm_ultimo_contrato", nm_contrato },
                        { "nm_ultimo_matricula", nm_matricula }
                    };
          var parametroResult = await SQLServerService.Update("T_PARAMETRO", parametroUpdate, source, "cd_pessoa_escola", model.cd_pessoa_escola);
          if (!parametroResult.success) return BadRequest(parametroResult.error);
        }

        var dataVencimento = model.dt_vencimento_parcela_1 ?? null;

        var cd_usuario = model.cd_usuario;
        if (cd_usuario == null)
        {
          var accessToken = Request.Headers[HeaderNames.Authorization];
          var tokenInfo = Util.GetUserInfoFromToken(accessToken);
          var user = SQLServerService.GetFirstByFields(source, "T_SYS_USUARIO", new List<(string campo, object valor)> { ("cd_pessoa", tokenInfo["cd_pessoa"]) }).Result;
          if (user == null)
            return BadRequest("Usuário logado não encontrado");
          cd_usuario = user["cd_usuario"].ToString();
        }

        var matricula_dict = new Dictionary<string, object>
        {
          ["cd_aluno"] = model.cd_aluno,
          ["cd_usuario"] = cd_usuario,
          ["cd_pessoa_responsavel"] = responsavel,
          ["cd_tipo_financeiro"] = model.cd_tipo_financeiro,
          ["cd_plano_conta"] = parametroExists["cd_plano_conta_mat"],
          ["cd_produto_atual"] = model.cd_produto_atual,
          ["cd_curso_atual"] = model.cd_curso_atual,
          ["cd_regime_atual"] = model.cd_regime_atual,
          ["cd_duracao_atual"] = model.cd_duracao_atual,
          ["cd_pessoa_escola"] = model.cd_pessoa_escola,
          ["dt_inicial_contrato"] = model.dt_inicial_contrato.Date.ToString("yyyy-MM-ddTHH:mm:ss") ?? null,
          ["dt_final_contrato"] = model.dt_final_contrato?.ToString("yyyy-MM-ddTHH:mm:ss") ?? null,
          ["dt_matricula_contrato"] = model.dt_matricula_contrato?.ToString("yyyy-MM-ddTHH:mm:ss") ?? null,
          ["id_nf_servico"] = 0,
          ["id_ajuste_manual"] = 0,
          ["id_contrato_aula"] = 0,
          ["id_divida_primeira_parcela"] = 0,
          ["id_tipo_matricula"] = model.id_tipo_matricula,
          ["nm_contrato"] = nm_contrato,
          ["dt_vencimento_parcela_1"] = model.dt_vencimento_parcela_1?.ToString("yyyy-MM-ddTHH:mm:ss") ?? null,
          ["dt_vencimento_parcela_1_material"] = model.dt_vencimento_parcela_1_material?.ToString("yyyy-MM-ddTHH:mm:ss") ?? null,
          ["nm_dia_vcto"] = dataVencimento?.Day,
          ["nm_mes_vcto"] = dataVencimento?.Month,
          ["nm_ano_vcto"] = dataVencimento?.Year,
          ["nm_parcelas_mensalidade"] = model.nm_parcelas_mensalidade,
          ["nm_matricula_contrato"] = nm_matricula,
          ["pc_responsavel_contrato"] = (model.pc_responsavel_contrato ?? 0) == 0 ? 100 : Math.Round(model.pc_responsavel_contrato.Value, 2),
          ["pc_desconto_contrato"] = model.id_tipo_contrato == 1 ? 0m : Math.Round(model.pc_desconto_contrato ?? 0m, 4),
          ["vl_curso_contrato"] = Math.Round(model.vl_curso_contrato ?? 0m, 2),
          ["vl_matricula_contrato"] = 0m,
          ["vl_parcela_contrato"] = model.id_tipo_contrato == 1 ? 0m : Math.Round(model.vl_parcela_contrato ?? 0m, 2),
          ["vl_desconto_contrato"] = model.id_tipo_contrato == 1 ? 0m : Math.Round(model.vl_desconto_contrato ?? 0m, 2),
          ["vl_divida_contrato"] = 0m,
          ["vl_desc_primeira_parcela"] = 0m,
          ["vl_parcela_liquida"] = model.id_tipo_contrato == 1 ? 0m : Math.Round(model.vl_parcela_liquida ?? 0m, 2),
          ["vl_liquido_contrato"] = Math.Round(model.vl_liquido_contrato ?? 0m, 2),
          ["id_renegociacao"] = 0,
          ["id_transferencia"] = model.id_transferencia,
          ["id_retorno"] = model.id_retorno,
          ["id_venda_pacote"] = 0,
          ["pc_desconto_bolsa"] = model.pc_desconto_bolsa ?? 0m,
          ["vl_pre_matricula"] = 0m,
          ["cd_ano_escolar"] = model.cd_ano_escolar,
          ["id_liberar_certificado"] = 1,
          ["id_tipo_contrato"] = model.id_tipo_contrato,
          ["nm_mes_curso_inicial"] = model.nm_mes_curso_inicial,
          ["nm_ano_curso_inicial"] = model.nm_ano_curso_inicial,
          ["nm_mes_curso_final"] = model.nm_mes_curso_final,
          ["nm_ano_curso_final"] = model.nm_ano_curso_final,
          ["nm_arquivo_digitalizado"] = model.nm_arquivo_digitalizado,
          ["nm_parcelas_material"] = model.nm_parcelas_material,
          ["vl_parcela_material"] = Math.Round(model.vl_parcela_material ?? 0m, 2),
          ["vl_material_contrato"] = Math.Round(model.vl_material_contrato ?? 0m, 2),
          ["vl_parcela_liq_material"] = Math.Round(model.vl_parcela_liq_material ?? 0m, 2),
          ["pc_bolsa_material"] = model.pc_bolsa_material ?? 0m,
          ["cd_nome_contrato"] = model.cd_nome_contrato == 0 ? null : model.cd_nome_contrato,
          ["id_tipo_data_inicio"] = model.id_tipo_data_inicio,
          //["dt_inicio_aditamento"] = string.IsNullOrEmpty(model.dt_inicio_adto) ? null : model.dt_inicio_aditamento,
          ["nm_dia_vcto_desconto"] = model.nm_dia_vcto_desconto,
          ["nm_previsao_inicial"] = model.nm_previsao_inicial,
          ["vl_aula_hora"] = model.vl_aula_hora,
          ["tx_obs_contrato"] = model.tx_obs_contrato,
          ["pc_desconto_material"] = Math.Round(model.pc_desconto_material ?? 0m, 2),
          ["vl_liquido_material"] = Math.Round(model.vl_liquido_material ?? 0m, 2),
          ["vl_desconto_material"] = Math.Round(model.vl_desconto_material ?? 0m, 2),
          ["id_opcao_venda"] = model.id_opcao_venda,
          ["cd_tipo_financeiro_material"] = model.cd_tipo_financeiro_material,
          ["cd_pessoa_responsavel_material"] = model.cd_pessoa_responsavel_material,
          ["pc_responsavel_material"] = (model.pc_responsavel_material ?? 0m) == 0m ? 100m : Math.Round(model.pc_responsavel_material.Value, 2),
          ["id_status_contrato"] = 0,
          ["cd_fila_matricula"] = model.cd_fila_matricula
        };

        var matriculaResult = await SQLServerService.Insert("T_CONTRATO", matricula_dict, source);
        if (!matriculaResult.success) return BadRequest(matriculaResult.error);

        var matriculaCadastradaGet = await SQLServerService.GetList("T_CONTRATO", 1, 1, "cd_contrato", true, null, null, "", source, SearchModeEnum.Equals, null, null);
        var matricula = matriculaCadastradaGet.data.First();
        var cd_escola = model.cd_pessoa_escola;
        var cd_contrato = matricula["cd_contrato"];

        // ✅ RESTAURADO: O LEGADO cria aditamento vazio (id_tipo_aditamento = NULL) ao criar matrícula
        // Esse aditamento vazio é necessário para o sistema antigo funcionar corretamente
        // Filtrar apenas aditamentos COM id_tipo_aditamento (excluir o aditamento vazio criado na matrícula)
        var aditamentos_anteriores = await SQLServerService.GetList("T_ADITAMENTO", null, "[cd_contrato]", $"[{cd_contrato}]", source);
        var sequencia_aditamento = aditamentos_anteriores.success && aditamentos_anteriores.data != null ? aditamentos_anteriores.data.Count + 1 : 1;

        var dict_aditamento = new Dictionary<string, object>
        {
          ["cd_contrato"] = cd_contrato,
          ["vl_aula_hora"] = 0,
          ["nm_titulos_aditamento"] = 0,
          ["cd_usuario"] = model.cd_usuario,
          ["vl_aditivo"] = 0,
          ["vl_parcela_titulo_aditamento"] = 0,
          ["id_ajuste_manual"] = 0,
          ["dt_aditamento"] = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
          ["cd_tipo_financeiro"] = model.cd_tipo_financeiro,
          ["cd_nome_contrato"] = model.cd_nome_contrato == 0 ? null : model.cd_nome_contrato,
          ["nm_sequencia_aditamento"] = sequencia_aditamento.ToString(),
          ["dt_inicio_aditamento"] = model.dt_inicio_aditamento,
          ["id_tipo_data_inicio"] = model.id_tipo_data_inicio ?? 0,
          ["nm_previsao_inicial"] = model.nm_previsao_inicial,
          ["nm_dia_vcto_desconto"] = model.nm_dia_vcto_desconto
          // ⚠️ NOTE: id_tipo_aditamento é deixado em NULL intencional (aditamento vazio para compatibilidade com legado)
        };
        var result_aditamento = await SQLServerService.Insert("T_ADITAMENTO", dict_aditamento, source);
        if (!result_aditamento.success) return BadRequest(result_aditamento.error);

        //atualizar crm
        if (!string.IsNullOrEmpty(model.cd_fila_matricula))
        {
          var fila_matricula_result = await SQLServerService.GetFirstByFields(source, "T_FILA_MATRICULA", new List<(string campo, object valor)> { new("cd_fila_matricula", model.cd_fila_matricula) });
          if (fila_matricula_result != null)
          {
            var fila_matricula_update = new Dictionary<string, object>
                {
                    { "id_status_fila", 3 }, // 3 - Matrículado
                };
            var fila_matricula_update_result = await SQLServerService.Update("T_FILA_MATRICULA", fila_matricula_update, source, "cd_fila_matricula", model.cd_fila_matricula);
            if (!fila_matricula_update_result.success) return BadRequest(fila_matricula_update_result.error);
          }

          var filtrosPipeline = new List<(string campo, object valor)>
            {
                new("cd_pessoa_pipeline", fila_matricula_result["cd_pessoa_fila"]),
                new("cd_acao", fila_matricula_result["cd_acao"]),
                new("cd_etapa_pipeline", 3)
            };
          var pipelineResult = await SQLServerService.GetFirstByFields(source, "T_PIPELINE", filtrosPipeline);
          if (pipelineResult != null)
          {
            var pipelineUpdate = new Dictionary<string, object>
                {
                    { "cd_etapa_pipeline", 5 }, // 2 - Matrículado
                    { "id_posicao_pipeline", 5 }, // 2 - Matrículado
                };
            var fila_matricula_update_result = await SQLServerService.Update("T_PIPELINE", pipelineUpdate, source, "cd_pipeline", pipelineResult["cd_pipeline"]);
            if (!fila_matricula_update_result.success) return BadRequest(fila_matricula_update_result.error);
          }
        }

        var cursosContrato = new List<int>();
        if (!model.CursoContrato.IsNullOrEmpty())
        {
          foreach (var curso_contrato in model.CursoContrato)
          {
            var curso = new Dictionary<string, object?>
                        {
                            { "cd_contrato", cd_contrato },
                            { "cd_curso", curso_contrato.cd_curso },
                            { "cd_duracao", curso_contrato.cd_duracao },
                            { "cd_tipo_financeiro", curso_contrato.cd_tipo_financeiro_curso },
                            { "cd_pessoa_responsavel", curso_contrato.cd_pessoa_responsavel_curso },
                            { "nm_dia_vcto", curso_contrato.nm_dia_vcto_curso },
                            { "nm_mes_vcto", curso_contrato.nm_mes_vcto_curso },
                            { "nm_ano_vcto", curso_contrato.nm_ano_vcto_curso },
                            { "nm_parcelas_mensalidade", curso_contrato.nm_parcelas_curso },
                            { "vl_curso_contrato", curso_contrato.vl_curso_total },
                            { "pc_desconto_contrato", curso_contrato.pc_desconto_contrato_curso },
                            { "vl_matricula_curso", curso_contrato.vl_matricula_curso },
                            { "vl_parcela_contrato", curso_contrato.vl_parcela_curso },
                            { "vl_desconto_contrato", curso_contrato.vl_desconto_curso },
                            { "pc_responsavel_contrato", curso_contrato.pc_responsavel_curso },
                            { "vl_parcela_liquida", curso_contrato.vl_parcela_liquida_curso },
                            { "id_liberar_certificado", curso_contrato.id_liberar_certificado },
                            { "vl_curso_liquido", curso_contrato.vl_curso_liquido },
                            { "nm_mes_curso_inicial", curso_contrato.nm_mes_curso_inicial_curso },
                            { "nm_ano_curso_inicial", curso_contrato.nm_ano_curso_inicial_curso },
                            { "nm_mes_curso_final", curso_contrato.nm_mes_curso_final_curso },
                            { "nm_ano_curso_final", curso_contrato.nm_ano_curso_final_curso },
                            { "id_valor_incluso", curso_contrato.id_valor_incluso },
                            { "id_incorporar_valor_material", curso_contrato.id_incorporar_valor_material },
                            { "nm_parcelas_material", curso_contrato.nm_parcelas_material_curso },

                            { "vl_parcela_material", curso_contrato.vl_parcelas_material_curso },
                            { "vl_material_contrato", curso_contrato.vl_material_curso },
                            { "vl_parcela_liq_material", curso_contrato.vl_parcela_liq_material_curso },
                            { "pc_bolsa_material", curso_contrato.pc_bolsa_material_curso },
                            { "pc_desconto_material", curso_contrato.pc_desconto_material_curso },
                            { "vl_liquido_material", curso_contrato.vl_liquido_material_curso },
                            { "vl_desconto_material", curso_contrato.vl_desconto_material_curso },
                            { "id_opcao_venda", curso_contrato.opcao_venda_curso },
                            { "cd_tipo_financeiro_material", curso_contrato.cd_tipo_financeiro_material_curso },
                            { "cd_pessoa_responsavel_material", curso_contrato.cd_pessoa_responsavel_material_curso },
                            { "pc_responsavel_material", curso_contrato.pc_responsavel_material_curso },
                            { "dt_vencimento_parcela_1", curso_contrato.dt_vencimento_parcela_1_curso?.ToString("yyyy-MM-ddTHH:mm:ss") },
                            { "cd_regime", curso_contrato.cd_regime },
                            { "pc_bolsa_contrato", curso_contrato.pc_bolsa_curso },
                            { "dt_vencimento_parcela_1_material", curso_contrato.dt_vencimento_parcela_1_material_curso?.ToString("yyyy-MM-ddTHH:mm:ss") }
                        };
            //T_CURSO_MATRICULA

            var t_curso_contrato_Result = await SQLServerService.InsertWithResult("T_CURSO_CONTRATO", curso, source);
            if (!t_curso_contrato_Result.success) return BadRequest(t_curso_contrato_Result.error);

            cursosContrato.Add(int.Parse(t_curso_contrato_Result.inserted["cd_curso_contrato"].ToString()));
          }
        }

        //T_TAXA
        if (model.Taxa != null && model.Taxa.vl_matricula_taxa != null && model.Taxa.vl_matricula_taxa > 0)
        {
          var taxa_dict = new Dictionary<string, object>
                    {
                        { "cd_contrato", cd_contrato },
                        { "vl_matricula_taxa", model.Taxa.vl_matricula_taxa },
                        { "dt_vcto_taxa", model.Taxa.dt_vcto_taxa.ToString("yyyy-MM-ddTHH:mm:ss") },
                        { "nm_parcelas_taxa", model.Taxa.nm_parcelas_taxa },
                        { "pc_responsavel_taxa", model.Taxa.pc_responsavel_taxa },
                        { "cd_pessoa_responsavel_taxa", model.Taxa.cd_pessoa_responsavel_taxa },
                        { "cd_tipo_financeiro_taxa", model.Taxa.cd_tipo_financeiro_taxa },
                        { "cd_plano_conta_taxa", model.Taxa.cd_plano_conta_taxa },
                        { "vl_parcela_taxa", model.Taxa.vl_parcela_taxa }
                    };
          var t_Taxa_matricula_Result = await SQLServerService.Insert("T_TAXA_MATRICULA", taxa_dict, source);
          if (!t_Taxa_matricula_Result.success) return BadRequest(t_Taxa_matricula_Result.error);
        }

        //T_Desconto_Contrato
        if (!model.Descontos.IsNullOrEmpty())
        {
          foreach (var desconto in model.Descontos)
          {
            var dict = new Dictionary<string, object>
            {
              ["cd_contrato"] = cd_contrato,
              ["cd_desconto"] = desconto.cd_desconto,
              ["dc_desconto_contrato"] = desconto.dc_desconto,
              ["id_desconto_ativo"] = desconto.id_desconto_ativo,
              ["pc_desconto_contrato"] = desconto.pc_desconto,
              ["vl_desconto_contrato"] = desconto.vl_desconto,
              ["id_incide_baixa"] = desconto.id_incide_baixa,
              ["nm_parcela_ini"] = desconto.nm_parcela_inicial,
              ["nm_parcela_fim"] = desconto.nm_parcela_final,
              ["id_incide_matricula"] = desconto.id_incide_matricula,
              ["id_incide_material"] = desconto.id_incide_material,
              ["id_aditamento"] = desconto.id_aditamento,
              ["cd_tipo_desconto"] = desconto.cd_tipo_desconto,
            };
            var t_Desconto_matricula_Result = await SQLServerService.Insert("T_DESCONTO_CONTRATO", dict, source);
            if (!t_Desconto_matricula_Result.success) return BadRequest(t_Desconto_matricula_Result.error);
          }
        }
        //T_Titulo_Taxa
        if (!model.TitulosTaxa.IsNullOrEmpty())
        {
          foreach (var titulo in model.TitulosTaxa)
          {
            var dictTitulo = new Dictionary<string, object>
            {
              ["cd_pessoa_empresa"] = cd_escola,
              ["cd_pessoa_titulo"] = titulo.cd_pessoa_titulo,
              ["cd_pessoa_responsavel"] = titulo.cd_pessoa_responsavel != 0 ? titulo.cd_pessoa_responsavel : responsavel,

              ["cd_local_movto"] = parametroExists["cd_local_movto"],
              ["dt_emissao_titulo"] = titulo.dt_emissao_titulo.ToString("yyyy-MM-ddTHH:mm:ss"),

              ["cd_origem_titulo"] = cd_contrato,
              ["dt_vcto_titulo"] = titulo.dt_vcto_titulo.ToString("yyyy-MM-ddTHH:mm:ss"),
              ["dh_cadastro_titulo"] = DateTime.Now.Date,
              ["vl_titulo"] = titulo.vl_titulo,
              ["vl_saldo_titulo"] = titulo.vl_saldo_titulo,
              ["dc_tipo_titulo"] = "TX",
              ["dc_num_documento_titulo"] = titulo.dc_num_documento_titulo,
              ["nm_titulo"] = nm_contrato,
              ["nm_parcela_titulo"] = titulo.nm_parcela_titulo,
              ["cd_tipo_financeiro"] = titulo.cd_tipo_financeiro,
              ["id_status_titulo"] = 1,
              ["id_status_cnab"] = titulo.id_status_cnab,
              ["id_origem_titulo"] = 22,
              ["id_natureza_titulo"] = 1,
              ["vl_material_titulo"] = titulo.vl_material_titulo,
              ["pc_taxa_cartao"] = titulo.pc_taxa_cartao,
              ["nm_dias_cartao"] = titulo.nm_dias_cartao,
              ["id_cnab_contrato"] = titulo.id_cnab_contrato,
              ["vl_taxa_cartao"] = titulo.vl_taxa_cartao,
              ["cd_aluno"] = titulo.cd_aluno,
              ["pc_responsavel"] = titulo.pc_responsavel == null || titulo.pc_responsavel == 0 ? 100 : titulo.pc_responsavel,
              ["vl_mensalidade"] = titulo.vl_mensalidade,
              ["pc_bolsa"] = titulo.pc_bolsa,
              ["vl_bolsa"] = titulo.vl_bolsa,
              ["pc_desconto_mensalidade"] = titulo.pc_desconto_mensalidade,
              ["vl_desconto_mensalidade"] = titulo.vl_desconto_mensalidade,
              ["pc_bolsa_material"] = titulo.pc_bolsa_material,
              ["vl_bolsa_material"] = titulo.vl_bolsa_material,
              ["pc_desconto_material"] = titulo.pc_desconto_material,
              ["vl_desconto_material"] = titulo.vl_desconto_material,
              ["pc_desconto_total"] = titulo.pc_desconto_total,
              ["vl_desconto_total"] = titulo.vl_desconto_total,
              ["opcao_venda"] = titulo.opcao_venda,
              ["cd_curso"] = titulo.cd_curso
            };
            var t_titulo_Result = await SQLServerService.Insert("T_TITULO", dictTitulo, source);
            if (!t_titulo_Result.success) return BadRequest(t_titulo_Result.error);

            var t_tituloGet = await SQLServerService.GetList("T_TITULO", 1, 1, "cd_titulo", true, null, null, "", source, SearchModeEnum.Equals, null, null);
            var titulo_inserido = t_tituloGet.data.First();

            var id_origem_titulo = titulo_inserido["id_origem_titulo"]?.ToString() ?? "0";

            if (id_origem_titulo == "22" && titulo.dc_tipo_titulo == "TX")
            {
              //T_plano_titulo
              var dict_plano = new Dictionary<string, object>
              {
                ["cd_titulo"] = titulo_inserido["cd_titulo"],
                ["cd_plano_conta"] = cd_plano_conta_tax,
                ["vl_plano_titulo"] = titulo.vl_titulo
              };
              var t_plano_titulo_Result = await SQLServerService.Insert("T_PLANO_TITULO", dict_plano, source);
              if (!t_plano_titulo_Result.success) return BadRequest(t_plano_titulo_Result.error);
            }
          }
        }
        //T_titulo_mensalidade
        if (!model.TitulosMensalidade.IsNullOrEmpty())
        {
          foreach (var titulo in model.TitulosMensalidade)
          {
            var dictTitulo = new Dictionary<string, object>
            {
              ["cd_pessoa_empresa"] = cd_escola,
              ["cd_pessoa_titulo"] = titulo.cd_pessoa_titulo,
              ["cd_pessoa_responsavel"] = titulo.cd_pessoa_responsavel != 0 ? titulo.cd_pessoa_responsavel : responsavel,
              ["cd_local_movto"] = parametroExists["cd_local_movto"],
              ["dt_emissao_titulo"] = titulo.dt_emissao_titulo.ToString("yyyy-MM-ddTHH:mm:ss"),
              ["cd_origem_titulo"] = cd_contrato,
              ["dt_vcto_titulo"] = titulo.dt_vcto_titulo.ToString("yyyy-MM-ddTHH:mm:ss"),
              ["dh_cadastro_titulo"] = DateTime.Now.Date.ToString("yyyy-MM-ddTHH:mm:ss"),
              ["vl_titulo"] = titulo.vl_titulo,
              ["vl_saldo_titulo"] = titulo.vl_saldo_titulo,
              ["dc_tipo_titulo"] = "ME",
              ["dc_num_documento_titulo"] = titulo.dc_num_documento_titulo,
              ["nm_titulo"] = nm_contrato,
              ["nm_parcela_titulo"] = titulo.nm_parcela_titulo,
              ["cd_tipo_financeiro"] = titulo.cd_tipo_financeiro,
              ["id_status_titulo"] = 1,
              ["id_status_cnab"] = titulo.id_status_cnab,
              ["id_origem_titulo"] = 22,
              ["id_natureza_titulo"] = 1,
              ["vl_material_titulo"] = titulo.vl_material_titulo,
              ["pc_taxa_cartao"] = titulo.pc_taxa_cartao,
              ["nm_dias_cartao"] = titulo.nm_dias_cartao,
              ["id_cnab_contrato"] = titulo.id_cnab_contrato,
              ["vl_taxa_cartao"] = titulo.vl_taxa_cartao,
              ["cd_aluno"] = titulo.cd_aluno,
              ["pc_responsavel"] = titulo.pc_responsavel == null || titulo.pc_responsavel == 0 ? 100 : titulo.pc_responsavel,
              ["vl_mensalidade"] = titulo.vl_mensalidade,
              ["pc_bolsa"] = titulo.pc_bolsa,
              ["vl_bolsa"] = titulo.vl_bolsa,
              ["pc_desconto_mensalidade"] = titulo.pc_desconto_mensalidade,
              ["vl_desconto_mensalidade"] = titulo.vl_desconto_mensalidade,
              ["pc_bolsa_material"] = titulo.pc_bolsa_material,
              ["vl_bolsa_material"] = titulo.vl_bolsa_material,
              ["pc_desconto_material"] = titulo.pc_desconto_material,
              ["vl_desconto_material"] = titulo.vl_desconto_material,
              ["pc_desconto_total"] = titulo.pc_desconto_total,
              ["vl_desconto_total"] = titulo.vl_desconto_total,
              ["opcao_venda"] = titulo.opcao_venda,
              ["cd_curso"] = titulo.cd_curso
            };
            var t_titulo_Result = await SQLServerService.Insert("T_TITULO", dictTitulo, source);
            if (!t_titulo_Result.success) return BadRequest(t_titulo_Result.error);

            var t_tituloGet = await SQLServerService.GetList("T_TITULO", 1, 1, "cd_titulo", true, null, null, "", source, SearchModeEnum.Equals, null, null);
            var titulo_inserido = t_tituloGet.data.First();

            var id_origem_titulo = titulo_inserido["id_origem_titulo"]?.ToString() ?? "0";

            if (id_origem_titulo == "22" && titulo.dc_tipo_titulo == "ME")
            {
              //T_plano_titulo
              var dict_plano = new Dictionary<string, object>
              {
                ["cd_titulo"] = titulo_inserido["cd_titulo"],
                ["cd_plano_conta"] = cd_plano_conta_mat,
                ["vl_plano_titulo"] = titulo.vl_mensalidade
              };
              var t_plano_titulo_Result = await SQLServerService.Insert("T_PLANO_TITULO", dict_plano, source);
              if (!t_plano_titulo_Result.success) return BadRequest(t_plano_titulo_Result.error);
            }

            if (id_origem_titulo == "22" && titulo.dc_tipo_titulo == "ME" && titulo.vl_material_titulo > 0)
            {
              //T_plano_titulo
              var dict_plano = new Dictionary<string, object>
              {
                ["cd_titulo"] = titulo_inserido["cd_titulo"],
                ["cd_plano_conta"] = cd_plano_conta_mtr,
                ["vl_plano_titulo"] = titulo.vl_material_titulo
              };
              var t_plano_titulo_Result = await SQLServerService.Insert("T_PLANO_TITULO", dict_plano, source);
              if (!t_plano_titulo_Result.success) return BadRequest(t_plano_titulo_Result.error);
            }
          }
        }

        if (model.id_tipo_contrato != 2)
        {
          //T_titulo_Material
          if (!model.TitulosMaterial.IsNullOrEmpty())
          {
            foreach (var titulo in model.TitulosMaterial)
            {
              var dictTitulo = new Dictionary<string, object>
              {
                ["cd_pessoa_empresa"] = cd_escola,
                ["cd_pessoa_titulo"] = titulo.cd_pessoa_titulo,
                ["cd_pessoa_responsavel"] = titulo.cd_pessoa_responsavel != 0 ? titulo.cd_pessoa_responsavel : responsavel,

                ["cd_local_movto"] = parametroExists["cd_local_movto"],
                ["dt_emissao_titulo"] = titulo.dt_emissao_titulo.ToString("yyyy-MM-ddTHH:mm:ss"),

                ["cd_origem_titulo"] = cd_contrato,
                ["dt_vcto_titulo"] = titulo.dt_vcto_titulo.ToString("yyyy-MM-ddTHH:mm:ss"),
                ["dh_cadastro_titulo"] = DateTime.Now.Date,
                ["vl_titulo"] = titulo.vl_titulo,
                ["vl_saldo_titulo"] = titulo.vl_saldo_titulo,
                ["dc_tipo_titulo"] = "MT",
                ["dc_num_documento_titulo"] = titulo.dc_num_documento_titulo,
                ["nm_titulo"] = nm_contrato,
                ["nm_parcela_titulo"] = titulo.nm_parcela_titulo,
                ["cd_tipo_financeiro"] = titulo.cd_tipo_financeiro,
                ["id_status_titulo"] = 1,
                ["id_status_cnab"] = titulo.id_status_cnab,
                ["id_origem_titulo"] = 22,
                ["id_natureza_titulo"] = 1,
                ["vl_material_titulo"] = titulo.vl_material_titulo,
                ["pc_taxa_cartao"] = titulo.pc_taxa_cartao,
                ["nm_dias_cartao"] = titulo.nm_dias_cartao,
                ["id_cnab_contrato"] = titulo.id_cnab_contrato,
                ["vl_taxa_cartao"] = titulo.vl_taxa_cartao,
                ["cd_aluno"] = titulo.cd_aluno,
                ["pc_responsavel"] = titulo.pc_responsavel == null || titulo.pc_responsavel == 0 ? 100 : titulo.pc_responsavel,
                ["vl_mensalidade"] = titulo.vl_mensalidade,
                ["pc_bolsa"] = titulo.pc_bolsa,
                ["vl_bolsa"] = titulo.vl_bolsa,
                ["pc_desconto_mensalidade"] = titulo.pc_desconto_mensalidade,
                ["vl_desconto_mensalidade"] = titulo.vl_desconto_mensalidade,
                ["pc_bolsa_material"] = titulo.pc_bolsa_material,
                ["vl_bolsa_material"] = titulo.vl_bolsa_material,
                ["pc_desconto_material"] = titulo.pc_desconto_material,
                ["vl_desconto_material"] = titulo.vl_desconto_material,
                ["pc_desconto_total"] = titulo.pc_desconto_total,
                ["vl_desconto_total"] = titulo.vl_desconto_total,
                ["opcao_venda"] = titulo.opcao_venda,
                ["cd_curso"] = titulo.cd_curso
              };
              var t_titulo_Result = await SQLServerService.Insert("T_TITULO", dictTitulo, source);
              if (!t_titulo_Result.success) return BadRequest(t_titulo_Result.error);
              var titulo_inseridoGet = await SQLServerService.GetList("T_TITULO", 1, 1, "cd_titulo", true, null, null, "", source, SearchModeEnum.Equals, null, null);
              var titulo_inserido = titulo_inseridoGet.data.First();

              var id_origem_titulo = titulo_inserido["id_origem_titulo"]?.ToString() ?? "0";

              if (id_origem_titulo == "22" && titulo.dc_tipo_titulo == "MT")
              {
                //T_plano_titulo
                var dict_plano = new Dictionary<string, object>
                {
                  ["cd_titulo"] = titulo_inserido["cd_titulo"],
                  ["cd_plano_conta"] = cd_plano_conta_mtr,
                  ["vl_plano_titulo"] = titulo.vl_titulo
                };
                var t_plano_titulo_Result = await SQLServerService.Insert("T_PLANO_TITULO", dict_plano, source);
                if (!t_plano_titulo_Result.success) return BadRequest(t_plano_titulo_Result.error);
              }
            }
          }
        }
        //T_cheque
        if (model.Cheque != null)
        {
          var cheque_dict = new Dictionary<string, object?>
          {
            ["cd_contrato"] = cd_contrato,
            ["no_emitente_cheque"] = model.Cheque.no_emitente_cheque,
            ["no_agencia_cheque"] = model.Cheque.no_agencia_cheque,
            ["nm_agencia_cheque"] = model.Cheque.nm_agencia_cheque,
            ["nm_digito_agencia_cheque"] = model.Cheque.nm_digito_agencia_cheque,
            ["nm_conta_corrente_cheque"] = model.Cheque.nm_conta_corrente_cheque,
            ["nm_digito_cc_cheque"] = model.Cheque.nm_digito_cc_cheque,
            ["nm_primeiro_cheque"] = model.Cheque.nm_primeiro_cheque,
            ["cd_banco"] = model.Cheque.cd_banco
          };

          var t_cheque_Result = await SQLServerService.Insert("T_CHEQUE", cheque_dict, source);
          if (!t_cheque_Result.success) return BadRequest(t_cheque_Result.error);
        }


        ////venda material
        if (!model.VendasMaterial.IsNullOrEmpty())
        {
          var estoque_ok = true;
          var cd_curso_numero = 0;
          foreach (var venda in model.VendasMaterial)
          {
            if (venda.cd_curso != cd_curso_numero)
            {
              nm_nf_mercantil++;
              cd_curso_numero = venda.cd_curso;
            }
            // Validação conforme procedure: verificar se curso está vinculado ao contrato
            if (venda.cd_curso == null || venda.cd_curso == 0)
            {
              return BadRequest("Parâmetro Curso não informado.");
            }
            var curso_contrato = await SQLServerService.GetFirstByFields(source, "T_CURSO_CONTRATO",
                new List<(string campo, object valor)> { new("cd_curso", venda.cd_curso), new("cd_contrato", cd_contrato) });

            if (curso_contrato == null)
            {
              return BadRequest("Favor salvar a alteração do curso primeiro para poder prosseguir com a geração da venda de material.");
            }

            // Validação da modalidade/regime conforme procedure
            var contrato = await SQLServerService.GetFirstByFields(source, "T_CONTRATO",
                new List<(string campo, object valor)> { new("cd_contrato", cd_contrato) });

            var cd_regime = model.cd_regime_atual;

            if (cd_regime == null)
            {
              return BadRequest("A modalidade do contrato não foi definida. Para vincular a venda de material didático esta informação é necessária.");
            }

            var regime = await SQLServerService.GetFirstByFields(source, "T_REGIME",
                new List<(string campo, object valor)> { new("cd_regime", cd_regime) });

            var no_regime_abreviado = regime?["no_regime_abreviado"]?.ToString();

            if (string.IsNullOrEmpty(no_regime_abreviado))
            {
              return BadRequest("A modalidade do contrato não foi definida. Para vincular a venda de material didático esta informação é necessária.");
            }

            var item_escola = await SQLServerService.GetFirstByFields(source, "T_ITEM_ESCOLA", new List<(string campo, object valor)> { new("cd_item", venda.cd_item), new("cd_pessoa_escola", cd_escola) });

            var item = await SQLServerService.GetFirstByFields(source, "T_ITEM", new List<(string campo, object valor)> { new("cd_item", venda.cd_item) });

            //não gerar venda se não ha estoque para livro ou apostila
            if (venda.venda && !estoque_ok) continue;

            // Verificação de movimento existente conforme procedure
            List<(string campo, object valor)> filtroMovimento;
            var id_normal = contrato?["id_tipo_contrato"]?.ToString() == "0"; // Matricula normal

            if (id_normal)
            {
              // Para matrículas normais, verificar sem o curso
              filtroMovimento = new List<(string campo, object valor)>
                            {
                                new("id_origem_movimento", 22),
                                new("cd_origem_movimento", cd_contrato),
                                new("id_venda_futura", venda.venda ? 0 : 1),
                                new("id_material_didatico", 1)
                            };
            }
            else
            {
              // Para outras matrículas, verificar com o curso
              filtroMovimento = new List<(string campo, object valor)>
                            {
                                new("id_origem_movimento", 22),
                                new("cd_origem_movimento", cd_contrato),
                                new("cd_curso", venda.cd_curso),
                                new("id_venda_futura", venda.venda ? 0 : 1),
                                new("id_material_didatico", 1)
                            };
            }

            var movimento_existente = await SQLServerService.GetFirstByFields(source, "T_MOVIMENTO", filtroMovimento);

            // Verificar se já existe nota sem curso definido (conforme procedure)
            var movimento_sem_curso = await SQLServerService.GetFirstByFields(source, "T_MOVIMENTO",
                new List<(string campo, object valor)>
                {
                                new("id_origem_movimento", 22),
                                new("cd_origem_movimento", cd_contrato),
                                new("id_venda_futura", venda.venda ? 0 : 1),
                                new("id_material_didatico", 1),
                                new("cd_curso", DBNull.Value)
                });

            if (movimento_sem_curso != null)
            {
              var nm_movimento = movimento_sem_curso["nm_movimento"];
              var id_nf = movimento_sem_curso["id_nf"];
              var tipoDoc = (bool)id_nf ? "Nota Fiscal" : "Movimento";
              return BadRequest($"Não foi definido o curso no {tipoDoc}, já existente com o número {nm_movimento}");
            }

            if (movimento_existente != null)
            {
              // Conforme procedure, verificar se precisa gerar novos itens ou se já está completo
              // Por ora, vamos permitir o processamento se o movimento já existe
              // mas verificar se está completo conforme a lógica da procedure
            }

            var cd_tipo_nota_fiscal = parametroExists["cd_tipo_nf_material"];
            var tipo_nota_fiscal = await SQLServerService.GetFirstByFields(source, "t_tipo_nota_fiscal", new List<(string campo, object valor)> { new("cd_tipo_nota_fiscal", cd_tipo_nota_fiscal) });
            var dc_cfop = tipo_nota_fiscal?["dc_CFOP"];
            // Calcular CFOP baseado nos estados (conforme procedure)
            var cfopCalculado = await VerificaEstadoEscAluno(Convert.ToInt32(cd_escola), Convert.ToInt32(cd_pessoa_aluno), (int)TipoMovimentoEnum.SERVICO, source);
            var dc_cfop_final = cfopCalculado;

            var tx_obs_fiscal = tipo_nota_fiscal?["tx_obs_tipo_nota"];
            var cd_cfop = tipo_nota_fiscal?["cd_cfop"];
            var cd_movimento = 0;
            Dictionary<string, object>? movimento = null;
            if (movimento_existente == null)
            {
              // Buscar o responsável do contrato (conforme a procedure)
              var cd_responsavel = contrato?["cd_pessoa_responsavel"];

              // Buscar tipo financeiro (conforme procedure: 'Titulo')
              var tipoFinanceiro = await SQLServerService.GetFirstByFields(source, "T_TIPO_FINANCEIRO", new List<(string campo, object valor)> { new("dc_tipo_financeiro", "Titulo") });
              var cd_tipo_financeiro = tipoFinanceiro?["cd_tipo_financeiro"] ?? 3;

              //movimento
              var movimento_dict = new Dictionary<string, object>
                                {
                                    {"cd_origem_movimento",cd_contrato },
                                    { "cd_pessoa_empresa", cd_escola},
                                    { "cd_pessoa", cd_responsavel ?? cd_pessoa_aluno}, // Usar responsável conforme procedure
                                    { "cd_aluno", model.cd_aluno},
                                    { "cd_politica_comercial", parametroExists["cd_politica_comercial_nf"]},
                                    { "cd_tipo_financeiro", cd_tipo_financeiro },
                                    { "id_tipo_movimento", 2 },
                                    { "nm_movimento", nm_nf_mercantil},
                                    { "dc_serie_movimento", (bool)parametroExists["id_emitir_nf_mercantil"] ? parametroExists["dc_serie_nf_mercantil"] ?? "1" : (venda.venda ? "M" : "F") },
                                    { "dt_emissao_movimento", DateTime.Now.Date.ToString("yyyy-MM-ddTHH:mm:ss") ?? DateTime.Now.Date.ToString("yyyy-MM-ddTHH:mm:ss") },
                                    { "dt_vcto_movimento", DateTime.Now.Date.ToString("yyyy-MM-ddTHH:mm:ss") ?? DateTime.Now.Date.ToString("yyyy-MM-ddTHH:mm:ss") },
                                    { "dt_mov_movimento", DateTime.Now.Date.ToString("yyyy-MM-ddTHH:mm:ss") ?? DateTime.Now.Date.ToString("yyyy-MM-ddTHH:mm:ss") },
                                    { "pc_acrescimo",  0 },
                                    { "vl_acrescimo",  0 },
                                    { "pc_desconto",  0 },
                                    { "vl_desconto", 0 },
                                    { "id_nf", parametroExists["id_emitir_nf_mercantil"]},
                                    { "id_status_nf", 1 }, // Conforme procedure
                                    { "id_nf_escola", parametroExists["id_emitir_nf_mercantil"]},
                                    { "vl_base_calculo_ICMS_nf", 0 }, // Será calculado pelos itens
                                    { "vl_base_calculo_PIS_nf", 0 },
                                    { "vl_base_calculo_COFINS_nf", 0},
                                    { "vl_base_calculo_IPI_nf", 0},
                                    { "vl_base_calculo_ISS_nf", 0},
                                    { "vl_ICMS_nf", 0 }, // Será calculado pelos itens
                                    { "vl_PIS_nf", 0 },
                                    { "vl_COFINS_nf", 0},
                                    { "vl_IPI_nf", 0 },
                                    { "vl_ISS_nf", 0 },
                                    { "pc_aliquota_aproximada", 0 },
                                    { "vl_aproximado", 0 },
                                    { "id_exportado", 0 },
                                    { "id_importacao_xml", 0 },
                                    { "id_material_didatico", 1 },
                                    { "id_venda_futura", venda.venda ? 0 : 1 },
                                    { "id_origem_movimento", 22 },
                                    { "nm_nfe", venda.venda ? nm_nf_mercantil : (object)DBNull.Value },
                                    { "cd_curso", venda.cd_curso },
                                    { "tx_obs_fiscal", tx_obs_fiscal},
                                    { "cd_tipo_nota_fiscal", (bool)parametroExists["id_emitir_nf_mercantil"] ? parametroExists["cd_tipo_nf_material"] : (object)DBNull.Value},
                                    { "cd_cfop_nf", (bool)parametroExists["id_emitir_nf_mercantil"] ? cd_cfop : (object)DBNull.Value},
                                    { "dc_cfop_nf", (bool)parametroExists["id_emitir_nf_mercantil"] ? dc_cfop_final : (object)DBNull.Value },
                                    { "dc_key_nfe", "" } // Conforme procedure
                                };
              var t_movimento_Result = await SQLServerService.Insert("T_MOVIMENTO", movimento_dict, source);
              if (!t_movimento_Result.success) return BadRequest(t_movimento_Result.error);

              var movimento_inseridoGet = await SQLServerService.GetList("T_MOVIMENTO", 1, 1, "cd_movimento", true, null, null, "", source, SearchModeEnum.Equals, null, null);
              var movimento_inserido = movimento_inseridoGet.data.First();
              movimento = movimento_inserido;
              cd_movimento = int.Parse(movimento_inserido["cd_movimento"]?.ToString());

              // Atualizar numeração conforme stored procedure - DEPOIS de inserir o movimento
              var isEmitirNF = (bool)parametroExists["id_emitir_nf_mercantil"];
              var numeroMovimento = 0;

              // Atualizar parâmetros conforme SP (linhas 858-877)
              var parametro_update = new Dictionary<string, object>();

              if (isEmitirNF)
              {
                // Se emitir NF mercantil, atualizar nm_nf_mercantil
                parametro_update["nm_nf_mercantil"] = nm_nf_mercantil;
              }
              else
              {
                // Se não emitir NF mercantil, atualizar nm_nf_material (conforme SP linha 869-877)
                parametro_update["nm_nf_material"] = nm_nf_mercantil;
              }

              var param_result = await SQLServerService.Update("T_PARAMETRO", parametro_update, source, "cd_pessoa_escola", cd_escola);
              if (!param_result.success) return BadRequest(param_result.error);
              //movimento item
            }
            else
            {
              movimento = movimento_existente;
              cd_movimento = int.Parse(movimento_existente["cd_movimento"]?.ToString());

              var movimento_update_dict = new Dictionary<string, object>
                            {
                                { "id_venda_futura", venda.venda ? 0 : 1 },
                                { "nm_nfe", venda.venda ? nm_nf_mercantil : (object)DBNull.Value },
                                { "nm_movimento", nm_nf_mercantil },
                                { "dc_serie_movimento", (bool)parametroExists["id_emitir_nf_mercantil"] ? parametroExists["dc_serie_nf_mercantil"] ?? "1" : (venda.venda ? "M" : "F") }
                            };
              var t_movimento_Result = await SQLServerService.Update("T_MOVIMENTO", movimento_update_dict, source, "cd_movimento", cd_movimento);
              if (!t_movimento_Result.success) return BadRequest(t_movimento_Result.error);
            }


            var item_movimento_existente = await SQLServerService.GetFirstByFields(source, "T_ITEM_MOVIMENTO", new List<(string campo, object valor)> { new("cd_item", venda.cd_item), new("cd_movimento", cd_movimento) });


            if (item_movimento_existente == null)
            {
              // Buscar valor do item na escola conforme procedure
              var vl_item = 0m;
              if (item_escola != null)
              {
                var vl_item_escola = item_escola["vl_item"];
                var vl_custo_escola = item_escola["vl_custo"];
                vl_item = Convert.ToDecimal(vl_item_escola) > 0 ? Convert.ToDecimal(vl_item_escola) : Convert.ToDecimal(vl_custo_escola ?? 0);
              }

              // Buscar plano de conta conforme procedure
              var cd_plano_conta_item = cd_plano_conta_mtr; // default
              var item_subgrupo = await SQLServerService.GetFirstByFields(source, "T_ITEM_SUBGRUPO",
                  new List<(string campo, object valor)> { new("cd_item", venda.cd_item), new("id_tipo_movimento", 2) });

              if (item_subgrupo != null)
              {
                var cd_subgrupo_conta = item_subgrupo["cd_subgrupo_conta"];
                var plano_conta = await SQLServerService.GetFirstByFields(source, "T_PLANO_CONTA",
                    new List<(string campo, object valor)> { new("cd_pessoa_empresa", cd_escola), new("cd_subgrupo_conta", cd_subgrupo_conta) });

                if (plano_conta != null)
                  cd_plano_conta_item = plano_conta["cd_plano_conta"].ToString();
              }

              // Situações tributárias conforme procedure (valores padrão)
              var cd_situacao_tributaria_ICMS = (object)DBNull.Value;
              var cd_situacao_tributaria_PIS = 65;
              var cd_situacao_tributaria_COFINS = 107;
              var vl_base_calculo_ICMS = 0m;
              var vl_base_calculo_PIS = vl_item;
              var vl_base_calculo_COFINS = vl_item;
              var vl_base_calculo_IPI = vl_item;
              var vl_ICMS_item = 0m;

              // Se for para emitir NF, calcular impostos
              if ((bool)parametroExists["id_emitir_nf_mercantil"] && parametroExists["cd_tipo_nf_material"] != null)
              {
                // Aqui seria necessário implementar os cálculos tributários da procedure
                // Por ora, manter valores zerados para não quebrar
              }

              var item_movimento_dict = new Dictionary<string, object>
                                {
                                    {"cd_plano_conta", cd_plano_conta_item },
                                    {"dc_item_movimento", item != null ? item["no_item"] : "" },
                                    { "cd_movimento", cd_movimento },
                                    { "cd_item", venda.cd_item },
                                    { "qt_item_movimento", 1 },
                                    { "vl_unitario_item", vl_item },
                                    { "vl_total_item", vl_item },
                                    { "vl_liquido_item", vl_item },
                                    { "vl_acrescimo_item", 0 },
                                    { "vl_desconto_item", 0 },
                                    { "cd_situacao_tributaria_ICMS", cd_situacao_tributaria_ICMS },
                                    { "cd_situacao_tributaria_PIS", cd_situacao_tributaria_PIS },
                                    { "cd_situacao_tributaria_COFINS", cd_situacao_tributaria_COFINS },
                                    { "vl_base_calculo_ICMS_item", vl_base_calculo_ICMS },
                                    { "vl_base_calculo_PIS_item", vl_base_calculo_PIS },
                                    { "vl_base_calculo_COFINS_item", vl_base_calculo_COFINS },
                                    { "vl_base_calculo_IPI_item", vl_base_calculo_IPI },
                                    { "vl_base_calculo_ISS_item", 0 },
                                    { "vl_ICMS_item", vl_ICMS_item },
                                    { "vl_PIS_item", 0},
                                    { "vl_COFINS_item", 0 },
                                    { "vl_IPI_item", 0 },
                                    { "vl_ISS_item", 0 },
                                    { "pc_aliquota_ICMS", 0},
                                    { "pc_aliquota_PIS", 0},
                                    { "pc_aliquota_COFINS", 0 },
                                    { "pc_aliquota_IPI", 0 },
                                    { "pc_aliquota_ISS", 0 },
                                    { "cd_cfop", (bool)parametroExists["id_emitir_nf_mercantil"] ? cd_cfop : (object)DBNull.Value },
                                    { "dc_cfop", (bool)parametroExists["id_emitir_nf_mercantil"] ? dc_cfop_final : (object)DBNull.Value },
                                    { "pc_aliquota_aproximada", 0 },
                                    { "vl_aproximado", 0},
                                    { "pc_desconto_item", 0 }
                                };
              var t_item_movimento_Result = await SQLServerService.Insert("T_ITEM_MOVIMENTO", item_movimento_dict, source);
              if (!t_item_movimento_Result.success) return BadRequest(t_item_movimento_Result.error);
            }

            //remover do estoque
            if (venda.venda)
            {

              if (item_escola != null)
              {
                var cd_item_escola = item_escola["cd_item_escola"];
                var qtde = item_escola["qt_estoque"];
                var qtde_item = int.Parse(qtde?.ToString() ?? "1");

                if ((qtde_item - 1) < 0)
                {
                  estoque_ok = false;
                  continue;
                }
                item_escola.Remove("cd_item_escola");
                item_escola["qt_estoque"] = int.Parse(qtde?.ToString() ?? "1") - 1;
                var t_item_escola_update = await SQLServerService.Update("T_ITEM_ESCOLA", item_escola, source, "cd_item_escola", cd_item_escola);
                if (!t_item_escola_update.success) return BadRequest(t_item_escola_update.error);

              }
            }



          }


        }

        // validação turma(Não é possíver gerar matricula para duas turmas ainda não matriculadas )
        //turma
        if (!model.Turmas.IsNullOrEmpty())
        {
          for (int i = 0; i < model.Turmas.Count; i++)
          {
            var turma = model.Turmas[i];
            var filtroTurma = new List<(string campo, object valor)> { new("cd_turma", turma.cd_turma) };
            var turmaExists = await SQLServerService.GetFirstByFields(source, "T_TURMA", filtroTurma);
            if (turmaExists == null) continue;
            var no_turma = turmaExists["no_turma"];

            if (no_turma == null) continue;
            var cd_turma_original = turmaExists["cd_turma"];
            var original = no_turma?.ToString() ?? string.Empty;

            var partes = original.Split('-', 2); // corta só na primeira barra

            var situacao_aluno = model.id_tipo_matricula == 1 ? 1 :
                          model.id_tipo_matricula == 3 ? 10 :
                          model.id_tipo_matricula == 2 ? 8 : 9;

            var dt_inicio = model.dt_inicial_contrato > turma.dt_inicio_aula ? model.dt_inicial_contrato : turma.dt_inicio_aula;
            if ((bool)turmaExists["id_turma_ppt"])
            {
              //remove campos que não serão inseridos
              //comentando para funcionar o cadastro de turma personalizada
              //turmaExists.Remove("cd_turma");
              turmaExists.Remove("no_turma");

              //Busca a sigla do estagio
              var filtroCurso = new List<(string campo, object valor)> { new("cd_curso", turma.cd_curso) };
              var cursoExists = await SQLServerService.GetFirstByFields(source, "T_CURSO", filtroCurso);
              var filtroEstagio = new List<(string campo, object valor)> { new("cd_estagio", cursoExists["cd_estagio"]) };
              var estagioExists = await SQLServerService.GetFirstByFields(source, "T_ESTAGIO", filtroEstagio);

              //Busca turmas irmas existentes
              var ultima_turma_irma = await SQLServerService.GetList("T_TURMA", 1, 1, "cd_turma", true, null, "[cd_turma_ppt],[cd_curso]", $"[{cd_turma_original}],[{turma.cd_curso}]", source, SearchModeEnum.Equals, null, null);
              string complemento_nome = partes[1];
              complemento_nome = Regex.Replace(complemento_nome, @"\d+$", "");
              var nm_turma = ultima_turma_irma.success && ultima_turma_irma.data.Count > 0 ? (int)ultima_turma_irma.data[0]["nm_turma"] + 1 : 1;
              string novo_nome = $"PERSF/{estagioExists["no_estagio_abreviado"]}-{complemento_nome}{nm_turma}";

              // adiciona nome montado
              turmaExists.Add("no_turma", novo_nome);
              turmaExists.Remove("cd_turma_ppt");
              turmaExists.Add("cd_turma_ppt", cd_turma_original);
              turmaExists.Remove("cd_curso");
              turmaExists.Add("cd_curso", turma.cd_curso);
              turmaExists.Remove("cd_turma");
              turmaExists["id_turma_ppt"] = 0;
              turmaExists["nm_turma"] = nm_turma;

              var t_turma_insert = await SQLServerService.Insert("T_TURMA", turmaExists, source);
              
              if (!t_turma_insert.success)
              {
                string input = "PERSF/ESP1-SEG-17:00/21:00-2S/15-12";
                Match match = Regex.Match(input, @"-(\d+)$");

                if (match.Success)
                {
                    string lastNumber = match.Groups[1].Value;
                    nm_turma = int.Parse(lastNumber) + 1;
                    novo_nome = $"PERSF/{estagioExists["no_estagio_abreviado"]}-{complemento_nome}{nm_turma}";
                    turmaExists["no_turma"] = novo_nome;
                    turmaExists["nm_turma"] = nm_turma;

                    t_turma_insert = await SQLServerService.Insert("T_TURMA", turmaExists, source);
                    if (!t_turma_insert.success)
                    {
                        return BadRequest(t_turma_insert.error);
                    }
                }
              }

              var turmaCadastradaGet = await SQLServerService.GetList("T_TURMA", 1, 1, "cd_turma", true, null, null, "", source, SearchModeEnum.Equals, null, null);
              var turmaCadastrada = turmaCadastradaGet.data.First();
              int cdTurmaId = (int)turmaCadastrada["cd_turma"];

              var horario = await SQLServerService.GetList("T_HORARIO", 1, 10000000, "cd_horario", true, null, "[cd_registro]", $"[{cd_turma_original}]", source, SearchModeEnum.Equals, null, null);
              var turma_escola = await SQLServerService.GetList("T_TURMA_ESCOLA", 1, 10000000, "cd_turma_escola", true, null, "[cd_turma]", $"[{cd_turma_original}]", source, SearchModeEnum.Equals, null, null);
              var turma_professor = await SQLServerService.GetList("T_PROFESSOR_TURMA", 1, 10000000, "cd_turma", true, null, "[cd_turma]", $"[{cd_turma_original}]", source, SearchModeEnum.Equals, null, null);
              var programacao_turma = await SQLServerService.GetList("T_PROGRAMACAO_TURMA", 1, 10000000, "cd_programacao_turma", true, null, "[cd_turma]", $"[{cd_turma_original}]", source, SearchModeEnum.Equals, null, null);

              var feriado_desconsiderado = await SQLServerService.GetList("T_FERIADO_DESCONSIDERADO", 1, 10000000, "cd_feriado_desconsiderado", true, null, "[cd_turma]", $"[{cd_turma_original}]", source, SearchModeEnum.Equals, null, null);

              //vinculos para nova turma criada
              foreach (var item in horario.data)
              {
                item.Remove("cd_horario");
                item["cd_registro"] = cdTurmaId;
                var t_insert = await SQLServerService.InsertWithResult("T_HORARIO", item, source);
                if (!t_insert.success) continue;
                var cd_horario = t_insert.inserted["cd_horario"];
                
                foreach(var professor in turma_professor.data)
                {
                    var horario_professor_turma = new Dictionary<string, object> 
                    {
                        { "cd_horario", cd_horario },
                        { "cd_professor", professor["cd_professor"]}
                    };
                    var h_insert = await SQLServerService.Insert("T_HORARIO_PROFESSOR_TURMA", horario_professor_turma, source);
                }
              }
              if (turma_escola.success)
              {
                foreach (var item in turma_escola.data)
                {
                    item.Remove("cd_turma_escola");
                    item["cd_turma"] = cdTurmaId;
                    var t_insert = await SQLServerService.Insert("T_TURMA_ESCOLA", item, source);
                    if (!t_insert.success) continue;
                }
              }
              
              if (turma_professor.success)
              {
                  foreach (var item in turma_professor.data)
                  {
                    item.Remove("cd_professor_turma");
                    item["cd_turma"] = cdTurmaId;
                    var t_insert = await SQLServerService.Insert("T_PROFESSOR_TURMA", item, source);
                    if (!t_insert.success) continue;
                  }
              }
              
              if (programacao_turma.success)
              {
                  foreach (var item in programacao_turma.data)
                  {
                    item.Remove("cd_programacao_turma");
                    item["cd_turma"] = cdTurmaId;
                    var t_insert = await SQLServerService.Insert("T_PROGRAMACAO_TURMA", item, source);
                    if (!t_insert.success) continue;
                  }
              }
              
              if (feriado_desconsiderado.success)
              {
                  foreach (var item in feriado_desconsiderado.data)
                  {
                    item.Remove("cd_feriado_desconsiderado");
                    item["cd_turma"] = cdTurmaId;
                    var t_insert = await SQLServerService.Insert("T_FERIADO_DESCONSIDERADO", item, source);
                    if (!t_insert.success) continue;
                  }
              }
              //foreach (var cursoContratoId in cursosContrato)
              //{
              //  //cria vinculo entre aluno e turma
              //  var alunoTurmaDict = new Dictionary<string, object>
              //  {
              //    ["cd_aluno"] = model.cd_aluno,
              //    ["cd_turma"] = cdTurmaId,
              //    ["cd_contrato"] = cd_contrato,
              //    ["cd_situacao_aluno_turma"] = situacao_aluno,
              //    ["dt_matricula"] = model.dt_matricula_contrato?.ToString("yyyy-MM-ddTHH:mm:ss") ?? null,
              //    ["dt_inicio"] = dt_inicio.ToString("yyyy-MM-ddTHH:mm:ss"),
              //    ["nm_matricula_turma"] = nm_matricula,
              //    ["dt_movimento"] = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
              //    ["cd_curso_contrato"] = cursoContratoId,
              //    ["cd_curso"] = turma.cd_curso
              //  };
              //  var t_aluno_Result = await SQLServerService.Insert("T_ALUNO_TURMA", alunoTurmaDict, source);
              //  if (!t_aluno_Result.success) return BadRequest(t_aluno_Result.error);
              //}
              var alunoTurmaDict = new Dictionary<string, object>
              {
                  ["cd_aluno"] = model.cd_aluno,
                  ["cd_turma"] = cdTurmaId,
                  ["cd_contrato"] = cd_contrato,
                  ["cd_situacao_aluno_turma"] = situacao_aluno,
                  ["dt_matricula"] = model.dt_matricula_contrato?.ToString("yyyy-MM-ddTHH:mm:ss") ?? null,
                  ["dt_inicio"] = dt_inicio.ToString("yyyy-MM-ddTHH:mm:ss"),
                  ["nm_matricula_turma"] = nm_matricula,
                  ["dt_movimento"] = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                  ["cd_curso_contrato"] = cursosContrato[i],
                  ["cd_curso"] = turma.cd_curso
              };
              var t_aluno_Result = await SQLServerService.Insert("T_ALUNO_TURMA", alunoTurmaDict, source);
              if (!t_aluno_Result.success) return BadRequest(t_aluno_Result.error);

              var id_tipo_movimento = situacao_aluno == 1 ? 0
                                    : situacao_aluno == 8 ? 6
                                    : 10;
              //gera historico aluno
              //obtem ultimo historico para atualizar quantidade
              var ultimoHistorico = await SQLServerService.GetList("T_HISTORICO_ALUNO", 1, 1, "nm_sequencia", true, null, "[cd_aluno]", $"[{model.cd_aluno}]", source, SearchModeEnum.Equals, null, null);
              var sequencia_historico = 0;
              if (ultimoHistorico.success)
              {
                sequencia_historico = int.Parse(ultimoHistorico.data.FirstOrDefault()?["nm_sequencia"]?.ToString() ?? "0");
              }
              sequencia_historico += 1;

              var historicoAlunoDict = new Dictionary<string, object>
              {
                ["cd_aluno"] = model.cd_aluno,
                ["cd_turma"] = cdTurmaId,
                ["cd_contrato"] = cd_contrato,
                ["id_situacao_historico"] = situacao_aluno,
                ["cd_usuario"] = model.cd_usuario,
                ["dt_cadastro"] = DateTime.Now.Date,
                ["id_tipo_movimento"] = id_tipo_movimento,
                ["cd_produto"] = model.cd_produto_atual,
                ["dt_historico"] = dt_inicio,
                ["nm_sequencia"] = sequencia_historico
              };
              var t_Historico_Result = await SQLServerService.Insert("T_HISTORICO_ALUNO", historicoAlunoDict, source);
              if (!t_Historico_Result.success) return BadRequest(t_Historico_Result.error);
            }
            else
            {
              //validação aluno existente
              var filtrosAluno = new List<(string campo, object valor)> { new("cd_aluno", model.cd_aluno), new("cd_situacao_aluno_turma", 9) };
              var alunoExists = await SQLServerService.GetFirstByFields(source, "T_ALUNO_TURMA", filtrosAluno);

              if (alunoExists != null)
              {
                //foreach (var cursoContratoId in cursosContrato)
                //{
                //  //atualiza cd_contrato e situação aluno
                //  var aluno_atualizar = new Dictionary<string, object>
                //  {
                //    ["cd_contrato"] = cd_contrato,
                //    ["cd_situacao_aluno_turma"] = situacao_aluno,
                //    ["dt_matricula"] = model.dt_matricula_contrato?.ToString("yyyy-MM-ddTHH:mm:ss"),
                //    ["nm_matricula_turma"] = nm_matricula,
                //    ["cd_curso_contrato"] = cursoContratoId,
                //    ["dt_inicio"] = dt_inicio.ToString("yyyy-MM-ddTHH:mm:ss"),
                //  };
                //  var t_aluno_Result = await SQLServerService.Update("T_ALUNO_TURMA", aluno_atualizar, source, "cd_aluno", model.cd_aluno);
                //  if (!t_aluno_Result.success) return BadRequest(t_aluno_Result.error);
                //}
                                  //atualiza cd_contrato e situação aluno
                  var aluno_atualizar = new Dictionary<string, object>
                  {
                    ["cd_contrato"] = cd_contrato,
                    ["cd_situacao_aluno_turma"] = situacao_aluno,
                    ["dt_matricula"] = model.dt_matricula_contrato?.ToString("yyyy-MM-ddTHH:mm:ss"),
                    ["nm_matricula_turma"] = nm_matricula,
                    ["cd_curso_contrato"] = cursosContrato[i],
                    ["dt_inicio"] = dt_inicio.ToString("yyyy-MM-ddTHH:mm:ss"),
                  };
                  var t_aluno_Result = await SQLServerService.Update("T_ALUNO_TURMA", aluno_atualizar, source, "cd_aluno", model.cd_aluno);
                  if (!t_aluno_Result.success) return BadRequest(t_aluno_Result.error);
              }
              else
              {
                foreach (var cursoContratoId in cursosContrato)
                {
                  var alunoTurmaExists = await SQLServerService.GetFirstByFields(source, "T_ALUNO_TURMA", new List<(string campo, object valor)> { new("cd_aluno", model.cd_aluno), new("cd_curso", turma.cd_curso) });
                  if (alunoTurmaExists == null)
                  {
                    //cria vinculo entre aluno e turma
                    var alunoTurmaDict = new Dictionary<string, object>
                    {
                      ["cd_aluno"] = model.cd_aluno,
                      ["cd_turma"] = turma.cd_turma,
                      ["cd_contrato"] = cd_contrato,
                      ["cd_situacao_aluno_turma"] = situacao_aluno,
                      ["dt_matricula"] = model.dt_matricula_contrato?.ToString("yyyy-MM-ddTHH:mm:ss") ?? null,
                      ["dt_inicio"] = dt_inicio.ToString("yyyy-MM-ddTHH:mm:ss"),
                      ["nm_matricula_turma"] = nm_matricula,
                      ["dt_movimento"] = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                      ["cd_curso_contrato"] = cursoContratoId,
                      ["cd_curso"] = turma.cd_curso
                    };
                    var t_aluno_Result = await SQLServerService.Insert("T_ALUNO_TURMA", alunoTurmaDict, source);
                    if (!t_aluno_Result.success) return BadRequest(t_aluno_Result.error);
                  }
                  else
                  {
                    var aluno_atualizar = new Dictionary<string, object>
                    {
                      ["cd_contrato"] = cd_contrato,
                      ["cd_situacao_aluno_turma"] = situacao_aluno,
                      ["dt_matricula"] = model.dt_matricula_contrato?.ToString("yyyy-MM-ddTHH:mm:ss"),
                      ["nm_matricula_turma"] = nm_matricula,
                      ["cd_curso_contrato"] = cursoContratoId,
                      ["dt_inicio"] = dt_inicio.ToString("yyyy-MM-ddTHH:mm:ss"),
                    };
                    var t_aluno_Result = await SQLServerService.Update("T_ALUNO_TURMA", aluno_atualizar, source, "cd_aluno", model.cd_aluno);
                    if (!t_aluno_Result.success) return BadRequest(t_aluno_Result.error);

                  }
                }
              }
              var id_tipo_movimento = situacao_aluno == 1 ? 0
                                    : situacao_aluno == 8 ? 6
                                    : 10;
              //gera historico aluno
              //obtem ultimo historico para atualizar quantidade
              var ultimoHistorico = await SQLServerService.GetList("T_HISTORICO_ALUNO", 1, 1, "nm_sequencia", true, null, "[cd_aluno]", $"[{model.cd_aluno}]", source, SearchModeEnum.Equals, null, null);
              var sequencia_historico = 0;
              if (ultimoHistorico.success)
              {
                sequencia_historico = int.Parse(ultimoHistorico.data.FirstOrDefault()?["nm_sequencia"]?.ToString() ?? "0");
              }
              sequencia_historico += 1;

              var historicoAlunoDict = new Dictionary<string, object>
              {
                ["cd_aluno"] = model.cd_aluno,
                ["cd_turma"] = turma.cd_turma,
                ["cd_contrato"] = cd_contrato,
                ["id_situacao_historico"] = situacao_aluno,
                ["cd_usuario"] = model.cd_usuario,
                ["dt_cadastro"] = DateTime.Now.Date,
                ["id_tipo_movimento"] = id_tipo_movimento,
                ["cd_produto"] = model.cd_produto_atual,
                ["dt_historico"] = dt_inicio,
                ["nm_sequencia"] = sequencia_historico
              };
              var t_Historico_Result = await SQLServerService.Insert("T_HISTORICO_ALUNO", historicoAlunoDict, source);
              if (!t_Historico_Result.success) return BadRequest(t_Historico_Result.error);
            }
          }

          //Atualiza pipeline pela fila de matricula
          if (model.cd_fila_matricula != null)
          {
            //pegar fila de matricula por Id e pegar cd_contato para chegar em pipeline
            var filtrosfilaMatricula = new List<(string campo, object valor)> { new("cd_fila_matricula", model.cd_fila_matricula) };
            var filaExists = await SQLServerService.GetFirstByFields(source, "T_FILA_MATRICULA", filtrosfilaMatricula);
            if (filaExists != null)
            {
              var cd_contato = filaExists["cd_contato"];

              var filtrosPipeline = new List<(string campo, object valor)> { new("cd_etapa_pipeline", 5), new("cd_contato_pipeline ", cd_contato) };
              var pipelineExists = await SQLServerService.GetFirstByFields(source, "T_PIPELINE", filtrosPipeline);
              if (pipelineExists != null)
              {
                var cd_pipeline = pipelineExists["cd_pipeline"];

                var pipelineAtualizar = new Dictionary<string, object>
                {
                  ["id_posicao_pipeline"] = 5,
                  ["cd_etapa_pipeline"] = 5
                };
                var t_pipeline_update = await SQLServerService.Update("T_PIPELINE", pipelineAtualizar, source, "cd_pipeline", cd_pipeline);
                if (!t_pipeline_update.success) return BadRequest(t_pipeline_update.error);
              }
            }
          }
          else
          {
            //atualizar pipeline sem fila de matricula.
            //pega aluno por Id -> cd_pessoa -> pipeline cd_pessoa
            var filtrosAluno = new List<(string campo, object valor)> { new("cd_aluno", model.cd_aluno) };
            var alunoExists = await SQLServerService.GetFirstByFields(source, "T_ALUNO", filtrosAluno);
            if (alunoExists != null)
            {
              var cd_pessoa = alunoExists["cd_pessoa_aluno"];

              //pega todas as pipelines do usuario
              var pipelines_result = await SQLServerService.GetList("T_PIPELINE", null, "[cd_pessoa_pipeline]", "cd_pessoa", source, SearchModeEnum.Equals);
              if (pipelines_result.success)
              {
                //pega somente a pipeline que não for id_posicao_pipeline 5 ou 6
                var pipeline = pipelines_result.data.FirstOrDefault(x => x["id_posicao_pipeline"].ToString() != "5" && x["id_posicao_pipeline"].ToString() != "6");
                if (pipeline != null)
                {
                  var cd_pipeline = pipeline["cd_pipeline"];

                  var pipelineAtualizar = new Dictionary<string, object>
                  {
                    ["id_posicao_pipeline"] = 5,
                    ["cd_etapa_pipeline"] = 5
                  };
                  var t_pipeline_update = await SQLServerService.Update("T_PIPELINE", pipelineAtualizar, source, "cd_pipeline", cd_pipeline);
                  if (!t_pipeline_update.success) return BadRequest(t_pipeline_update.error);
                }
              }
            }
          }
        }



        var resultado = await BaixaAutomaticaBolsaAluno(int.Parse(cd_contrato.ToString()), source);

        // ===== GERAÇÃO AUTOMÁTICA DE TÍTULOS =====
        // TODO: Implementar geração automática de títulos baseada na sp_gerar_titulo_contrato
        // Por enquanto, mantemos a lógica original onde o frontend envia os títulos prontos
        // Quando ativar a geração automática, descomentar as linhas abaixo:
        //
        // var titulosGerados = await GerarTitulosContrato(model, int.Parse(nm_contrato.ToString()), source);
        //
        // Isso substituirá a necessidade do frontend enviar:
        // - model.TitulosTaxa
        // - model.TitulosMensalidade
        // - model.TitulosMaterial
        // Os títulos serão calculados automaticamente baseados nos parâmetros do contrato

        return ResponseDefault(new
        {
          cd_contrato = cd_contrato,

          erro = !resultado.success ? $"erro baixa automatica bolsa:{resultado.error}" : null,

          nm_contrato = nm_contrato

        });
      }
      return BadRequest(new
      {
        error = "Fonte de dados não configurada ou inativa."
      });
    }

    /// <summary>
    /// Valida se existe matrícula duplicada por produto/aluno (contratos SEM turma)
    /// Equivalente ao método existeMatriculaByProdutoAluno do SGF1
    /// </summary>
    [Authorize]
    [HttpPost("validar-matricula-duplicada")]
    public async Task<IActionResult> ValidarMatriculaDuplicada([FromBody] ValidarMatriculaDuplicadaModel model)
    {
      try
      {
        var schemaName = "T_Pessoa";
        if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
        var schema = _schemaRepository.GetSchemaByField("name", schemaName);
        var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
        var source = _sourceRepository.GetByField("description", schemaModel.Source);
        if (source == null || source.Active != true)
        {
          return BadRequest(new { error = "Fonte de dados não configurada ou inativa." });
        }

        // Para cada curso do contrato, verificar se existe conflito
        var conflitos = new List<object>();

        if (model.CursoContrato != null && model.CursoContrato.Any())
        {
          foreach (var cursoContrato in model.CursoContrato)
          {
            // Calcular data final se não fornecida
            var dt_final_calculada = model.dt_final_contrato;
            if (dt_final_calculada == null && model.cd_duracao.HasValue)
            {
              dt_final_calculada = await CalcularDataFinalContrato(cursoContrato.cd_curso, model.cd_duracao.Value, model.dt_inicial_contrato, source);
            }

            bool existeConflito = await VerificarMatriculaPorProdutoAluno(
              model.cd_produto_atual,
              model.cd_aluno,
              model.cd_pessoa_escola,
              model.dt_inicial_contrato,
              cursoContrato.cd_curso,
              model.cd_contrato_ignorar ?? 0,
              dt_final_calculada,
              model.cd_duracao ?? 0,
              source
            );

            if (existeConflito)
            {
              conflitos.Add(new
              {
                cd_curso = cursoContrato.cd_curso,
                conflito = true,
                mensagem = "Já existe matrícula para este curso/produto no período informado"
              });
            }
          }
        }
        else
        {
          // Calcular data final se não fornecida
          var dt_final_calculada = model.dt_final_contrato;
          if (dt_final_calculada == null && model.cd_duracao.HasValue)
          {
            dt_final_calculada = await CalcularDataFinalContrato(model.cd_curso_atual, model.cd_duracao.Value, model.dt_inicial_contrato, source);
          }

          // Validação para o curso atual apenas
          bool existeConflito = await VerificarMatriculaPorProdutoAluno(
            model.cd_produto_atual,
            model.cd_aluno,
            model.cd_pessoa_escola,
            model.dt_inicial_contrato,
            model.cd_curso_atual,
            model.cd_contrato_ignorar ?? 0,
            dt_final_calculada,
            model.cd_duracao ?? 0,
            source
          );

          if (existeConflito)
          {
            conflitos.Add(new
            {
              cd_curso = model.cd_curso_atual,
              conflito = true,
              mensagem = "Já existe matrícula para este curso/produto no período informado"
            });
          }
        }

        return Ok(new
        {
          temConflito = conflitos.Any(),
          conflitos = conflitos,
          mensagem = conflitos.Any()
            ? "Foram encontrados conflitos de matrícula"
            : "Não foram encontrados conflitos de matrícula"
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { error = "Erro interno do servidor", details = ex.Message });
      }
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> Put(MatriculaUpdateModel model)
    {
      var schemaName = "T_Pessoa";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);
      if (source != null && source.Active != null && source.Active == true)
      {
        //valida se matricula existe
        var filtrosContrato = new List<(string campo, object valor)> { new("cd_contrato", model.cd_contrato) };
        var matriculaExists = await SQLServerService.GetFirstByFields(source, "T_CONTRATO", filtrosContrato);
        var nm_matricula = matriculaExists["nm_matricula_contrato"];
        var cd_escola = matriculaExists["cd_pessoa_escola"];
        if (matriculaExists == null) return NotFound("contrato");

        if (matriculaExists["id_status_contrato"].ToString() == "1") return BadRequest("contrato cancelado nada poderá ser alterado");

        var dataVencimento = model.dt_vencimento_parcela_1;

        var matricula_dict = new Dictionary<string, object>
        {
          ["cd_tipo_financeiro"] = model.cd_tipo_financeiro,
          //["cd_produto_atual"] = model.cd_produto_atual,
          //["cd_curso_atual"] = model.cd_curso_atual,
          ["cd_regime_atual"] = model.cd_regime_atual,
          ["cd_duracao_atual"] = model.cd_duracao_atual,
          ["cd_pessoa_escola"] = model.cd_pessoa_escola,
          ["id_nf_servico"] = 0,
          ["id_ajuste_manual"] = 0,
          ["id_contrato_aula"] = 0,
          ["id_divida_primeira_parcela"] = 0,
          ["dt_vencimento_parcela_1"] = model.dt_vencimento_parcela_1?.ToString("yyyy-MM-ddTHH:mm:ss"),
          ["dt_vencimento_parcela_1_material"] = model.dt_vencimento_parcela_1_material?.ToString("yyyy-MM-ddTHH:mm:ss"),
          ["nm_dia_vcto"] = dataVencimento?.Day,
          ["nm_mes_vcto"] = dataVencimento?.Month,
          ["nm_ano_vcto"] = dataVencimento?.Year,
          ["nm_parcelas_mensalidade"] = model.nm_parcelas_mensalidade,
          ["pc_responsavel_contrato"] = (model.pc_responsavel_contrato ?? 0) == 0 ? 100 : Math.Round(model.pc_responsavel_contrato.Value, 2),
          ["pc_desconto_contrato"] = model.id_tipo_contrato == 1 ? 0m : Math.Round(model.pc_desconto_contrato ?? 0m, 4),
          //["vl_curso_contrato"] = Math.Round(model.vl_curso_contrato ?? 0m, 2),
          ["vl_matricula_contrato"] = 0m,
          ["vl_parcela_contrato"] = model.id_tipo_contrato == 1 ? 0m : Math.Round(model.vl_parcela_contrato ?? 0m, 2),
          ["vl_desconto_contrato"] = model.id_tipo_contrato == 1 ? 0m : Math.Round(model.vl_desconto_contrato ?? 0m, 2),
          ["vl_divida_contrato"] = 0m,
          ["vl_desc_primeira_parcela"] = 0m,
          ["vl_parcela_liquida"] = model.id_tipo_contrato == 1 ? 0m : Math.Round(model.vl_parcela_liquida ?? 0m, 2),
          ["vl_liquido_contrato"] = Math.Round(model.vl_liquido_contrato ?? 0m, 2),
          ["id_renegociacao"] = 0,
          ["id_venda_pacote"] = 0,
          ["pc_desconto_bolsa"] = model.pc_desconto_bolsa ?? 0m,
          ["vl_pre_matricula"] = 0m,
          ["id_liberar_certificado"] = 1,
          //["nm_mes_curso_inicial"] = model.nm_mes_curso_inicial,
          //["nm_ano_curso_inicial"] = model.nm_ano_curso_inicial,
          //["nm_mes_curso_final"] = model.nm_mes_curso_final,
          //["nm_ano_curso_final"] = model.nm_ano_curso_final,
          ["nm_arquivo_digitalizado"] = model.nm_arquivo_digitalizado,
          ["nm_parcelas_material"] = model.nm_parcelas_material,
          ["vl_parcela_material"] = Math.Round(model.vl_parcela_material ?? 0m, 2),
          ["vl_material_contrato"] = Math.Round(model.vl_material_contrato ?? 0m, 2),
          ["vl_parcela_liq_material"] = Math.Round(model.vl_parcela_liq_material ?? 0m, 2),
          ["pc_bolsa_material"] = model.pc_bolsa_material ?? 0m,
          ["vl_aula_hora"] = model.vl_aula_hora,
          ["pc_desconto_material"] = Math.Round(model.pc_desconto_material ?? 0m, 2),
          ["vl_liquido_material"] = Math.Round(model.vl_liquido_material ?? 0m, 2),
          ["vl_desconto_material"] = Math.Round(model.vl_desconto_material ?? 0m, 2),
          ["id_opcao_venda"] = model.id_opcao_venda,
          ["cd_tipo_financeiro_material"] = model.cd_tipo_financeiro_material,
          ["cd_pessoa_responsavel_material"] = model.cd_pessoa_responsavel_material,
          ["cd_pessoa_responsavel"] = model.cd_pessoa_responsavel,
          ["pc_responsavel_material"] = (model.pc_responsavel_material ?? 0m) == 0m ? 100m : Math.Round(model.pc_responsavel_material.Value, 2),
          ["id_status_contrato"] = 0,
          ["cd_nome_contrato"] = model.cd_nome_contrato,
          ["cd_fila_matricula"] = model.cd_fila_matricula
        };

        var titulosComBaixa = await SQLServerService.GetFirstByFields(source, "T_TITULO", new List<(string campo, object valor)> { ("cd_origem_titulo", model.cd_contrato), ("id_status_titulo", 2) });
        var titulosComCnab = await SQLServerService.GetFirstByFields(source, "T_TITULO", new List<(string campo, object valor)> { ("cd_origem_titulo", model.cd_contrato), ("id_status_cnab", 2) });
        var renegociacao = bool.Parse(matriculaExists["id_renegociacao"]?.ToString() ?? "0");
        var validacaoSemBaixaCnbRenegociacao = false;

        var filtroParametro = new List<(string campo, object valor)> { new("cd_pessoa_escola", model.cd_pessoa_escola) };
        var parametroExists = await SQLServerService.GetFirstByFields(source, "T_PARAMETRO", filtroParametro);
        if (parametroExists == null) return NotFound("parametros não encontratos para esta escola");
        var id_nro_contrato_automatico = parametroExists["id_nro_contrato_automatico"]?.ToString() ?? "0";
        var nm_nf_mercantil = (bool)parametroExists["id_emitir_nf_mercantil"] == true ? int.Parse(parametroExists["nm_nf_mercantil"].ToString()) : int.Parse(parametroExists["nm_nf_material"].ToString());
        var atualizarTurma = false;
        var atualizarPlanoConta = false;
        var atualizarComplemento = false;
        var turma_cursoAtual = await SQLServerService.GetFirstByFields(source, "T_TURMA", new List<(string campo, object valor)> { ("cd_curso", matriculaExists["cd_curso_atual"]) });
        // Sem Baixa e Sem Cnab e Sem renegociação
        if (titulosComBaixa == null && titulosComCnab == null && renegociacao == false)
        {
          validacaoSemBaixaCnbRenegociacao = true;

          AddIfNotExists(matricula_dict, "dt_inicial_contrato", model.dt_inicial_contrato.ToString("yyyy-MM-ddTHH:mm:ss"));
          AddIfNotExists(matricula_dict, "dt_final_contrato", model.dt_final_contrato?.ToString("yyyy-MM-ddTHH:mm:ss"));
          AddIfNotExists(matricula_dict, "dt_matricula_contrato", model.dt_matricula_contrato?.ToString("yyyy-MM-ddTHH:mm:ss"));
          AddIfNotExists(matricula_dict, "cd_produto_atual", model.cd_produto_atual);
          AddIfNotExists(matricula_dict, "cd_curso_atual", model.cd_curso_atual);
          AddIfNotExists(matricula_dict, "id_tipo_matricula", model.id_tipo_matricula);
          AddIfNotExists(matricula_dict, "cd_ano_escolar", model.cd_ano_escolar);
          AddIfNotExists(matricula_dict, "id_transferencia", model.id_transferencia);
          AddIfNotExists(matricula_dict, "id_retorno", model.id_retorno);
          if (id_nro_contrato_automatico == "0" && model.nm_matricula_contrato != null) AddIfNotExists(matricula_dict, "nm_matricula_contrato", model.nm_matricula_contrato);
          atualizarPlanoConta = true;
          atualizarComplemento = true;
          if (turma_cursoAtual == null) atualizarTurma = true;

          /*
       carga horária
       */
        }

        var aluno = await SQLServerService.GetFirstByFields(source, "T_ALUNO", new List<(string campo, object valor)> { new("cd_aluno", matriculaExists["cd_aluno"]) });
        var cd_pessoa_aluno = aluno["cd_pessoa_aluno"];
        var movimento_gerado = await SQLServerService.GetFirstByFields(source, "T_MOVIMENTO", new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa_aluno) });

        //Com movimento de venda de material gerado
        var possui_movimento = movimento_gerado == null ? false : true;
        if (possui_movimento)
        {
          AddIfNotExists(matricula_dict, "dt_inicial_contrato", model.dt_inicial_contrato.ToString("yyyy-MM-ddTHH:mm:ss"));
          AddIfNotExists(matricula_dict, "dt_final_contrato", model.dt_final_contrato?.ToString("yyyy-MM-ddTHH:mm:ss"));
          AddIfNotExists(matricula_dict, "dt_matricula_contrato", model.dt_matricula_contrato?.ToString("yyyy-MM-ddTHH:mm:ss"));
          AddIfNotExists(matricula_dict, "cd_ano_escolar", model.cd_ano_escolar);
          AddIfNotExists(matricula_dict, "id_transferencia", model.id_transferencia);
          AddIfNotExists(matricula_dict, "id_retorno", model.id_retorno);
          if (id_nro_contrato_automatico == "0" && model.nm_matricula_contrato != null) AddIfNotExists(matricula_dict, "nm_matricula_contrato", model.nm_matricula_contrato);
          atualizarPlanoConta = true;
          atualizarComplemento = true;
          if (turma_cursoAtual == null) atualizarTurma = true;

        }
        // Com Baixa e/ou Com Cnab e/ou Renegociação
        if (titulosComBaixa != null || titulosComCnab != null || renegociacao == true)
        {
          AddIfNotExists(matricula_dict, "dt_inicial_contrato", model.dt_inicial_contrato.ToString("yyyy-MM-ddTHH:mm:ss"));
          AddIfNotExists(matricula_dict, "dt_final_contrato", model.dt_final_contrato?.ToString("yyyy-MM-ddTHH:mm:ss"));
          AddIfNotExists(matricula_dict, "cd_ano_escolar", model.cd_ano_escolar);
          if (id_nro_contrato_automatico == "0" && model.nm_matricula_contrato != null) AddIfNotExists(matricula_dict, "nm_matricula_contrato", model.nm_matricula_contrato);
          AddIfNotExists(matricula_dict, "id_transferencia", model.id_transferencia);
          AddIfNotExists(matricula_dict, "id_retorno", model.id_retorno);
          atualizarComplemento = true;
          if (turma_cursoAtual == null) atualizarTurma = true;

        }
        // com turma
        if (turma_cursoAtual != null)
        {
          atualizarPlanoConta = true;
          AddIfNotExists(matricula_dict, "dt_inicial_contrato", model.dt_inicial_contrato.ToString("yyyy-MM-ddTHH:mm:ss"));
          AddIfNotExists(matricula_dict, "dt_final_contrato", model.dt_final_contrato?.ToString("yyyy-MM-ddTHH:mm:ss"));
          AddIfNotExists(matricula_dict, "dt_matricula_contrato", model.dt_matricula_contrato?.ToString("yyyy-MM-ddTHH:mm:ss"));
          AddIfNotExists(matricula_dict, "cd_ano_escolar", model.cd_ano_escolar);
          AddIfNotExists(matricula_dict, "id_transferencia", model.id_transferencia);
          AddIfNotExists(matricula_dict, "id_retorno", model.id_retorno);
          atualizarComplemento = true;
        }


        if (atualizarComplemento)
        {

          AddIfNotExists(matricula_dict, "tx_obs_contrato", model.tx_obs_contrato);
          AddIfNotExists(matricula_dict, "cd_nome_contrato", model.cd_nome_contrato);
          AddIfNotExists(matricula_dict, "id_tipo_data_inicio", model.id_tipo_data_inicio);
          AddIfNotExists(matricula_dict, "nm_dia_vcto_desconto", model.nm_dia_vcto_desconto);
          AddIfNotExists(matricula_dict, "nm_previsao_inicial", model.nm_previsao_inicial);
        }
        if (atualizarTurma && model.cd_turma != null)
        {
          var turmaExistente = await SQLServerService.GetFirstByFields(source, "T_TURMA", new List<(string campo, object valor)> { new("cd_turma", model.cd_turma) });
          if (turmaExistente != null)
          {
            var propAtualizar = new Dictionary<string, object>()
                        {
                            {"cd_curso",matriculaExists["cd_curso_atual"] }
                        };

            var atualizaTurma_result = await SQLServerService.Update("T_Turma", propAtualizar, source, "cd_turma", model.cd_turma);
            if (!atualizaTurma_result.success) BadRequest("erro ao atualizar turma");
          }
        }

        if (matricula_dict.ContainsKey("dt_inicial_contrato"))
        {
          string connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};MultipleActiveResultSets=True;";
          using (var connection = new SqlConnection(connectionString))
          {
            connection.Open();

            var query = @"
                        SELECT 1
                        FROM T_ALUNO_TURMA AT
                        INNER JOIN T_TURMA T
                            ON AT.cd_turma = T.cd_turma
                        INNER JOIN T_DIARIO_AULA DA
                            ON T.cd_turma = DA.cd_turma
                        WHERE AT.cd_contrato = @cd_contrato";

            using (var cmd = new SqlCommand(query, connection))
            {
              cmd.Parameters.AddWithValue("@cd_contrato", model.cd_contrato);

              var result = cmd.ExecuteScalar();
              bool existe = result != null;

              if (existe)
              {
                matricula_dict.Remove("dt_inicial_contrato");
              }
            }
          }
        }


        var matriculaResult = await SQLServerService.Update("T_CONTRATO", matricula_dict, source, "cd_contrato", model.cd_contrato);

        if (!matriculaResult.success) return BadRequest(matriculaResult.error);

        //aditamento cd_nome_contrato
        // Buscar aditamentos anteriores para gerar sequência
        var aditamentos_update = await SQLServerService.GetList("T_ADITAMENTO", null, "[cd_contrato]", $"[{model.cd_contrato}]", source);
        var sequencia_update = aditamentos_update.success && aditamentos_update.data != null ? aditamentos_update.data.Count + 1 : 1;

        var dict_aditamento = new Dictionary<string, object>
        {
          ["cd_contrato"] = model.cd_contrato,
          ["vl_aula_hora"] = 0,
          ["nm_titulos_aditamento"] = 0,
          ["cd_usuario"] = model.cd_usuario,
          ["vl_aditivo"] = 0,
          ["vl_parcela_titulo_aditamento"] = 0,
          ["id_ajuste_manual"] = 0,
          ["dt_aditamento"] = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
          ["cd_tipo_financeiro"] = model.cd_tipo_financeiro,
          ["nm_sequencia_aditamento"] = sequencia_update.ToString(),
          ["cd_nome_contrato"] = model.cd_nome_contrato,
          ["dt_inicio_aditamento"] = model.dt_inicio_aditamento,
          ["id_tipo_data_inicio"] = model.id_tipo_data_inicio ?? 0,
          ["nm_previsao_inicial"] = model.nm_previsao_inicial,
          ["nm_dia_vcto_desconto"] = model.nm_dia_vcto_desconto
        };
        var result_aditamento = await SQLServerService.Insert("T_ADITAMENTO", dict_aditamento, source);
        if (!result_aditamento.success) return BadRequest(result_aditamento.error);
        //cadastra ou atualiza taxa da matricula
        if (model.Taxa != null && model.Taxa.vl_matricula_taxa != null && model.Taxa.vl_matricula_taxa > 0 && atualizarPlanoConta)
        {
          var taxa_dict = new Dictionary<string, object>
                    {
                        { "cd_contrato", model.cd_contrato },
                        { "vl_matricula_taxa", model.Taxa.vl_matricula_taxa },
                        { "dt_vcto_taxa", model.Taxa.dt_vcto_taxa.ToString("yyyy-MM-ddTHH:mm:ss") },
                        { "nm_parcelas_taxa", model.Taxa.nm_parcelas_taxa },
                        { "pc_responsavel_taxa", model.Taxa.pc_responsavel_taxa },
                        { "cd_pessoa_responsavel_taxa", model.Taxa.cd_pessoa_responsavel_taxa },
                        { "cd_tipo_financeiro_taxa", model.Taxa.cd_tipo_financeiro_taxa },
                        { "cd_plano_conta_taxa", model.Taxa.cd_plano_conta_taxa },
                        { "vl_parcela_taxa", model.Taxa.vl_parcela_taxa }
                    };
          if (model.Taxa.cd_taxa_matricula == null)
          {
            var t_Taxa_matricula_Result = await SQLServerService.Insert("T_TAXA_MATRICULA", taxa_dict, source);
            if (!t_Taxa_matricula_Result.success) return BadRequest(t_Taxa_matricula_Result.error);
          }
          else
          {
            var t_Taxa_matricula_Result = await SQLServerService.Update("T_TAXA_MATRICULA", taxa_dict, source, "cd_taxa_matricula", model.Taxa.cd_taxa_matricula);
            if (!t_Taxa_matricula_Result.success) return BadRequest(t_Taxa_matricula_Result.error);
          }
        }

        //cadastra/atualiza
        //T_Desconto_Contrato
        if (!model.Descontos.IsNullOrEmpty() && atualizarPlanoConta)
        {
          //pegar descontos existes e verificar se estão sendo mandados? rota só para excluir desconto?
          await SQLServerService.Delete("T_DESCONTO_CONTRATO", "cd_contrato", model.cd_contrato.ToString(), source);

          foreach (var desconto in model.Descontos)
          {
            var dict = new Dictionary<string, object>
            {
              ["cd_contrato"] = model.cd_contrato,
              ["id_desconto_ativo"] = desconto.id_desconto_ativo,
              ["pc_desconto_contrato"] = desconto.pc_desconto,
              ["vl_desconto_contrato"] = desconto.vl_desconto,
              ["id_incide_baixa"] = desconto.id_incide_baixa,
              ["nm_parcela_ini"] = desconto.nm_parcela_inicial,
              ["nm_parcela_fim"] = desconto.nm_parcela_final,
              ["id_incide_matricula"] = desconto.id_incide_matricula,
              ["id_incide_material"] = desconto.id_incide_material,
              ["id_aditamento"] = desconto.id_aditamento,
              ["cd_tipo_desconto"] = desconto.cd_tipo_desconto
            };
            if (desconto.cd_desconto_contrato == null)
            {
              var t_Desconto_matricula_Result = await SQLServerService.Insert("T_DESCONTO_CONTRATO", dict, source);
              if (!t_Desconto_matricula_Result.success) return BadRequest(t_Desconto_matricula_Result.error);
            }
            else
            {
              var t_Desconto_matricula_Result = await SQLServerService.Update("T_DESCONTO_CONTRATO", dict, source, "cd_desconto_contrato", desconto.cd_desconto_contrato);
              if (!t_Desconto_matricula_Result.success) return BadRequest(t_Desconto_matricula_Result.error);
            }
          }
        }



        var cd_plano_conta_mat = parametroExists["cd_plano_conta_mat"] != null ? parametroExists["cd_plano_conta_mat"].ToString() : "0";
        var cd_plano_conta_mtr = parametroExists["cd_plano_conta_material"] != null ? parametroExists["cd_plano_conta_material"].ToString() : "0";
        var cd_plano_conta_tax = parametroExists["cd_plano_conta_tax"] != null ? parametroExists["cd_plano_conta_tax"].ToString() : "0";

        var responsavel = model.cd_pessoa_responsavel;
        if (string.IsNullOrEmpty(responsavel))
        {
          responsavel = model.cd_aluno.ToString();
        }

        if (!model.TitulosTaxa.IsNullOrEmpty() && atualizarPlanoConta)
        {
          var delete_taxas = await SQLServerService.DeleteByTwoFields("T_TITULO", "cd_origem_titulo", model.cd_contrato.ToString(), "dc_tipo_titulo", "TX", source);
          if (delete_taxas.success)
          {
            foreach (var titulo in model.TitulosTaxa)
            {
              var dictTitulo = new Dictionary<string, object>
              {
                ["cd_pessoa_empresa"] = cd_escola,
                ["cd_pessoa_titulo"] = titulo.cd_pessoa_titulo,
                ["cd_pessoa_responsavel"] = titulo.cd_pessoa_responsavel != 0 ? titulo.cd_pessoa_responsavel : responsavel,
                ["cd_local_movto"] = parametroExists["cd_local_movto"],
                ["dt_emissao_titulo"] = titulo.dt_emissao_titulo.ToString("yyyy-MM-ddTHH:mm:ss"),
                ["cd_origem_titulo"] = model.cd_contrato,
                ["dt_vcto_titulo"] = titulo.dt_vcto_titulo.ToString("yyyy-MM-ddTHH:mm:ss"),
                ["dh_cadastro_titulo"] = DateTime.Now.Date.ToString("yyyy-MM-ddTHH:mm:ss"),
                ["vl_titulo"] = titulo.vl_titulo,
                ["vl_saldo_titulo"] = titulo.vl_saldo_titulo,
                ["dc_tipo_titulo"] = "TX",
                ["dc_num_documento_titulo"] = titulo.dc_num_documento_titulo,
                ["nm_titulo"] = matriculaExists["nm_contrato"],
                ["nm_parcela_titulo"] = titulo.nm_parcela_titulo,
                ["cd_tipo_financeiro"] = titulo.cd_tipo_financeiro,
                ["id_status_titulo"] = 1,
                ["id_status_cnab"] = titulo.id_status_cnab,
                ["id_origem_titulo"] = 22,
                ["id_natureza_titulo"] = 1,
                ["vl_material_titulo"] = titulo.vl_material_titulo,
                ["pc_taxa_cartao"] = titulo.pc_taxa_cartao,
                ["nm_dias_cartao"] = titulo.nm_dias_cartao,
                ["id_cnab_contrato"] = titulo.id_cnab_contrato,
                ["vl_taxa_cartao"] = titulo.vl_taxa_cartao,
                ["cd_aluno"] = titulo.cd_aluno,
                ["pc_responsavel"] = titulo.pc_responsavel == null || titulo.pc_responsavel == 0 ? 100 : titulo.pc_responsavel,
                ["vl_mensalidade"] = titulo.vl_mensalidade,
                ["pc_bolsa"] = titulo.pc_bolsa,
                ["vl_bolsa"] = titulo.vl_bolsa,
                ["pc_desconto_mensalidade"] = titulo.pc_desconto_mensalidade,
                ["vl_desconto_mensalidade"] = titulo.vl_desconto_mensalidade,
                ["pc_bolsa_material"] = titulo.pc_bolsa_material,
                ["vl_bolsa_material"] = titulo.vl_bolsa_material,
                ["pc_desconto_material"] = titulo.pc_desconto_material,
                ["vl_desconto_material"] = titulo.vl_desconto_material,
                ["pc_desconto_total"] = titulo.pc_desconto_total,
                ["vl_desconto_total"] = titulo.vl_desconto_total,
                ["opcao_venda"] = titulo.opcao_venda,
                ["cd_curso"] = titulo.cd_curso
              };
              var t_titulo_Result = await SQLServerService.Insert("T_TITULO", dictTitulo, source);
              if (!t_titulo_Result.success) return BadRequest(t_titulo_Result.error);

              var t_tituloGet = await SQLServerService.GetList("T_TITULO", 1, 1, "cd_titulo", true, null, null, "", source, SearchModeEnum.Equals, null, null);
              var titulo_inserido = t_tituloGet.data.First();

              var id_origem_titulo = titulo_inserido["id_origem_titulo"]?.ToString() ?? "0";

              if (id_origem_titulo == "22" && titulo.dc_tipo_titulo == "TX")
              {
                //T_plano_titulo
                var dict_plano = new Dictionary<string, object>
                {
                  ["cd_titulo"] = titulo_inserido["cd_titulo"],
                  ["cd_plano_conta"] = cd_plano_conta_tax,
                  ["vl_plano_titulo"] = titulo.vl_titulo
                };
                var t_plano_titulo_Result = await SQLServerService.Insert("T_PLANO_TITULO", dict_plano, source);
                if (!t_plano_titulo_Result.success) return BadRequest(t_plano_titulo_Result.error);
              }

            }
          }

        }

        if (!model.TitulosMensalidade.IsNullOrEmpty() && atualizarPlanoConta)
        {
          var delete_mensalidades = await SQLServerService.DeleteByTwoFields("T_TITULO", "cd_origem_titulo", model.cd_contrato.ToString(), "dc_tipo_titulo", model.TitulosMensalidade.First().dc_tipo_titulo, source);
          if (delete_mensalidades.success)
          {
            foreach (var titulo in model.TitulosMensalidade)
            {
              var dictTitulo = new Dictionary<string, object>
              {
                ["cd_pessoa_empresa"] = cd_escola,
                ["cd_pessoa_titulo"] = titulo.cd_pessoa_titulo,
                ["cd_pessoa_responsavel"] = titulo.cd_pessoa_responsavel != 0 ? titulo.cd_pessoa_responsavel : responsavel,
                ["cd_local_movto"] = parametroExists["cd_local_movto"],
                ["dt_emissao_titulo"] = titulo.dt_emissao_titulo.ToString("yyyy-MM-ddTHH:mm:ss"),
                ["cd_origem_titulo"] = model.cd_contrato,
                ["dt_vcto_titulo"] = titulo.dt_vcto_titulo.ToString("yyyy-MM-ddTHH:mm:ss"),
                ["dh_cadastro_titulo"] = DateTime.Now.Date.ToString("yyyy-MM-ddTHH:mm:ss"),
                ["vl_titulo"] = titulo.vl_titulo,
                ["vl_saldo_titulo"] = titulo.vl_saldo_titulo,
                ["dc_tipo_titulo"] = titulo.dc_tipo_titulo,
                ["dc_num_documento_titulo"] = titulo.dc_num_documento_titulo,
                ["nm_titulo"] = matriculaExists["nm_contrato"],
                ["nm_parcela_titulo"] = titulo.nm_parcela_titulo,
                ["cd_tipo_financeiro"] = titulo.cd_tipo_financeiro,
                ["id_status_titulo"] = 1,
                ["id_status_cnab"] = titulo.id_status_cnab,
                ["id_origem_titulo"] = 22,
                ["id_natureza_titulo"] = 1,
                ["vl_material_titulo"] = titulo.vl_material_titulo,
                ["pc_taxa_cartao"] = titulo.pc_taxa_cartao,
                ["nm_dias_cartao"] = titulo.nm_dias_cartao,
                ["id_cnab_contrato"] = titulo.id_cnab_contrato,
                ["vl_taxa_cartao"] = titulo.vl_taxa_cartao,
                ["cd_aluno"] = titulo.cd_aluno,
                ["pc_responsavel"] = titulo.pc_responsavel == null || titulo.pc_responsavel == 0 ? 100 : titulo.pc_responsavel,
                ["vl_mensalidade"] = titulo.vl_mensalidade,
                ["pc_bolsa"] = titulo.pc_bolsa,
                ["vl_bolsa"] = titulo.vl_bolsa,
                ["pc_desconto_mensalidade"] = titulo.pc_desconto_mensalidade,
                ["vl_desconto_mensalidade"] = titulo.vl_desconto_mensalidade,
                ["pc_bolsa_material"] = titulo.pc_bolsa_material,
                ["vl_bolsa_material"] = titulo.vl_bolsa_material,
                ["pc_desconto_material"] = titulo.pc_desconto_material,
                ["vl_desconto_material"] = titulo.vl_desconto_material,
                ["pc_desconto_total"] = titulo.pc_desconto_total,
                ["vl_desconto_total"] = titulo.vl_desconto_total,
                ["opcao_venda"] = titulo.opcao_venda,
                ["cd_curso"] = titulo.cd_curso
              };
              var t_titulo_Result = await SQLServerService.Insert("T_TITULO", dictTitulo, source);
              if (!t_titulo_Result.success) return BadRequest(t_titulo_Result.error);

              var t_tituloGet = await SQLServerService.GetList("T_TITULO", 1, 1, "cd_titulo", true, null, null, "", source, SearchModeEnum.Equals, null, null);
              var titulo_inserido = t_tituloGet.data.First();

              var id_origem_titulo = titulo_inserido["id_origem_titulo"]?.ToString() ?? "0";

              if (id_origem_titulo == "22" && titulo.dc_tipo_titulo == "ME")
              {
                //T_plano_titulo
                var dict_plano = new Dictionary<string, object>
                {
                  ["cd_titulo"] = titulo_inserido["cd_titulo"],
                  ["cd_plano_conta"] = cd_plano_conta_mat,
                  ["vl_plano_titulo"] = titulo.vl_mensalidade
                };
                var t_plano_titulo_Result = await SQLServerService.Insert("T_PLANO_TITULO", dict_plano, source);
                if (!t_plano_titulo_Result.success) return BadRequest(t_plano_titulo_Result.error);
              }

              if (id_origem_titulo == "22" && titulo.dc_tipo_titulo == "ME" && titulo.vl_material_titulo > 0)
              {
                //T_plano_titulo
                var dict_plano = new Dictionary<string, object>
                {
                  ["cd_titulo"] = titulo_inserido["cd_titulo"],
                  ["cd_plano_conta"] = cd_plano_conta_mtr,
                  ["vl_plano_titulo"] = titulo.vl_material_titulo
                };
                var t_plano_titulo_Result = await SQLServerService.Insert("T_PLANO_TITULO", dict_plano, source);
                if (!t_plano_titulo_Result.success) return BadRequest(t_plano_titulo_Result.error);
              }
            }
          }

        }

        if (model.id_tipo_contrato != 2)
        {
          //T_titulo_Material
          if (!model.TitulosMaterial.IsNullOrEmpty() && atualizarPlanoConta)
          {
            var delete_materiais = await SQLServerService.DeleteByTwoFields("T_TITULO", "cd_origem_titulo", model.cd_contrato.ToString(), "dc_tipo_titulo", "MT", source);
            if (delete_materiais.success)
            {
              foreach (var titulo in model.TitulosMaterial)
              {
                var dictTitulo = new Dictionary<string, object>
                {
                  ["cd_pessoa_empresa"] = cd_escola,
                  ["cd_pessoa_titulo"] = titulo.cd_pessoa_titulo,
                  ["cd_pessoa_responsavel"] = titulo.cd_pessoa_responsavel != 0 ? titulo.cd_pessoa_responsavel : responsavel,

                  ["cd_local_movto"] = parametroExists["cd_local_movto"],
                  ["dt_emissao_titulo"] = titulo.dt_emissao_titulo.ToString("yyyy-MM-ddTHH:mm:ss"),

                  ["cd_origem_titulo"] = model.cd_contrato,
                  ["dt_vcto_titulo"] = titulo.dt_vcto_titulo.ToString("yyyy-MM-ddTHH:mm:ss"),
                  ["dh_cadastro_titulo"] = DateTime.Now.Date,
                  ["vl_titulo"] = titulo.vl_titulo,
                  ["vl_saldo_titulo"] = titulo.vl_saldo_titulo,
                  ["dc_tipo_titulo"] = "MT",
                  ["dc_num_documento_titulo"] = titulo.dc_num_documento_titulo,
                  ["nm_titulo"] = matriculaExists["nm_contrato"],
                  ["nm_parcela_titulo"] = titulo.nm_parcela_titulo,
                  ["cd_tipo_financeiro"] = titulo.cd_tipo_financeiro,
                  ["id_status_titulo"] = 1,
                  ["id_status_cnab"] = titulo.id_status_cnab,
                  ["id_origem_titulo"] = 22,
                  ["id_natureza_titulo"] = 1,
                  ["vl_material_titulo"] = titulo.vl_material_titulo,
                  ["pc_taxa_cartao"] = titulo.pc_taxa_cartao,
                  ["nm_dias_cartao"] = titulo.nm_dias_cartao,
                  ["id_cnab_contrato"] = titulo.id_cnab_contrato,
                  ["vl_taxa_cartao"] = titulo.vl_taxa_cartao,
                  ["cd_aluno"] = titulo.cd_aluno,
                  ["pc_responsavel"] = titulo.pc_responsavel == null || titulo.pc_responsavel == 0 ? 100 : titulo.pc_responsavel,
                  ["vl_mensalidade"] = titulo.vl_mensalidade,
                  ["pc_bolsa"] = titulo.pc_bolsa,
                  ["vl_bolsa"] = titulo.vl_bolsa,
                  ["pc_desconto_mensalidade"] = titulo.pc_desconto_mensalidade,
                  ["vl_desconto_mensalidade"] = titulo.vl_desconto_mensalidade,
                  ["pc_bolsa_material"] = titulo.pc_bolsa_material,
                  ["vl_bolsa_material"] = titulo.vl_bolsa_material,
                  ["pc_desconto_material"] = titulo.pc_desconto_material,
                  ["vl_desconto_material"] = titulo.vl_desconto_material,
                  ["pc_desconto_total"] = titulo.pc_desconto_total,
                  ["vl_desconto_total"] = titulo.vl_desconto_total,
                  ["opcao_venda"] = titulo.opcao_venda,
                  ["cd_curso"] = titulo.cd_curso
                };
                var t_titulo_Result = await SQLServerService.Insert("T_TITULO", dictTitulo, source);
                if (!t_titulo_Result.success) return BadRequest(t_titulo_Result.error);
                var titulo_inseridoGet = await SQLServerService.GetList("T_TITULO", 1, 1, "cd_titulo", true, null, null, "", source, SearchModeEnum.Equals, null, null);
                var titulo_inserido = titulo_inseridoGet.data.First();

                var id_origem_titulo = titulo_inserido["id_origem_titulo"]?.ToString() ?? "0";

                if (id_origem_titulo == "22" && titulo.dc_tipo_titulo == "MT")
                {
                  //T_plano_titulo
                  var dict_plano = new Dictionary<string, object>
                  {
                    ["cd_titulo"] = titulo_inserido["cd_titulo"],
                    ["cd_plano_conta"] = cd_plano_conta_mtr,
                    ["vl_plano_titulo"] = titulo.vl_titulo
                  };
                  var t_plano_titulo_Result = await SQLServerService.Insert("T_PLANO_TITULO", dict_plano, source);
                  if (!t_plano_titulo_Result.success) return BadRequest(t_plano_titulo_Result.error);
                }
              }
            }
          }
        }

        //Aditamentos
        if (!model.Aditamentos.IsNullOrEmpty() && atualizarPlanoConta)
        {
          var cd_pessoa_responsavel = matriculaExists["cd_pessoa_responsavel"];
          var cd_tipo_financeiro = matriculaExists["cd_tipo_financeiro"];
          var ultimo_titulo_contratoGet = await SQLServerService.GetList("T_TITULO", 1, 1, "cd_titulo", true, null, "[cd_contrato]", $"[{model.cd_contrato}]", source, SearchModeEnum.Equals, null, null);
          var ultimo_titulo_contrato = ultimo_titulo_contratoGet.data.FirstOrDefault();
          //Aditamentos
          foreach (var ad in model.Aditamentos)
          {
            var dict = new Dictionary<string, object>
            {
              ["cd_contrato"] = model.cd_contrato,
              ["dt_aditamento"] = ad.dt_aditamento?.ToString("yyyy-MM-ddTHH:mm:ss"),
              ["dt_inicio_aditamento"] = ad.dt_inicio_aditamento?.ToString("yyyy-MM-ddTHH:mm:ss"),
              ["dt_vcto_aditamento"] = ad.dt_vcto_aditamento?.ToString("yyyy-MM-ddTHH:mm:ss"),
              ["dt_vencto_inicial"] = ad.dt_vencto_inicial?.ToString("yyyy-MM-ddTHH:mm:ss"),
              ["cd_nome_contrato"] = ad.cd_nome_contrato,
              ["id_tipo_aditamento"] = ad.id_tipo_aditamento,
              ["nm_titulos_aditamento"] = ad.nm_titulos_aditamento,
              ["vl_aditivo"] = ad.vl_aditivo,
              ["vl_saldo_aberto"] = ad.vl_saldo_aberto,
              ["vl_anterior"] = ad.vl_anterior,
              ["cd_tipo_financeiro"] = ad.cd_tipo_financeiro,
              ["vl_parcela_titulo_aditamento"] = ad.vl_parcela_titulo_aditamento,
              ["tx_obs_aditamento"] = ad.tx_obs_aditamento,
              ["id_status_renegociacao"] = 0
            };


            int? cd_aditamento = ad.cd_aditamento;
            if (ad.cd_aditamento == null)
            {
              // Buscar todos os aditamentos anteriores do contrato para gerar sequência contínua
              var aditamentos_geral = await SQLServerService.GetList("T_ADITAMENTO", null, "[cd_contrato]", $"[{model.cd_contrato}]", source);
              var sequencia_geral = aditamentos_geral.success && aditamentos_geral.data != null ? aditamentos_geral.data.Count + 1 : 1;
              dict["nm_sequencia_aditamento"] = sequencia_geral.ToString();

              var t_aditamento_Result = await SQLServerService.InsertWithResult("T_ADITAMENTO", dict, source);
              if (!t_aditamento_Result.success) continue;
              cd_aditamento = int.Parse(t_aditamento_Result.inserted["cd_aditamento"].ToString());

              //'pc_bolsa','dt_comunicado_bolsa','dc_validade_bolsa','cd_motivo_bolsa'
              if (ad.pc_bolsa != null && ad.dt_comunicado_bolsa != null && ad.dc_validade_bolsa != null && ad.cd_motivo_bolsa != null)
              {
                var dict_bolsa = new Dictionary<string, object>
                {
                  ["cd_aditamento"] = cd_aditamento,
                  ["pc_bolsa"] = ad.pc_bolsa,
                  ["dt_comunicado_bolsa"] = ad.dt_comunicado_bolsa?.ToString("yyyy-MM-ddTHH:mm:ss"),
                  ["dc_validade_bolsa"] = ad.dc_validade_bolsa,
                  ["cd_motivo_bolsa"] = ad.cd_motivo_bolsa
                };
                var t_aditamento_bolsa_Result = await SQLServerService.Insert("T_ADITAMENTO_BOLSA", dict_bolsa, source);
                if (!t_aditamento_bolsa_Result.success) continue;
              }
            }
            else
            {
              var t_aditamento_Result = await SQLServerService.Update("T_ADITAMENTO", dict, source, "cd_aditamento", ad.cd_aditamento);
              if (!t_aditamento_Result.success) continue;

              var filtros_bolsa = new List<(string campo, object valor)> { new("cd_aditamento", ad.cd_aditamento.ToString()) };
              var t_aditamento_bolsa_result = await SQLServerService.GetFirstByFields(source, "T_ADITAMENTO_BOLSA", filtros_bolsa);
              var dict_bolsa = new Dictionary<string, object>
              {
                ["cd_aditamento"] = ad.cd_aditamento,
                ["pc_bolsa"] = ad.pc_bolsa,
                ["dt_comunicado_bolsa"] = ad.dt_comunicado_bolsa?.ToString("yyyy-MM-ddTHH:mm:ss"),
                ["dc_validade_bolsa"] = ad.dc_validade_bolsa,
                ["cd_motivo_bolsa"] = ad.cd_motivo_bolsa
              };
              if (t_aditamento_bolsa_result == null)
              {
                var t_aditamento_bolsa_Result = await SQLServerService.Insert("T_ADITAMENTO_BOLSA", dict_bolsa, source);
                if (!t_aditamento_bolsa_Result.success) continue;
              }
              else
              {
                var cd_aditamento_bolsa = t_aditamento_bolsa_result["cd_aditamento_bolsa"];
                var t_aditamento_bolsa_Result = await SQLServerService.Update("T_ADITAMENTO_BOLSA", dict_bolsa, source, "cd_aditamento_bolsa", cd_aditamento_bolsa);
                if (!t_aditamento_bolsa_Result.success) continue;
              }
            }

            //adiciona titulos para adicionar parcelas e adicionar parcelas material
            if (ad.id_tipo_aditamento == 5 || ad.id_tipo_aditamento == 8)
            {
              var dictTitulo = new Dictionary<string, object>
                            {
                                {"cd_origem_titulo",cd_aditamento },
                                { "cd_pessoa_empresa",  cd_escola},
                                { "cd_pessoa_titulo", null },
                                { "cd_pessoa_responsavel", cd_pessoa_responsavel },
                                { "cd_local_movto",  ultimo_titulo_contrato["cd_local_movto"]??0},
                                { "dt_emissao_titulo",  DateTime.Now.Date.ToString("yyyy-MM-ddTHH:mm:ss") },
                                { "dt_vcto_titulo", ad.dt_vcto_aditamento?.ToString("yyyy-MM-ddTHH:mm:ss") ?? DateTime.Now.Date.ToString("yyyy-MM-ddTHH:mm:ss") },
                                { "vl_titulo", ad.vl_parcela_titulo_aditamento },
                                { "vl_saldo_titulo", ad.vl_saldo_aberto },
                                { "cd_tipo_financeiro", cd_tipo_financeiro },
                                { "id_status_cnab", 0 },
                                { "vl_multa_titulo", 0 },
                                { "vl_juros_titulo", 0 },
                                { "vl_desconto_titulo", 0 },
                                { "vl_liquidacao_titulo", 0 },
                                { "vl_multa_liquidada", 0 },
                                { "vl_juros_liquidado", 0 },
                                { "vl_desconto_juros", 0 },
                                { "vl_desconto_multa", 0 },
                                { "pc_juros_titulo", 0 },
                                { "vl_material_titulo", 0 },
                                { "vl_abatimento", 0 },
                                { "vl_desconto_contrato", 0 },
                                { "pc_taxa_cartao", 0 },
                                { "nm_dias_cartao", 0 },
                                { "id_cnab_contrato",0 },
                                { "vl_taxa_cartao", 0 },
                                { "id_origem_titulo",22 },
                                { "id_natureza_titulo", 1 },
                                { "nm_parcela_titulo",ad.nm_titulos_aditamento }
                            };
              var t_titulo_Result = await SQLServerService.Insert("T_TITULO", dictTitulo, source);
              if (!t_titulo_Result.success) return BadRequest(t_titulo_Result.error);
            }
          }

        }


        //Venda material
        if (!model.VendasMaterial.IsNullOrEmpty())
        {
          var cd_curso_numero = 0;
          var estoque_ok = true;
          foreach (var venda in model.VendasMaterial)
          {
            if (venda.cd_curso != cd_curso_numero)
            {
              nm_nf_mercantil++;
              cd_curso_numero = venda.cd_curso;
            }
            // Validação conforme procedure: verificar se curso está vinculado ao contrato
            if (venda.cd_curso == null || venda.cd_curso == 0)
            {
              return BadRequest("Parâmetro Curso não informado.");
            }

            var curso_contrato = await SQLServerService.GetFirstByFields(source, "T_CURSO_CONTRATO",
                new List<(string campo, object valor)> { new("cd_curso", venda.cd_curso), new("cd_contrato", model.cd_contrato) });

            if (curso_contrato == null)
            {
              return BadRequest("Favor salvar a alteração do curso primeiro para poder prosseguir com a geração da venda de material.");
            }

            // Validação da modalidade/regime conforme procedure
            var contrato = await SQLServerService.GetFirstByFields(source, "T_CONTRATO",
                new List<(string campo, object valor)> { new("cd_contrato", model.cd_contrato) });

            var cd_regime = model.cd_regime_atual;

            if (cd_regime == null)
            {
              return BadRequest("A modalidade do contrato não foi definida. Para vincular a venda de material didático esta informação é necessária.");
            }

            var regime = await SQLServerService.GetFirstByFields(source, "T_REGIME",
                new List<(string campo, object valor)> { new("cd_regime", cd_regime) });

            var no_regime_abreviado = regime?["no_regime_abreviado"]?.ToString();

            if (string.IsNullOrEmpty(no_regime_abreviado))
            {
              return BadRequest("A modalidade do contrato não foi definida. Para vincular a venda de material didático esta informação é necessária.");
            }

            var item_escola = await SQLServerService.GetFirstByFields(source, "T_ITEM_ESCOLA", new List<(string campo, object valor)> { new("cd_item", venda.cd_item), new("cd_pessoa_escola", cd_escola) });

            var item = await SQLServerService.GetFirstByFields(source, "T_ITEM", new List<(string campo, object valor)> { new("cd_item", venda.cd_item) });

            //não gerar venda se não ha estoque para livro ou apostila
            if (venda.venda && !estoque_ok) continue;

            // Verificação de movimento existente conforme procedure
            List<(string campo, object valor)> filtroMovimento;
            var id_normal = contrato?["id_tipo_contrato"]?.ToString() == "0"; // Matricula normal

            if (id_normal)
            {
              // Para matrículas normais, verificar sem o curso
              filtroMovimento = new List<(string campo, object valor)>
                    {
                        new("id_origem_movimento", 22),
                        new("cd_origem_movimento", model.cd_contrato),
                        new("id_venda_futura", venda.venda ? 0 : 1),
                        new("id_material_didatico", 1)
                    };
            }
            else
            {
              // Para outras matrículas, verificar com o curso
              filtroMovimento = new List<(string campo, object valor)>
                    {
                        new("id_origem_movimento", 22),
                        new("cd_origem_movimento", model.cd_contrato),
                        new("cd_curso", venda.cd_curso),
                        new("id_venda_futura", venda.venda ? 0 : 1),
                        new("id_material_didatico", 1)
                    };
            }

            var movimento_existente = await SQLServerService.GetFirstByFields(source, "T_MOVIMENTO", filtroMovimento);

            // Verificar se já existe nota sem curso definido (conforme procedure)
            var movimento_sem_curso = await SQLServerService.GetFirstByFields(source, "T_MOVIMENTO",
                new List<(string campo, object valor)>
                {
                        new("id_origem_movimento", 22),
                        new("cd_origem_movimento", model.cd_contrato),
                        new("id_venda_futura", venda.venda ? 0 : 1),
                        new("id_material_didatico", 1),
                        new("cd_curso", DBNull.Value)
                });

            if (movimento_sem_curso != null)
            {
              var nm_movimento = movimento_sem_curso["nm_movimento"];
              var id_nf = movimento_sem_curso["id_nf"];
              var tipoDoc = (bool)id_nf ? "Nota Fiscal" : "Movimento";
              return BadRequest($"Não foi definido o curso no {tipoDoc}, já existente com o número {nm_movimento}");
            }

            if (movimento_existente != null)
            {
              // Conforme procedure, verificar se precisa gerar novos itens ou se já está completo
              // Por ora, vamos permitir o processamento se o movimento já existe
              // mas verificar se está completo conforme a lógica da procedure
            }

            var cd_tipo_nota_fiscal = parametroExists["cd_tipo_nf_material"];
            var tipo_nota_fiscal = await SQLServerService.GetFirstByFields(source, "t_tipo_nota_fiscal", new List<(string campo, object valor)> { new("cd_tipo_nota_fiscal", cd_tipo_nota_fiscal) });
            var dc_cfop = tipo_nota_fiscal?["dc_CFOP"];
            // Calcular CFOP baseado nos estados (conforme procedure)
            var cfopCalculado = await VerificaEstadoEscAluno(Convert.ToInt32(cd_escola), Convert.ToInt32(cd_pessoa_aluno), (int)TipoMovimentoEnum.SERVICO, source);
            var dc_cfop_final = cfopCalculado;

            var tx_obs_fiscal = tipo_nota_fiscal?["tx_obs_tipo_nota"];
            var cd_cfop = tipo_nota_fiscal?["cd_cfop"];
            var cd_movimento = 0;
            Dictionary<string, object>? movimento = null;
            if (movimento_existente == null)
            {
              // Buscar o responsável do contrato (conforme a procedure)
              var cd_responsavel = contrato?["cd_pessoa_responsavel"];

              // Buscar tipo financeiro (conforme procedure: 'Titulo')
              var tipoFinanceiro = await SQLServerService.GetFirstByFields(source, "T_TIPO_FINANCEIRO", new List<(string campo, object valor)> { new("dc_tipo_financeiro", "Titulo") });
              var cd_tipo_financeiro = tipoFinanceiro?["cd_tipo_financeiro"] ?? 3;

              //movimento
              var movimento_dict = new Dictionary<string, object>
                        {
                            {"cd_origem_movimento",model.cd_contrato },
                            { "cd_pessoa_empresa", cd_escola},
                            { "cd_pessoa", cd_responsavel ?? cd_pessoa_aluno}, // Usar responsável conforme procedure
                            { "cd_aluno", model.cd_aluno},
                            { "cd_politica_comercial", parametroExists["cd_politica_comercial_nf"]},
                            { "cd_tipo_financeiro", cd_tipo_financeiro },
                            { "id_tipo_movimento", 2 },
                            { "nm_movimento", nm_nf_mercantil},
                            { "dc_serie_movimento", (bool)parametroExists["id_emitir_nf_mercantil"] ? parametroExists["dc_serie_nf_mercantil"] ?? "1" : (venda.venda ? "M" : "F") },
                            { "dt_emissao_movimento", DateTime.Now.Date.ToString("yyyy-MM-ddTHH:mm:ss") ?? DateTime.Now.Date.ToString("yyyy-MM-ddTHH:mm:ss") },
                            { "dt_vcto_movimento", DateTime.Now.Date.ToString("yyyy-MM-ddTHH:mm:ss") ?? DateTime.Now.Date.ToString("yyyy-MM-ddTHH:mm:ss") },
                            { "dt_mov_movimento", DateTime.Now.Date.ToString("yyyy-MM-ddTHH:mm:ss") ?? DateTime.Now.Date.ToString("yyyy-MM-ddTHH:mm:ss") },
                            { "pc_acrescimo",  0 },
                            { "vl_acrescimo",  0 },
                            { "pc_desconto",  0 },
                            { "vl_desconto", 0 },
                            { "id_nf", parametroExists["id_emitir_nf_mercantil"]},
                            { "id_status_nf", 1 }, // Conforme procedure
                            { "id_nf_escola", parametroExists["id_emitir_nf_mercantil"]},
                            { "vl_base_calculo_ICMS_nf", 0 }, // Será calculado pelos itens
                            { "vl_base_calculo_PIS_nf", 0 },
                            { "vl_base_calculo_COFINS_nf", 0},
                            { "vl_base_calculo_IPI_nf", 0},
                            { "vl_base_calculo_ISS_nf", 0},
                            { "vl_ICMS_nf", 0 }, // Será calculado pelos itens
                            { "vl_PIS_nf", 0 },
                            { "vl_COFINS_nf", 0},
                            { "vl_IPI_nf", 0 },
                            { "vl_ISS_nf", 0 },
                            { "pc_aliquota_aproximada", 0 },
                            { "vl_aproximado", 0 },
                            { "id_exportado", 0 },
                            { "id_importacao_xml", 0 },
                            { "id_material_didatico", 1 },
                            { "id_venda_futura", venda.venda ? 0 : 1 },
                            { "id_origem_movimento", 22 },
                            { "nm_nfe", venda.venda ? nm_nf_mercantil : (object)DBNull.Value },
                            { "cd_curso", venda.cd_curso },
                            { "tx_obs_fiscal", tx_obs_fiscal},
                            { "cd_tipo_nota_fiscal", (bool)parametroExists["id_emitir_nf_mercantil"] ? parametroExists["cd_tipo_nf_material"] : (object)DBNull.Value},
                            { "cd_cfop_nf", (bool)parametroExists["id_emitir_nf_mercantil"] ? cd_cfop : (object)DBNull.Value},
                            { "dc_cfop_nf", (bool)parametroExists["id_emitir_nf_mercantil"] ? dc_cfop_final : (object)DBNull.Value },
                            { "dc_key_nfe", "" } // Conforme procedure
                        };
              var t_movimento_Result = await SQLServerService.Insert("T_MOVIMENTO", movimento_dict, source);
              if (!t_movimento_Result.success) return BadRequest(t_movimento_Result.error);

              var movimento_inseridoGet = await SQLServerService.GetList("T_MOVIMENTO", 1, 1, "cd_movimento", true, null, null, "", source, SearchModeEnum.Equals, null, null);
              var movimento_inserido = movimento_inseridoGet.data.First();
              movimento = movimento_inserido;
              cd_movimento = int.Parse(movimento_inserido["cd_movimento"]?.ToString());

              // Atualizar numeração conforme stored procedure - DEPOIS de inserir o movimento
              var isEmitirNF = (bool)parametroExists["id_emitir_nf_mercantil"];
              var numeroMovimento = 0;

              // Atualizar parâmetros conforme SP (linhas 858-877)
              var parametro_update = new Dictionary<string, object>();

              if (isEmitirNF)
              {
                // Se emitir NF mercantil, atualizar nm_nf_mercantil
                parametro_update["nm_nf_mercantil"] = nm_nf_mercantil;
              }
              else
              {
                // Se não emitir NF mercantil, atualizar nm_nf_material (conforme SP linha 869-877)
                parametro_update["nm_nf_material"] = nm_nf_mercantil;
              }

              var param_result = await SQLServerService.Update("T_PARAMETRO", parametro_update, source, "cd_pessoa_escola", cd_escola);
              if (!param_result.success) return BadRequest(param_result.error);
              //movimento item
            }
            else
            {
              movimento = movimento_existente;
              cd_movimento = int.Parse(movimento_existente["cd_movimento"]?.ToString());

              var movimento_update_dict = new Dictionary<string, object>
                    {
                        { "id_venda_futura", venda.venda ? 0 : 1 },
                        { "nm_nfe", venda.venda ? nm_nf_mercantil : (object)DBNull.Value },
                        { "nm_movimento", nm_nf_mercantil },
                        { "dc_serie_movimento", (bool)parametroExists["id_emitir_nf_mercantil"] ? parametroExists["dc_serie_nf_mercantil"] ?? "1" : (venda.venda ? "M" : "F") }
                    };
              var t_movimento_Result = await SQLServerService.Update("T_MOVIMENTO", movimento_update_dict, source, "cd_movimento", cd_movimento);
              if (!t_movimento_Result.success) return BadRequest(t_movimento_Result.error);
            }


            var item_movimento_existente = await SQLServerService.GetFirstByFields(source, "T_ITEM_MOVIMENTO", new List<(string campo, object valor)> { new("cd_item", venda.cd_item), new("cd_movimento", cd_movimento) });


            if (item_movimento_existente == null)
            {
              // Buscar valor do item na escola conforme procedure
              var vl_item = 0m;
              if (item_escola != null)
              {
                var vl_item_escola = item_escola["vl_item"];
                var vl_custo_escola = item_escola["vl_custo"];
                vl_item = Convert.ToDecimal(vl_item_escola) > 0 ? Convert.ToDecimal(vl_item_escola) : Convert.ToDecimal(vl_custo_escola ?? 0);
              }

              // Buscar plano de conta conforme procedure
              var cd_plano_conta_item = cd_plano_conta_mtr; // default
              var item_subgrupo = await SQLServerService.GetFirstByFields(source, "T_ITEM_SUBGRUPO",
                  new List<(string campo, object valor)> { new("cd_item", venda.cd_item), new("id_tipo_movimento", 2) });

              if (item_subgrupo != null)
              {
                var cd_subgrupo_conta = item_subgrupo["cd_subgrupo_conta"];
                var plano_conta = await SQLServerService.GetFirstByFields(source, "T_PLANO_CONTA",
                    new List<(string campo, object valor)> { new("cd_pessoa_empresa", cd_escola), new("cd_subgrupo_conta", cd_subgrupo_conta) });

                if (plano_conta != null)
                  cd_plano_conta_item = plano_conta["cd_plano_conta"].ToString();
              }

              // Situações tributárias conforme procedure (valores padrão)
              var cd_situacao_tributaria_ICMS = (object)DBNull.Value;
              var cd_situacao_tributaria_PIS = 65;
              var cd_situacao_tributaria_COFINS = 107;
              var vl_base_calculo_ICMS = 0m;
              var vl_base_calculo_PIS = vl_item;
              var vl_base_calculo_COFINS = vl_item;
              var vl_base_calculo_IPI = vl_item;
              var vl_ICMS_item = 0m;

              // Se for para emitir NF, calcular impostos
              if ((bool)parametroExists["id_emitir_nf_mercantil"] && parametroExists["cd_tipo_nf_material"] != null)
              {
                // Aqui seria necessário implementar os cálculos tributários da procedure
                // Por ora, manter valores zerados para não quebrar
              }

              var item_movimento_dict = new Dictionary<string, object>
                        {
                            {"cd_plano_conta", cd_plano_conta_item },
                            {"dc_item_movimento", item != null ? item["no_item"] : "" },
                            { "cd_movimento", cd_movimento },
                            { "cd_item", venda.cd_item },
                            { "qt_item_movimento", 1 },
                            { "vl_unitario_item", vl_item },
                            { "vl_total_item", vl_item },
                            { "vl_liquido_item", vl_item },
                            { "vl_acrescimo_item", 0 },
                            { "vl_desconto_item", 0 },
                            { "cd_situacao_tributaria_ICMS", cd_situacao_tributaria_ICMS },
                            { "cd_situacao_tributaria_PIS", cd_situacao_tributaria_PIS },
                            { "cd_situacao_tributaria_COFINS", cd_situacao_tributaria_COFINS },
                            { "vl_base_calculo_ICMS_item", vl_base_calculo_ICMS },
                            { "vl_base_calculo_PIS_item", vl_base_calculo_PIS },
                            { "vl_base_calculo_COFINS_item", vl_base_calculo_COFINS },
                            { "vl_base_calculo_IPI_item", vl_base_calculo_IPI },
                            { "vl_base_calculo_ISS_item", 0 },
                            { "vl_ICMS_item", vl_ICMS_item },
                            { "vl_PIS_item", 0},
                            { "vl_COFINS_item", 0 },
                            { "vl_IPI_item", 0 },
                            { "vl_ISS_item", 0 },
                            { "pc_aliquota_ICMS", 0},
                            { "pc_aliquota_PIS", 0},
                            { "pc_aliquota_COFINS", 0 },
                            { "pc_aliquota_IPI", 0 },
                            { "pc_aliquota_ISS", 0 },
                            { "cd_cfop", (bool)parametroExists["id_emitir_nf_mercantil"] ? cd_cfop : (object)DBNull.Value },
                            { "dc_cfop", (bool)parametroExists["id_emitir_nf_mercantil"] ? dc_cfop_final : (object)DBNull.Value },
                            { "pc_aliquota_aproximada", 0 },
                            { "vl_aproximado", 0},
                            { "pc_desconto_item", 0 }
                        };
              var t_item_movimento_Result = await SQLServerService.Insert("T_ITEM_MOVIMENTO", item_movimento_dict, source);
              if (!t_item_movimento_Result.success) return BadRequest(t_item_movimento_Result.error);
            }

            //remover do estoque
            if (venda.venda)
            {

              if (item_escola != null)
              {
                var cd_item_escola = item_escola["cd_item_escola"];
                var qtde = item_escola["qt_estoque"];
                var qtde_item = int.Parse(qtde?.ToString() ?? "1");

                if ((qtde_item - 1) < 0)
                {
                  estoque_ok = false;
                  continue;
                }
                item_escola.Remove("cd_item_escola");
                item_escola["qt_estoque"] = int.Parse(qtde?.ToString() ?? "1") - 1;
                var t_item_escola_update = await SQLServerService.Update("T_ITEM_ESCOLA", item_escola, source, "cd_item_escola", cd_item_escola);
                if (!t_item_escola_update.success) return BadRequest(t_item_escola_update.error);

              }
            }



          }


        }


        var curso_contratos_get = await SQLServerService.GetList("T_CURSO_CONTRATO", null, "[cd_contrato]", $"[{model.cd_contrato}]", source);
        var cursosContrato_remover = curso_contratos_get.data.Select(x => int.Parse(x["cd_curso_contrato"].ToString())).ToList();
        // remover vinculos aluno turma
        for (int i = 0; i < cursosContrato_remover.Count; i++)
        {
          var t_aluno_turma_result = await SQLServerService.Delete("T_ALUNO_TURMA", "cd_curso_contrato", cursosContrato_remover[i].ToString(), source);
          if (!t_aluno_turma_result.success) return BadRequest("erro ao remover vinculo de t_aluno_turma e curso_contrato: " + t_aluno_turma_result.error);
        }
        //remover todos os cursos do contrato
        var t_curso_contrato_result = await SQLServerService.Delete("T_CURSO_CONTRATO", "cd_contrato", model.cd_contrato.ToString(), source);
        //cursoContrato

        var cursosContrato = new List<int>();
        if (!model.CursoContrato.IsNullOrEmpty())
        {
          foreach (var curso_contrato in model.CursoContrato)
          {
            var curso = new Dictionary<string, object?>
                        {
                            { "cd_contrato", model.cd_contrato },
                            { "cd_curso", curso_contrato.cd_curso },
                            { "cd_duracao", curso_contrato.cd_duracao },
                            { "cd_tipo_financeiro", curso_contrato.cd_tipo_financeiro_curso },
                            { "cd_pessoa_responsavel", curso_contrato.cd_pessoa_responsavel_curso },
                            { "nm_dia_vcto", curso_contrato.nm_dia_vcto_curso },
                            { "nm_mes_vcto", curso_contrato.nm_mes_vcto_curso },
                            { "nm_ano_vcto", curso_contrato.nm_ano_vcto_curso },
                            { "nm_parcelas_mensalidade", curso_contrato.nm_parcelas_curso },
                            { "vl_curso_contrato", curso_contrato.vl_curso_total },
                            { "pc_desconto_contrato", curso_contrato.pc_desconto_contrato_curso },
                            { "vl_matricula_curso", curso_contrato.vl_matricula_curso },
                            { "vl_parcela_contrato", curso_contrato.vl_parcela_curso },
                            { "vl_desconto_contrato", curso_contrato.vl_desconto_curso },
                            { "pc_responsavel_contrato", curso_contrato.pc_responsavel_curso },
                            { "vl_parcela_liquida", curso_contrato.vl_parcela_liquida_curso },
                            { "id_liberar_certificado", curso_contrato.id_liberar_certificado },
                            { "vl_curso_liquido", curso_contrato.vl_curso_liquido },
                            { "nm_mes_curso_inicial", curso_contrato.nm_mes_curso_inicial_curso },
                            { "nm_ano_curso_inicial", curso_contrato.nm_ano_curso_inicial_curso },
                            { "nm_mes_curso_final", curso_contrato.nm_mes_curso_final_curso },
                            { "nm_ano_curso_final", curso_contrato.nm_ano_curso_final_curso },
                            { "id_valor_incluso", curso_contrato.id_valor_incluso },
                            { "id_incorporar_valor_material", curso_contrato.id_incorporar_valor_material },
                            { "nm_parcelas_material", curso_contrato.nm_parcelas_material_curso },

                            { "vl_parcela_material", curso_contrato.vl_parcelas_material_curso },
                            { "vl_material_contrato", curso_contrato.vl_material_curso },
                            { "vl_parcela_liq_material", curso_contrato.vl_parcela_liq_material_curso },
                            { "pc_bolsa_material", curso_contrato.pc_bolsa_material_curso },
                            { "pc_desconto_material", curso_contrato.pc_desconto_material_curso },
                            { "vl_liquido_material", curso_contrato.vl_liquido_material_curso },
                            { "vl_desconto_material", curso_contrato.vl_desconto_material_curso },
                            { "id_opcao_venda", curso_contrato.opcao_venda_curso },
                            { "cd_tipo_financeiro_material", curso_contrato.cd_tipo_financeiro_material_curso },
                            { "cd_pessoa_responsavel_material", curso_contrato.cd_pessoa_responsavel_material_curso },
                            { "pc_responsavel_material", curso_contrato.pc_responsavel_material_curso },
                            { "dt_vencimento_parcela_1", curso_contrato.dt_vencimento_parcela_1_curso?.ToString("yyyy-MM-ddTHH:mm:ss") },
                            { "cd_regime", curso_contrato.cd_regime },
                            { "pc_bolsa_contrato", curso_contrato.pc_bolsa_curso },
                            { "dt_vencimento_parcela_1_material", curso_contrato.dt_vencimento_parcela_1_material_curso?.ToString("yyyy-MM-ddTHH:mm:ss") }
                        };
            //T_CURSO_MATRICULA

            var t_curso_contrato_Result = await SQLServerService.InsertWithResult("T_CURSO_CONTRATO", curso, source);
            if (!t_curso_contrato_Result.success) return BadRequest(t_curso_contrato_Result.error);

            cursosContrato.Add(int.Parse(t_curso_contrato_Result.inserted["cd_curso_contrato"].ToString()));
          }
        }

        //turma
        if (!model.Turmas.IsNullOrEmpty())
        {
          for (int i = 0; i < model.Turmas.Count; i++)
          {
            var turma = model.Turmas[i];
            var filtroTurma = new List<(string campo, object valor)> { new("cd_turma", turma.cd_turma) };
            var turmaExists = await SQLServerService.GetFirstByFields(source, "T_TURMA", filtroTurma);
            if (turmaExists == null) continue;
            var no_turma = turmaExists["no_turma"];

            if (no_turma == null) continue;
            var cd_turma_original = turmaExists["cd_turma"];
            var original = no_turma?.ToString() ?? string.Empty;

            var partes = original.Split('/', 2); // corta só na primeira barra

            var situacao_aluno = model.id_tipo_matricula == 1 ? 1 :
                          model.id_tipo_matricula == 3 ? 10 :
                          model.id_tipo_matricula == 2 ? 8 : 9;

            var dt_inicio = model.dt_inicial_contrato > turma.dt_inicio_aula ? model.dt_inicial_contrato : turma.dt_inicio_aula;
            if ((bool)turmaExists["id_turma_ppt"])
            {
              //remove campos que não serão inseridos
              //comentando para funcionar o cadastro de turma personalizada
              //turmaExists.Remove("cd_turma");
              turmaExists.Remove("no_turma");

              //Busca a sigla do estagio
              var filtroCurso = new List<(string campo, object valor)> { new("cd_curso", turma.cd_curso) };
              var cursoExists = await SQLServerService.GetFirstByFields(source, "T_CURSO", filtroCurso);
              var filtroEstagio = new List<(string campo, object valor)> { new("cd_estagio", cursoExists["cd_estagio"]) };
              var estagioExists = await SQLServerService.GetFirstByFields(source, "T_ESTAGIO", filtroEstagio);

              //Busca turmas irmas existentes
              var ultima_turma_irma = await SQLServerService.GetList("T_TURMA", 1, 1, "nm_turma", true, null, "[cd_turma_ppt],[cd_curso]", $"[{cd_turma_original}],[{turma.cd_curso}]", source, SearchModeEnum.Equals, null, null);
              string complemento_nome = partes[1];
              complemento_nome = Regex.Replace(complemento_nome, @"\d+$", "");
              var nm_turma = ultima_turma_irma.success && ultima_turma_irma.data.Count > 0 ? (int)ultima_turma_irma.data[0]["nm_turma"] + 1 : 1;
              string novo_nome = $"PERSF/{estagioExists["no_estagio_abreviado"]}-{complemento_nome}{nm_turma}";

              // adiciona nome montado
              turmaExists.Add("no_turma", novo_nome);
              turmaExists.Remove("cd_turma_ppt");
              turmaExists.Add("cd_turma_ppt", cd_turma_original);
              turmaExists.Remove("cd_curso");
              turmaExists.Add("cd_curso", turma.cd_curso);
              turmaExists.Remove("cd_turma");
              turmaExists["id_turma_ppt"] = 0;
              turmaExists["nm_turma"] = nm_turma;

              var t_turma_insert = await SQLServerService.Insert("T_TURMA", turmaExists, source);
              if (!t_turma_insert.success)
              {
                string input = "PERSF/ESP1-SEG-17:00/21:00-2S/15-12";
                Match match = Regex.Match(input, @"-(\d+)$");

                if (match.Success)
                {
                    string lastNumber = match.Groups[1].Value;
                    nm_turma = int.Parse(lastNumber) + 1;
                    novo_nome = $"PERSF/{estagioExists["no_estagio_abreviado"]}-{complemento_nome}{nm_turma}";
                    turmaExists["no_turma"] = novo_nome;
                    turmaExists["nm_turma"] = nm_turma;

                    t_turma_insert = await SQLServerService.Insert("T_TURMA", turmaExists, source);
                    if (!t_turma_insert.success)
                    {
                        return BadRequest(t_turma_insert.error);
                    }
                }
              }

              var turmaCadastradaGet = await SQLServerService.GetList("T_TURMA", 1, 1, "cd_turma", true, null, null, "", source, SearchModeEnum.Equals, null, null);
              var turmaCadastrada = turmaCadastradaGet.data.First();
              int cdTurmaId = (int)turmaCadastrada["cd_turma"];

              var horario = await SQLServerService.GetList("T_HORARIO", 1, 10000000, "cd_horario", true, null, "[cd_registro]", $"[{cd_turma_original}]", source, SearchModeEnum.Equals, null, null);
              var turma_escola = await SQLServerService.GetList("T_TURMA_ESCOLA", 1, 10000000, "cd_turma_escola", true, null, "[cd_turma]", $"[{cd_turma_original}]", source, SearchModeEnum.Equals, null, null);
              var turma_professor = await SQLServerService.GetList("T_PROFESSOR_TURMA", 1, 10000000, "cd_turma", true, null, "[cd_turma]", $"[{cd_turma_original}]", source, SearchModeEnum.Equals, null, null);
              var programacao_turma = await SQLServerService.GetList("T_PROGRAMACAO_TURMA", 1, 10000000, "cd_programacao_turma", true, null, "[cd_turma]", $"[{cd_turma_original}]", source, SearchModeEnum.Equals, null, null);

              var feriado_desconsiderado = await SQLServerService.GetList("T_FERIADO_DESCONSIDERADO", 1, 10000000, "cd_feriado_desconsiderado", true, null, "[cd_turma]", $"[{cd_turma_original}]", source, SearchModeEnum.Equals, null, null);

              //vinculos para nova turma criada
              foreach (var item in horario.data)
              {
                item.Remove("cd_horario");
                item["cd_registro"] = cdTurmaId;
                var t_insert = await SQLServerService.InsertWithResult("T_HORARIO", item, source);
                if (!t_insert.success) continue;
                var cd_horario = t_insert.inserted["cd_horario"];
                
                foreach(var professor in turma_professor.data)
                {
                    var horario_professor_turma = new Dictionary<string, object> 
                    {
                        { "cd_horario", cd_horario },
                        { "cd_professor", professor["cd_professor"]}
                    };
                    var h_insert = await SQLServerService.Insert("T_HORARIO_PROFESSOR_TURMA", horario_professor_turma, source);
                }
              }
              if (turma_escola.success)
              {
                foreach (var item in turma_escola.data)
                {
                    item.Remove("cd_turma_escola");
                    item["cd_turma"] = cdTurmaId;
                    var t_insert = await SQLServerService.Insert("T_TURMA_ESCOLA", item, source);
                    if (!t_insert.success) continue;
                }
              }
              
              if (turma_professor.success)
              {
                  foreach (var item in turma_professor.data)
                  {
                    item.Remove("cd_professor_turma");
                    item["cd_turma"] = cdTurmaId;
                    var t_insert = await SQLServerService.Insert("T_PROFESSOR_TURMA", item, source);
                    if (!t_insert.success) continue;
                  }
              }
              
              if (programacao_turma.success)
              {
                  foreach (var item in programacao_turma.data)
                  {
                    item.Remove("cd_programacao_turma");
                    item["cd_turma"] = cdTurmaId;
                    var t_insert = await SQLServerService.Insert("T_PROGRAMACAO_TURMA", item, source);
                    if (!t_insert.success) continue;
                  }
              }
              
              if (feriado_desconsiderado.success)
              {
                  foreach (var item in feriado_desconsiderado.data)
                  {
                    item.Remove("cd_feriado_desconsiderado");
                    item["cd_turma"] = cdTurmaId;
                    var t_insert = await SQLServerService.Insert("T_FERIADO_DESCONSIDERADO", item, source);
                    if (!t_insert.success) continue;
                  }
              }
              //foreach (var cursoContratoId in cursosContrato)
              //{
              //  var cursoContratoAtualizar = new Dictionary<string, object>
              //  {
              //    ["cd_turma"] = cdTurmaId
              //  };

              //  //cria vinculo entre aluno e turma
              //  var alunoTurmaDict = new Dictionary<string, object>
              //  {
              //    ["cd_aluno"] = model.cd_aluno,
              //    ["cd_turma"] = cdTurmaId,
              //    ["cd_contrato"] = model.cd_contrato,
              //    ["cd_situacao_aluno_turma"] = situacao_aluno,
              //    ["dt_matricula"] = model.dt_matricula_contrato?.ToString("yyyy-MM-ddTHH:mm:ss") ?? null,
              //    ["dt_inicio"] = dt_inicio.ToString("yyyy-MM-ddTHH:mm:ss"),
              //    ["nm_matricula_turma"] = nm_matricula,
              //    ["dt_movimento"] = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
              //    ["cd_curso_contrato"] = cursoContratoId,
              //    ["cd_curso"] = turma.cd_curso
              //  };
              //  var t_aluno_Result = await SQLServerService.Insert("T_ALUNO_TURMA", alunoTurmaDict, source);
              //  if (!t_aluno_Result.success) return BadRequest(t_aluno_Result.error);
              //}
            //cria vinculo entre aluno e turma
              var alunoTurmaDict = new Dictionary<string, object>
              {
                  ["cd_aluno"] = model.cd_aluno,
                  ["cd_turma"] = cdTurmaId,
                  ["cd_contrato"] = model.cd_contrato,
                  ["cd_situacao_aluno_turma"] = situacao_aluno,
                  ["dt_matricula"] = model.dt_matricula_contrato?.ToString("yyyy-MM-ddTHH:mm:ss") ?? null,
                  ["dt_inicio"] = dt_inicio.ToString("yyyy-MM-ddTHH:mm:ss"),
                  ["nm_matricula_turma"] = nm_matricula,
                  ["dt_movimento"] = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                  ["cd_curso_contrato"] = cursosContrato[i],
                  ["cd_curso"] = turma.cd_curso
              };
              var t_aluno_Result = await SQLServerService.Insert("T_ALUNO_TURMA", alunoTurmaDict, source);
              if (!t_aluno_Result.success) return BadRequest(t_aluno_Result.error);
              
              var id_tipo_movimento = situacao_aluno == 1 ? 0
                                    : situacao_aluno == 8 ? 6
                                    : 10;
              //gera historico aluno
              //obtem ultimo historico para atualizar quantidade
              var ultimoHistorico = await SQLServerService.GetList("T_HISTORICO_ALUNO", 1, 1, "nm_sequencia", true, null, "[cd_aluno]", $"[{model.cd_aluno}]", source, SearchModeEnum.Equals, null, null);
              var sequencia_historico = 0;
              if (ultimoHistorico.success)
              {
                sequencia_historico = int.Parse(ultimoHistorico.data.FirstOrDefault()?["nm_sequencia"]?.ToString() ?? "0");
              }
              sequencia_historico += 1;

              var historicoAlunoDict = new Dictionary<string, object>
              {
                ["cd_aluno"] = model.cd_aluno,
                ["cd_turma"] = cdTurmaId,
                ["cd_contrato"] = model.cd_contrato,
                ["id_situacao_historico"] = situacao_aluno,
                ["cd_usuario"] = model.cd_usuario,
                ["dt_cadastro"] = DateTime.Now.Date,
                ["id_tipo_movimento"] = id_tipo_movimento,
                ["cd_produto"] = model.cd_produto_atual,
                ["dt_historico"] = dt_inicio,
                ["nm_sequencia"] = sequencia_historico
              };
              var t_Historico_Result = await SQLServerService.Insert("T_HISTORICO_ALUNO", historicoAlunoDict, source);
              if (!t_Historico_Result.success) return BadRequest(t_Historico_Result.error);
            }
            else
            {
              //validação aluno existente
              var filtrosAluno = new List<(string campo, object valor)> { new("cd_aluno", model.cd_aluno), new("cd_situacao_aluno_turma", 9), new("cd_contrato", model.cd_contrato) };
              var alunoExists = await SQLServerService.GetFirstByFields(source, "T_ALUNO_TURMA", filtrosAluno);

              if (alunoExists != null)
              {
                //foreach (var cursoContratoId in cursosContrato)
                //{
                //  //atualiza cd_contrato e situação aluno
                //  var aluno_atualizar = new Dictionary<string, object>
                //  {
                //    ["cd_contrato"] = model.cd_contrato,
                //    ["cd_situacao_aluno_turma"] = situacao_aluno,
                //    ["dt_matricula"] = model.dt_matricula_contrato?.ToString("yyyy-MM-ddTHH:mm:ss"),
                //    ["nm_matricula_turma"] = nm_matricula,
                //    ["cd_curso_contrato"] = cursoContratoId,
                //    ["dt_inicio"] = dt_inicio.ToString("yyyy-MM-ddTHH:mm:ss"),
                //  };
                //  var t_aluno_Result = await SQLServerService.Update("T_ALUNO_TURMA", aluno_atualizar, source, "cd_aluno", model.cd_aluno);
                //  if (!t_aluno_Result.success) return BadRequest(t_aluno_Result.error);
                //}
                                  //atualiza cd_contrato e situação aluno
                  var aluno_atualizar = new Dictionary<string, object>
                  {
                    ["cd_contrato"] = model.cd_contrato,
                    ["cd_situacao_aluno_turma"] = situacao_aluno,
                    ["dt_matricula"] = model.dt_matricula_contrato?.ToString("yyyy-MM-ddTHH:mm:ss"),
                    ["nm_matricula_turma"] = nm_matricula,
                    ["cd_curso_contrato"] = cursosContrato[i],
                    ["dt_inicio"] = dt_inicio.ToString("yyyy-MM-ddTHH:mm:ss"),
                  };
                  var t_aluno_Result = await SQLServerService.Update("T_ALUNO_TURMA", aluno_atualizar, source, "cd_aluno", model.cd_aluno);
                  if (!t_aluno_Result.success) return BadRequest(t_aluno_Result.error);
              }
              else
              {
                foreach (var cursoContratoId in cursosContrato)
                {
                  var alunoTurmaExists = await SQLServerService.GetFirstByFields(source, "T_ALUNO_TURMA", new List<(string campo, object valor)> { new("cd_aluno", model.cd_aluno), new("cd_curso", turma.cd_curso), new("cd_contrato", model.cd_contrato) });
                  if (alunoTurmaExists == null)
                  {
                    //cria vinculo entre aluno e turma
                    var alunoTurmaDict = new Dictionary<string, object>
                    {
                      ["cd_aluno"] = model.cd_aluno,
                      ["cd_turma"] = turma.cd_turma,
                      ["cd_contrato"] = model.cd_contrato,
                      ["cd_situacao_aluno_turma"] = situacao_aluno,
                      ["dt_matricula"] = model.dt_matricula_contrato?.ToString("yyyy-MM-ddTHH:mm:ss") ?? null,
                      ["dt_inicio"] = dt_inicio.ToString("yyyy-MM-ddTHH:mm:ss"),
                      ["nm_matricula_turma"] = nm_matricula,
                      ["dt_movimento"] = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                      ["cd_curso_contrato"] = cursoContratoId,
                      ["cd_curso"] = turma.cd_curso
                    };
                    var t_aluno_Result = await SQLServerService.Insert("T_ALUNO_TURMA", alunoTurmaDict, source);
                    if (!t_aluno_Result.success) return BadRequest(t_aluno_Result.error);
                  }
                  else
                  {
                    var aluno_atualizar = new Dictionary<string, object>
                    {
                      ["cd_contrato"] = model.cd_contrato,
                      ["cd_situacao_aluno_turma"] = situacao_aluno,
                      ["dt_matricula"] = model.dt_matricula_contrato?.ToString("yyyy-MM-ddTHH:mm:ss"),
                      ["nm_matricula_turma"] = nm_matricula,
                      ["cd_curso_contrato"] = cursoContratoId,
                      ["dt_inicio"] = dt_inicio.ToString("yyyy-MM-ddTHH:mm:ss"),
                    };
                    var t_aluno_Result = await SQLServerService.Update("T_ALUNO_TURMA", aluno_atualizar, source, "cd_aluno", model.cd_aluno);
                    if (!t_aluno_Result.success) return BadRequest(t_aluno_Result.error);

                  }
                }
              }
              var id_tipo_movimento = situacao_aluno == 1 ? 0
                                    : situacao_aluno == 8 ? 6
                                    : 10;
              //gera historico aluno
              //obtem ultimo historico para atualizar quantidade
              var ultimoHistorico = await SQLServerService.GetList("T_HISTORICO_ALUNO", 1, 1, "nm_sequencia", true, null, "[cd_aluno]", $"[{model.cd_aluno}]", source, SearchModeEnum.Equals, null, null);
              var sequencia_historico = 0;
              if (ultimoHistorico.success)
              {
                sequencia_historico = int.Parse(ultimoHistorico.data.FirstOrDefault()?["nm_sequencia"]?.ToString() ?? "0");
              }
              sequencia_historico += 1;

              var historicoAlunoDict = new Dictionary<string, object>
              {
                ["cd_aluno"] = model.cd_aluno,
                ["cd_turma"] = turma.cd_turma,
                ["cd_contrato"] = model.cd_contrato,
                ["id_situacao_historico"] = situacao_aluno,
                ["cd_usuario"] = model.cd_usuario,
                ["dt_cadastro"] = DateTime.Now.Date,
                ["id_tipo_movimento"] = id_tipo_movimento,
                ["cd_produto"] = model.cd_produto_atual,
                ["dt_historico"] = dt_inicio,
                ["nm_sequencia"] = sequencia_historico
              };
              var t_Historico_Result = await SQLServerService.Insert("T_HISTORICO_ALUNO", historicoAlunoDict, source);
              if (!t_Historico_Result.success) return BadRequest(t_Historico_Result.error);
            }
          }

          //Atualiza pipeline pela fila de matricula
          if (model.cd_fila_matricula != null)
          {
            //pegar fila de matricula por Id e pegar cd_contato para chegar em pipeline
            var filtrosfilaMatricula = new List<(string campo, object valor)> { new("cd_fila_matricula", model.cd_fila_matricula) };
            var filaExists = await SQLServerService.GetFirstByFields(source, "T_FILA_MATRICULA", filtrosfilaMatricula);
            if (filaExists != null)
            {
              var cd_contato = filaExists["cd_contato"];

              var filtrosPipeline = new List<(string campo, object valor)> { new("cd_etapa_pipeline", 5), new("cd_contato_pipeline ", cd_contato) };
              var pipelineExists = await SQLServerService.GetFirstByFields(source, "T_PIPELINE", filtrosPipeline);
              if (pipelineExists != null)
              {
                var cd_pipeline = pipelineExists["cd_pipeline"];

                var pipelineAtualizar = new Dictionary<string, object>
                {
                  ["id_posicao_pipeline"] = 5,
                  ["cd_etapa_pipeline"] = 5
                };
                var t_pipeline_update = await SQLServerService.Update("T_PIPELINE", pipelineAtualizar, source, "cd_pipeline", cd_pipeline);
                if (!t_pipeline_update.success) return BadRequest(t_pipeline_update.error);
              }
            }
          }
          else
          {
            //atualizar pipeline sem fila de matricula.
            //pega aluno por Id -> cd_pessoa -> pipeline cd_pessoa
            var filtrosAluno = new List<(string campo, object valor)> { new("cd_aluno", model.cd_aluno) };
            var alunoExists = await SQLServerService.GetFirstByFields(source, "T_ALUNO", filtrosAluno);
            if (alunoExists != null)
            {
              var cd_pessoa = alunoExists["cd_pessoa_aluno"];

              //pega todas as pipelines do usuario
              var pipelines_result = await SQLServerService.GetList("T_PIPELINE", null, "[cd_pessoa_pipeline]", "cd_pessoa", source, SearchModeEnum.Equals);
              if (pipelines_result.success)
              {
                //pega somente a pipeline que não for id_posicao_pipeline 5 ou 6
                var pipeline = pipelines_result.data.FirstOrDefault(x => x["id_posicao_pipeline"].ToString() != "5" && x["id_posicao_pipeline"].ToString() != "6");
                if (pipeline != null)
                {
                  var cd_pipeline = pipeline["cd_pipeline"];

                  var pipelineAtualizar = new Dictionary<string, object>
                  {
                    ["id_posicao_pipeline"] = 5,
                    ["cd_etapa_pipeline"] = 5
                  };
                  var t_pipeline_update = await SQLServerService.Update("T_PIPELINE", pipelineAtualizar, source, "cd_pipeline", cd_pipeline);
                  if (!t_pipeline_update.success) return BadRequest(t_pipeline_update.error);
                }
              }
            }
          }
        }

        return ResponseDefault();
      }
      return BadRequest(new
      {
        error = "Fonte de dados não configurada ou inativa."
      });
    }

    [Authorize]
    [HttpPut]
    [Route("titulos")]
    public async Task<IActionResult> AtualizaTitulos([FromBody] MatriculaTitulosModel model)
    {
      var schemaName = "T_Pessoa";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);
      if (source != null && source.Active != null && source.Active == true)
      {
        if (model == null || model.cd_contrato == 0) return BadRequest("dados invalidos");

        // Validar se existem baixas que não sejam de bolsa (tipo 100)
        var query = $@"SELECT 1 FROM T_BAIXA_TITULO bt
                       INNER JOIN T_titulo t ON bt.cd_titulo = t.cd_titulo
                       WHERE t.cd_origem_titulo = {model.cd_contrato}
                       AND bt.cd_tipo_liquidacao != 100";
        var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};";
        bool possui_baixa = false;
        using (var connection = new SqlConnection(connectionString))
        {
          await connection.OpenAsync();
          using (var command = new SqlCommand(query, connection))
          {
            using (var reader = await command.ExecuteReaderAsync())
            {
              possui_baixa = await reader.ReadAsync();
            }
          }
        }


        if (possui_baixa) return BadRequest("Existem titulos com baixa, não é possivel atualizar os titulos");


        var filtrosContrato = new List<(string campo, object valor)> { new("cd_contrato", model.cd_contrato) };
        var matriculaExists = await SQLServerService.GetFirstByFields(source, "T_CONTRATO", filtrosContrato);
        if (matriculaExists == null) return NotFound("matricula não encontrata");

        var cd_pessoa_escola_update = matriculaExists["cd_pessoa_escola"];

        // Verificar se há títulos com status baixado (2) mas que não sejam baixas de bolsa
        var titulosComBaixaNaoBolsa = await SQLServerService.GetList("T_TITULO", null, "[cd_origem_titulo],[id_status_titulo],[cd_pessoa_empresa]", $"[{model.cd_contrato}],[2],[{cd_pessoa_escola_update}]", source, SearchModeEnum.Equals);
        if (titulosComBaixaNaoBolsa.success && titulosComBaixaNaoBolsa.data != null && titulosComBaixaNaoBolsa.data.Any())
        {
          // Verificar se essas baixas são apenas de bolsa
          var cd_titulos_baixados = string.Join(",", titulosComBaixaNaoBolsa.data.Select(x => x["cd_titulo"].ToString()));
          var baixas_nao_bolsa = await SQLServerService.GetList("T_BAIXA_TITULO", cd_titulos_baixados, "cd_titulo", null, source);

          bool tem_baixa_nao_bolsa = false;
          if (baixas_nao_bolsa.success && baixas_nao_bolsa.data != null && baixas_nao_bolsa.data.Any())
          {
            tem_baixa_nao_bolsa = baixas_nao_bolsa.data.Any(b =>
              b.ContainsKey("cd_tipo_liquidacao") &&
              b["cd_tipo_liquidacao"] != null &&
              b["cd_tipo_liquidacao"].ToString() != "100"
            );
          }

          if (tem_baixa_nao_bolsa)
          {
            return BadRequest("Existem titulos com baixa, não é possivel atualizar os titulos");
          }
        }
        var nm_matricula = matriculaExists["nm_matricula_contrato"];
        var cd_escola = matriculaExists["cd_pessoa_escola"];

        var filtroParametro = new List<(string campo, object valor)> { new("cd_pessoa_escola", cd_escola) };
        var parametroExists = await SQLServerService.GetFirstByFields(source, "T_PARAMETRO", filtroParametro);
        if (parametroExists == null) return NotFound("parametros não encontratos para esta escola");

        var cd_plano_conta_mat = parametroExists["cd_plano_conta_mat"] != null ? parametroExists["cd_plano_conta_mat"].ToString() : "0";
        var cd_plano_conta_mtr = parametroExists["cd_plano_conta_material"] != null ? parametroExists["cd_plano_conta_material"].ToString() : "0";

        var responsavel = matriculaExists["cd_pessoa_responsavel"];
        if (responsavel == null || Convert.ToInt32(responsavel) == 0)
        {
          var cd_aluno = matriculaExists["cd_aluno"];
          var alunoExists = await SQLServerService.GetFirstByFields(source, "T_ALUNO", new List<(string campo, object valor)> { ("cd_aluno", cd_aluno) });
          if (alunoExists == null) return BadRequest($"Aluno não encontrado (cd_aluno: {cd_aluno})");
          responsavel = alunoExists["cd_pessoa_aluno"]?.ToString() ?? "0";
        }

        {
          responsavel = matriculaExists["cd_aluno"];
        }
        var dict_contrato = new Dictionary<string, object>();
        if (model.Desconto != null && model.Desconto.Value)
        {
          dict_contrato.Add("pc_desconto_contrato", "0");
          dict_contrato.Add("vl_desconto_contrato", "0");

          //remover vinculos de desconto
          var delete_result = await SQLServerService.Delete("T_DESCONTO_CONTRATO", "cd_contrato", model.cd_contrato.ToString(), source);
          if (!delete_result.success) return BadRequest(delete_result.error);
        }

        if (model.Bolsa != null && model.Bolsa.Value)
        {
          //remover vinculos de bolsa
          dict_contrato.Add("pc_desconto_bolsa", "0");
        }
        if (dict_contrato.Any())
        {
          var update_result = await SQLServerService.Update("T_CONTRATO", dict_contrato, source, "cd_contrato", model.cd_contrato);
          if (!update_result.success) return BadRequest(update_result.error);
        }
        if (!model.TitulosMensalidade.IsNullOrEmpty())
        {
          var delete_mensalidades_result = await SQLServerService.DeleteByTwoFields("T_TITULO", "cd_origem_titulo", model.cd_contrato.ToString(), "dc_tipo_titulo", model.TitulosMensalidade.First().dc_tipo_titulo, source);
          if (!delete_mensalidades_result.success) return BadRequest(delete_mensalidades_result.error);
          foreach (var titulo in model.TitulosMensalidade)
          {
            var dictTitulo = new Dictionary<string, object>
            {
              ["cd_pessoa_empresa"] = cd_escola,
              ["cd_pessoa_titulo"] = titulo.cd_pessoa_titulo,
              ["cd_pessoa_responsavel"] = titulo.cd_pessoa_responsavel != 0 ? titulo.cd_pessoa_responsavel : responsavel,
              ["cd_local_movto"] = parametroExists["cd_local_movto"],
              ["dt_emissao_titulo"] = titulo.dt_emissao_titulo.ToString("yyyy-MM-ddTHH:mm:ss"),
              ["cd_origem_titulo"] = model.cd_contrato,
              ["dt_vcto_titulo"] = titulo.dt_vcto_titulo.ToString("yyyy-MM-ddTHH:mm:ss"),
              ["dh_cadastro_titulo"] = DateTime.Now.Date.ToString("yyyy-MM-ddTHH:mm:ss"),
              ["vl_titulo"] = titulo.vl_titulo,
              ["vl_saldo_titulo"] = titulo.vl_saldo_titulo,
              ["dc_tipo_titulo"] = titulo.dc_tipo_titulo,
              ["dc_num_documento_titulo"] = titulo.dc_num_documento_titulo,
              ["nm_titulo"] = matriculaExists["nm_contrato"],
              ["nm_parcela_titulo"] = titulo.nm_parcela_titulo,
              ["cd_tipo_financeiro"] = titulo.cd_tipo_financeiro,
              ["id_status_titulo"] = 1,
              ["id_status_cnab"] = titulo.id_status_cnab,
              ["id_origem_titulo"] = 22,
              ["id_natureza_titulo"] = 1,
              ["vl_material_titulo"] = titulo.vl_material_titulo,
              ["pc_taxa_cartao"] = titulo.pc_taxa_cartao,
              ["nm_dias_cartao"] = titulo.nm_dias_cartao,
              ["id_cnab_contrato"] = titulo.id_cnab_contrato,
              ["vl_taxa_cartao"] = titulo.vl_taxa_cartao,
              ["cd_aluno"] = titulo.cd_aluno,
              ["pc_responsavel"] = titulo.pc_responsavel == null || titulo.pc_responsavel == 0 ? 100 : titulo.pc_responsavel,
              ["vl_mensalidade"] = titulo.vl_mensalidade,
              ["pc_bolsa"] = titulo.pc_bolsa,
              ["vl_bolsa"] = titulo.vl_bolsa,
              ["pc_desconto_mensalidade"] = titulo.pc_desconto_mensalidade,
              ["vl_desconto_mensalidade"] = titulo.vl_desconto_mensalidade,
              ["pc_bolsa_material"] = titulo.pc_bolsa_material,
              ["vl_bolsa_material"] = titulo.vl_bolsa_material,
              ["pc_desconto_material"] = titulo.pc_desconto_material,
              ["vl_desconto_material"] = titulo.vl_desconto_material,
              ["pc_desconto_total"] = titulo.pc_desconto_total,
              ["vl_desconto_total"] = titulo.vl_desconto_total,
              ["opcao_venda"] = titulo.opcao_venda,
              ["cd_curso"] = titulo.cd_curso
            };
            var t_titulo_Result = await SQLServerService.Insert("T_TITULO", dictTitulo, source);
            if (!t_titulo_Result.success) return BadRequest(t_titulo_Result.error);

            var t_tituloGet = await SQLServerService.GetList("T_TITULO", 1, 1, "cd_titulo", true, null, null, "", source, SearchModeEnum.Equals, null, null);
            var titulo_inserido = t_tituloGet.data.First();

            var id_origem_titulo = titulo_inserido["id_origem_titulo"]?.ToString() ?? "0";

            if (id_origem_titulo == "22" && titulo.dc_tipo_titulo == "ME")
            {
              //T_plano_titulo
              var dict_plano = new Dictionary<string, object>
              {
                ["cd_titulo"] = titulo_inserido["cd_titulo"],
                ["cd_plano_conta"] = cd_plano_conta_mat,
                ["vl_plano_titulo"] = titulo.opcao_venda != null && titulo.opcao_venda == "1" ? titulo.vl_mensalidade : 0
              };
              var t_plano_titulo_Result = await SQLServerService.Insert("T_PLANO_TITULO", dict_plano, source);
              if (!t_plano_titulo_Result.success) return BadRequest(t_plano_titulo_Result.error);
            }

            if (id_origem_titulo == "22" && titulo.dc_tipo_titulo == "ME" && titulo.vl_material_titulo > 0)
            {
              //T_plano_titulo
              var dict_plano = new Dictionary<string, object>
              {
                ["cd_titulo"] = titulo_inserido["cd_titulo"],
                ["cd_plano_conta"] = cd_plano_conta_mtr,
                ["vl_plano_titulo"] = titulo.vl_material_titulo
              };
              var t_plano_titulo_Result = await SQLServerService.Insert("T_PLANO_TITULO", dict_plano, source);
              if (!t_plano_titulo_Result.success) return BadRequest(t_plano_titulo_Result.error);
            }
          }
        }

        if (!model.TitulosMaterial.IsNullOrEmpty())
        {
          var delete_material_result = await SQLServerService.DeleteByTwoFields("T_TITULO", "cd_origem_titulo", model.cd_contrato.ToString(), "dc_tipo_titulo", model.TitulosMaterial.First().dc_tipo_titulo, source);
          if (!delete_material_result.success) return BadRequest(delete_material_result.error);
          foreach (var titulo in model.TitulosMaterial)
          {
            var dictTitulo = new Dictionary<string, object>
            {
              ["cd_pessoa_empresa"] = cd_escola,
              ["cd_pessoa_titulo"] = titulo.cd_pessoa_titulo,
              ["cd_pessoa_responsavel"] = titulo.cd_pessoa_responsavel != 0 ? titulo.cd_pessoa_responsavel : responsavel,

              ["cd_local_movto"] = parametroExists["cd_local_movto"],
              ["dt_emissao_titulo"] = titulo.dt_emissao_titulo.ToString("yyyy-MM-ddTHH:mm:ss"),

              ["cd_origem_titulo"] = model.cd_contrato,
              ["dt_vcto_titulo"] = titulo.dt_vcto_titulo.ToString("yyyy-MM-ddTHH:mm:ss"),
              ["dh_cadastro_titulo"] = DateTime.Now.Date,
              ["vl_titulo"] = titulo.vl_titulo,
              ["vl_saldo_titulo"] = titulo.vl_saldo_titulo,
              ["dc_tipo_titulo"] = titulo.dc_tipo_titulo,
              ["dc_num_documento_titulo"] = titulo.dc_num_documento_titulo,
              ["nm_titulo"] = matriculaExists["nm_contrato"],
              ["nm_parcela_titulo"] = titulo.nm_parcela_titulo,
              ["cd_tipo_financeiro"] = titulo.cd_tipo_financeiro,
              ["id_status_titulo"] = 1,
              ["id_status_cnab"] = titulo.id_status_cnab,
              ["id_origem_titulo"] = 22,
              ["id_natureza_titulo"] = 1,
              ["vl_material_titulo"] = titulo.vl_material_titulo,
              ["pc_taxa_cartao"] = titulo.pc_taxa_cartao,
              ["nm_dias_cartao"] = titulo.nm_dias_cartao,
              ["id_cnab_contrato"] = titulo.id_cnab_contrato,
              ["vl_taxa_cartao"] = titulo.vl_taxa_cartao,
              ["cd_aluno"] = titulo.cd_aluno,
              ["pc_responsavel"] = titulo.pc_responsavel == null || titulo.pc_responsavel == 0 ? 100 : titulo.pc_responsavel,
              ["vl_mensalidade"] = titulo.vl_mensalidade,
              ["pc_bolsa"] = titulo.pc_bolsa,
              ["vl_bolsa"] = titulo.vl_bolsa,
              ["pc_desconto_mensalidade"] = titulo.pc_desconto_mensalidade,
              ["vl_desconto_mensalidade"] = titulo.vl_desconto_mensalidade,
              ["pc_bolsa_material"] = titulo.pc_bolsa_material,
              ["vl_bolsa_material"] = titulo.vl_bolsa_material,
              ["pc_desconto_material"] = titulo.pc_desconto_material,
              ["vl_desconto_material"] = titulo.vl_desconto_material,
              ["pc_desconto_total"] = titulo.pc_desconto_total,
              ["vl_desconto_total"] = titulo.vl_desconto_total,
              ["opcao_venda"] = titulo.opcao_venda,
              ["cd_curso"] = titulo.cd_curso
            };
            var t_titulo_Result = await SQLServerService.Insert("T_TITULO", dictTitulo, source);
            if (!t_titulo_Result.success) return BadRequest(t_titulo_Result.error);
            var titulo_inseridoGet = await SQLServerService.GetList("T_TITULO", 1, 1, "cd_titulo", true, null, null, "", source, SearchModeEnum.Equals, null, null);
            var titulo_inserido = titulo_inseridoGet.data.First();

            var id_origem_titulo = titulo_inserido["id_origem_titulo"]?.ToString() ?? "0";

            if (id_origem_titulo == "22" && titulo.dc_tipo_titulo == "MT")
            {
              //T_plano_titulo
              var dict_plano = new Dictionary<string, object>
              {
                ["cd_titulo"] = titulo_inserido["cd_titulo"],
                ["cd_plano_conta"] = cd_plano_conta_mtr,
                ["vl_plano_titulo"] = titulo.vl_titulo
              };
              var t_plano_titulo_Result = await SQLServerService.Insert("T_PLANO_TITULO", dict_plano, source);
              if (!t_plano_titulo_Result.success) return BadRequest(t_plano_titulo_Result.error);
            }
          }
        }

        return ResponseDefault();
      }
      return BadRequest(new
      {
        error = "Fonte de dados não configurada ou inativa."
      });
    }

    [Authorize]
    [HttpPut]
    [Route("info")]
    public async Task<IActionResult> Put(MatriculaUpdateInfoModel model)
    {
      var schemaName = "T_Pessoa";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);
      if (source != null && source.Active != null && source.Active == true)
      {
        //valida se matricula existe
        var filtrosContrato = new List<(string campo, object valor)> { new("cd_contrato", model.cd_contrato) };
        var matriculaExists = await SQLServerService.GetFirstByFields(source, "T_CONTRATO", filtrosContrato);
        if (matriculaExists == null) return NotFound("contrato");

        var matricula_dict = new Dictionary<string, object>();

        if (model.id_tipo_matricula != null)
          matricula_dict["id_tipo_matricula"] = model.id_tipo_matricula;

        if (model.dt_inicial_contrato != null)
          matricula_dict["dt_inicial_contrato"] = model.dt_inicial_contrato?.ToString("yyyy-MM-ddTHH:mm:ss");

        if (model.dt_final_contrato != null)
          matricula_dict["dt_final_contrato"] = model.dt_final_contrato?.ToString("yyyy-MM-ddTHH:mm:ss");

        if (model.id_retorno != null)
          matricula_dict["id_retorno"] = model.id_retorno;

        if (model.id_transferencia != null)
          matricula_dict["id_transferencia"] = model.id_transferencia;

        if (matricula_dict.Any())
        {
          var matriculaResult = await SQLServerService.Update("T_CONTRATO", matricula_dict, source, "cd_contrato", model.cd_contrato);
          if (!matriculaResult.success) return BadRequest(matriculaResult.error);
        }
        return ResponseDefault();
      }
      return BadRequest(new
      {
        error = "Fonte de dados não configurada ou inativa."
      });
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="cd_contrato"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPut]
    [Route("aditamento/{cd_contrato}")]
    public async Task<IActionResult> Put(int cd_contrato, List<MatriculaUpdateAditamentosModel> model)
    {
      var schemaName = "T_Pessoa";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);
      if (source != null && source.Active != null && source.Active == true)
      {
        //valida se matricula existe
        var filtrosContrato = new List<(string campo, object valor)> { new("cd_contrato", cd_contrato) };
        var matriculaExists = await SQLServerService.GetFirstByFields(source, "T_CONTRATO", filtrosContrato);
        if (matriculaExists == null) return NotFound("contrato");
        var cd_escola = matriculaExists["cd_pessoa_escola"];
        var cd_pessoa_responsavel = matriculaExists["cd_pessoa_responsavel"];
        var cd_tipo_financeiro = matriculaExists["cd_tipo_financeiro"];
        var ultimo_titulo_contratoGet = await SQLServerService.GetList("T_TITULO", 1, 1, "cd_titulo", true, null, "[cd_origem_titulo],[cd_pessoa_empresa]", $"[{cd_contrato}],[{cd_escola}]", source, SearchModeEnum.Equals, null, null);
        var ultimo_titulo_contrato = ultimo_titulo_contratoGet.data.FirstOrDefault();

        var nm_contrato = matriculaExists["nm_contrato"];
        var responsavel = matriculaExists["cd_pessoa_responsavel"];
        var responsavel_material = matriculaExists["cd_pessoa_responsavel_material"];

        var filtroParametro = new List<(string campo, object valor)> { new("cd_pessoa_escola", cd_escola) };
        var parametroExists = await SQLServerService.GetFirstByFields(source, "T_PARAMETRO", filtroParametro);
        if (parametroExists == null) return NotFound($"parametros não encontratos para esta escola({cd_escola})");


        var nm_contrato_p = parametroExists["nm_ultimo_contrato"].ToString() ?? "0";
        var nm_matricula_p = parametroExists["nm_ultimo_matricula"].ToString() ?? "0";
        var cd_plano_conta_mat = parametroExists["cd_plano_conta_mat"].ToString() ?? "0";
        var cd_plano_conta_tax = parametroExists["cd_plano_conta_tax"].ToString() ?? "0";
        var cd_plano_conta_mtr = parametroExists["cd_plano_conta_material"].ToString() ?? "0";

        // Buscar cd_pessoa_aluno para usar como fallback em cd_pessoa_titulo
        var cd_aluno = matriculaExists["cd_aluno"];
        var alunoExists = await SQLServerService.GetFirstByFields(source, "T_ALUNO", new List<(string campo, object valor)> { new("cd_aluno", cd_aluno) });
        if (alunoExists == null) return NotFound($"Aluno não encontrado (cd_aluno: {cd_aluno})");
        var cd_pessoa_aluno = alunoExists["cd_pessoa_aluno"];

        // Validar se cd_pessoa_aluno existe em T_PESSOA
        var pessoaAlunoExists = await SQLServerService.GetFirstByFields(source, "T_PESSOA", new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa_aluno) });
        if (pessoaAlunoExists == null)
        {
          return BadRequest($"cd_pessoa_aluno ({cd_pessoa_aluno}) do aluno {cd_aluno} não existe na tabela T_PESSOA. Corrija o cadastro do aluno antes de criar o aditamento.");
        }

        // Validar se responsavel existe em T_PESSOA (se informado)
        if (responsavel != null && Convert.ToInt32(responsavel) != 0)
        {
          var pessoaResponsavelExists = await SQLServerService.GetFirstByFields(source, "T_PESSOA", new List<(string campo, object valor)> { new("cd_pessoa", responsavel) });
          if (pessoaResponsavelExists == null)
          {
            return BadRequest($"cd_pessoa_responsavel ({responsavel}) do contrato não existe na tabela T_PESSOA. Corrija o cadastro do contrato antes de criar o aditamento.");
          }
        }

        // Validar se cd_escola existe em T_PESSOA
        var pessoaEscolaExists = await SQLServerService.GetFirstByFields(source, "T_PESSOA", new List<(string campo, object valor)> { new("cd_pessoa", cd_escola) });
        if (pessoaEscolaExists == null)
        {
          return BadRequest($"cd_pessoa_escola ({cd_escola}) não existe na tabela T_PESSOA. Corrija o cadastro da escola/empresa antes de criar o aditamento.");
        }

        //Aditamentos
        if (!model.IsNullOrEmpty())
        {
          var dict_contrato = new Dictionary<string, object>();
          foreach (var ad in model)
          {
            var dict = new Dictionary<string, object>
            {
              ["cd_contrato"] = ad.cd_contrato,
              ["id_tipo_data_inicio"] = ad.id_tipo_data_inicio,
              ["vl_aula_hora"] = ad.vl_aula_hora,
              ["nm_titulos_aditamento"] = ad.nm_titulos_aditamento,
              ["cd_usuario"] = ad.cd_usuario,
              ["vl_aditivo"] = ad.vl_aditivo,
              ["vl_parcela_titulo_aditamento"] = ad.vl_parcela_titulo_aditamento,
              ["id_ajuste_manual"] = ad.id_ajuste_manual,
              ["id_tipo_aditamento"] = ad.id_tipo_aditamento,
              ["id_tipo_pagamento"] = ad.id_tipo_pagamento,
              ["cd_reajuste_anual"] = ad.cd_reajuste_anual,
              ["cd_tipo_financeiro"] = ad.cd_tipo_financeiro,
              ["vl_saldo_aberto"] = ad.vl_saldo_aberto,
              ["vl_anterior"] = ad.vl_anterior,
              ["id_status_renegociacao"] = 0
            };

            if (ad.cd_nome_contrato.HasValue) dict.Add("cd_nome_contrato", ad.cd_nome_contrato);
            if (ad.dt_aditamento.HasValue) dict["dt_aditamento"] = ad.dt_aditamento.Value.ToString("yyyy-MM-ddTHH:mm:ss");

            if (ad.dt_inicio_aditamento.HasValue) dict["dt_inicio_aditamento"] = ad.dt_inicio_aditamento.Value.ToString("yyyy-MM-ddTHH:mm:ss");

            if (!string.IsNullOrEmpty(ad.nm_dia_vcto_desconto)) dict["nm_dia_vcto_desconto"] = ad.nm_dia_vcto_desconto;

            if (ad.nm_previsao_inicial.HasValue) dict["nm_previsao_inicial"] = ad.nm_previsao_inicial.Value;

            if (ad.dt_vcto_aditamento.HasValue) dict["dt_vcto_aditamento"] = ad.dt_vcto_aditamento.Value.ToString("yyyy-MM-ddTHH:mm:ss");

            if (!string.IsNullOrEmpty(ad.tx_obs_aditamento)) dict["tx_obs_aditamento"] = ad.tx_obs_aditamento;

            if (ad.dt_vencto_inicial.HasValue) dict["dt_vencto_inicial"] = ad.dt_vencto_inicial.Value.ToString("yyyy-MM-ddTHH:mm:ss");

            if (!string.IsNullOrEmpty(ad.nm_sequencia_aditamento)) dict["nm_sequencia_aditamento"] = ad.nm_sequencia_aditamento;

            var dict_bolsa = new Dictionary<string, object>();

            if (ad.pc_bolsa != null && ad.pc_bolsa > 0) dict_contrato["pc_desconto_bolsa"] = ad.pc_bolsa;

            if (ad.pc_desconto_contrato != null && ad.pc_desconto_contrato > 0) dict_contrato["pc_desconto_contrato"] = ad.pc_desconto_contrato;

            if (ad.pc_bolsa_material != null && ad.pc_bolsa_material > 0) dict_contrato["pc_bolsa_material"] = ad.pc_bolsa_material;

            if (ad.dt_comunicado_bolsa != null) dict_bolsa["dt_comunicado_bolsa"] = ad.dt_comunicado_bolsa?.ToString("yyyy-MM-ddTHH:mm:ss");

            if (ad.dc_validade_bolsa != null) dict_bolsa["dc_validade_bolsa"] = ad.dc_validade_bolsa;


            if (ad.cd_motivo_bolsa != null) dict_bolsa["cd_motivo_bolsa"] = ad.cd_motivo_bolsa;

            //buscar todos os aditamentos do contrato antes de cadastrar o novo
            var aditamentos_contrato = await SQLServerService.GetList("T_ADITAMENTO", null, "[cd_contrato]", $"[{cd_contrato}]", source);

            // Gerar sequência automática se não foi informada
            if (string.IsNullOrEmpty(ad.nm_sequencia_aditamento))
            {
              // Filtrar apenas aditamentos COM id_tipo_aditamento (excluir o aditamento vazio criado na matrícula)
              var aditamentosValidos = aditamentos_contrato.success && aditamentos_contrato.data != null
                ? aditamentos_contrato.data.Where(a => a["id_tipo_aditamento"] != null && a["id_tipo_aditamento"] != DBNull.Value).ToList()
                : new List<Dictionary<string, object>>();

              var sequencia = aditamentosValidos.Count + 1;
              dict["nm_sequencia_aditamento"] = sequencia.ToString();
            }

            var t_aditamento_Result = await SQLServerService.Insert("T_ADITAMENTO", dict, source);
            if (!t_aditamento_Result.success) continue;
            var aditamentoCadastradaGet = await SQLServerService.GetList("T_ADITAMENTO", 1, 1, "cd_aditamento", true, null, null, "", source, SearchModeEnum.Equals, null, null);
            var aditamentoCadastrado = aditamentoCadastradaGet.data.First();

            var cd_aditamento = int.Parse(aditamentoCadastrado["cd_aditamento"].ToString());
            await AddHistoricoAditamento(cd_aditamento, ad.cd_usuario, 0, source);





            //Adicionar Parcelas/material
            if (ad.id_tipo_aditamento == 5 || ad.id_tipo_aditamento == 8)
            {
              // Validar parcelas inicial e final para desconto
              var parcelaInicial = ad.nm_parcela_inicial ?? 1;
              var parcelaFinal = ad.nm_parcela_final ?? 1;

              if (!ad.TitulosMensalidade.IsNullOrEmpty())
              {
                foreach (var titulo in ad.TitulosMensalidade)
                {
                  // Converter nm_parcela_titulo para int para comparação
                  var numeroParcela = int.TryParse(titulo.nm_parcela_titulo?.ToString(), out var result) ? result : 0;

                  // VALIDAÇÃO CRÍTICA: Aplicar desconto apenas se a parcela está no intervalo [nm_parcela_inicial, nm_parcela_final]
                  var deveAplicarDesconto = numeroParcela >= parcelaInicial && numeroParcela <= parcelaFinal;
                  
                  // Se a parcela NÃO está no intervalo, ZERAR os descontos
                  if (!deveAplicarDesconto)
                  {
                    titulo.pc_desconto_mensalidade = 0;
                    titulo.vl_desconto_mensalidade = 0;
                    titulo.pc_desconto_material = 0;
                    titulo.vl_desconto_material = 0;
                    titulo.pc_desconto_total = 0;
                    titulo.vl_desconto_total = 0;
                    
                    Console.WriteLine($"[DESCONTO BLOQUEADO] Mensalidade Parcela {numeroParcela}: Fora do intervalo [{parcelaInicial}, {parcelaFinal}]");
                  }
                  else
                  {
                    Console.WriteLine($"[DESCONTO APLICADO] Mensalidade Parcela {numeroParcela}: Dentro do intervalo [{parcelaInicial}, {parcelaFinal}]");
                  }

                  // Validação e fallback para cd_pessoa_titulo e cd_pessoa_responsavel
                  var pessoaTitulo = titulo.cd_pessoa_titulo.HasValue && titulo.cd_pessoa_titulo.Value != 0 ? titulo.cd_pessoa_titulo.Value :
                                     (titulo.cd_pessoa_responsavel.HasValue && titulo.cd_pessoa_responsavel.Value != 0 ? titulo.cd_pessoa_responsavel.Value :
                                     (responsavel != null && Convert.ToInt32(responsavel) != 0 ? Convert.ToInt32(responsavel) : Convert.ToInt32(cd_pessoa_aluno)));

                  var pessoaResponsavel = titulo.cd_pessoa_responsavel.HasValue && titulo.cd_pessoa_responsavel.Value != 0 ? titulo.cd_pessoa_responsavel.Value :
                                          (responsavel != null && Convert.ToInt32(responsavel) != 0 ? Convert.ToInt32(responsavel) : Convert.ToInt32(cd_pessoa_aluno));

                  // Validação de cd_local_movto
                  var localMovto = titulo.cd_local_movto.HasValue && titulo.cd_local_movto.Value != 0
                    ? titulo.cd_local_movto.Value
                    : Convert.ToInt32(parametroExists["cd_local_movto"]);

                  // Validação de cd_tipo_financeiro
                  var tipoFinanceiro = titulo.cd_tipo_financeiro.HasValue && titulo.cd_tipo_financeiro.Value != 0
                    ? titulo.cd_tipo_financeiro.Value
                    : (cd_tipo_financeiro != null ? Convert.ToInt32(cd_tipo_financeiro) : 0);

                  // Log para debug
                  // Validação final - garantir que nenhum valor seja 0 ou inválido
                  if (pessoaTitulo == 0 || pessoaResponsavel == 0)
                  {
                    var errorMsg = $"[ERRO VALIDAÇÃO] Parcela {titulo.nm_parcela_titulo}: cd_pessoa_titulo={pessoaTitulo}, cd_pessoa_responsavel={pessoaResponsavel} - Valores não podem ser zero!";
                    Console.WriteLine(errorMsg);
                    return BadRequest(errorMsg);
                  }

                  if (tipoFinanceiro == 0)
                  {
                    var errorMsg = $"[ERRO VALIDAÇÃO] Parcela {titulo.nm_parcela_titulo}: cd_tipo_financeiro não pode ser 0. Deve ser informado um tipo financeiro válido.";
                    Console.WriteLine(errorMsg);
                    return BadRequest(errorMsg);
                  }

                  Console.WriteLine($"[DEBUG MENSALIDADE] Parcela {titulo.nm_parcela_titulo}:");
                  Console.WriteLine($"  - cd_pessoa_titulo={pessoaTitulo}");
                  Console.WriteLine($"  - cd_pessoa_responsavel={pessoaResponsavel}");
                  Console.WriteLine($"  - cd_local_movto={localMovto}");
                  Console.WriteLine($"  - cd_pessoa_empresa={cd_escola}");
                  Console.WriteLine($"  - cd_tipo_financeiro={tipoFinanceiro}");
                  Console.WriteLine($"  - cd_aluno={titulo.cd_aluno}");

                  var dictTitulo = new Dictionary<string, object>
                  {
                    ["cd_pessoa_empresa"] = cd_escola,
                    ["cd_pessoa_titulo"] = pessoaTitulo,
                    ["cd_pessoa_responsavel"] = pessoaResponsavel,
                    ["cd_local_movto"] = localMovto,
                    ["dt_emissao_titulo"] = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                    ["cd_origem_titulo"] = cd_contrato,
                    ["dt_vcto_titulo"] = titulo.dt_vcto_titulo.ToString("yyyy-MM-ddTHH:mm:ss"),
                    ["dh_cadastro_titulo"] = DateTime.Now.Date.ToString("yyyy-MM-ddTHH:mm:ss"),
                    ["vl_titulo"] = titulo.vl_titulo,
                    ["vl_saldo_titulo"] = titulo.vl_saldo_titulo,
                     ["dc_tipo_titulo"] = "AD", // ✅ FORÇAR: Títulos de mensalidade em aditamento sempre são tipo "AD" 
                    ["dc_num_documento_titulo"] = titulo.dc_num_documento_titulo,
                    ["nm_titulo"] = nm_contrato,
                    ["nm_parcela_titulo"] = titulo.nm_parcela_titulo,
                    ["cd_tipo_financeiro"] = tipoFinanceiro,
                    ["id_status_titulo"] = 1,
                    ["id_status_cnab"] = titulo.id_status_cnab,
                    ["id_origem_titulo"] = 22,
                    ["id_natureza_titulo"] = 1,
                    ["vl_material_titulo"] = titulo.vl_material_titulo,
                    ["pc_taxa_cartao"] = titulo.pc_taxa_cartao,
                    ["nm_dias_cartao"] = titulo.nm_dias_cartao,
                    ["id_cnab_contrato"] = titulo.id_cnab_contrato,
                    ["vl_taxa_cartao"] = titulo.vl_taxa_cartao,
                    ["cd_aluno"] = titulo.cd_aluno,
                    ["pc_responsavel"] = titulo.pc_responsavel == null || titulo.pc_responsavel == 0 ? 100 : titulo.pc_responsavel,
                    ["vl_mensalidade"] = titulo.vl_mensalidade,
                    ["pc_bolsa"] = titulo.pc_bolsa,
                    ["vl_bolsa"] = titulo.vl_bolsa,
                    ["pc_desconto_mensalidade"] = titulo.pc_desconto_mensalidade,
                    ["vl_desconto_mensalidade"] = titulo.vl_desconto_mensalidade,
                    ["pc_bolsa_material"] = titulo.pc_bolsa_material,
                    ["vl_bolsa_material"] = titulo.vl_bolsa_material,
                    ["pc_desconto_material"] = titulo.pc_desconto_material,
                    ["vl_desconto_material"] = titulo.vl_desconto_material,
                    ["pc_desconto_total"] = titulo.pc_desconto_total,
                    ["vl_desconto_total"] = titulo.vl_desconto_total,
                    ["opcao_venda"] = titulo.opcao_venda,
                    ["cd_curso"] = titulo.cd_curso
                  };
                  var t_titulo_Result = await SQLServerService.Insert("T_TITULO", dictTitulo, source);
                  if (!t_titulo_Result.success) return BadRequest(t_titulo_Result.error);

                  var t_tituloGet = await SQLServerService.GetList("T_TITULO", 1, 1, "cd_titulo", true, null, null, "", source, SearchModeEnum.Equals, null, null);
                  var titulo_inserido = t_tituloGet.data.First();

                  //cadastrar vinculo entre titulo e aditamento
                  var dict_aditamento_titulo = new Dictionary<string, object>
                  {
                    ["cd_aditamento"] = cd_aditamento,
                    ["cd_titulo"] = titulo_inserido["cd_titulo"]
                  };
                  var result_titulo_aditamento = await SQLServerService.Insert("T_TITULO_ADITAMENTO", dict_aditamento_titulo, source);
                  if (!result_titulo_aditamento.success) return BadRequest(result_titulo_aditamento.error);

                  var id_origem_titulo = titulo_inserido["id_origem_titulo"]?.ToString() ?? "0";
                  var dc_tipo_titulo_salvo = titulo_inserido["dc_tipo_titulo"]?.ToString() ?? "";

                  // ✅ Títulos de aditamento (AD) devem ser associados ao plano de contas de mensalidade
                  if (id_origem_titulo == "22" && (dc_tipo_titulo_salvo == "ME" || dc_tipo_titulo_salvo == "AD"))
                  {
                    //T_plano_titulo
                    var dict_plano = new Dictionary<string, object>
                    {
                      ["cd_titulo"] = titulo_inserido["cd_titulo"],
                      ["cd_plano_conta"] = cd_plano_conta_mat,
                      ["vl_plano_titulo"] = titulo.vl_mensalidade
                    };
                    var t_plano_titulo_Result = await SQLServerService.Insert("T_PLANO_TITULO", dict_plano, source);
                    if (!t_plano_titulo_Result.success) return BadRequest(t_plano_titulo_Result.error);
                  }

                  if (id_origem_titulo == "22" && (dc_tipo_titulo_salvo == "ME" || dc_tipo_titulo_salvo == "AD") && titulo.vl_material_titulo > 0)
                  {
                    //T_plano_titulo
                    var dict_plano = new Dictionary<string, object>
                    {
                      ["cd_titulo"] = titulo_inserido["cd_titulo"],
                      ["cd_plano_conta"] = cd_plano_conta_mtr,
                      ["vl_plano_titulo"] = titulo.vl_material_titulo
                    };
                    var t_plano_titulo_Result = await SQLServerService.Insert("T_PLANO_TITULO", dict_plano, source);
                    if (!t_plano_titulo_Result.success) return BadRequest(t_plano_titulo_Result.error);
                  }
                }
              }

              if (!ad.TitulosMaterial.IsNullOrEmpty())
              {
                foreach (var titulo in ad.TitulosMaterial)
                {
                  // Converter nm_parcela_titulo para int para comparação
                  var numeroParcela = int.TryParse(titulo.nm_parcela_titulo?.ToString(), out var result) ? result : 0;

                  // VALIDAÇÃO CRÍTICA: Aplicar desconto apenas se a parcela está no intervalo [nm_parcela_inicial, nm_parcela_final]
                  var deveAplicarDesconto = numeroParcela >= parcelaInicial && numeroParcela <= parcelaFinal;
                  
                  // Se a parcela NÃO está no intervalo, ZERAR os descontos
                  if (!deveAplicarDesconto)
                  {
                    titulo.pc_desconto_mensalidade = 0;
                    titulo.vl_desconto_mensalidade = 0;
                    titulo.pc_desconto_material = 0;
                    titulo.vl_desconto_material = 0;
                    titulo.pc_desconto_total = 0;
                    titulo.vl_desconto_total = 0;
                    
                    Console.WriteLine($"[DESCONTO BLOQUEADO] Material Parcela {numeroParcela}: Fora do intervalo [{parcelaInicial}, {parcelaFinal}]");
                  }
                  else
                  {
                    Console.WriteLine($"[DESCONTO APLICADO] Material Parcela {numeroParcela}: Dentro do intervalo [{parcelaInicial}, {parcelaFinal}]");
                  }

                  // Validação e fallback para cd_pessoa_titulo e cd_pessoa_responsavel
                  // Para títulos de MATERIAL, usar responsavel_material como fallback
                  var pessoaTitulo = titulo.cd_pessoa_titulo.HasValue && titulo.cd_pessoa_titulo.Value != 0 ? titulo.cd_pessoa_titulo.Value :
                                     (titulo.cd_pessoa_responsavel.HasValue && titulo.cd_pessoa_responsavel.Value != 0 ? titulo.cd_pessoa_responsavel.Value :
                                     (responsavel_material != null && Convert.ToInt32(responsavel_material) != 0 ? Convert.ToInt32(responsavel_material) :
                                     (responsavel != null && Convert.ToInt32(responsavel) != 0 ? Convert.ToInt32(responsavel) : Convert.ToInt32(cd_pessoa_aluno))));

                  var pessoaResponsavel = titulo.cd_pessoa_responsavel.HasValue && titulo.cd_pessoa_responsavel.Value != 0 ? titulo.cd_pessoa_responsavel.Value :
                                          (responsavel_material != null && Convert.ToInt32(responsavel_material) != 0 ? Convert.ToInt32(responsavel_material) :
                                          (responsavel != null && Convert.ToInt32(responsavel) != 0 ? Convert.ToInt32(responsavel) : Convert.ToInt32(cd_pessoa_aluno)));

                  // Validação de cd_local_movto
                  var localMovto = titulo.cd_local_movto.HasValue && titulo.cd_local_movto.Value != 0
                    ? titulo.cd_local_movto.Value
                    : Convert.ToInt32(parametroExists["cd_local_movto"]);

                  // Validação de cd_tipo_financeiro
                  var tipoFinanceiro = titulo.cd_tipo_financeiro.HasValue && titulo.cd_tipo_financeiro.Value != 0
                    ? titulo.cd_tipo_financeiro.Value
                    : (cd_tipo_financeiro != null ? Convert.ToInt32(cd_tipo_financeiro) : 0);

                  // Validação final
                  if (pessoaTitulo == 0 || pessoaResponsavel == 0)
                  {
                    var errorMsg = $"[ERRO VALIDAÇÃO MATERIAL] Parcela {titulo.nm_parcela_titulo}: cd_pessoa_titulo={pessoaTitulo}, cd_pessoa_responsavel={pessoaResponsavel} - Valores não podem ser zero!";
                    Console.WriteLine(errorMsg);
                    return BadRequest(errorMsg);
                  }

                  if (tipoFinanceiro == 0)
                  {
                    var errorMsg = $"[ERRO VALIDAÇÃO MATERIAL] Parcela {titulo.nm_parcela_titulo}: cd_tipo_financeiro não pode ser 0. Deve ser informado um tipo financeiro válido.";
                    Console.WriteLine(errorMsg);
                    return BadRequest(errorMsg);
                  }

                  var dictTitulo = new Dictionary<string, object>
                  {
                    ["cd_pessoa_empresa"] = cd_escola,
                    ["cd_pessoa_titulo"] = pessoaTitulo,
                    ["cd_pessoa_responsavel"] = pessoaResponsavel,
                    ["cd_local_movto"] = localMovto,
                    ["dt_emissao_titulo"] = titulo.dt_emissao_titulo.ToString("yyyy-MM-ddTHH:mm:ss"),
                    ["cd_origem_titulo"] = cd_contrato,
                    ["dt_vcto_titulo"] = titulo.dt_vcto_titulo.ToString("yyyy-MM-ddTHH:mm:ss"),
                    ["dh_cadastro_titulo"] = DateTime.Now.Date,
                    ["vl_titulo"] = titulo.vl_titulo,
                    ["vl_saldo_titulo"] = titulo.vl_saldo_titulo,
                     ["dc_tipo_titulo"] = "AD", // ✅ FORÇAR: Títulos de material em aditamento sempre são tipo "AD"
                    ["dc_num_documento_titulo"] = titulo.dc_num_documento_titulo,
                    ["nm_titulo"] = nm_contrato,
                    ["nm_parcela_titulo"] = titulo.nm_parcela_titulo,
                    ["cd_tipo_financeiro"] = tipoFinanceiro,
                    ["id_status_titulo"] = 1,
                    ["id_status_cnab"] = titulo.id_status_cnab,
                    ["id_origem_titulo"] = 22,
                    ["id_natureza_titulo"] = 1,
                    ["vl_material_titulo"] = titulo.vl_material_titulo,
                    ["pc_taxa_cartao"] = titulo.pc_taxa_cartao,
                    ["nm_dias_cartao"] = titulo.nm_dias_cartao,
                    ["id_cnab_contrato"] = titulo.id_cnab_contrato,
                    ["vl_taxa_cartao"] = titulo.vl_taxa_cartao,
                    ["cd_aluno"] = titulo.cd_aluno,
                    ["pc_responsavel"] = titulo.pc_responsavel == null || titulo.pc_responsavel == 0 ? 100 : titulo.pc_responsavel,
                    ["vl_mensalidade"] = titulo.vl_mensalidade,
                    ["pc_bolsa"] = titulo.pc_bolsa,
                    ["vl_bolsa"] = titulo.vl_bolsa,
                    ["pc_desconto_mensalidade"] = titulo.pc_desconto_mensalidade,
                    ["vl_desconto_mensalidade"] = titulo.vl_desconto_mensalidade,
                    ["pc_bolsa_material"] = titulo.pc_bolsa_material,
                    ["vl_bolsa_material"] = titulo.vl_bolsa_material,
                    ["pc_desconto_material"] = titulo.pc_desconto_material,
                    ["vl_desconto_material"] = titulo.vl_desconto_material,
                    ["pc_desconto_total"] = titulo.pc_desconto_total,
                    ["vl_desconto_total"] = titulo.vl_desconto_total,
                    ["opcao_venda"] = titulo.opcao_venda,
                    ["cd_curso"] = titulo.cd_curso
                  };
                  var t_titulo_Result = await SQLServerService.Insert("T_TITULO", dictTitulo, source);
                  if (!t_titulo_Result.success) return BadRequest(t_titulo_Result.error);
                  var titulo_inseridoGet = await SQLServerService.GetList("T_TITULO", 1, 1, "cd_titulo", true, null, null, "", source, SearchModeEnum.Equals, null, null);
                  var titulo_inserido = titulo_inseridoGet.data.First();

                  //cadastrar vinculo entre titulo e aditamento
                  var dict_aditamento_titulo = new Dictionary<string, object>
                  {
                    ["cd_aditamento"] = cd_aditamento,
                    ["cd_titulo"] = titulo_inserido["cd_titulo"]
                  };
                  var result_titulo_aditamento = await SQLServerService.Insert("T_TITULO_ADITAMENTO", dict_aditamento_titulo, source);
                  if (!result_titulo_aditamento.success) return BadRequest(result_titulo_aditamento.error);

                  var id_origem_titulo = titulo_inserido["id_origem_titulo"]?.ToString() ?? "0";
                  var dc_tipo_titulo_salvo = titulo_inserido["dc_tipo_titulo"]?.ToString() ?? "";

                  // ✅ Títulos de aditamento (AD) de material devem ser associados ao plano de contas de material
                  if (id_origem_titulo == "22" && (dc_tipo_titulo_salvo == "MT" || dc_tipo_titulo_salvo == "AD"))
                  {
                    //T_plano_titulo
                    var dict_plano = new Dictionary<string, object>
                    {
                      ["cd_titulo"] = titulo_inserido["cd_titulo"],
                      ["cd_plano_conta"] = cd_plano_conta_mtr,
                      ["vl_plano_titulo"] = titulo.vl_titulo
                    };
                    var t_plano_titulo_Result = await SQLServerService.Insert("T_PLANO_TITULO", dict_plano, source);
                    if (!t_plano_titulo_Result.success) return BadRequest(t_plano_titulo_Result.error);
                  }
                }
              }

              if (!ad.TitulosTaxa.IsNullOrEmpty())
              {
                foreach (var titulo in ad.TitulosTaxa)
                {
                  // ⚠️ CRÍTICO: TAXA NUNCA DEVE RECEBER DESCONTO!
                  // Zerar TODOS os descontos para títulos de taxa
                  titulo.pc_desconto_mensalidade = 0;
                  titulo.vl_desconto_mensalidade = 0;
                  titulo.pc_desconto_material = 0;
                  titulo.vl_desconto_material = 0;
                  titulo.pc_desconto_total = 0;
                  titulo.vl_desconto_total = 0;
                  
                  Console.WriteLine($"[BLOQUEIO TAXA] Parcela {titulo.nm_parcela_titulo}: Todos os descontos zerados (Taxa nunca deve ter desconto)");

                  // ✅ VERIFICAÇÃO DE DUPLICAÇÃO: Não inserir títulos TX que já existem
                  var tituloExistenteResult = await SQLServerService.GetList(
                    "T_TITULO",
                    null,
                    "[cd_origem_titulo],[dc_tipo_titulo],[nm_parcela_titulo],[cd_pessoa_empresa]",
                    $"[{cd_contrato}],[TX],[{titulo.nm_parcela_titulo}],[{cd_escola}]",
                    source,
                    SearchModeEnum.Equals
                  );

                  if (tituloExistenteResult.success && tituloExistenteResult.data != null && tituloExistenteResult.data.Any())
                  {
                    var tituloExistente = tituloExistenteResult.data.First();
                    var cd_titulo_existente = tituloExistente["cd_titulo"];
                    Console.WriteLine($"[DUPLICAÇÃO EVITADA] Título TX parcela {titulo.nm_parcela_titulo} já existe (cd_titulo: {cd_titulo_existente}). Pulando inserção e vinculando ao aditamento.");

                    // Apenas vincular o título existente ao aditamento
                    var dict_aditamento_titulo_existente = new Dictionary<string, object>
                    {
                      ["cd_aditamento"] = cd_aditamento,
                      ["cd_titulo"] = cd_titulo_existente
                    };
                    var result_titulo_aditamento_existente = await SQLServerService.Insert("T_TITULO_ADITAMENTO", dict_aditamento_titulo_existente, source);
                    if (!result_titulo_aditamento_existente.success)
                    {
                      Console.WriteLine($"[AVISO] Não foi possível vincular título existente {cd_titulo_existente} ao aditamento {cd_aditamento}: {result_titulo_aditamento_existente.error}");
                    }

                    continue; // Pular para o próximo título
                  }

                  // Validação e fallback para cd_pessoa_titulo e cd_pessoa_responsavel
                  var pessoaTitulo = titulo.cd_pessoa_titulo.HasValue && titulo.cd_pessoa_titulo.Value != 0 ? titulo.cd_pessoa_titulo.Value :
                                     (titulo.cd_pessoa_responsavel.HasValue && titulo.cd_pessoa_responsavel.Value != 0 ? titulo.cd_pessoa_responsavel.Value :
                                     (responsavel != null && Convert.ToInt32(responsavel) != 0 ? Convert.ToInt32(responsavel) : Convert.ToInt32(cd_pessoa_aluno)));

                  var pessoaResponsavel = titulo.cd_pessoa_responsavel.HasValue && titulo.cd_pessoa_responsavel.Value != 0 ? titulo.cd_pessoa_responsavel.Value :
                                          (responsavel != null && Convert.ToInt32(responsavel) != 0 ? Convert.ToInt32(responsavel) : Convert.ToInt32(cd_pessoa_aluno));

                  // Validação de cd_local_movto
                  var localMovto = titulo.cd_local_movto.HasValue && titulo.cd_local_movto.Value != 0
                    ? titulo.cd_local_movto.Value
                    : Convert.ToInt32(parametroExists["cd_local_movto"]);

                  // Validação de cd_tipo_financeiro
                  var tipoFinanceiro = titulo.cd_tipo_financeiro.HasValue && titulo.cd_tipo_financeiro.Value != 0
                    ? titulo.cd_tipo_financeiro.Value
                    : (cd_tipo_financeiro != null ? Convert.ToInt32(cd_tipo_financeiro) : 0);

                  // Validação final
                  if (pessoaTitulo == 0 || pessoaResponsavel == 0)
                  {
                    var errorMsg = $"[ERRO VALIDAÇÃO TAXA] Parcela {titulo.nm_parcela_titulo}: cd_pessoa_titulo={pessoaTitulo}, cd_pessoa_responsavel={pessoaResponsavel} - Valores não podem ser zero!";
                    Console.WriteLine(errorMsg);
                    return BadRequest(errorMsg);
                  }

                  if (tipoFinanceiro == 0)
                  {
                    var errorMsg = $"[ERRO VALIDAÇÃO TAXA] Parcela {titulo.nm_parcela_titulo}: cd_tipo_financeiro não pode ser 0. Deve ser informado um tipo financeiro válido.";
                    Console.WriteLine(errorMsg);
                    return BadRequest(errorMsg);
                  }

                  var dictTitulo = new Dictionary<string, object>
                  {
                    ["cd_pessoa_empresa"] = cd_escola,
                    ["cd_pessoa_titulo"] = pessoaTitulo,
                    ["cd_pessoa_responsavel"] = pessoaResponsavel,
                    ["cd_local_movto"] = localMovto,
                    ["dt_emissao_titulo"] = titulo.dt_emissao_titulo.ToString("yyyy-MM-ddTHH:mm:ss"),
                    ["cd_origem_titulo"] = cd_contrato,
                    ["dt_vcto_titulo"] = titulo.dt_vcto_titulo.ToString("yyyy-MM-ddTHH:mm:ss"),
                    ["dh_cadastro_titulo"] = DateTime.Now.Date,
                    ["vl_titulo"] = titulo.vl_titulo,
                    ["vl_saldo_titulo"] = titulo.vl_saldo_titulo,
                     ["dc_tipo_titulo"] = titulo.dc_tipo_titulo, // ✅ CORRIGIDO: Títulos de aditamento devem ser "AD", não "TX"
                    ["dc_num_documento_titulo"] = titulo.dc_num_documento_titulo,
                    ["nm_titulo"] = nm_contrato,
                    ["nm_parcela_titulo"] = titulo.nm_parcela_titulo,
                    ["cd_tipo_financeiro"] = tipoFinanceiro,
                    ["id_status_titulo"] = 1,
                    ["id_status_cnab"] = titulo.id_status_cnab,
                    ["id_origem_titulo"] = 22,
                    ["id_natureza_titulo"] = 1,
                    ["vl_material_titulo"] = titulo.vl_material_titulo,
                    ["pc_taxa_cartao"] = titulo.pc_taxa_cartao,
                    ["nm_dias_cartao"] = titulo.nm_dias_cartao,
                    ["id_cnab_contrato"] = titulo.id_cnab_contrato,
                    ["vl_taxa_cartao"] = titulo.vl_taxa_cartao,
                    ["cd_aluno"] = titulo.cd_aluno,
                    ["pc_responsavel"] = titulo.pc_responsavel == null || titulo.pc_responsavel == 0 ? 100 : titulo.pc_responsavel,
                    ["vl_mensalidade"] = titulo.vl_mensalidade,
                    ["pc_bolsa"] = titulo.pc_bolsa,
                    ["vl_bolsa"] = titulo.vl_bolsa,
                    ["pc_desconto_mensalidade"] = titulo.pc_desconto_mensalidade,
                    ["vl_desconto_mensalidade"] = titulo.vl_desconto_mensalidade,
                    ["pc_bolsa_material"] = titulo.pc_bolsa_material,
                    ["vl_bolsa_material"] = titulo.vl_bolsa_material,
                    ["pc_desconto_material"] = titulo.pc_desconto_material,
                    ["vl_desconto_material"] = titulo.vl_desconto_material,
                    ["pc_desconto_total"] = titulo.pc_desconto_total,
                    ["vl_desconto_total"] = titulo.vl_desconto_total,
                    ["opcao_venda"] = titulo.opcao_venda,
                    ["cd_curso"] = titulo.cd_curso
                  };
                  var t_titulo_Result = await SQLServerService.Insert("T_TITULO", dictTitulo, source);
                  if (!t_titulo_Result.success) return BadRequest(t_titulo_Result.error);
                  var titulo_inseridoGet = await SQLServerService.GetList("T_TITULO", 1, 1, "cd_titulo", true, null, null, "", source, SearchModeEnum.Equals, null, null);
                  var titulo_inserido = titulo_inseridoGet.data.First();

                  //cadastrar vinculo entre titulo e aditamento
                  var dict_aditamento_titulo = new Dictionary<string, object>
                  {
                    ["cd_aditamento"] = cd_aditamento,
                    ["cd_titulo"] = titulo_inserido["cd_titulo"]
                  };
                  var result_titulo_aditamento = await SQLServerService.Insert("T_TITULO_ADITAMENTO", dict_aditamento_titulo, source);
                  if (!result_titulo_aditamento.success) return BadRequest(result_titulo_aditamento.error);

                  var id_origem_titulo = titulo_inserido["id_origem_titulo"]?.ToString() ?? "0";

                  if (id_origem_titulo == "22" && titulo.dc_tipo_titulo == "TX")
                  {
                    //T_plano_titulo
                    var dict_plano = new Dictionary<string, object>
                    {
                      ["cd_titulo"] = titulo_inserido["cd_titulo"],
                      ["cd_plano_conta"] = cd_plano_conta_tax,
                      ["vl_plano_titulo"] = titulo.vl_titulo
                    };
                    var t_plano_titulo_Result = await SQLServerService.Insert("T_PLANO_TITULO", dict_plano, source);
                    if (!t_plano_titulo_Result.success) return BadRequest(t_plano_titulo_Result.error);
                  }
                }
              }


            }

            //perda de desconto
            if (ad.id_tipo_aditamento == 2)
            {
              if (dict_contrato.ContainsKey("pc_desconto_bolsa"))
              {
                dict_contrato["pc_desconto_bolsa"] = 0;
              }
              else
                dict_contrato.Add("pc_desconto_bolsa", 0);

              if (dict_contrato.ContainsKey("pc_bolsa_material"))
              {
                dict_contrato["pc_bolsa_material"] = 0;
              }
              else
                dict_contrato.Add("pc_bolsa_material", 0);

              if (dict_contrato.ContainsKey("vl_desconto_material"))
              {
                dict_contrato["vl_desconto_material"] = 0;
              }
              else
                dict_contrato.Add("vl_desconto_material", 0);


              if (dict_contrato.ContainsKey("vl_desconto_contrato"))
              {
                dict_contrato["vl_desconto_contrato"] = 0;
              }
              else
                dict_contrato.Add("vl_desconto_contrato", 0);


              if (dict_contrato.ContainsKey("pc_desconto_contrato"))
              {
                dict_contrato["pc_desconto_contrato"] = 0;
              }
              else dict_contrato.Add("pc_desconto_contrato", 0);

              var result = await SQLServerService.Delete("T_DESCONTO_CONTRATO", "cd_contrato", cd_contrato.ToString(), source);
              if (!result.success) return BadRequest(result.error);

              // BUSCAR APENAS TÍTULOS NÃO PAGOS (id_status_titulo = 1) - mesma lógica do LEGADO
              var todosTitulosResult = await SQLServerService.GetList("T_TITULO", null, "[cd_origem_titulo],[id_status_titulo]", $"[{cd_contrato}],[1]", source, SearchModeEnum.Equals);

              if (todosTitulosResult.success && todosTitulosResult.data != null && todosTitulosResult.data.Any())
              {
                Console.WriteLine($"[INFO] Tipo 2 - Encontrados {todosTitulosResult.data.Count} títulos NÃO PAGOS para zerar bolsa/desconto");

                foreach (var tituloDb in todosTitulosResult.data)
                {
                  var cd_titulo = tituloDb["cd_titulo"];
                  var vl_titulo = Convert.ToDecimal(tituloDb["vl_titulo"]);
                  var vl_material_titulo = Convert.ToDecimal(tituloDb["vl_material_titulo"] ?? 0);

                  // FÓRMULA DO LEGADO: vl_saldo = vl_titulo - (vl_titulo - vl_material) * 0 / 100 = vl_titulo
                  var vl_saldo_titulo = vl_titulo - decimal.Round((vl_titulo - vl_material_titulo) * 0 / 100, 2);

                  var dictTituloZerar = new Dictionary<string, object>
                  {
                    { "vl_saldo_titulo", vl_saldo_titulo },  // ✅ ADICIONAR - Volta ao valor cheio!
                    { "pc_bolsa", 0 },
                    { "vl_bolsa", 0 },
                    { "pc_bolsa_material", 0 },
                    { "vl_bolsa_material", 0 },
                    { "pc_desconto_mensalidade", 0 },
                    { "vl_desconto_mensalidade", 0 },
                    { "pc_desconto_material", 0 },
                    { "vl_desconto_material", 0 }
                  };

                  var updateResult = await SQLServerService.Update("T_TITULO", dictTituloZerar, source, "cd_titulo", cd_titulo);
                  if (!updateResult.success)
                  {
                    Console.WriteLine($"[ERROR] Falha ao atualizar título {cd_titulo}: {updateResult.error}");
                  }
                  else
                  {
                    Console.WriteLine($"[SUCCESS] Título {cd_titulo} atualizado - vl_saldo_titulo voltou para {vl_saldo_titulo}");
                  }
                }
              }
              else
              {
                Console.WriteLine($"[WARNING] Tipo 2 - Nenhum título NÃO PAGO encontrado para o contrato {cd_contrato}");
              }

              // DELETAR baixas parciais de bolsa (cd_tipo_liquidacao = 100 MOTIVO_BOLSA) - Mesma lógica do LEGADO
              var deletarBaixasResult = await DeletarBaixasBolsaTituloContrato(cd_contrato, cd_escola, source);
              if (!deletarBaixasResult.success)
              {
                Console.WriteLine($"[WARNING] Erro ao deletar baixas de bolsa: {deletarBaixasResult.error}");
              }
              else
              {
                Console.WriteLine($"[SUCCESS] Baixas parciais de bolsa deletadas com sucesso");
              }
            }

            //concessão desconto
            if (ad.id_tipo_aditamento == 3)
            {
              Console.WriteLine($"[DEBUG] Tipo 3 detectado - Processando múltiplos descontos");

              // Lista de descontos a processar (com suporte a múltiplos descontos ou desconto único)
              var descontosParaProcessar = new List<Models.Matricula.MatriculaUpdateAditamentosModel.DescontoModel>();

              // Verificar se há múltiplos descontos no novo formato
              if (ad.Descontos != null && ad.Descontos.Any())
              {
                Console.WriteLine($"[DEBUG] Encontrados {ad.Descontos.Count} descontos no formato novo (array Descontos)");
                descontosParaProcessar.AddRange(ad.Descontos);
              }
              // Retrocompatibilidade: se não houver array Descontos, usar campos legados
              else if (ad.pc_desconto_contrato != null || ad.vl_desconto_contrato != null)
              {
                Console.WriteLine($"[DEBUG] Usando formato legado (campos únicos de desconto)");
                descontosParaProcessar.Add(new Models.Matricula.MatriculaUpdateAditamentosModel.DescontoModel
                {
                  cd_tipo_desconto = ad.cd_tipo_desconto,
                  pc_desconto_contrato = ad.pc_desconto_contrato,
                  vl_desconto_contrato = ad.vl_desconto_contrato,
                  id_incide_matricula = ad.id_incide_matricula,
                  id_incide_material = ad.id_incide_material,
                  id_incide_baixa = ad.id_incide_baixa,
                  nm_parcela_inicial = ad.nm_parcela_inicial,
                  nm_parcela_final = ad.nm_parcela_final
                });
              }

              // Processar cada desconto
              foreach (var desconto in descontosParaProcessar)
              {
                Console.WriteLine($"[DEBUG] Processando desconto: {desconto.pc_desconto_contrato}% nas parcelas {desconto.nm_parcela_inicial} a {desconto.nm_parcela_final}");

                // Inserir na tabela T_DESCONTO_CONTRATO
                var desconto_contrato = new Dictionary<string, object>
                {
                  { "cd_tipo_desconto", desconto.cd_tipo_desconto ?? 146 },
                  { "pc_desconto_contrato", desconto.pc_desconto_contrato ?? 0 },
                  { "id_desconto_ativo", 1 },
                  { "vl_desconto_contrato", desconto.vl_desconto_contrato ?? 0 },
                  { "id_incide_baixa", desconto.id_incide_baixa ?? false },
                  { "nm_parcela_ini", desconto.nm_parcela_inicial ?? 1 },
                  { "nm_parcela_fim", desconto.nm_parcela_final ?? 1 },
                  { "id_incide_parcela_1", 0 },
                  { "id_aditamento", 1 },
                  { "cd_contrato", cd_contrato },
                  { "cd_aditamento", cd_aditamento }
                };

                var t_desconto_contrato_Result = await SQLServerService.Insert("T_DESCONTO_CONTRATO", desconto_contrato, source);
                Console.WriteLine($"[DEBUG] INSERT result - success: {t_desconto_contrato_Result.success}, error: {t_desconto_contrato_Result.error}");

                if (!t_desconto_contrato_Result.success)
                {
                  Console.WriteLine($"[ERROR] Falha ao inserir em T_DESCONTO_CONTRATO: {t_desconto_contrato_Result.error}");
                  return BadRequest($"Erro ao criar desconto no contrato: {t_desconto_contrato_Result.error}");
                }
              }

              // Atualizar títulos NÃO PAGOS com os descontos aplicados (acumulados)
              Console.WriteLine($"[INFO] Tipo 3 - Buscando títulos NÃO PAGOS para aplicar descontos");
              var titulosNaoPagosResult = await SQLServerService.GetList("T_TITULO", null, "[cd_origem_titulo],[id_status_titulo]", $"[{cd_contrato}],[1]", source, SearchModeEnum.Equals);

              if (titulosNaoPagosResult.success && titulosNaoPagosResult.data != null && titulosNaoPagosResult.data.Any())
              {
                Console.WriteLine($"[INFO] Tipo 3 - Encontrados {titulosNaoPagosResult.data.Count} títulos NÃO PAGOS");

                // Agrupar descontos por parcela para acumular percentuais
                var descontosPorParcela = new Dictionary<int, decimal>();

                foreach (var desconto in descontosParaProcessar)
                {
                  if (desconto.pc_desconto_contrato == null || desconto.pc_desconto_contrato <= 0) continue;

                  int nm_parcela_ini = desconto.nm_parcela_inicial ?? 1;
                  int nm_parcela_fim = desconto.nm_parcela_final ?? int.MaxValue;

                  for (int parcela = nm_parcela_ini; parcela <= nm_parcela_fim; parcela++)
                  {
                    if (!descontosPorParcela.ContainsKey(parcela))
                      descontosPorParcela[parcela] = 0;

                    descontosPorParcela[parcela] += Convert.ToDecimal(desconto.pc_desconto_contrato);
                  }
                }

                Console.WriteLine($"[DEBUG] Mapa de descontos acumulados por parcela:");
                foreach (var kvp in descontosPorParcela)
                {
                  Console.WriteLine($"  Parcela {kvp.Key}: {kvp.Value}% acumulado");
                }

                // Aplicar descontos acumulados nos títulos
                foreach (var titulo in titulosNaoPagosResult.data)
                {
                  var nm_parcela_titulo = Convert.ToInt32(titulo["nm_parcela_titulo"]);

                  // Verificar se há desconto para esta parcela
                  if (!descontosPorParcela.ContainsKey(nm_parcela_titulo))
                  {
                    Console.WriteLine($"[INFO] Título {titulo["cd_titulo"]} (parcela {nm_parcela_titulo}) sem desconto - pulado");
                    continue;
                  }

                  // Respeitar tipo do título
                  var tipoTitulo = (titulo.ContainsKey("dc_tipo_titulo") && titulo["dc_tipo_titulo"] != null)
                    ? titulo["dc_tipo_titulo"].ToString().ToUpperInvariant()
                    : string.Empty;

                  // TAXA (TX) nunca deve receber desconto
                  if (tipoTitulo == "TX")
                  {
                    Console.WriteLine($"[INFO] Título {titulo["cd_titulo"]} (tipo TX) não permite desconto - pulado");
                    continue;
                  }

                  var cd_titulo = titulo["cd_titulo"];
                  var vl_titulo = Convert.ToDecimal(titulo["vl_titulo"]);
                  var vl_material_titulo = Convert.ToDecimal(titulo["vl_material_titulo"] ?? 0);
                  var pc_desconto_acumulado = descontosPorParcela[nm_parcela_titulo];

                  // FÓRMULA: vl_desconto = (vl_titulo - vl_material) * pc_desconto_acumulado / 100
                  var vl_desconto_mensalidade = decimal.Round((vl_titulo - vl_material_titulo) * pc_desconto_acumulado / 100, 2);
                  var vl_saldo_titulo = vl_titulo - vl_desconto_mensalidade;

                  var dictTituloAtualizar = new Dictionary<string, object>
                  {
                    { "pc_desconto_mensalidade", pc_desconto_acumulado },
                    { "vl_desconto_mensalidade", vl_desconto_mensalidade },
                    { "vl_saldo_titulo", vl_saldo_titulo }
                  };

                  var updateResult = await SQLServerService.Update("T_TITULO", dictTituloAtualizar, source, "cd_titulo", cd_titulo);

                  if (updateResult.success)
                  {
                    Console.WriteLine($"[SUCCESS] Título {cd_titulo} (parcela {nm_parcela_titulo}) atualizado - vl_saldo: {vl_saldo_titulo}, pc_desconto: {pc_desconto_acumulado}%");
                  }
                  else
                  {
                    Console.WriteLine($"[ERROR] Falha ao atualizar título {cd_titulo}: {updateResult.error}");
                  }
                }
              }
              else
              {
                Console.WriteLine($"[WARNING] Tipo 3 - Nenhum título NÃO PAGO encontrado para o contrato {cd_contrato}");
              }
            }
            //bolsa
            if (ad.id_tipo_aditamento == 7)
            {
              if (ad.pc_bolsa != null && ad.pc_bolsa > 0)
              {
                dict_bolsa.Add("cd_aditamento", cd_aditamento);
                var t_aditamento_bolsa_Result = await SQLServerService.Insert("T_ADITAMENTO_BOLSA", dict_bolsa, source);
                if (!t_aditamento_bolsa_Result.success) continue;

                Console.WriteLine($"[INFO] Tipo 7 - Atualizando títulos NÃO PAGOS com bolsa de {ad.pc_bolsa}%");

                // Buscar títulos NÃO PAGOS do contrato (mesma lógica do LEGADO)
                var titulosNaoPagosResult = await SQLServerService.GetList("T_TITULO", null, "[cd_origem_titulo],[id_status_titulo]", $"[{cd_contrato}],[1]", source, SearchModeEnum.Equals);

                if (titulosNaoPagosResult.success && titulosNaoPagosResult.data != null && titulosNaoPagosResult.data.Any())
                {
                  Console.WriteLine($"[INFO] Tipo 7 - Encontrados {titulosNaoPagosResult.data.Count} títulos NÃO PAGOS");

                  foreach (var titulo in titulosNaoPagosResult.data)
                  {
                    // Filtrar por data de comunicado (só títulos com vcto >= data comunicado)
                    var dtVctoTitulo = Convert.ToDateTime(titulo["dt_vcto_titulo"]);
                    if (ad.dt_comunicado_bolsa != null && dtVctoTitulo < ad.dt_comunicado_bolsa.Value)
                    {
                      Console.WriteLine($"[INFO] Título {titulo["cd_titulo"]} pulado - vencimento anterior à data do comunicado");
                      continue;
                    }

                    var cd_titulo = titulo["cd_titulo"];
                    var vl_titulo = Convert.ToDecimal(titulo["vl_titulo"]);
                    var vl_material_titulo = Convert.ToDecimal(titulo["vl_material_titulo"] ?? 0);
                    var pc_bolsa = Convert.ToDecimal(ad.pc_bolsa);

                    // FÓRMULA DO LEGADO (Partial/Titulo.cs:545)
                    var vl_bolsa = decimal.Round((vl_titulo - vl_material_titulo) * pc_bolsa / 100, 2);
                    var vl_saldo_titulo = vl_titulo - vl_bolsa;

                    var dictTituloAtualizar = new Dictionary<string, object>
                    {
                      { "pc_bolsa", pc_bolsa },
                      { "vl_bolsa", vl_bolsa },
                      { "vl_saldo_titulo", vl_saldo_titulo }
                    };

                    var updateResult = await SQLServerService.Update("T_TITULO", dictTituloAtualizar, source, "cd_titulo", cd_titulo);

                    if (updateResult.success)
                    {
                      Console.WriteLine($"[SUCCESS] Título {cd_titulo} atualizado - vl_saldo: {vl_saldo_titulo}, pc_bolsa: {pc_bolsa}%");
                    }
                    else
                    {
                      Console.WriteLine($"[ERROR] Falha ao atualizar título {cd_titulo}: {updateResult.error}");
                    }
                  }
                }
                else
                {
                  Console.WriteLine($"[WARNING] Tipo 7 - Nenhum título NÃO PAGO encontrado para o contrato {cd_contrato}");
                }
              }
            }

          }

          if (dict_contrato.Any()) await SQLServerService.Update("T_CONTRATO", dict_contrato, source, "cd_contrato", cd_contrato);

          // EXECUTAR baixa automática BASEADO NA LÓGICA DO LEGADO
          // Executa SE:
          // - Tipo 7 (ADITIVO_BOLSA) com pc_bolsa > 0
          // - Qualquer aditamento com pc_bolsa > 0 (não zerar)
          // NÃO executa SE:
          // - Tipo 2 (PERDA_DESCONTO) - pc_bolsa = 0
          // - Aditamento sem bolsa
          bool temConcessaoBolsa = model.Any(ad =>
            (ad.id_tipo_aditamento == 7 && ad.pc_bolsa != null && ad.pc_bolsa > 0) ||
            (ad.pc_bolsa != null && ad.pc_bolsa > 0)
          );

          bool temPerdaBolsa = model.Any(ad => ad.id_tipo_aditamento == 2);

          if (temConcessaoBolsa && !temPerdaBolsa)
          {
            Console.WriteLine($"[INFO] Executando baixa automática de bolsa para contrato {cd_contrato}");
            var resultado = await BaixaAutomaticaBolsaAluno(int.Parse(cd_contrato.ToString()), source);
            if (!resultado.success)
            {
              Console.WriteLine($"[WARNING] Erro na baixa automática de bolsa: {resultado.error}");
              // Não retorna erro, apenas loga (mesma lógica do legado)
            }
          }
          else
          {
            Console.WriteLine($"[INFO] Baixa automática NÃO executada - temConcessaoBolsa: {temConcessaoBolsa}, temPerdaBolsa: {temPerdaBolsa}");
          }

          return ResponseDefault();
        }
      }
      return BadRequest(new
      {
        error = "Fonte de dados não configurada ou inativa."
      });
    }

    [Authorize]
    [HttpPatch()]
    [Route("{cd_contrato}")]
    public async Task<IActionResult> Patch(int cd_contrato)
    {
      var schemaName = "T_Contrato";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);
      if (source != null && source.Active != null && source.Active == true)
      {
        var filtrosContrato = new List<(string campo, object valor)> { new("cd_contrato", cd_contrato) };
        var contratoExists = await SQLServerService.GetFirstByFields(source, "T_CONTRATO", filtrosContrato);
        if (contratoExists == null) return NotFound("contrato não encontrado");
        var value = "0";

        //cancelar
        if (contratoExists["id_status_contrato"].ToString() == "0")
        {
          //Turma e/ou títulos pagos e/ou venda de material gerada.
          value = "1";
        }
        else
        {
          // se o aluno matriculado não estiver matriculado no produto que está tentando descancelar.
          var filtrosContratoDescancelar = new List<(string campo, object valor)> { new("cd_aluno", contratoExists["cd_aluno"]), new("id_status_contrato", "1"), new("cd_curso_atual", contratoExists["cd_curso_atual"]) };
          var contratoExistsDescancelarExists = await SQLServerService.GetFirstByFields(source, "T_CONTRATO", filtrosContrato);
          if (contratoExistsDescancelarExists != null) return BadRequest("o aluno matriculado não estiver matriculado no produto que está tentando descancelar");
          //desCancelar
        }
        var contratoDict = new Dictionary<string, object>
                {
                    { "id_status_contrato", value }
                };

        var t_contrato = await SQLServerService.Update("T_CONTRATO", contratoDict, source, "cd_contrato", cd_contrato);
        if (!t_contrato.success) return BadRequest(t_contrato.error);
        return ResponseDefault();
      }
      return BadRequest(new
      {
        error = "Fonte de dados não configurada ou inativa."
      });
    }

    /// <summary>
    ///  Cancela o aditamento
    /// </summary>
    /// <param name="cd_aditamento"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPatch()]
    [Route("aditamento/{cd_aditamento}")]
    public async Task<IActionResult> PatchAditamento(int cd_aditamento)
    {
      var accessToken = Request.Headers[HeaderNames.Authorization];
      var tokenInfo = Util.GetUserInfoFromToken(accessToken);

      var schemaName = "T_Pessoa";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);
      if (source != null && source.Active != null && source.Active == true)
      {
        //validação de token
        var cd_pessoa_logada = "";
        var cd_usuario = "1";
        var userId = "";
        if (tokenInfo.Count > 0)
        {
          cd_pessoa_logada = tokenInfo["cd_pessoa"];
          userId = tokenInfo["userid"]; // ID do MongoDB para verificação de admin
        }

        var aditamentoExists = await SQLServerService.GetFirstByFields(source, "T_ADITAMENTO", new List<(string campo, object valor)> { new("cd_aditamento", cd_aditamento) });
        if (aditamentoExists == null) return NotFound("aditamento não encontrado");
        int statusAditamento;

        if (aditamentoExists["id_status_renegociacao"] == null)
        {
          statusAditamento = 1;
        }
        else
        {
          int valor = Convert.ToInt32(aditamentoExists["id_status_renegociacao"]);
          statusAditamento = valor == 4 ? 1 : 4;
        }
        var aditamentoDict = new Dictionary<string, object>
                {
                    { "id_status_renegociacao", statusAditamento }
                };
        var t_aditamento = await SQLServerService.Update("T_ADITAMENTO", aditamentoDict, source, "cd_aditamento", cd_aditamento);
        if (!t_aditamento.success) return BadRequest(t_aditamento.error);
        await AddHistoricoAditamento(cd_aditamento, int.Parse(cd_usuario), statusAditamento, source);
        return ResponseDefault();
      }
      return BadRequest(new
      {
        error = "Fonte de dados não configurada ou inativa."
      });
    }


    private async Task<(bool valido, string msg, List<int>? cd_itens)> ValidaVendaMaterial(VendaMaterial model, Source source, int cd_empresa, int cd_modalidade)
    {
      string connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};MultipleActiveResultSets=True;";
      string msg = null;

      using (var connection = new SqlConnection(connectionString))
      {
        await connection.OpenAsync();

        // Buscar dados dos itens do curso
        var selectCmd = new SqlCommand(@"
                          SELECT i.cd_item,i.no_item, ie.qt_estoque, ic.id_ppt
                          FROM T_ITEM_CURSO ic
                          inner join T_ITEM i on i.cd_item = ic.cd_item
                          inner join T_ITEM_ESCOLA ie on ie.cd_item = ic.cd_item
                          where cd_curso = @cd_curso and cd_pessoa_escola = @cd_escola", connection);

        selectCmd.Parameters.AddWithValue("@cd_escola", cd_empresa);
        selectCmd.Parameters.AddWithValue("@cd_curso", model.cd_curso);
        var itens = new List<(int cd_item, string no_item, int qt_estoque, int id_ppt)>();

        using (var reader = await selectCmd.ExecuteReaderAsync())
        {
          while (await reader.ReadAsync())
          {
            itens.Add((
                cd_item: Convert.ToInt32(reader["cd_item"]),
                no_item: reader["no_item"].ToString(),
                qt_estoque: Convert.ToInt32(reader["qt_estoque"]),
                id_ppt: Convert.ToInt32(reader["id_ppt"])
            ));
          }
          await reader.CloseAsync();
        }
        var itens_validos = new List<(int cd_item, string no_item, int qt_estoque, int id_ppt)>();
        foreach (var item in itens)
        {

          if (model.venda)
          {
            if (item.id_ppt == 1) // Apostila
            {
              if (cd_modalidade != 2) // modalidade personalizada
              {
                continue;
              }

              if (item.qt_estoque <= 0)
              {
                continue;
              }
            }
            else // Livro
            {
              if (item.qt_estoque <= 0) continue;
            }

          }
          itens_validos.Add(item);
        }
        if (cd_modalidade == 2)
        {
          if (!itens_validos.Any(x => x.id_ppt == 1)) return (false, "apostila não encontrada no estoque", null);
          if (!itens_validos.Any(x => x.id_ppt == 0)) return (false, "material não encontrada no estoque", null);
        }
        if (!itens_validos.Any()) return (false, "materiais não encontrada no estoque", null);

        return (true, null, itens_validos.Select(x => x.cd_item).ToList());
      }
    }

    [Authorize]
    [HttpGet]
    [Route("gerar-contrato")]
    public async Task<IActionResult> GerarContrato(int cd_contrato, int cd_id_escola)
    {
      try
      {
        var (arquivo, nomeContrato) = await _matriculaService.GerarContratoMatricula(cd_contrato, cd_id_escola);

        return File(arquivo, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"{nomeContrato}");
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        
        // Verificar se é um erro de "layout não definido" e retornar NotFound
        if (ex.Message.Contains("Contrato não possui layout definido"))
        {
          return NotFound(new { error = ex.Message });
        }
        
        return BadRequest(new
        {
          error = ex.Message,
          stackTrace = ex.StackTrace,
          innerException = ex.InnerException?.Message,
          type = ex.GetType().Name
        });
      }
    }


    private void AddIfNotExists(Dictionary<string, object> dict, string key, object value)
    {
      if (!dict.ContainsKey(key))
      {
        dict.Add(key, value);
      }
    }


    private async Task<(bool success, string? error)> AddHistoricoAditamento(int cd_aditamento, int cd_usuario, int id_status_renegociacao, Source source)

    {
      var dc_historico_aditamento = id_status_renegociacao switch
      {
        0 => "Cadastro de renegociação efetuada.",
        1 => "O contrato está formalizado, porém foi dado início aos pagamentos de títulos.",
        2 => "A renegociação foi concluída com todos os pagamentos realizados.",
        3 => "O acordo foi firmado, mas houve atraso ou falta de pagamento.",
        4 => "A renegociação perdeu validade por descumprimento, desistência ou acordo entre as partes.",
        _ => "Status desconhecido."
      };

      var dict = new Dictionary<string, object>
                {
                    { "dt_aditamento_historico", DateTime.Now },
                    { "id_status_renegociacao", id_status_renegociacao },
                    { "dc_historico_aditamento", dc_historico_aditamento },
                    { "cd_usuario", cd_usuario },
                    { "cd_aditamento", cd_aditamento }
                };

      var t_aditamento_historico_Result = await SQLServerService.Insert("T_ADITAMENTO_HISTORICO", dict, source);
      if (!t_aditamento_historico_Result.success) return (false, t_aditamento_historico_Result.error);
      return (true, null);
    }

    private async Task<(bool success, string? error)> BaixaAutomaticaBolsaAluno(int cd_contrato, Source source)
    {
      //buscar usuario sistema
      var accessToken = Request.Headers[HeaderNames.Authorization];
      var tokenInfo = Util.GetUserInfoFromToken(accessToken);


      var cd_pessoa_logada = "";
      var userId = "";
      var cd_usuario = "1";
      if (tokenInfo.Count > 0)
      {
        cd_pessoa_logada = tokenInfo["cd_pessoa"];
        userId = tokenInfo["userid"]; // ID do MongoDB para verificação de admin
      }

      if (string.IsNullOrEmpty(cd_pessoa_logada)) return (false, "usuario de sistema não configurado");

      var filtrosUsuario = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa_logada) };
      var sys_usuario = await SQLServerService.GetFirstByFields(source, "T_SYS_USUARIO", filtrosUsuario);
      if (sys_usuario != null) cd_usuario = sys_usuario["cd_usuario"].ToString() ?? "1";

      //validar se aluno possui bolsa de material ou mensalidade
      var filtrosContrato = new List<(string campo, object valor)> { new("cd_contrato", cd_contrato) };
      var contratoExists = await SQLServerService.GetFirstByFields(source, "T_CONTRATO", filtrosContrato);
      if (contratoExists == null) return (false, "contrato não encontrado");
      var cd_aluno = contratoExists["cd_aluno"];
      var cd_produto = contratoExists["cd_produto_atual"];
      if (cd_produto == null) return (false, "contrato sem produto vinculado");
      if (cd_aluno == null) return (false, "contrato sem aluno vinculado");
      //pegar bolsas do aluno
      var bolsas = await SQLServerService.GetList("vi_aluno_bolsa", null, "[cd_aluno]", $"[{cd_aluno}]", source, SearchModeEnum.Equals);
      if (!bolsas.data.Any()) return (true, "nenhuma bolsa configurada");
      var bolsa_aluno = bolsas.data.FirstOrDefault(x => x["cd_produto"] != null && x["cd_produto"].ToString() == cd_produto.ToString());
      if (bolsa_aluno == null) bolsa_aluno = bolsas.data.FirstOrDefault();
      if (bolsa_aluno == null) return (false, "bolsa não configuradas");
      if (bolsa_aluno["pc_bolsa"] == null && bolsa_aluno["pc_bolsa_material"] == null) return (false, "porcentagem de bolsa não configuradas");
      var pc_bolsa = bolsa_aluno["pc_bolsa"] != null ? Convert.ToDecimal(bolsa_aluno["pc_bolsa"]) : 0;
      var pc_bolsa_material = bolsa_aluno["pc_bolsa_material"] != null ? Convert.ToDecimal(bolsa_aluno["pc_bolsa_material"]) : 0;
      //aplicar mesma logica de baixa automatica do conta receber
      //pegar titulos do contrato de mensalidade ou material
      var titulos = new List<Dictionary<string, object>>();
      decimal vl_total_baixa = 0;
      decimal vl_desconto_bolsa = 0;
      decimal vl_desconto_bolsa_material = 0;
      var cd_pessoa_empresa_bolsa = contratoExists["cd_pessoa_escola"];

      if (pc_bolsa != 0)
      {
        var get_titulos_mensalidade = await SQLServerService.GetList("T_TITULO", null, "[cd_origem_titulo],[id_origem_titulo],[dc_tipo_titulo],[cd_pessoa_empresa]", $"[{cd_contrato}],[22],[ME],[{cd_pessoa_empresa_bolsa}]", source, SearchModeEnum.Equals);
        titulos.AddRange(get_titulos_mensalidade.data);

        var nm_parcelas_mensalidade = contratoExists["nm_parcelas_mensalidade"] != null ? Convert.ToInt32(contratoExists["nm_parcelas_mensalidade"]) : 0;
        var vl_parcela_mensalidade = contratoExists["vl_parcela_contrato"] != null ? Convert.ToDecimal(contratoExists["vl_parcela_contrato"]) : 0;

        var vl_desconto = (vl_parcela_mensalidade * pc_bolsa) / 100;
        vl_total_baixa += vl_desconto * nm_parcelas_mensalidade;
        vl_desconto_bolsa = vl_desconto;

      }
      if (pc_bolsa_material != 0)
      {
        var get_titulos_mensalidade = await SQLServerService.GetList("T_TITULO", null, "[cd_origem_titulo],[id_origem_titulo],[dc_tipo_titulo],[cd_pessoa_empresa]", $"[{cd_contrato}],[22],[MT],[{cd_pessoa_empresa_bolsa}]", source, SearchModeEnum.Equals);
        titulos.AddRange(get_titulos_mensalidade.data);

        var nm_parcelas_material = contratoExists["nm_parcelas_material"] != null ? Convert.ToInt32(contratoExists["nm_parcelas_material"]) : 0;
        var vl_parcela_material = contratoExists["vl_parcela_material"] != null ? Convert.ToDecimal(contratoExists["vl_parcela_material"]) : 0;

        var vl_desconto_material = (vl_parcela_material * pc_bolsa_material) / 100;
        vl_total_baixa += vl_desconto_material * nm_parcelas_material;
        vl_desconto_bolsa_material = vl_desconto_material;
      }
      if (!titulos.Any()) return (true, "nenhum titulo encontrado");
      var cd_pessoa_empresa = contratoExists["cd_pessoa_escola"];
      //pegar local de movimento padrao da empresa
      var filtroParametro = new List<(string campo, object valor)> { new("cd_pessoa_escola", cd_pessoa_empresa) };
      var parametroExists = await SQLServerService.GetFirstByFields(source, "T_PARAMETRO", filtroParametro);
      var cd_tipo_liquidacao = 100; //motivo bolsa
      var tranFinDict = new Dictionary<string, object>
            {
                { "cd_pessoa_empresa", cd_pessoa_empresa },
                { "cd_local_movto", parametroExists["cd_local_movto"] },
                { "dt_tran_finan", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") },
                { "cd_tipo_liquidacao", cd_tipo_liquidacao },
                { "vl_total_baixa", vl_total_baixa}
            };
      var t_tranFin_insert = await SQLServerService.InsertWithResult("T_TRAN_FINAN", tranFinDict, source);
      if (!t_tranFin_insert.success) return (false, "erro ao gerar T_TRAN_FINAN: " + t_tranFin_insert.error);
      var cd_tran_fin = t_tranFin_insert.inserted["cd_tran_finan"];
      var nm_recibo = int.Parse(parametroExists["nm_ultimo_recibo"].ToString());
      foreach (var t in titulos)
      {
        // Nesse caso, nao deve aplicar o parametro pois deve permitir baixar em todos os titulos
        // Validar se existem títulos anteriores em aberto
        //var validacaoTituloAnterior = await ValidacaoTituloAnteriorService.ValidarTituloAnteriorAberto(
        //    t,
        //    Convert.ToInt32(cd_pessoa_empresa),
        //    source,
        //    userId,
        //    _userService,
        //    _groupService);

        //if (!validacaoTituloAnterior.sucesso)
        //{
        //  return (false, $"Título {t["cd_titulo"]}: {validacaoTituloAnterior.mensagemErro}");
        //}

        var vl_liquidacao = t["dc_tipo_titulo"].ToString() == "ME" ? vl_desconto_bolsa : vl_desconto_bolsa_material;
        nm_recibo++;
        var titulo_baixa_dic = new Dictionary<string, object>
                        {
                            { "cd_titulo", t["cd_titulo"] },
                            { "cd_tran_finan", cd_tran_fin },
                            { "cd_tipo_liquidacao", cd_tipo_liquidacao },
                            { "cd_local_movto", parametroExists["cd_local_movto"] },
                            { "dt_baixa_titulo", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") },
                            { "id_baixa_processada", 0 },
                            { "id_baixa_parcial", 1 },
                            { "nm_dias_float", 0 },
                            { "vl_liquidacao_baixa", vl_liquidacao },
                            { "vl_juros_baixa", 0 },
                            { "vl_desconto_baixa", 0 },
                            { "vl_principal_baixa", 0 },
                            { "vl_juros_calculado", 0 },
                            { "vl_multa_calculada", 0 },
                            { "vl_desc_multa_baixa", 0 },
                            { "vl_desc_juros_baixa", 0 },
                            { "vl_multa_baixa", 0 },
                            { "pc_pontualidade", 0 },
                            { "tx_obs_baixa", "" },
                            { "vl_desconto_baixa_calculado", 0 },
                            { "vl_baixa_saldo_titulo", vl_liquidacao },
                            { "cd_usuario", cd_usuario},
                            { "vl_taxa_cartao", 0 },
                            { "vl_acr_liquidacao", 0 },
                            { "vl_liquidacao_calculado", 0 },
                            { "nm_recibo", nm_recibo }
                        };
        var t_titulo_baixa = await SQLServerService.Insert("T_BAIXA_TITULO", titulo_baixa_dic, source);
        if (!t_titulo_baixa.success) return (false, "erro ao gerar T_BAIXA_TITULO: " + t_titulo_baixa.error);
        var titulo_baixa_CadastradaGet = await SQLServerService.GetList("T_BAIXA_TITULO", 1, 1, "cd_baixa_titulo", true, null, null, "", source, SearchModeEnum.Equals, null, null);
        var titulo_baixa_Cadastrada = titulo_baixa_CadastradaGet.data.First();
        int cd_baixa_titulo = (int)titulo_baixa_Cadastrada["cd_baixa_titulo"];

        //atualizar vl_saldo titulos
        //var update_titulo_dict = new Dictionary<string, object>
        //        {
        //            { "vl_saldo_titulo", Convert.ToDecimal(t["vl_saldo_titulo"]) - vl_liquidacao },

        //        };

        //var atualiza_titulo = await SQLServerService.Update("T_TITULO", update_titulo_dict,source, "cd_titulo", t["cd_titulo"]);
        //if(!atualiza_titulo.success) return (false, "erro ao atualizar T_TITULO: " + atualiza_titulo.error);

        var atualizaDependentes = await AtualizarDependentesBaixa(cd_baixa_titulo, source);
      }

      return (true, null);

    }

    private async Task<(bool success, string error)> AtualizarDependentesBaixa(int cd_baixa_titulo, Source source)
    {
      string connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};MultipleActiveResultSets=True;";
      string msg = null;

      try
      {
        int cd_tipo_liquidacao = 0, cd_plano_conta = 0, cd_titulo = 0;
        DateTime? dt_baixa_titulo = null;
        decimal vl_juros = 0, vl_multa = 0;

        // Buscar dados principais
        using (var connection = new SqlConnection(connectionString))
        {
          await connection.OpenAsync();

          // Buscar dados da baixa
          var selectCmd = new SqlCommand(@"
                    SELECT
                        b.cd_tipo_liquidacao,
                        ISNULL(p.cd_plano_conta_taxbco, 0) as cd_plano_conta,
                        b.cd_titulo,
                        b.dt_baixa_titulo,
                        b.vl_juros_calculado,
                        b.vl_multa_calculada
                    FROM T_BAIXA_TITULO b
                    INNER JOIN T_TITULO t ON b.cd_titulo = t.cd_titulo
                    INNER JOIN T_PARAMETRO p ON p.cd_pessoa_escola = t.cd_pessoa_empresa
                    WHERE b.cd_baixa_titulo = @cd_baixa_titulo", connection);

          selectCmd.Parameters.AddWithValue("@cd_baixa_titulo", Math.Abs(cd_baixa_titulo));
          using (var reader = await selectCmd.ExecuteReaderAsync())
          {
            if (await reader.ReadAsync())
            {
              cd_tipo_liquidacao = Convert.ToInt32(reader["cd_tipo_liquidacao"]);
              cd_plano_conta = Convert.ToInt32(reader["cd_plano_conta"]);
              cd_titulo = Convert.ToInt32(reader["cd_titulo"]);
              dt_baixa_titulo = reader["dt_baixa_titulo"] as DateTime?;
              vl_juros = Convert.ToDecimal(reader["vl_juros_calculado"]);
              vl_multa = Convert.ToDecimal(reader["vl_multa_calculada"]);

              await reader.CloseAsync();
            }
            else
            {
              return (false, "Baixa não encontrada.");
            }
          }

          // Excluir T_CONTA_CORRENTE relacionado
          var deleteContaCorrente = new SqlCommand("DELETE FROM T_CONTA_CORRENTE WHERE cd_baixa_titulo = @cd_baixa_titulo", connection);
          deleteContaCorrente.Parameters.AddWithValue("@cd_baixa_titulo", Math.Abs(cd_baixa_titulo));
          await deleteContaCorrente.ExecuteNonQueryAsync();

          if (cd_baixa_titulo > 0)
          {
            // Atualizar T_TITULO com os cálculos

            //TODO: AQUI REALMENTE DEVERIA SER O vl_baixa_saldo_titulo?
            //t.vl_saldo_titulo = t.vl_titulo - ISNULL((SELECT SUM(vl_baixa_saldo_titulo) FROM T_BAIXA_TITULO b WHERE b.cd_titulo = t.cd_titulo),0),
            var updateTitulo = new SqlCommand(@"
                        UPDATE t SET
                            t.dt_liquidacao_titulo = @dt_baixa_titulo,
                            t.vl_saldo_titulo = t.vl_titulo - ISNULL((SELECT SUM(vl_baixa_saldo_titulo) FROM T_BAIXA_TITULO b WHERE b.cd_titulo = t.cd_titulo),0),
                            t.vl_juros_titulo = t.vl_juros_titulo + (@vl_juros + t.vl_juros_liquidado - t.vl_juros_titulo),
                            t.vl_multa_titulo = t.vl_multa_titulo + (@vl_multa + t.vl_multa_liquidada - t.vl_multa_titulo),
                            t.vl_desconto_titulo = ISNULL((SELECT SUM(vl_desconto_baixa) FROM T_BAIXA_TITULO b WHERE b.cd_titulo = t.cd_titulo),0),
                            t.vl_juros_liquidado = ISNULL((SELECT SUM(vl_juros_baixa) FROM T_BAIXA_TITULO b WHERE b.cd_titulo = t.cd_titulo),0),
                            t.vl_multa_liquidada = ISNULL((SELECT SUM(vl_multa_baixa) FROM T_BAIXA_TITULO b WHERE b.cd_titulo = t.cd_titulo),0),
                            t.vl_desconto_multa = ISNULL((SELECT SUM(vl_desc_multa_baixa) FROM T_BAIXA_TITULO b WHERE b.cd_titulo = t.cd_titulo),0),
                            t.vl_desconto_juros = ISNULL((SELECT SUM(vl_desc_juros_baixa) FROM T_BAIXA_TITULO b WHERE b.cd_titulo = t.cd_titulo),0),
                            t.vl_liquidacao_titulo = ISNULL((SELECT SUM(vl_baixa_saldo_titulo) FROM T_BAIXA_TITULO b WHERE b.cd_titulo = t.cd_titulo),0)
                        FROM T_TITULO t
                        WHERE t.cd_titulo = @cd_titulo", connection);

            updateTitulo.Parameters.AddWithValue("@dt_baixa_titulo", (object)dt_baixa_titulo ?? DBNull.Value);
            updateTitulo.Parameters.AddWithValue("@vl_juros", vl_juros);
            updateTitulo.Parameters.AddWithValue("@vl_multa", vl_multa);
            updateTitulo.Parameters.AddWithValue("@cd_titulo", cd_titulo);
            await updateTitulo.ExecuteNonQueryAsync();

            // Atualizar status do título baseado no saldo remanescente
            var updateStatus = new SqlCommand(@"
                            UPDATE t SET
                                t.id_status_titulo = CASE
                                    WHEN t.vl_saldo_titulo <= 0 THEN 2
                                    ELSE 1
                                END
                            FROM T_TITULO t
                            WHERE t.cd_titulo = @cd_titulo", connection);
            updateStatus.Parameters.AddWithValue("@cd_titulo", cd_titulo);
            await updateStatus.ExecuteNonQueryAsync();

            // Gerar T_CONTA_CORRENTE se necessário
            if (!new[] { 6, 101, 110 }.Contains(cd_tipo_liquidacao))
            {
              // Buscar dados necessários para o insert
              var selectDados = new SqlCommand(@"
                                SELECT
                                    tf.cd_local_movto,
                                    tf.dt_tran_finan,
                                    tf.cd_pessoa_empresa,
                                    tf.cd_tipo_liquidacao,
                                    b.cd_baixa_titulo,
                                    t.cd_titulo,
                                    t.nm_titulo,
                                    t.nm_parcela_titulo,
                                    t.dt_vcto_titulo,
                                    r.no_pessoa,
                                    pt.cd_plano_conta,
                                    pt.vl_plano_titulo,
                                    t.vl_titulo,
                                    b.vl_liquidacao_baixa,
                                    b.nm_recibo
                                FROM T_BAIXA_TITULO b
                                INNER JOIN T_TRAN_FINAN tf ON b.cd_tran_finan = tf.cd_tran_finan
                                INNER JOIN T_TITULO t ON b.cd_titulo = t.cd_titulo
                                INNER JOIN T_PLANO_TITULO pt ON t.cd_titulo = pt.cd_titulo
                                INNER JOIN T_PESSOA r ON t.cd_pessoa_responsavel = r.cd_pessoa
                                WHERE b.cd_baixa_titulo = @cd_baixa_titulo", connection);

              selectDados.Parameters.AddWithValue("@cd_baixa_titulo", cd_baixa_titulo);

              using (var reader = await selectDados.ExecuteReaderAsync())
              {
                if (await reader.ReadAsync())
                {
                  // Calcule o valor proporcional
                  decimal vl_liquidacao_baixa = Convert.ToDecimal(reader["vl_liquidacao_baixa"]);
                  decimal vl_plano_titulo = Convert.ToDecimal(reader["vl_plano_titulo"]);
                  decimal vl_titulo = Convert.ToDecimal(reader["vl_titulo"]);
                  decimal valorContaCorrente = Math.Round(vl_liquidacao_baixa * vl_plano_titulo / vl_titulo, 2);

                  // Montar a descrição
                  string descricao = $"Recebimento do titulo Nº: {reader["nm_titulo"]}-{reader["nm_parcela_titulo"]}. Recibo Nº{reader["nm_recibo"]}, vcto.:{Convert.ToDateTime(reader["dt_vcto_titulo"]).ToString("dd/MM/yyyy")} - {reader["no_pessoa"]}.";

                  var cd_local_movto = reader["cd_local_movto"];
                  var cd_baixa_titulo_new = reader["cd_baixa_titulo"];
                  var dt_tran_finan = reader["dt_tran_finan"];
                  var cd_pessoa_empresa = reader["cd_pessoa_empresa"];
                  var cd_plano_conta_new = reader["cd_plano_conta"];
                  var cd_tipo_liquidacao_new = reader["cd_tipo_liquidacao"];

                  // Fechar o reader antes do insert
                  await reader.CloseAsync();

                  // Insert na T_CONTA_CORRENTE
                  var insertContaCorrente = new SqlCommand(@"
                                    INSERT INTO T_CONTA_CORRENTE
                                    (cd_local_origem, cd_movimentacao_financeira, cd_baixa_titulo, dta_conta_corrente, id_tipo_movimento,
                                     cd_pessoa_empresa, cd_plano_conta, vl_conta_corrente, cd_tipo_liquidacao, dc_obs_conta_corrente)
                                    VALUES
                                    (@cd_local_origem, @cd_movimentacao_financeira, @cd_baixa_titulo, @dta_conta_corrente, @id_tipo_movimento,
                                     @cd_pessoa_empresa, @cd_plano_conta, @vl_conta_corrente, @cd_tipo_liquidacao, @dc_obs_conta_corrente)", connection);

                  insertContaCorrente.Parameters.AddWithValue("@cd_local_origem", cd_local_movto);
                  insertContaCorrente.Parameters.AddWithValue("@cd_movimentacao_financeira", 2);
                  insertContaCorrente.Parameters.AddWithValue("@cd_baixa_titulo", cd_baixa_titulo_new);
                  insertContaCorrente.Parameters.AddWithValue("@dta_conta_corrente", dt_tran_finan);
                  insertContaCorrente.Parameters.AddWithValue("@id_tipo_movimento", 1);
                  insertContaCorrente.Parameters.AddWithValue("@cd_pessoa_empresa", cd_pessoa_empresa);
                  insertContaCorrente.Parameters.AddWithValue("@cd_plano_conta", cd_plano_conta_new);
                  insertContaCorrente.Parameters.AddWithValue("@vl_conta_corrente", valorContaCorrente);
                  insertContaCorrente.Parameters.AddWithValue("@cd_tipo_liquidacao", cd_tipo_liquidacao_new);
                  insertContaCorrente.Parameters.AddWithValue("@dc_obs_conta_corrente", descricao);

                  try
                  {
                    int linhasAfetadas = await insertContaCorrente.ExecuteNonQueryAsync();
                    Console.WriteLine($"Insert T_CONTA_CORRENTE: {linhasAfetadas} linha(s) afetada(s)");
                  }
                  catch (Exception ex)
                  {
                    Console.WriteLine($"Erro no insert: {ex}");
                    msg = ex.ToString();
                    return (false, msg);
                  }
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        msg = ex.Message;
        return (false, msg);
      }

      return (true, null);
    }

    /// <summary>
    /// Deleta baixas parciais de bolsa de um contrato (cd_tipo_liquidacao = 100 MOTIVO_BOLSA)
    /// Baseado no método deletarBaixasBolsaTituloContrato do LEGADO
    /// </summary>
    private async Task<(bool success, string error)> DeletarBaixasBolsaTituloContrato(object cd_contrato, object cd_escola, Source source)
    {
      try
      {
        Console.WriteLine($"[INFO] Deletando baixas de bolsa do contrato {cd_contrato}...");

        // Buscar títulos do contrato
        var titulosResult = await SQLServerService.GetList("T_TITULO", null, "[cd_origem_titulo],[cd_pessoa_empresa]", $"[{cd_contrato}],[{cd_escola}]", source, SearchModeEnum.Equals);

        if (!titulosResult.success || titulosResult.data == null || !titulosResult.data.Any())
        {
          Console.WriteLine($"[INFO] Nenhum título encontrado para o contrato {cd_contrato}");
          return (true, null);
        }

        int deletedCount = 0;
        foreach (var titulo in titulosResult.data)
        {
          var cd_titulo = titulo["cd_titulo"];

          // Buscar baixas parciais de MOTIVO_BOLSA (cd_tipo_liquidacao = 100)
          var baixasResult = await SQLServerService.GetList(
            "T_BAIXA_TITULO",
            null,
            "[cd_titulo],[cd_tipo_liquidacao],[id_baixa_parcial]",
            $"[{cd_titulo}],[100],[1]",
            source,
            SearchModeEnum.Equals
          );

          if (baixasResult.success && baixasResult.data != null && baixasResult.data.Any())
          {
            foreach (var baixa in baixasResult.data)
            {
              var cd_baixa_titulo = baixa["cd_baixa_titulo"];
              var deleteResult = await SQLServerService.Delete("T_BAIXA_TITULO", "cd_baixa_titulo", cd_baixa_titulo.ToString(), source);

              if (deleteResult.success)
              {
                deletedCount++;
                Console.WriteLine($"[SUCCESS] Baixa {cd_baixa_titulo} do título {cd_titulo} deletada");
              }
              else
              {
                Console.WriteLine($"[ERROR] Falha ao deletar baixa {cd_baixa_titulo}: {deleteResult.error}");
              }
            }
          }
        }

        Console.WriteLine($"[SUCCESS] Total de {deletedCount} baixas de bolsa deletadas");
        return (true, null);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[ERROR] Erro ao deletar baixas de bolsa: {ex.Message}");
        return (false, ex.Message);
      }
    }

    private async Task<Dictionary<string, object>> BuscarParametrosEscola(int cd_empresa, Source source)
    {
      try
      {
        var filtroParametro = new List<(string campo, object valor)> { new("cd_pessoa_escola", cd_empresa) };
        return await SQLServerService.GetFirstByFields(source, "T_PARAMETRO", filtroParametro);
      }
      catch (Exception)
      {
        return null;
      }
    }

    /// <summary>
    /// Concatena vencimentos dos títulos em formato de string separado por vírgulas
    /// Implementação idêntica ao Titulo.concatenarVencimentosTitulo do projeto original
    /// </summary>
    private string ConcatenarVencimentosTitulos(List<Dictionary<string, object>> titulos)
    {
      string retorno = "";
      if (titulos != null && titulos.Any())
      {
        foreach (var titulo in titulos)
        {
          var dtVencimento = Convert.ToDateTime(titulo["dt_vcto_titulo"]).ToString("dd/MM/yyyy");
          retorno += ", " + dtVencimento;
        }
      }
      if (retorno.Length >= 1)
        retorno = retorno.Substring(2, retorno.Length - 2);
      return retorno;
    }

    /// <summary>
    /// Gera descrição completa dos horários formatada
    /// Implementação idêntica ao Horario.getDescricaoCompletaHorarios do projeto original
    /// </summary>
    private string GerarDescricaoCompletaHorarios(List<Dictionary<string, object>> horarios)
    {
      string diasHorarios = "";
      if (horarios != null && horarios.Any())
      {
        var horariosOrdenados = horarios
            .Select(h => new
            {
              DiasSemana = h.ContainsKey("diaSemana") ? h["diaSemana"]?.ToString() ?? "" : "",
              HoraInicio = h.ContainsKey("dt_hora_ini") ? h["dt_hora_ini"]?.ToString() ?? "" : "",
              HoraFim = h.ContainsKey("dt_hora_fim") ? h["dt_hora_fim"]?.ToString() ?? "" : "",
              IdDiaSemana = h.ContainsKey("id_dia_semana") ? Convert.ToInt32(h["id_dia_semana"]) : 0
            })
            .OrderBy(h => h.IdDiaSemana)
            .ThenBy(h => h.HoraInicio)
            .ToList();

        foreach (var horario in horariosOrdenados)
        {
          string diasFormatados = horario.DiasSemana;
          string horarioFormatado = $"{horario.HoraInicio} às {horario.HoraFim}";

          diasHorarios += "; " + diasFormatados + " das " + horarioFormatado;
        }

        if (diasHorarios.Length >= 2)
          diasHorarios = diasHorarios.Substring(2, diasHorarios.Length - 2);
      }
      return diasHorarios;
    }

    /// <summary>
    /// Converte número do dia para nome do dia da semana
    /// Implementação baseada no projeto original
    /// </summary>
    private string GetDiaSemanaPorDia(string dia)
    {
      return dia.Trim() switch
      {
        "1" => "Segunda",
        "2" => "Terça",
        "3" => "Quarta",
        "4" => "Quinta",
        "5" => "Sexta",
        "6" => "Sábado",
        "7" => "Domingo",
        _ => dia
      };
    }

    /// <summary>
    /// Verifica se a escola e o aluno estão no mesmo estado e retorna o CFOP apropriado
    /// </summary>
    private async Task<int> VerificaEstadoEscAluno(int cd_pessoa_escola, int cd_pessoa_aluno, int tipoMovimento, Source source)
    {
      try
      {
        // Buscar estado da escola
        var enderecoEscola = await SQLServerService.GetFirstByFields(source, "T_ENDERECO",
            new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa_escola) });

        // Buscar estado do aluno
        var enderecoAluno = await SQLServerService.GetFirstByFields(source, "T_ENDERECO",
            new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa_aluno) });

        if (enderecoEscola == null || enderecoAluno == null)
        {
          throw new Exception("Estado da escola ou aluno não encontrado");
        }

        int estadoEscola = Convert.ToInt32(enderecoEscola["cd_loc_estado"] ?? 0);
        int estadoAluno = Convert.ToInt32(enderecoAluno["cd_loc_estado"] ?? 0);

        bool? estadosIguais = null;

        if (estadoAluno > 0 && estadoEscola > 0)
        {
          estadosIguais = estadoEscola == estadoAluno;
        }

        if (!estadosIguais.HasValue)
        {
          throw new Exception("Erro ao verificar estados da escola e pessoa");
        }

        // Determinar CFOP baseado no tipo de movimento e se os estados são iguais
        int tipoCfop = 0;

        // CFOPs baseados nos enums do sistema original

        if (tipoMovimento == (int)TipoMovimentoEnum.ENTRADA)
        {
          if (!estadosIguais.Value)
            tipoCfop = (int)CfOPEnum.ENTRADAFORAESTADO;
          else
            tipoCfop = (int)CfOPEnum.ENTRADADENTROESTADO;
        }
        else
        {
          if (!estadosIguais.Value)
            tipoCfop = (int)CfOPEnum.SAIDAFORADOESTADO;
          else
            tipoCfop = (int)CfOPEnum.SAIDADENTROESTADO;
        }

        return tipoCfop;
      }
      catch (Exception ex)
      {
        throw new Exception($"Erro ao verificar estado escola/aluno: {ex.Message}");
      }
    }

    /// <summary>
    /// Valida se o aluno já possui matrícula no mesmo período/produto
    /// Implementação fiel à lógica do sgf1-prod
    /// </summary>
    private async Task ValidarMatriculaDuplicada(MatriculaInputModel model, Source source)
    {
      // Implementa a mesma lógica da rota ValidarMatriculaDuplicada
      if (model.CursoContrato != null && model.CursoContrato.Any())
      {
        foreach (var cursoContrato in model.CursoContrato)
        {
          // Calcular data final se não fornecida
          var dt_final_calculada = model.dt_final_contrato;
          if (dt_final_calculada == null && !string.IsNullOrEmpty(model.cd_duracao_atual))
          {
            if (int.TryParse(model.cd_duracao_atual, out int duracao))
            {
              dt_final_calculada = await CalcularDataFinalContrato(cursoContrato.cd_curso, duracao, model.dt_inicial_contrato, source);
            }
          }

          bool existeConflito = await VerificarMatriculaPorProdutoAluno(
            Convert.ToInt32(model.cd_produto_atual ?? "0"),
            model.cd_aluno,
            Convert.ToInt32(model.cd_pessoa_escola ?? "0"),
            model.dt_inicial_contrato,
            cursoContrato.cd_curso,
            0, // cd_contrato_ignorar = 0 pois ainda não existe
            dt_final_calculada,
            int.TryParse(model.cd_duracao_atual, out int dur) ? dur : 0,
            source
          );

          if (existeConflito)
          {
            throw new Exception("Já existe matrícula para este curso/produto no período informado");
          }
        }
      }
      else
      {
        // Calcular data final se não fornecida
        var dt_final_calculada = model.dt_final_contrato;
        if (dt_final_calculada == null && !string.IsNullOrEmpty(model.cd_duracao_atual))
        {
          if (int.TryParse(model.cd_duracao_atual, out int duracao) && int.TryParse(model.cd_curso_atual, out int curso))
          {
            dt_final_calculada = await CalcularDataFinalContrato(curso, duracao, model.dt_inicial_contrato, source);
          }
        }

        // Validação para o curso atual apenas
        bool existeConflito = await VerificarMatriculaPorProdutoAluno(
          Convert.ToInt32(model.cd_produto_atual ?? "0"),
          model.cd_aluno,
          Convert.ToInt32(model.cd_pessoa_escola ?? "0"),
          model.dt_inicial_contrato,
          Convert.ToInt32(model.cd_curso_atual ?? "0"),
          0, // cd_contrato_ignorar = 0 pois ainda não existe
          dt_final_calculada,
          int.TryParse(model.cd_duracao_atual, out int dur) ? dur : 0,
          source
        );

        if (existeConflito)
        {
          throw new Exception("Já existe matrícula para este curso/produto no período informado");
        }
      }
    }

    /// <summary>
    /// Valida matrícula duplicada para contratos sem turma
    /// </summary>
    private async Task ValidarMatriculaDuplicadaSemTurma(MatriculaInputModel model, Source source)
    {
      // Validar por cada curso do contrato (usando CursoContrato ao invés de Cursos)
      if (model.CursoContrato != null && model.CursoContrato.Any())
      {
        foreach (var curso in model.CursoContrato)
        {
          var isPPT = Convert.ToInt32(model.cd_regime_atual ?? "0") == 2; // PPT = regime 2

          // Verificar matrícula existente por produto (com turma)
          var existeMatriculaComTurma = await VerificarMatriculaPorProduto(
              Convert.ToInt32(model.cd_produto_atual ?? "0"),
              model.cd_aluno,
              model.dt_inicial_contrato,
              curso.cd_curso,
              isPPT,
              0, // cd_contrato ainda não existe
              model.dt_final_contrato,
              curso.cd_curso,
              source
          );

          if (existeMatriculaComTurma)
          {
            throw new Exception("Aluno já está matriculado neste produto no período informado (com turma).");
          }

          // Verificar matrícula existente por produto/aluno (sem turma)
          var existeMatriculaSemTurma = await VerificarMatriculaPorProdutoAluno(
              Convert.ToInt32(model.cd_produto_atual ?? "0"),
              model.cd_aluno,
              Convert.ToInt32(model.cd_pessoa_escola ?? "0"),
              model.dt_inicial_contrato,
              curso.cd_curso,
              0, // cd_contrato ainda não existe
              model.dt_final_contrato,
              curso.cd_curso,
              source
          );

          if (existeMatriculaSemTurma)
          {
            throw new Exception("Aluno já está matriculado neste produto no período informado (sem turma).");
          }
        }
      }

      // Validação adicional para o curso atual principal
      var isPPTAtual = Convert.ToInt32(model.cd_regime_atual ?? "0") == 2;
      var existeMatriculaAtual = await VerificarMatriculaPorProdutoAluno(
          Convert.ToInt32(model.cd_produto_atual ?? "0"),
          model.cd_aluno,
          Convert.ToInt32(model.cd_pessoa_escola ?? "0"),
          model.dt_inicial_contrato,
          Convert.ToInt32(model.cd_curso_atual ?? "0"),
          0, // cd_contrato ainda não existe
          model.dt_final_contrato,
          Convert.ToInt32(model.cd_duracao_atual ?? "0"),
          source
      );

      if (existeMatriculaAtual)
      {
        throw new Exception("Aluno já está matriculado neste produto no período informado.");
      }
    }

    /// <summary>
    /// Valida matrícula duplicada para contratos com turma
    /// </summary>
    private async Task ValidarMatriculaDuplicadaComTurma(MatriculaInputModel model, Source source)
    {
      foreach (var turma in model.Turmas)
      {
        // Buscar dados da turma
        var turmaData = await SQLServerService.GetFirstByFields(source, "T_TURMA",
            new List<(string campo, object valor)> { ("cd_turma", turma.cd_turma) });

        if (turmaData != null)
        {
          var cd_produto_turma = Convert.ToInt32(turmaData["cd_produto"]);
          var cd_curso_turma = Convert.ToInt32(turmaData["cd_curso"]);
          var cd_duracao_turma = Convert.ToInt32(turmaData["cd_duracao"] ?? 0);
          var dt_inicio_turma = Convert.ToDateTime(turmaData["dt_inicio_aula"]);
          var dt_fim_turma = turmaData["dt_final_aula"] != null ?
              Convert.ToDateTime(turmaData["dt_final_aula"]) : model.dt_final_contrato;

          var isPPT = Convert.ToInt32(model.cd_regime_atual ?? "0") == 2;

          // Verificar se já existe matrícula neste produto/período
          var existeMatricula = await VerificarMatriculaPorProduto(
              cd_produto_turma,
              model.cd_aluno,
              dt_inicio_turma,
              cd_curso_turma,
              isPPT,
              0, // cd_contrato ainda não existe
              dt_fim_turma,
              cd_duracao_turma,
              source
          );

          if (existeMatricula)
          {
            throw new Exception($"Aluno já está matriculado no produto da turma {turmaData["no_turma"]} no período informado.");
          }
        }
      }
    }

    /// <summary>
    /// Verifica se existe matrícula por produto (contratos COM turma)
    /// Equivalente ao DataAccessMatricula.existeMatriculaByProduto do sgf1-prod
    /// </summary>
    private async Task<bool> VerificarMatriculaPorProduto(
        int cd_produto, int cd_aluno, DateTime? dt_inicial, int cd_curso,
        bool isPPT, int cd_contrato_excluir, DateTime? dt_final, int cd_duracao, Source source)
    {
      try
      {
        // Buscar contratos COM turma que atendam os critérios
        var contratos = await SQLServerService.GetList("T_CONTRATO", null, null, "cd_contrato", false,
            null, "[cd_aluno]", $"[{cd_aluno}]", source, SearchModeEnum.Equals, null, null);

        if (!contratos.success || contratos.data == null) return false;

        foreach (var contrato in contratos.data)
        {
          var cd_contrato = Convert.ToInt32(contrato["cd_contrato"]);
          if (cd_contrato == cd_contrato_excluir) continue;

          // Verificar se este contrato tem turmas
          var alunoTurmas = await SQLServerService.GetList("T_ALUNO_TURMA", null, null, "cd_aluno_turma", false,
              null, "[cd_contrato]", $"[{cd_contrato}]", source, SearchModeEnum.Equals, null, null);

          if (alunoTurmas.success && alunoTurmas.data != null && alunoTurmas.data.Any())
          {
            foreach (var alunoTurma in alunoTurmas.data)
            {
              var cd_turma = Convert.ToInt32(alunoTurma["cd_turma"]);

              // Buscar dados da turma
              var turmaData = await SQLServerService.GetFirstByFields(source, "T_TURMA",
                  new List<(string campo, object valor)> { ("cd_turma", cd_turma) });

              if (turmaData != null)
              {
                var produto_turma = Convert.ToInt32(turmaData["cd_produto"]);

                // Verificar se é o mesmo produto e curso
                if (produto_turma == cd_produto)
                {
                  // Verificar PPT se necessário
                  if (isPPT && turmaData["cd_turma_ppt"] == null) continue;

                  // Verificar sobreposição de período
                  if (dt_inicial.HasValue)
                  {
                    var dt_inicial_contrato = Convert.ToDateTime(contrato["dt_inicial_contrato"]);
                    var dt_final_contrato = contrato["dt_final_contrato"] != null ?
                        Convert.ToDateTime(contrato["dt_final_contrato"]) : dt_inicial_contrato.AddMonths(12);

                    var dt_final_validacao = dt_final ?? dt_inicial.Value.AddMonths(12);

                    // Verifica sobreposição
                    bool temSobreposicao = (dt_inicial_contrato <= dt_final_validacao) && (dt_final_contrato >= dt_inicial.Value);
                    if (temSobreposicao) return true;
                  }
                }
              }
            }
          }
        }

        return false;
      }
      catch
      {
        return false;
      }
    }

    /// <summary>
    /// Verifica se existe matrícula por produto/aluno (contratos SEM turma)
    /// Equivalente ao DataAccessMatricula.existeMatriculaByProdutoAluno do sgf1-prod
    /// </summary>
    private async Task<bool> VerificarMatriculaPorProdutoAluno(
        int cd_produto, int cd_aluno, int cd_escola, DateTime? dt_inicial, int cd_curso,
        int cd_contrato_ignorar, DateTime? dt_final, int cd_duracao, Source source)
    {
      try
      {
        // Buscar contratos SEM turma que atendam os critérios
        var contratos = await SQLServerService.GetList("T_CONTRATO", null, null, "cd_contrato", false,
            null, "[cd_aluno]", $"[{cd_aluno}]", source, SearchModeEnum.Equals, null, null);

        if (!contratos.success || contratos.data == null) return false;

        foreach (var contrato in contratos.data)
        {
          var cd_contrato = Convert.ToInt32(contrato["cd_contrato"]);
          if (cd_contrato == cd_contrato_ignorar) continue;

          // Verificar se este contrato NÃO tem turmas
          var alunoTurmas = await SQLServerService.GetList("T_ALUNO_TURMA", null, null, "cd_aluno_turma", false,
              null, "[cd_contrato]", $"[{cd_contrato}]", source, SearchModeEnum.Equals, null, null);

          // Se tem turmas, pula este contrato (será validado pela outra função)
          if (alunoTurmas.success && alunoTurmas.data != null && alunoTurmas.data.Any()) continue;

          // Verificar se é o mesmo produto, curso, escola e duração
          var produto_contrato = Convert.ToInt32(contrato["cd_produto_atual"] ?? "0");
          var curso_contrato = Convert.ToInt32(contrato["cd_curso_atual"] ?? "0");
          var escola_contrato = Convert.ToInt32(contrato["cd_pessoa_escola"] ?? "0");

          if (produto_contrato == cd_produto && curso_contrato == cd_curso &&
              escola_contrato == cd_escola)
          {
            // Verificar sobreposição de período
            if (dt_inicial.HasValue)
            {
              var dt_inicial_contrato = Convert.ToDateTime(contrato["dt_inicial_contrato"]);
              var dt_final_contrato = contrato["dt_final_contrato"] != null ?
                  Convert.ToDateTime(contrato["dt_final_contrato"]) : dt_inicial_contrato.AddMonths(12);

              var dt_final_validacao = dt_final ?? dt_inicial.Value.AddMonths(12);

              // Verifica sobreposição
              bool temSobreposicao = (dt_inicial_contrato <= dt_final_validacao) && (dt_final_contrato >= dt_inicial.Value);
              if (temSobreposicao) return true;
            }
          }
        }

        return false;
      }
      catch
      {
        return false;
      }
    }

    /// <summary>
    /// Calcula a data final do contrato baseada na carga horária do curso e duração
    /// Equivalente ao cálculo: dt_inicio_aula.Value.AddDays((carga_horaria/nmDuracao)*7)
    /// </summary>
    private async Task<DateTime> CalcularDataFinalContrato(int cd_curso, int cd_duracao, DateTime dt_inicial, Source source)
    {
      try
      {
        // Buscar carga horária do curso
        var curso = await SQLServerService.GetFirstByFields(source, "T_CURSO",
            new List<(string campo, object valor)> { ("cd_curso", cd_curso) });

        // Buscar duração
        var duracao = await SQLServerService.GetFirstByFields(source, "T_DURACAO",
            new List<(string campo, object valor)> { ("cd_duracao", cd_duracao) });

        if (curso != null && duracao != null)
        {
          var carga_horaria = Convert.ToInt32(curso["nm_carga_horaria"] ?? "0");
          var nm_duracao = Convert.ToDecimal(duracao["nm_duracao"] ?? "0");

          if (nm_duracao > 0)
          {
            // Cálculo: (carga_horaria / nm_duracao) * 7 dias por semana
            int diasTotais = (int)((carga_horaria / nm_duracao) * 7);
            return dt_inicial.AddDays(diasTotais);
          }
        }

        // Fallback: se não conseguir calcular, usar 12 meses (padrão do sistema)
        return dt_inicial.AddMonths(12);
      }
      catch
      {
        // Fallback em caso de erro
        return dt_inicial.AddMonths(12);
      }
    }

    private string FormatarData(object data)
    {
      if (data != null && DateTime.TryParse(data.ToString(), out DateTime dt))
        return dt.ToString("dd/MM/yyyy");
      return "";
    }


  }
}
