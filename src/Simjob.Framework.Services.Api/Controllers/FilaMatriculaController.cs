using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Simjob.Framework.Application.Controllers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Schemas.Entities;
using Simjob.Framework.Services.Api.Models;
using Simjob.Framework.Services.Api.Services;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using Simjob.Framework.Services.Api.Models.FilaMatricula;
using Simjob.Framework.Services.Api.Enums;
using static MongoDB.Driver.WriteConcern;

namespace Simjob.Framework.Services.Api.Controllers
{
    public class FilaMatriculaController : BaseController
    {
        private readonly IRepository<SourceContext, Source> _sourceRepository;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;

        public FilaMatriculaController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, IRepository<SourceContext, Source> sourceRepository, IRepository<MongoDbContext, Schema> schemaRepository) : base(bus, notifications)
        {
            _sourceRepository = sourceRepository;
            _schemaRepository = schemaRepository;
        }

        [Authorize]
        [HttpPost()]
        public async Task<IActionResult> Insert([FromBody] InsertFilaMatriculaModel command)
        {
            var schemaName = "T_Pessoa";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                var resultReturn = await ValidateCommand(command, source);
                var result = new
                {
                    resultReturn.sucess,
                    resultReturn.error
                };
                return resultReturn.sucess ? ResponseDefault(result) : BadRequest(result);

            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }

        [Authorize]
        [HttpPost]
        [Route("VendaPerdida")]
        public async Task<IActionResult> VendaPerdida(VendaPerdidaModel model)
        {
            var schemaName = "T_Pessoa";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                var filtrosFilaMatricula = new List<(string campo, object valor)> { new("cd_fila_matricula", model.cd_fila_matricula) };
                var filaMatriculaExists = await SQLServerService.GetFirstByFields(source, "T_FILA_MATRICULA", filtrosFilaMatricula);
                if (filaMatriculaExists == null) return NotFound("fila matricula não encontrada");

                var cd_pessoa = filaMatriculaExists["cd_pessoa_fila"];
                var cd_acao = filaMatriculaExists["cd_acao"];
                object cd_pipeline = null;
                object cd_pessoa_filho = null;

                // Buscar pipeline da pessoa principal (como no SP)
                var filtrosPipelinePrincipal = new List<(string campo, object valor)> 
                { 
                    new("cd_pessoa_pipeline", cd_pessoa),
                    new("cd_acao", cd_acao),
                    new("cd_etapa_pipeline", 3)
                };
                var pipelinePrincipal = await SQLServerService.GetFirstByFields(source, "T_PIPELINE", filtrosPipelinePrincipal);
                
                if (pipelinePrincipal != null)
                {
                    cd_pipeline = pipelinePrincipal["cd_pipeline"];
                }
                else
                {
                    // Se não encontrou pipeline da pessoa principal, verificar se é responsável por dependente
                    var filtrosRelacionamento = new List<(string campo, object valor)> 
                    { 
                        new("cd_pessoa_pai", cd_pessoa),
                        new("cd_papel_filho", 3)
                    };
                    var relacionamento = await SQLServerService.GetFirstByFields(source, "T_RELACIONAMENTO", filtrosRelacionamento);
                    
                    if (relacionamento != null)
                    {
                        cd_pessoa_filho = relacionamento["cd_pessoa_filho"];
                        
                        // Buscar pipeline do dependente
                        var filtrosPipelineFilho = new List<(string campo, object valor)> 
                        { 
                            new("cd_pessoa_pipeline", cd_pessoa_filho),
                            new("cd_acao", cd_acao),
                            new("cd_etapa_pipeline", 3)
                        };
                        var pipelineFilho = await SQLServerService.GetFirstByFields(source, "T_PIPELINE", filtrosPipelineFilho);
                        
                        if (pipelineFilho != null)
                        {
                            cd_pipeline = pipelineFilho["cd_pipeline"];
                        }
                    }
                }

                // Atualizar pipeline se encontrado (seguindo lógica da SP)
                if (cd_pipeline != null)
                {
                    var observacao = cd_pessoa_filho == null 
                        ? "Motivo de Perda realizado através da fila de matricula " 
                        : "Motivo de Perda realizado através da fila de matricula do responsável";

                    var pipelineDict = new Dictionary<string, object>
                    {
                        { "cd_motivo_perda", model.cd_motivo_perda },
                        { "dt_realizada", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") },
                        { "tx_obs_pipeline", observacao }
                    };

                    var updatePipelineResult = await SQLServerService.Update("T_PIPELINE", pipelineDict, source, "cd_pipeline", cd_pipeline);
                    if (!updatePipelineResult.success) 
                    {
                        return BadRequest(new
                        {
                            success = false,
                            error = updatePipelineResult.error
                        });
                    }
                }

                // Atualizar status da fila para 2 (perdida) - como no SP
                var filaDict = new Dictionary<string, object>
                {
                    { "id_status_fila", 2 }
                };
                var updateFilaResult = await SQLServerService.Update("T_FILA_MATRICULA", filaDict, source, "cd_fila_matricula", model.cd_fila_matricula);
                if (!updateFilaResult.success) 
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = updateFilaResult.error
                    });
                }

                return ResponseDefault(new
                {
                    success = true,
                    message = "Registro alterado com sucesso!"
                });
            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }


        [Authorize]
        [HttpPut("{cd_fila_matricula}")]
        public async Task<IActionResult> Update([FromBody] InsertFilaMatriculaModel command,int cd_fila_matricula)
        {
            var schemaName = "T_Pessoa";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                var resultReturn = await ProcessaUpdate(cd_fila_matricula,command, source);
                var result = new
                {
                    resultReturn.sucess,
                    resultReturn.error
                };
                return resultReturn.sucess ? ResponseDefault(result) : BadRequest(result);

            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }

        private async Task<(bool sucess, string error)> ValidateCommand(InsertFilaMatriculaModel command, Source source)
        {
            object? cd_pessoa = null;
            //valida cpf
            if (command.nm_cpf != null && !string.IsNullOrEmpty(command.nm_cpf))
            {
                var filtros = new List<(string campo, object valor)> { new("nm_cpf", command.nm_cpf) };
                var cpfExist = await SQLServerService.GetFirstByFields(source, "T_PESSOA_FISICA", filtros);
                if (cpfExist != null) cd_pessoa = cpfExist["cd_pessoa_fisica"];
            }

            try
            {
                if (cd_pessoa == null)
                {
                    //valida email
                    if (command.email != null && !string.IsNullOrEmpty(command.email))
                    {
                        var filtros = new List<(string campo, object valor)> { new("dc_fone_mail", command.email) };
                        var emailExist = await SQLServerService.GetFirstByFields(source, "T_TELEFONE", filtros);
                        if (emailExist != null) return (false, $"Já existe um registro com este e-mail({command.email}) cadastrado");
                    }

                    var pessoaDict = new Dictionary<string, object>
                    {
                        //{ "cd_pessoa", null },
                        { "no_pessoa", command.no_pessoa },
                        { "dt_cadastramento", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") },
                        { "cd_atividade_principal", null },
                        { "cd_endereco_principal", null },
                        { "cd_telefone_principal", null },
                        { "id_pessoa_empresa",0 },
                        { "nm_natureza_pessoa",1 },
                        { "dc_num_pessoa", null },
                        { "id_exportado", 0 },
                        { "cd_papel_principal", null },
                        { "txt_obs_pessoa", null }
                    };

                    var t_pessoa_insert = await SQLServerService.InsertWithResult("T_PESSOA", pessoaDict, source);
                    if (!t_pessoa_insert.success) return new(t_pessoa_insert.success, t_pessoa_insert.error);
                    var t_pessoa = t_pessoa_insert.inserted;
                    cd_pessoa = t_pessoa["cd_pessoa"];

                    if (command.email != null)
                    {
                        //T_TELEFONE(email)
                        var telefoneDictEmail = new Dictionary<string, object>
                        {
                            //{ "cd_telefone", null },
                            { "cd_pessoa", cd_pessoa },
                            { "cd_tipo_telefone", 4 },
                            { "cd_classe_telefone", 1 },
                            { "dc_fone_mail", command.email },
                            { "cd_endereco", null },
                            { "id_telefone_principal",1 },
                            { "cd_operadora", null }
                        };
                        var t_telefone_email_insert = await SQLServerService.Insert("T_TELEFONE", telefoneDictEmail, source);
                        if (!t_telefone_email_insert.success) return new(t_telefone_email_insert.success, t_telefone_email_insert.error);
                    }

                    if (command.telefone != null)
                    {
                        //T_TELEFONE(telefone)
                        var telefoneDictTelefone = new Dictionary<string, object>
                        {
                            //{ "cd_telefone", null },
                            { "cd_pessoa", cd_pessoa },
                            { "cd_tipo_telefone", 1 },
                            { "cd_classe_telefone", 1 },
                            { "dc_fone_mail", command.telefone },
                            { "cd_endereco", null },
                            { "id_telefone_principal", 1 },
                            { "cd_operadora", null }
                        };

                        var t_telefone_telefone_insert = await SQLServerService.Insert("T_TELEFONE", telefoneDictTelefone, source);

                        if (!t_telefone_telefone_insert.success) return new(t_telefone_telefone_insert.success, t_telefone_telefone_insert.error);
                    }

                    //T_PESSOA_FISICA
                    var pessoa_fisicaDict = new Dictionary<string, object>
                    {
                        { "cd_pessoa_fisica", cd_pessoa },
                        { "nm_sexo", command.nm_sexo },
                        { "id_exportado", 0 },
                        { "nm_cpf", command.nm_cpf }
                    };
                    var t_pessoa_fisica_insert = await SQLServerService.Insert("T_PESSOA_FISICA", pessoa_fisicaDict, source);
                    if (!t_pessoa_fisica_insert.success) return new(t_pessoa_fisica_insert.success, t_pessoa_fisica_insert.error);
                }
                var cd_pessoa_escola = command.cd_pessoa_escola;

                //valida se existe uma registro em fila
                var filtroFila = new List<(string campo, object valor)> { new("cd_pessoa_fila", cd_pessoa),new("cd_pessoa_escola",cd_pessoa_escola),new ("id_status_fila",1) };
                var filaExist = await SQLServerService.GetFirstByFields(source, "T_FILA_MATRICULA", filtroFila);
                if (filaExist != null) return (false, $"Já existe um registro de fila de matricula com este usuario cadastrado");

                //T_PESSOA_EMPRESA
                var t_pessoa_empresa_dict = new Dictionary<string, object>
                {                      
                    { "cd_pessoa", cd_pessoa },          
                    { "cd_empresa", cd_pessoa_escola }          
                };
                var t_pessoa_empresa_insert = await SQLServerService.Insert("T_PESSOA_EMPRESA", t_pessoa_empresa_dict, source);
                if (!t_pessoa_empresa_insert.success) return new(t_pessoa_empresa_insert.success, t_pessoa_empresa_insert.error);

                if (cd_pessoa_escola > 0)
                {
                    //fila_matricula
                    var fila_matricula_dict = new Dictionary<string, object>
                    {                                         
                        { "cd_pessoa_escola", cd_pessoa_escola },                  
                        { "cd_pessoa_fila", cd_pessoa },                   
                        { "id_status_fila", command.id_status_fila },              
                        { "dt_programada_contato", command.dt_programada_contato.ToString("yyyy-MM-ddTHH:mm:ss") }, 
                        { "cd_produto", command.cd_produto },                     
                        //{ "cd_curso_recomendado", command.cd_curso_recomendado },           
                        { "cd_acao", command.cd_acao }                
                    };
                    var fila_matricula_insert = await SQLServerService.InsertWithResult("T_FILA_MATRICULA", fila_matricula_dict, source);
                    if (!fila_matricula_insert.success) return new(fila_matricula_insert.success, fila_matricula_insert.error);
                    var t_fila_matricula = fila_matricula_insert.inserted;
                    var cd_fila_matricula = t_fila_matricula["cd_fila_matricula"];
                    foreach (var d in command.dias)
                    {
                        //T_FILA_MATRICULA_DIA
                        var fila_matricula_dia_dict = new Dictionary<string, object>
                        {                                    
                            { "cd_fila_matricula",cd_fila_matricula }, 
                            { "id_dia_semana", d.id_dia_semana}      
                        };
                        var fila_matricula_dia_insert = await SQLServerService.Insert("T_FILA_DIA", fila_matricula_dia_dict, source);
                        if (!fila_matricula_insert.success) return new(fila_matricula_insert.success, fila_matricula_insert.error);
                    }
                    foreach (var p in command.periodos)
                    {
                        //T_FILA_MATRICULA_PERIODO
                        var fila_matricula_periodo_dict = new Dictionary<string, object>
                        {
                            { "cd_fila_matricula",cd_fila_matricula },
                            { "id_periodo", p.id_periodo}
                        };
                        
                        var fila_matricula_periodo_insert = await SQLServerService.Insert("T_FILA_PERIODO", fila_matricula_periodo_dict, source);
                        if (!fila_matricula_insert.success) return new(fila_matricula_insert.success, fila_matricula_insert.error);
                    }


                }
            }
            catch (Exception ex)
            {
                return (false, $"Erro: {ex.Message}");
            }


            return (true, string.Empty);
        }

        private async Task<(bool sucess, string error)> ProcessaUpdate(int cd_fila_matricula,InsertFilaMatriculaModel command, Source source)
        {
            try
            {
                //valida se existe uma registro em fila
                var filtroFila = new List<(string campo, object valor)> { new("cd_fila_matricula", cd_fila_matricula) };
                var filaExist = await SQLServerService.GetFirstByFields(source, "T_FILA_MATRICULA", filtroFila);
                if (filaExist == null) return (false, $"fila de matricula não encontrada");
                var cd_pessoa = filaExist["cd_pessoa_fila"];

                // Atualizar T_PESSOA
                var pessoaDict = new Dictionary<string, object>
                {
                    { "no_pessoa", command.no_pessoa }
                };
                var t_pessoa_update = await SQLServerService.Update("T_PESSOA", pessoaDict, source, "cd_pessoa", cd_pessoa);
                if (!t_pessoa_update.success) return new(t_pessoa_update.success, t_pessoa_update.error);

                // Atualizar/Inserir EMAIL em T_TELEFONE
                if (command.email != null)
                {
                    var filtrosEmail = new List<(string campo, object valor)> 
                    { 
                        new("cd_pessoa", cd_pessoa),
                        new("cd_tipo_telefone", 4)
                    };
                    var emailExist = await SQLServerService.GetFirstByFields(source, "T_TELEFONE", filtrosEmail);
                    
                    var telefoneDictEmail = new Dictionary<string, object>
                    {
                        { "cd_pessoa", cd_pessoa },
                        { "cd_tipo_telefone", 4 },
                        { "cd_classe_telefone", 1 },
                        { "dc_fone_mail", command.email },
                        { "cd_endereco", null },
                        { "id_telefone_principal", 1 },
                        { "cd_operadora", null }
                    };

                    if (emailExist != null)
                    {
                        var cd_telefone = emailExist["cd_telefone"];
                        var t_telefone_email_update = await SQLServerService.Update("T_TELEFONE", telefoneDictEmail, source, "cd_telefone", cd_telefone);
                        if (!t_telefone_email_update.success) return new(t_telefone_email_update.success, t_telefone_email_update.error);
                    }
                    else
                    {
                        var t_telefone_email_insert = await SQLServerService.Insert("T_TELEFONE", telefoneDictEmail, source);
                        if (!t_telefone_email_insert.success) return new(t_telefone_email_insert.success, t_telefone_email_insert.error);
                    }
                }

                // Atualizar/Inserir TELEFONE em T_TELEFONE
                if (command.telefone != null)
                {
                    var filtrosTelefone = new List<(string campo, object valor)> 
                    { 
                        new("cd_pessoa", cd_pessoa),
                        new("cd_tipo_telefone", 1)
                    };
                    var telefoneExist = await SQLServerService.GetFirstByFields(source, "T_TELEFONE", filtrosTelefone);
                    
                    var telefoneDictTelefone = new Dictionary<string, object>
                    {
                        { "cd_pessoa", cd_pessoa },
                        { "cd_tipo_telefone", 1 },
                        { "cd_classe_telefone", 1 },
                        { "dc_fone_mail", command.telefone },
                        { "cd_endereco", null },
                        { "id_telefone_principal", 1 },
                        { "cd_operadora", null }
                    };

                    if (telefoneExist != null)
                    {
                        var cd_telefone = telefoneExist["cd_telefone"];
                        var t_telefone_telefone_update = await SQLServerService.Update("T_TELEFONE", telefoneDictTelefone, source, "cd_telefone", cd_telefone);
                        if (!t_telefone_telefone_update.success) return new(t_telefone_telefone_update.success, t_telefone_telefone_update.error);
                    }
                    else
                    {
                        var t_telefone_telefone_insert = await SQLServerService.Insert("T_TELEFONE", telefoneDictTelefone, source);
                        if (!t_telefone_telefone_insert.success) return new(t_telefone_telefone_insert.success, t_telefone_telefone_insert.error);
                    }
                }

                // Atualizar/Inserir TELEFONE em T_TELEFONE
                if (command.celular != null)
                {
                    var filtrosTelefone = new List<(string campo, object valor)>
                    {
                        new("cd_pessoa", cd_pessoa),
                        new("cd_tipo_telefone", 3)
                    };
                    var telefoneExist = await SQLServerService.GetFirstByFields(source, "T_TELEFONE", filtrosTelefone);

                    var telefoneDictTelefone = new Dictionary<string, object>
                    {
                        { "cd_pessoa", cd_pessoa },
                        { "cd_tipo_telefone", 3 },
                        { "cd_classe_telefone", 1 },
                        { "dc_fone_mail", command.celular },
                        { "cd_endereco", null },
                        { "cd_operadora", null }
                    };

                    if (telefoneExist != null)
                    {
                        var cd_telefone = telefoneExist["cd_telefone"];
                        var t_telefone_telefone_update = await SQLServerService.Update("T_TELEFONE", telefoneDictTelefone, source, "cd_telefone", cd_telefone);
                        if (!t_telefone_telefone_update.success) return new(t_telefone_telefone_update.success, t_telefone_telefone_update.error);
                    }
                    else
                    {
                        var t_telefone_telefone_insert = await SQLServerService.Insert("T_TELEFONE", telefoneDictTelefone, source);
                        if (!t_telefone_telefone_insert.success) return new(t_telefone_telefone_insert.success, t_telefone_telefone_insert.error);
                    }
                }

                // Atualizar T_PESSOA_FISICA
                var pessoa_fisicaDict = new Dictionary<string, object>
                {
                    { "nm_sexo", command.nm_sexo },
                    { "nm_cpf", command.nm_cpf }
                };
                var t_pessoa_fisica_update = await SQLServerService.Update("T_PESSOA_FISICA", pessoa_fisicaDict, source, "cd_pessoa_fisica", cd_pessoa);
                if (!t_pessoa_fisica_update.success) return new(t_pessoa_fisica_update.success, t_pessoa_fisica_update.error);

                var cd_pessoa_escola = command.cd_pessoa_escola;

                if (cd_pessoa_escola > 0)
                {
                    //Atualizar T_FILA_MATRICULA
                    var fila_matricula_dict = new Dictionary<string, object>
                    {
                        { "cd_pessoa_escola", cd_pessoa_escola },
                        { "cd_pessoa_fila", cd_pessoa },
                        { "id_status_fila", command.id_status_fila },
                        { "dt_programada_contato", command.dt_programada_contato.ToString("yyyy-MM-ddTHH:mm:ss") },
                        { "cd_produto", command.cd_produto },                     
                        { "cd_acao", command.cd_acao },
                        { "nm_resultado_teste", command.nm_resultado_teste },
                        { "cd_curso_recomendado", command.cd_curso_recomendado },
                    };
                    var fila_matricula_update = await SQLServerService.Update("T_FILA_MATRICULA", fila_matricula_dict, source, "cd_fila_matricula", cd_fila_matricula);
                    if (!fila_matricula_update.success) return new(fila_matricula_update.success, fila_matricula_update.error);

                    //remove todos os registros de fila_dia e cadastra novamente.
                    await SQLServerService.Delete("T_FILA_DIA", "cd_fila_matricula", cd_fila_matricula.ToString(), source);
                    foreach (var d in command.dias)
                    {
                        //T_FILA_MATRICULA_DIA
                        var fila_matricula_dia_dict = new Dictionary<string, object>
                        {
                            { "cd_fila_matricula", cd_fila_matricula },
                            { "id_dia_semana", d.id_dia_semana}
                        };
                        var fila_matricula_dia_insert = await SQLServerService.Insert("T_FILA_DIA", fila_matricula_dia_dict, source);
                        if (!fila_matricula_dia_insert.success) return new(fila_matricula_dia_insert.success, fila_matricula_dia_insert.error);
                    }
                    
                    //remove todos os registros de fila_periodo e cadastra novamente.
                    await SQLServerService.Delete("T_FILA_PERIODO", "cd_fila_matricula", cd_fila_matricula.ToString(), source);
                    foreach (var p in command.periodos)
                    {
                        //T_FILA_MATRICULA_PERIODO
                        var fila_matricula_periodo_dict = new Dictionary<string, object>
                        {
                            { "cd_fila_matricula", cd_fila_matricula },
                            { "id_periodo", p.id_periodo}
                        };

                        var fila_matricula_periodo_insert = await SQLServerService.Insert("T_FILA_PERIODO", fila_matricula_periodo_dict, source);
                        if (!fila_matricula_periodo_insert.success) return new(fila_matricula_periodo_insert.success, fila_matricula_periodo_insert.error);
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, $"Erro: {ex.Message}");
            }

            return (true, string.Empty);
        }
        [Authorize]
        [HttpGet()]
        public async Task<IActionResult> GetAll(string value, SearchModeEnum mode, int? page, int? limit, string sortField, bool sortDesc = false, string ids = "", string searchFields = null, string? cd_empresa = null)
        {
            if (cd_empresa == null) return BadRequest("campo cd_empresa não informado");
            var schemaName = "T_FilaMatricula";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                var filasResult = await SQLServerService.GetList("vi_fila_matricula", page, limit, sortField, sortDesc, ids, searchFields, value, source, mode, "cd_pessoa_escola",cd_empresa);
                if (filasResult.success)
                {

                    var filas = filasResult.data;
                    //dependentes
                       
                    var retorno = new
                    {
                        data = filas.Select(x => new
                        {
                            cd_fila_matricula = x["cd_fila_matricula"],
                            cd_pessoa_fila = x["cd_pessoa_fila"],
                            no_pessoa = x["no_pessoa"], 
                            cd_acao = x["cd_acao"],
                            no_acao = x["no_acao"],
                            dt_programada_contato = x["dt_programada_contato"],
                            cd_produto = x["cd_produto"],
                            no_produto = x["no_produto"],
                            cd_curso_recomendado = x["cd_curso_recomendado"],
                            no_curso = x["no_curso"],
                            id_status_fila = x["id_status_fila"],
                            Email = x["email"],
                            Celular = x["celular"],
                            Telefone = x["telefone"]
                        }),
                        filasResult.total,
                        page,
                        limit,
                        pages = limit != null ? (int)Math.Ceiling((double)filasResult.total / limit.Value) : 0
                    };

                    return ResponseDefault(retorno);

                }
                return BadRequest(new
                {
                    sucess = false,
                    error = filasResult.error
                });
            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }


        [Authorize]
        [HttpGet()]
        [Route("{cd_fila_matricula}")]
        public async Task<IActionResult> Get(int cd_fila_matricula)
        {
            var schemaName = "T_FilaMatricula";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                var retorno = new InsertFilaMatriculaModel();

                var filtrosFilaMatricula = new List<(string campo, object valor)> { new("cd_fila_matricula", cd_fila_matricula) };
                var fila_matriculaExistente = await SQLServerService.GetFirstByFields(source, "vi_fila_matricula", filtrosFilaMatricula);
                if (fila_matriculaExistente == null) return NotFound("fila_matricula");

                var cd_pessoa = fila_matriculaExistente["cd_pessoa_fila"].ToString();
                retorno.no_pessoa = fila_matriculaExistente["no_pessoa"].ToString();
                retorno.cd_pessoa = cd_pessoa;
                retorno.cd_pessoa_escola = (int)fila_matriculaExistente["cd_pessoa_escola"];
                retorno.cd_acao = (int)fila_matriculaExistente["cd_acao"];
                retorno.id_status_fila = int.Parse(fila_matriculaExistente["id_status_fila"].ToString());
                retorno.cd_produto = fila_matriculaExistente["cd_produto"] != null ? (int)fila_matriculaExistente["cd_produto"] : null;
                retorno.dt_programada_contato = fila_matriculaExistente["dt_programada_contato"] != null ? DateTime.Parse(fila_matriculaExistente["dt_programada_contato"].ToString()) : DateTime.MinValue;
                retorno.cd_curso_recomendado = fila_matriculaExistente["cd_curso_recomendado"] != null ? (int)fila_matriculaExistente["cd_curso_recomendado"] : null;
                retorno.nm_resultado_teste = (double)fila_matriculaExistente["nm_resultado_teste"];

                var filtrosEmail = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa), new("cd_tipo_telefone", 4) };
                var emailExists = await SQLServerService.GetFirstByFields(source, "T_Telefone", filtrosEmail);
                if (emailExists != null) retorno.email = emailExists["dc_fone_mail"].ToString();

                var filtrosTelefone = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa), new("cd_tipo_telefone", 1) };
                var telefoneExists = await SQLServerService.GetFirstByFields(source, "T_Telefone", filtrosTelefone);
                if (telefoneExists != null) retorno.telefone = telefoneExists["dc_fone_mail"].ToString();

                var filtroscelular = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa), new("cd_tipo_telefone", 3) };
                var celularExists = await SQLServerService.GetFirstByFields(source, "T_Telefone", filtroscelular);
                if (telefoneExists != null) retorno.celular = celularExists != null?celularExists["dc_fone_mail"].ToString():"";


                var filtrosPessoaFisica = new List<(string campo, object valor)> { new("cd_pessoa_fisica", cd_pessoa) };
                var pessoaFisicaExists = await SQLServerService.GetFirstByFields(source, "T_PESSOA_FISICA", filtrosPessoaFisica);
                if (pessoaFisicaExists != null)
                {
                    var nm_sexo = pessoaFisicaExists["nm_sexo"];
                    retorno.nm_sexo = int.Parse(nm_sexo.ToString());
                    retorno.nm_cpf = pessoaFisicaExists["nm_cpf"] != null ? pessoaFisicaExists["nm_cpf"].ToString() : "";                  
                }
                //Fila_dia
                var t_fila_dia_result = await SQLServerService.GetList("T_FILA_DIA",null,null, "cd_fila_matricula", false,null,"[cd_fila_matricula]",$"[{cd_fila_matricula.ToString()}]",source,SearchModeEnum.Equals);
                if (t_fila_dia_result.success)
                {
                    foreach (var dia in t_fila_dia_result.data)
                    {
                        retorno.dias.Add(new InsertFilaMatriculaModel.Dia
                        {
                            id_dia_semana = int.Parse(dia["id_dia_semana"].ToString())
                        });
                    }
                }
                var t_fila_periodo_result = await SQLServerService.GetList("T_FILA_PERIODO", null, null, "cd_fila_matricula", false, null, "[cd_fila_matricula]", $"[{cd_fila_matricula.ToString()}]", source, SearchModeEnum.Equals);
                if (t_fila_periodo_result.success)
                {
                    foreach (var periodo in t_fila_periodo_result.data)
                    {
                        retorno.periodos.Add(new InsertFilaMatriculaModel.Periodo
                        {
                            id_periodo = int.Parse(periodo["id_periodo"].ToString())
                        });
                    }
                }






                return ResponseDefault(retorno);
            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }

        [Authorize]
        [HttpPatch()]
        [Route("{cd_fila_matricula}")]
        public async Task<IActionResult> Patch(int cd_fila_matricula)
        {
            var schemaName = "T_Pessoa";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                var filtrosFilaMatricula = new List<(string campo, object valor)> { new("cd_fila_matricula", cd_fila_matricula) };
                var filaMatriculaExists = await SQLServerService.GetFirstByFields(source, "T_FILA_MATRICULA", filtrosFilaMatricula);
                if (filaMatriculaExists == null) return NotFound("fila matricula não encontrada");

                var value = 1;
                if (int.Parse(filaMatriculaExists["id_status_fila"].ToString()) != 0) value = 0;
                var filaMatriculaDict = new Dictionary<string, object>
                {
                    { "id_status_fila", value }
                };

                var t_fila_matricula = await SQLServerService.Update("T_FILA_MATRICULA", filaMatriculaDict, source, "cd_fila_matricula", cd_fila_matricula);
                if (!t_fila_matricula.success) return BadRequest(t_fila_matricula.error);
                //var t_fila_matricula = await SQLServerService.Delete("T_FILA_MATRICULA", "cd_fila_matricula", cd_fila_matricula.ToString(), source);
                //if (!t_fila_matricula.success) return BadRequest(t_fila_matricula.error);

                return ResponseDefault();
            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }
    }
}
