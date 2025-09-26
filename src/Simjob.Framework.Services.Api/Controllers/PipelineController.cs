using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
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
using Simjob.Framework.Services.Api.Models;
using Simjob.Framework.Services.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simjob.Framework.Services.Api.Controllers
{
    public class PipelineController : BaseController
    {
        private readonly IRepository<SourceContext, Source> _sourceRepository;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;

        public PipelineController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, IRepository<SourceContext, Source> sourceRepository, IRepository<MongoDbContext, Schema> schemaRepository) : base(bus, notifications)
        {
            _sourceRepository = sourceRepository;
            _schemaRepository = schemaRepository;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetPipeline(string value, SearchModeEnum mode, int? page, int? limit, string sortField, bool sortDesc = false, string ids = "", string searchFields = null, string? cd_empresa = null, DateTime? dataInicio = null, DateTime? dataFim = null, DateTime? dataInicioReprogramar = null, DateTime? dataFimReprogramar = null, bool diaAtual = false, int? cd_pessoa = null)
        {
            if (cd_empresa == null) return BadRequest("campo cd_empresa não informado");
            var schemaName = "T_Pessoa";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                string? campoDiaAtual = null;
                if (diaAtual) campoDiaAtual = "dt_inicial";

                if (cd_pessoa != null && cd_pessoa > 0)
                {
                    var filtrosUsuario = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa) };
                    var sys_usuario = await SQLServerService.GetFirstByFields(source, "T_SYS_USUARIO", filtrosUsuario);
                    if (sys_usuario != null)
                    {
                        if (string.IsNullOrEmpty(searchFields) && string.IsNullOrEmpty(value))
                        {
                            searchFields = "[cd_usuario]";
                            value = $"[{sys_usuario["cd_usuario"].ToString()}]";
                        }
                        else
                        {
                            searchFields = $"{searchFields},[cd_usuario]";
                            value = $"{value},[{sys_usuario["cd_usuario"].ToString()}]";
                        }
                    }
                }

                DateTime? datainicio = dataInicio != null ? dataInicio.Value.Date : null;
                DateTime? datafim = dataFim != null ? dataFim.Value.Date : null;
                DateTime? datainicioRepro = dataInicioReprogramar != null ? dataInicioReprogramar.Value.Date : null;
                DateTime? datafimRepro = dataFimReprogramar != null ? dataFimReprogramar.Value.Date : null;
                var pipelineResult = await SQLServerService.GetListFiltroData("vi_pipeline", page, limit, sortField, sortDesc, ids, "cd_pipeline", searchFields, value, source, mode, "cd_pessoa_escola_pipeline", cd_empresa, "dt_inicial", "dt_inicial", datainicio, datafim, "dt_reprogramar", datainicioRepro, datafimRepro, campoDiaAtual);
                if (!pipelineResult.success) return BadRequest(pipelineResult.error);
                var pipelines = pipelineResult.data;

                var pessoas_pipeline = pipelines.Select(x => x["cd_pessoa_pipeline"]);

                var dependentes_query = await SQLServerService.GetList("vi_relacionamento", string.Join(",", pessoas_pipeline), "cd_pessoa_pai", null, source, SearchModeEnum.Contains);
                var dependentes = dependentes_query.data;
                var retorno = new
                {
                    data = pipelines.Select(x =>
                    {
                        var dependentes_pessoa = dependentes.Where(y => y["cd_pessoa_pai"].ToString() == x["cd_pessoa_pipeline"].ToString());

                        return new
                        {
                            cd_pipeline = x.ContainsKey("cd_pipeline") ? x["cd_pipeline"] : null,
                            cd_etapa_pipeline = x.ContainsKey("cd_etapa_pipeline") ? x["cd_etapa_pipeline"] : null,
                            no_etapa_pipeline = x.ContainsKey("no_etapa_pipeline") ? x["no_etapa_pipeline"] : null,
                            cd_contato_pipeline = x.ContainsKey("cd_contato_pipeline") ? x["cd_contato_pipeline"] : null,
                            cd_pessoa_contato = x.ContainsKey("cd_pessoa_contato") ? x["cd_pessoa_contato"] : null,
                            no_pessoa_contato = x["no_pessoa_contato"],
                            cd_pessoa_pipeline = x.ContainsKey("cd_pessoa_pipeline") ? x["cd_pessoa_pipeline"] : null,
                            no_pessoa_pipeline = x.ContainsKey("no_pessoa_pipeline") ? x["no_pessoa_pipeline"] : null,
                            cd_pessoa_escola_pipeline = x.ContainsKey("cd_pessoa_escola_pipeline") ? x["cd_pessoa_escola_pipeline"] : null,
                            cd_acao = x.ContainsKey("cd_acao") ? x["cd_acao"] : null,
                            no_acao = x.ContainsKey("no_acao") ? x["no_acao"] : null,
                            cd_atividade_pipeline = x.ContainsKey("cd_atividade_pipeline") ? x["cd_atividade_pipeline"] : null,
                            no_atividade_pipeline = x.ContainsKey("no_atividade_pipeline") ? x["no_atividade_pipeline"] : null,
                            cd_usuario = x.ContainsKey("cd_usuario") ? x["cd_usuario"] : null,
                            no_usuario = x.ContainsKey("no_usuario") ? x["no_usuario"] : null,
                            cd_etapa_posterior = x.ContainsKey("cd_etapa_posterior") ? x["cd_etapa_posterior"] : null,
                            no_etapa_posterior = x["no_etapa_posterior"],
                            cd_motivo_perda = x.ContainsKey("cd_motivo_perda") ? x["cd_motivo_perda"] : null,
                            no_motivo_perda = x["no_motivo_perda"],
                            cd_produto_pipeline = x.ContainsKey("cd_produto_pipeline") ? x["cd_produto_pipeline"] : null,
                            no_produto_pipeline = x["no_produto"],
                            cd_curso_pipeline = x.ContainsKey("cd_curso_pipeline") ? x["cd_curso_pipeline"] : null,
                            no_curso_pipeline = x["no_curso_pipeline"],
                            id_posicao_pipeline = x.ContainsKey("id_posicao_pipeline") ? x["id_posicao_pipeline"] : null,
                            dt_inicial = x.ContainsKey("dt_inicial") ? x["dt_inicial"] : null,
                            dt_reprogramar = x.ContainsKey("dt_reprogramar") ? x["dt_reprogramar"] : null,
                            dt_realizada = x.ContainsKey("dt_realizada") ? x["dt_realizada"] : null,
                            tx_obs_pipeline = x.ContainsKey("tx_obs_pipeline") ? x["tx_obs_pipeline"] : null,
                            nm_resultado_teste = x.ContainsKey("nm_resultado_teste") ? x["nm_resultado_teste"] : null,
                            id_pipeline_usuario = x.ContainsKey("id_pipeline_usuario") ? x["id_pipeline_usuario"] : null,
                            dt_interacao = x.ContainsKey("dt_interacao") ? x["dt_interacao"] : null,
                            Email = x["email"],
                            Telefone = x["telefone"],
                            Celular = x["celular"],
                            id_posicao_contato = x["id_posicao_contato"],
                            Temperatura = x["temperatura"],
                            cd_pessoa_bloqueio = x["cd_pessoa_bloqueio"],
                            Dependentes = dependentes_pessoa,
                            nm_cpf = x["nm_cpf"],
                            cd_escolaridade = x["cd_escolaridade"],
                            no_escolaridade = x["no_escolaridade"],
                            dt_nascimento = x["dt_nascimento"],
                            possui_atendente = x["possui_atendente"]
                        };
                    }),
                    pipelineResult.total,
                    page,
                    limit,
                    pages = limit != null ? (int)Math.Ceiling((double)pipelineResult.total / limit.Value) : 0
                };

                return ResponseDefault(retorno);
            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostPipeline([FromBody] InsertPipelineModel model)
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
                //validar contato
                var filtrosContato = new List<(string campo, object valor)> { new("cd_contato", model.cd_contato) };
                var contato = await SQLServerService.GetFirstByFields(source, "T_CONTATO", filtrosContato);
                if (contato == null) return NotFound("contato não encontrado");
                var cd_contato = contato["cd_contato"];

                //validação de token
                var cd_pessoa = "";
                var cd_usuario = "1";
                if (tokenInfo.Count > 0) cd_pessoa = tokenInfo["cd_pessoa"];
                if (model.cd_usuario != null) cd_pessoa = model.cd_usuario.ToString();

                if (string.IsNullOrEmpty(cd_pessoa)) return BadRequest("cd_pessoa não configurado");

                var filtrosUsuario = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa) };
                var sys_usuario = await SQLServerService.GetFirstByFields(source, "T_SYS_USUARIO", filtrosUsuario);
                if (sys_usuario != null) cd_usuario = sys_usuario["cd_usuario"].ToString() ?? "1";

                int? cd_curso_pipeline = null;
                int? cd_produto_pipeline = null;
                if (!string.IsNullOrEmpty(model.Curso))
                {
                    var produtoResult = await SQLServerService.GetList("T_PRODUTO", 1, 1, "cd_produto", true, null, "[no_produto]", $"[{model.Curso}]", source, SearchModeEnum.Contains, null, null);
                    if (produtoResult.success && produtoResult.data.Count() > 0)
                    {
                        var produto = produtoResult.data.First();
                        cd_produto_pipeline = int.Parse(produto["cd_produto"].ToString());
                    }
                }
                //gerar T_pipeline
                var t_pipeline_dict = new Dictionary<string, object>
                {
                    { "cd_etapa_pipeline",1 },
                    { "cd_contato_pipeline", model.cd_contato },
                    { "cd_pessoa_pipeline", contato["cd_pessoa_contato"] },
                    { "cd_pessoa_escola_pipeline",model.cd_empresa },
                    { "id_posicao_pipeline", contato["id_posicao_contato"]},
                    { "cd_acao", contato["cd_acao"] },
                    { "cd_atividade_pipeline", 1 },
                    { "cd_usuario", cd_usuario }, //T_SYS_USUARIO
                    { "dt_inicial", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") },
                    { "dt_reprogramar", null },
                    { "cd_etapa_posterior", null },
                    { "dt_realizada", null },
                    { "cd_motivo_perda", null },
                    { "tx_obs_pipeline", null },
                    { "cd_produto_pipeline", cd_produto_pipeline },
                    { "cd_curso_pipeline",cd_curso_pipeline },
                    { "nm_resultado_teste", model.nm_resultado_teste },
                    { "id_pipeline_usuario", 0 },
                    { "dt_interacao", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") }
                };

                if (model.cd_usuario != null)
                {
                    t_pipeline_dict.Add("possui_atendente", 1);
                }

                var t_pipeline_insert = await SQLServerService.Insert("T_PIPELINE", t_pipeline_dict, source);
                if (!t_pipeline_insert.success) return BadRequest(t_pipeline_insert.error);

                if (model.manual == null || false)
                {
                    contato["id_posicao_contato"] = 2;
                    contato.Remove("cd_contato");

                    var t_contato_update = await SQLServerService.Update("T_CONTATO", contato, source, "cd_contato", cd_contato);
                    if (!t_contato_update.success) return BadRequest(t_contato_update.error);
                }

                return ResponseDefault(t_pipeline_dict);
            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> PutPipeline(UpdatePipelineModel model)
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
                //validar pipeline
                var filtrosPipeline = new List<(string campo, object valor)> { new("cd_pipeline", model.cd_pipeline) };
                var pipeline = await SQLServerService.GetFirstByFields(source, "T_PIPELINE", filtrosPipeline);
                if (pipeline == null) return NotFound("pipeline não encontrada");

                //validação de token
                var cd_pessoa_logada = "";
                var cd_usuario = "1";
                if (tokenInfo.Count > 0) cd_pessoa_logada = tokenInfo["cd_pessoa"];

                if (string.IsNullOrEmpty(cd_pessoa_logada)) return BadRequest("cd_pessoa não configurado");

                var filtrosUsuario = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa_logada) };
                var sys_usuario = await SQLServerService.GetFirstByFields(source, "T_SYS_USUARIO", filtrosUsuario);
                if (sys_usuario != null) cd_usuario = sys_usuario["cd_usuario"].ToString() ?? "1";

                var cd_contato_pipeline = pipeline["cd_contato_pipeline"];
                var cd_pessoa = pipeline["cd_pessoa_pipeline"];
                var cd_pessoa_escola = pipeline["cd_pessoa_escola_pipeline"];
                //montar objeto pipeline
                var pipeline_dict = new Dictionary<string, object>
                {
                    { "cd_usuario", cd_usuario }
                };
                var cd_acao = pipeline["cd_acao"];
                if (model.Pessoa != null && model.Pessoa.cd_acao != null)
                {
                    cd_acao = model.Pessoa.cd_acao;
                    pipeline_dict.Add("cd_acao", cd_acao);
                }
                //atualizar infos pipeline
                if (model.cd_curso_pipeline != null) pipeline_dict.Add("cd_curso_pipeline", model.cd_curso_pipeline);
                if (model.cd_produto_pipeline != null) pipeline_dict.Add("cd_produto_pipeline", model.cd_produto_pipeline);
                if (model.nm_resultado_teste != null) pipeline_dict.Add("nm_resultado_teste", model.nm_resultado_teste);
                if (model.cd_etapa_pipeline != null) pipeline_dict.Add("cd_etapa_pipeline", model.cd_etapa_pipeline);
                if (model.cd_motivo_perda != null) pipeline_dict.Add("cd_motivo_perda", model.cd_motivo_perda);
                if (model.Reprogramar == null)
                {
                    pipeline_dict.Add("dt_realizada", DateTime.Now.Date.ToString("yyyy-MM-ddTHH:mm:ss"));
                    pipeline_dict.Add("dt_reprogramar", null);
                }
                if (model.Reprogramar != null)
                {
                    pipeline_dict.Add("dt_reprogramar", model.Reprogramar?.ToString("yyyy-MM-ddTHH:mm:ss"));
                    pipeline_dict.Add("dt_realizada", null);
                }
                pipeline_dict.Add("tx_obs_pipeline", model.txt_observacao_pipeline);
                pipeline_dict.Add("temperatura", model.Temperatura);

                pipeline_dict.Add("possui_atendente", 1);
                //aproveitamento?

                var t_pipeline_update = await SQLServerService.Update("T_PIPELINE", pipeline_dict, source, "cd_pipeline", model.cd_pipeline);
                if (!t_pipeline_update.success) return BadRequest(t_pipeline_update.error);
                //PIPELINE DIA

                if (!model.id_dia_semana.IsNullOrEmpty())
                {
                    //remove dia semana pipeline
                    await SQLServerService.Delete("T_PIPELINE_DIA", "cd_pipeline", model.cd_pipeline.ToString(), source);
                    foreach (var dia_semana in model.id_dia_semana)
                    {
                        var dia_pipeline_dict = new Dictionary<string, object>
                        {
                            { "cd_pipeline", model.cd_pipeline },
                            { "id_dia_semana", dia_semana}
                        };
                        var t_pipeline_dia_update = await SQLServerService.Insert("T_PIPELINE_DIA", dia_pipeline_dict, source);
                        if (!t_pipeline_dia_update.success) return BadRequest(t_pipeline_dia_update.error);
                    }
                }
                //PIPELINE Periodo

                if (!model.id_periodo.IsNullOrEmpty())
                {
                    //remover id_periodo
                    await SQLServerService.Delete("T_PIPELINE_PERIODO", "cd_pipeline", model.cd_pipeline.ToString(), source);
                    foreach (var periodo in model.id_periodo)
                    {
                        var periodo_pipeline_dict = new Dictionary<string, object>
                        {
                            { "cd_pipeline", model.cd_pipeline },
                            { "id_periodo", periodo }
                        };
                        var t_pipeline_periodo_insert = await SQLServerService.Insert("T_PIPELINE_PERIODO", periodo_pipeline_dict, source);
                        if (!t_pipeline_periodo_insert.success) return BadRequest(t_pipeline_periodo_insert.error);
                    }
                }
                //atualizar dados pessoa
                if (model.Pessoa != null)
                {
                    //T_PESSOA
                    var pessoaDict = new Dictionary<string, object>
                    {
                        { "no_pessoa", model.Pessoa.no_pessoa }
                    };

                    var t_pessoa_insert = await SQLServerService.Update("T_PESSOA", pessoaDict, source, "cd_pessoa", cd_pessoa);
                    if (!t_pessoa_insert.success) return BadRequest(t_pessoa_insert.error);

                    if (!string.IsNullOrEmpty(model.Pessoa.dc_email))
                    {
                        var filtrosEmail = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa), new("cd_tipo_telefone", 4) };
                        var emailExists = await SQLServerService.GetFirstByFields(source, "T_Telefone", filtrosEmail);

                        var telefoneDictEmail = new Dictionary<string, object>
                            {
                                { "cd_pessoa", cd_pessoa },
                                { "cd_tipo_telefone", 4 },
                                { "cd_classe_telefone", 1 },
                                { "dc_fone_mail", model.Pessoa.dc_email },
                                { "cd_endereco", null },
                                { "id_telefone_principal",1 },
                                { "cd_operadora", null }
                            };
                        if (emailExists != null)
                        {
                            var cd_telefone = emailExists["cd_telefone"];
                            var t_telefone_email_insert = await SQLServerService.Update("T_TELEFONE", telefoneDictEmail, source, "cd_telefone", cd_telefone);
                            if (!t_telefone_email_insert.success) return BadRequest(t_telefone_email_insert.error);
                        }
                        else
                        {
                            var t_telefone_email_insert = await SQLServerService.Insert("T_TELEFONE", telefoneDictEmail, source);
                            if (!t_telefone_email_insert.success) return BadRequest(t_telefone_email_insert.error);
                        }
                    }
                    if (!string.IsNullOrEmpty(model.Pessoa.telefone))
                    {
                        var filtrosTelefone = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa), new("cd_tipo_telefone", 1) };
                        var telefoneExists = await SQLServerService.GetFirstByFields(source, "T_Telefone", filtrosTelefone);
                        var telefoneDictTelefone = new Dictionary<string, object>
                        {
                            { "cd_pessoa", cd_pessoa },
                            { "cd_tipo_telefone", 1 },
                            { "cd_classe_telefone", 1 },
                            { "dc_fone_mail", model.Pessoa.telefone },
                            { "cd_endereco", null },
                            { "id_telefone_principal", 1 },
                            { "cd_operadora", null }
                        };
                        if (telefoneExists != null)
                        {
                            var cd_telefone = telefoneExists["cd_telefone"];
                            var t_telefone_telefone_insert = await SQLServerService.Update("T_TELEFONE", telefoneDictTelefone, source, "cd_telefone", cd_telefone);
                            if (!t_telefone_telefone_insert.success) return BadRequest(t_telefone_telefone_insert.error);
                        }
                        else
                        {
                            var t_telefone_telefone_insert = await SQLServerService.Insert("T_TELEFONE", telefoneDictTelefone, source);
                            if (!t_telefone_telefone_insert.success) return BadRequest(t_telefone_telefone_insert.error);
                        }
                    }
                    if (!string.IsNullOrEmpty(model.Pessoa.celular))
                    {
                        var filtrosCelular = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa), new("cd_tipo_telefone", 3) };
                        var celularexists = await SQLServerService.GetFirstByFields(source, "T_Telefone", filtrosCelular);
                        var telefoneDictTelefone = new Dictionary<string, object>
                        {
                            { "cd_pessoa", cd_pessoa },
                            { "cd_tipo_telefone", 3 },
                            { "cd_classe_telefone", 1 },
                            { "dc_fone_mail", model.Pessoa.celular},
                            { "cd_endereco", null },
                            { "id_telefone_principal", 1 },
                            { "cd_operadora", null }
                        };
                        if (celularexists != null)
                        {
                            var cd_telefone = celularexists["cd_telefone"];
                            var t_telefone_celular_insert = await SQLServerService.Update("T_TELEFONE", telefoneDictTelefone, source, "cd_telefone", cd_telefone);
                            if (!t_telefone_celular_insert.success) return BadRequest(t_telefone_celular_insert.error);
                        }
                        else
                        {
                            var t_telefone_celular_insert = await SQLServerService.Insert("T_TELEFONE", telefoneDictTelefone, source);
                            if (!t_telefone_celular_insert.success) return BadRequest(t_telefone_celular_insert.error);
                        }
                    }
                    if (model.Pessoa.EnderecoPessoaPipeLine != null)
                    {
                        //T_ENDERECO
                        var filtrosEndereco = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa) };
                        var enderecoExists = await SQLServerService.GetFirstByFields(source, "T_ENDERECO", filtrosEndereco);
                        if (enderecoExists != null)
                        {
                            //T_ENDERECO
                            var endereco = model.Pessoa.EnderecoPessoaPipeLine;
                            var enderecoDict = new Dictionary<string, object>
                            {
                                { "cd_pessoa", cd_pessoa }
                            };

                            if (endereco.cd_endereco.HasValue)
                                enderecoDict.Add("cd_endereco", endereco.cd_endereco.Value);
                            if (endereco.cd_loc_cidade.HasValue)
                                enderecoDict.Add("cd_loc_cidade", endereco.cd_loc_cidade.Value);
                            if (endereco.cd_loc_estado.HasValue)
                                enderecoDict.Add("cd_loc_estado", endereco.cd_loc_estado.Value);
                            if (endereco.cd_loc_pais.HasValue)
                                enderecoDict.Add("cd_loc_pais", endereco.cd_loc_pais.Value);
                            if (endereco.cd_tipo_endereco.HasValue)
                                enderecoDict.Add("cd_tipo_endereco", endereco.cd_tipo_endereco.Value);
                            if (endereco.cd_tipo_logradouro.HasValue)
                                enderecoDict.Add("cd_tipo_logradouro", endereco.cd_tipo_logradouro.Value);
                            if (endereco.cd_loc_bairro.HasValue)
                                enderecoDict.Add("cd_loc_bairro", endereco.cd_loc_bairro.Value);
                            if (endereco.cd_loc_logradouro.HasValue)
                                enderecoDict.Add("cd_loc_logradouro", endereco.cd_loc_logradouro.Value);
                            if (!string.IsNullOrWhiteSpace(endereco.dc_compl_endereco))
                                enderecoDict.Add("dc_compl_endereco", endereco.dc_compl_endereco);
                            if (!string.IsNullOrWhiteSpace(endereco.dc_num_cep))
                                enderecoDict.Add("dc_num_cep", endereco.dc_num_cep);
                            if (!string.IsNullOrWhiteSpace(endereco.dc_num_endereco))
                                enderecoDict.Add("dc_num_endereco", endereco.dc_num_endereco);

                            var t_endereco_insert = await SQLServerService.Update("T_ENDERECO", enderecoDict, source, "cd_endereco", enderecoExists["cd_endereco"]);
                            if (!t_endereco_insert.success) return BadRequest(t_endereco_insert.error);
                        }
                        else
                        {
                            //T_ENDERECO
                            var endereco = model.Pessoa.EnderecoPessoaPipeLine;
                            var enderecoDict = new Dictionary<string, object>
                            {
                                { "cd_pessoa", cd_pessoa }
                            };

                            if (endereco.cd_endereco.HasValue)
                                enderecoDict.Add("cd_endereco", endereco.cd_endereco.Value);
                            if (endereco.cd_loc_cidade.HasValue)
                                enderecoDict.Add("cd_loc_cidade", endereco.cd_loc_cidade.Value);
                            if (endereco.cd_loc_estado.HasValue)
                                enderecoDict.Add("cd_loc_estado", endereco.cd_loc_estado.Value);
                            if (endereco.cd_loc_pais.HasValue) enderecoDict.Add("cd_loc_pais", endereco.cd_loc_pais.Value);
                            else enderecoDict.Add("cd_loc_pais", 1);
                            if (endereco.cd_tipo_endereco.HasValue)
                                enderecoDict.Add("cd_tipo_endereco", endereco.cd_tipo_endereco.Value);
                            if (endereco.cd_tipo_logradouro.HasValue)
                                enderecoDict.Add("cd_tipo_logradouro", endereco.cd_tipo_logradouro.Value);
                            if (endereco.cd_loc_bairro.HasValue)
                                enderecoDict.Add("cd_loc_bairro", endereco.cd_loc_bairro.Value);
                            if (endereco.cd_loc_logradouro.HasValue)
                                enderecoDict.Add("cd_loc_logradouro", endereco.cd_loc_logradouro.Value);
                            if (!string.IsNullOrWhiteSpace(endereco.dc_compl_endereco))
                                enderecoDict.Add("dc_compl_endereco", endereco.dc_compl_endereco);
                            if (!string.IsNullOrWhiteSpace(endereco.dc_num_cep))
                                enderecoDict.Add("dc_num_cep", endereco.dc_num_cep);
                            if (!string.IsNullOrWhiteSpace(endereco.dc_num_endereco))
                                enderecoDict.Add("dc_num_endereco", endereco.dc_num_endereco);

                            var t_endereco_insert = await SQLServerService.Insert("T_ENDERECO", enderecoDict, source);
                            if (!t_endereco_insert.success) return BadRequest(t_endereco_insert.error);
                        }
                    }
                    if (cd_contato_pipeline != null)
                    {
                        var contato_dict = new Dictionary<string, object>();
                        if (model.Pessoa.cd_acao != null) contato_dict.Add("cd_acao", model.Pessoa.cd_acao);
                        if (model.Pessoa.id_posicao_contato != null) contato_dict.Add("id_posicao_contato", model.Pessoa.id_posicao_contato);
                        if (contato_dict.Any())
                        {
                            var contato_update = await SQLServerService.Update("T_CONTATO", contato_dict, source, "cd_contato", cd_contato_pipeline);
                            if (!contato_update.success) return BadRequest(contato_update.error);
                        }
                    }

                    var filtrosPessoaFisica = new List<(string campo, object valor)> { new("cd_pessoa_fisica", cd_pessoa) };
                    var pessoaFisicaExists = await SQLServerService.GetFirstByFields(source, "T_PESSOA_FISICA", filtrosPessoaFisica);
                    if (pessoaFisicaExists != null)
                    {
                        //T_PESSOA_FISICA
                        var pessoa_fisicaDict = new Dictionary<string, object>();

                        if (model.Pessoa.nm_cpf != null) pessoa_fisicaDict.Add("nm_cpf", model.Pessoa.nm_cpf);
                        if (model.Pessoa.nm_sexo != null) pessoa_fisicaDict.Add("nm_sexo", model.Pessoa.nm_sexo);
                        if (model.Pessoa.cd_escolaridade != null) pessoa_fisicaDict.Add("cd_escolaridade", model.Pessoa.cd_escolaridade);

                        var t_pessoa_fisica_insert = await SQLServerService.Update("T_PESSOA_FISICA", pessoa_fisicaDict, source, "cd_pessoa_fisica", cd_pessoa);
                        if (!t_pessoa_fisica_insert.success) return BadRequest(t_pessoa_fisica_insert.error);
                    }
                }

                //criar registro de historico de pessoa

                var ultimoHistorico = await SQLServerService.GetList("T_HISTORICO_PESSOA", 1, 1, "nm_sequencia_historico", true, null, "[cd_contato]", $"[{cd_contato_pipeline}]", source, SearchModeEnum.Equals, null, null);

                //var sequencia = 0;
                //if (ultimoHistorico.success)
                //{
                //    sequencia = int.Parse(ultimoHistorico.data.FirstOrDefault()?["nm_sequencia_historico"]?.ToString()?? "1");
                //}
                //sequencia += 1;
                //var historico = new Dictionary<string, object>
                //{
                //    { "cd_contato",cd_contato_pipeline},
                //    { "cd_pessoa_escola", cd_pessoa_escola },
                //    { "cd_pessoa_historico", cd_pessoa },
                //    { "dt_historico_pessoa", DateTime.Now },
                //    { "dt_cadastro_historico", DateTime.Now },
                //    { "cd_usuario", 1 },
                //    { "cd_acao",  cd_acao},
                //    { "id_posicao_historico",  1},
                //    { "nm_sequencia_historico", sequencia },
                //    { "id_status_historico",  model.cd_etapa_pipeline},
                //    { "nm_resultado_teste",model.Resultado},
                //    { "cd_pipeline",model.cd_pipeline},
                //    { "tx_obs_historico",model.txt_observacao_pipeline}
                //};
                //var t_historio = await SQLServerService.Insert("T_HISTORICO_PESSOA", historico, source);
                //if (!t_historio.success) return BadRequest(t_historio.error);

                //validar cd_etapa_pipeline
                //4 - validar se pessoa ja não esta na fila de matricula, e inserir um registro na fila de matricula
                if (model.cd_etapa_pipeline == 4)
                {
                    //validar se pessoa ja não esta na fila de matricula
                    var filtrosFilaMatriculaPendente = new List<(string campo, object valor)> { new("cd_pessoa_fila", cd_pessoa), new("id_status_fila", 1) };
                    var filaMatriculaPendente = await SQLServerService.GetFirstByFields(source, "T_FILA_MATRICULA", filtrosFilaMatriculaPendente);

                    var filtrosFilaMatriculaMatriculado = new List<(string campo, object valor)> { new("cd_pessoa_fila", cd_pessoa), new("id_status_fila", 3) };
                    var filaMatriculaMatriculado = await SQLServerService.GetFirstByFields(source, "T_FILA_MATRICULA", filtrosFilaMatriculaMatriculado);

                    var filtrosFilaMatriculaContato = new List<(string campo, object valor)> { new("cd_pessoa_fila", cd_pessoa), new("cd_contato", cd_contato_pipeline) };
                    var filaMatriculaContato = await SQLServerService.GetFirstByFields(source, "T_FILA_MATRICULA", filtrosFilaMatriculaContato);

                    if (filaMatriculaMatriculado == null && filaMatriculaPendente == null && filaMatriculaContato == null)
                    {
                        //cria registro em fila matricula
                        var fila_matricula_dict = new Dictionary<string, object>
                        {
                            { "cd_pessoa_escola", cd_pessoa_escola },
                            { "cd_pessoa_fila", cd_pessoa },
                            { "id_status_fila", 1 },
                            { "dt_programada_contato", model.Reprogramar?.ToString("yyyy-MM-ddTHH:mm:ss") ?? model.DataRealizada.ToString("yyyy-MM-ddTHH:mm:ss") },
                            { "cd_acao", cd_acao },
                            { "cd_contato",cd_contato_pipeline }
                        };
                        var fila_matricula_insert = await SQLServerService.InsertWithResult("T_FILA_MATRICULA", fila_matricula_dict, source);
                    }
                    return ResponseDefault();
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
        /// <param name="id"> cd_pipeline</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetPipelineId(int id)
        {
            var schemaName = "T_Pessoa";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                var retorno = new Dictionary<string, object>();

                //pipeLine
                //validar pipeline
                var filtrosPipeline = new List<(string campo, object valor)> { new("cd_pipeline", id) };
                var pipeline = await SQLServerService.GetFirstByFields(source, "T_PIPELINE", filtrosPipeline);
                if (pipeline == null) return NotFound("pipeline não encontrada");
                var cd_contato_pipeline = pipeline["cd_contato_pipeline"];
                var cd_pessoa = pipeline["cd_pessoa_pipeline"];
                var cd_pessoa_escola = pipeline["cd_pessoa_escola_pipeline"];

                retorno.Add("cd_pipeline", id);
                retorno.Add("nm_resultado_teste", pipeline["nm_resultado_teste"]);
                retorno.Add("cd_etapa_pipeline", pipeline["cd_etapa_pipeline"]);
                retorno.Add("cd_motivo_perda", pipeline["cd_motivo_perda"]);
                retorno.Add("dataRealizada", pipeline["dt_realizada"]);
                retorno.Add("reprogramar", pipeline["dt_reprogramar"]);
                retorno.Add("aproveitamento", null);
                retorno.Add("temperatura", pipeline["temperatura"]);
                retorno.Add("cd_pessoa_bloqueio", pipeline["cd_pessoa_bloqueio"]);
                retorno.Add("possui_atendente", pipeline["possui_atendente"]);
                //dia e periodo
                var filtrosPipelineDia = new List<(string campo, object valor)> { new("cd_pipeline", id) };
                var pipelineDia = await SQLServerService.GetList("T_PIPELINE_DIA", null, "[cd_pipeline]", $"[{id}]", source, SearchModeEnum.Equals);
                if (pipelineDia.success && pipelineDia.data.Any()) retorno.Add("id_dia_semana", pipelineDia.data.Select(x => x["id_dia_semana"]));
                var pipelinePeriodo = await SQLServerService.GetList("T_PIPELINE_PERIODO", null, "[cd_pipeline]", $"[{id}]", source, SearchModeEnum.Equals);
                if (pipelinePeriodo.success && pipelinePeriodo.data.Any()) retorno.Add("id_periodo", pipelinePeriodo.data.Select(x => x["id_periodo"]));

                var pessoa_dict = new Dictionary<string, object>
                {
                    { "cd_acao", pipeline["cd_acao"] }
                };
                //Contato
                var filtrosContato = new List<(string campo, object valor)> { new("cd_contato", cd_contato_pipeline) };
                var contatoExists = await SQLServerService.GetFirstByFields(source, "T_CONTATO", filtrosContato);
                if (contatoExists != null)
                {
                    pessoa_dict.Add("id_posicao_contato", contatoExists["id_posicao_contato"]);
                }

                //Pessoa
                var filtrosPessoa = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa) };
                var pessoaExists = await SQLServerService.GetFirstByFields(source, "T_Pessoa", filtrosPessoa);
                if (pessoaExists != null)
                {
                    pessoa_dict.Add("no_pessoa", pessoaExists["no_pessoa"]);

                    var filtrosEmail = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa), new("cd_tipo_telefone", 4) };
                    var emailExists = await SQLServerService.GetFirstByFields(source, "T_Telefone", filtrosEmail);
                    if (emailExists != null) pessoa_dict.Add("dc_email", emailExists["dc_fone_mail"]);

                    var filtrosTelefone = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa), new("cd_tipo_telefone", 1) };
                    var telefoneExists = await SQLServerService.GetFirstByFields(source, "T_Telefone", filtrosTelefone);
                    if (telefoneExists != null) pessoa_dict.Add("telefone", telefoneExists["dc_fone_mail"]);

                    var filtrosPessoaFisica = new List<(string campo, object valor)> { new("cd_pessoa_fisica", cd_pessoa) };
                    var pessoaFisicaExists = await SQLServerService.GetFirstByFields(source, "T_PESSOA_FISICA", filtrosPessoaFisica);
                    if (pessoaFisicaExists != null)
                    {
                        pessoa_dict.Add("nm_cpf", pessoaFisicaExists["nm_cpf"]);
                        pessoa_dict.Add("nm_sexo", pessoaFisicaExists["nm_sexo"]);
                        pessoa_dict.Add("cd_escolaridade", pessoaFisicaExists["cd_escolaridade"]);
                    }
                    //endereço
                    var filtrosEndereco = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa) };
                    var enderecoExists = await SQLServerService.GetFirstByFields(source, "T_ENDERECO", filtrosEndereco);
                    if (enderecoExists != null)
                    {
                        var endereco_dict = new Dictionary<string, object>
                        {
                            ["cd_loc_pais"] = enderecoExists["cd_loc_pais"],
                            ["cd_loc_estado"] = enderecoExists["cd_loc_estado"],
                            ["cd_loc_cidade"] = enderecoExists["cd_loc_cidade"],
                            ["cd_tipo_endereco"] = enderecoExists["cd_tipo_endereco"],
                            ["cd_loc_bairro"] = enderecoExists["cd_loc_bairro"],
                            ["cd_tipo_logradouro"] = enderecoExists["cd_tipo_logradouro"],
                            ["cd_loc_logradouro"] = enderecoExists["cd_loc_logradouro"],
                            ["dc_compl_endereco"] = enderecoExists["dc_compl_endereco"],
                            ["dc_num_cep"] = enderecoExists["dc_num_cep"],
                            ["dc_num_endereco"] = enderecoExists["dc_num_endereco"]
                        };
                        pessoa_dict.Add("enderecoPessoaPipeLine", endereco_dict);
                    }
                }
                if (pessoa_dict.Any()) retorno.Add("pessoa", pessoa_dict);

                return ResponseDefault(retorno);
            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }

        [Authorize]
        [HttpPatch]
        [Route("{id}/bloqueio")]
        public async Task<IActionResult> BloqueioPipeline(int id, bool bloqueio)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            var tokenInfo = Util.GetUserInfoFromToken(accessToken);

            var cd_pessoa = "";
            if (tokenInfo.Count > 0) cd_pessoa = tokenInfo["cd_pessoa"];

            if (string.IsNullOrEmpty(cd_pessoa)) return BadRequest("cd_pessoa não configurado");

            var schemaName = "T_Pessoa";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                var filtrosPipeline = new List<(string campo, object valor)> { new("cd_pipeline", id) };
                var pipeline = await SQLServerService.GetFirstByFields(source, "T_PIPELINE", filtrosPipeline);
                if (pipeline == null) return NotFound("pipeline não encontrada");

                var pipeline_dict = new Dictionary<string, object>();

                var id_pipeline_usuario = 1;
                var possui_atendente = 1;
                if (!bloqueio)
                {
                    id_pipeline_usuario = 0;
                    possui_atendente = 0;
                }
                pipeline_dict.Add("id_pipeline_usuario", id_pipeline_usuario);
                pipeline_dict.Add("cd_pessoa_bloqueio", cd_pessoa);
                pipeline_dict.Add("possui_atendente", possui_atendente);
                if (pipeline_dict.Any())
                {
                    var t_pipeline_update = await SQLServerService.Update("T_PIPELINE", pipeline_dict, source, "cd_pipeline", id);
                    if (!t_pipeline_update.success) return BadRequest(t_pipeline_update.error);
                }

                return ResponseDefault();
            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }
    }
}