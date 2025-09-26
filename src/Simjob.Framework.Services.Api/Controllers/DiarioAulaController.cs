using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Simjob.Framework.Application.Controllers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Schemas.Entities;
using Simjob.Framework.Services.Api.Enums;
using Simjob.Framework.Services.Api.Models;
using Simjob.Framework.Services.Api.Models.DiarioAula;
using Simjob.Framework.Services.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Simjob.Framework.Services.Api.Models.InsertUsuarioModel;

namespace Simjob.Framework.Services.Api.Controllers
{
    public class DiarioAulaController : BaseController
    {
        private readonly IRepository<SourceContext, Source> _sourceRepository;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;

        public DiarioAulaController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, IRepository<SourceContext, Source> sourceRepository, IRepository<MongoDbContext, Schema> schemaRepository) : base(bus, notifications)
        {
            _sourceRepository = sourceRepository;
            _schemaRepository = schemaRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">cd_diario_aula</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet()]
        [Route("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var schemaName = "T_Pessoa";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                //diario de aula
                var filtrosDiarioAula = new List<(string campo, object valor)> { new("cd_diario_aula", id) };
                var diaroExists = await SQLServerService.GetFirstByFields(source, "vi_diario_aula", filtrosDiarioAula);
                if (diaroExists == null) return NotFound("diario");
                var id_diario = diaroExists["cd_diario_aula"];
                var alunoEvento = await SQLServerService.GetList("vi_aluno_evento", null, "[cd_diario_aula]", $"[{id_diario}]", source, SearchModeEnum.Equals);
                diaroExists.Add("aluno_eventos", alunoEvento.data);
                return ResponseDefault(diaroExists);
            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }
        /// <summary>
        /// listagem de diario de aula
        /// </summary>
        /// <param name="value">campo de busca</param>
        /// <param name="mode"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="sortField"></param>
        /// <param name="sortDesc"></param>
        /// <param name="ids"></param>
        /// <param name="searchFields"></param>
        /// <param name="cd_empresa"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet()]
        public async Task<IActionResult> GetAll(string value, SearchModeEnum mode, int? page, int? limit, string sortField, bool sortDesc = false, string ids = "", string searchFields = null, string? cd_empresa = null)
        {
            if (cd_empresa == null) return BadRequest("campo cd_empresa não informado");

            var schemaName = "T_Pessoa";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (string.IsNullOrEmpty(sortField)) sortField = "cd_pessoa_escola";
            if (source != null && source.Active != null && source.Active == true)
            {

                var diarioResult = await SQLServerService.GetList("vi_diario_aula", page, limit, sortField, sortDesc, ids, searchFields, value, source, mode, "cd_pessoa_empresa", cd_empresa);
                if (diarioResult.success)
                {
                    var diarios = diarioResult.data;

                    var retorno = new
                    {
                        data = diarios,
                        diarioResult.total,
                        page,
                        limit,
                        pages = limit != null ? (int)Math.Ceiling((double)diarioResult.total / limit.Value) : 0
                    };

                    return ResponseDefault(retorno);

                }
                return BadRequest(new
                {
                    sucess = false,
                    error = diarioResult.error
                });
            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }


        /// <summary>
        /// criacao de diario de aula
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost()]
        public async Task<IActionResult> Post(DiarioAulaInsertModel model)
        {
            var schemaName = "T_Pessoa";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                var filtrosProgramacaoTurma = new List<(string campo, object valor)> { new("cd_programacao_turma", model.cd_programacao_turma) };
                var programacaoExists = await SQLServerService.GetFirstByFields(source, "T_PROGRAMACAO_TURMA", filtrosProgramacaoTurma);
                if (programacaoExists == null) return NotFound("programacao_turma");

                //cadastro diario aula
                var nm_aula = programacaoExists["nm_programacao_real"];

                //validar se diario de aula ja existe para turma e nm_aula
                var filtrosTurma = new List<(string campo, object valor)> { new("cd_turma", model.cd_turma),new("nm_aula_turma",nm_aula) };
                var diario_aulaExists = await SQLServerService.GetFirstByFields(source, "T_DIARIO_AULA", filtrosTurma);
                if (diario_aulaExists != null) return BadRequest($"diario de aula ja cadastrado para turma {model.cd_turma} e nm_aula {nm_aula}");

                var diario_dict = new Dictionary<string, object>
                {
                    ["cd_pessoa_empresa"] = model.cd_pessoa_empresa,
                    ["cd_turma"] = model.cd_turma,
                    ["cd_professor"] = model.cd_professor,
                    ["cd_tipo_aula"] = model.cd_tipo_aula,
                    ["cd_usuario"] = model.cd_usuario,
                    ["cd_professor_substituto"] = model.cd_professor_substituto,
                    ["cd_programacao_turma"] = model.cd_programacao_turma,
                    ["cd_sala"] = model.cd_sala,
                    //["cd_avaliacao"] = model.cd_avaliacao,
                    ["dt_aula"] = model.dt_aula.ToString("yyyy-MM-ddTHH:mm:ss"),
                    ["dt_cadastro_aula"] = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                    ["hr_inicial_aula"] = model.hr_inicial_aula,
                    ["hr_final_aula"] = model.hr_final_aula,
                    ["id_aula_externa"] = model.id_aula_externa,
                    ["id_status_aula"] = model.id_status_aula,
                    ["id_falta_professor"] = model.id_falta_professor,
                    ["dc_obs_falta"] = model.dc_obs_falta,
                    ["nm_aula_turma"] = nm_aula,
                    ["tx_obs_aula"] = model.tx_obs_aula,
                    ["cd_motivo_falta"] = model.cd_motivo_falta
                };

                var t_diario_insert = await SQLServerService.Insert("T_DIARIO_AULA", diario_dict, source);
                if (!t_diario_insert.success) return BadRequest(t_diario_insert.error);

                var t_diarioGet = await SQLServerService.GetList("T_DIARIO_AULA", 1, 1, "cd_diario_aula", true, null, null, "", source, SearchModeEnum.Equals, null, null);
                var t_diario = t_diarioGet.data.First();

                var cd_diario_aula = t_diario["cd_diario_aula"];


                //atualizar Alunoturma
                if (!model.grid_aluno.IsNullOrEmpty())
                {
                    foreach (var aluno in model.grid_aluno)
                    {
                        var filtrosaluno = new List<(string campo, object valor)> { new("cd_aluno_turma", aluno.cd_aluno_turma)};
                        var aluno_turmaExists = await SQLServerService.GetFirstByFields(source, "T_ALUNO_TURMA", filtrosaluno);
                        if (aluno_turmaExists == null) continue;

                        var nm_aulas_dadas = int.Parse(aluno_turmaExists["nm_aulas_dadas"]?.ToString() ?? "0");
                        var nm_faltas = int.Parse(aluno_turmaExists["nm_faltas"]?.ToString() ?? "0");

                        var aluno_atualizar = new Dictionary<string, object>
                        {
                            ["nm_aulas_dadas"] = nm_aulas_dadas+1,
                            ["nm_faltas"] = aluno.flg_falta ?nm_faltas + 1 : nm_faltas
                        };
                        var t_aluno_turma = await SQLServerService.Update("T_ALUNO_TURMA", aluno_atualizar, source, "cd_aluno_turma", aluno.cd_aluno_turma);
                        if (!t_aluno_turma.success) continue;

                        var cd_aluno = aluno_turmaExists["cd_aluno"];
                        if (aluno.flg_falta || aluno.flg_falta_justificada)
                        {
                            var t_aluno_evento = new Dictionary<string, object>
                            {
                                ["cd_diario_aula"] = cd_diario_aula,
                                ["cd_aluno"] = cd_aluno,
                                ["cd_evento"] = aluno.flg_falta ? 1:2,
                                ["dc_obs_justificada"] = aluno.dc_obs_justificada??"",

                            };
                            var t_aluno_evento_result = await SQLServerService.Insert("T_ALUNO_EVENTO", t_aluno_evento, source);
                            if (!t_aluno_evento_result.success) continue;

                        }
                    }
                }


                //atualizar T_programacao_turma
                var programacao_turma_dict = new Dictionary<string, object>
                {
                    ["id_aula_dada"] = 1
                };
                var t_programacao_turma = await SQLServerService.Update("T_PROGRAMACAO_TURMA", programacao_turma_dict, source, "cd_programacao_turma",model.cd_programacao_turma);
                if (!t_programacao_turma.success) return BadRequest(t_programacao_turma.error);

                return ResponseDefault();

            }

            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
           
        }

        /// <summary>
        /// atualiza diario de aula
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cd_diario_aula"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut()]
        [Route("{cd_diario_aula}")]
        public async Task<IActionResult> Put(DiarioAulaInsertModel model, int cd_diario_aula)
        {
            var schemaName = "T_Pessoa";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {


                var filtrosDiarioAula = new List<(string campo, object valor)> { new("cd_diario_aula", cd_diario_aula) };
                var diarioExists = await SQLServerService.GetFirstByFields(source, "T_DIARIO_AULA",filtrosDiarioAula);
                if (diarioExists == null) return NotFound("diario aula");

                var filtrosProgramacaoTurma = new List<(string campo, object valor)> { new("cd_programacao_turma", model.cd_programacao_turma) };
                var programacaoExists = await SQLServerService.GetFirstByFields(source, "T_PROGRAMACAO_TURMA", filtrosProgramacaoTurma);
                if (programacaoExists == null) return NotFound("programacao_turma");





                //diario aula
                var nm_aula = programacaoExists["nm_programacao_real"];
                var diario_dict = new Dictionary<string, object>
                {
                    ["cd_pessoa_empresa"] = model.cd_pessoa_empresa,
                    ["cd_turma"] = model.cd_turma,
                    ["cd_professor"] = model.cd_professor,
                    ["cd_tipo_aula"] = model.cd_tipo_aula,
                    ["cd_usuario"] = model.cd_usuario,
                    ["cd_professor_substituto"] = model.cd_professor_substituto,
                    ["cd_programacao_turma"] = model.cd_programacao_turma,
                    ["cd_sala"] = model.cd_sala,
                    ["cd_avaliacao"] = model.cd_avaliacao,
                    ["dt_aula"] = model.dt_aula.ToString("yyyy-MM-ddTHH:mm:ss"),
                    ["dt_cadastro_aula"] = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                    ["hr_inicial_aula"] = model.hr_inicial_aula,
                    ["hr_final_aula"] = model.hr_final_aula,
                    ["id_aula_externa"] = model.id_aula_externa,
                    ["id_status_aula"] = model.id_status_aula,
                    ["id_falta_professor"] = model.id_falta_professor,
                    ["dc_obs_falta"] = model.dc_obs_falta,
                    ["nm_aula_turma"] = nm_aula,
                    ["tx_obs_aula"] = model.tx_obs_aula,
                    ["cd_motivo_falta"] = model.cd_motivo_falta
                };


                var t_diario_insert = await SQLServerService.Update("T_DIARIO_AULA", diario_dict, source, "cd_diario_aula", cd_diario_aula);
                if (!t_diario_insert.success) return BadRequest(t_diario_insert.error);


                //atualizar Alunoturma
                if (!model.grid_aluno.IsNullOrEmpty())
                {
                    await SQLServerService.Delete("T_ALUNO_EVENTO", "cd_diario_aula", cd_diario_aula.ToString(), source);
                    foreach (var aluno in model.grid_aluno)
                    {
                        var filtrosaluno = new List<(string campo, object valor)> { new("cd_aluno_turma", aluno.cd_aluno_turma) };
                        var aluno_turmaExists = await SQLServerService.GetFirstByFields(source, "T_ALUNO_TURMA", filtrosaluno);
                        if (aluno_turmaExists == null) continue;

                        var nm_aulas_dadas = int.Parse(aluno_turmaExists["nm_aulas_dadas"]?.ToString() ?? "0");
                        var nm_faltas = int.Parse(aluno_turmaExists["nm_faltas"]?.ToString() ?? "0");

                        var aluno_atualizar = new Dictionary<string, object>
                        {
                            ["nm_aulas_dadas"] = nm_aulas_dadas + 1,
                            ["nm_faltas"] = aluno.flg_falta ? nm_faltas + 1 : nm_faltas
                        };
                        var t_aluno_turma = await SQLServerService.Update("T_ALUNO_TURMA", aluno_atualizar, source, "cd_aluno_turma", aluno.cd_aluno_turma);
                        if (!t_aluno_turma.success) continue;

                        
                        var cd_aluno = aluno_turmaExists["cd_aluno"];
                        if (aluno.flg_falta || aluno.flg_falta_justificada)
                        {

                            var t_aluno_evento = new Dictionary<string, object>
                            {
                                ["cd_diario_aula"] = cd_diario_aula,
                                ["cd_aluno"] = cd_aluno,
                                ["cd_evento"] = aluno.flg_falta ? 1 : 2,
                                ["dc_obs_justificada"] = aluno.dc_obs_justificada ?? "",

                            };
                            var t_aluno_evento_result = await SQLServerService.Insert("T_ALUNO_EVENTO", t_aluno_evento, source);
                            if (!t_aluno_evento_result.success) continue;

                        }
                    }
                }


                //atualizar T_programacao_turma
                var programacao_turma_dict = new Dictionary<string, object>
                {
                    ["id_aula_dada"] = 1
                };
                var t_programacao_turma = await SQLServerService.Update("T_PROGRAMACAO_TURMA", programacao_turma_dict, source, "cd_programacao_turma", model.cd_programacao_turma);
                if (!t_programacao_turma.success) return BadRequest(t_programacao_turma.error);

                return ResponseDefault();
            }

            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }

        /// <summary>
        /// Remove um diario de aula (fisico)
        /// </summary>
        /// <param name="cd_diario_aula"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete()]
        [Route("{cd_diario_aula}")]
        public async Task<IActionResult> Delete(int cd_diario_aula)
        {
            var schemaName = "T_Pessoa";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                var filtrosDiarioAula = new List<(string campo, object valor)> { new("cd_diario_aula", cd_diario_aula) };
                var diarioExists = await SQLServerService.GetFirstByFields(source, "T_DIARIO_AULA", filtrosDiarioAula);
                if (diarioExists == null) return NotFound("diario aula");

                //diario aula
                var diario_dict = new Dictionary<string, object>
                {
                    ["id_status_aula"] = 1              
                };
                var t_diario_insert = await SQLServerService.Update("T_DIARIO_AULA", diario_dict, source, "cd_diario_aula", cd_diario_aula);
                if (!t_diario_insert.success) return BadRequest(t_diario_insert.error);
                return ResponseDefault();

            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }


    }

}
