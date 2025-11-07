using DocumentFormat.OpenXml.EMMA;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyModel;
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
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using static Simjob.Framework.Services.Api.Models.InsertUsuarioModel;

namespace Simjob.Framework.Services.Api.Controllers
{
    public class AlunoController : BaseController
    {
        private readonly IRepository<SourceContext, Source> _sourceRepository;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;
        private readonly IConfiguration _config;

        public AlunoController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, IRepository<SourceContext, Source> sourceRepository, IRepository<MongoDbContext, Schema> schemaRepository, IConfiguration config) : base(bus, notifications)
        {
            _sourceRepository = sourceRepository;
            _schemaRepository = schemaRepository;
            _config = config;
        }

        [Authorize]
        [HttpPost()]
        public async Task<IActionResult> Insert([FromBody] InsertUsuarioModel command)
        {
            var schemaName = "T_Pessoa";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                if (!string.IsNullOrEmpty(command.pessoa.img_pessoa) && command.pessoa.img_pessoa.Contains(","))
                    command.pessoa.img_pessoa = command.pessoa.img_pessoa.Split(',')[1];

                var resultReturn = await ValidateCommand(command, source);
                var result = new
                {
                    resultReturn.sucess,
                    resultReturn.error,
                    resultReturn.data
                };
                return resultReturn.sucess ? ResponseDefault(result) : BadRequest(result);
            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }

        [Authorize]
        [HttpPut()]
        [Route("{cd_pessoa}")]
        public async Task<IActionResult> Update([FromBody] InsertUsuarioModel command, int cd_pessoa)
        {
            var schemaName = "T_Pessoa";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                if (!string.IsNullOrEmpty(command.pessoa.img_pessoa) && command.pessoa.img_pessoa.Contains(","))
                    command.pessoa.img_pessoa = command.pessoa.img_pessoa.Split(',')[1];

                var resultReturn = await ProcessaUpdate(cd_pessoa, command, source);
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
        [HttpGet()]
        public async Task<IActionResult> GetAll(string value, SearchModeEnum mode, int? page, int? limit, string sortField, bool sortDesc = false, string ids = "", string searchFields = null, string? cd_empresa = null)
        {
            if (cd_empresa == null) return BadRequest("campo cd_empresa não informado");

            var schemaName = "T_Pessoa";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                var alunosResult = await SQLServerService.GetList("vi_aluno", page, limit, sortField, sortDesc, ids, searchFields, value, source, mode, "cd_pessoa_escola", cd_empresa);
                if (alunosResult.success)
                {
                    var alunos = alunosResult.data;

                    var retorno = new
                    {
                        data = alunos.Select(x => new
                        {
                            cd_aluno = x["cd_aluno"],
                            cd_pessoa = x["cd_pessoa"],
                            no_pessoa = x["no_pessoa"],
                            nm_cpf = x["nm_cpf"] ?? string.Empty,
                            Email = x["email"],
                            Telefone = x["telefone"],
                            Celular = x["celular"],
                            nm_sexo = x["nm_sexo"],
                            id_aluno_ativo = (bool)x["id_aluno_ativo"] == true ? "Ativo" : "Inativo",
                            ExisteMatricula = x["existeMatricula"],
                            Possui_vinculo = x["possui_vinculo"]
                        }),
                        alunosResult.total,
                        page,
                        limit,
                        pages = limit != null ? (int)Math.Ceiling((double)alunosResult.total / limit.Value) : 0
                    };

                    return ResponseDefault(retorno);
                }
                return BadRequest(new
                {
                    sucess = false,
                    error = alunosResult.error
                });
            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }

        private async Task<(bool sucess, string error, Dictionary<string, object>? data)> ValidateCommand(InsertUsuarioModel command, Source source)
        {
            var t_pessoa = new Dictionary<string, object>();
            var t_aluno = new Dictionary<string, object>();
            if(command.pessoa.cd_pessoa == 0)
            {
                //valida cpf
                if (!string.IsNullOrEmpty(command.pessoa.nm_cpf))
                {
                    var filtros = new List<(string campo, object valor)> { new("nm_cpf", command.pessoa.nm_cpf) };
                    var cpfExist = await SQLServerService.GetFirstByFields(source, "T_PESSOA_FISICA", filtros);
                    if (cpfExist != null)
                    {
                        //verifica se ja tem aluno cadastrado
                        var cd_pessoa_cpf = cpfExist["cd_pessoa_fisica"];
                        var filtrosAluno = new List<(string campo, object valor)> { new("cd_pessoa_aluno", cd_pessoa_cpf) };
                        var alunoExist = await SQLServerService.GetFirstByFields(source, "T_ALUNO", filtrosAluno);
                        if (alunoExist != null)
                            return (false, $"Já existe um registro com este CPF({command.pessoa.nm_cpf}) cadastrado", null);
                    }
                }

                //valida email
                if (command.pessoa.dc_email != null && !string.IsNullOrEmpty(command.pessoa.dc_email))
                {
                    var filtros = new List<(string campo, object valor)> { new("dc_fone_mail", command.pessoa.dc_email) };
                    var emailExist = await SQLServerService.GetFirstByFields(source, "T_TELEFONE", filtros);
                    if (emailExist != null)
                    {
                        var responsavelFinanceiro = command.relacionamentosAluno?.FirstOrDefault(r => r.cd_papel_pai == 9 && r.cd_papel_filho == 3);
                        if (responsavelFinanceiro == null)
                            return (false, $"Já existe um registro com este e-mail({command.pessoa.dc_email}) cadastrado", null);

                        var filtrosResponsavel = new List<(string campo, object valor)> { new("dc_fone_mail", command.pessoa.dc_email),
                            new("cd_pessoa", responsavelFinanceiro.cd_pessoa_filho)};
                        var exists = await SQLServerService.GetFirstByFields(source, "T_TELEFONE", filtrosResponsavel);
                        if (exists == null)
                            return (false, $"Já existe um registro com este e-mail({command.pessoa.dc_email}) cadastrado", null);

                    }
                }
            }
            

            try
            {
                var pessoaDict = new Dictionary<string, object>
                {
                    //{ "cd_pessoa", null },
                    { "no_pessoa", command.pessoa.no_pessoa },
                    { "dt_cadastramento", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") },
                    { "dc_reduzido_pessoa", command.pessoa.dc_reduzido_pessoa },
                    { "cd_atividade_principal", command.pessoa.cd_atividade_principal != 0 ? command.pessoa.cd_atividade_principal :null },
                    { "cd_endereco_principal", null }, 
                    { "cd_telefone_principal", null },
                    { "id_pessoa_empresa",0 },
                    { "nm_natureza_pessoa",1 },
                    { "dc_num_pessoa", null },
                    { "id_exportado", 0 },
                    { "cd_papel_principal", null },
                    { "txt_obs_pessoa", command.pessoa.txt_obs_pessoa },
                    { "ext_img_pessoa", command.pessoa.ext_img_pessoa}
                };
                if (command.pessoa.img_pessoa != null)
                {
                    var imgBytes = Convert.FromBase64String(command.pessoa.img_pessoa);
                    pessoaDict.Add("img_pessoa", imgBytes);
                }
                if(command.pessoa.cd_pessoa != 0)
                {
                    var t_pessoa_insert = await SQLServerService.UpdateWithResult("T_PESSOA", pessoaDict, source, "cd_pessoa", command.pessoa.cd_pessoa);
                    if (!t_pessoa_insert.success) return new(t_pessoa_insert.success, t_pessoa_insert.error, null);
                    t_pessoa = t_pessoa_insert.updatedData;
                }
                else
                {
                    var t_pessoa_insert = await SQLServerService.InsertWithResult("T_PESSOA", pessoaDict, source);
                    if (!t_pessoa_insert.success) return new(t_pessoa_insert.success, t_pessoa_insert.error, null);
                    t_pessoa = t_pessoa_insert.inserted;

                    //T_PESSOA_EMPRESA
                    var t_pessoa_empresa_dict = new Dictionary<string, object>
                    {
                        { "cd_pessoa", t_pessoa["cd_pessoa"] },
                        { "cd_empresa", command.cd_pessoa_escola }
                    };
                    var t_pessoa_empresa_insert = await SQLServerService.Insert("T_PESSOA_EMPRESA", t_pessoa_empresa_dict, source);
                    if (!t_pessoa_empresa_insert.success) return new(t_pessoa_empresa_insert.success, t_pessoa_empresa_insert.error, null);
                }
                var cd_pessoa = t_pessoa["cd_pessoa"];

                if (command.relacionamentosAluno != null)
                {
                    foreach (var dependente in command.relacionamentosAluno)
                    {
                        var relacionamento_dict = new Dictionary<string, object>
                             {
                                 { "cd_pessoa_filho", dependente.cd_pessoa_filho },
                                 { "cd_pessoa_pai", cd_pessoa },
                                 { "cd_papel_filho", dependente.cd_papel_filho }
                             };

                        if (dependente.cd_papel_pai != null) relacionamento_dict.Add("cd_papel_pai", dependente.cd_papel_pai);

                        if (relacionamento_dict.Any())
                        {
                            var t_relacionamento_insert = await SQLServerService.Insert("T_RELACIONAMENTO", relacionamento_dict, source);
                            if (!t_relacionamento_insert.success)
                            {
                                return (false, $"Erro ao criar relacionamento com pessoa {dependente.cd_pessoa_filho}: {t_relacionamento_insert.error}", null);
                            }
                        }
                    }
                }

                int? cd_telefone_principal = null;
                if (command.pessoa.dc_email != null)
                {
                    //T_TELEFONE(email)
                    var telefoneDictEmail = new Dictionary<string, object>
                    {
                        //{ "cd_telefone", null },
                        { "cd_pessoa", cd_pessoa },
                        { "cd_tipo_telefone", 4 },
                        { "cd_classe_telefone", 1 },
                        { "dc_fone_mail", command.pessoa.dc_email },
                        { "cd_endereco", null },
                        { "id_telefone_principal",1 },
                        { "cd_operadora", null }
                    };
                    var filtrosEmail = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa), new("cd_tipo_telefone", 4) };
                    var emailExists = await SQLServerService.GetFirstByFields(source, "T_Telefone", filtrosEmail);
                    if (emailExists != null)
                    {
                        var cd_telefone = emailExists["cd_telefone"];
                        var t_telefone_email_insert = await SQLServerService.Update("T_TELEFONE", telefoneDictEmail, source, "cd_telefone", cd_telefone);
                        if (!t_telefone_email_insert.success) return new(t_telefone_email_insert.success, t_telefone_email_insert.error, null);
                    }
                    else
                    {
                        var t_telefone_email_insert = await SQLServerService.Insert("T_TELEFONE", telefoneDictEmail, source);
                        if (!t_telefone_email_insert.success) return new(t_telefone_email_insert.success, t_telefone_email_insert.error, null);
                    }
                }

                if (!string.IsNullOrEmpty(command.pessoa.telefone))
                {
                    //T_TELEFONE(telefone)
                    var telefoneDictTelefone = new Dictionary<string, object>
                    {
                        //{ "cd_telefone", null },
                        { "cd_pessoa", cd_pessoa },
                        { "cd_tipo_telefone", 1 },
                        { "cd_classe_telefone", 1 },
                        { "dc_fone_mail", command.pessoa.telefone },
                        { "cd_endereco", null },
                        { "id_telefone_principal", 1 },
                        { "cd_operadora", null }
                    };
                    var filtrosEmail = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa), new("cd_tipo_telefone", 1) };
                    var emailExists = await SQLServerService.GetFirstByFields(source, "T_Telefone", filtrosEmail);
                    if (emailExists != null)
                    {
                        var cd_telefone = emailExists["cd_telefone"];
                        var t_telefone_email_insert = await SQLServerService.Update("T_TELEFONE", telefoneDictTelefone, source, "cd_telefone", cd_telefone);
                        if (!t_telefone_email_insert.success) return new(t_telefone_email_insert.success, t_telefone_email_insert.error, null);
                    }
                    else
                    {
                        var t_telefone_email_insert = await SQLServerService.Insert("T_TELEFONE", telefoneDictTelefone, source);
                        if (!t_telefone_email_insert.success) return new(t_telefone_email_insert.success, t_telefone_email_insert.error, null);
                    }

                    var filtrosTelefone = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa), new("dc_fone_mail", command.pessoa.telefone) };
                    var telefone_cadastrado = await SQLServerService.GetFirstByFields(source, "T_TELEFONE", filtrosTelefone);
                    cd_telefone_principal = Convert.ToInt32(telefone_cadastrado["cd_telefone"]);
                }
                if (!string.IsNullOrEmpty(command.pessoa.celular))
                {
                    var telefoneDictCelular = new Dictionary<string, object>
                    {
                        { "cd_pessoa", cd_pessoa },
                        { "cd_tipo_telefone", 3 },
                        { "cd_classe_telefone", 1 },
                        { "dc_fone_mail", command.pessoa.celular },
                        { "cd_endereco", null },
                        { "id_telefone_principal", 1 },
                        { "cd_operadora", null }
                    };

                    var filtrosEmail = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa), new("cd_tipo_telefone", 3) };
                    var emailExists = await SQLServerService.GetFirstByFields(source, "T_Telefone", filtrosEmail);
                    if (emailExists != null)
                    {
                        var cd_telefone = emailExists["cd_telefone"];
                        var t_telefone_email_insert = await SQLServerService.Update("T_TELEFONE", telefoneDictCelular, source, "cd_telefone", cd_telefone);
                        if (!t_telefone_email_insert.success) return new(t_telefone_email_insert.success, t_telefone_email_insert.error, null);
                    }
                    else
                    {
                        var t_telefone_email_insert = await SQLServerService.Insert("T_TELEFONE", telefoneDictCelular, source);
                        if (!t_telefone_email_insert.success) return new(t_telefone_email_insert.success, t_telefone_email_insert.error, null);
                    }

                    var filtrosTelefone = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa), new("dc_fone_mail", command.pessoa.celular) };
                    var telefone_cadastrado = await SQLServerService.GetFirstByFields(source, "T_TELEFONE", filtrosTelefone);
                    cd_telefone_principal = Convert.ToInt32(telefone_cadastrado["cd_telefone"]);
                }

                int? cd_endereco_principal = null;
                //T_ENDERECO
                var enderecoDict = new Dictionary<string, object>
                {
                    //{ "cd_endereco", command.pessoa.endereco.cd_endereco },
                    { "cd_pessoa", cd_pessoa },
                    { "cd_loc_pais", command.pessoa.endereco.cd_loc_pais },
                    { "cd_loc_estado", command.pessoa.endereco.cd_loc_estado },
                    { "cd_loc_cidade", command.pessoa.endereco.cd_loc_cidade },
                    { "cd_tipo_endereco", command.pessoa.endereco.cd_tipo_endereco},
                    //{ "cd_loc_distrito", null },
                    { "cd_loc_bairro", command.pessoa.endereco.cd_loc_bairro },
                    { "cd_tipo_logradouro",command.pessoa.endereco.cd_tipo_logradouro},
                    { "cd_loc_logradouro", command.pessoa.endereco.cd_loc_logradouro },
                    //{ "nm_caixa_postal", command.pessoa.endereco.nm_caixa_postal },
                    //{ "nm_status_endereco", command.pessoa.endereco.nm_status_endereco },
                    { "dc_compl_endereco", command.pessoa.endereco.dc_compl_endereco },
                    { "id_exportado", 0 },
                    { "dc_num_cep", command.pessoa.endereco.dc_num_cep },
                    { "dc_num_endereco",  command.pessoa.endereco.dc_num_endereco },
                    //{ "dc_num_local_geografico", command.pessoa.endereco.dc_num_local_geografico}
                };
                var t_endereco_insert = await SQLServerService.InsertWithResult("T_ENDERECO", enderecoDict, source);
                if (!t_endereco_insert.success) return new(t_endereco_insert.success, t_endereco_insert.error, null);

                cd_endereco_principal = Convert.ToInt32(t_endereco_insert.inserted["cd_endereco"]);
                //T_PESSOA_FISICA
                var pessoa_fisicaDict = new Dictionary<string, object>
                {
                    { "cd_pessoa_fisica", cd_pessoa },
                    { "cd_estado_civil", command.pessoa.cd_estado_civil},
                    { "cd_loc_nacionalidade", command.pessoa.cd_loc_nacionalidade },
                    { "nm_sexo", command.pessoa.nm_sexo },
                    { "dt_nascimento", command.pessoa.dt_nascimento.ToString("yyyy-MM-ddTHH:mm:ss") },
                    { "id_exportado", 0 },
                    { "nm_cpf", command.pessoa.nm_cpf },
                    { "nm_doc_identidade", command.pessoa.nm_doc_identidade },
                    { "cd_orgao_expedidor", command.pessoa.cd_orgao_expedidor },
                    { "cd_estado_expedidor", command.pessoa.cd_estado_expedidor},
                    { "dt_emis_expedidor", command.pessoa.dt_emis_expedidor?.ToString("yyyy-MM-ddTHH:mm:ss") }
                };
                if (command.pessoa.cd_escolaridade != null) pessoa_fisicaDict.Add("cd_escolaridade", command.pessoa.cd_escolaridade);


                var filtroPessoaFisica = new List<(string campo, object valor)> { new("cd_pessoa_fisica", cd_pessoa) };
                var pfExists = await SQLServerService.GetFirstByFields(source, "T_PESSOA_FISICA", filtroPessoaFisica);
                if (pfExists != null)
                {
                    var t_telefone_email_insert = await SQLServerService.Update("T_PESSOA_FISICA", pessoa_fisicaDict, source, "cd_pessoa_fisica", cd_pessoa);
                    if (!t_telefone_email_insert.success) return new(t_telefone_email_insert.success, t_telefone_email_insert.error, null);
                }
                else
                {
                    var t_pessoa_fisica_insert = await SQLServerService.Insert("T_PESSOA_FISICA", pessoa_fisicaDict, source);
                    if (!t_pessoa_fisica_insert.success) return new(t_pessoa_fisica_insert.success, t_pessoa_fisica_insert.error, null);
                }

                var cd_pessoa_escola = command.cd_pessoa_escola;

                //atualiza cd_telefone_principal e cd_endereco_principal na T_PESSOA
                var pessoaUpdateDict = new Dictionary<string, object>
                {
                    { "cd_telefone_principal", cd_telefone_principal },
                    { "cd_endereco_principal", cd_endereco_principal }
                };
                var t_pessoa_update = await SQLServerService.Update("T_PESSOA", pessoaUpdateDict, source, "cd_pessoa", cd_pessoa);

                if (command.Raf != null)
                {
                    var dict_raf = new Dictionary<string, object?>
                    {
                        { "cd_pessoa", cd_pessoa },
                        { "nm_raf", command.Raf.nm_raf },
                        { "id_raf_liberado", command.Raf.id_raf_liberado },
                        { "nm_tentativa", command.Raf.nm_tentativa },
                        { "id_bloqueado", command.Raf.id_bloqueado },
                        { "id_trocar_senha", command.Raf.id_trocar_senha },
                        { "dt_expiracao_senha", command.Raf.dt_expiracao_senha.ToString("yyyy-MM-ddTHH:mm:ss") },
                        { "dc_senha_raf", command.Raf.dc_senha_raf },
                        { "dt_limite_bloqueio", command.Raf.dt_limite_bloqueio?.ToString("yyyy-MM-ddTHH:mm:ss")}
                    };
                    var t_pessoa_raf_insert = await SQLServerService.Insert("T_PESSOA_RAF", dict_raf, source);
                    if (!t_pessoa_raf_insert.success) return new(t_pessoa_raf_insert.success, t_pessoa_raf_insert.error, null);
                }

                if (cd_pessoa_escola > 0)
                {
                    var cd_atendente =command.cd_usuario_atendente;

                    // T_ALUNO
                    var alunoDict = new Dictionary<string, object>
                    {
                        //{ "cd_aluno", null },
                        { "cd_pessoa_aluno", cd_pessoa },
                        { "cd_pessoa_escola", cd_pessoa_escola },
                        { "cd_midia", command.cd_midia },
                        { "cd_escolaridade", command.pessoa.cd_escolaridade },
                        { "cd_usuario_atendente", cd_atendente },
                        { "id_aluno_ativo", 1 },
                        //{ "cd_prospect", null },
                        { "cd_contato", command.cd_contato }
                    };

                    var t_aluno_insert = await SQLServerService.InsertWithResult("T_ALUNO", alunoDict, source);
                    t_aluno = t_aluno_insert.inserted;
                    var cd_aluno = t_aluno["cd_aluno"];

                    if (command.alunoBolsas != null)
                    {
                        foreach (var bolsa in command.alunoBolsas)
                        {
                            var bolsaDict = new Dictionary<string, object>
                                {
                                    {"cd_produto",bolsa.cd_produto },
                                    { "cd_aluno", cd_aluno },
                                    { "pc_bolsa", bolsa.pc_bolsa??0 },
                                    { "dt_inicio_bolsa", bolsa.dt_inicio_bolsa.ToString("yyyy-MM-ddTHH:mm:ss") },
                                    { "dc_validade_bolsa", bolsa.dc_validade_bolsa },
                                    { "dt_cancelamento_bolsa", bolsa.dt_cancelamento_bolsa?.ToString("yyyy-MM-ddTHH:mm:ss") },
                                    { "cd_motivo_bolsa", bolsa.cd_motivo_bolsa },
                                    { "id_bolsa_material", bolsa.id_bolsa_material },
                                    { "pc_bolsa_material", bolsa.pc_bolsa_material }
                                };
                            if (bolsa.dt_comunicado_bolsa != null) bolsaDict.Add("dt_comunicado_bolsa", bolsa.dt_comunicado_bolsa?.ToString("yyyy-MM-ddTHH:mm:ss"));
                            var t_aluno_bolsa = await SQLServerService.Insert("T_ALUNO_BOLSA", bolsaDict, source);

                            if (!t_aluno_bolsa.success) return new(t_aluno_bolsa.success, t_aluno_bolsa.error, null);
                        }
                    }
                    

                    if (command.motivoMatricula != null && command.motivoMatricula.Count() > 0)
                    {
                        //T_ALUNO_MOTIVO_MATRICULA
                        foreach (var motivo in command.motivoMatricula)
                        {
                            var alunoMotivoMatriculaDict = new Dictionary<string, object>
                            {
                                { "cd_aluno", cd_aluno },
                                { "cd_motivo_matricula", motivo.cd_motivo_matricula },
                            };

                            var t_aluno_motivo_insert = await SQLServerService.InsertWithResult("T_ALUNO_MOTIVO_MATRICULA", alunoMotivoMatriculaDict, source);

                            if (!t_aluno_motivo_insert.success) return new(t_aluno_motivo_insert.success, t_aluno_motivo_insert.error, null);
                        }

                        if (command.horarios != null && command.horarios.gridHorario != null)
                        {
                            //T_HORARIO
                            foreach (var h in command.horarios.gridHorario)
                            {
                                var horarioDict = new Dictionary<string, object>
                            {
                                //{ "cd_horario", null },
                                { "cd_pessoa_escola", command.cd_pessoa_escola },
                                { "cd_registro",cd_aluno },
                                { "id_origem", h.id_origem },
                                { "id_disponivel", h.id_disponivel },
                                { "id_dia_semana", h.id_dia_semana },
                                { "dt_hora_ini", h.dt_hora_ini },
                                { "dt_hora_fim", h.dt_hora_fim }
                            };
                                var t_horario_insert = await SQLServerService.Insert("T_HORARIO", horarioDict, source);
                                if (!t_horario_insert.success) return new(t_horario_insert.success, t_horario_insert.error, null);
                            }
                        }
                    }

                    if (command.restricoes != null && command.restricoes.Count() > 0)
                    {
                        foreach (var restricao in command.restricoes)
                        {
                            var dict = new Dictionary<string, object>
                                {
                                    { "cd_aluno", cd_aluno },
                                    { "cd_orgao_financeiro", restricao.cd_orgao_financeiro },
                                    { "cd_usuario", cd_atendente },
                                    { "dt_inicio_restricao", restricao.dt_inicio_restricao.ToString("yyyy-MM-ddTHH:mm:ss")},
                                    { "dt_final_restricao", restricao.dt_fim_restricao?.ToString("yyyy-MM-ddTHH:mm:ss") },
                                    { "dt_cadastro", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") }
                                };
                            var t_restricao = await SQLServerService.Insert("T_ALUNO_RESTRICAO", dict, source);

                            if (!t_restricao.success) return new(t_restricao.success, t_restricao.error, null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, $"Erro: {ex.Message}", null);
            }
            var data = new Dictionary<string, object>
            {
                {"cd_aluno", t_aluno["cd_aluno"] },
                {"cd_pessoa", t_pessoa["cd_pessoa"] },
                {"no_pessoa", t_pessoa["no_pessoa"] },
                {"nm_cpf", command.pessoa.nm_cpf ?? string.Empty },
            };
            return (true, string.Empty, data);
        }

        private async Task<(bool sucess, string error)> ProcessaUpdate(int cd_pessoa, InsertUsuarioModel command, Source source)
        {
            //valida cpf
            if (command.pessoa.nm_cpf != null && !string.IsNullOrEmpty(command.pessoa.nm_cpf))
            {
                var filtros = new List<(string campo, object valor)> { new("nm_cpf", command.pessoa.nm_cpf) };
                var cpfExist = await SQLServerService.GetFirstByFields(source, "T_PESSOA_FISICA", filtros);
                if (cpfExist != null)
                {
                    var cd_pessoa_cpf = cpfExist["cd_pessoa_fisica"];
                    if (cd_pessoa_cpf != null && cd_pessoa_cpf.ToString() != cd_pessoa.ToString())
                    {
                        return (false, $"Já existe um registro com este CPF({command.pessoa.nm_cpf}) cadastrado");
                    }
                }
            }

            //valida email
            if (command.pessoa.dc_email != null && !string.IsNullOrEmpty(command.pessoa.dc_email))
            {
                var filtros = new List<(string campo, object valor)> { new("dc_fone_mail", command.pessoa.dc_email) };
                var emailExist = await SQLServerService.GetFirstByFields(source, "T_TELEFONE", filtros);
                if (emailExist != null && emailExist["cd_pessoa"].ToString() != cd_pessoa.ToString())
                {
                    // Busca todos os responsáveis financeiros (papel_filho=3) que terão o mesmo email
                    // A regra de negócio permite que aluno e responsável financeiro compartilhem o mesmo email
                    // Verifica tanto nos relacionamentos ATUAIS (banco) quanto nos NOVOS (payload)

                    var cd_pessoa_email_duplicado = Convert.ToInt32(emailExist["cd_pessoa"]);

                    // Busca relacionamentos ATUAIS no banco onde cd_pessoa_pai = aluno atual
                    var relacionamentosAtuaisBanco = await SQLServerService.GetList(
                        "T_RELACIONAMENTO", null, "[cd_pessoa_pai]", $"[{cd_pessoa}]", source, SearchModeEnum.Equals);

                    // Filtra apenas responsáveis financeiros (cd_papel_filho = 3)
                    var responsaveisAtuais = relacionamentosAtuaisBanco.data
                        ?.Where(r => {
                            var papel_filho = r.ContainsKey("cd_papel_filho") && r["cd_papel_filho"] != null
                                ? Convert.ToInt32(r["cd_papel_filho"])
                                : 0;
                            return papel_filho == 3;
                        })
                        .Select(r => Convert.ToInt32(r["cd_pessoa_filho"]))
                        .ToList() ?? new List<int>();

                    // Busca responsáveis financeiros no payload (cd_papel_filho = 3)
                    var responsaveisNovos = command.relacionamentosAluno
                        ?.Where(r => r.cd_papel_filho == 3)
                        .Select(r => r.cd_pessoa_filho)
                        .ToList() ?? new List<int>();

                    // Combina: considera responsáveis que existem ATUALMENTE ou que EXISTIRÃO após o update
                    var todosResponsaveis = responsaveisAtuais.Union(responsaveisNovos).Distinct().ToList();

                    // Se o email duplicado pertence a algum responsável financeiro (atual ou futuro), permite
                    if (!todosResponsaveis.Contains(cd_pessoa_email_duplicado))
                    {
                        return (false, $"Já existe um registro com este e-mail({command.pessoa.dc_email}) cadastrado");
                    }
                }
            }

            try
            {
                var pessoaDict = new Dictionary<string, object>
                {
                    { "no_pessoa", command.pessoa.no_pessoa },
                    { "dt_cadastramento", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") },
                    { "dc_reduzido_pessoa", command.pessoa.dc_reduzido_pessoa },
                    { "cd_atividade_principal", command.pessoa.cd_atividade_principal != 0 ? command.pessoa.cd_atividade_principal :null },
                    { "txt_obs_pessoa", command.pessoa.txt_obs_pessoa },
                    { "ext_img_pessoa", command.pessoa.ext_img_pessoa}
                };
                if (command.pessoa.img_pessoa != null)
                {
                    var imgBytes = Convert.FromBase64String(command.pessoa.img_pessoa);
                    pessoaDict.Add("img_pessoa", imgBytes);
                }
                var t_pessoa_insert = await SQLServerService.Update("T_PESSOA", pessoaDict, source, "cd_pessoa", cd_pessoa);
                if (!t_pessoa_insert.success) return new(t_pessoa_insert.success, t_pessoa_insert.error);

                if (command.pessoa.dc_email != null)
                {
                    var filtrosEmail = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa), new("cd_tipo_telefone", 4) };
                    var emailExists = await SQLServerService.GetFirstByFields(source, "T_Telefone", filtrosEmail);
                    var telefoneDictEmail = new Dictionary<string, object>
                        {
                            { "cd_pessoa", cd_pessoa },
                            { "cd_tipo_telefone", 4 },
                            { "cd_classe_telefone", 1 },
                            { "dc_fone_mail", command.pessoa.dc_email },
                            { "cd_endereco", null },
                            { "id_telefone_principal",1 },
                            { "cd_operadora", null }
                        };
                    if (emailExists != null)
                    {
                        var cd_telefone = emailExists["cd_telefone"];
                        var t_telefone_email_insert = await SQLServerService.Update("T_TELEFONE", telefoneDictEmail, source, "cd_telefone", cd_telefone);
                        if (!t_telefone_email_insert.success) return new(t_telefone_email_insert.success, t_telefone_email_insert.error);
                    }
                    else
                    {
                        var t_telefone_email_insert = await SQLServerService.Insert("T_TELEFONE", telefoneDictEmail, source);
                        if (!t_telefone_email_insert.success) return new(t_telefone_email_insert.success, t_telefone_email_insert.error);
                    }
                }

                if (command.relacionamentosAluno != null)
                {
                    // ESTRATÉGIA: Não deletar todos e recriar, mas sim fazer MERGE (delete apenas removidos, insert apenas novos)
                    // Isso evita trigger que impede exclusão quando aluno e responsável têm o mesmo e-mail

                    // Busca os relacionamentos atuais do aluno
                    var relacionamentosAtuais = await SQLServerService.GetList(
                        "T_RELACIONAMENTO", null, "[cd_pessoa_pai]", $"[{cd_pessoa}]", source, SearchModeEnum.Equals);

                    // Cria listas para comparar
                    var atuais = relacionamentosAtuais.data
                        .Select(r => new {
                            cd_relacionamento = Convert.ToInt32(r["cd_relacionamento"]),
                            cd_pessoa_filho = Convert.ToInt32(r["cd_pessoa_filho"]),
                            cd_papel_filho = Convert.ToInt32(r["cd_papel_filho"]),
                            cd_papel_pai = r.ContainsKey("cd_papel_pai") && r["cd_papel_pai"] != null
                                ? (int?)Convert.ToInt32(r["cd_papel_pai"])
                                : null
                        })
                        .ToList();

                    var novos = command.relacionamentosAluno
                        .Select(r => new {
                            cd_pessoa_filho = r.cd_pessoa_filho,
                            cd_papel_filho = r.cd_papel_filho,
                            cd_papel_pai = r.cd_papel_pai
                        })
                        .ToList();

                    // Identifica relacionamentos a REMOVER (existem em atuais mas não em novos)
                    var paraRemover = atuais
                        .Where(a => !novos.Any(n =>
                            n.cd_pessoa_filho == a.cd_pessoa_filho &&
                            n.cd_papel_filho == a.cd_papel_filho &&
                            n.cd_papel_pai == a.cd_papel_pai))
                        .ToList();

                    // Identifica relacionamentos a ADICIONAR (existem em novos mas não em atuais)
                    var paraAdicionar = novos
                        .Where(n => !atuais.Any(a =>
                            n.cd_pessoa_filho == a.cd_pessoa_filho &&
                            n.cd_papel_filho == a.cd_papel_filho &&
                            n.cd_papel_pai == a.cd_papel_pai))
                        .ToList();

                    // REMOVE apenas os relacionamentos que foram excluídos do payload
                    foreach (var rel in paraRemover)
                    {
                        var deleteRelacionamento = await SQLServerService.Delete("T_RELACIONAMENTO", "cd_relacionamento", rel.cd_relacionamento.ToString(), source);
                        if (!deleteRelacionamento.success)
                        {
                            return new(deleteRelacionamento.success, $"Erro ao excluir relacionamento com pessoa {rel.cd_pessoa_filho}: {deleteRelacionamento.error}");
                        }
                    }

                    // ADICIONA apenas os novos relacionamentos
                    foreach (var rel in paraAdicionar)
                    {
                        var relacionamento_dict = new Dictionary<string, object>
                        {
                            { "cd_pessoa_filho", rel.cd_pessoa_filho },
                            { "cd_pessoa_pai", cd_pessoa },
                            { "cd_papel_filho", rel.cd_papel_filho }
                        };
                        if (rel.cd_papel_pai != null) relacionamento_dict.Add("cd_papel_pai", rel.cd_papel_pai);

                        var t_relacionamento_insert = await SQLServerService.Insert("T_RELACIONAMENTO", relacionamento_dict, source);
                        if (!t_relacionamento_insert.success)
                        {
                            return new(t_relacionamento_insert.success, $"Erro ao criar relacionamento com pessoa {rel.cd_pessoa_filho}: {t_relacionamento_insert.error}");
                        }
                    }
                }

                if (command.Raf != null)
                {
                    var dict_raf = new Dictionary<string, object?>
                    {
                        { "cd_pessoa", cd_pessoa },
                        { "nm_raf", command.Raf.nm_raf },
                        { "id_raf_liberado", command.Raf.id_raf_liberado },
                        { "nm_tentativa", command.Raf.nm_tentativa },
                        { "id_bloqueado", command.Raf.id_bloqueado },
                        { "id_trocar_senha", command.Raf.id_trocar_senha },
                        { "dt_expiracao_senha", command.Raf.dt_expiracao_senha.ToString("yyyy-MM-ddTHH:mm:ss") },
                        { "dc_senha_raf", command.Raf.dc_senha_raf },
                        { "dt_limite_bloqueio", command.Raf.dt_limite_bloqueio?.ToString("yyyy-MM-ddTHH:mm:ss") }
                    };
                    var filtrosRaf = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa) };
                    var pessoaRaf = await SQLServerService.GetFirstByFields(source, "T_PESSOA_RAF", filtrosRaf);
                    if (pessoaRaf != null)
                    {
                        var cd_pessoa_raf = pessoaRaf["cd_pessoa_raf"];

                        var t_pessoa_raf_update = await SQLServerService.Update("T_PESSOA_RAF", dict_raf, source, "cd_pessoa_raf", cd_pessoa_raf);
                        if (!t_pessoa_raf_update.success) return new(t_pessoa_raf_update.success, t_pessoa_raf_update.error);
                    }
                    else
                    {
                        var t_pessoa_raf_insert = await SQLServerService.Insert("T_PESSOA_RAF", dict_raf, source);
                        if (!t_pessoa_raf_insert.success) return new(t_pessoa_raf_insert.success, t_pessoa_raf_insert.error);
                    }
                }

                if (!string.IsNullOrEmpty(command.pessoa.telefone))
                {
                    var filtrosTelefone = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa), new("cd_tipo_telefone", 1) };
                    var telefoneExists = await SQLServerService.GetFirstByFields(source, "T_Telefone", filtrosTelefone);
                    var telefoneDictTelefone = new Dictionary<string, object>
                        {
                            { "cd_pessoa", cd_pessoa },
                            { "cd_tipo_telefone", 1 },
                            { "cd_classe_telefone", 1 },
                            { "dc_fone_mail", command.pessoa.telefone },
                            { "cd_endereco", null },
                            { "id_telefone_principal", 1 },
                            { "cd_operadora", null }
                        };
                    if (telefoneExists != null)
                    {
                        var cd_telefone = telefoneExists["cd_telefone"];

                        var t_telefone_telefone_insert = await SQLServerService.Update("T_TELEFONE", telefoneDictTelefone, source, "cd_telefone", cd_telefone);
                        if (!t_telefone_telefone_insert.success) return new(t_telefone_telefone_insert.success, t_telefone_telefone_insert.error);
                    }
                    else
                    {
                        var t_telefone_telefone_insert = await SQLServerService.Insert("T_TELEFONE", telefoneDictTelefone, source);
                        if (!t_telefone_telefone_insert.success) return new(t_telefone_telefone_insert.success, t_telefone_telefone_insert.error);
                    }
                }

                if (!string.IsNullOrEmpty(command.pessoa.celular))
                {
                    var filtrosCelular = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa), new("cd_tipo_telefone", 3) };
                    var celularExists = await SQLServerService.GetFirstByFields(source, "T_Telefone", filtrosCelular);
                    var telefoneDictCelular = new Dictionary<string, object>
                    {
                        { "cd_pessoa", cd_pessoa },
                        { "cd_tipo_telefone", 3 },
                        { "cd_classe_telefone", 1 },
                        { "dc_fone_mail", command.pessoa.celular },
                        { "cd_endereco", null },
                        { "id_telefone_principal", 1 },
                        { "cd_operadora", null }
                    };
                    if (celularExists != null)
                    {
                        var cd_telefone = celularExists["cd_telefone"];
                        var t_telefone_celular_insert = await SQLServerService.Update("T_TELEFONE", telefoneDictCelular, source, "cd_telefone", cd_telefone);
                        if (!t_telefone_celular_insert.success) return new(t_telefone_celular_insert.success, t_telefone_celular_insert.error);
                    }
                    else
                    {
                        var t_telefone_celular_insert = await SQLServerService.Insert("T_TELEFONE", telefoneDictCelular, source);
                        if (!t_telefone_celular_insert.success) return new(t_telefone_celular_insert.success, t_telefone_celular_insert.error);
                    }
                }

                if (command.pessoa.endereco != null)
                {
                    //T_ENDERECO
                    var filtrosEndereco = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa) };
                    var enderecoExists = await SQLServerService.GetFirstByFields(source, "T_ENDERECO", filtrosEndereco);
                    if (enderecoExists != null)
                    {
                        //T_ENDERECO
                        var enderecoDict = new Dictionary<string, object>
                        {
                            { "cd_pessoa", cd_pessoa },
                            { "cd_loc_pais", command.pessoa.endereco.cd_loc_pais },
                            { "cd_loc_estado", command.pessoa.endereco.cd_loc_estado },
                            { "cd_loc_cidade", command.pessoa.endereco.cd_loc_cidade },
                            { "cd_tipo_endereco", command.pessoa.endereco.cd_tipo_endereco},
                            { "cd_loc_bairro", command.pessoa.endereco.cd_loc_bairro },
                            { "cd_tipo_logradouro",command.pessoa.endereco.cd_tipo_logradouro},
                            { "cd_loc_logradouro", command.pessoa.endereco.cd_loc_logradouro },
                            { "dc_compl_endereco", command.pessoa.endereco.dc_compl_endereco },
                            { "id_exportado", 0 },
                            { "dc_num_cep", command.pessoa.endereco.dc_num_cep },
                            { "dc_num_endereco",  command.pessoa.endereco.dc_num_endereco },
                        };
                        var t_endereco_insert = await SQLServerService.Update("T_ENDERECO", enderecoDict, source, "cd_endereco", enderecoExists["cd_endereco"]);
                        if (!t_endereco_insert.success) return new(t_endereco_insert.success, t_endereco_insert.error);
                    }
                    else
                    {
                        var enderecoDict = new Dictionary<string, object>
                        {
                            { "cd_pessoa", cd_pessoa },
                            { "cd_loc_pais", command.pessoa.endereco.cd_loc_pais },
                            { "cd_loc_estado", command.pessoa.endereco.cd_loc_estado },
                            { "cd_loc_cidade", command.pessoa.endereco.cd_loc_cidade },
                            { "cd_tipo_endereco", command.pessoa.endereco.cd_tipo_endereco},
                            { "cd_loc_bairro", command.pessoa.endereco.cd_loc_bairro },
                            { "cd_tipo_logradouro",command.pessoa.endereco.cd_tipo_logradouro},
                            { "cd_loc_logradouro", command.pessoa.endereco.cd_loc_logradouro },
                            { "dc_compl_endereco", command.pessoa.endereco.dc_compl_endereco },
                            { "id_exportado", 0 },
                            { "dc_num_cep", command.pessoa.endereco.dc_num_cep },
                            { "dc_num_endereco",  command.pessoa.endereco.dc_num_endereco },
                        };
                        var t_endereco_insert = await SQLServerService.Insert("T_ENDERECO", enderecoDict, source);
                        if (!t_endereco_insert.success) return new(t_endereco_insert.success, t_endereco_insert.error);
                    }
                }

                var filtrosPessoaFisica = new List<(string campo, object valor)> { new("cd_pessoa_fisica", cd_pessoa) };
                var pessoaFisicaExists = await SQLServerService.GetFirstByFields(source, "T_PESSOA_FISICA", filtrosPessoaFisica);
                if (pessoaFisicaExists != null)
                {
                    //T_PESSOA_FISICA
                    var pessoa_fisicaDict = new Dictionary<string, object>
                    {
                        { "cd_estado_civil", command.pessoa.cd_estado_civil},
                        { "cd_loc_nacionalidade", command.pessoa.cd_loc_nacionalidade },
                        { "nm_sexo", command.pessoa.nm_sexo },
                        { "dt_nascimento", command.pessoa.dt_nascimento.ToString("yyyy-MM-ddTHH:mm:ss") },
                        { "id_exportado", 0 },
                        { "nm_cpf", command.pessoa.nm_cpf },
                        { "nm_doc_identidade", command.pessoa.nm_doc_identidade },
                        { "cd_orgao_expedidor", command.pessoa.cd_orgao_expedidor },
                        { "cd_estado_expedidor", command.pessoa.cd_estado_expedidor},
                        { "dt_emis_expedidor", command.pessoa.dt_emis_expedidor?.ToString("yyyy-MM-ddTHH:mm:ss")}
                    };
                    if (command.pessoa.cd_escolaridade != null) pessoa_fisicaDict.Add("cd_escolaridade", command.pessoa.cd_escolaridade);

                    var t_pessoa_fisica_insert = await SQLServerService.Update("T_PESSOA_FISICA", pessoa_fisicaDict, source, "cd_pessoa_fisica", cd_pessoa);
                    if (!t_pessoa_fisica_insert.success) return new(t_pessoa_fisica_insert.success, t_pessoa_fisica_insert.error);
                }

                var cd_pessoa_escola = command.cd_pessoa_escola;

                if (cd_pessoa_escola > 0)
                {   // T_ALUNO
                    var filtrosAluno = new List<(string campo, object valor)> { new("cd_pessoa_aluno", cd_pessoa), new("cd_pessoa_escola", cd_pessoa_escola) };
                    var alunoExists = await SQLServerService.GetFirstByFields(source, "T_ALUNO", filtrosAluno);
                    if (alunoExists != null)
                    {
                        var cd_usuario = command.cd_usuario_atendente;

                        var cd_aluno = int.Parse(alunoExists["cd_aluno"].ToString());
                        var alunoDict = new Dictionary<string, object>
                        {
                            { "cd_pessoa_aluno", cd_pessoa },
                            { "cd_pessoa_escola", cd_pessoa_escola },
                            { "cd_midia", command.cd_midia },
                            { "cd_escolaridade", command.pessoa.cd_escolaridade },
                            { "cd_usuario_atendente", cd_usuario },
                            { "id_aluno_ativo", 1 },
                            { "cd_contato", command.cd_contato }
                        };

                        var t_aluno_insert = await SQLServerService.Update("T_ALUNO", alunoDict, source, "cd_aluno", alunoExists["cd_aluno"]);
                        if (!t_aluno_insert.success) return new(t_aluno_insert.success, t_aluno_insert.error);

                        if (command.motivoMatricula != null && command.motivoMatricula.Count() > 0)
                        {
                            //remover todas os Motivo_matricula
                            await SQLServerService.Delete("T_ALUNO_MOTIVO_MATRICULA", "cd_aluno", cd_aluno.ToString(), source);
                            //T_ALUNO_MOTIVO_MATRICULA
                            foreach (var motivo in command.motivoMatricula)
                            {
                                var alunoMotivoMatriculaDict = new Dictionary<string, object>
                                {
                                    { "cd_aluno", cd_aluno },
                                    { "cd_motivo_matricula", motivo.cd_motivo_matricula },
                                };

                                var t_aluno_motivo_insert = await SQLServerService.InsertWithResult("T_ALUNO_MOTIVO_MATRICULA", alunoMotivoMatriculaDict, source);

                                if (!t_aluno_motivo_insert.success) return new(t_aluno_motivo_insert.success, t_aluno_motivo_insert.error);
                            }
                        }

                        if (command.horarios != null && command.horarios.gridHorario != null)
                        {
                            // Deletar horários de Aluno do banco
                            // Observação: alguns horários antigos podem ter id_origem = 0, então deletamos ambos (0 e 10)
                            // Não deletamos horários de Turma (id_origem = 19) para evitar erro da trigger

                            // Deletar horários com id_origem = 0 (horários antigos de aluno)
                            var resDeleteHorario0 = await SQLServerService.DeleteByTwoFields("T_HORARIO", "cd_registro", cd_aluno.ToString(), "id_origem", "0", source);
                            // Deletar horários com id_origem = 10 (horários de aluno)
                            var resDeleteHorario10 = await SQLServerService.DeleteByTwoFields("T_HORARIO", "cd_registro", cd_aluno.ToString(), "id_origem", "10", source);

                            // Se algum delete falhou (não por falta de registros), retornar erro
                            if (!resDeleteHorario0.success && (resDeleteHorario0.error == null || !resDeleteHorario0.error.Contains("No records")))
                                return resDeleteHorario0;
                            if (!resDeleteHorario10.success && (resDeleteHorario10.error == null || !resDeleteHorario10.error.Contains("No records")))
                                return resDeleteHorario10;

                            //T_HORARIO - Inserir novos horários
                            foreach (var h in command.horarios.gridHorario)
                            {
                                var horarioDict = new Dictionary<string, object>
                            {
                                //{ "cd_horario", null },
                                { "cd_pessoa_escola", command.cd_pessoa_escola },
                                { "cd_registro",cd_aluno },
                                { "id_origem", h.id_origem != 0 ? h.id_origem : 10 }, // Se vier 0, corrigir para 10
                                { "id_disponivel", h.id_disponivel },
                                { "id_dia_semana", h.id_dia_semana },
                                { "dt_hora_ini", h.dt_hora_ini },
                                { "dt_hora_fim", h.dt_hora_fim }
                            };
                                var t_horario_insert = await SQLServerService.Insert("T_HORARIO", horarioDict, source);
                                if (!t_horario_insert.success) return new(t_horario_insert.success, t_horario_insert.error);
                            }
                        }

                        if (command.alunoBolsas != null)
                        {
                            //remover todas as bolsas
                            var resDeleteAlunoBolsa = await SQLServerService.Delete("T_ALUNO_BOLSA", "cd_aluno", cd_aluno.ToString(), source);
                            if (!resDeleteAlunoBolsa.success) return resDeleteAlunoBolsa;

                            foreach (var bolsa in command.alunoBolsas)
                            {
                                var bolsaDict = new Dictionary<string, object>
                                {
                                    {"cd_produto",bolsa.cd_produto },
                                    { "cd_aluno", cd_aluno },
                                    { "pc_bolsa", bolsa.pc_bolsa ??0},
                                    { "dt_inicio_bolsa", bolsa.dt_inicio_bolsa.ToString("yyyy-MM-ddTHH:mm:ss") },
                                    { "dc_validade_bolsa", bolsa.dc_validade_bolsa },
                                    { "dt_cancelamento_bolsa", bolsa.dt_cancelamento_bolsa?.ToString("yyyy-MM-ddTHH:mm:ss") },
                                    { "cd_motivo_bolsa", bolsa.cd_motivo_bolsa },
                                    { "id_bolsa_material", bolsa.id_bolsa_material },
                                    { "pc_bolsa_material", bolsa.pc_bolsa_material }
                                };
                                if (bolsa.dt_comunicado_bolsa != null) bolsaDict.Add("dt_comunicado_bolsa", bolsa.dt_comunicado_bolsa?.ToString("yyyy-MM-ddTHH:mm:ss"));
                                var t_aluno_bolsa = await SQLServerService.Insert("T_ALUNO_BOLSA", bolsaDict, source);

                                if (!t_aluno_bolsa.success) return new(t_aluno_bolsa.success, t_aluno_bolsa.error);
                            }
                        }

                        if (command.restricoes != null && command.restricoes.Count() > 0)
                        {
                            var resDeleteAlunoRestricao = await SQLServerService.Delete("T_ALUNO_RESTRICAO", "cd_aluno", cd_aluno.ToString(), source);
                            if (!resDeleteAlunoRestricao.success) return resDeleteAlunoRestricao;
                            foreach (var restricao in command.restricoes)
                            {
                                var dict = new Dictionary<string, object>
                                {
                                    { "cd_aluno", cd_aluno },
                                    { "cd_orgao_financeiro", restricao.cd_orgao_financeiro },
                                    { "cd_usuario", cd_usuario },
                                    { "dt_inicio_restricao", restricao.dt_inicio_restricao.ToString("yyyy-MM-ddTHH:mm:ss")},
                                    { "dt_final_restricao", restricao.dt_fim_restricao?.ToString("yyyy-MM-ddTHH:mm:ss")},
                                    { "dt_cadastro", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") }
                                };
                                var t_restricao = await SQLServerService.Insert("T_ALUNO_RESTRICAO", dict, source);

                                if (!t_restricao.success) return new(t_restricao.success, t_restricao.error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += $"\r\n{ex.InnerException.Message}";
                }

                // Log completo para debug
                Console.WriteLine($"[ProcessaUpdate] Erro completo: {ex.ToString()}");

                return (false, $"Erro: {errorMessage}");
            }

            return (true, string.Empty);
        }

        [Authorize]
        [HttpGet()]
        [Route("GetLogradouro")]
        public async Task<IActionResult> GetLogradouro(string cep)
        {
            try
            {
                // Console.WriteLine($"[GetLogradouro] Iniciando busca para CEP: {cep}");

                if (cep.IsNullOrEmpty())
                {
                    // Console.WriteLine("[GetLogradouro] CEP inválido ou vazio");
                    return BadRequest("cep invalido");
                }

                // Console.WriteLine("[GetLogradouro] Buscando schema T_Localidade...");
                var schemaName = "T_Localidade";
                if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");

                var schema = _schemaRepository.GetSchemaByField("name", schemaName);
                if (schema == null)
                {
                    // Console.WriteLine("[GetLogradouro] ERRO: Schema não encontrado!");
                    return BadRequest("Schema não encontrado");
                }
                // Console.WriteLine($"[GetLogradouro] Schema encontrado: {schema.Name}");

                // Console.WriteLine("[GetLogradouro] Deserializando schema model...");
                var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
                if (schemaModel == null)
                {
                    // Console.WriteLine("[GetLogradouro] ERRO: Falha ao deserializar schema model!");
                    return BadRequest("Erro ao deserializar schema");
                }
                // Console.WriteLine($"[GetLogradouro] Schema model source: {schemaModel.Source}");

                // Console.WriteLine("[GetLogradouro] Buscando source...");
                var source = _sourceRepository.GetByField("description", schemaModel.Source);
                if (source == null)
                {
                    // Console.WriteLine("[GetLogradouro] ERRO: Source não encontrado!");
                    return BadRequest("Source não encontrado");
                }
                // Console.WriteLine($"[GetLogradouro] Source encontrado: {source.Description}, Active: {source.Active}");

                if (source != null && source.Active != null && source.Active == true)
                {
                    // Console.WriteLine("[GetLogradouro] Formatando CEP...");
                    var cepTratado = cep.Replace(".", "").Replace("-", "");
                    // Console.WriteLine($"[GetLogradouro] CEP sem pontuação: {cepTratado} (Length: {cepTratado.Length})");

                    if (cepTratado.Length == 8)
                    {
                        cepTratado = cepTratado.Insert(5, "-");
                        // Console.WriteLine($"[GetLogradouro] CEP formatado: {cepTratado}");
                    }

                    // Console.WriteLine("[GetLogradouro] Buscando logradouro no banco...");
                    var filtrosCep = new List<(string campo, object valor)> { new("dc_num_cep", cepTratado) };
                    var logradouro = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtrosCep);

                    if (logradouro == null)
                    {
                        // Console.WriteLine("[GetLogradouro] Logradouro não encontrado para o CEP");
                        return NotFound("cep");
                    }
                    // Console.WriteLine($"[GetLogradouro] Logradouro encontrado: cd_localidade={logradouro.GetValueOrDefault("cd_localidade")}, no_localidade={logradouro.GetValueOrDefault("no_localidade")}, cd_loc_relacionada={logradouro.GetValueOrDefault("cd_loc_relacionada")}");

                    // Busca a hierarquia: bairro -> cidade -> estado -> país
                    // Console.WriteLine("[GetLogradouro] Buscando bairro...");
                    var cdLocRelacionadaBairro = logradouro.GetValueOrDefault("cd_loc_relacionada");
                    // Console.WriteLine($"[GetLogradouro] cd_loc_relacionada do logradouro: {cdLocRelacionadaBairro}");

                    Dictionary<string, object> bairro = null;
                    if (cdLocRelacionadaBairro != null)
                    {
                        bairro = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE",
                            new List<(string campo, object valor)> { new("cd_localidade", cdLocRelacionadaBairro) });
                        // Console.WriteLine($"[GetLogradouro] Bairro encontrado: {(bairro != null ? $"cd_localidade={bairro.GetValueOrDefault("cd_localidade")}, no_localidade={bairro.GetValueOrDefault("no_localidade")}" : "NULL")}");
                    }
                    // else
                    // {
                    //     Console.WriteLine("[GetLogradouro] cd_loc_relacionada do logradouro é NULL, pulando busca do bairro");
                    // }

                    // Console.WriteLine("[GetLogradouro] Buscando cidade...");
                    Dictionary<string, object> cidade = null;
                    if (bairro != null && bairro.ContainsKey("cd_loc_relacionada") && bairro["cd_loc_relacionada"] != null)
                    {
                        // Console.WriteLine($"[GetLogradouro] cd_loc_relacionada do bairro: {bairro["cd_loc_relacionada"]}");
                        cidade = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE",
                            new List<(string campo, object valor)> { new("cd_localidade", bairro["cd_loc_relacionada"]) });
                        // Console.WriteLine($"[GetLogradouro] Cidade encontrada: {(cidade != null ? $"cd_localidade={cidade.GetValueOrDefault("cd_localidade")}, no_localidade={cidade.GetValueOrDefault("no_localidade")}" : "NULL")}");
                    }
                    // else
                    // {
                    //     Console.WriteLine("[GetLogradouro] Bairro NULL ou sem cd_loc_relacionada, pulando busca da cidade");
                    // }

                    // Console.WriteLine("[GetLogradouro] Buscando estado...");
                    Dictionary<string, object> estado = null;
                    if (cidade != null && cidade.ContainsKey("cd_loc_relacionada") && cidade["cd_loc_relacionada"] != null)
                    {
                        // Console.WriteLine($"[GetLogradouro] cd_loc_relacionada da cidade: {cidade["cd_loc_relacionada"]}");
                        estado = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE",
                            new List<(string campo, object valor)> { new("cd_localidade", cidade["cd_loc_relacionada"]) });
                        // Console.WriteLine($"[GetLogradouro] Estado encontrado: {(estado != null ? $"cd_localidade={estado.GetValueOrDefault("cd_localidade")}, no_localidade={estado.GetValueOrDefault("no_localidade")}" : "NULL")}");
                    }
                    // else
                    // {
                    //     Console.WriteLine("[GetLogradouro] Cidade NULL ou sem cd_loc_relacionada, pulando busca do estado");
                    // }

                    // Console.WriteLine("[GetLogradouro] Buscando país...");
                    Dictionary<string, object> pais = null;
                    if (estado != null && estado.ContainsKey("cd_loc_relacionada") && estado["cd_loc_relacionada"] != null)
                    {
                        // Console.WriteLine($"[GetLogradouro] cd_loc_relacionada do estado: {estado["cd_loc_relacionada"]}");
                        pais = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE",
                            new List<(string campo, object valor)> { new("cd_localidade", estado["cd_loc_relacionada"]) });
                        // Console.WriteLine($"[GetLogradouro] País encontrado: {(pais != null ? $"cd_localidade={pais.GetValueOrDefault("cd_localidade")}, no_localidade={pais.GetValueOrDefault("no_localidade")}" : "NULL")}");
                    }
                    // else
                    // {
                    //     Console.WriteLine("[GetLogradouro] Estado NULL ou sem cd_loc_relacionada, pulando busca do país");
                    // }

                    // Console.WriteLine("[GetLogradouro] Montando retorno...");
                    var retorno = new
                    {
                        cd_bairro = bairro?.GetValueOrDefault("cd_localidade"),
                        cd_cidade = cidade?.GetValueOrDefault("cd_localidade"),
                        cd_estado = estado?.GetValueOrDefault("cd_localidade"),
                        cd_logradouro = logradouro.GetValueOrDefault("cd_localidade"),
                        cd_pais = pais?.GetValueOrDefault("cd_localidade"),
                        cd_tipo_logradouro = logradouro.GetValueOrDefault("cd_tipo_localidade"),
                        dc_num_cep = cepTratado,
                        no_bairro = bairro?.GetValueOrDefault("no_localidade"),
                        no_cidade = cidade?.GetValueOrDefault("no_localidade"),
                        no_estado = estado?.GetValueOrDefault("no_localidade"),
                        no_logradouro = logradouro.GetValueOrDefault("no_localidade"),
                        no_pais = pais?.GetValueOrDefault("no_localidade"),
                        count = 1
                    };

                    // Console.WriteLine($"[GetLogradouro] Retorno montado com sucesso: {JsonConvert.SerializeObject(retorno)}");
                    return ResponseDefault(retorno);
                }

                // Console.WriteLine("[GetLogradouro] Source inativo ou NULL");
                return BadRequest(new
                {
                    error = "Fonte de dados não configurada ou inativa."
                });
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"[GetLogradouro] EXCEÇÃO: {ex.Message}");
                // Console.WriteLine($"[GetLogradouro] StackTrace: {ex.StackTrace}");
                // Console.WriteLine($"[GetLogradouro] InnerException: {ex.InnerException?.Message}");

                return BadRequest(new
                {
                    error = $"Erro ao buscar logradouro: {ex.Message}",
                    stackTrace = ex.StackTrace,
                    innerException = ex.InnerException?.Message
                });
            }
        }

        [Authorize]
        [HttpGet()]
        [Route("GetLogradouroByCodigo")]
        public async Task<IActionResult> GetLogradouroByCodigo(int cd_localidade)
        {
            if (cd_localidade <= 0) return BadRequest("cd_localidade invalido");
            var schemaName = "T_Localidade";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                var filtrosLogradouro = new List<(string campo, object valor)> { new("cd_localidade", cd_localidade) };
                var logradouroExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtrosLogradouro);
                if (logradouroExists == null) return NotFound("logradouro");

                var filtroBairro = new List<(string campo, object valor)> { new("cd_localidade", logradouroExists["cd_loc_relacionada"].ToString()) };
                var bairroExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroBairro);
                if (bairroExists == null) return NotFound("bairro");

                var filtroCidade = new List<(string campo, object valor)> { new("cd_localidade", bairroExists["cd_loc_relacionada"].ToString()) };
                var cidadeExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroCidade);
                if (cidadeExists == null) return NotFound("cidade");

                var filtroEstado = new List<(string campo, object valor)> { new("cd_localidade", cidadeExists["cd_loc_relacionada"].ToString()) };
                var estadoExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroEstado);
                if (estadoExists == null) return NotFound("estado");

                var filtroPais = new List<(string campo, object valor)> { new("cd_localidade", estadoExists["cd_loc_relacionada"].ToString()) };
                var paisExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroPais);
                if (paisExists == null) return NotFound("pais");

                var retorno = new
                {
                    cd_bairro = bairroExists["cd_localidade"],
                    cd_cidade = cidadeExists["cd_localidade"],
                    cd_estado = estadoExists["cd_localidade"],
                    cd_logradouro = logradouroExists["cd_localidade"],
                    cd_pais = paisExists["cd_localidade"],
                    cd_tipo_logradouro = logradouroExists["cd_tipo_localidade"],
                    dc_num_cep = logradouroExists["dc_num_cep"],
                    no_bairro = bairroExists["no_localidade"],
                    no_cidade = cidadeExists["no_localidade"],
                    no_estado = estadoExists["no_localidade"],
                    no_logradouro = logradouroExists["no_localidade"],
                    no_pais = paisExists["no_localidade"]
                };

                return ResponseDefault(retorno);
            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }

        [Authorize]
        [HttpGet()]
        [Route("{cd_pessoa}")]
        public async Task<IActionResult> Get(int cd_pessoa)
        {
            var schemaName = "T_Pessoa";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                var retorno = new InsertUsuarioModel();

                var filtrosPessoa = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa) };
                var pessoaExists = await SQLServerService.GetFirstByFields(source, "T_Pessoa", filtrosPessoa);
                if (pessoaExists == null) return NotFound("pessoa");

                retorno.pessoa.txt_obs_pessoa = pessoaExists.ContainsKey("txt_obs_pessoa") && pessoaExists["txt_obs_pessoa"] != null ? pessoaExists["txt_obs_pessoa"].ToString() : string.Empty;
                retorno.pessoa.img_pessoa = pessoaExists.ContainsKey("img_pessoa") && pessoaExists["img_pessoa"] is byte[] bytes
                 ? Convert.ToBase64String(bytes)
                 : null;
                retorno.pessoa.ext_img_pessoa = pessoaExists.ContainsKey("ext_img_pessoa") ? pessoaExists["ext_img_pessoa"]?.ToString() : null;
                retorno.pessoa.cd_pessoa = cd_pessoa;
                retorno.pessoa.no_pessoa = pessoaExists["no_pessoa"].ToString();
                retorno.pessoa.dt_cadastramento = DateTime.Parse(pessoaExists["dt_cadastramento"].ToString());
                retorno.pessoa.dc_reduzido_pessoa = pessoaExists["dc_reduzido_pessoa"] != null ? pessoaExists["dc_reduzido_pessoa"].ToString() : "";

                var filtrosEmail = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa), new("cd_tipo_telefone", 4) };
                var emailExists = await SQLServerService.GetFirstByFields(source, "T_Telefone", filtrosEmail);
                if (emailExists != null) retorno.pessoa.dc_email = emailExists["dc_fone_mail"].ToString();

                var filtrosTelefone = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa), new("cd_tipo_telefone", 1) };
                var telefoneExists = await SQLServerService.GetFirstByFields(source, "T_Telefone", filtrosTelefone);
                if (telefoneExists != null) retorno.pessoa.telefone = telefoneExists["dc_fone_mail"].ToString();

                var filtrosCelular = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa), new("cd_tipo_telefone", 3) };
                var celularExists = await SQLServerService.GetFirstByFields(source, "T_Telefone", filtrosCelular);
                if (celularExists != null) retorno.pessoa.celular = celularExists["dc_fone_mail"].ToString();

                var filtrosEndereco = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa) };
                var enderecoExists = await SQLServerService.GetFirstByFields(source, "T_ENDERECO", filtrosEndereco);
                if (enderecoExists != null)
                {
                    retorno.pessoa.endereco.cd_loc_pais = (int)enderecoExists["cd_loc_pais"];
                    retorno.pessoa.endereco.cd_loc_estado = (int)enderecoExists["cd_loc_estado"];
                    retorno.pessoa.endereco.cd_loc_cidade = (int)enderecoExists["cd_loc_cidade"];
                    retorno.pessoa.endereco.cd_tipo_endereco = (int)enderecoExists["cd_tipo_endereco"];
                    retorno.pessoa.endereco.cd_loc_bairro = (int)enderecoExists["cd_loc_bairro"];
                    retorno.pessoa.endereco.cd_tipo_logradouro = (int)enderecoExists["cd_tipo_logradouro"];
                    retorno.pessoa.endereco.cd_loc_logradouro = (int)enderecoExists["cd_loc_logradouro"];
                    retorno.pessoa.endereco.dc_compl_endereco = enderecoExists["dc_compl_endereco"] != null ? enderecoExists["dc_compl_endereco"].ToString() : "";
                    retorno.pessoa.endereco.dc_num_cep = enderecoExists["dc_num_cep"] != null ? enderecoExists["dc_num_cep"].ToString() : "";
                    retorno.pessoa.endereco.dc_num_endereco = enderecoExists["dc_num_endereco"]?.ToString();

                    // Buscar nomes das localidades
                    var filtrosLogradouro = new List<(string campo, object valor)> { new("cd_localidade", retorno.pessoa.endereco.cd_loc_logradouro) };
                    var logradouroExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtrosLogradouro);
                    if (logradouroExists != null)
                    {
                        retorno.pessoa.endereco.no_logradouro = logradouroExists["no_localidade"]?.ToString();
                        // Se dc_num_cep está vazio no endereço, pegar do logradouro
                        if (string.IsNullOrEmpty(retorno.pessoa.endereco.dc_num_cep) && logradouroExists["dc_num_cep"] != null)
                        {
                            retorno.pessoa.endereco.dc_num_cep = logradouroExists["dc_num_cep"].ToString();
                        }
                    }

                    var filtrosBairro = new List<(string campo, object valor)> { new("cd_localidade", retorno.pessoa.endereco.cd_loc_bairro) };
                    var bairroExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtrosBairro);
                    if (bairroExists != null)
                    {
                        retorno.pessoa.endereco.no_bairro = bairroExists["no_localidade"]?.ToString();
                    }

                    var filtrosCidade = new List<(string campo, object valor)> { new("cd_localidade", retorno.pessoa.endereco.cd_loc_cidade) };
                    var cidadeExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtrosCidade);
                    if (cidadeExists != null)
                    {
                        retorno.pessoa.endereco.no_cidade = cidadeExists["no_localidade"]?.ToString();
                    }

                    var filtrosEstado = new List<(string campo, object valor)> { new("cd_localidade", retorno.pessoa.endereco.cd_loc_estado) };
                    var estadoExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtrosEstado);
                    if (estadoExists != null)
                    {
                        retorno.pessoa.endereco.no_estado = estadoExists["no_localidade"]?.ToString();
                    }

                    var filtrosPais = new List<(string campo, object valor)> { new("cd_localidade", retorno.pessoa.endereco.cd_loc_pais) };
                    var paisExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtrosPais);
                    if (paisExists != null)
                    {
                        retorno.pessoa.endereco.no_pais = paisExists["no_localidade"]?.ToString();
                    }
                }
                var filtrosPessoaFisica = new List<(string campo, object valor)> { new("cd_pessoa_fisica", cd_pessoa) };
                var pessoaFisicaExists = await SQLServerService.GetFirstByFields(source, "T_PESSOA_FISICA", filtrosPessoaFisica);
                if (pessoaFisicaExists != null)
                {
                    if (pessoaFisicaExists.ContainsKey("dt_emis_expedidor") && pessoaFisicaExists["dt_emis_expedidor"] != null) retorno.pessoa.dt_emis_expedidor = DateTime.Parse(pessoaFisicaExists["dt_emis_expedidor"].ToString());
                    var nm_sexo = pessoaFisicaExists["nm_sexo"];
                    int? cd_estadocivil = null;
                    if (pessoaFisicaExists.ContainsKey("cd_estado_civil") && pessoaFisicaExists["cd_estado_civil"] != null) cd_estadocivil = (int)pessoaFisicaExists["cd_estado_civil"];
                    retorno.pessoa.cd_estado_civil = cd_estadocivil;
                    int? cd_loc_nacionalidade = null;
                    if (pessoaFisicaExists.ContainsKey("cd_loc_nacionalidade") && pessoaFisicaExists["cd_loc_nacionalidade"] != null) cd_loc_nacionalidade = (int)pessoaFisicaExists["cd_loc_nacionalidade"];
                    retorno.pessoa.cd_loc_nacionalidade = cd_loc_nacionalidade;
                    retorno.pessoa.nm_sexo = nm_sexo != null ? int.Parse(nm_sexo.ToString()) : 0;
                    retorno.pessoa.dt_nascimento = pessoaFisicaExists["dt_nascimento"] != null ? DateTime.Parse(pessoaFisicaExists["dt_nascimento"].ToString()) : DateTime.MinValue;
                    retorno.pessoa.id_exportado = pessoaFisicaExists["id_exportado"] != null ? (bool)pessoaFisicaExists["id_exportado"] : false;
                    retorno.pessoa.nm_cpf = pessoaFisicaExists["nm_cpf"] != null ? pessoaFisicaExists["nm_cpf"].ToString() : "";
                    retorno.pessoa.nm_doc_identidade = pessoaFisicaExists["nm_doc_identidade"] != null ? pessoaFisicaExists["nm_doc_identidade"].ToString() : "";

                    int? cd_orgao_expedidor = null;
                    if (pessoaFisicaExists.ContainsKey("cd_orgao_expedidor"))
                    {
                        if (pessoaFisicaExists["cd_orgao_expedidor"] != null) cd_orgao_expedidor = (int)pessoaFisicaExists["cd_orgao_expedidor"];
                    }
                    int? cd_estado_expedidor = null;
                    if (pessoaFisicaExists.ContainsKey("cd_estado_expedidor"))
                    {
                        if (pessoaFisicaExists["cd_estado_expedidor"] != null) cd_estado_expedidor = (int)pessoaFisicaExists["cd_estado_expedidor"];
                    }
                    if (pessoaExists.ContainsKey("cd_atividade_principal"))
                    {
                        if (pessoaExists["cd_atividade_principal"] != null) retorno.pessoa.cd_atividade_principal = (int)pessoaExists["cd_atividade_principal"];
                    }

                    retorno.pessoa.cd_orgao_expedidor = cd_orgao_expedidor;
                    retorno.pessoa.cd_estado_expedidor = cd_estado_expedidor;
                    retorno.pessoa.cd_escolaridade = pessoaFisicaExists["cd_escolaridade"] != null ? pessoaFisicaExists["cd_escolaridade"].ToString() : null;
                }
                var filtrosAluno = new List<(string campo, object valor)> { new("cd_pessoa_aluno", cd_pessoa) };
                var alunoExists = await SQLServerService.GetFirstByFields(source, "T_ALUNO", filtrosAluno);
                var filtrosAlunoView = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa) };
                var alunoExistsView = await SQLServerService.GetFirstByFields(source, "vi_aluno", filtrosAlunoView);
                if (alunoExists != null)
                {
                    retorno.cd_aluno = (int)alunoExists["cd_aluno"];
                    retorno.cd_pessoa_escola = (int)alunoExists["cd_pessoa_escola"];
                    retorno.cd_midia = (int)alunoExists["cd_midia"];
                    retorno.pessoa.cd_escolaridade = alunoExists["cd_escolaridade"] != null ? alunoExists["cd_escolaridade"].ToString() : null;
                    retorno.cd_usuario_atendente = (int)alunoExists["cd_usuario_atendente"];
                    retorno.cd_contato = alunoExists["cd_contato"] != null ? alunoExists["cd_contato"].ToString() : null;

                    //MOTIVOS
                    var motivos = await SQLServerService.GetList("T_ALUNO_MOTIVO_MATRICULA", null, "[cd_aluno]", $"[{alunoExists["cd_aluno"].ToString()}]", source, SearchModeEnum.Equals);
                    var motivosAdicionar = new List<Motivomatricula>();
                    foreach (var m in motivos.data)
                    {
                        var motivo = new Motivomatricula
                        {
                            cd_motivo_matricula = (int)m["cd_motivo_matricula"]
                        };
                        motivosAdicionar.Add(motivo);
                    }
                    retorno.motivoMatricula = motivosAdicionar.ToArray();
                    //HORARIOS
                    var horarios = await SQLServerService.GetList("T_HORARIO", null, "[cd_registro]", $"[{alunoExists["cd_aluno"].ToString()}]", source, SearchModeEnum.Equals);
                    var horariosAdicionar = new List<Gridhorario>();
                    foreach (var h in horarios.data)
                    {
                        var horario = new Gridhorario
                        {
                            id_dia_semana = h.ContainsKey("id_dia_semana") ? Convert.ToInt32(h["id_dia_semana"]) : 0,
                            dc_dia_semana = h.ContainsKey("dc_dia_semana") ? h["dc_dia_semana"]?.ToString() ?? "" : "",
                            cd_horario = h.ContainsKey("cd_horario") ? Convert.ToInt32(h["cd_horario"]) : 0,
                            id_disponivel = h.ContainsKey("id_disponivel") ? Convert.ToBoolean(h["id_disponivel"]) : false,
                            cd_registro = h.ContainsKey("cd_registro") ? Convert.ToInt32(h["cd_registro"]) : 0,
                            id_origem = h.ContainsKey("id_origem") ? Convert.ToInt32(h["id_origem"]) : 0,
                            cd_pessoa_escola = h.ContainsKey("cd_pessoa_escola") ? Convert.ToInt32(h["cd_pessoa_escola"]) : 0,
                            dt_hora_ini = h.ContainsKey("dt_hora_ini") ? h["dt_hora_ini"]?.ToString() ?? "" : "",
                            dt_hora_fim = h.ContainsKey("dt_hora_fim") ? h["dt_hora_fim"]?.ToString() ?? "" : ""
                        };
                        if(horario.cd_pessoa_escola == retorno.cd_pessoa_escola)
                            horariosAdicionar.Add(horario);
                    }

                    retorno.horarios = new Horarios { gridHorario = horariosAdicionar.ToArray() };

                    //vi_aluno_restricao
                    var restricoes = await SQLServerService.GetList("vi_aluno_restricao", null, "[cd_aluno]", $"[{alunoExists["cd_aluno"].ToString()}]", source, SearchModeEnum.Equals);

                    var bolsas = await SQLServerService.GetList("vi_aluno_bolsa", null, "[cd_aluno]", $"[{alunoExists["cd_aluno"].ToString()}]", source, SearchModeEnum.Equals);

                    var relacionamentos_query = await SQLServerService.GetList("vi_relacionamento", null, "[cd_pessoa_pai]", $"[{cd_pessoa}]", source, SearchModeEnum.Equals);

                    var filtrosRaf = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa) };
                    var pessoaRaf = await SQLServerService.GetFirstByFields(source, "T_PESSOA_RAF", filtrosRaf);
                    dynamic aluno = new ExpandoObject();

                    // Copia propriedades de "retorno" para o ExpandoObject, se necessário
                    foreach (var prop in retorno.GetType().GetProperties())
                    {
                        ((IDictionary<string, object>)aluno)[prop.Name] = prop.GetValue(retorno);
                    }

                    var ultima_matricula = await SQLServerService.GetList("T_CONTRATO", 1, 1, "dt_matricula_contrato", true, null,"[cd_aluno]", $"[{alunoExists["cd_aluno"]}]", source, SearchModeEnum.Equals, null, null);

                    var cd_ultimo_curso = ultima_matricula.data.Any() ? (int)ultima_matricula.data.First()["cd_curso_atual"] : (int?)null;
                    bool? id_permitir_matricula = null;
                    var ultimo_curso = await SQLServerService.GetFirstByFields(source, "T_CURSO", new List<(string campo, object valor)> { new("cd_curso", cd_ultimo_curso ?? 0) });
                    id_permitir_matricula = ultimo_curso != null && ultimo_curso.ContainsKey("id_permitir_matricula") && ultimo_curso["id_permitir_matricula"] != null ? (bool?)ultimo_curso["id_permitir_matricula"] : null;
                    aluno.bolsas = bolsas.data;
                    aluno.restricoes = restricoes.data;
                    aluno.relacionamentos = relacionamentos_query.data;
                    aluno.existeMatricula = alunoExistsView["existeMatricula"];
                    aluno.Raf = pessoaRaf;
                    aluno.cd_ultimo_curso = cd_ultimo_curso;
                    aluno.id_permitir_matricula = id_permitir_matricula;

                    return ResponseDefault(aluno);
                }
                else return NotFound("aluno não encontrado");
            }

            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }

        [Authorize]
        [HttpGet]
        [Route("ultima-programacao")]
        public async Task<IActionResult> GetUltimaProgramacao(int cd_aluno,int cd_produto)
        {
            var schemaName = "T_Pessoa";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                var t_cursos_get = await SQLServerService.GetList("T_CURSO", null, "[cd_produto]", $"{cd_produto}",source,SearchModeEnum.Equals);
                if (!t_cursos_get.success || !t_cursos_get.data.Any()) return NotFound("cursos não encontrados");


                var cd_cursos = string.Join(",",t_cursos_get.data.Select(x => x["cd_curso"]));
                //source, "T_ALUNO_TURMA", new List<(string campo, object valor)> { new("cd_aluno", cd_aluno), new("cd_curso", 1) }
                var t_aluno_turma_get = await SQLServerService.GetList("T_ALUNO_TURMA",null,"[cd_aluno],[cd_curso_contrato]",$"[{cd_aluno}],[{cd_cursos}]",source, SearchModeEnum.Equals);
                if (!t_aluno_turma_get.success || !t_aluno_turma_get.data.Any()) return NotFound("aluno turma não encontrado");
                var cd_turmas = string.Join(",", t_aluno_turma_get.data.Select(x => x["cd_turma"]));

                var ultima_programacao_turma_get = await SQLServerService.GetList("T_PROGRAMACAO_TURMA", 1, 1, "dta_programacao_turma", true, null, "[id_aula_dada],[cd_turma]", $"[1],[{cd_turmas}]", source, SearchModeEnum.Equals, null, null);
                if (!ultima_programacao_turma_get.data.Any()) return NotFound("programação não encontrada");
                var ultima_programacao_turma = ultima_programacao_turma_get.data.First();


                var retorno = new Dictionary<string, object>();

                retorno.Add("dta_programacao_turma", ultima_programacao_turma["dta_programacao_turma"]);
               

                return ResponseDefault(retorno);
            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }

        [Authorize]
        [HttpPatch()]
        [Route("{cd_pessoa}")]
        public async Task<IActionResult> Patch(int cd_pessoa)
        {
            var schemaName = "T_Titulo";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                var filtrosAluno = new List<(string campo, object valor)> { new("cd_pessoa_aluno", cd_pessoa) };
                var alunoExists = await SQLServerService.GetFirstByFields(source, "T_ALUNO", filtrosAluno);
                if (alunoExists == null) return NotFound("aluno não encontrado");
                var value = 1;

                if (bool.Parse(alunoExists["id_aluno_ativo"].ToString()) != false) value = 0;
                var alunoDict = new Dictionary<string, object>
                {
                    { "id_aluno_ativo", value }
                };

                var t_aluno = await SQLServerService.Update("T_ALUNO", alunoDict, source, "cd_pessoa_aluno", cd_pessoa);
                if (!t_aluno.success) return BadRequest(t_aluno.error);
                return ResponseDefault();
            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }

        [HttpGet("getEmailRaf"), AllowAnonymous]
        public async Task<ActionResult> GetEmailRaf(string nm_raf, string email)
        {
            try
            {
                var parms = "nm_raf=" + nm_raf + "&dc_email=" + email + "&" + "data" + "=" + DateTime.Now;
                string parametros = "Parametros" + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parms, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                string url = $"{parametros}";
                string jsonResult = JsonConvert.SerializeObject(new { parametros = url });
                return Content(jsonResult, "application/json");
            }
            catch (Exception)
            {
                return StatusCode(500, "Erro interno ao processar a requisição.");
            }
        }

        [Authorize]
        [HttpPost("enviarSenhaRAF")]
        public async Task<IActionResult> EnviarSenhaRAF(string parametros)
        {
            var schemaName = "T_Pessoa";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                try
                {
                    string nm_raf = "";
                    string dc_email = "";
                    DateTime? data = null;
                    string urlD = HttpUtility.UrlDecode(parametros, System.Text.Encoding.UTF8).Replace(" ", "+");
                    string urlD2 = MD5CryptoHelper.descriptografaSenha(urlD, MD5CryptoHelper.KEY);
                    string parms = HttpUtility.UrlDecode(urlD2, System.Text.Encoding.UTF8);
                    string[] parametrosGet = parms.Split('&');

                    for (int i = 0; i < parametrosGet.Length; i++)
                    {
                        string[] parametrosHash = parametrosGet[i].Split('=');

                        if ("nm_raf".Equals(parametrosHash[0]))
                            nm_raf = parametrosHash[1] + "";
                        else
                        if ("dc_email".Equals(parametrosHash[0]))
                            dc_email = parametrosHash[1] + "";
                        else
                        if ("data".Equals(parametrosHash[0]))
                            data = DateTime.Parse(parametrosHash[1] + "");
                    }
                ;
                    DateTime dtHoraAtualRelat = DateTime.Now;
#pragma warning disable CS8629 // Nullable value type may be null.
                    TimeSpan t = dtHoraAtualRelat - data.Value;
#pragma warning restore CS8629 // Nullable value type may be null.
                    int timeout = 120;
                    if (t.TotalSeconds > timeout)
                    {
                        Exception ex = null;
                        return StatusCode(400, "url expirada");
                    }

                    var pessoa_raf = await SQLServerService.GetFirstByFields(source, "T_PESSOA_RAF", new List<(string campo, object valor)> { new("nm_raf", nm_raf) });
                    if (pessoa_raf == null) return NotFound("RAf não encontrado");

                    var login = nm_raf;
                    var senha = dc_email;

                    var retorno = await VerificaEnvioEmailRafAsync(source, login, senha);
                    if (!retorno.success)
                    {
                        return BadRequest(retorno.msg);
                    }

                    //enviar email com a nova senha
                    string nova_senha = MD5CryptoHelper.gerarSenha(8);

                    // Validar configurações de email
                    if (string.IsNullOrEmpty(_config["EmailSettings:PrimaryDomain"]) ||
                        string.IsNullOrEmpty(_config["EmailSettings:FromUser"]))
                    {
                        return BadRequest("Configurações de email não encontradas. Verifique o appsettings.json.");
                    }

                    SendEmailUI sendEmail = new SendEmailUI()
                    {
                        login = nm_raf,
                        email = dc_email
                    };

                    var emailConfig = new EmailConfigUI
                    {
                        PrimaryDomain = _config["EmailSettings:PrimaryDomain"],
                        PrimaryPort = int.Parse(_config["EmailSettings:PrimaryPort"] ?? "587"),
                        UsernameEmail = _config["EmailSettings:UsernameEmail"],
                        UsernamePassword = _config["EmailSettings:UsernamePassword"],
                        FromEmail = _config["EmailSettings:FromEmail"],
                        FromUser = _config["EmailSettings:FromUser"],
                        CcEmail = _config["EmailSettings:CcEmail"] ?? "",
                        ssl = _config["EmailSettings:ssl"] ?? "true",
                        UseDefaultCredentials = _config["EmailSettings:UseDefaultCredentials"] ?? "false"
                    };

                    SendEmailUI.configurarEmailSection(sendEmail, emailConfig);

                    sendEmail.destinatario = sendEmail.email;

                    sendEmail.assunto = "Envio de nova senha";
                    string unidade = "SGF";
                    string nomesEscolas = "ESCOLA SGF";
                    EMailParameters emailParams = new EMailParameters()
                    {
                        titulo = _config.GetSection("EmailParameters:titulo").Value,
                        subTitulo = String.Format(_config.GetSection("EmailParameters:subTitulo").Value, _config.GetSection("EmailParameters:nomeApp").Value),
                        usuario = sendEmail.login,
                        senha = nova_senha,
                        escola = unidade + nomesEscolas,
                        btnLinkValue = _config.GetSection("EmailParameters:urlApp").Value,
                        btnValue = String.Format(_config.GetSection("EmailParameters:labelLinkUrl").Value, _config.GetSection("EmailParameters:nomeApp").Value),
                        btnBackgroundColor = _config.GetSection("EmailParameters:btnBackgroundColor").Value,
                        linkColor = _config.GetSection("EmailParameters:linkColor").Value,
                        empresaFooter = _config.GetSection("EmailParameters:empresaEmail").Value,
                        telefoneTag = _config.GetSection("EmailParameters:telefoneTag").Value,
                        telefoneValue = _config.GetSection("EmailParameters:telefoneValue").Value,
                        telefoneLabel = _config.GetSection("EmailParameters:telefoneLabel").Value,
                        enviadoPorValue = String.Format(_config.GetSection("EmailParameters:enviadoPorValue").Value, _config.GetSection("EmailParameters:nomeApp").Value),
                        urlImagem = _config.GetSection("EmailParameters:urlImage").Value
                    };
                    sendEmail.mensagem =
                      TemplatesEmailRaf.getTemplateEmailChangePassword(emailParams);

                    var result = await EnviarEmailAsync(sendEmail, emailConfig);
                    if (!result.success)
                    {
                        Console.WriteLine($"[EnviarSenhaRAF] Falha ao enviar email para {dc_email}: {result.errorMessage}");
                        return BadRequest(new
                        {
                            error = "Erro ao enviar email",
                            message = result.errorMessage
                        });
                    }

                    // Atualiza a senha no banco de dados
                    //Gera o hash da senha do usuário
                    string senhaHash = MD5CryptoHelper.geraSenhaHashSHA1(nova_senha);

                    var cd_pessoa_raf = pessoa_raf["cd_pessoa_raf"];
                    pessoa_raf.Remove("cd_pessoa_raf");
                    pessoa_raf["dc_senha_raf"] = senhaHash;
                    pessoa_raf["id_trocar_senha"] = 1;
                    pessoa_raf["id_bloqueado"] = 0;
                    pessoa_raf["nm_tentativa"] = 0;

                    var t_pessoa_raf_update = await SQLServerService.Update("T_PESSOA_RAF", pessoa_raf, source, "cd_pessoa_raf", cd_pessoa_raf);
                    if (!t_pessoa_raf_update.success) return BadRequest(t_pessoa_raf_update.error);
                    return Ok();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[EnviarSenhaRAF] Erro: {ex.ToString()}");
                    return BadRequest($"Erro ao processar requisição: {ex.Message}");
                }
            }

            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }

        [Authorize]
        [HttpDelete("{cd_pessoa}")]
        public async Task<IActionResult> Delete(int cd_pessoa)
        {
            var schemaName = "T_Aluno";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            
            if (source != null && source.Active != null && source.Active == true)
            {
                try
                {
                    // Verifica se o aluno existe
                    var filtrosAluno = new List<(string campo, object valor)> { new("cd_pessoa_aluno", cd_pessoa) };
                    var alunoExists = await SQLServerService.GetFirstByFields(source, "T_ALUNO", filtrosAluno);
                    
                    if (alunoExists == null)
                    {
                        return NotFound(new
                        {
                            succeeded = false,
                            message = "Aluno não encontrado"
                        });
                    }

                    // Tenta deletar o aluno
                    var deleteResult = await SQLServerService.Delete("T_ALUNO", "cd_aluno", alunoExists["cd_aluno"].ToString(), source);
                    
                    if (!deleteResult.success)
                    {
                        // Verifica se é erro de constraint (relacionamento com outras tabelas)
                        if (deleteResult.error != null && 
                            (deleteResult.error.Contains("REFERENCE") || 
                             deleteResult.error.Contains("DELETE") ||
                             deleteResult.error.Contains("constraint")))
                        {
                            return BadRequest(new
                            {
                                succeeded = false,
                                message = "Não foi possível excluir o aluno pois possui vínculos com outros registros (matrículas, turmas, etc.)"
                            });
                        }
                        
                        return BadRequest(new
                        {
                            succeeded = false,
                            message = $"Erro ao excluir aluno: {deleteResult.error}"
                        });
                    }

                    return ResponseDefault(new
                    {
                        succeeded = true,
                        message = "Registro excluído com sucesso!"
                    });
                }
                catch (Exception ex)
                {
                    return BadRequest(new
                    {
                        succeeded = false,
                        message = $"Erro ao excluir aluno: {ex.Message}"
                    });
                }
            }

            return BadRequest(new
            {
                succeeded = false,
                message = "Fonte de dados não configurada ou inativa."
            });
        }

        private async Task<(bool success, string msg)> VerificaEnvioEmailRafAsync(Source source, string login, string email,
        CancellationToken ct = default)
        {
            int? cdPessoa = null;
            int? cdAluno = null;
            bool? idLiberado = null;

            // Busca cd_aluno, cd_pessoa e id_raf_liberado

            string connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};MultipleActiveResultSets=True;";
            string msg = null;

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                var selectCmd = new SqlCommand(@"
                          select
                    a.cd_aluno,
	                a.cd_pessoa_aluno,
	                pa.id_raf_liberado
                  from
                    T_ALUNO a
                    inner join t_pessoa_raf pa
                    on a.cd_pessoa_aluno = pa.cd_pessoa
                  where
                    pa.nm_raf = @login", connection);

                selectCmd.Parameters.AddWithValue("@login", login);
                using (var reader = await selectCmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        cdAluno = reader.IsDBNull("cd_aluno") ? (int?)null : Convert.ToInt32(reader["cd_aluno"]);
                        cdPessoa = reader.IsDBNull("cd_pessoa_aluno") ? (int?)null : Convert.ToInt32(reader["cd_pessoa_aluno"]);
                        idLiberado = reader.IsDBNull("id_raf_liberado") ? (bool?)null : Convert.ToBoolean(reader["id_raf_liberado"]);
                    }
                }

                //Verifica existência de e-mail para a pessoa no tipo 'E-Mail'
                bool idOk = false;
                if (cdPessoa.HasValue && !string.IsNullOrWhiteSpace(email))
                {
                    const string qExists = @"
                SELECT CASE WHEN EXISTS (
                    SELECT 1
                    FROM T_TELEFONE t
                    WHERE t.cd_pessoa = @cd_pessoa
                      AND t.dc_fone_mail = @mail
                      AND t.cd_tipo_telefone = @cd_tipo
                ) THEN 1 ELSE 0 END;";

                    using (var cmdEx = new SqlCommand(qExists, connection))
                    {
                        cmdEx.Parameters.Add(new SqlParameter("@cd_pessoa", SqlDbType.Int) { Value = cdPessoa.Value });
                        cmdEx.Parameters.Add(new SqlParameter("@mail", SqlDbType.VarChar, 128) { Value = email });
                        cmdEx.Parameters.Add(new SqlParameter("@cd_tipo", SqlDbType.Int) { Value = 4 });

                        var existsObj = await cmdEx.ExecuteScalarAsync();
                        idOk = existsObj != null && existsObj != DBNull.Value && Convert.ToInt32(existsObj) == 1;
                    }
                }
                if (!idOk) return new(false, "msgErroRAFsemEmail");

                if (idLiberado.HasValue && !idLiberado.Value) return new(false, "RAFLiberado");

                return new(true, "");
            }
        }

        private async Task<(bool success, string errorMessage)> EnviarEmailAsync(SendEmailUI email, EmailConfigUI emailConfig)
        {
            try
            {
                Console.WriteLine($"[EnviarEmailAsync] Iniciando envio de email para: {email.destinatario}");
                Console.WriteLine($"[EnviarEmailAsync] SMTP: {emailConfig.PrimaryDomain}:{emailConfig.PrimaryPort}");
                Console.WriteLine($"[EnviarEmailAsync] SSL: {emailConfig.ssl}");
                Console.WriteLine($"[EnviarEmailAsync] From: {emailConfig.FromEmail}");

                var client = new SmtpClient
                {
                    UseDefaultCredentials = Convert.ToBoolean(emailConfig.UseDefaultCredentials),
                    Host = emailConfig.PrimaryDomain,
                    Port = emailConfig.PrimaryPort,
                    EnableSsl = Convert.ToBoolean(emailConfig.ssl),
                    Credentials = new NetworkCredential(emailConfig.UsernameEmail, emailConfig.UsernamePassword)
                };

                // Trusting in all certificates
                if (client.EnableSsl && ServicePointManager.ServerCertificateValidationCallback == null)
                    ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;

                // Applying message settings
                MailMessage message = new()
                {
                    From = new MailAddress(email.FromEmail.Trim(), emailConfig.FromUser.Trim(), Encoding.UTF8),
                    IsBodyHtml = true,
                    Body = email.mensagem,
                    Priority = MailPriority.Normal,
                    Subject = email.assunto,
                    BodyEncoding = Encoding.UTF8,
                    SubjectEncoding = Encoding.UTF8
                };

                message.To.Add(email.destinatario);

                Console.WriteLine($"[EnviarEmailAsync] Enviando email...");
                await Task.Run(() => client.Send(message));
                Console.WriteLine($"[EnviarEmailAsync] Email enviado com sucesso!");
                return (true, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EnviarEmailAsync] ERRO: {ex.ToString()}");

                // Capturar inner exception se existir
                string errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += $" | Inner: {ex.InnerException.Message}";
                    Console.WriteLine($"[EnviarEmailAsync] INNER EXCEPTION: {ex.InnerException.ToString()}");
                }

                return (false, errorMessage);
            }
        }
    }
}