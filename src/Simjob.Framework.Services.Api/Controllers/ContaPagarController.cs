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
using Simjob.Framework.Services.Api.Models.Contas;
using Simjob.Framework.Services.Api.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Simjob.Framework.Services.Api.Controllers
{
    public class ContaPagarController : BaseController
    {
        private readonly IRepository<SourceContext, Source> _sourceRepository;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;

        public ContaPagarController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, IRepository<SourceContext, Source> sourceRepository, IRepository<MongoDbContext, Schema> schemaRepository) : base(bus, notifications)
        {
            _sourceRepository = sourceRepository;
            _schemaRepository = schemaRepository;
        }

        [Authorize]
        [HttpPost()]
        public async Task<IActionResult> InsertContaPagar([FromBody] InsertContaAPagarModel model)
        {
            var schemaName = "T_Titulo";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                var resultReturn = await ValidateCommand(model, source);
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

        private async Task<(bool sucess, string error)> ValidateCommand(InsertContaAPagarModel model, Source source)
        {
            try
            {
                model.dh_cadastro_titulo = DateTime.Now;
                model.id_natureza_titulo = 2;
                var t_titulo_dict = ToDictionary(model);
                t_titulo_dict.Remove("plano_titulo");
                var t_titulo_insert = await SQLServerService.Insert("T_TITULO", t_titulo_dict, source);
                if (!t_titulo_insert.success) return new(t_titulo_insert.success, t_titulo_insert.error);

                var titulo_CadastradoGet = await SQLServerService.GetList("T_TITULO", 1, 1, "cd_titulo", true, null, null, "", source, SearchModeEnum.Equals, null, null);
                var titulo_Cadastrado = titulo_CadastradoGet.data.First();
                var cd_titulo = titulo_Cadastrado["cd_titulo"];

                if (model.plano_titulo != null)
                {
                    foreach (var plano in model.plano_titulo)
                    {
                        var plano_dict = new Dictionary<string, object>
                        {
                            { "cd_titulo", cd_titulo },
                            { "cd_plano_conta", plano.cd_plano_conta },
                            { "vl_plano_titulo", plano.vl_plano_titulo }
                        };
                        var t_plano_titulo_insert = await SQLServerService.Insert("T_PLANO_TITULO", plano_dict, source);
                        if (!t_plano_titulo_insert.success) return new(t_plano_titulo_insert.success, t_plano_titulo_insert.error);
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
        [HttpPut()]
        [Route("{cd_titulo}")]
        public async Task<IActionResult> UpdateContaPagar([FromBody] InsertContaAPagarModel model, int cd_titulo)
        {
            var schemaName = "T_Titulo";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                var resultReturn = await ProcessaUpdate(cd_titulo, model, source);
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

        private async Task<(bool sucess, string error)> ProcessaUpdate(int cd_titulo, InsertContaAPagarModel model, Source source)
        {
            try
            {
                var filtrosTitulo = new List<(string campo, object valor)> { new("cd_titulo", cd_titulo) };
                var tituloExists = await SQLServerService.GetFirstByFields(source, "T_TITULO", filtrosTitulo);
                if (tituloExists == null) return (false, "titulo não encontrado");

                model.id_natureza_titulo = 2;
                var t_titulo_dict = ToDictionary(model);
                t_titulo_dict.Remove("plano_titulo");
                var t_titulo_insert = await SQLServerService.Update("T_TITULO", t_titulo_dict, source, "cd_titulo", cd_titulo);
                if (!t_titulo_insert.success) return new(t_titulo_insert.success, t_titulo_insert.error);

                if (model.plano_titulo != null)
                {
                    //Delete de plano_titulo
                    await SQLServerService.Delete("T_PLANO_TITULO", "cd_titulo", cd_titulo.ToString(), source);
                    foreach (var plano in model.plano_titulo)
                    {
                        var plano_dict = new Dictionary<string, object>
                        {
                            { "cd_titulo", cd_titulo },
                            { "cd_plano_conta", plano.cd_plano_conta },
                            { "vl_plano_titulo", plano.vl_plano_titulo }
                        };
                        var t_plano_titulo_insert = await SQLServerService.Insert("T_PLANO_TITULO", plano_dict, source);
                        if (!t_plano_titulo_insert.success) return new(t_plano_titulo_insert.success, t_plano_titulo_insert.error);
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, $"Erro: {ex.Message}");
            }

            return (true, string.Empty);
        }

        private Dictionary<string, object> ToDictionary(InsertContaAPagarModel model)
        {
            return model.GetType()
                      .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                      .Where(p => p.CanRead)
                      .Select(p => new { p.Name, Value = p.GetValue(model) })
                      .Where(x => x.Value != null)
                      .ToDictionary(x => x.Name, x => x.Value);
        }

        private async Task<int?> GetLocalMovto(int cd_pessoa_empresa, Source source)
        {
            try
            {
                var filtroParametro = new List<(string campo, object valor)> { new("cd_pessoa_escola", cd_pessoa_empresa) };
                var parametroExists = await SQLServerService.GetFirstByFields(source, "T_PARAMETRO", filtroParametro);
                if (parametroExists != null && parametroExists.ContainsKey("cd_local_movto"))
                {
                    return Convert.ToInt32(parametroExists["cd_local_movto"]);
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [Authorize]
        [HttpGet()]
        public async Task<IActionResult> GetAll(string value, SearchModeEnum mode, int? page, int? limit, string sortField, bool sortDesc = false, string ids = "", string searchFields = null, string? cd_empresa = null)
        {
            if (cd_empresa == null) return BadRequest("campo cd_empresa não informado");
            var schemaName = "T_Titulo";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                if (string.IsNullOrEmpty(searchFields) && string.IsNullOrEmpty(value))
                {
                    searchFields = "[id_natureza_titulo]";

                    value = $"[2]";
                }
                else
                {
                    searchFields = searchFields + ",[id_natureza_titulo]";

                    value = value + $",[2]";
                }
                var tituloResult = await SQLServerService.GetList("vi_titulo", page, limit, sortField, sortDesc, ids, searchFields, value, source, mode, "cd_pessoa_empresa", cd_empresa);
                if (tituloResult.success)
                {
                    var retorno = new
                    {
                        data = tituloResult.data,
                        tituloResult.total,
                        page,
                        limit,
                        pages = limit != null ? (int)Math.Ceiling((double)tituloResult.total / limit.Value) : 0
                    };

                    return ResponseDefault(retorno);
                }
                return BadRequest(new
                {
                    sucess = false,
                    error = tituloResult.error
                });
            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }

        [Authorize]
        [HttpGet()]
        [Route("{cd_titulo}")]
        public async Task<IActionResult> GetById(string cd_titulo)
        {
            var schemaName = "T_Titulo";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                var filtros = new List<(string campo, object valor)> { new("id_natureza_titulo", 2), new("cd_titulo", cd_titulo) };
                var tituloExiste = await SQLServerService.GetFirstByFields(source, "vi_titulo", filtros);
                if (tituloExiste == null) return NotFound();

                var planoTituloExiste = await SQLServerService.GetList(schemaName: "T_PLANO_TITULO", page: 1, limit: 1, sortField: "cd_titulo", sortDesc: false, ids: null, searchFields: "[cd_titulo]", value: $"[{cd_titulo}]", source: source, mode: SearchModeEnum.Equals);

                var retorno = new
                {
                    cd_titulo = tituloExiste["cd_titulo"],
                    nm_titulo = tituloExiste["nm_titulo"],
                    nm_parcela_titulo = tituloExiste["nm_parcela_titulo"],
                    dc_tipo_titulo = tituloExiste["dc_tipo_titulo"],
                    dt_emissao_titulo = tituloExiste["dt_emissao_titulo"],
                    dt_vcto_titulo = tituloExiste["dt_vcto_titulo"],
                    vl_titulo = tituloExiste["vl_titulo"],
                    vl_saldo_titulo = tituloExiste["vl_saldo_titulo"],
                    id_status_titulo = tituloExiste["id_status_titulo"],
                    id_natureza_titulo = tituloExiste["id_natureza_titulo"],
                    dc_tipo_financeiro = tituloExiste["dc_tipo_financeiro"],
                    cd_tipo_financeiro = tituloExiste["cd_tipo_financeiro"],
                    no_cliente = tituloExiste["no_cliente"],
                    cd_pessoa_titulo = tituloExiste["cd_pessoa_titulo"],
                    no_responsavel = tituloExiste["no_responsavel"],
                    cd_pessoa_responsavel = tituloExiste["cd_pessoa_responsavel"],
                    cd_pessoa_empresa = tituloExiste["cd_pessoa_empresa"],
                    id_origem_titulo = tituloExiste["id_origem_titulo"],
                    plano_titulo = planoTituloExiste.data
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
        [Route("{cd_titulo}")]
        public async Task<IActionResult> BaixaTitulo(int cd_titulo)
        {
            var schemaName = "T_Titulo";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                var filtrosTitulo = new List<(string campo, object valor)> { new("cd_titulo", cd_titulo) };
                var tituloExists = await SQLServerService.GetFirstByFields(source, "T_TITULO", filtrosTitulo);
                if (tituloExists == null) return NotFound("titulo não encontrado");

                var tituloDict = new Dictionary<string, object>
                {
                    { "id_status_titulo", 2 }
                };

                var t_titulo_insert = await SQLServerService.Update("T_TITULO", tituloDict, source, "cd_titulo", cd_titulo);
                if (!t_titulo_insert.success) return BadRequest(t_titulo_insert.error);
                return ResponseDefault();
            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }

        [Authorize]
        [HttpPatch()]
        [Route("baixaTitulo")]
        public async Task<IActionResult> Baixa(BaixaContaModel model)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            var tokenInfo = Util.GetUserInfoFromToken(accessToken);

            var schemaName = "T_Titulo";
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

                if (string.IsNullOrEmpty(cd_pessoa_logada)) return BadRequest("cd_pessoa não configurado");

                var filtrosUsuario = new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa_logada) };
                var sys_usuario = await SQLServerService.GetFirstByFields(source, "T_SYS_USUARIO", filtrosUsuario);
                if (sys_usuario != null) cd_usuario = sys_usuario["cd_usuario"].ToString() ?? "1";



                var filtroParametro = new List<(string campo, object valor)> { new("cd_pessoa_escola", model.cd_pessoa_empresa) };
                var parametroExists = await SQLServerService.GetFirstByFields(source, "T_PARAMETRO", filtroParametro);
                if (parametroExists == null) return NotFound("parametros não encontratos para esta escola");

                var nm_recibo = int.Parse(parametroExists["nm_ultimo_recibo"].ToString());

                //validação de cd_tranferencia
                if (model.cd_tran_finan != null)
                {
                    var filtrosTranFin = new List<(string campo, object valor)> { new("cd_tran_finan", model.cd_tran_finan) };
                    var tranFinExists = await SQLServerService.GetFirstByFields(source, "T_TRAN_FINAN", filtrosTranFin);
                    if (tranFinExists != null) return Conflict("Já existe um registro com este ID cadastrado, tente alterá-lo");
                }

                // Determinar cd_local_movto baseado no tipo de liquidação
                int? cd_local_movto_final;
                if (model.cd_tipo_liquidacao == 6) // Cancelamento
                {
                    cd_local_movto_final = await GetLocalMovto(model.cd_pessoa_empresa, source);
                    if (cd_local_movto_final == null)
                    {
                        return BadRequest($"Parâmetro cd_local_movto não encontrado para a empresa {model.cd_pessoa_empresa}");
                    }
                }
                else
                {
                    cd_local_movto_final = model.cd_local_movto;
                }

                //gera T_TRANS_FINAN
                var tranFinDict = new Dictionary<string, object>
                {
                    { "cd_pessoa_empresa", model.cd_pessoa_empresa },
                    { "cd_local_movto", cd_local_movto_final },
                    { "dt_tran_finan", model.dt_baixa.ToString("yyyy-MM-ddTHH:mm:ss") },
                    { "cd_tipo_liquidacao", model.cd_tipo_liquidacao },
                    { "vl_total_baixa", model.gridBaixaEfetuada.Sum(x=>x.vl_liquidacao_baixa)}
                };
                var t_tranFin_insert = await SQLServerService.InsertWithResult("T_TRAN_FINAN", tranFinDict, source);
                if (!t_tranFin_insert.success) return BadRequest(t_tranFin_insert.error);
                var cd_tran_fin = t_tranFin_insert.inserted["cd_tran_finan"];
                var cd_tipo_liquidacao = model.cd_tipo_liquidacao;

                if ((model.cd_tipo_liquidacao_old is 2 || model.cd_tipo_liquidacao_old is 3) && cd_tipo_liquidacao == 110)
                {
                    //validações de LOCAL_MOVTO
                }
                if (!model.gridBaixaEfetuada.IsNullOrEmpty())
                {
                    foreach (var baixa in model.gridBaixaEfetuada)
                    {
                        nm_recibo++;
                        var titulo_baixa_dic = new Dictionary<string, object>
                        {
                            { "cd_titulo", baixa.cd_titulo },
                            { "cd_tran_finan", cd_tran_fin },
                            { "cd_tipo_liquidacao", baixa.cd_tipo_liquidacao },
                            { "cd_local_movto", model.cd_tipo_liquidacao == 6 ? cd_local_movto_final : baixa.cd_local_movto },
                            { "dt_baixa_titulo", model.dt_baixa.ToString("yyyy-MM-ddTHH:mm:ss") },
                            { "id_baixa_processada", 0 },
                            { "id_baixa_parcial", baixa.id_baixa_parcial },
                            { "nm_dias_float", 0 },
                            { "vl_liquidacao_baixa", baixa.vl_liquidacao_baixa },
                            { "vl_juros_baixa", baixa.vl_juros_baixa },
                            { "vl_desconto_baixa", baixa.vl_desconto_baixa },
                            { "vl_principal_baixa", baixa.vl_principal_baixa },
                            { "vl_juros_calculado", baixa.vl_juros_calculado },
                            { "vl_multa_calculada", baixa.vl_multa_calculada },
                            { "vl_desc_multa_baixa", baixa.vl_desc_multa_baixa },
                            { "vl_desc_juros_baixa", baixa.vl_desc_juros_baixa },
                            { "vl_multa_baixa", baixa.vl_multa_baixa },
                            { "pc_pontualidade", baixa.pc_pontualidade },
                            { "tx_obs_baixa", baixa.tx_obs_baixa },
                            { "vl_desconto_baixa_calculado", baixa.vl_desconto_baixa_calculado },
                            { "vl_baixa_saldo_titulo", baixa.vl_baixa_saldo_titulo + baixa.vl_desconto_baixa },
                            { "cd_politica_desconto", baixa.cd_politica_desconto },
                            { "cd_usuario", 1},
                            { "vl_taxa_cartao", baixa.vl_taxa_cartao },
                            { "vl_acr_liquidacao", baixa.vl_acr_liquidacao },
                            { "vl_liquidacao_calculado", baixa.vl_liquidacao_calculado },
                            { "nm_recibo", nm_recibo }
                        };
                        var t_titulo_baixa = await SQLServerService.Insert("T_BAIXA_TITULO", titulo_baixa_dic, source);
                        if (!t_titulo_baixa.success) return BadRequest(t_titulo_baixa.error);
                        var titulo_baixa_CadastradaGet = await SQLServerService.GetList("T_BAIXA_TITULO", 1, 1, "cd_baixa_titulo", true, null, null, "", source, SearchModeEnum.Equals, null, null);
                        var titulo_baixa_Cadastrada = titulo_baixa_CadastradaGet.data.First();
                        int cd_baixa_titulo = (int)titulo_baixa_Cadastrada["cd_baixa_titulo"];

                        var atualizaDependentes = await AtualizarDependentesBaixa(cd_baixa_titulo, source);

                        if (model.cd_caixa != null)
                        {
                            // Buscar o último registro de conta corrente criado para este cd_baixa_titulo
                            List<(string campo, object valor)> filtrosContaCorrente = new List<(string campo, object valor)>
                            {
                                new("cd_baixa_titulo", cd_baixa_titulo)
                            };
                            var contaCorrente = await SQLServerService.GetFirstByFields(source, "T_CONTA_CORRENTE", filtrosContaCorrente);
                            if (contaCorrente != null)
                            {
                                var caixaTituloDict = new Dictionary<string, object>
                                {
                                    { "cd_caixa", model.cd_caixa },
                                    { "cd_titulo", baixa.cd_titulo },
                                    { "cd_conta_corrente", contaCorrente["cd_conta_corrente"] },
                                    { "dt_recebimento", model.dt_baixa.ToString("yyyy-MM-ddTHH:mm:ss") }
                                };
                                var insertCaixaTitulo = await SQLServerService.Insert("T_CAIXA_TITULO", caixaTituloDict, source);
                                if (!insertCaixaTitulo.success) return BadRequest(insertCaixaTitulo.error);
                            }
                        }

                        //atualiza status renegociação do aditamento se houver
                        var titulo = await SQLServerService.GetFirstByFields(source, "T_TITULO", new List<(string campo, object valor)> { new("cd_titulo", baixa.cd_titulo) });
                        if(titulo != null)
                        {
                            var cd_origem_titulo = titulo["id_origem_titulo"];
                            var aditamento = await SQLServerService.GetFirstByFields(source, "T_ADITAMENTO", new List<(string campo, object valor)> { new("cd_aditamento", cd_origem_titulo) });
                            if(aditamento != null)
                            {
                                var titulos = await SQLServerService.GetList("T_TITULO", null, "[cd_origem_titulo],[id_status_titulo]", $"[{aditamento["cd_aditamento"]}],[1]",source,SearchModeEnum.Equals);
                                if(titulos.success)
                                {
                                    var id_status_renegociacao = titulos.data.Count == 0 ? 2 : 1;
                                   
                                    var aditamentoUpdate = new Dictionary<string, object>
                                    {
                                        { "id_status_renegociacao", id_status_renegociacao }
                                    };
                                    var updateAditamento = await SQLServerService.Update("T_ADITAMENTO", aditamentoUpdate, source, "cd_aditamento", aditamento["cd_aditamento"]);
                                    if (!updateAditamento.success) return BadRequest(updateAditamento.error);
                                    await AddHistoricoAditamento(int.Parse(aditamento["cd_aditamento"].ToString()), int.Parse(cd_usuario), id_status_renegociacao, source);
                                }
                            }

                        }

                    }
                }
                //atualizando ultimo recibo

                var parametroUpdate = new Dictionary<string, object>
                    {
                        { "nm_ultimo_recibo", nm_recibo }
                    };
                var parametroResult = await SQLServerService.Update("T_PARAMETRO", parametroUpdate, source, "cd_pessoa_escola", model.cd_pessoa_empresa);
                if (!parametroResult.success) return BadRequest(parametroResult.error);

                if (model.cd_tipo_liquidacao == 110)
                {
                    //troca financeira
                }

                return ResponseDefault();
            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
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

        [Authorize]
        [HttpGet()]
        [Route("baixaTitulo")]
        public async Task<IActionResult> GetAllBaixa(string value, SearchModeEnum mode, int? page, int? limit, string sortField, bool sortDesc = false, string ids = "", string searchFields = null, string cd_empresa = null)
        {
            if (cd_empresa == null) return BadRequest("campo cd_empresa não informado");
            var schemaName = "T_Titulo";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true)
            {
                var baixaResult = await SQLServerService.GetList("vi_baixa_titulo", page, limit, sortField, sortDesc, ids, searchFields, value, source, mode, "cd_pessoa_empresa", cd_empresa);
                if (baixaResult.success)
                {
                    var retorno = new
                    {
                        data = baixaResult.data,
                        baixaResult.total,
                        page,
                        limit,
                        pages = limit != null ? (int)Math.Ceiling((double)baixaResult.total / limit.Value) : 0
                    };

                    return ResponseDefault(retorno);
                }
                return BadRequest(new
                {
                    sucess = false,
                    error = baixaResult.error
                });
            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
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
                    { "dt_aditamento_historico", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") },
                    { "id_status_renegociacao", id_status_renegociacao },
                    { "dc_historico_aditamento", dc_historico_aditamento },
                    { "cd_usuario", cd_usuario },
                    { "cd_aditamento", cd_aditamento }
                };

            var t_aditamento_historico_Result = await SQLServerService.Insert("T_ADITAMENTO_HISTORICO", dict, source);
            if (!t_aditamento_historico_Result.success) return (false, t_aditamento_historico_Result.error);
            return (true, null);
        }
        
        [Authorize]
        [HttpGet()]
        [Route("informacoesReciboBaixa/{cd_baixa_titulo}")]
        public async Task<IActionResult> GetInformacoesReciboBaixa(int cd_baixa_titulo)
        {
            var schemaName = "T_Titulo";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            
            if (source != null && source.Active != null && source.Active == true)
            {
                if (cd_baixa_titulo <= 0)
                {
                    return BadRequest("cd_baixa_titulo deve ser maior que zero");
                }

                string connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};MultipleActiveResultSets=True;";
                
                try
                {
                    using (var connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        
                        var query = @"
                            SELECT bt.*
                            , t.nm_parcela_titulo
                            , tl.dc_tipo_liquidacao
                            , t.dt_vcto_titulo
                            , t.nm_titulo
                            , mf.dc_movimentacao_financeira
                            , pr.no_pessoa AS responsavel
                            , pa.no_pessoa AS aluno
                            , pf.nm_cpf
                            , lo.no_local_movto AS no_local_movto_origem
                            , ld.no_local_movto AS no_local_movto_destino
                            , pe.no_pessoa AS no_empresa
                            , pe.dc_reduzido_pessoa AS no_empresa_reduzido
                            , pj.dc_num_cgc
                            , pc.no_subgrupo_conta referentea
                            , l5.no_localidade AS pais
                            , l4.no_localidade AS estado
                            , l3.no_localidade AS cidade
                            , l2.no_localidade AS bairro
                            , l1.no_localidade AS logradouro
                            , en.dc_num_endereco AS numero
                            , en.dc_compl_endereco AS complemento
                            , en.dc_num_cep
                            FROM T_BAIXA_TITULO bt
                            INNER JOIN T_CONTA_CORRENTE cc ON cc.cd_baixa_titulo = bt.cd_baixa_titulo
                            INNER JOIN T_TITULO t ON bt.cd_titulo = t.cd_titulo
                            INNER JOIN T_PESSOA pr ON t.cd_pessoa_responsavel = pr.cd_pessoa
                            INNER JOIN T_PESSOA pa ON t.cd_pessoa_titulo = pa.cd_pessoa
                            INNER JOIN T_PESSOA_FISICA pf ON pf.cd_pessoa_fisica = pa.cd_pessoa
                            INNER JOIN T_TIPO_LIQUIDACAO tl ON tl.cd_tipo_liquidacao = bt.cd_tipo_liquidacao
                            INNER JOIN T_MOVIMENTACAO_FINANCEIRA mf ON mf.cd_movimentacao_financeira = cc.cd_movimentacao_financeira
                            INNER JOIN vi_plano_conta pc ON pc.cd_plano_conta = cc.cd_plano_conta
                            LEFT JOIN T_LOCAL_MOVTO lo ON lo.cd_local_movto = cc.cd_local_origem
                            LEFT JOIN T_LOCAL_MOVTO ld ON ld.cd_local_movto = cc.cd_local_destino
                            INNER JOIN T_PESSOA pe ON t.cd_pessoa_empresa = pe.cd_pessoa 
                            INNER JOIN T_PESSOA_JURIDICA pj ON pj.cd_pessoa_juridica = pe.cd_pessoa 
                            LEFT JOIN T_ENDERECO en ON pe.cd_endereco_principal = en.cd_endereco
                            LEFT JOIN T_LOCALIDADE l1 ON en.cd_loc_logradouro = l1.cd_localidade
                            LEFT JOIN T_LOCALIDADE l2 ON en.cd_loc_bairro = l2.cd_localidade
                            LEFT JOIN T_LOCALIDADE l3 ON en.cd_loc_cidade = l3.cd_localidade
                            LEFT JOIN T_LOCALIDADE l4 ON en.cd_loc_estado = l4.cd_localidade
                            LEFT JOIN T_LOCALIDADE l5 ON en.cd_loc_pais = l5.cd_localidade
                            WHERE bt.cd_baixa_titulo = @cd_baixa_titulo";

                        var command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@cd_baixa_titulo", cd_baixa_titulo);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var resultado = new Dictionary<string, object>();
                            
                            if (await reader.ReadAsync())
                            {
                                // Separar campos da empresa
                                var empresa = new Dictionary<string, object>();
                                var camposEmpresa = new[] { "no_empresa", "no_empresa_reduzido", "dc_num_cgc", "pais", "estado", "cidade", "bairro", "logradouro", "numero", "complemento", "dc_num_cep" };
                                
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string fieldName = reader.GetName(i);
                                    object fieldValue = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                    
                                    if (camposEmpresa.Contains(fieldName))
                                    {
                                        empresa[fieldName] = fieldValue;
                                    }
                                    else
                                    {
                                        resultado[fieldName] = fieldValue;
                                    }
                                }
                                
                                // Adicionar objeto empresa ao resultado
                                resultado["empresa"] = empresa;
                                
                                return ResponseDefault(resultado);
                            }
                            else
                            {
                                return NotFound($"Baixa de título com código {cd_baixa_titulo} não encontrada");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest($"Erro ao consultar informações do recibo: {ex.Message}");
                }
            }
            
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }
    }
}