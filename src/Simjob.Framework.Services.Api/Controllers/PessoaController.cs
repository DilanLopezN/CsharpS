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
using Simjob.Framework.Services.Api.Models.Contato;
using Simjob.Framework.Services.Api.Models.Pessoas;
using Simjob.Framework.Services.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simjob.Framework.Services.Api.Controllers
{
    public class PessoaController : BaseController
    {
        private readonly IRepository<SourceContext, Source> _sourceRepository;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;

        public PessoaController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, IRepository<SourceContext, Source> sourceRepository, IRepository<MongoDbContext, Schema> schemaRepository) : base(bus, notifications)
        {
            _sourceRepository = sourceRepository;
            _schemaRepository = schemaRepository;
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
                if (string.IsNullOrEmpty(searchFields) && string.IsNullOrEmpty(value))
                {
                    searchFields = "is_delete";

                    value = $"0,";
                }
                else
                {
                    searchFields = searchFields + ",is_delete";

                    value = value + $",0";
                }
                var pessoasResult = await SQLServerService.GetList("vi_pessoa", page, limit, sortField, sortDesc, ids, searchFields, value, source, mode, "cd_empresa", cd_empresa);
                if (pessoasResult.success)
                {
                    var pessoas = pessoasResult.data;

                    var retorno = new
                    {
                        data = pessoas.Select(x => new
                        {
                            cd_pessoa = x["cd_pessoa"],
                            no_pessoa = x["no_pessoa"],
                            nm_cpf = x["nm_cpf"],
                            nm_cnpj = x["nm_cnpj2"],
                            Email = x["email"],
                            Telefone = x["telefone"],
                            Celular = x["celular"],
                            nm_natureza_pessoa = x["nm_natureza_pessoa"],
                            id_ativo = "Ativo"
                        }),
                        pessoasResult.total,
                        page,
                        limit,
                        pages = limit != null ? (int)Math.Ceiling((double)pessoasResult.total / limit.Value) : 0
                    };

                    return ResponseDefault(retorno);
                }
                return BadRequest(new
                {
                    sucess = false,
                    error = pessoasResult.error
                });
            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }

        [Authorize]
        [HttpPost()]
        public async Task<IActionResult> Insert([FromBody] InsertPessoaModel command)
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
                    cd_pessoa = resultReturn.pessoaId,
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
        [HttpPut()]
        [Route("{cd_pessoa}")]
        public async Task<IActionResult> Update([FromBody] InsertPessoaModel command, int cd_pessoa)
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

                var resultReturn = await ProcessaUpdatePessoa(cd_pessoa, command, source);
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
                var retorno = new Dictionary<string, object>();

                var filtrosPessoa = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa) };
                var pessoaExists = await SQLServerService.GetFirstByFields(source, "T_Pessoa", filtrosPessoa);
                if (pessoaExists == null) return NotFound("pessoa");

                var pessoa = new Dictionary<string, object>
                {
                    { "cd_pessoa", cd_pessoa },
                    { "no_pessoa", pessoaExists["no_pessoa"]?.ToString() },
                    { "dt_cadastramento", DateTime.Parse(pessoaExists["dt_cadastramento"]?.ToString() ?? DateTime.MinValue.ToString()) },
                    { "dc_reduzido_pessoa", pessoaExists["dc_reduzido_pessoa"]?.ToString() ?? "" },
                    { "nm_natureza_pessoa", int.Parse(pessoaExists["nm_natureza_pessoa"]?.ToString() ?? null) },
                    { "cd_atividade_principal", pessoaExists["cd_atividade_principal"] },
                    { "txt_obs_pessoa", pessoaExists["txt_obs_pessoa"] },
                    { "img_pessoa", pessoaExists["img_pessoa"] },
                    { "ext_img_pessoa", pessoaExists["ext_img_pessoa"]}
                };

                // Email
                var filtrosEmail = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa), new("cd_tipo_telefone", 4) };
                var emailExists = await SQLServerService.GetFirstByFields(source, "T_Telefone", filtrosEmail);
                if (emailExists != null) pessoa.Add("dc_email", emailExists["dc_fone_mail"]?.ToString());

                // Telefone
                var filtrosTelefone = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa), new("cd_tipo_telefone", 1) };
                var telefoneExists = await SQLServerService.GetFirstByFields(source, "T_Telefone", filtrosTelefone);
                if (telefoneExists != null) pessoa.Add("telefone", telefoneExists["dc_fone_mail"]?.ToString());

                // Celular
                var filtrosCelular = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa), new("cd_tipo_telefone", 3) };
                var celularExists = await SQLServerService.GetFirstByFields(source, "T_Telefone", filtrosCelular);
                if (celularExists != null) pessoa.Add("celular", celularExists["dc_fone_mail"]?.ToString());

                // Endereço
                var filtrosEndereco = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa) };
                var enderecoExists = await SQLServerService.GetFirstByFields(source, "T_ENDERECO", filtrosEndereco);
                if (enderecoExists != null)
                {
                    var endereco = new Dictionary<string, object>();

                    var cep = enderecoExists["dc_num_cep"];
                    if (cep != null)
                    {
                        var complemento = enderecoExists["dc_compl_endereco"]?.ToString() ?? "";
                        var numero = enderecoExists["dc_num_endereco"]?.ToString() ?? "";
                        // cd_tipo_logradouro
                        var cd_tipo_logradouro = enderecoExists["cd_tipo_logradouro"]?.ToString() ?? "";

                        var infosCep = await BuscarCEP(cep.ToString(), numero, complemento, cd_tipo_logradouro, source);
                        if (infosCep != null)
                        {
                            pessoa.Add("endereco", infosCep);
                        }
                    }
                }

                // Pessoa Física
                if ((int)pessoa["nm_natureza_pessoa"] == 1)
                {
                    var filtrosPessoaFisica = new List<(string campo, object valor)> { new("cd_pessoa_fisica", cd_pessoa) };
                    var pessoaFisicaExists = await SQLServerService.GetFirstByFields(source, "T_PESSOA_FISICA", filtrosPessoaFisica);
                    if (pessoaFisicaExists != null)
                    {
                        pessoa.Add("cd_estado_civil", pessoaFisicaExists["cd_estado_civil"] != null ? (int)pessoaFisicaExists["cd_estado_civil"] : null);
                        pessoa.Add("cd_loc_nacionalidade", pessoaFisicaExists["cd_loc_nacionalidade"] != null ? (int)pessoaFisicaExists["cd_loc_nacionalidade"] : null);
                        pessoa.Add("nm_sexo", int.TryParse(pessoaFisicaExists["nm_sexo"]?.ToString(), out var nmSexo) ? nmSexo : null);
                        pessoa.Add("dt_nascimento", DateTime.TryParse(pessoaFisicaExists["dt_nascimento"]?.ToString(), out var dtNascimento) ? dtNascimento : DateTime.MinValue);
                        pessoa.Add("id_exportado", false);
                        pessoa.Add("nm_cpf", pessoaFisicaExists["nm_cpf"]?.ToString() ?? "");
                        pessoa.Add("nm_doc_identidade", pessoaFisicaExists["nm_doc_identidade"]?.ToString() ?? "");
                        pessoa.Add("cd_orgao_expedidor", pessoaFisicaExists["cd_orgao_expedidor"] != null ? (int)pessoaFisicaExists["cd_orgao_expedidor"] : null);
                        pessoa.Add("cd_estado_expedidor", pessoaFisicaExists["cd_estado_expedidor"] != null ? (int)pessoaFisicaExists["cd_estado_expedidor"] : null);
                        pessoa.Add("cd_escolaridade", pessoaFisicaExists["cd_escolaridade"] != null ? (int)pessoaFisicaExists["cd_escolaridade"] : null);
                    }
                }
                else
                {
                    var filtrosPessoaJuridica = new List<(string campo, object valor)> { new("cd_pessoa_juridica", cd_pessoa) };
                    var pessoaJuridicaExists = await SQLServerService.GetFirstByFields(source, "T_PESSOA_JURIDICA", filtrosPessoaJuridica);
                    if (pessoaJuridicaExists != null)
                    {
                        pessoa.Add("cd_tipo_sociedade", pessoaJuridicaExists["cd_tipo_sociedade"]?.ToString() ?? "");
                        pessoa.Add("dc_num_insc_estadual", pessoaJuridicaExists["dc_num_insc_estadual"]?.ToString() ?? "");
                        pessoa.Add("dc_num_insc_municipal", pessoaJuridicaExists["dc_num_insc_municipal"]?.ToString() ?? "");
                        pessoa.Add("nm_cnpj_cgc", pessoaJuridicaExists["dc_num_cnpj_cnab"]?.ToString() ?? "");
                    }
                }
                //dependentes
                var dependentes_query = await SQLServerService.GetList("vi_relacionamento", null, "[cd_pessoa_pai]", $"[{cd_pessoa}]", source, SearchModeEnum.Contains);
                var dependentes = dependentes_query.data;
                pessoa.Add("dependentes", dependentes);

                // Adiciona a pessoa ao retorno
                retorno.Add("pessoa", pessoa);

                return ResponseDefault(retorno);
            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }

        /// <summary>
        /// Exclusão fisica de pessoa.
        /// </summary>
        /// <param name="cd_pessoa"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete]
        [Route("{cd_pessoa}")]
        public async Task<IActionResult> DeletePessoa(int cd_pessoa)
        {
            var schemaName = "T_Pessoa";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                var filtrosPessoa = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa) };
                var pessoaExists = await SQLServerService.GetFirstByFields(source, "T_Pessoa", filtrosPessoa);
                if (pessoaExists == null) return NotFound("pessoa");

                var pessoas_dependentes = await SQLServerService.GetList("T_RELACIONAMENTO", null, "[cd_pessoa_pai]", $"[{cd_pessoa}]", source, SearchModeEnum.Equals);
                if (pessoas_dependentes.success && pessoas_dependentes.data.Any()) return BadRequest("pessoa possui dependentes");

                var delete = await SQLServerService.Delete("T_PESSOA", "cd_pessoa", cd_pessoa.ToString(), source);
                if (!delete.success) return BadRequest(delete.error);
                return ResponseDefault();

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
            var schemaName = "T_Pessoa";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                var filtrosPessoa = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa) };
                var pessoaExists = await SQLServerService.GetFirstByFields(source, "T_PESSOA", filtrosPessoa);
                if (pessoaExists == null) return NotFound("contato não encontrado");
                var pessoaDict = new Dictionary<string, object>();

                if (int.Parse(pessoaExists["is_delete"].ToString()) == 0)
                {
                    pessoaDict = new Dictionary<string, object>
                    {
                        { "is_delete", 1 },
                        {"delete_at", DateTime.Now}
                    };
                }
                else
                {
                    pessoaDict = new Dictionary<string, object>
                    {
                        { "is_delete", 0 }
                    };
                }

                var t_pessoa = await SQLServerService.Update("T_PESSOA", pessoaDict, source, "cd_pessoa", cd_pessoa);
                if (!t_pessoa.success) return BadRequest(t_pessoa.error);
                return ResponseDefault();
            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }

        [Authorize]
        [HttpPost]
        [Route("Contato")]
        public async Task<IActionResult> PostContato(ContatoInsertModel model)
        {
            var schemaName = "T_Pessoa";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                var resultReturn = await ValidateCommandContato(model, source);
                var result = new
                {
                    resultReturn.sucess,
                    resultReturn.error
                };
                return resultReturn.sucess ? ResponseDefault(result.error) : BadRequest(result);
            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }

        [Authorize]
        [HttpPost]
        [Route("dependente")]
        public async Task<IActionResult> PostDependente([FromBody] List<PessoaRelacionamento>? dependentes)
        {
            if (dependentes == null || !dependentes.Any()) return BadRequest("nenhum dependente informado");
            var schemaName = "T_Pessoa";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                foreach (var dependente in dependentes)
                {
                    var relacionamento_dict = new Dictionary<string, object>
                    {
                        { "cd_pessoa_filho", dependente.cd_pessoa_filho },
                        { "cd_pessoa_pai", dependente.cd_pessoa_pai },
                        { "cd_papel_filho", dependente.cd_papel_filho }
                    };
                    if (dependente.cd_qualif_relacionamento != null) relacionamento_dict.Add("cd_qualif_relacionamento", dependente.cd_qualif_relacionamento);
                    if (dependente.cd_papel_pai != null) relacionamento_dict.Add("cd_papel_pai", dependente.cd_papel_pai);

                    if (relacionamento_dict.Any())
                    {
                        var t_relacionamento_insert = await SQLServerService.Insert("T_RELACIONAMENTO", relacionamento_dict, source);
                        if (!t_relacionamento_insert.success) return BadRequest(t_relacionamento_insert.error);
                    }

                    if (dependente.email_pessoa_filho != null)
                    {
                        //T_TELEFONE(email)
                        var telefoneDictEmail = new Dictionary<string, object>
                        {
                            //{ "cd_telefone", null },
                            { "cd_pessoa", dependente.cd_pessoa_filho },
                            { "cd_tipo_telefone", 4 },
                            { "cd_classe_telefone", 1 },
                            { "dc_fone_mail", dependente.email_pessoa_filho },
                            { "cd_endereco", null },
                            { "id_telefone_principal",1 },
                            { "cd_operadora", null }
                        };
                        var t_telefone_email_insert = await SQLServerService.Insert("T_TELEFONE", telefoneDictEmail, source);
                        if (!t_telefone_email_insert.success) return BadRequest();
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
        [Route("Contato/{cd_contato}")]
        public async Task<IActionResult> PutContato(ContatoInsertModel model, int cd_contato)
        {
            var schemaName = "T_Pessoa";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                var resultReturn = await ProcessaUpdateContato(cd_contato, model, source);
                var result = new
                {
                    resultReturn.sucess,
                    resultReturn.error
                };
                return resultReturn.sucess ? ResponseDefault() : BadRequest(result);
            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }

        [Authorize]
        [HttpGet]
        [Route("Contato")]
        public async Task<IActionResult> GetContato(string value, SearchModeEnum mode, int? page, int? limit, string sortField, bool sortDesc = false, string ids = "", string searchFields = null, string? cd_empresa = null)
        {
            if (cd_empresa == null) return BadRequest("campo cd_empresa não informado");
            //vi_contato_listagem
            var schemaName = "T_Pessoa";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                var contatosResult = await SQLServerService.GetList("vi_contato_listagem", page, limit, sortField, sortDesc, ids, searchFields, value, source, mode, "cd_pessoa_escola", cd_empresa);
                var contatos = contatosResult.data;

                var retorno = new
                {
                    data = contatos.Select(x =>
                    {
                        int id = x["id_posicao_contato"] != DBNull.Value
                        ? Convert.ToInt32(x["id_posicao_contato"])
                        : -1;

                        return new
                        {
                            cd_contato = x["cd_contato"],
                            no_acao = x["no_acao"],
                            no_pessoa = x["no_pessoa"],
                            id_posicao_contato = id switch
                            {
                                1 => "Lead",
                                2 => "Prospect",
                                3 => "Ex-aluno",
                                4 => "Desistente",
                                5 => "Aluno",
                                _ => ""
                            },
                            Email = x["email"],
                            Telefone = x["telefone"],
                            Celular = x["celular"],
                            CPF = x["nm_cpf"],
                            existe_pipeline = x["existe_pipeline"],
                            id_status_contato = x["id_status_contato"]
                        };
                    }),
                    contatosResult.total,
                    page,
                    limit,
                    pages = limit != null ? (int)Math.Ceiling((double)contatosResult.total / limit.Value) : 0
                };

                return ResponseDefault(retorno);
            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }

        [Authorize]
        [HttpPatch()]
        [Route("Contato/{cd_contato}")]
        public async Task<IActionResult> PatchContato(int cd_contato)
        {
            var schemaName = "T_Contato";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                var filtrosContato = new List<(string campo, object valor)> { new("cd_contato", cd_contato) };
                var contatoExists = await SQLServerService.GetFirstByFields(source, "T_CONTATO", filtrosContato);
                if (contatoExists == null) return NotFound("contato não encontrado");

                var value = 1;
                if (int.Parse(contatoExists["id_status_contato"].ToString()) != 0) value = 0;
                var contatoDict = new Dictionary<string, object>
                {
                    { "id_status_contato", value }
                };

                var t_contato = await SQLServerService.Update("T_CONTATO", contatoDict, source, "cd_contato", cd_contato);
                if (!t_contato.success) return BadRequest(t_contato.error);
                return ResponseDefault();
            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }

        [Authorize]
        [HttpGet()]
        [Route("Contato/{cd_contato}")]
        public async Task<IActionResult> GetContatoById(int cd_contato)
        {
            var schemaName = "T_Pessoa";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                var retorno = new Dictionary<string, object>();

                var filtrosContato = new List<(string campo, object valor)> { new("cd_contato", cd_contato) };
                var contatoExists = await SQLServerService.GetFirstByFields(source, "T_CONTATO", filtrosContato);
                if (contatoExists == null) return NotFound("contato não encontrado");
                var cd_pessoa = contatoExists["cd_pessoa_contato"];

                var filtrosPessoa = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa) };
                var pessoaExists = await SQLServerService.GetFirstByFields(source, "T_Pessoa", filtrosPessoa);
                if (pessoaExists == null) return NotFound("pessoa");

                retorno.Add("cd_contato", cd_contato);
                retorno.Add("cd_pessoa_contato", int.Parse(cd_pessoa?.ToString() ?? "0"));
                retorno.Add("cd_pessoa_escola", int.Parse(contatoExists["cd_pessoa_escola"]?.ToString() ?? "0"));
                retorno.Add("cd_acao", int.Parse(contatoExists["cd_acao"]?.ToString() ?? "0"));
                retorno.Add("id_posicao_contato", int.Parse(contatoExists["id_posicao_contato"]?.ToString() ?? "0"));
                retorno.Add("no_pessoa", pessoaExists["no_pessoa"]?.ToString());
                retorno.Add("dt_cadastramento", DateTime.Parse(pessoaExists["dt_cadastramento"]?.ToString() ?? DateTime.MinValue.ToString()));
                retorno.Add("id_status_contato", int.Parse(contatoExists["id_status_contato"]?.ToString() ?? "0"));

                var filtrosEmail = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa), new("cd_tipo_telefone", 4) };
                var emailExists = await SQLServerService.GetFirstByFields(source, "T_Telefone", filtrosEmail);
                if (emailExists != null) retorno.Add("email", emailExists["dc_fone_mail"]?.ToString());

                var filtrosTelefone = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa), new("cd_tipo_telefone", 1) };
                var telefoneExists = await SQLServerService.GetFirstByFields(source, "T_Telefone", filtrosTelefone);
                if (telefoneExists != null) retorno.Add("telefone", telefoneExists["dc_fone_mail"]?.ToString());

                var filtrosCelular = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa), new("cd_tipo_telefone", 3) };
                var celularExists = await SQLServerService.GetFirstByFields(source, "T_Telefone", filtrosCelular);
                if (celularExists != null) retorno.Add("celular", celularExists["dc_fone_mail"]?.ToString());

                var filtrosPessoaFisica = new List<(string campo, object valor)> { new("cd_pessoa_fisica", cd_pessoa) };
                var pessoaFisicaExists = await SQLServerService.GetFirstByFields(source, "T_PESSOA_FISICA", filtrosPessoaFisica);
                if (pessoaFisicaExists != null)
                {
                    var nm_sexo = pessoaFisicaExists["nm_sexo"];
                    retorno.Add("nm_sexo", nm_sexo != null ? int.Parse(nm_sexo.ToString()) : 0);
                    retorno.Add("nm_cpf", pessoaFisicaExists["nm_cpf"]?.ToString() ?? "");
                    retorno.Add("cd_escolaridade", pessoaFisicaExists["cd_escolaridade"] != null ? (int)pessoaFisicaExists["cd_escolaridade"] : 0);
                    retorno.Add("dt_nascimento", pessoaFisicaExists["dt_nascimento"]);
                }

                var filtrosEndereco = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa) };
                var enderecoExists = await SQLServerService.GetFirstByFields(source, "T_ENDERECO", filtrosEndereco);
                if (enderecoExists != null)
                {
                    var cep = enderecoExists["dc_num_cep"];
                    var complemento = enderecoExists["dc_compl_endereco"]?.ToString() ?? "";
                    var numero = enderecoExists["dc_num_endereco"]?.ToString() ?? "";
                    var cd_tipo_logradouro = enderecoExists["cd_tipo_logradouro"]?.ToString() ?? "";
                    var infosCep = await BuscarCEP(cep.ToString(), numero, complemento, cd_tipo_logradouro, source);
                    if (infosCep != null)
                    {
                        retorno.Add("endereco", infosCep);
                    }
                }
                var contatoExistsDetalhe = await SQLServerService.GetFirstByFields(source, "vi_contato_listagem", filtrosContato);
                if (contatoExistsDetalhe != null)
                {
                    retorno.Add("existe_pipeline", contatoExistsDetalhe["existe_pipeline"]);
                }

                return ResponseDefault(retorno);
            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }

        [Authorize]
        [HttpGet]
        [Route("historico-pessoa")]
        public async Task<IActionResult> Gethistorico(string value, SearchModeEnum mode, int? page, int? limit, string sortField, bool sortDesc = false, string ids = "", string searchFields = null)
        {
            var schemaName = "T_Pessoa";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                var historicoResult = await SQLServerService.GetList("vi_historico_pessoa", page, limit, sortField, sortDesc, ids, searchFields, value, source, mode);
                var historico = historicoResult.data;

                var retorno = new
                {
                    data = historico.Select(x =>
                    {
                        return new
                        {
                            cd_historico_pessoa = x["cd_historico_pessoa"],
                            dt_reprogramada_pipeline = x["dt_reprogramada_pipeline"],
                            dt_cadastro_historico = x["dt_cadastro_historico"],
                            dt_realizada_pipeline = x["dt_realizada_pipeline"],
                            cd_etapa_pipeline = x["cd_etapa_pipeline"],
                            cd_pessoa_historico = x["cd_pessoa_historico"],
                            no_pessoa = x["no_pessoa"],
                            tx_obs_historico = x["tx_obs_historico"],
                            cd_acao = x["cd_acao"],
                            cd_contato = x["cd_contato"],
                            id_status_historico = x["id_status_historico"]
                        };
                    }),
                    historicoResult.total,
                    page,
                    limit,
                    pages = limit != null ? (int)Math.Ceiling((double)historicoResult.total / limit.Value) : 0
                };

                return ResponseDefault(retorno);
            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }

        private async Task<(bool sucess, string error, int? pessoaId)> ValidateCommand(InsertPessoaModel command, Source source)
        {
            //valida cpf
            if (command.pessoa.nm_natureza_pessoa == 1)
            {
                if (command.pessoa.nm_cpf != null && !string.IsNullOrEmpty(command.pessoa.nm_cpf))
                {
                    var filtros = new List<(string campo, object valor)> { new("nm_cpf", command.pessoa.nm_cpf) };
                    var cpfExist = await SQLServerService.GetFirstByFields(source, "T_PESSOA_FISICA", filtros);
                    if (cpfExist != null) return (false, $"Já existe um registro com este CPF({command.pessoa.nm_cpf}) cadastrado", null);
                }
            }
            else
            {
                if (command.pessoa.nm_cnpj_cgc != null && !string.IsNullOrEmpty(command.pessoa.nm_cnpj_cgc))
                {
                    var filtros = new List<(string campo, object valor)> { new("dc_num_cnpj_cnab", command.pessoa.nm_cnpj_cgc) };
                    var cpfExist = await SQLServerService.GetFirstByFields(source, "T_PESSOA_JURIDICA", filtros);
                    if (cpfExist != null) return (false, $"Já existe um registro com este CPF({command.pessoa.nm_cnpj_cgc}) cadastrado", null);
                }
            }

            //valida email
            if (command.pessoa.dc_email != null && !string.IsNullOrEmpty(command.pessoa.dc_email))
            {
                var filtros = new List<(string campo, object valor)> { new("dc_fone_mail", command.pessoa.dc_email) };
                var emailExist = await SQLServerService.GetFirstByFields(source, "T_TELEFONE", filtros);
                if (emailExist != null)
                {
                    var responsavelFinanceiro = command.Dependentes.FirstOrDefault(r => r.cd_papel_pai == 9 && r.cd_papel_filho == 3);
                    if (responsavelFinanceiro == null)
                        return (false, $"Já existe um registro com este e-mail({command.pessoa.dc_email}) cadastrado", null);

                    var filtrosResponsavel = new List<(string campo, object valor)> { new("dc_fone_mail", command.pessoa.dc_email),
                            new("cd_pessoa", responsavelFinanceiro.cd_pessoa_filho)};
                    var exists = await SQLServerService.GetFirstByFields(source, "T_TELEFONE", filtrosResponsavel);
                    if (exists == null)
                        return (false, $"Já existe um registro com este e-mail({command.pessoa.dc_email}) cadastrado", null);

                }
            }


            var cd_pessoa_cadastrada = 0;
            try
            {
                var pessoaDict = new Dictionary<string, object>
                {
                    //{ "cd_pessoa", null },
                    { "no_pessoa", command.pessoa.no_pessoa },
                    { "dt_cadastramento", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") },
                    { "dc_reduzido_pessoa", command.pessoa.dc_reduzido_pessoa },
                    { "cd_atividade_principal", command.pessoa.cd_atividade_principal != 0 ? command.pessoa.cd_atividade_principal :null },
                    { "cd_endereco_principal", null }, //cadastrar depois?
                    { "cd_telefone_principal", null }, //cadastrar depois?
                    { "id_pessoa_empresa",0 },
                    { "nm_natureza_pessoa", command.pessoa.nm_natureza_pessoa },
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
                var t_pessoa_insert = await SQLServerService.InsertWithResult("T_PESSOA", pessoaDict, source);
                if (!t_pessoa_insert.success) return new(t_pessoa_insert.success, t_pessoa_insert.error, null);
                var t_pessoa = t_pessoa_insert.inserted;
                var cd_pessoa = t_pessoa["cd_pessoa"];
                cd_pessoa_cadastrada = int.Parse(cd_pessoa.ToString());

                if (command.pessoa.dc_email != null && !string.IsNullOrEmpty(command.pessoa.dc_email))
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
                    var t_telefone_email_insert = await SQLServerService.Insert("T_TELEFONE", telefoneDictEmail, source);
                    if (!t_telefone_email_insert.success) return new(t_telefone_email_insert.success, t_telefone_email_insert.error, null);
                }

                if (command.pessoa.celular != null && !string.IsNullOrEmpty(command.pessoa.celular))
                {
                    //T_TELEFONE(email)
                    var telefoneDictcelular = new Dictionary<string, object>
                    {
                        //{ "cd_telefone", null },
                        { "cd_pessoa", cd_pessoa },
                        { "cd_tipo_telefone", 3 },
                        { "cd_classe_telefone", 1 },
                        { "dc_fone_mail", command.pessoa.celular },
                        { "cd_endereco", null },
                        { "id_telefone_principal",1 },
                        { "cd_operadora", null }
                    };
                    var t_telefone_celular_insert = await SQLServerService.Insert("T_TELEFONE", telefoneDictcelular, source);
                    if (!t_telefone_celular_insert.success) return new(t_telefone_celular_insert.success, t_telefone_celular_insert.error, null);
                }

                if (command.pessoa.telefone != null && !string.IsNullOrEmpty(command.pessoa.telefone))
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

                    var t_telefone_telefone_insert = await SQLServerService.Insert("T_TELEFONE", telefoneDictTelefone, source);
                    if (!t_telefone_telefone_insert.success) return new(t_telefone_telefone_insert.success, t_telefone_telefone_insert.error, null);
                }
                if (command.pessoa.endereco != null && command.pessoa.endereco.cd_loc_estado != null && command.pessoa.endereco.cd_loc_estado != 0)
                {
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
                    var t_endereco_insert = await SQLServerService.Insert("T_ENDERECO", enderecoDict, source);
                    if (!t_endereco_insert.success) return new(t_endereco_insert.success, t_endereco_insert.error, null);
                }

                if (command.pessoa.nm_natureza_pessoa == 1)
                {
                    //T_PESSOA_FISICA
                    var pessoa_fisicaDict = new Dictionary<string, object>
                    {
                        { "cd_pessoa_fisica", cd_pessoa },
                        { "cd_loc_nacionalidade", command.pessoa.cd_loc_nacionalidade },
                        { "nm_sexo", command.pessoa.nm_sexo },
                        { "id_exportado", 0 },
                        { "nm_cpf", command.pessoa.nm_cpf },
                        { "nm_doc_identidade", command.pessoa.nm_doc_identidade },
                        { "cd_escolaridade", command.cd_escolaridade },
                        { "dt_emis_expedidor", command.pessoa.dt_emis_expedidor?.ToString("yyyy-MM-ddTHH:mm:ss") }
                    };
                    if (command.pessoa.dt_nascimento != null) pessoa_fisicaDict.Add("dt_nascimento", command.pessoa.dt_nascimento?.ToString("yyyy-MM-ddTHH:mm:ss"));
                    var t_pessoa_fisica_insert = await SQLServerService.Insert("T_PESSOA_FISICA", pessoa_fisicaDict, source);
                    if (!t_pessoa_fisica_insert.success) return new(t_pessoa_fisica_insert.success, t_pessoa_fisica_insert.error, null);
                }
                else
                {
                    var pessoa_fisicaDict = new Dictionary<string, object>
                    {
                        { "cd_pessoa_fisica", cd_pessoa }
                    };

                    var t_pessoa_fisica_insert = await SQLServerService.Insert("T_PESSOA_FISICA", pessoa_fisicaDict, source);
                    if (!t_pessoa_fisica_insert.success) return new(t_pessoa_fisica_insert.success, t_pessoa_fisica_insert.error, null);

                    var pessoa_juridica_dic = new Dictionary<string, object>
                        {
                            { "cd_pessoa_juridica", cd_pessoa },
                            { "cd_tipo_sociedade", !string.IsNullOrEmpty(command.pessoa.cd_tipo_sociedade) ? command.pessoa.cd_tipo_sociedade : "1" },
                            //{ "dc_num_cgc", "" },
                            //{ "dt_registro_junta_comercial", "2000-01-01" },
                            { "dc_num_insc_estadual", command.pessoa.dc_num_insc_estadual },
                            { "id_exportado", 0 },
                            { "dc_num_insc_municipal", command.pessoa.dc_num_insc_municipal },
                            { "dc_num_cnpj_cnab", command.pessoa.nm_cnpj_cgc },
                            //{ "dc_registro_junta_comercial", "" },
                            //{ "dt_baixa", "[NULO]" },
                            { "dc_nom_presidente", command.pessoa.no_pessoa }
                        };
                    var t_pessoa_juridica_insert = await SQLServerService.Insert("T_PESSOA_JURIDICA", pessoa_juridica_dic, source);
                    if (!t_pessoa_juridica_insert.success) return new(t_pessoa_juridica_insert.success, t_pessoa_juridica_insert.error, null);
                }
                //T_PESSOA_EMPRESA
                var t_pessoa_empresa_dict = new Dictionary<string, object>
                {
                    { "cd_pessoa", cd_pessoa },
                    { "cd_empresa", command.cd_pessoa_escola }
                };
                var t_pessoa_empresa_insert = await SQLServerService.Insert("T_PESSOA_EMPRESA", t_pessoa_empresa_dict, source);
                if (!t_pessoa_empresa_insert.success) return new(t_pessoa_empresa_insert.success, t_pessoa_empresa_insert.error, null);

                if (!command.Dependentes.IsNullOrEmpty())
                {
                    foreach (var dependente in command.Dependentes)
                    {
                        var relacionamento_dict = new Dictionary<string, object>
                        {
                            { "cd_pessoa_filho", dependente.cd_pessoa_filho },
                            { "cd_pessoa_pai", cd_pessoa },
                            { "cd_papel_filho", dependente.cd_papel_filho }
                        };
                        if (dependente.cd_qualif_relacionamento != null) relacionamento_dict.Add("cd_qualif_relacionamento", dependente.cd_qualif_relacionamento);
                        if (dependente.cd_papel_pai != null) relacionamento_dict.Add("cd_papel_pai", dependente.cd_papel_pai);

                        if (relacionamento_dict.Any())
                        {
                            var t_relacionamento_insert = await SQLServerService.Insert("T_RELACIONAMENTO", relacionamento_dict, source);
                            if (!t_relacionamento_insert.success) continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, $"Erro: {ex.Message}", null);
            }

            return (true, string.Empty, cd_pessoa_cadastrada);
        }

        private async Task<(bool sucess, string error)> ValidateCommandContato(ContatoInsertModel model, Source source)
        {
            var cd_pessoa = 0;

            if (model.cd_pessoa_contato == null || model.cd_pessoa_contato == 0)
            {
                if (!string.IsNullOrEmpty(model.nm_cpf))
                {
                    var filtros = new List<(string campo, object valor)> { new("nm_cpf", model.nm_cpf) };
                    var cpfExist = await SQLServerService.GetFirstByFields(source, "T_PESSOA_FISICA", filtros);
                    if (cpfExist != null) return (false, $"nm_cpf ja existente({model.nm_cpf})");
                }
                if (!string.IsNullOrEmpty(model.email))
                {
                    var filtrosEmail = new List<(string campo, object valor)> { new("dc_fone_mail", model.email), new("cd_tipo_telefone", 4) };
                    var emailExists = await SQLServerService.GetFirstByFields(source, "T_Telefone", filtrosEmail);
                    if (emailExists != null) return (false, "email ja existente.");
                }

                //Cadastrar pessoa
                var pessoaDict = new Dictionary<string, object>
                {
                    //{ "cd_pessoa", null },
                    { "no_pessoa", model.no_pessoa },
                    { "dt_cadastramento", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") },
                    { "id_pessoa_empresa",0 },
                    { "nm_natureza_pessoa",1 },
                    { "id_exportado", 0 }
                };

                var t_pessoa_insert = await SQLServerService.InsertWithResult("T_PESSOA", pessoaDict, source);
                if (!t_pessoa_insert.success) return new(t_pessoa_insert.success, t_pessoa_insert.error);
                var t_pessoa = t_pessoa_insert.inserted;
                cd_pessoa = int.Parse(t_pessoa["cd_pessoa"].ToString());

                //Cadastrar pessoa fisica
                var pessoa_fisicaDict = new Dictionary<string, object>
                    {
                        { "cd_pessoa_fisica", cd_pessoa },
                        { "dt_nascimento", model.dt_nascimento?.ToString("yyyy-MM-ddTHH:mm:ss") },
                        { "id_exportado", 0 },
                        { "cd_escolaridade", model.cd_escolaridade },
                        { "nm_cpf", model.nm_cpf },
                        { "nm_sexo", model.nm_sexo}
                    };
                var t_pessoa_fisica_insert = await SQLServerService.Insert("T_PESSOA_FISICA", pessoa_fisicaDict, source);
                if (!t_pessoa_fisica_insert.success) return new(t_pessoa_fisica_insert.success, t_pessoa_fisica_insert.error);

                //Cadastrar endereço
                if (model.endereco != null)
                {
                    var enderecoDict = new Dictionary<string, object>
                {
                    //{ "cd_endereco", command.pessoa.endereco.cd_endereco },
                    { "cd_pessoa", cd_pessoa },
                    { "cd_loc_pais", model.endereco.cd_loc_pais },
                    { "cd_loc_estado",  model.endereco.cd_loc_estado },
                    { "cd_loc_cidade",  model.endereco.cd_loc_cidade },
                    { "cd_tipo_endereco",  model.endereco.cd_tipo_endereco},
                    //{ "cd_loc_distrito", null },
                    { "cd_loc_bairro",  model.endereco.cd_loc_bairro },
                    { "cd_tipo_logradouro", model.endereco.cd_tipo_logradouro},
                    { "cd_loc_logradouro",  model.endereco.cd_loc_logradouro },
                    //{ "nm_caixa_postal", command.pessoa.endereco.nm_caixa_postal },
                    //{ "nm_status_endereco", command.pessoa.endereco.nm_status_endereco },
                    { "dc_compl_endereco",  model.endereco.dc_compl_endereco },
                    { "id_exportado", 0 },
                    { "dc_num_cep",  model.endereco.dc_num_cep },
                    { "dc_num_endereco",   model.endereco.dc_num_endereco },
                    //{ "dc_num_local_geografico", command.pessoa.endereco.dc_num_local_geografico}
                };
                    var t_endereco_insert = await SQLServerService.Insert("T_ENDERECO", enderecoDict, source);
                    if (!t_endereco_insert.success) return new(t_endereco_insert.success, t_endereco_insert.error);
                }
                //cadastrar T_Telefone
                if (model.email != null)
                {
                    //T_TELEFONE(email)
                    var telefoneDictEmail = new Dictionary<string, object>
                    {
                        //{ "cd_telefone", null },
                        { "cd_pessoa", cd_pessoa },
                        { "cd_tipo_telefone", 4 },
                        { "cd_classe_telefone", 1 },
                        { "dc_fone_mail", model.email },
                        { "cd_endereco", null },
                        { "id_telefone_principal",0 },
                        { "cd_operadora", null }
                    };
                    var t_telefone_email_insert = await SQLServerService.Insert("T_TELEFONE", telefoneDictEmail, source);
                    if (!t_telefone_email_insert.success) return new(t_telefone_email_insert.success, t_telefone_email_insert.error);
                }

                if (model.telefone != null)
                {
                    //T_TELEFONE(telefone)
                    var telefoneDictTelefone = new Dictionary<string, object>
                    {
                        //{ "cd_telefone", null },
                        { "cd_pessoa", cd_pessoa },
                        { "cd_tipo_telefone", 1 },
                        { "cd_classe_telefone", 1 },
                        { "dc_fone_mail", model.telefone },
                        { "cd_endereco", null },
                        { "id_telefone_principal", 1 },
                        { "cd_operadora", null }
                    };

                    var t_telefone_telefone_insert = await SQLServerService.Insert("T_TELEFONE", telefoneDictTelefone, source);
                    if (!t_telefone_telefone_insert.success) return new(t_telefone_telefone_insert.success, t_telefone_telefone_insert.error);
                }

                if (model.celular != null)
                {
                    //T_TELEFONE(celular)
                    var telefoneDictCelular = new Dictionary<string, object>
                    {
                        //{ "cd_telefone", null },
                        { "cd_pessoa", cd_pessoa },
                        { "cd_tipo_telefone", 3 },
                        { "cd_classe_telefone", 1 },
                        { "dc_fone_mail", model.celular },
                        { "cd_endereco", null },
                        { "id_telefone_principal", 1 },
                        { "cd_operadora", null }
                    };

                    var t_telefone_celular_insert = await SQLServerService.Insert("T_TELEFONE", telefoneDictCelular, source);
                    if (!t_telefone_celular_insert.success) return new(t_telefone_celular_insert.success, t_telefone_celular_insert.error);
                }
            }

            //cadastrar Contato
            var contato_dict = new Dictionary<string, object>
            {
                { "cd_pessoa_contato", cd_pessoa },
                { "cd_acao", model.cd_acao },
                { "cd_pessoa_escola", model.cd_pessoa_escola },
                { "id_posicao_contato", model.id_posicao_contato },
                { "id_status_contato", model.id_status_contato }
            };
            var contato_insert = await SQLServerService.InsertWithResult("T_CONTATO", contato_dict, source);
            if (!contato_insert.success) return new(contato_insert.success, contato_insert.error);
            var cd_contato = contato_insert.inserted["cd_contato"];

            return (true, cd_contato?.ToString() ?? "");
        }

        private async Task<(bool sucess, string error)> ProcessaUpdateContato(int cd_contato, ContatoInsertModel model, Source source)
        {
            var filtrosContato = new List<(string campo, object valor)> { new("cd_contato", cd_contato) };
            var contatoExists = await SQLServerService.GetFirstByFields(source, "vi_contato_listagem", filtrosContato);
            if (contatoExists == null) return (false, "contato não encontrado");
            var cd_pessoa = contatoExists["cd_pessoa_contato"];

            if (model.nm_cpf != null && !string.IsNullOrEmpty(model.nm_cpf))
            {
                var filtros = new List<(string campo, object valor)> { new("nm_cpf", model.nm_cpf) };
                var cpfExist = await SQLServerService.GetFirstByFields(source, "T_PESSOA_FISICA", filtros);
                if (cpfExist != null)
                {
                    var cd_pessoa_cpf = cpfExist["cd_pessoa_fisica"];
                    if (cd_pessoa_cpf != null && cd_pessoa_cpf.ToString() != cd_pessoa.ToString())
                    {
                        return (false, $"Já existe um registro com este CPF({model.nm_cpf}) cadastrado");
                    }
                }
            }

            //Cadastrar pessoa
            var pessoaDict = new Dictionary<string, object>
                {
                    //{ "cd_pessoa", null },
                    { "no_pessoa", model.no_pessoa },
                    { "dt_cadastramento", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") },
                    { "id_pessoa_empresa",0 },
                    { "nm_natureza_pessoa",1 },
                    { "id_exportado", 0 }
                };

            var t_pessoa_insert = await SQLServerService.Update("T_PESSOA", pessoaDict, source, "cd_pessoa", cd_pessoa);
            if (!t_pessoa_insert.success) return new(t_pessoa_insert.success, t_pessoa_insert.error);

            //Cadastrar pessoa fisica
            var pessoa_fisicaDict = new Dictionary<string, object>
            {
                { "dt_nascimento", model.dt_nascimento?.ToString("yyyy-MM-ddTHH:mm:ss") },
                { "id_exportado", 0 },
                { "cd_escolaridade", model.cd_escolaridade },
                { "nm_sexo", model.nm_sexo},
                { "nm_cpf", model.nm_cpf }
            };
            var t_pessoa_fisica_insert = await SQLServerService.Update("T_PESSOA_FISICA", pessoa_fisicaDict, source, "cd_pessoa_fisica", cd_pessoa);
            if (!t_pessoa_fisica_insert.success) return new(t_pessoa_fisica_insert.success, t_pessoa_fisica_insert.error);

            //cadastrar Contato
            var contato_dict = new Dictionary<string, object>
            {
                { "cd_pessoa_contato", cd_pessoa },
                { "cd_acao", model.cd_acao },
                { "cd_pessoa_escola", model.cd_pessoa_escola },
                { "id_posicao_contato", model.id_posicao_contato },
                { "id_status_contato", model.id_status_contato }
            };
            var contato_insert = await SQLServerService.Update("T_CONTATO", contato_dict, source, "cd_contato", cd_contato);
            if (!contato_insert.success) return new(contato_insert.success, contato_insert.error);
            //Cadastrar endereço
            var filtrosEndereco = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa) };
            var enderecoExists = await SQLServerService.GetFirstByFields(source, "T_ENDERECO", filtrosEndereco);
            if (enderecoExists != null)
            {
                //T_ENDERECO
                var enderecoDict = new Dictionary<string, object>
                    {
                        { "cd_pessoa", cd_pessoa },
                        { "cd_loc_pais", model.endereco.cd_loc_pais },
                        { "cd_loc_estado", model.endereco.cd_loc_estado },
                        { "cd_loc_cidade", model.endereco.cd_loc_cidade },
                        { "cd_tipo_endereco", model.endereco.cd_tipo_endereco},
                        { "cd_loc_bairro", model.endereco.cd_loc_bairro },
                        { "cd_tipo_logradouro",model.endereco.cd_tipo_logradouro},
                        { "cd_loc_logradouro", model.endereco.cd_loc_logradouro },
                        { "dc_compl_endereco", model.endereco.dc_compl_endereco },
                        { "id_exportado", 0 },
                        { "dc_num_cep", model.endereco.dc_num_cep },
                        { "dc_num_endereco",   model.endereco.dc_num_endereco },
                    };
                var t_endereco_insert = await SQLServerService.Update("T_ENDERECO", enderecoDict, source, "cd_endereco", enderecoExists["cd_endereco"]);
                if (!t_endereco_insert.success) return new(t_endereco_insert.success, t_endereco_insert.error);
            }
            else if (model.endereco != null)
            {
                var enderecoDict = new Dictionary<string, object>
                {
                    //{ "cd_endereco", command.pessoa.endereco.cd_endereco },
                    { "cd_pessoa", cd_pessoa },
                    { "cd_loc_pais", model.endereco.cd_loc_pais },
                    { "cd_loc_estado", model.endereco.cd_loc_estado },
                    { "cd_loc_cidade", model.endereco.cd_loc_cidade },
                    { "cd_tipo_endereco", model.endereco.cd_tipo_endereco},
                    //{ "cd_loc_distrito", null },
                    { "cd_loc_bairro", model.endereco.cd_loc_bairro },
                    { "cd_tipo_logradouro",model.endereco.cd_tipo_logradouro},
                    { "cd_loc_logradouro", model.endereco.cd_loc_logradouro },
                    //{ "nm_caixa_postal", command.pessoa.endereco.nm_caixa_postal },
                    //{ "nm_status_endereco", command.pessoa.endereco.nm_status_endereco },
                    { "dc_compl_endereco", model.endereco.dc_compl_endereco },
                    { "id_exportado", 0 },
                    { "dc_num_cep", model.endereco.dc_num_cep },
                    { "dc_num_endereco",   model.endereco.dc_num_endereco },
                    //{ "dc_num_local_geografico", command.pessoa.endereco.dc_num_local_geografico}
                };
                var t_endereco_insert = await SQLServerService.Insert("T_ENDERECO", enderecoDict, source);
                if (!t_endereco_insert.success) return new(t_endereco_insert.success, t_endereco_insert.error);
            }

            if (model.email != null)
            {
                var filtrosEmail = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa), new("cd_tipo_telefone", 4) };
                var emailExists = await SQLServerService.GetFirstByFields(source, "T_Telefone", filtrosEmail);
                var telefoneDictEmail = new Dictionary<string, object>
                        {
                            { "cd_pessoa", cd_pessoa },
                            { "cd_tipo_telefone", 4 },
                            { "cd_classe_telefone", 1 },
                            { "dc_fone_mail",model.email },
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
            if (model.telefone != null)
            {
                var filtrosTelefone = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa), new("cd_tipo_telefone", 1) };
                var telefoneExists = await SQLServerService.GetFirstByFields(source, "T_Telefone", filtrosTelefone);
                var telefoneDictTelefone = new Dictionary<string, object>
                        {
                            { "cd_pessoa", cd_pessoa },
                            { "cd_tipo_telefone", 1 },
                            { "cd_classe_telefone", 1 },
                            { "dc_fone_mail", model.telefone },
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

            if (model.celular != null)
            {
                var filtrosCelular = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa), new("cd_tipo_telefone", 3) };
                var celularExists = await SQLServerService.GetFirstByFields(source, "T_Telefone", filtrosCelular);
                var telefoneDictCelular = new Dictionary<string, object>
                    {
                        { "cd_pessoa", cd_pessoa },
                        { "cd_tipo_telefone", 3 },
                        { "cd_classe_telefone", 1 },
                        { "dc_fone_mail", model.celular},
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

            return (true, string.Empty);
        }

        private async Task<(bool sucess, string error)> ProcessaUpdatePessoa(int cd_pessoa, InsertPessoaModel command, Source source)
        {
            ////valida cpf
            if (command.pessoa.nm_natureza_pessoa == 1)
            {
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
            }
            else
            {
                if (command.pessoa.nm_cnpj_cgc != null && !string.IsNullOrEmpty(command.pessoa.nm_cnpj_cgc))
                {
                    var filtros = new List<(string campo, object valor)> { new("dc_num_cnpj_cnab", command.pessoa.nm_cnpj_cgc) };
                    var cnpjExist = await SQLServerService.GetFirstByFields(source, "T_PESSOA_JURIDICA", filtros);
                    if (cnpjExist != null)
                    {
                        var cd_pessoa_cnpj = cnpjExist["cd_pessoa_juridica"];
                        if (cd_pessoa_cnpj != null && cd_pessoa_cnpj.ToString() != cd_pessoa.ToString())
                        {
                            return (false, $"Já existe um registro com este CNPJ({command.pessoa.nm_cnpj_cgc}) cadastrado");
                        }
                    }
                }
            }

            //valida email
            if (!command.pessoa.dc_email.IsNullOrEmpty())
            {
                var filtros = new List<(string campo, object valor)> { new("dc_fone_mail", command.pessoa.dc_email) };
                var emailExist = await SQLServerService.GetFirstByFields(source, "T_TELEFONE", filtros);
                if (emailExist != null)
                {
                    var cd_pessoa_email = emailExist["cd_pessoa"];
                    if (cd_pessoa_email != null && cd_pessoa_email.ToString() != cd_pessoa.ToString())
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
                    { "cd_endereco_principal", null }, //cadastrar depois?
                    { "cd_telefone_principal", null }, //cadastrar depois?
                    { "id_pessoa_empresa",0 },
                    { "nm_natureza_pessoa",command.pessoa.nm_natureza_pessoa },
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
                if (command.pessoa.telefone != null)
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

                if (command.pessoa.celular != null)
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

                if (command.pessoa.nm_natureza_pessoa == 1)
                {
                    //T_PESSOA_FISICA
                    var filtrosPessoaFisica = new List<(string campo, object valor)> { new("cd_pessoa_fisica", cd_pessoa) };
                    var pessoaFisicaExists = await SQLServerService.GetFirstByFields(source, "T_PESSOA_FISICA", filtrosPessoaFisica);
                    if (pessoaFisicaExists != null)
                    {
                        //T_PESSOA_FISICA
                        var pessoa_fisicaDict = new Dictionary<string, object>
                        {
                         { "cd_pessoa_fisica", cd_pessoa },
                        { "cd_loc_nacionalidade", command.pessoa.cd_loc_nacionalidade },
                        { "nm_sexo", command.pessoa.nm_sexo },
                        { "id_exportado", 0 },
                        { "nm_cpf", command.pessoa.nm_cpf },
                        { "nm_doc_identidade", command.pessoa.nm_doc_identidade },
                        { "cd_escolaridade", command.cd_escolaridade },
                        { "dt_emis_expedidor", command.pessoa.dt_emis_expedidor?.ToString("yyyy-MM-ddTHH:mm:ss") }
                        };
                        if (command.pessoa.dt_nascimento != null) pessoa_fisicaDict.Add("dt_nascimento", command.pessoa.dt_nascimento);

                        var t_pessoa_fisica_insert = await SQLServerService.Update("T_PESSOA_FISICA", pessoa_fisicaDict, source, "cd_pessoa_fisica", cd_pessoa);
                        if (!t_pessoa_fisica_insert.success) return new(t_pessoa_fisica_insert.success, t_pessoa_fisica_insert.error);
                    }
                }
                else
                {
                    var filtrosPessoaJuridica = new List<(string campo, object valor)> { new("cd_pessoa_juridica", cd_pessoa) };
                    var pessoaJuridicaExists = await SQLServerService.GetFirstByFields(source, "T_PESSOA_JURIDICA", filtrosPessoaJuridica);
                    if (pessoaJuridicaExists != null)
                    {
                        var pessoa_juridica_dic = new Dictionary<string, object>
                        {
                            { "dc_num_insc_estadual", command.pessoa.dc_num_insc_estadual },
                            { "id_exportado", 0 },
                            { "dc_num_insc_municipal", command.pessoa.dc_num_insc_municipal },
                            { "dc_num_cnpj_cnab", command.pessoa.nm_cnpj_cgc },
                            { "dc_nom_presidente", command.pessoa.no_pessoa }
                        };
                        var t_pessoa_juridica_insert = await SQLServerService.Update("T_PESSOA_JURIDICA", pessoa_juridica_dic, source, "cd_pessoa_juridica", cd_pessoa);
                        if (!t_pessoa_juridica_insert.success) return new(t_pessoa_juridica_insert.success, t_pessoa_juridica_insert.error);
                    }
                }

                if (!command.Dependentes.IsNullOrEmpty())
                {
                    //remove relacionamentos e cadastra novamente.
                    await SQLServerService.Delete("T_RELACIONAMENTO", "cd_pessoa_pai", cd_pessoa.ToString(), source);

                    foreach (var dependente in command.Dependentes)
                    {
                        var relacionamento_dict = new Dictionary<string, object>
                        {
                            { "cd_pessoa_filho", dependente.cd_pessoa_filho },
                            { "cd_pessoa_pai", cd_pessoa },
                            { "cd_papel_filho", dependente.cd_papel_filho }
                        };
                        if (dependente.cd_qualif_relacionamento != null) relacionamento_dict.Add("cd_qualif_relacionamento", dependente.cd_qualif_relacionamento);
                        if (dependente.cd_papel_pai != null) relacionamento_dict.Add("cd_papel_pai", dependente.cd_papel_pai);

                        if (relacionamento_dict.Any())
                        {
                            var t_relacionamento_insert = await SQLServerService.Insert("T_RELACIONAMENTO", relacionamento_dict, source);
                            if (!t_relacionamento_insert.success) continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, $"Erro: {ex.Message}");
            }

            return (true, string.Empty);
        }

        private async Task<dynamic?> BuscarCEP(string cep, string numero, string complemento, string? cd_tipo_logradouro, Source source)
        {
            if (cep.IsNullOrEmpty()) return null;

            if (source != null && source.Active != null && source.Active == true)
            {
                var cepTratado = cep.Replace(".", "").Replace("-", "").Insert(5, "-");

                var filtrosCep = new List<(string campo, object valor)> { new("dc_num_cep", cepTratado) };
                var cepExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtrosCep);
                if (cepExists == null) return null;

                var filtroBairro = new List<(string campo, object valor)> { new("cd_localidade", cepExists["cd_loc_relacionada"].ToString()) };
                var bairroExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroBairro);
                if (bairroExists == null) return null;

                var filtroCidade = new List<(string campo, object valor)> { new("cd_localidade", bairroExists["cd_loc_relacionada"].ToString()) };
                var cidadeExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroCidade);
                if (cidadeExists == null) return null;

                var filtroEstado = new List<(string campo, object valor)> { new("cd_localidade", cidadeExists["cd_loc_relacionada"].ToString()) };
                var estadoExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroEstado);
                if (estadoExists == null) return null;

                var filtroPais = new List<(string campo, object valor)> { new("cd_localidade", estadoExists["cd_loc_relacionada"].ToString()) };
                var paisExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroPais);
                if (paisExists == null) return null;

                var retorno = new
                {
                    cd_bairro = bairroExists["cd_localidade"],
                    cd_cidade = cidadeExists["cd_localidade"],
                    cd_estado = estadoExists["cd_localidade"],
                    cd_logradouro = cepExists["cd_localidade"],
                    cd_pais = paisExists["cd_localidade"],
                    cd_tipo_logradouro = cd_tipo_logradouro,
                    dc_num_cep = cep,
                    no_bairro = bairroExists["no_localidade"],
                    no_cidade = cidadeExists["no_localidade"],
                    no_estado = estadoExists["no_localidade"],
                    no_logradouro = cepExists["no_localidade"],
                    no_pais = paisExists["no_localidade"],
                    dc_compl_endereco = complemento,
                    dc_num_endereco = numero
                };

                return retorno;
            }
            return null;
        }
    }
}