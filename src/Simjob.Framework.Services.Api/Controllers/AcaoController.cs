using Flurl.Http;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OfficeOpenXml;
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
using Simjob.Framework.Services.Api.Models.Acao;
using Simjob.Framework.Services.Api.Services;
using Simjob.Framework.Services.Api.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Simjob.Framework.Services.Api.Controllers
{
    public class AcaoController : BaseController
    {
        private readonly IRepository<SourceContext, Source> _sourceRepository;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;
        private readonly FiskLpConfig _fiskLpConfig;

        public AcaoController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, IRepository<SourceContext, Source> sourceRepository, IRepository<MongoDbContext, Schema> schemaRepository, IOptions<FiskLpConfig> fiskLpConfig) : base(bus, notifications)
        {
            _sourceRepository = sourceRepository;
            _schemaRepository = schemaRepository;
            _fiskLpConfig = fiskLpConfig.Value;
        }

        /// <summary>
        /// Importação de novos contatos de um arquivo xlsx no formato base64.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("import")]
        public async Task<IActionResult> Import([FromBody] ImportAcaoModel model)
        {
            var schemaName = "T_Pessoa";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                var arquivo = Util.GetArquivoMemoryStream(model.Arquivo);
                if (arquivo.FileExtension == "xlsx")
                {
                    using (var stream = arquivo.MemoryStream)
                    {
                        var result = await ImportAcao(stream, model.Cd_acao, model.Cd_pessoa_escola, source);
                        return result.success ? ResponseDefault(result.erros.Select(x => x)) : BadRequest(result.erros.Select(x => x));
                    }
                }
                else
                {
                    return BadRequest("formato invalido de arquivo.");
                }
            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }

        [Authorize]
        [HttpGet()]
        public async Task<IActionResult> GetAll(string value, SearchModeEnum mode, int? page, int? limit, string sortField, bool sortDesc = false, string ids = "", string searchFields = null, string? cd_empresa = null, DateTime? dataInicio = null, DateTime? dataFim = null, string? pipelinesIds = null,string? campo_data = null)
        {
            if (cd_empresa == null) return BadRequest("campo cd_empresa não informado");
            var schemaName = "T_Acao";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                var idsAcoes = ids;
                if (!pipelinesIds.IsNullOrEmpty())
                {
                    var pipelinesGet = await SQLServerService.GetList("T_PIPELINE", pipelinesIds, "cd_pipeline", null, source, SearchModeEnum.Equals);
                    if (pipelinesGet.success)
                    {
                        var pipelines = pipelinesGet.data;
                        string ids_acoes = string.Join(",", pipelines
                           .Where(d => d.ContainsKey("cd_acao") && d["cd_acao"] != null)
                           .Select(d => d["cd_acao"].ToString()));
                        if (idsAcoes.IsNullOrEmpty()) idsAcoes = ids_acoes;
                        else idsAcoes = $"{idsAcoes},{ids_acoes}";
                    }
                }
                var acaoResult = await SQLServerService.GetListFiltroData("vi_acao_listagem", page, limit, sortField, sortDesc, idsAcoes, "cd_acao", searchFields, value, source, mode, "cd_pessoa_escola", cd_empresa, campo_data,campo_data, dataInicio, dataFim);
                if (acaoResult.success)
                {
                    var acoes = acaoResult.data;

                    var retorno = new
                    {
                        data = acoes,
                        acaoResult.total,
                        page,
                        limit,
                        pages = limit != null ? (int)Math.Ceiling((double)acaoResult.total / limit.Value) : 0
                    };

                    return ResponseDefault(retorno);
                }
                return BadRequest(new
                {
                    sucess = false,
                    error = acaoResult.error
                });
            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }

        private async Task<(bool success, List<string> erros)> ImportAcao(MemoryStream arquivo, int cd_acao, int cd_pessoa_escola, Source source)
        {
            var errosLeitura = new List<string>();
            ExcelPackage.License.SetNonCommercialPersonal("Evo");
            var importModels = new List<ImportAcaoInfoModel>();
            using (var package = new ExcelPackage())
            {
                package.Load(arquivo);
                var worksheet = package.Workbook.Worksheets[0];

                for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[row, 1].Text)) continue;
                    var acaoModel = new ImportAcaoInfoModel();

                    var errosLinha = new List<string>();
                    acaoModel.Nome = worksheet.Cells[row, 1].Text.Trim();
                    acaoModel.Email = worksheet.Cells[row, 2].Text.Trim();
                    acaoModel.Genero = worksheet.Cells[row, 3].Text.Trim();
                    acaoModel.Telefone = worksheet.Cells[row, 4].Text.Trim();
                    acaoModel.Celular = worksheet.Cells[row, 5].Text.Trim();
                    acaoModel.Cpf = worksheet.Cells[row, 6].Text.Trim();
                    acaoModel.Escolaridade = worksheet.Cells[row, 7].Text.Trim();
                    if (DateTime.TryParse(worksheet.Cells[row, 8].Text.Trim(), out DateTime parsedDate))
                    {
                        acaoModel.DataNascimento = parsedDate;
                    }
                    else
                    {
                        errosLinha.Add($"Linha {row}: Data de nascimento inválida.");
                    }
                    acaoModel.Numero = worksheet.Cells[row, 14].Text.Trim();
                    acaoModel.Cep = worksheet.Cells[row, 15].Text.Trim();
                    acaoModel.Complemento = worksheet.Cells[row, 16].Text.Trim();
                    acaoModel.TipoLogradouro = worksheet.Cells[row, 13].Text.Trim();
                    if (string.IsNullOrEmpty(acaoModel.Nome)) errosLinha.Add($"Linha {row}: Nome não pode ser vazio.");
                    if (string.IsNullOrEmpty(acaoModel.Cpf)) errosLinha.Add($"Linha {row}: CPF não pode ser vazio.");
                    if (errosLinha.Any())
                    {
                        errosLeitura.AddRange(errosLinha);
                        continue;
                    }
                    importModels.Add(acaoModel);
                }
            }

            if (errosLeitura.Count > 0)
            {
                return (false, errosLeitura);
            }

            foreach (var model in importModels)
            {
                var processaResult = await ProcessaImportAcaoModel(model, cd_acao, cd_pessoa_escola, source);
                if (!processaResult.success)
                {
                    errosLeitura.Add(processaResult.erro ?? "Erro desconhecido ao processar o modelo.");
                }
            }

            return (true, errosLeitura);
        }

        private async Task<(bool success, string? erro)> ProcessaImportAcaoModel(ImportAcaoInfoModel model, int cd_acao, int cd_pessoa_escola, Source source)
        {
            var cd_pessoa = 0;
            if (model.Cpf != null && !string.IsNullOrEmpty(model.Cpf))
            {
                var filtros = new List<(string campo, object valor)> { new("nm_cpf", model.Cpf) };
                var cpfExist = await SQLServerService.GetFirstByFields(source, "T_PESSOA_FISICA", filtros);
                if (cpfExist != null) cd_pessoa = int.Parse(cpfExist["cd_pessoa_fisica"].ToString());
            }
            if (!model.Email.IsNullOrEmpty())
            {
                var emailExist = await SQLServerService.GetFirstByFields(source, "T_TELEFONE", new List<(string campo, object valor)> { new("dc_fone_mail", model.Email) });
                if(emailExist != null) cd_pessoa = int.Parse(emailExist["cd_pessoa"].ToString());
            }

           
            if(cd_pessoa == 0)
            {
                //Cadastrar pessoa
                var pessoaDict = new Dictionary<string, object>
                {
                    //{ "cd_pessoa", null },
                    { "no_pessoa", model.Nome },
                    { "dt_cadastramento", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") },
                    { "id_pessoa_empresa",0 },
                    { "nm_natureza_pessoa",1 },
                    { "id_exportado", 0 }
                };

                var t_pessoa_insert = await SQLServerService.InsertWithResult("T_PESSOA", pessoaDict, source);
                if (!t_pessoa_insert.success) return new(t_pessoa_insert.success, t_pessoa_insert.error);
                var t_pessoa = t_pessoa_insert.inserted;
                cd_pessoa = int.Parse(t_pessoa["cd_pessoa"]?.ToString() ?? "0");
                //Cadastrar pessoa fisica
                var pessoa_fisicaDict = new Dictionary<string, object>
                    {
                        { "nm_cpf", model.Cpf },
                        { "cd_pessoa_fisica", cd_pessoa },
                        { "dt_nascimento", model.DataNascimento.Date.ToString("yyyy-MM-ddTHH:mm:ss") },
                        { "id_exportado", 0 },
                        {"cd_escolaridade",string.IsNullOrEmpty(model.Escolaridade)?null:model.Escolaridade }
                    };
                var t_pessoa_fisica_insert = await SQLServerService.Insert("T_PESSOA_FISICA", pessoa_fisicaDict, source);
                if (!t_pessoa_fisica_insert.success) return new(t_pessoa_fisica_insert.success, t_pessoa_fisica_insert.error);

                //Cadastrar endereço
                if (!string.IsNullOrEmpty(model.Cep))
                {
                    var infosCep = await BuscarCEP(model.Cep, source);
                    if (infosCep == null) return new(false, $"CEP {model.Cep} não encontrado ou inválido.");
                    var enderecoDict = new Dictionary<string, object>
                {
                    { "cd_pessoa", cd_pessoa },
                    { "cd_loc_pais", infosCep.cd_pais },
                    { "cd_loc_estado",  infosCep.cd_estado },
                    { "cd_loc_cidade", infosCep.cd_cidade },
                    { "cd_loc_bairro", infosCep.cd_bairro },
                    { "cd_tipo_logradouro", infosCep.cd_tipo_logradouro},
                    { "cd_loc_logradouro",   infosCep.cd_logradouro },
                    { "dc_compl_endereco",  model.Complemento },
                    { "cd_tipo_endereco",  string.IsNullOrEmpty(model.TipoLogradouro)?2:model.TipoLogradouro},
                    { "id_exportado", 0 },
                    { "dc_num_cep",  model.Cep },
                    { "dc_num_endereco",   model.Numero }
                };
                    var t_endereco_insert = await SQLServerService.Insert("T_ENDERECO", enderecoDict, source);
                    if (!t_endereco_insert.success) return new(t_endereco_insert.success, t_endereco_insert.error);
                }

                //cadastrar T_Telefone
                if (!string.IsNullOrEmpty(model.Email))
                {
                    //T_TELEFONE(email)
                    var telefoneDictEmail = new Dictionary<string, object>
                    {
                        { "cd_pessoa", cd_pessoa },
                        { "cd_tipo_telefone", 4 },
                        { "cd_classe_telefone", 1 },
                        { "dc_fone_mail", model.Email },
                        { "cd_endereco", null },
                        { "id_telefone_principal",1 },
                        { "cd_operadora", null }
                    };
                    var t_telefone_email_insert = await SQLServerService.Insert("T_TELEFONE", telefoneDictEmail, source);
                    if (!t_telefone_email_insert.success) return new(t_telefone_email_insert.success, t_telefone_email_insert.error);
                }

                if (!string.IsNullOrEmpty(model.Telefone))
                {
                    //T_TELEFONE(telefone)
                    var telefoneDictTelefone = new Dictionary<string, object>
                    {
                        { "cd_pessoa", cd_pessoa },
                        { "cd_tipo_telefone", 1 },
                        { "cd_classe_telefone", 1 },
                        { "dc_fone_mail", model.Telefone },
                        { "cd_endereco", null },
                        { "id_telefone_principal", 1 },
                        { "cd_operadora", null }
                    };

                    var t_telefone_telefone_insert = await SQLServerService.Insert("T_TELEFONE", telefoneDictTelefone, source);
                    if (!t_telefone_telefone_insert.success) return new(t_telefone_telefone_insert.success, t_telefone_telefone_insert.error);
                }

                if (!string.IsNullOrEmpty(model.Celular))
                {
                    //T_TELEFONE(celular)
                    var telefoneDictCelular = new Dictionary<string, object>
                    {
                        { "cd_pessoa", cd_pessoa },
                        { "cd_tipo_telefone", 3 },
                        { "cd_classe_telefone", 1 },
                        { "dc_fone_mail", model.Celular },
                        { "cd_endereco", null },
                        { "id_telefone_principal", 1 },
                        { "cd_operadora", null }
                    };

                    var t_telefone_celular_insert = await SQLServerService.Insert("T_TELEFONE", telefoneDictCelular, source);
                    if (!t_telefone_celular_insert.success) return new(t_telefone_celular_insert.success, t_telefone_celular_insert.error);
                }

                //T_PESSOA_EMPRESA
                var t_pessoa_empresa_dict = new Dictionary<string, object>
                {
                    { "cd_pessoa", cd_pessoa },
                    { "cd_empresa", cd_pessoa_escola }
                };
                var t_pessoa_empresa_insert = await SQLServerService.Insert("T_PESSOA_EMPRESA", t_pessoa_empresa_dict, source);
                if (!t_pessoa_empresa_insert.success) return new(t_pessoa_empresa_insert.success, t_pessoa_empresa_insert.error);

            }
            var contato_existe = await SQLServerService.GetFirstByFields(source, "T_CONTATO", new List<(string campo, object valor)> { new("cd_pessoa_contato", cd_pessoa), new("cd_acao", cd_acao), new("cd_pessoa_escola", cd_pessoa_escola) });
            if (contato_existe != null) return (new(false, $"contato ja cadastrado para cd_pessoa({cd_pessoa}),cd_acao({cd_acao})"));
            //cadastrar Contato
            var contato_dict = new Dictionary<string, object>
            {
                { "cd_pessoa_contato", cd_pessoa },
                { "cd_acao", cd_acao },
                { "cd_pessoa_escola", cd_pessoa_escola },
                { "id_posicao_contato", 1 },
                { "id_status_contato", 1 }
            };
            var contato_insert = await SQLServerService.Insert("T_CONTATO", contato_dict, source);
            if (!contato_insert.success) return new(contato_insert.success, contato_insert.error);
           

            return (new(true, null));
        }

        private async Task<InfosCEP?> BuscarCEP(string cep, Source source)
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

                var retorno = new InfosCEP
                {
                    cd_bairro = bairroExists["cd_localidade"].ToString(),
                    cd_cidade = cidadeExists["cd_localidade"].ToString(),
                    cd_estado = estadoExists["cd_localidade"].ToString(),
                    cd_logradouro = cepExists["cd_localidade"].ToString(),
                    cd_pais = paisExists["cd_localidade"].ToString(),
                    cd_tipo_logradouro = cepExists["cd_tipo_localidade"].ToString()
                };

                return retorno;
            }
            return null;
        }

        [Authorize]
        [HttpPatch()]
        [Route("{cd_acao}")]
        public async Task<IActionResult> Patch(int cd_acao)
        {
            var schemaName = "T_Acao";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                var filtrosAcao = new List<(string campo, object valor)> { new("cd_acao", cd_acao) };
                var acaoExists = await SQLServerService.GetFirstByFields(source, "T_ACAO", filtrosAcao);
                if (acaoExists == null) return NotFound("contato não encontrado");

                var acaoDict = new Dictionary<string, object> { };
                if (int.Parse(acaoExists["id_status_acao"].ToString()) == 1)
                {
                    acaoDict = new Dictionary<string, object>
                    {
                        { "id_status_acao", 0 },
                        { "dt_final_acao",DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") },
                        { "dt_inativacao",DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") }
                    };
                    //chama inativação por cd_acao da LP

                    var urlLogin = $"{_fiskLpConfig.Urlbase}/Auth";
                    var urlInativacao = $"{_fiskLpConfig.Urlbase}/LP/desativar/acao/{cd_acao}";

                    var responseLogin = await urlLogin.PostJsonAsync(new
                    {
                        login = _fiskLpConfig.Login,
                        senha = _fiskLpConfig.Senha
                    });

                    if (responseLogin.StatusCode != 200)
                    {
                        var errorMessage = await responseLogin.GetStringAsync();
                        return BadRequest("erro ao efetuar o login para inativar lps vinculadas");
                    }

                    var json = await responseLogin.GetJsonAsync<JsonElement>();

                    string token = json.GetProperty("data").GetProperty("token").GetString();

                    var response = await urlInativacao.WithOAuthBearerToken(token).SendAsync(HttpMethod.Patch, null);
                    if (response.StatusCode != 200)
                    {
                        return BadRequest("erro ao inativar lps vinculadas");
                    }
                }
                else
                {
                    acaoDict = new Dictionary<string, object>
                    {
                        { "id_status_acao", 1 },
                        { "dt_final_acao",null },
                        { "dt_inativacao",null}
                    };
                }

                var t_contato = await SQLServerService.Update("T_ACAO", acaoDict, source, "cd_acao", cd_acao);
                if (!t_contato.success) return BadRequest(t_contato.error);
                return ResponseDefault();
            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }

        /// <summary>
        /// Copia contatos de uma ação para outra.
        /// </summary>
        /// <param name="cd_acao_origem">ação de onde os contatos serão importados</param>
        /// <param name="cd_acao_destino">ação que recebera os contatos.</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost()]
        [Route("contatos")]
        public async Task<IActionResult> Contatos(int cd_acao_origem, int cd_acao_destino)
        {
            var schemaName = "T_Contato";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                var contatosOrigemGet = await SQLServerService.GetList("T_CONTATO", null, "[cd_acao]", $"[{cd_acao_origem}]", source, SearchModeEnum.Equals);
                if (!contatosOrigemGet.success) return BadRequest(contatosOrigemGet.error);
                var contatosOrigem = contatosOrigemGet.data;

                var contatosDestinoGet = await SQLServerService.GetList("T_CONTATO", null, "[cd_acao]", $"[{cd_acao_destino}]", source, SearchModeEnum.Equals);
                if (!contatosDestinoGet.success) return BadRequest(contatosDestinoGet.error);
                var contatosDestino = contatosDestinoGet.data;
                var destinoSet = new HashSet<string>(
                     contatosDestino
                         .Where(c => c.TryGetValue("cd_pessoa_contato", out var val) && val != null)
                         .Select(c => c["cd_pessoa_contato"].ToString())
                 );
                foreach (var contato in contatosOrigem)
                {
                    if (contato.TryGetValue("cd_pessoa_contato", out var cdOrigem) && cdOrigem != null)
                    {
                        var cdOrigemStr = cdOrigem.ToString();
                        if (!destinoSet.Contains(cdOrigemStr))
                        {
                            contato.Remove("cd_contato");
                            contato["cd_acao"] = cd_acao_destino;

                            var contato_insert = await SQLServerService.Insert("T_CONTATO", contato, source);
                            if (!contato_insert.success) return BadRequest(contato_insert.error);
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
        [HttpPost()]
        [Route("contatos/import-manual")]
        public async Task<IActionResult>ContatosCopia(int cd_acao,List<int> cd_contatos)
        {
            var schemaName = "T_Contato";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                var contatosOrigemGet = await SQLServerService.GetList("T_CONTATO",string.Join(",",cd_contatos), "cd_contato", null, source, SearchModeEnum.Equals);
                if (!contatosOrigemGet.success) return BadRequest(contatosOrigemGet.error);
                var contatosOrigem = contatosOrigemGet.data;

                var contatosDestinoGet = await SQLServerService.GetList("T_CONTATO", null, "[cd_acao]", $"[{cd_acao}]", source, SearchModeEnum.Equals);
                if (!contatosDestinoGet.success) return BadRequest(contatosDestinoGet.error);
                var contatosDestino = contatosDestinoGet.data;
                var destinoSet = new HashSet<string>(
                     contatosDestino
                         .Where(c => c.TryGetValue("cd_pessoa_contato", out var val) && val != null)
                         .Select(c => c["cd_pessoa_contato"].ToString())
                 );
                foreach (var contato in contatosOrigem)
                {
                    if (contato.TryGetValue("cd_pessoa_contato", out var cdOrigem) && cdOrigem != null)
                    {
                        var cdOrigemStr = cdOrigem.ToString();
                        if (!destinoSet.Contains(cdOrigemStr))
                        {
                            contato.Remove("cd_contato");
                            contato["cd_acao"] = cd_acao;
                            contato["id_posicao_contato"] = 1;
                                 
                            var contato_insert = await SQLServerService.Insert("T_CONTATO", contato, source);
                            if (!contato_insert.success) return BadRequest(contato_insert.error);
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

    }
}