using Amazon.Runtime.Internal.Transform;
using DotLiquid.Tags;
using DotLiquid.Util;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
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
using Simjob.Framework.Infra.Schemas.Entities;
using Simjob.Framework.Services.Api.Enums;
using Simjob.Framework.Services.Api.Models.Matricula;
using Simjob.Framework.Services.Api.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Words.NET;

namespace Simjob.Framework.Services.Api.Controllers
{
    public class MatriculaController : BaseController
    {
        private readonly IRepository<SourceContext, Source> _sourceRepository;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MatriculaController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, IRepository<SourceContext, Source> sourceRepository, IRepository<MongoDbContext, Schema> schemaRepository, IWebHostEnvironment webHostEnvironment) : base(bus, notifications)
        {
            _sourceRepository = sourceRepository;
            _schemaRepository = schemaRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize]
        [HttpGet()]
        public async Task<IActionResult> GetAll(string value, SearchModeEnum mode, int? page, int? limit, string sortField, bool sortDesc = false, string ids = "", string searchFields = null, string? cd_empresa = null, bool aditamento = false, DateTime? dataInicio = null, DateTime? dataMatriculaInicio = null, DateTime? dataMatriculaFim = null)
        {
            if (cd_empresa == null) return BadRequest("campo cd_empresa não informado");
            var schemaName = "T_Pessoa";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                int op_aditamento = aditamento ? 1 : 0;
                if (string.IsNullOrEmpty(searchFields))
                {
                    searchFields = "[possui_aditamento]";
                    value = $"[{op_aditamento}]";
                }
                else
                {
                    searchFields = $"{searchFields},[possui_aditamento]";
                    value = $"{value},[{op_aditamento}]";
                }

                //var matriculaResult = await SQLServerService.GetListFiltroData("vi_contrato", page, limit, sortField, sortDesc, ids, searchFields, value, source, mode, "cd_pessoa_escola", cd_empresa);
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

                var gridTaxa_result = await SQLServerService.GetList("T_TAXA_MATRICULA", null, "[cd_contrato]", $"[{cd_contrato}]", source, SearchModeEnum.Equals);
                List<Dictionary<string, object>>? gridTaxa = null;
                if (gridTaxa_result.success)
                {
                    gridTaxa = gridTaxa_result.data;
                }

                var titulos_result = await SQLServerService.GetList("vi_contrato_titulos", null, "[cd_origem]", $"[{cd_contrato}]", source, SearchModeEnum.Equals);
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
                if (aditamentos_result.success)
                {
                    aditamentos = aditamentos_result.data;
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
                    ["item_movimento"] = item_movimento.data
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

                var t_desconto_contrato = await SQLServerService.GetFirstByFields(source, "T_DESCONTO_CONTRATO", new List<(string campo, object valor)> { new("cd_aditamento", cd_aditamento) });
                aditamentoExists.Add("desconto", t_desconto_contrato);

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
                var nm_nf_material = parametroExists["nm_nf_material"] != null ? int.Parse(parametroExists["nm_nf_material"].ToString()) : 0;
                var id_nro_contrato_automatico = (bool)parametroExists["id_nro_contrato_automatico"];
                var id_tipo_numero_contrato = parametroExists["id_tipo_numero_contrato"]?.ToString() ?? "0";

                if (model.nm_matricula_contrato > 0 && id_tipo_numero_contrato == "2") return BadRequest("Numeração das matriculas está programada para ser automática, portanto não  deve ser informada(nm_matricula_contrato)");

                var responsavel = model.cd_pessoa_responsavel;
                if (string.IsNullOrEmpty(responsavel))
                {
                    responsavel = model.cd_aluno.ToString();
                }

                // validação e cadastro de relacionamento

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

                var matricula_dict = new Dictionary<string, object>
                {
                    ["cd_aluno"] = model.cd_aluno,
                    ["cd_usuario"] = 1,
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
                var cd_contrato = matricula["cd_contrato"];
                var cd_escola = model.cd_pessoa_escola;

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
                            ["dc_tipo_titulo"] = titulo.dc_tipo_titulo,
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
                            ["dc_tipo_titulo"] = titulo.dc_tipo_titulo,
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
                                ["dc_tipo_titulo"] = titulo.dc_tipo_titulo,
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
                    foreach (var venda in model.VendasMaterial)
                    {
                        var item_escola = await SQLServerService.GetFirstByFields(source, "T_ITEM_ESCOLA", new List<(string campo, object valor)> { new("cd_item", venda.cd_item), new("cd_pessoa_escola", cd_escola) });
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
                                nm_nf_material++;
                            }
                        }


                        //não gerar venda se não ha estoque para livro ou apostila
                        if (venda.venda && !estoque_ok) continue;
                        var movimento_existente = await SQLServerService.GetFirstByFields(source, "T_MOVIMENTO", new List<(string campo, object valor)> { new("cd_curso", venda.cd_curso), new("cd_aluno", model.cd_aluno) });
                        var cd_movimento = 0;
                        Dictionary<string, object>? movimento = null;
                        if (movimento_existente == null)
                        {
                            //movimento
                            var movimento_dict = new Dictionary<string, object>
                                {
                                    { "cd_curso",venda.cd_curso },
                                    { "cd_aluno",model.cd_aluno },
                                    { "cd_pessoa_empresa", cd_escola},
                                    { "cd_pessoa", cd_pessoa_aluno},
                                    { "cd_politica_comercial", 2},
                                    { "cd_tipo_financeiro",  3 },
                                    { "id_tipo_movimento", 1 },
                                    { "dt_emissao_movimento", DateTime.Now.Date.ToString("yyyy-MM-ddTHH:mm:ss") },
                                    { "dt_vcto_movimento", DateTime.Now.Date.AddDays(60).ToString("yyyy-MM-ddTHH:mm:ss") },
                                    { "dt_mov_movimento", DateTime.Now.Date.ToString("yyyy-MM-ddTHH:mm:ss") },
                                    { "pc_acrescimo",  0 },
                                    { "vl_acrescimo",  0 },
                                    { "pc_desconto",  0 },
                                    { "vl_desconto", 0 },
                                    { "id_nf", 0},
                                    { "id_nf_escola", 0},
                                    { "vl_base_calculo_ICMS_nf", 0 },
                                    { "vl_base_calculo_PIS_nf", 0 },
                                    { "vl_base_calculo_COFINS_nf", 0},
                                    { "vl_base_calculo_IPI_nf", 0},
                                    { "vl_base_calculo_ISS_nf", 0},
                                    { "vl_ICMS_nf", 0 },
                                    { "vl_PIS_nf", 0 },
                                    { "vl_COFINS_nf", 0},
                                    { "vl_IPI_nf", 0 },
                                    { "vl_ISS_nf", 0 },
                                    { "pc_aliquota_aproximada", 0 },
                                    { "vl_aproximado", 0 },
                                    { "id_exportado", 0 },
                                    { "id_importacao_xml", 0 },
                                    { "id_material_didatico", 1 },
                                    { "id_venda_futura", venda.venda?0:1 },
                                    { "id_origem_movimento", 22 },
                                    { "nm_nfe",venda.venda?nm_nf_material:null }
                                };
                            var t_movimento_Result = await SQLServerService.Insert("T_MOVIMENTO", movimento_dict, source);
                            if (!t_movimento_Result.success) return BadRequest(t_movimento_Result.error);
                            var movimento_inseridoGet = await SQLServerService.GetList("T_MOVIMENTO", 1, 1, "cd_movimento", true, null, null, "", source, SearchModeEnum.Equals, null, null);
                            var movimento_inserido = movimento_inseridoGet.data.First();
                            movimento = movimento_inserido;
                            cd_movimento = int.Parse(movimento_inserido["cd_movimento"]?.ToString());
                            //movimento item
                        }
                        else
                        {
                            movimento = movimento_existente;
                            cd_movimento = int.Parse(movimento_existente["cd_movimento"]?.ToString());

                            var movimento_update_dict = new Dictionary<string, object>
                            {
                                { "id_venda_futura", venda.venda?0:1 },
                                { "nm_nfe",venda.venda?nm_nf_material:null }

                            };
                            var t_movimento_Result = await SQLServerService.Update("T_MOVIMENTO", movimento_update_dict, source, "cd_movimento", cd_movimento);
                            if (!t_movimento_Result.success) return BadRequest(t_movimento_Result.error);
                        }


                        var item_movimento_existente = await SQLServerService.GetFirstByFields(source, "T_ITEM_MOVIMENTO", new List<(string campo, object valor)> { new("cd_item", venda.cd_item), new("cd_movimento", cd_movimento) });
                        if (item_movimento_existente == null)
                        {
                            var item_movimento_dict = new Dictionary<string, object>
                                {
                                    { "cd_movimento", cd_movimento },
                                    { "cd_item",venda.cd_item },
                                    { "qt_item_movimento", 1 },
                                    { "vl_unitario_item",  0  },
                                    { "vl_total_item",0 },
                                    { "vl_liquido_item",  0 },
                                    { "vl_acrescimo_item", 0 },
                                    { "vl_desconto_item", movimento.ContainsKey("vl_desconto") ? movimento["vl_desconto"] : 0 },
                                    { "vl_base_calculo_ICMS_item",0 },
                                    { "vl_base_calculo_PIS_item",0 },
                                    { "vl_base_calculo_COFINS_item", 0 },
                                    { "vl_base_calculo_IPI_item",0 },
                                    { "vl_base_calculo_ISS_item", 0 },
                                    { "vl_ICMS_item",0 },
                                    { "vl_PIS_item", 0},
                                    { "vl_COFINS_item", 0 },
                                    { "vl_IPI_item", 0 },
                                    { "vl_ISS_item", 0 },
                                    { "pc_aliquota_ICMS", 0},
                                    { "pc_aliquota_PIS", 0},
                                    { "pc_aliquota_COFINS", 0 },
                                    { "pc_aliquota_IPI", 0 },
                                    { "pc_aliquota_ISS", 0 },
                                    { "pc_aliquota_aproximada", movimento.ContainsKey("pc_aliquota_aproximada") ? movimento["pc_aliquota_aproximada"] : 0 },
                                    { "vl_aproximado", movimento.ContainsKey("vl_aproximado") ? movimento["vl_aproximado"] : 0},
                                    { "pc_desconto_item", movimento.ContainsKey("pc_desconto") ? movimento["pc_desconto"] : 0 }
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

                                //atualiza nm_nf_material
                                var parametroUpdate = new Dictionary<string, object>
                                {
                                    { "nm_nf_material", nm_nf_material }
                                };
                                var parametroResult = await SQLServerService.Update("T_PARAMETRO", parametroUpdate, source, "cd_pessoa_escola", model.cd_pessoa_escola);
                                if (!parametroResult.success) return BadRequest(parametroResult.error);
                            }
                        }



                    }


                }

                // validação turma(Não é possíver gerar matricula para duas turmas ainda não matriculadas )
                //turma
                if (!model.Turmas.IsNullOrEmpty())
                {
                    foreach (var turma in model.Turmas)
                    {
                        var filtroTurma = new List<(string campo, object valor)> { new("cd_turma", turma.cd_turma) };
                        var turmaExists = await SQLServerService.GetFirstByFields(source, "T_TURMA", filtroTurma);
                        if (turmaExists == null) continue;
                        var no_turma = turmaExists["no_turma"];

                        if (no_turma == null) continue;
                        var cd_turma_original = turmaExists["cd_turma"];
                        var original = no_turma?.ToString() ?? string.Empty;

                        var partes = original.Split('/', 2); // corta só na primeira barra
                        bool primeiroEhPERS = partes.Length == 2 &&
                                              string.Equals(partes[0], "PERS", StringComparison.OrdinalIgnoreCase);

                        var situacao_aluno = model.id_tipo_matricula == 1 ? 1 :
                                      model.id_tipo_matricula == 3 ? 10 :
                                      model.id_tipo_matricula == 2 ? 8 : 9;

                        var dt_inicio = model.dt_inicial_contrato > turma.dt_inicio_aula ? model.dt_inicial_contrato : turma.dt_inicio_aula;
                        if (primeiroEhPERS)
                        {
                            //remove campos que não serão inseridos
                            turmaExists.Remove("cd_turma");
                            turmaExists.Remove("no_turma");

                            string novo_nome = primeiroEhPERS
                                ? $"PERSF/{partes[1]}"   // troca PERS -> PERSF e mantém o resto
                                : original;

                            // adiciona nome montado
                            turmaExists.Add("no_turma", novo_nome);

                            var t_turma_insert = await SQLServerService.Insert("T_TURMA", turmaExists, source);
                            if (!t_turma_insert.success) continue;

                            var turmaCadastradaGet = await SQLServerService.GetList("T_TURMA", 1, 1, "cd_turma", true, null, null, "", source, SearchModeEnum.Equals, null, null);
                            var turmaCadastrada = turmaCadastradaGet.data.First();
                            int cdTurmaId = (int)turmaCadastrada["cd_turma"];

                            var horario = await SQLServerService.GetList("T_HORARIO", 1, 10000000, "cd_horario", true, null, "[{cd_turma}]", $"[{cd_turma_original}]", source, SearchModeEnum.Equals, null, null);
                            var turma_escola = await SQLServerService.GetList("T_TURMA_ESCOLA", 1, 10000000, "cd_turma_escola", true, null, "[{cd_turma}]", $"[{cd_turma_original}]", source, SearchModeEnum.Equals, null, null);
                            var turma_professor = await SQLServerService.GetList("T_TURMA_PROFESSOR", 1, 10000000, "cd_turma_professor", true, null, "[{cd_turma}]", $"[{cd_turma_original}]", source, SearchModeEnum.Equals, null, null);
                            var programacao_turma = await SQLServerService.GetList("T_PROGRAMACAO_TURMA", 1, 10000000, "cd_programacao_turma", true, null, "[{cd_turma}]", $"[{cd_turma_original}]", source, SearchModeEnum.Equals, null, null);
                            var horario_professor_turma = await SQLServerService.GetList("T_HORARIO_PROFESSOR_TURMA", 1, 10000000, "cd_horario_professor_turma", true, null, "[{cd_turma}]", $"[{cd_turma_original}]", source, SearchModeEnum.Equals, null, null);

                            var feriado_desconsiderado = await SQLServerService.GetList("T_FERIADO_DESCONSIDERADO", 1, 10000000, "cd_feriado_desconsiderado", true, null, "[{cd_turma}]", $"[{cd_turma_original}]", source, SearchModeEnum.Equals, null, null);

                            //vinculos para nova turma criada
                            foreach (var item in horario.data)
                            {
                                item.Remove("cd_horario");
                                item["cd_turma"] = cdTurmaId;
                                var t_insert = await SQLServerService.Insert("T_HORARIO", item, source);
                                if (!t_insert.success) continue;
                            }
                            foreach (var item in turma_escola.data)
                            {
                                item.Remove("cd_turma_escola");
                                item["cd_turma"] = cdTurmaId;
                                var t_insert = await SQLServerService.Insert("T_TURMA_ESCOLA", item, source);
                                if (!t_insert.success) continue;
                            }
                            foreach (var item in turma_professor.data)
                            {
                                item.Remove("cd_turma_professor");
                                item["cd_turma"] = cdTurmaId;
                                var t_insert = await SQLServerService.Insert("T_TURMA_PROFESSOR", item, source);
                                if (!t_insert.success) continue;
                            }
                            foreach (var item in turma_professor.data)
                            {
                                item.Remove("cd_turma_professor");
                                item["cd_turma"] = cdTurmaId;
                                var t_insert = await SQLServerService.Insert("T_TURMA_PROFESSOR", item, source);
                                if (!t_insert.success) continue;
                            }
                            foreach (var item in programacao_turma.data)
                            {
                                item.Remove("cd_programacao_turma");
                                item["cd_turma"] = cdTurmaId;
                                var t_insert = await SQLServerService.Insert("T_PROGRAMACAO_TURMA", item, source);
                                if (!t_insert.success) continue;
                            }
                            foreach (var item in horario_professor_turma.data)
                            {
                                item.Remove("cd_horario_professor_turma");
                                item["cd_turma"] = cdTurmaId;
                                var t_insert = await SQLServerService.Insert("T_HORARIO_PROFESSOR_TURMA", item, source);
                                if (!t_insert.success) continue;
                            }
                            foreach (var item in feriado_desconsiderado.data)
                            {
                                item.Remove("cd_feriado_desconsiderado");
                                item["cd_turma"] = cdTurmaId;
                                var t_insert = await SQLServerService.Insert("T_FERIADO_DESCONSIDERADO", item, source);
                                if (!t_insert.success) continue;
                            }
                            foreach (var cursoContratoId in cursosContrato)
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
                        }
                        else
                        {
                            //validação aluno existente
                            var filtrosAluno = new List<(string campo, object valor)> { new("cd_aluno", model.cd_aluno), new("cd_situacao_aluno_turma", 9) };
                            var alunoExists = await SQLServerService.GetFirstByFields(source, "T_ALUNO_TURMA", filtrosAluno);

                            if (alunoExists != null)
                            {
                                foreach (var cursoContratoId in cursosContrato)
                                {
                                    //atualiza cd_contrato e situação aluno
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
                var nm_nf_material = parametroExists["nm_nf_material"] != null ? int.Parse(parametroExists["nm_nf_material"].ToString()) : 0;
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
                            ["id_aditamento"] = desconto.id_aditamento
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

                var responsavel = model.cd_pessoa_responsavel;
                if (string.IsNullOrEmpty(responsavel))
                {
                    responsavel = model.cd_aluno.ToString();
                }

                if (!model.TitulosMensalidade.IsNullOrEmpty() && atualizarPlanoConta)
                {
                    await SQLServerService.DeleteByTwoFields("T_TITULO", "cd_origem_titulo", model.cd_contrato.ToString(), "dc_tipo_titulo", model.TitulosMensalidade.First().dc_tipo_titulo, source);
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

                if (model.id_tipo_contrato != 2)
                {
                    //T_titulo_Material
                    if (!model.TitulosMaterial.IsNullOrEmpty() && atualizarPlanoConta)
                    {
                        await SQLServerService.DeleteByTwoFields("T_TITULO", "cd_origem_titulo", model.cd_contrato.ToString(), "dc_tipo_titulo", model.TitulosMaterial.First().dc_tipo_titulo, source);
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
                    foreach (var venda in model.VendasMaterial)
                    {
                        var item_escola = await SQLServerService.GetFirstByFields(source, "T_ITEM_ESCOLA", new List<(string campo, object valor)> { new("cd_item", venda.cd_item), new("cd_pessoa_escola", cd_escola) });
                        if (venda.venda)
                        {
                            if (item_escola != null)
                            {
                                var cd_item_escola = item_escola["cd_item_escola"];
                                var qtde = item_escola["qt_estoque"];
                                var qtde_item = int.Parse(qtde?.ToString() ?? "1");

                                if ((qtde_item - 1) < 0)
                                {
                                    continue;
                                }
                            }
                            nm_nf_material++;
                        }

                        var movimento_existente = await SQLServerService.GetFirstByFields(source, "T_MOVIMENTO", new List<(string campo, object valor)> { new("cd_curso", venda.cd_curso), new("cd_aluno", model.cd_aluno) });
                        var cd_movimento = 0;
                        Dictionary<string, object>? movimento = null;
                        if (movimento_existente == null)
                        {
                            //movimento
                            var movimento_dict = new Dictionary<string, object>
                                {
                                    { "cd_curso",venda.cd_curso },
                                    { "cd_aluno",model.cd_aluno },
                                    { "cd_pessoa_empresa", cd_escola},
                                    { "cd_pessoa", cd_pessoa_aluno},
                                    { "cd_politica_comercial", 2},
                                    { "cd_tipo_financeiro",  3 },
                                    { "id_tipo_movimento", 1 },
                                    { "dt_emissao_movimento", DateTime.Now.Date.ToString("yyyy-MM-ddTHH:mm:ss") },
                                    { "dt_vcto_movimento", DateTime.Now.Date.AddDays(60).ToString("yyyy-MM-ddTHH:mm:ss") },
                                    { "dt_mov_movimento", DateTime.Now.Date.ToString("yyyy-MM-ddTHH:mm:ss") },
                                    { "pc_acrescimo",  0 },
                                    { "vl_acrescimo",  0 },
                                    { "pc_desconto",  0 },
                                    { "vl_desconto", 0 },
                                    { "id_nf", 0},
                                    { "id_nf_escola", 0},
                                    { "vl_base_calculo_ICMS_nf", 0 },
                                    { "vl_base_calculo_PIS_nf", 0 },
                                    { "vl_base_calculo_COFINS_nf", 0},
                                    { "vl_base_calculo_IPI_nf", 0},
                                    { "vl_base_calculo_ISS_nf", 0},
                                    { "vl_ICMS_nf", 0 },
                                    { "vl_PIS_nf", 0 },
                                    { "vl_COFINS_nf", 0},
                                    { "vl_IPI_nf", 0 },
                                    { "vl_ISS_nf", 0 },
                                    { "pc_aliquota_aproximada", 0 },
                                    { "vl_aproximado", 0 },
                                    { "id_exportado", 0 },
                                    { "id_importacao_xml", 0 },
                                    { "id_material_didatico", 1 },
                                    { "id_venda_futura", venda.venda?0:1 },
                                    { "id_origem_movimento", 22 },
                                    { "nm_nfe",venda.venda?nm_nf_material:null }
                                };
                            var t_movimento_Result = await SQLServerService.Insert("T_MOVIMENTO", movimento_dict, source);
                            if (!t_movimento_Result.success) return BadRequest(t_movimento_Result.error);
                            var movimento_inseridoGet = await SQLServerService.GetList("T_MOVIMENTO", 1, 1, "cd_movimento", true, null, null, "", source, SearchModeEnum.Equals, null, null);
                            var movimento_inserido = movimento_inseridoGet.data.First();
                            movimento = movimento_inserido;
                            cd_movimento = int.Parse(movimento_inserido["cd_movimento"]?.ToString());
                            //movimento item
                        }
                        else
                        {
                            movimento = movimento_existente;
                            cd_movimento = int.Parse(movimento_existente["cd_movimento"]?.ToString());

                            var movimento_update_dict = new Dictionary<string, object>
                            {
                                { "id_venda_futura", venda.venda?0:1 },
                                { "nm_nfe",venda.venda?nm_nf_material:null }

                            };
                            var t_movimento_Result = await SQLServerService.Update("T_MOVIMENTO", movimento_update_dict, source, "cd_movimento", cd_movimento);
                            if (!t_movimento_Result.success) return BadRequest(t_movimento_Result.error);
                        }


                        var item_movimento_existente = await SQLServerService.GetFirstByFields(source, "T_ITEM_MOVIMENTO", new List<(string campo, object valor)> { new("cd_item", venda.cd_item), new("cd_movimento", cd_movimento) });
                        if (item_movimento_existente == null)
                        {
                            var item_movimento_dict = new Dictionary<string, object>
                                {
                                    { "cd_movimento", cd_movimento },
                                    { "cd_item",venda.cd_item },
                                    { "qt_item_movimento", 1 },
                                    { "vl_unitario_item",  0  },
                                    { "vl_total_item",0 },
                                    { "vl_liquido_item",  0 },
                                    { "vl_acrescimo_item", 0 },
                                    { "vl_desconto_item", movimento.ContainsKey("vl_desconto") ? movimento["vl_desconto"] : 0 },
                                    { "vl_base_calculo_ICMS_item",0 },
                                    { "vl_base_calculo_PIS_item",0 },
                                    { "vl_base_calculo_COFINS_item", 0 },
                                    { "vl_base_calculo_IPI_item",0 },
                                    { "vl_base_calculo_ISS_item", 0 },
                                    { "vl_ICMS_item",0 },
                                    { "vl_PIS_item", 0},
                                    { "vl_COFINS_item", 0 },
                                    { "vl_IPI_item", 0 },
                                    { "vl_ISS_item", 0 },
                                    { "pc_aliquota_ICMS", 0},
                                    { "pc_aliquota_PIS", 0},
                                    { "pc_aliquota_COFINS", 0 },
                                    { "pc_aliquota_IPI", 0 },
                                    { "pc_aliquota_ISS", 0 },
                                    { "pc_aliquota_aproximada", movimento.ContainsKey("pc_aliquota_aproximada") ? movimento["pc_aliquota_aproximada"] : 0 },
                                    { "vl_aproximado", movimento.ContainsKey("vl_aproximado") ? movimento["vl_aproximado"] : 0},
                                    { "pc_desconto_item", movimento.ContainsKey("pc_desconto") ? movimento["pc_desconto"] : 0 }
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
                                    continue;
                                }
                                item_escola.Remove("cd_item_escola");
                                item_escola["qt_estoque"] = int.Parse(qtde?.ToString() ?? "1") - 1;
                                var t_item_escola_update = await SQLServerService.Update("T_ITEM_ESCOLA", item_escola, source, "cd_item_escola", cd_item_escola);
                                if (!t_item_escola_update.success) return BadRequest(t_item_escola_update.error);

                                //atualiza nm_nf_material
                                var parametroUpdate = new Dictionary<string, object>
                                {
                                    { "nm_nf_material", nm_nf_material }
                                };
                                var parametroResult = await SQLServerService.Update("T_PARAMETRO", parametroUpdate, source, "cd_pessoa_escola", model.cd_pessoa_escola);
                                if (!parametroResult.success) return BadRequest(parametroResult.error);
                            }
                        }



                    }


                }


                var curso_contratos_get = await SQLServerService.GetList("T_CURSO_CONTRATO", null, "[cd_contrato]", $"[{model.cd_contrato}]", source);
                var cursosContrato = curso_contratos_get.data.Select(x => int.Parse(x["cd_curso_contrato"].ToString())).ToList();


                //atualizar valores curso contrato
                foreach (var cursoContratoId in cursosContrato)
                {
                    var cursoContratoAtualizar = new Dictionary<string, object>
                    {
                        ["vl_material_contrato"] = model.vl_material_contrato
                    };

                    var curso_contrato_update = await SQLServerService.Update("T_CURSO_CONTRATO", cursoContratoAtualizar, source, "cd_curso_contrato", cursoContratoId);
                    if (!curso_contrato_update.success) return BadRequest(curso_contrato_update.error);
                }

                //turma
                if (!model.Turmas.IsNullOrEmpty())
                {
                    foreach (var turma in model.Turmas)
                    {
                        var filtroTurma = new List<(string campo, object valor)> { new("cd_turma", turma.cd_turma) };
                        var turmaExists = await SQLServerService.GetFirstByFields(source, "T_TURMA", filtroTurma);
                        if (turmaExists == null) continue;
                        var no_turma = turmaExists["no_turma"];

                        if (no_turma == null) continue;
                        var cd_turma_original = turmaExists["cd_turma"];
                        var original = no_turma?.ToString() ?? string.Empty;

                        var partes = original.Split('/', 2); // corta só na primeira barra
                        bool primeiroEhPERS = partes.Length == 2 &&
                                              string.Equals(partes[0], "PERS", StringComparison.OrdinalIgnoreCase);

                        var situacao_aluno = model.id_tipo_matricula == 1 ? 1 :
                                      model.id_tipo_matricula == 3 ? 10 :
                                      model.id_tipo_matricula == 2 ? 8 : 9;

                        var dt_inicio = model.dt_inicial_contrato > turma.dt_inicio_aula ? model.dt_inicial_contrato : turma.dt_inicio_aula;
                        if (primeiroEhPERS)
                        {
                            //remove campos que não serão inseridos
                            turmaExists.Remove("cd_turma");
                            turmaExists.Remove("no_turma");

                            string novo_nome = primeiroEhPERS
                                ? $"PERSF/{partes[1]}"   // troca PERS -> PERSF e mantém o resto
                                : original;

                            // adiciona nome montado
                            turmaExists.Add("no_turma", novo_nome);

                            var t_turma_insert = await SQLServerService.Insert("T_TURMA", turmaExists, source);
                            if (!t_turma_insert.success) continue;

                            var turmaCadastradaGet = await SQLServerService.GetList("T_TURMA", 1, 1, "cd_turma", true, null, null, "", source, SearchModeEnum.Equals, null, null);
                            var turmaCadastrada = turmaCadastradaGet.data.First();
                            int cdTurmaId = (int)turmaCadastrada["cd_turma"];

                            var horario = await SQLServerService.GetList("T_HORARIO", 1, 10000000, "cd_horario", true, null, "[{cd_turma}]", $"[{cd_turma_original}]", source, SearchModeEnum.Equals, null, null);
                            var turma_escola = await SQLServerService.GetList("T_TURMA_ESCOLA", 1, 10000000, "cd_turma_escola", true, null, "[{cd_turma}]", $"[{cd_turma_original}]", source, SearchModeEnum.Equals, null, null);
                            var turma_professor = await SQLServerService.GetList("T_TURMA_PROFESSOR", 1, 10000000, "cd_turma_professor", true, null, "[{cd_turma}]", $"[{cd_turma_original}]", source, SearchModeEnum.Equals, null, null);
                            var programacao_turma = await SQLServerService.GetList("T_PROGRAMACAO_TURMA", 1, 10000000, "cd_programacao_turma", true, null, "[{cd_turma}]", $"[{cd_turma_original}]", source, SearchModeEnum.Equals, null, null);
                            var horario_professor_turma = await SQLServerService.GetList("T_HORARIO_PROFESSOR_TURMA", 1, 10000000, "cd_horario_professor_turma", true, null, "[{cd_turma}]", $"[{cd_turma_original}]", source, SearchModeEnum.Equals, null, null);

                            var feriado_desconsiderado = await SQLServerService.GetList("T_FERIADO_DESCONSIDERADO", 1, 10000000, "cd_feriado_desconsiderado", true, null, "[{cd_turma}]", $"[{cd_turma_original}]", source, SearchModeEnum.Equals, null, null);

                            //vinculos para nova turma criada
                            foreach (var item in horario.data)
                            {
                                item.Remove("cd_horario");
                                item["cd_turma"] = cdTurmaId;
                                var t_insert = await SQLServerService.Insert("T_HORARIO", item, source);
                                if (!t_insert.success) continue;
                            }
                            foreach (var item in turma_escola.data)
                            {
                                item.Remove("cd_turma_escola");
                                item["cd_turma"] = cdTurmaId;
                                var t_insert = await SQLServerService.Insert("T_TURMA_ESCOLA", item, source);
                                if (!t_insert.success) continue;
                            }
                            foreach (var item in turma_professor.data)
                            {
                                item.Remove("cd_turma_professor");
                                item["cd_turma"] = cdTurmaId;
                                var t_insert = await SQLServerService.Insert("T_TURMA_PROFESSOR", item, source);
                                if (!t_insert.success) continue;
                            }
                            foreach (var item in turma_professor.data)
                            {
                                item.Remove("cd_turma_professor");
                                item["cd_turma"] = cdTurmaId;
                                var t_insert = await SQLServerService.Insert("T_TURMA_PROFESSOR", item, source);
                                if (!t_insert.success) continue;
                            }
                            foreach (var item in programacao_turma.data)
                            {
                                item.Remove("cd_programacao_turma");
                                item["cd_turma"] = cdTurmaId;
                                var t_insert = await SQLServerService.Insert("T_PROGRAMACAO_TURMA", item, source);
                                if (!t_insert.success) continue;
                            }
                            foreach (var item in horario_professor_turma.data)
                            {
                                item.Remove("cd_horario_professor_turma");
                                item["cd_turma"] = cdTurmaId;
                                var t_insert = await SQLServerService.Insert("T_HORARIO_PROFESSOR_TURMA", item, source);
                                if (!t_insert.success) continue;
                            }
                            foreach (var item in feriado_desconsiderado.data)
                            {
                                item.Remove("cd_feriado_desconsiderado");
                                item["cd_turma"] = cdTurmaId;
                                var t_insert = await SQLServerService.Insert("T_FERIADO_DESCONSIDERADO", item, source);
                                if (!t_insert.success) continue;
                            }

                            foreach (var cursoContratoId in cursosContrato)
                            {
                                var cursoContratoAtualizar = new Dictionary<string, object>
                                {
                                    ["cd_turma"] = cdTurmaId
                                };

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
                        }
                        else
                        {
                            //validação aluno existente
                            var filtrosAluno = new List<(string campo, object valor)> { new("cd_aluno", model.cd_aluno), new("cd_situacao_aluno_turma", 9) };
                            var alunoExists = await SQLServerService.GetFirstByFields(source, "T_ALUNO_TURMA", filtrosAluno);

                            if (alunoExists != null)
                            {
                                foreach (var cursoContratoId in cursosContrato)
                                {
                                    //atualiza cd_contrato e situação aluno
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

        //[Authorize]
        //[HttpPost]
        //[Route("venda_material")]
        //public async Task<IActionResult> VendaMaterial(VendaMaterial vendaMaterial, int cd_contrato)
        //{
        //    var schemaName = "T_Pessoa";
        //    if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
        //    var schema = _schemaRepository.GetSchemaByField("name", schemaName);
        //    var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
        //    var source = _sourceRepository.GetByField("description", schemaModel.Source);
        //    if (source != null && source.Active != null && source.Active == true)
        //    {
        //        //buscar Contrato
        //        var filtrosContrato = new List<(string campo, object valor)> { new("cd_contrato", cd_contrato) };
        //        var contrato = await SQLServerService.GetFirstByFields(source, "T_CONTRATO", filtrosContrato);
        //        if (contrato == null) return NotFound("contrato");

        //        var cd_regime_atual = contrato["cd_regime_atual"]?.ToString() ?? "0";
        //        //cd_pessoa_escola
        //        var cd_pessoa_escola = contrato["cd_pessoa_escola"]?.ToString() ?? "0";

        //        var cd_aluno = contrato["cd_aluno"];
        //        var aluno = await SQLServerService.GetFirstByFields(source, "T_ALUNO", new List<(string campo, object valor)> { new("cd_aluno", cd_aluno) });

        //        if (aluno == null) return BadRequest("aluno não encontrado");
        //        var cd_pessoa_aluno = aluno["cd_pessoa_aluno"];

        //        List<int> itens_movimento = new List<int>();
        //        if (vendaMaterial != null)
        //        {
        //            //validar venda material
        //            var resultado = await ValidaVendaMaterial(vendaMaterial, source, int.Parse(cd_pessoa_escola), int.Parse(cd_regime_atual ?? "0"));
        //            if (!resultado.valido) return BadRequest(resultado.msg);
        //            itens_movimento = resultado.cd_itens;
        //        }
        //        if (!itens_movimento.Any()) return BadRequest("nenhum item valido encontrato");

        //        //buscar todos os titulos do contrato
        //        var titulos_result = await SQLServerService.GetList("T_TITULO", null, "[cd_origem_titulo]", $"[{cd_contrato}]", source, SearchModeEnum.Equals);

        //        if (!titulos_result.success) return BadRequest(titulos_result.error);
        //        var titulos = titulos_result.data;
        //        //
        //        foreach (var cd_item in itens_movimento)
        //        {
        //            //filtrar os titulos por cd_item para pegar infos de movimento
        //            var titulo_material = titulos.FirstOrDefault(x => x["cd_origem_titulo"]?.ToString() == cd_item.ToString());

        //            //movimento
        //            var movimento_dict = new Dictionary<string, object>
        //                        {
        //                            //{ "cd_curso",cd_curso },
        //                            { "cd_aluno",cd_aluno },
        //                            { "cd_pessoa_empresa", cd_pessoa_escola},
        //                            { "cd_pessoa", cd_pessoa_aluno},
        //                            { "cd_politica_comercial", 2},
        //                            { "cd_tipo_financeiro", titulo_material != null?titulo_material["cd_tipo_financeiro"] : 3 },
        //                            { "id_tipo_movimento", 1 },
        //                            { "dt_emissao_movimento", titulo_material != null?titulo_material["dt_emissao_titulo"]: DateTime.Now.Date.ToString("yyyy-MM-ddTHH:mm:ss") },
        //                            { "dt_vcto_movimento", titulo_material != null?titulo_material["dt_vcto_titulo"]: DateTime.Now.Date.AddDays(60).ToString("yyyy-MM-ddTHH:mm:ss") },
        //                            { "dt_mov_movimento", DateTime.Now.Date.ToString("yyyy-MM-ddTHH:mm:ss") },
        //                            { "pc_acrescimo", titulo_material != null?titulo_material["pc_taxa_cartao"] : 0 },
        //                            { "vl_acrescimo", titulo_material != null?titulo_material["vl_taxa_cartao"] : 0 },
        //                            { "pc_desconto", titulo_material != null?titulo_material["pc_desconto_total"] : 0 },
        //                            { "vl_desconto", titulo_material != null?titulo_material["vl_desconto_total"] : 0 },
        //                            { "id_nf", 0},
        //                            { "id_nf_escola", 0},
        //                            { "vl_base_calculo_ICMS_nf", 0 },
        //                            { "vl_base_calculo_PIS_nf", 0 },
        //                            { "vl_base_calculo_COFINS_nf", 0},
        //                            { "vl_base_calculo_IPI_nf", 0},
        //                            { "vl_base_calculo_ISS_nf", 0},
        //                            { "vl_ICMS_nf", 0 },
        //                            { "vl_PIS_nf", 0 },
        //                            { "vl_COFINS_nf", 0},
        //                            { "vl_IPI_nf", 0 },
        //                            { "vl_ISS_nf", 0 },
        //                            { "pc_aliquota_aproximada", 0 },
        //                            { "vl_aproximado", titulo_material != null?titulo_material["vl_titulo"] : 0 },
        //                            { "id_exportado", 0 },
        //                            { "id_importacao_xml", 0 },
        //                            { "id_material_didatico", 1 },
        //                            { "id_venda_futura", 0 }
        //                        };
        //            var t_movimento_Result = await SQLServerService.Insert("T_MOVIMENTO", movimento_dict, source);
        //            if (!t_movimento_Result.success) return BadRequest(t_movimento_Result.error);
        //            var movimento_inseridoGet = await SQLServerService.GetList("T_MOVIMENTO", 1, 1, "cd_movimento", true, null, null, "", source, SearchModeEnum.Equals, null, null);
        //            var movimento_inserido = movimento_inseridoGet.data.First();
        //            var cd_movimento = movimento_inserido["cd_movimento"];
        //            //movimento item

        //            var item_movimento_dict = new Dictionary<string, object>
        //                        {
        //                            { "cd_movimento", cd_movimento },
        //                            { "cd_item",cd_item },
        //                            { "qt_item_movimento", 1 },
        //                            { "vl_unitario_item", titulo_material != null? titulo_material["vl_material_titulo"] : 0  },
        //                            { "vl_total_item",titulo_material != null? titulo_material["vl_material_titulo"] : 0 },
        //                            { "vl_liquido_item", titulo_material != null? titulo_material["vl_material_titulo"] : 0 },
        //                            { "vl_acrescimo_item", 0 },
        //                            { "vl_desconto_item", movimento_dict.ContainsKey("vl_desconto") ? movimento_dict["vl_desconto"] : 0 },
        //                            { "vl_base_calculo_ICMS_item",0 },
        //                            { "vl_base_calculo_PIS_item",0 },
        //                            { "vl_base_calculo_COFINS_item", 0 },
        //                            { "vl_base_calculo_IPI_item",0 },
        //                            { "vl_base_calculo_ISS_item", 0 },
        //                            { "vl_ICMS_item",0 },
        //                            { "vl_PIS_item", 0},
        //                            { "vl_COFINS_item", 0 },
        //                            { "vl_IPI_item", 0 },
        //                            { "vl_ISS_item", 0 },
        //                            { "pc_aliquota_ICMS", 0},
        //                            { "pc_aliquota_PIS", 0},
        //                            { "pc_aliquota_COFINS", 0 },
        //                            { "pc_aliquota_IPI", 0 },
        //                            { "pc_aliquota_ISS", 0 },
        //                            { "pc_aliquota_aproximada", movimento_dict.ContainsKey("pc_aliquota_aproximada") ? movimento_dict["pc_aliquota_aproximada"] : 0 },
        //                            { "vl_aproximado", movimento_dict.ContainsKey("vl_aproximado") ? movimento_dict["vl_aproximado"] : 0},
        //                            { "pc_desconto_item", movimento_dict.ContainsKey("pc_desconto") ? movimento_dict["pc_desconto"] : 0 }
        //                        };
        //            var t_item_movimento_Result = await SQLServerService.Insert("T_ITEM_MOVIMENTO", item_movimento_dict, source);
        //            if (!t_item_movimento_Result.success) return BadRequest(t_item_movimento_Result.error);
        //            //remover do estoque

        //            var item_escola = await SQLServerService.GetFirstByFields(source, "T_ITEM_ESCOLA", new List<(string campo, object valor)> { new("cd_item", cd_item) });
        //            if (item_escola != null)
        //            {
        //                var cd_item_escola = item_escola["cd_item_escola"];
        //                var qtde = item_escola["qt_estoque"];

        //                item_escola.Remove("cd_item_escola");
        //                item_escola["qt_estoque"] = int.Parse(qtde?.ToString() ?? "1") - 1;
        //                var t_item_escola_update = await SQLServerService.Update("T_ITEM_ESCOLA", item_escola, source, "cd_item_escola", cd_item_escola);
        //                if (!t_item_escola_update.success) return BadRequest(t_item_escola_update.error);
        //            }
        //        }

        //        if (itens_movimento.Any())
        //        {
        //            //validar se pessoa ja não possui Raf
        //            var pessoaRaf = await SQLServerService.GetFirstByFields(source, "T_PESSOA_RAF", new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa_aluno) });

        //            //valida se pessoa esta em alguma turma
        //            var alunoTurma = await SQLServerService.GetFirstByFields(source, "T_ALUNO_TURMA", new List<(string campo, object valor)> { new("cd_aluno", cd_aluno) });

        //            if (pessoaRaf == null && alunoTurma != null)
        //            {
        //                //pegar ultimo raf existente
        //                var ultimoRafGet = await SQLServerService.GetList("T_PESSOA_RAF", 1, 1, "cd_pessoa_raf", true, null, null, "", source, SearchModeEnum.Equals, null, null);
        //                var ultimo_raf = ultimoRafGet.data.FirstOrDefault();
        //                if (ultimo_raf != null)
        //                {
        //                    var ultimo_nm_raf = ultimo_raf["nm_raf"]?.ToString() ?? "";

        //                    var partes = ultimo_nm_raf.Split('-');
        //                    string prefixo = partes[0];
        //                    int numero = int.Parse(partes[1]);
        //                    numero++;
        //                    string novo_nm_raf = $"{prefixo}-{numero:D3}";
        //                    //gerar novo raf
        //                    var novoRafPessoa = new Dictionary<string, object>
        //                        {
        //                            { "cd_pessoa", cd_pessoa_aluno },
        //                            { "nm_raf",novo_nm_raf  },
        //                            { "dc_senha_raf", null },
        //                            { "id_raf_liberado", 1 },
        //                            { "nm_tentativa", 0 },
        //                            { "id_bloqueado", 0 },
        //                            { "id_trocar_senha", 1 },
        //                            { "dt_expiracao_senha", DateTime.Now.Date.AddDays(30).ToString("yyyy-MM-ddTHH:mm:ss") },
        //                        };

        //                    var t_pessoa_raf_Result = await SQLServerService.Insert("T_PESSOA_RAF", novoRafPessoa, source);
        //                    if (!t_pessoa_raf_Result.success) return BadRequest(t_pessoa_raf_Result.error);
        //                }
        //            }
        //        }
        //        return ResponseDefault();
        //    }
        //    return BadRequest(new
        //    {
        //        error = "Fonte de dados não configurada ou inativa."
        //    });
        //}

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
                var ultimo_titulo_contratoGet = await SQLServerService.GetList("T_TITULO", 1, 1, "cd_titulo", true, null, "[cd_origem_titulo]", $"[{cd_contrato}]", source, SearchModeEnum.Equals, null, null);
                var ultimo_titulo_contrato = ultimo_titulo_contratoGet.data.FirstOrDefault();

                var nm_contrato = matriculaExists["nm_contrato"];
                var responsavel = matriculaExists["cd_pessoa_responsavel"];

                var filtroParametro = new List<(string campo, object valor)> { new("cd_pessoa_escola", cd_escola) };
                var parametroExists = await SQLServerService.GetFirstByFields(source, "T_PARAMETRO", filtroParametro);
                if (parametroExists == null) return NotFound($"parametros não encontratos para esta escola({cd_escola})");


                var nm_contrato_p = parametroExists["nm_ultimo_contrato"].ToString() ?? "0";
                var nm_matricula_p = parametroExists["nm_ultimo_matricula"].ToString() ?? "0";
                var cd_plano_conta_mat = parametroExists["cd_plano_conta_mat"].ToString() ?? "0";
                var cd_plano_conta_tax = parametroExists["cd_plano_conta_tax"].ToString() ?? "0";
                var cd_plano_conta_mtr = parametroExists["cd_plano_conta_material"].ToString() ?? "0";

                //Aditamentos
                if (!model.IsNullOrEmpty())
                {
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

                        if (ad.pc_bolsa != null) dict_bolsa["pc_bolsa"] = ad.pc_bolsa;

                        if (ad.dt_comunicado_bolsa != null) dict_bolsa["dt_comunicado_bolsa"] = ad.dt_comunicado_bolsa?.ToString("yyyy-MM-ddTHH:mm:ss");

                        if (ad.dc_validade_bolsa != null) dict_bolsa["dc_validade_bolsa"] = ad.dc_validade_bolsa;

                        if (ad.cd_motivo_bolsa != null) dict_bolsa["cd_motivo_bolsa"] = ad.cd_motivo_bolsa;

                        int? cd_aditamento = ad.cd_aditamento;

                        var desconto_contrato = new Dictionary<string, object>
                        {
                            { "pc_desconto_contrato", ad.pc_desconto_contrato },
                            { "id_desconto_ativo", 1 },
                            { "vl_desconto_contrato", ad.vl_desconto_contrato ??0 },
                            { "id_incide_baixa", 0 },
                            { "nm_parcela_ini", 1 },
                            { "nm_parcela_fim", 1 },
                            { "id_incide_parcela_1", 0 },
                            { "id_aditamento", 1 }
                        };

                        if (ad.cd_aditamento == null)
                        {
                            var t_aditamento_Result = await SQLServerService.Insert("T_ADITAMENTO", dict, source);
                            if (!t_aditamento_Result.success) continue;
                            var aditamentoCadastradaGet = await SQLServerService.GetList("T_ADITAMENTO", 1, 1, "cd_aditamento", true, null, null, "", source, SearchModeEnum.Equals, null, null);
                            var aditamentoCadastrado = aditamentoCadastradaGet.data.First();
                            cd_aditamento = int.Parse(aditamentoCadastrado["cd_aditamento"].ToString());
                            await AddHistoricoAditamento(cd_aditamento.Value, ad.cd_usuario, 0, source);
                            if (ad.pc_bolsa != null)
                            {
                                dict_bolsa.Add("cd_aditamento", cd_aditamento);
                                var t_aditamento_bolsa_Result = await SQLServerService.Insert("T_ADITAMENTO_BOLSA", dict_bolsa, source);
                                if (!t_aditamento_bolsa_Result.success) continue;
                            }
                            if (ad.pc_desconto_contrato != null && ad.vl_desconto_contrato != null && ad.id_tipo_aditamento == 3)
                            {
                                desconto_contrato.Add("cd_aditamento", cd_aditamento);
                                var t_desconto_contrato_Result = await SQLServerService.Insert("T_DESCONTO_CONTRATO", desconto_contrato, source);
                                if (!t_desconto_contrato_Result.success) continue;
                            }
                        }
                        else
                        {
                            var t_aditamento_Result = await SQLServerService.Update("T_ADITAMENTO", dict, source, "cd_aditamento", ad.cd_aditamento);
                            if (!t_aditamento_Result.success) continue;
                            var t_aditamento_historico_result = await SQLServerService.GetFirstByFields(source, "T_ADITAMENTO_HISTORICO", new List<(string campo, object valor)> { new("cd_aditamento", ad.cd_aditamento.ToString()) });
                            if (t_aditamento_historico_result == null) await AddHistoricoAditamento(cd_aditamento.Value, ad.cd_usuario, 0, source);

                            var filtros_bolsa = new List<(string campo, object valor)> { new("cd_aditamento", ad.cd_aditamento.ToString()) };
                            var t_aditamento_bolsa_result = await SQLServerService.GetFirstByFields(source, "T_ADITAMENTO_BOLSA", filtros_bolsa);

                            var t_desconto_result = await SQLServerService.GetFirstByFields(source, "T_DESCONTO_CONTRATO", new List<(string campo, object valor)> { new("cd_aditamento", ad.cd_aditamento.ToString()) });

                            if (t_desconto_result == null)
                            {
                                if (ad.pc_desconto_contrato != null && ad.vl_desconto_contrato != null && ad.id_tipo_aditamento == 3)
                                {
                                    desconto_contrato.Add("cd_aditamento", cd_aditamento);
                                    var t_desconto_contrato_Result = await SQLServerService.Insert("T_DESCONTO_CONTRATO", desconto_contrato, source);
                                    if (!t_desconto_contrato_Result.success) continue;
                                }


                            }
                            else
                            {
                                desconto_contrato.Add("cd_aditamento", cd_aditamento);
                                var cd_desconto_contrato = t_desconto_result["cd_desconto_contrato"];
                                var t_desconto_contrato_Result = await SQLServerService.Update("T_DESCONTO_CONTRATO", desconto_contrato, source, "cd_desconto_contrato", cd_desconto_contrato);
                                if (!t_desconto_contrato_Result.success) continue;
                            }


                            if (t_aditamento_bolsa_result == null)
                            {
                                if (dict_bolsa.Any())
                                {
                                    dict_bolsa.Add("cd_aditamento", ad.cd_aditamento);
                                    var t_aditamento_bolsa_Result = await SQLServerService.Insert("T_ADITAMENTO_BOLSA", dict_bolsa, source);
                                    if (!t_aditamento_bolsa_Result.success) continue;
                                }
                            }
                            else
                            {
                                if (dict_bolsa.Any())
                                {
                                    dict_bolsa.Add("cd_aditamento", ad.cd_aditamento);
                                    var cd_aditamento_bolsa = t_aditamento_bolsa_result["cd_aditamento_bolsa"];
                                    var t_aditamento_bolsa_Result = await SQLServerService.Update("T_ADITAMENTO_BOLSA", dict_bolsa, source, "cd_aditamento_bolsa", cd_aditamento_bolsa);
                                    if (!t_aditamento_bolsa_Result.success) continue;
                                }
                            }
                        }


                        //adiciona titulos para adicionar parcelas e adicionar parcelas material
                        if (ad.id_tipo_aditamento == 5 || ad.id_tipo_aditamento == 8)
                        {
                            var dictTitulo = new Dictionary<string, object>
                            {
                                {"cd_origem_titulo",cd_aditamento },
                                { "cd_pessoa_empresa", cd_escola },
                                { "cd_pessoa_titulo", ultimo_titulo_contrato["cd_pessoa_titulo"] },
                                { "cd_pessoa_responsavel", cd_pessoa_responsavel },
                                { "cd_local_movto",  ultimo_titulo_contrato["cd_local_movto"]??0},
                                { "dt_emissao_titulo",  DateTime.Now.Date.ToString("yyyy-MM-ddTHH:mm:ss") },
                                { "dt_vcto_titulo", ad.dt_vcto_aditamento.ToString() ?? DateTime.Now.Date.ToString("yyyy-MM-ddTHH:mm:ss") },
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

                        if (ad.id_tipo_aditamento == 2 || ad.id_tipo_aditamento == 3)
                        {
                            if (!ad.TitulosMensalidade.IsNullOrEmpty())
                            {
                                foreach (var titulo in ad.TitulosMensalidade)
                                {

                                    var dict_titulo_inativar = new Dictionary<string, object>
                                    {
                                        ["id_status_titulo"] = 0
                                    };
                                    var t_titulo_update_Result = await SQLServerService.Update("T_TITULO", dict_titulo_inativar, source, "cd_titulo", titulo.cd_titulo);
                                    if (!t_titulo_update_Result.success) return BadRequest(t_titulo_update_Result.error);


                                    var dictTitulo = new Dictionary<string, object>
                                    {
                                        ["cd_pessoa_empresa"] = cd_escola,
                                        ["cd_pessoa_titulo"] = titulo.cd_pessoa_titulo,
                                        ["cd_pessoa_responsavel"] = titulo.cd_pessoa_responsavel != 0 ? titulo.cd_pessoa_responsavel : responsavel,
                                        ["cd_local_movto"] = titulo.cd_local_movto,
                                        ["dt_emissao_titulo"] = titulo.dt_emissao_titulo.ToString("yyyy-MM-ddTHH:mm:ss"),
                                        ["cd_origem_titulo"] = cd_contrato,
                                        ["dt_vcto_titulo"] = titulo.dt_vcto_titulo.ToString("yyyy-MM-ddTHH:mm:ss"),
                                        ["dh_cadastro_titulo"] = DateTime.Now.Date.ToString("yyyy-MM-ddTHH:mm:ss"),
                                        ["vl_titulo"] = titulo.vl_titulo,
                                        ["vl_saldo_titulo"] = titulo.vl_saldo_titulo,
                                        ["dc_tipo_titulo"] = titulo.dc_tipo_titulo,
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

                            if (!ad.TitulosMaterial.IsNullOrEmpty())
                            {

                                foreach (var titulo in ad.TitulosMaterial)
                                {
                                    var dict_titulo_inativar = new Dictionary<string, object>
                                    {
                                        ["id_status_titulo"] = 0
                                    };
                                    var t_titulo_update_Result = await SQLServerService.Update("T_TITULO", dict_titulo_inativar, source, "cd_titulo", titulo.cd_titulo);
                                    if (!t_titulo_update_Result.success) return BadRequest(t_titulo_update_Result.error);

                                    var dictTitulo = new Dictionary<string, object>
                                    {
                                        ["cd_pessoa_empresa"] = cd_escola,
                                        ["cd_pessoa_titulo"] = titulo.cd_pessoa_titulo,
                                        ["cd_pessoa_responsavel"] = titulo.cd_pessoa_responsavel != 0 ? titulo.cd_pessoa_responsavel : responsavel,
                                        ["cd_local_movto"] = titulo.cd_local_movto,
                                        ["dt_emissao_titulo"] = titulo.dt_emissao_titulo.ToString("yyyy-MM-ddTHH:mm:ss"),
                                        ["cd_origem_titulo"] = cd_contrato,
                                        ["dt_vcto_titulo"] = titulo.dt_vcto_titulo.ToString("yyyy-MM-ddTHH:mm:ss"),
                                        ["dh_cadastro_titulo"] = DateTime.Now.Date,
                                        ["vl_titulo"] = titulo.vl_titulo,
                                        ["vl_saldo_titulo"] = titulo.vl_saldo_titulo,
                                        ["dc_tipo_titulo"] = titulo.dc_tipo_titulo,
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
                    }

                    //desconto e remoção desconto



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
            var schemaName = "T_CONTRATO";
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
                if (tokenInfo.Count > 0) cd_pessoa_logada = tokenInfo["cd_pessoa"];

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
        public async Task<IActionResult> GerarContrato(int cd_contrato)
        {
            var schemaName = "T_Pessoa";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                //valida se matricula existe
                var matriculaExists = await SQLServerService.GetFirstByFields(source, "T_CONTRATO", new List<(string campo, object valor)> { new("cd_contrato", cd_contrato) });
                if (matriculaExists == null) return NotFound("contrato");
                var cd_nome_contrato = matriculaExists["cd_nome_contrato"];
                if (cd_nome_contrato == null) return BadRequest("Contrato não possui modelo de contrato definido.");
                //pegar nome do contrato
                var nome_contrato = await SQLServerService.GetFirstByFields(source, "T_NOME_CONTRATO", new List<(string campo, object valor)> { new("cd_nome_contrato", cd_nome_contrato) });
                if (nome_contrato == null) NotFound("nome contrato não encontrado");

                var nomeContrato = nome_contrato["no_relatorio"]?.ToString();

                var cd_pessoa_escola = matriculaExists["cd_pessoa_escola"];
                //campos para substituir no contrato

                #region ESCOLA
                //ESCOLA
                var pessoa_escola = await SQLServerService.GetFirstByFields(source, "T_PESSOA", new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa_escola) });
                var nomeEscola = pessoa_escola["dc_reduzido_pessoa"]?.ToString() ?? "";
                var razaoSocialEscola = pessoa_escola["no_pessoa"]?.ToString() ?? "";
                var pessoa_escola_juridica = await SQLServerService.GetFirstByFields(source, "T_PESSOA_JURIDICA", new List<(string campo, object valor)> { new("cd_pessoa_juridica", cd_pessoa_escola) });
                var cnpjEscola = pessoa_escola_juridica != null ? pessoa_escola_juridica["dc_num_cgc"]?.ToString() ?? pessoa_escola_juridica["dc_num_cnpj_cnab"]?.ToString() ?? "" : "";
                var endereco_escola = await SQLServerService.GetFirstByFields(source, "T_ENDERECO", new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa_escola) });
                var enderecoEscolaMontado = "";
                var cidadeEstadoEscola = "";
                if (endereco_escola != null)
                {
                    var filtroLogradouro = new List<(string campo, object valor)> { new("cd_localidade", endereco_escola["cd_loc_logradouro"].ToString()) };
                    var logradouroExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroLogradouro);
                    if (logradouroExists != null)
                    {
                        enderecoEscolaMontado = $"{logradouroExists["no_localidade"]},{endereco_escola["dc_num_endereco"]}";
                    }

                    var filtroEstado = new List<(string campo, object valor)> { new("cd_localidade", endereco_escola["cd_loc_estado"].ToString()) };
                    var estadoExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroEstado);
                    if (estadoExists != null)
                    {

                        var filtroCidade = new List<(string campo, object valor)> { new("cd_localidade", endereco_escola["cd_loc_cidade"].ToString()) };
                        var cidadeExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroCidade);
                        if (cidadeExists != null)
                        {
                            cidadeEstadoEscola = $"{cidadeExists["no_localidade"]}/{estadoExists["no_localidade"]}";
                        }

                    }


                }
                #endregion

                #region RESPONSAVEL
                // RESPONSAVEL
                var cd_responsavel = matriculaExists["cd_pessoa_responsavel"];
                var pessoa_responsavel = await SQLServerService.GetFirstByFields(source, "T_PESSOA", new List<(string campo, object valor)> { new("cd_pessoa", cd_responsavel) });
                var nomeResponsavel = pessoa_responsavel["no_pessoa"]?.ToString() ?? "";
                var pessoa_responsavel_fisica = await SQLServerService.GetFirstByFields(source, "T_PESSOA_FISICA", new List<(string campo, object valor)> { new("cd_pessoa_fisica", cd_responsavel) });
                var rg_pessoa_responsavel = pessoa_responsavel_fisica != null ? pessoa_responsavel_fisica["nm_doc_identidade"]?.ToString() ?? "" : "";
                var cpfResponsavel = pessoa_responsavel_fisica != null ? pessoa_responsavel_fisica["nm_cpf"]?.ToString() ?? "" : "";
                var telefoneResponsavel = "";
                var telefone_responsavel = await SQLServerService.GetFirstByFields(source, "T_TELEFONE", new List<(string campo, object valor)> { new("cd_pessoa", cd_responsavel), new("cd_tipo_telefone", 1) });
                if (telefone_responsavel != null)
                {
                    telefoneResponsavel = telefone_responsavel["dc_fone_mail"]?.ToString() ?? "";
                }
                var endereco_responsavel = await SQLServerService.GetFirstByFields(source, "T_ENDERECO", new List<(string campo, object valor)> { new("cd_pessoa", cd_responsavel) });
                var enderecoResponsavel = "";
                if (endereco_responsavel != null)
                {
                    var filtroLogradouro = new List<(string campo, object valor)> { new("cd_localidade", endereco_responsavel["cd_loc_logradouro"].ToString()) };
                    var logradouroExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroLogradouro);
                    if (logradouroExists != null)
                    {
                        enderecoResponsavel = $"{logradouroExists["no_localidade"]},{endereco_responsavel["dc_num_endereco"]}";
                    }
                }

                #endregion
                //ALUNO
                #region ALUNO
                var cd_aluno = matriculaExists["cd_aluno"];
                var aluno = await SQLServerService.GetFirstByFields(source, "T_ALUNO", new List<(string campo, object valor)> { new("cd_aluno", cd_aluno) });
                if (aluno == null) return BadRequest("aluno não encontrado");
                var cd_pessoa_aluno = aluno["cd_pessoa_aluno"];
                var pessoa_aluno = await SQLServerService.GetFirstByFields(source, "T_PESSOA", new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa_aluno) });
                if (pessoa_aluno == null) return BadRequest("pessoa aluno não encontrada");
                var nomeAluno = pessoa_aluno["no_pessoa"]?.ToString() ?? "";
                var telefoneAluno = "";
                var telefone_aluno = await SQLServerService.GetFirstByFields(source, "T_TELEFONE", new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa_aluno), new("cd_tipo_telefone", 1) });
                if (telefone_aluno != null)
                {
                    telefoneAluno = telefone_aluno["dc_fone_mail"]?.ToString() ?? "";
                }

                var pessoa_aluno_fisica = await SQLServerService.GetFirstByFields(source, "T_PESSOA_FISICA", new List<(string campo, object valor)> { new("cd_pessoa_fisica", cd_pessoa_aluno) });
                var rg_pessoa_aluno = pessoa_aluno_fisica != null ? pessoa_aluno_fisica["nm_doc_identidade"]?.ToString() ?? "" : "";
                var cpfAluno = pessoa_aluno_fisica != null ? pessoa_aluno_fisica["nm_cpf"]?.ToString() ?? "" : "";

                var estadoCivilAluno = "";
                var estado_civil_aluno = await SQLServerService.GetFirstByFields(source, "T_ESTADO_CIVIL", new List<(string campo, object valor)> { new("cd_estado_civil", pessoa_aluno_fisica != null ? pessoa_aluno_fisica["cd_estado_civil"]?.ToString() ?? "" : "") });
                if (estado_civil_aluno != null)
                {
                    estadoCivilAluno = estado_civil_aluno["dc_estado_civil_masc"]?.ToString() ?? "";
                }
                var sexoAluno = pessoa_aluno_fisica["nm_sexo"];
                var sexoFAluno = sexoAluno != null && sexoAluno.ToString() == "2" ? "X" : "";
                var sexoMAluno = sexoAluno != null && sexoAluno.ToString() == "1" ? "X" : "";

                var dataNascimentoAluno = "";
                if (pessoa_aluno_fisica != null && pessoa_aluno_fisica["dt_nascimento"] != null)
                {
                    if (DateTime.TryParse(pessoa_aluno_fisica["dt_nascimento"].ToString(), out DateTime dt_nasc))
                    {
                        dataNascimentoAluno = dt_nasc.ToString("dd/MM/yyyy");
                    }
                }
                var endereco_aluno = await SQLServerService.GetFirstByFields(source, "T_ENDERECO", new List<(string campo, object valor)> { new("cd_pessoa", cd_aluno) });
                var enderecoaluno = "";
                if (endereco_aluno != null)
                {
                    var filtroLogradouro = new List<(string campo, object valor)> { new("cd_localidade", endereco_aluno["cd_loc_logradouro"].ToString()) };
                    var logradouroExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroLogradouro);
                    if (logradouroExists != null)
                    {
                        enderecoaluno = $"{logradouroExists["no_localidade"]},{endereco_aluno["dc_num_endereco"]}";
                    }
                }

                var celularAluno = "";
                var celular_aluno = await SQLServerService.GetFirstByFields(source, "T_TELEFONE", new List<(string campo, object valor)> { new("cd_pessoa", cd_aluno), new("cd_tipo_telefone", 3) });
                if (celular_aluno != null)
                {
                    celularAluno = celular_aluno["dc_fone_mail"]?.ToString() ?? "";
                }
                var emailAluno = "";
                var email_aluno = await SQLServerService.GetFirstByFields(source, "T_TELEFONE", new List<(string campo, object valor)> { new("cd_pessoa", cd_aluno), new("cd_tipo_telefone", 4) });
                if (email_aluno != null)
                {
                    emailAluno = email_aluno["dc_fone_mail"]?.ToString() ?? "";
                }
                #endregion

                #region CONTRATO
                var nomeCurso = "";
                var curso = await SQLServerService.GetFirstByFields(source, "T_CURSO", new List<(string campo, object valor)> { new("cd_curso", matriculaExists["cd_curso_atual"]) });
                if (curso != null)
                {
                    nomeCurso = curso["no_curso"]?.ToString() ?? "";
                }
                var duracaoAula = "";
                var duracao = await SQLServerService.GetFirstByFields(source, "T_DURACAO", new List<(string campo, object valor)> { new("cd_duracao", matriculaExists["cd_duracao_atual"]) });
                if (duracao != null)
                {
                    duracaoAula = $"{duracao["dc_duracao"]?.ToString() ?? ""}/aula";
                }

                var diasSemana = new Dictionary<int, string>
                {
                    {1, "domingo"},
                    {2, "segunda"},
                    {3, "terça"},
                    {4, "quarta"},
                    {5, "quinta"},
                    {6, "sexta"},
                    {7, "sábado"}
                };

                var diasMontado = "";
                var horarios = await SQLServerService.GetList("T_HORARIO", null, "[cd_registro]", $"[{aluno["cd_aluno"].ToString()}]", source, SearchModeEnum.Equals);
                if (horarios.data.Any())
                {
                    var diasList = horarios.data
                        .Select(h => diasSemana.TryGetValue(Convert.ToInt32(h["id_dia_semana"]), out var dia) ? dia : "")
                        .Where(d => !string.IsNullOrEmpty(d))
                        .Distinct();

                    diasMontado = string.Join(", ", diasList);
                }
                var horarioMontado = "";
                if (horarios.data.Any())
                {
                    var horariosList = horarios.data
                        .Select(h => $"{h["dt_hora_ini"]?.ToString() ?? ""} às {h["dt_hora_fim"]?.ToString() ?? ""}")
                        .Where(t => !string.IsNullOrEmpty(t))
                        .Distinct();
                    horarioMontado = string.Join(", ", horariosList);
                }

                var dataInicioAula = "";
                if (matriculaExists["dt_inicial_contrato"] != null)
                {
                    if (DateTime.TryParse(matriculaExists["dt_inicial_contrato"].ToString(), out DateTime dt_inicio))
                    {
                        dataInicioAula = dt_inicio.ToString("dd/MM/yyyy");
                    }
                }
                var dataFimAula = "";
                if (matriculaExists["dt_final_contrato"] != null)
                {
                    if (DateTime.TryParse(matriculaExists["dt_final_contrato"].ToString(), out DateTime dt_fim))
                    {
                        dataFimAula = dt_fim.ToString("dd/MM/yyyy");
                    }
                }

                var matriculaRematricula = "";
                if (matriculaExists["id_tipo_matricula"] != null)
                {
                    matriculaRematricula = matriculaExists["id_tipo_matricula"].ToString() == "1" ? "Matrícula" : "Rematrícula";
                }
                #endregion


                var replacements = new Dictionary<string, string>
            {
                { "«NomeEscola»", nomeEscola },
                { "«RazaoSocial»", razaoSocialEscola },
                { "«CNPJEscola»", cnpjEscola },
                { "«EnderecoEscola»", enderecoEscolaMontado },
                { "«CidadeEstadoEscola»", cidadeEstadoEscola },

                { "«NomeResponsavel»", nomeResponsavel },
                { "«RGResponsavel»", rg_pessoa_responsavel },
                { "«CPFCNPJResponsavel»", cpfResponsavel },
                { "«TelefoneResponsavel»", telefoneResponsavel },
                { "«EnderecoResponsavel»",enderecoResponsavel },

                { "«NomeAluno»", nomeAluno },
                { "«TelelfoneAluno»", telefoneAluno },
                { "«RGAluno»", rg_pessoa_aluno },
                { "«CPFAluno»", cpfAluno },
                { "«EstadoCivilAluno»", estadoCivilAluno },
                { "«SexoF»", sexoFAluno },
                { "«SexoM»", sexoMAluno },
                { "«DataNascimentoAluno»", dataNascimentoAluno },
                { "«EnderecoAluno»", enderecoaluno },
                { "«EmailAluno»", emailAluno },
                { "«CelularAluno»", celularAluno },

                { "«Curso»", nomeCurso },
                { "«ComplementoCursoComMinutosTurma»", duracaoAula },
                { "«Dias»", diasMontado },
                { "«HorariosCurso»", horarioMontado },
                { "«DataInicioAulas»", dataInicioAula},
                { "«DataFimTurma»", dataFimAula },
                { "«MatriculaRematricula»", matriculaRematricula },

                { "«ValorMaterial»", $"R$ {matriculaExists["vl_material_contrato"]}" },
                { "«ValorSemDesconto»", $"R$ {matriculaExists["vl_liquido_contrato"]}" },
                { "«NroVencimento»", matriculaExists["nm_dia_vcto"].ToString()??"" },
                { "«ValorComDesconto»", $"R$ {matriculaExists["vl_curso_contrato"]}" },
                { "«NroVencimentoComDesconto»",  matriculaExists["nm_dia_vcto"].ToString()??"" },
                { "«OpcoesPagamento»", "" },
                { "«NroParcelas»", $"{matriculaExists["nm_parcelas_mensalidade"]}" }
            };

                var (success, arquivo, erro) = GerarContrato(nomeContrato, replacements);
                if (!success) return BadRequest(erro);
                return File(arquivo, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"{nomeContrato}.docx");
            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }

        private (bool success, MemoryStream? arquivo, string? erro) GerarContrato(string nomeContrato, Dictionary<string, string> replacements)
        {
            // Monta o caminho do template
            string webRootPath = _webHostEnvironment.WebRootPath;
            string caminhoPasta = Path.Combine(webRootPath, "Contratos");
            var path = Path.Combine(caminhoPasta, $"{nomeContrato}");
            if (!System.IO.File.Exists(path))
                return (false, null, $"Contrato não encontrado: {path}");

            // Carrega o template (DOTX ou DOCX)
            using (var doc = DocX.Load(path))
            {
                // Faz os replaces
                foreach (var campo in replacements)
                {
                    doc.ReplaceText(campo.Key, campo.Value ?? string.Empty);
                }

                // Retorna como MemoryStream (sempre em DOCX)
                var memoryStream = new MemoryStream();
                doc.SaveAs(memoryStream);
                memoryStream.Position = 0;
                return (true, memoryStream, null);
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
            var cd_usuario = "1";
            if (tokenInfo.Count > 0) cd_pessoa_logada = tokenInfo["cd_pessoa"];

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
            var bolsas = await SQLServerService.GetList("vi_aluno_bolsa", null, "[cd_aluno],[cd_produto]", $"[{cd_aluno}],[{cd_produto}]", source, SearchModeEnum.Equals);
            if (!bolsas.data.Any()) return (true, null);
            var bolsa_aluno = bolsas.data.First();
            if (bolsa_aluno["pc_bolsa"] == null && bolsa_aluno["pc_bolsa_material"] == null) return (true, "porcentagem de bolsa não configuradas");
            var pc_bolsa = bolsa_aluno["pc_bolsa"] != null ? Convert.ToDecimal(bolsa_aluno["pc_bolsa"]) : 0;
            var pc_bolsa_material = bolsa_aluno["pc_bolsa_material"] != null ? Convert.ToDecimal(bolsa_aluno["pc_bolsa_material"]) : 0;
            //aplicar mesma logica de baixa automatica do conta receber
            //pegar titulos do contrato de mensalidade ou material
            var titulos = new List<Dictionary<string, object>>();
            decimal vl_total_baixa = 0;
            decimal vl_desconto_bolsa = 0;
            decimal vl_desconto_bolsa_material = 0;
            if (pc_bolsa != 0)
            {
                var get_titulos_mensalidade = await SQLServerService.GetList("T_TITULO", null, "[cd_origem_titulo],[id_origem_titulo],[dc_tipo_titulo]", $"[{cd_contrato}],[22],[ME]", source, SearchModeEnum.Equals);
                titulos.AddRange(get_titulos_mensalidade.data);

                var nm_parcelas_mensalidade = contratoExists["nm_parcelas_mensalidade"] != null ? Convert.ToInt32(contratoExists["nm_parcelas_mensalidade"]) : 0;
                var vl_parcela_mensalidade = contratoExists["vl_parcela_contrato"] != null ? Convert.ToDecimal(contratoExists["vl_parcela_contrato"]) : 0;

                var vl_desconto = (vl_parcela_mensalidade * pc_bolsa) / 100;
                vl_total_baixa += vl_desconto * nm_parcelas_mensalidade;
                vl_desconto_bolsa = vl_desconto;

            }
            if (pc_bolsa_material != 0)
            {
                var get_titulos_mensalidade = await SQLServerService.GetList("T_TITULO", null, "[cd_origem_titulo],[id_origem_titulo],[dc_tipo_titulo]", $"[{cd_contrato}],[22],[MT]", source, SearchModeEnum.Equals);
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
                            { "vl_baixa_saldo_titulo", 0 },
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
    }
}