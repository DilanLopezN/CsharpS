using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
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
using Simjob.Framework.Services.Api.Models.Contas;
using Simjob.Framework.Services.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static MongoDB.Driver.WriteConcern;
using static Simjob.Framework.Services.Api.Models.FilaMatricula.InsertFilaMatriculaModel;

namespace Simjob.Framework.Services.Api.Controllers
{
    public class ContaReceberController : BaseController
    {
        private readonly IRepository<SourceContext, Source> _sourceRepository;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;

        public ContaReceberController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, IRepository<SourceContext, Source> sourceRepository, IRepository<MongoDbContext, Schema> schemaRepository) : base(bus, notifications)
        {
            _sourceRepository = sourceRepository;
            _schemaRepository = schemaRepository;
        }

        [Authorize]
        [HttpPost()]
        public async Task<IActionResult> InsertContaReceber([FromBody] InsertContaAReceberModel model)
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

        private async Task<(bool sucess, string error)> ValidateCommand(InsertContaAReceberModel model, Source source)
        {
            try
            {
                model.dh_cadastro_titulo = DateTime.Now;
                model.id_natureza_titulo = 1;
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
        public async Task<IActionResult> UpdateContaReceber([FromBody] InsertContaAReceberModel model, int cd_titulo)
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

        private async Task<(bool sucess, string error)> ProcessaUpdate(int cd_titulo, InsertContaAReceberModel model, Source source)
        {
            try
            {
                var filtrosTitulo = new List<(string campo, object valor)> { new("cd_titulo", cd_titulo) };
                var tituloExists = await SQLServerService.GetFirstByFields(source, "T_TITULO", filtrosTitulo);
                if (tituloExists == null) return (false, "titulo não encontrado");

                model.id_natureza_titulo = 1;
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

        private Dictionary<string, object> ToDictionary(InsertContaAReceberModel model)
        {
            return model.GetType()
                      .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                      .Where(p => p.CanRead)
                      .Select(p => new { p.Name, Value = p.GetValue(model) })
                      .Where(x => x.Value != null)
                      .ToDictionary(x => x.Name, x => x.Value);
        }

        [Authorize]
        [HttpGet()]
        public async Task<IActionResult> GetAll(string value, SearchModeEnum mode, int? page, int? limit, string sortField, bool sortDesc = false, string ids = "", string searchFields = null, string cd_empresa = null)
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

                    value = $"[1]";
                }
                else
                {
                    searchFields = searchFields + ",[id_natureza_titulo]";

                    value = value + $",[1]";
                }
                var tituloResult = await SQLServerService.GetList("vi_titulo", page, limit, sortField, sortDesc, ids, searchFields, value, source, mode, "cd_pessoa_empresa", cd_empresa);
                if (tituloResult.success)
                {
                    // Buscar parâmetros da escola uma única vez
                    var parametrosEscola = await BuscarParametrosEscola(Convert.ToInt32(cd_empresa), source);
                    
                    // Processar cada título para calcular saldo corrigido, juros e multa
                    var titulosProcessados = new List<Dictionary<string, object>>();
                    DateTime dataBaixa = DateTime.Now;

                    foreach (var titulo in tituloResult.data)
                    {
                        var tituloProcessado = new Dictionary<string, object>(titulo);
                        
                        // Verificar se o título está em aberto (status 1)
                        if (Convert.ToInt32(titulo["id_status_titulo"]) == 1)
                        {
                            var cdTitulo = Convert.ToInt32(titulo["cd_titulo"]);
                            var dtVctoTitulo = Convert.ToDateTime(titulo["dt_vcto_titulo"]);
                            var vlTitulo = Convert.ToDecimal(titulo["vl_titulo"]);
                            var vlSaldoTitulo = Convert.ToDecimal(titulo["vl_saldo_titulo"] ?? vlTitulo);

                            // Calcular valores corrigidos usando os parâmetros da escola
                            var calculoCorrigido = await CalcularValoresCorrigidosCompleto(cdTitulo, vlSaldoTitulo, dtVctoTitulo, dataBaixa, parametrosEscola, source);

                            // Simular baixa do título
                            var simulacaoBaixa = await SimularBaixaTitulo(titulo, dataBaixa, parametrosEscola, source);

                            tituloProcessado["vl_saldo_corrigido"] = simulacaoBaixa.vl_liquidacao_baixa;
                            tituloProcessado["vl_juros_calculado"] = simulacaoBaixa.vl_juros_calculado;
                            tituloProcessado["vl_multa_calculada"] = simulacaoBaixa.vl_multa_calculada;
                            tituloProcessado["vl_principal_baixa"] = simulacaoBaixa.vl_principal_baixa;
                            tituloProcessado["vl_desconto_baixa"] = simulacaoBaixa.vl_desconto_baixa;
                            tituloProcessado["pc_pontualidade"] = simulacaoBaixa.pc_pontualidade;
                            tituloProcessado["dt_calculo"] = dataBaixa.ToString("yyyy-MM-ddTHH:mm:ss");
                            // Adicionar campos calculados
                            //tituloProcessado["vl_saldo_corrigido"] = calculoCorrigido.vlLiquidacao;
                            //tituloProcessado["vl_juros_calculado"] = calculoCorrigido.vlJuros;
                            //tituloProcessado["vl_multa_calculada"] = calculoCorrigido.vlMulta;
                            //tituloProcessado["dt_calculo"] = dataBaixa.ToString("yyyy-MM-ddTHH:mm:ss");
                        }
                        else
                        {
                            // Para títulos não em aberto, manter valores originais
                            tituloProcessado["vl_saldo_corrigido"] = titulo["vl_saldo_titulo"];
                            tituloProcessado["vl_juros_calculado"] = 0;
                            tituloProcessado["vl_multa_calculada"] = 0;
                            tituloProcessado["dt_calculo"] = dataBaixa.ToString("yyyy-MM-ddTHH:mm:ss");
                        }

                        titulosProcessados.Add(tituloProcessado);
                    }

                    var retorno = new
                    {
                        data = titulosProcessados,
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

        private async Task<(decimal vlLiquidacao, decimal vlJuros, decimal vlMulta)> CalcularValoresCorrigidosCompleto(
            int cdTitulo, decimal vlSaldoTitulo, DateTime dtVctoTitulo, DateTime dataBaixa, Dictionary<string, object> parametros, Source source)
        {
            try
            {
                // Se não há saldo, não há o que calcular
                if (vlSaldoTitulo <= 0)
                {
                    return (0, 0, 0);
                }

                // Buscar baixas parciais existentes
                var baixasParciais = await BuscarBaixasParciais(cdTitulo, source);
                
                decimal vlJurosCalculado = 0;
                decimal vlMultaCalculada = 0;
                decimal vlPrincipal = vlSaldoTitulo;

                // Se há baixas parciais no mesmo dia da simulação
                var baixasParcialDia = baixasParciais.Where(b => b.ContainsKey("dt_baixa_titulo") && 
                    Convert.ToDateTime(b["dt_baixa_titulo"]).Date == dataBaixa.Date).ToList();

                if (baixasParcialDia.Any())
                {
                    // Se existem baixas parciais no mesmo dia, usar os valores já calculados
                    vlMultaCalculada = baixasParcialDia.Sum(b => Convert.ToDecimal(b["vl_multa_calculada"] ?? 0));
                    vlJurosCalculado = baixasParcialDia.Sum(b => Convert.ToDecimal(b["vl_juros_calculado"] ?? 0));
                }
                else if (baixasParciais.Any())
                {
                    // Se há baixas parciais em outros dias
                    var dtPrimeiraBaixaParcial = baixasParciais
                        .Where(b => b.ContainsKey("dt_baixa_titulo"))
                        .Min(b => Convert.ToDateTime(b["dt_baixa_titulo"]));

                    DateTime dtVctoParaCalculo = dtVctoTitulo;
                    bool baixaParcialAposVencimento = false;

                    if (dtPrimeiraBaixaParcial > dtVctoTitulo)
                    {
                        dtVctoParaCalculo = dtPrimeiraBaixaParcial;
                        baixaParcialAposVencimento = true;
                    }

                    // Calcular juros e multa considerando as baixas parciais com parâmetros
                    var calculo = CalcularJurosMultaCompleto(vlPrincipal, dtVctoParaCalculo, dataBaixa, parametros);
                    
                    if (baixaParcialAposVencimento && baixasParciais.Sum(b => Convert.ToDecimal(b["vl_multa_calculada"] ?? 0)) > 0)
                    {
                        vlMultaCalculada = baixasParciais.Sum(b => Convert.ToDecimal(b["vl_multa_calculada"] ?? 0));
                    }
                    else
                    {
                        vlMultaCalculada = calculo.vlMulta;
                    }

                    if (dtVctoTitulo > dataBaixa)
                    {
                        vlMultaCalculada = 0;
                        vlJurosCalculado = 0;
                    }
                    else
                    {
                        vlJurosCalculado = calculo.vlJuros + baixasParciais.Sum(b => Convert.ToDecimal(b["vl_juros_calculado"] ?? 0));
                    }
                }
                else
                {
                    // Não há baixas parciais, calcular normalmente com parâmetros
                    var calculo = CalcularJurosMultaCompleto(vlPrincipal, dtVctoTitulo, dataBaixa, parametros);
                    vlJurosCalculado = calculo.vlJuros;
                    vlMultaCalculada = calculo.vlMulta;
                }

                // Calcular valor total de liquidação
                decimal vlLiquidacao = vlPrincipal + vlJurosCalculado + vlMultaCalculada;

                return (vlLiquidacao, vlJurosCalculado, vlMultaCalculada);
            }
            catch (Exception)
            {
                // Em caso de erro, retornar valores conservadores
                return (vlSaldoTitulo, 0, 0);
            }
        }

        private async Task<(decimal vlLiquidacao, decimal vlJuros, decimal vlMulta)> CalcularValoresCorrigidos(
            int cdTitulo, decimal vlSaldoTitulo, DateTime dtVctoTitulo, DateTime dataBaixa, Source source)
        {
            try
            {
                // Se não há saldo, não há o que calcular
                if (vlSaldoTitulo <= 0)
                {
                    return (0, 0, 0);
                }

                // Buscar baixas parciais existentes
                var baixasParciais = await BuscarBaixasParciais(cdTitulo, source);
                
                decimal vlJurosCalculado = 0;
                decimal vlMultaCalculada = 0;
                decimal vlPrincipal = vlSaldoTitulo;

                // Se há baixas parciais no mesmo dia da simulação
                var baixasParcialDia = baixasParciais.Where(b => b.ContainsKey("dt_baixa_titulo") && 
                    Convert.ToDateTime(b["dt_baixa_titulo"]).Date == dataBaixa.Date).ToList();

                if (baixasParcialDia.Any())
                {
                    // Se existem baixas parciais no mesmo dia, usar os valores já calculados
                    vlMultaCalculada = baixasParcialDia.Sum(b => Convert.ToDecimal(b["vl_multa_calculada"] ?? 0));
                    vlJurosCalculado = baixasParcialDia.Sum(b => Convert.ToDecimal(b["vl_juros_calculado"] ?? 0));
                }
                else if (baixasParciais.Any())
                {
                    // Se há baixas parciais em outros dias
                    var dtPrimeiraBaixaParcial = baixasParciais
                        .Where(b => b.ContainsKey("dt_baixa_titulo"))
                        .Min(b => Convert.ToDateTime(b["dt_baixa_titulo"]));

                    DateTime dtVctoParaCalculo = dtVctoTitulo;
                    bool baixaParcialAposVencimento = false;

                    if (dtPrimeiraBaixaParcial > dtVctoTitulo)
                    {
                        dtVctoParaCalculo = dtPrimeiraBaixaParcial;
                        baixaParcialAposVencimento = true;
                    }

                    // Calcular juros e multa considerando as baixas parciais (sem parâmetros específicos)
                    var calculo = CalcularJurosMulta(vlPrincipal, dtVctoParaCalculo, dataBaixa);
                    
                    if (baixaParcialAposVencimento && baixasParciais.Sum(b => Convert.ToDecimal(b["vl_multa_calculada"] ?? 0)) > 0)
                    {
                        vlMultaCalculada = baixasParciais.Sum(b => Convert.ToDecimal(b["vl_multa_calculada"] ?? 0));
                    }
                    else
                    {
                        vlMultaCalculada = calculo.vlMulta;
                    }

                    if (dtVctoTitulo > dataBaixa)
                    {
                        vlMultaCalculada = 0;
                        vlJurosCalculado = 0;
                    }
                    else
                    {
                        vlJurosCalculado = calculo.vlJuros + baixasParciais.Sum(b => Convert.ToDecimal(b["vl_juros_calculado"] ?? 0));
                    }
                }
                else
                {
                    // Não há baixas parciais, calcular normalmente (sem parâmetros específicos)
                    var calculo = CalcularJurosMulta(vlPrincipal, dtVctoTitulo, dataBaixa);
                    vlJurosCalculado = calculo.vlJuros;
                    vlMultaCalculada = calculo.vlMulta;
                }
                // Descontos
                var desconto = CalcularDesconto(cdTitulo, source);

                // Calcular valor total de liquidação
                decimal vlLiquidacao = (vlPrincipal + vlJurosCalculado + vlMultaCalculada);

                return (vlLiquidacao, vlJurosCalculado, vlMultaCalculada);
            }
            catch (Exception)
            {
                // Em caso de erro, retornar valores conservadores
                return (vlSaldoTitulo, 0, 0);
            }
        }

        private async Task<List<Dictionary<string, object>>> BuscarBaixasParciais(int cdTitulo, Source source)
        {
            try
            {
                // Buscar baixas parciais do título (excluindo bolsas e cancelamentos)
                var query = @"
                    SELECT * FROM T_BAIXA_TITULO 
                    WHERE cd_titulo = @cd_titulo 
                    AND cd_tipo_liquidacao NOT IN (6, 7, 8) -- Excluir cancelamentos e bolsas
                    AND id_baixa_parcial = 1
                    ORDER BY dt_baixa_titulo";

                var baixas = new List<Dictionary<string, object>>();
                var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};";

                using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new System.Data.SqlClient.SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@cd_titulo", cdTitulo);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var baixa = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    baixa[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                }
                                baixas.Add(baixa);
                            }
                        }
                    }
                }

                return baixas;
            }
            catch (Exception)
            {
                return new List<Dictionary<string, object>>();
            }
        }

        private (decimal vlJuros, decimal vlMulta) CalcularJurosMulta(decimal vlPrincipal, DateTime dtVencimento, DateTime dataBaixa, Dictionary<string, object> parametros = null)
        {
            // Se não está em atraso, não há juros nem multa
            if (dataBaixa <= dtVencimento)
            {
                return (0, 0);
            }

            // Calcular dias de atraso
            int diasAtraso = (dataBaixa.Date - dtVencimento.Date).Days;

            // Verificar carência se há parâmetros
            if (parametros != null)
            {
                int diasCarencia = 0;
                if (parametros.ContainsKey("nm_dias_carencia") && parametros["nm_dias_carencia"] != null)
                {
                    diasCarencia = Convert.ToInt32(parametros["nm_dias_carencia"]);
                }

                // Se ainda está na carência, não cobrar juros/multa
                if (diasAtraso <= diasCarencia)
                {
                    return (0, 0);
                }

                // Verificar se deve cobrar juros/multa
                bool cobrarJurosMulta = true;
                if (parametros.ContainsKey("id_cobrar_juros_multa") && parametros["id_cobrar_juros_multa"] != null)
                {
                    cobrarJurosMulta = Convert.ToBoolean(parametros["id_cobrar_juros_multa"]);
                }

                if (!cobrarJurosMulta)
                {
                    return (0, 0);
                }
            }

            // Taxas dos parâmetros - se não estiverem configuradas ou não há parâmetros, será zero
            decimal taxaJurosDia = 0;
            decimal taxaMulta = 0;

            if (parametros != null)
            {
                if (parametros.ContainsKey("pc_juros_dia") && parametros["pc_juros_dia"] != null)
                {
                    taxaJurosDia = Convert.ToDecimal(parametros["pc_juros_dia"]) / 100;
                }

                if (parametros.ContainsKey("pc_multa") && parametros["pc_multa"] != null)
                {
                    taxaMulta = Convert.ToDecimal(parametros["pc_multa"]) / 100;
                }
            }

            // Calcular juros simples
            decimal vlJuros = vlPrincipal * taxaJurosDia * diasAtraso;

            // Calcular multa (apenas após o primeiro dia de atraso)
            decimal vlMulta = diasAtraso > 0 ? vlPrincipal * taxaMulta : 0;

            return (Math.Round(vlJuros, 2), Math.Round(vlMulta, 2));
        }


        private async Task<(decimal soma_valores_desconto,decimal percentual_desconto,decimal percentual_politica)> CalcularDesconto(int cdTitulo, Source source)
        {
            decimal soma_valores_desconto = 0;
            decimal percentual_desconto = 0;
            decimal percentual_politica = 0;

            var titulo = await SQLServerService.GetFirstByFields(source, "T_TITULO", new List<(string campo, object valor)> { new("cd_titulo", cdTitulo) });
            var cd_contrato = titulo["cd_origem_titulo"];
            // pega os descontos do contrato com incide_baixa
            var desconto_contrato_get = await SQLServerService.GetList("T_DESCONTO_CONTRATO", null, "[cd_contrato],[id_incide_baixa]", $"[{cd_contrato}],[1]", source);
            if (!desconto_contrato_get.success) return new(soma_valores_desconto, percentual_desconto, percentual_politica);
            var desconto_contrato = desconto_contrato_get.data;
            // parametros da escola
            var parametro_escola = await SQLServerService.GetFirstByFields(source, "T_PARAMETRO", new List<(string campo, object valor)> { new("cd_pessoa_escola", titulo["cd_pessoa_empresa"]) });

            decimal vl_liquido = Convert.ToDecimal(titulo["vl_titulo"] ?? 0);          
            if (desconto_contrato != null)
            {
                foreach (var descontoContrato in desconto_contrato)
                {
                    decimal percentual_valor = 0;

                    soma_valores_desconto += Convert.ToDecimal(descontoContrato["vl_desconto_contrato"] ?? 0);                   

                    percentual_valor = ((Convert.ToDecimal(descontoContrato["vl_desconto_contrato"] ?? 0) == 0 || vl_liquido == 0) ? 0 :
                        (Convert.ToDecimal(descontoContrato["vl_desconto_contrato"] ?? 0) / vl_liquido * 100));
                    percentual_valor = percentual_valor == 0 ? Convert.ToDecimal(descontoContrato["pc_desconto_contrato"] ?? 0) : percentual_valor;

                    if (parametro_escola != null && parametro_escola.ContainsKey("id_somar_descontos_financeiros") && Convert.ToBoolean(parametro_escola["id_somar_descontos_financeiros"]))
                        percentual_desconto += percentual_valor;
                    else
                        percentual_desconto =
                            100 - ((1 - percentual_valor / 100) *
                                   (1 - percentual_desconto / 100)) * 100;
                }
            }
            var data_vcto_titulo = Convert.ToDateTime(titulo["dt_vcto_titulo"]);
            //politica de desconto aluno            
            var politica_desconto_aluno = await SQLServerService.GetFirstByFields(source,"T_POLITICA_DESCONTO_ALUNO", new List<(string campo, object valor)> { new("cd_aluno", titulo["cd_aluno"]) });
            if(politica_desconto_aluno != null)
            {
                var politica_desconto_escola = await SQLServerService.GetFirstByFields(source, "T_POLITICA_DESCONTO", new List<(string campo, object valor)> { new("cd_politica_desconto", politica_desconto_aluno["cd_politica_desconto"]) });
                if(politica_desconto_escola != null)
                {
                    var data_inicio_politica = Convert.ToDateTime(politica_desconto_escola["dt_inicial_politica"]);

                    //verifica se a data de inicio da politica ainda é valida para o vencimento do titulo
                    if (data_inicio_politica > data_vcto_titulo)
                    {
                        var dias_politica_get = await SQLServerService.GetList("T_DIAS_POLITICA", null, "[cd_politica_desconto]", $"[{politica_desconto_escola["cd_politica_desconto"]}]", source);
                        if (!dias_politica_get.success) return new(soma_valores_desconto, percentual_desconto, percentual_politica);
                        var dias_politica = dias_politica_get.data;
                        var dia_politica = dias_politica.Where(d => d.ContainsKey("nm_dia_semana") && d["nm_dia_semana"] != null && int.Parse(d["nm_dia_semana"].ToString()) <= data_vcto_titulo.Date.Day).FirstOrDefault();
                        if(dia_politica != null) 
                        {
                            var pc_desconto = Convert.ToDecimal(dia_politica["pc_desconto"] ?? 0);
                            percentual_politica = percentual_desconto;
                            if (parametro_escola != null && parametro_escola.ContainsKey("id_somar_descontos_financeiros") && Convert.ToBoolean(parametro_escola["id_somar_descontos_financeiros"]))
                                percentual_desconto += 0;
                            else
                                percentual_desconto =
                                    100 - ((1 - percentual_desconto / 100) *
                                           (1 - 0 / 100)) * 100;

                            if (parametro_escola != null && parametro_escola.ContainsKey("id_somar_descontos_financeiros") && Convert.ToBoolean(parametro_escola["id_somar_descontos_financeiros"]))
                                percentual_politica += pc_desconto;
                            else
                                percentual_politica =
                                    100 - (((1 - percentual_desconto / 100) *
                                            (1 - (pc_desconto) / 100))) * 100;
                        }

                    }
                }
                
            }






            return new(soma_valores_desconto, percentual_desconto, percentual_politica);
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
                var filtros = new List<(string campo, object valor)> { new("id_natureza_titulo", 1), new("cd_titulo", cd_titulo) };
                var tituloExiste = await SQLServerService.GetFirstByFields(source, "vi_titulo", filtros);
                if (tituloExiste == null) return NotFound();

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

                    cd_local_movto = tituloExiste["cd_local_movto"],
                    id_status_cnab = tituloExiste["id_status_cnab"],
                    vl_multa_titulo = tituloExiste["vl_multa_titulo"],
                    vl_juros_titulo = tituloExiste["vl_juros_titulo"],
                    vl_desconto_titulo = tituloExiste["vl_desconto_titulo"],
                    vl_liquidacao_titulo = tituloExiste["vl_liquidacao_titulo"],
                    vl_multa_liquidada = tituloExiste["vl_multa_liquidada"],
                    vl_juros_liquidado = tituloExiste["vl_juros_liquidado"],
                    vl_desconto_juros = tituloExiste["vl_desconto_juros"],
                    pc_juros_titulo = tituloExiste["pc_juros_titulo"],
                    pc_multa_titulo = tituloExiste["pc_multa_titulo"],
                    vl_material_titulo = tituloExiste["vl_material_titulo"],
                    vl_abatimento = tituloExiste["vl_abatimento"],
                    vl_desconto_contrato = tituloExiste["vl_desconto_contrato"],
                    pc_taxa_cartao = tituloExiste["pc_taxa_cartao"],
                    nm_dias_cartao = tituloExiste["nm_dias_cartao"],
                    id_cnab_contrato = tituloExiste["id_cnab_contrato"],
                    vl_taxa_cartao = tituloExiste["vl_taxa_cartao"]
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
        [HttpPost]
        [Route("cancelarMultiplosTitulos")]
        public async Task<IActionResult> CancelarMultiplosTitulos([FromBody] CancelamentoMultiploTitulosModel dados)
        {
            var schemaName = "T_Titulo";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);

            if (source != null && source.Active != null && source.Active == true)
            {
                try
                {
                    // Validar se todos os títulos existem
                    var titulosNaoEncontrados = new List<int>();
                    var titulosParaCancelar = new List<object>();

                    foreach (var cd_titulo in dados.cd_titulos)
                    {
                        var filtros = new List<(string campo, object valor)> { new("cd_titulo", cd_titulo) };
                        var titulo = await SQLServerService.GetFirstByFields(source, "T_TITULO", filtros);

                        if (titulo == null)
                        {
                            titulosNaoEncontrados.Add(cd_titulo);
                        }
                        else
                        {
                            titulosParaCancelar.Add(titulo);
                        }
                    }

                    if (titulosNaoEncontrados.Any())
                    {
                        return BadRequest(new
                        {
                            error = "Títulos não encontrados",
                            titulos_nao_encontrados = titulosNaoEncontrados
                        });
                    }

                    // Cancelar todos os títulos (cd_tipo_liquidacao = 6)
                    var titulosCancelados = new List<int>();
                    var erros = new List<string>();

                    foreach (var cd_titulo in dados.cd_titulos)
                    {
                        try
                        {
                            // Buscar dados do título para usar os valores reais
                            var filtrosTitulo = new List<(string campo, object valor)> { new("cd_titulo", cd_titulo) };
                            var titulo = await SQLServerService.GetFirstByFields(source, "T_TITULO", filtrosTitulo);

                            if (titulo == null)
                            {
                                erros.Add($"Título {cd_titulo} não encontrado");
                                continue;
                            }
                            // Obter cd_pessoa_empresa do título
                            int cd_pessoa_empresa = Convert.ToInt32(titulo["cd_pessoa_empresa"]);
                            
                            // Buscar cd_local_movto dos parâmetros
                            var cd_local_movto = await GetLocalMovto(cd_pessoa_empresa, source);
                            if (cd_local_movto == null)
                            {
                                erros.Add($"Parâmetro cd_local_movto não encontrado para a empresa {cd_pessoa_empresa}");
                                continue;
                            }

                            // Obter valores do título
                            decimal vl_titulo = Convert.ToDecimal(titulo["vl_titulo"] ?? 0);
                            decimal vl_saldo_titulo = Convert.ToDecimal(titulo["vl_saldo_titulo"] ?? vl_titulo);

                            //gera T_TRANS_FINAN
                            var tranFinDict = new Dictionary<string, object>
                            {
                                { "cd_pessoa_empresa", cd_pessoa_empresa },
                                { "cd_local_movto", cd_local_movto },
                                { "dt_tran_finan", dados.dt_cancelamento.ToString("yyyy-MM-ddTHH:mm:ss") },
                                { "cd_tipo_liquidacao", 6 },
                                { "vl_total_baixa", vl_saldo_titulo}
                            };
                            var t_tranFin_insert = await SQLServerService.InsertWithResult("T_TRAN_FINAN", tranFinDict, source);
                            if (!t_tranFin_insert.success) return BadRequest(t_tranFin_insert.error);
                            var cd_tran_fin = t_tranFin_insert.inserted["cd_tran_finan"];


                            // Inserir registro de baixa na T_BAIXA_TITULO com valores totais
                            var baixaDict = new Dictionary<string, object>
                            {
                                { "cd_titulo", cd_titulo },
                                { "cd_tran_finan", cd_tran_fin }, // Não tem transação financeira no cancelamento
                                { "cd_tipo_liquidacao", 6 }, // Cancelamento
                                { "cd_local_movto", cd_local_movto }, // Local de movimento obtido dos parâmetros
                                { "dt_baixa_titulo", dados.dt_cancelamento.ToString("yyyy-MM-ddTHH:mm:ss") },
                                { "id_baixa_processada", 0 },
                                { "id_baixa_parcial", 0 }, // Cancelamento é sempre total
                                { "nm_dias_float", 0 },
                                { "vl_liquidacao_baixa", vl_saldo_titulo },
                                { "vl_juros_baixa", 0 },
                                { "vl_desconto_baixa", 0 },
                                { "vl_principal_baixa", vl_saldo_titulo },
                                { "vl_juros_calculado", 0 },
                                { "vl_multa_calculada", 0 },
                                { "vl_desc_multa_baixa", 0 },
                                { "vl_desc_juros_baixa", 0 },
                                { "vl_multa_baixa", 0 },
                                { "pc_pontualidade", 0 },
                                { "tx_obs_baixa", dados.motivo_cancelamento ?? "Cancelamento múltiplo" },
                                { "vl_desconto_baixa_calculado", 0 },
                                { "vl_baixa_saldo_titulo", vl_saldo_titulo },
                                { "cd_politica_desconto", DBNull.Value },
                                { "cd_usuario", 1 },
                                { "vl_taxa_cartao", 0 },
                                { "vl_acr_liquidacao", 0 },
                                { "vl_liquidacao_calculado", vl_saldo_titulo },
                                { "nm_recibo", 0 } // Sem recibo para cancelamento
                            };

                            var baixaResult = await SQLServerService.Insert("T_BAIXA_TITULO", baixaDict, source);
                            if (!baixaResult.success)
                            {
                                erros.Add($"Erro ao inserir baixa para título {cd_titulo}: {baixaResult.error}");
                                continue;
                            }

                            // Atualizar status do título para cancelado
                            var tituloDict = new Dictionary<string, object>
                            {
                                { "id_status_titulo", 2 } // Status cancelado
                            };

                            var updateResult = await SQLServerService.Update("T_TITULO", tituloDict, source, "cd_titulo", cd_titulo);
                            if (!updateResult.success)
                            {
                                erros.Add($"Erro ao atualizar título {cd_titulo}: {updateResult.error}");
                                continue;
                            }

                            titulosCancelados.Add(cd_titulo);
                        }
                        catch (Exception ex)
                        {
                            erros.Add($"Erro ao processar título {cd_titulo}: {ex.Message}");
                        }
                    }

                    return Ok(new
                    {
                        sucesso = true,
                        titulos_cancelados = titulosCancelados,
                        total_cancelados = titulosCancelados.Count,
                        total_solicitados = dados.cd_titulos.Count,
                        erros = erros.Any() ? erros : null
                    });
                }
                catch (Exception ex)
                {
                    return BadRequest(new
                    {
                        error = "Erro interno ao cancelar títulos",
                        details = ex.Message
                    });
                }
            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }

        [Authorize]
        [HttpPost("simularBaixa")]
        public async Task<IActionResult> SimularBaixa([FromBody] SimularBaixaModel dados)
        {
            if (dados.cd_empresa == null) return BadRequest("campo cd_empresa não informado");
            if (dados.cd_titulos == null || !dados.cd_titulos.Any()) return BadRequest("lista de títulos não informada");
            if (dados.dt_baixa == default(DateTime)) return BadRequest("data da baixa não informada");

            var schemaName = "T_Titulo";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            
            if (source != null && source.Active != null && source.Active == true)
            {
                try
                {
                    // Buscar parâmetros da escola
                    var parametros = await BuscarParametrosEscola(Convert.ToInt32(dados.cd_empresa), source);
                    if (parametros == null)
                    {
                        return BadRequest($"Parâmetros não encontrados para a empresa {dados.cd_empresa}");
                    }

                    var titulosSimulados = new List<object>();
                    var titulosNaoEncontrados = new List<int>();
                    var erros = new List<string>();

                    foreach (var cd_titulo in dados.cd_titulos)
                    {
                        try
                        {
                            // Buscar dados do título
                            var filtrosTitulo = new List<(string campo, object valor)> 
                            { 
                                new("cd_titulo", cd_titulo),
                                new("id_natureza_titulo", 1),
                                new("cd_pessoa_empresa", dados.cd_empresa)
                            };
                            var titulo = await SQLServerService.GetFirstByFields(source, "vi_titulo", filtrosTitulo);

                            if (titulo == null)
                            {
                                titulosNaoEncontrados.Add(cd_titulo);
                                continue;
                            }

                            // Dados completos do título (como no GetAll)
                            var tituloSimulado = new Dictionary<string, object>(titulo);

                            // Verificar se o título está em aberto (status 1)
                            if (Convert.ToInt32(titulo["id_status_titulo"]) == 1)
                            {
                                decimal vlSaldoTitulo = Convert.ToDecimal(titulo["vl_saldo_titulo"] ?? titulo["vl_titulo"]);
                                
                                if (vlSaldoTitulo > 0)
                                {
                                    // Simular baixa do título
                                    var simulacaoBaixa = await SimularBaixaTitulo(titulo, dados.dt_baixa, parametros, source);
                                    
                                    tituloSimulado["vl_saldo_corrigido"] = simulacaoBaixa.vl_liquidacao_baixa;
                                    tituloSimulado["vl_juros_calculado"] = simulacaoBaixa.vl_juros_calculado;
                                    tituloSimulado["vl_multa_calculada"] = simulacaoBaixa.vl_multa_calculada;
                                    tituloSimulado["vl_principal_baixa"] = simulacaoBaixa.vl_principal_baixa;
                                    tituloSimulado["vl_desconto_baixa"] = simulacaoBaixa.vl_desconto_baixa;
                                    tituloSimulado["pc_pontualidade"] = simulacaoBaixa.pc_pontualidade;
                                    tituloSimulado["obs_baixa"] = simulacaoBaixa.obs_baixa;
                                }
                                else
                                {
                                    tituloSimulado["vl_saldo_corrigido"] = 0;
                                    tituloSimulado["vl_juros_calculado"] = 0;
                                    tituloSimulado["vl_multa_calculada"] = 0;
                                    tituloSimulado["vl_principal_baixa"] = 0;
                                    tituloSimulado["vl_desconto_baixa"] = 0;
                                    tituloSimulado["pc_pontualidade"] = 0;
                                    tituloSimulado["obs_baixa"] = "Título sem saldo";
                                }
                            }
                            else
                            {
                                // Título não está em aberto
                                tituloSimulado["vl_saldo_corrigido"] = titulo["vl_saldo_titulo"];
                                tituloSimulado["vl_juros_calculado"] = 0;
                                tituloSimulado["vl_multa_calculada"] = 0;
                                tituloSimulado["vl_principal_baixa"] = titulo["vl_saldo_titulo"];
                                tituloSimulado["vl_desconto_baixa"] = 0;
                                tituloSimulado["pc_pontualidade"] = 0;
                                tituloSimulado["obs_baixa"] = "Título não está em aberto";
                            }

                            titulosSimulados.Add(tituloSimulado);
                        }
                        catch (Exception ex)
                        {
                            erros.Add($"Erro ao processar título {cd_titulo}: {ex.Message}");
                        }
                    }

                    return Ok(new
                    {
                        sucesso = true,
                        dt_simulacao = dados.dt_baixa,
                        cd_empresa = dados.cd_empresa,
                        data = titulosSimulados,
                        total = titulosSimulados.Count,
                        titulos_nao_encontrados = titulosNaoEncontrados.Any() ? titulosNaoEncontrados : null,
                        erros = erros.Any() ? erros : null
                    });
                }
                catch (Exception ex)
                {
                    return BadRequest(new
                    {
                        error = "Erro interno ao simular baixa",
                        details = ex.Message
                    });
                }
            }
            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
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

        private async Task<SimulacaoBaixaResult> SimularBaixaTitulo(Dictionary<string, object> titulo, DateTime dataBaixa, Dictionary<string, object> parametros, Source source)
        {
            var resultado = new SimulacaoBaixaResult();
            
            try
            {
                int cdTitulo = Convert.ToInt32(titulo["cd_titulo"]);
                DateTime dtVctoTitulo = Convert.ToDateTime(titulo["dt_vcto_titulo"]);
                decimal vlSaldoTitulo = Convert.ToDecimal(titulo["vl_saldo_titulo"] ?? titulo["vl_titulo"]);
                decimal vlMaterialTitulo = Convert.ToDecimal(titulo["vl_material_titulo"] ?? 0);
                
                resultado.vl_principal_baixa = vlSaldoTitulo;
                resultado.obs_baixa = $"Conta a receber de {titulo["no_cliente"]}";

                // Buscar baixas parciais existentes
                var baixasParciais = await BuscarBaixasParciais(cdTitulo, source);
                
                if (baixasParciais.Any())
                {
                    // Lógica para baixas parciais (similar ao código original)
                    var baixasParcialDia = baixasParciais.Where(b => b.ContainsKey("dt_baixa_titulo") && 
                        Convert.ToDateTime(b["dt_baixa_titulo"]).Date == dataBaixa.Date).ToList();

                    if (baixasParcialDia.Any())
                    {
                        // Se existem baixas parciais no mesmo dia
                        resultado.vl_multa_calculada = baixasParcialDia.Sum(b => Convert.ToDecimal(b["vl_multa_calculada"] ?? 0));
                        resultado.vl_juros_calculado = baixasParcialDia.Sum(b => Convert.ToDecimal(b["vl_juros_calculado"] ?? 0));
                        resultado.vl_multa_baixa = resultado.vl_multa_calculada;
                        resultado.vl_juros_baixa = resultado.vl_juros_calculado;
                    }
                    else
                    {
                        // Baixas parciais em outros dias
                        var dtPrimeiraBaixaParcial = baixasParciais
                            .Where(b => b.ContainsKey("dt_baixa_titulo"))
                            .Min(b => Convert.ToDateTime(b["dt_baixa_titulo"]));

                        DateTime dtVctoParaCalculo = dtVctoTitulo;
                        bool baixaParcialAposVencimento = false;

                        if (dtPrimeiraBaixaParcial > dtVctoTitulo)
                        {
                            dtVctoParaCalculo = dtPrimeiraBaixaParcial;
                            baixaParcialAposVencimento = true;
                        }

                        // Calcular juros e multa
                        var calculo = CalcularJurosMultaCompleto(vlSaldoTitulo, dtVctoParaCalculo, dataBaixa, parametros);
                        
                        if (baixaParcialAposVencimento && baixasParciais.Sum(b => Convert.ToDecimal(b["vl_multa_calculada"] ?? 0)) > 0)
                        {
                            resultado.vl_multa_baixa = baixasParciais.Sum(b => Convert.ToDecimal(b["vl_multa_calculada"] ?? 0));
                        }
                        else
                        {
                            resultado.vl_multa_baixa = calculo.vlMulta;
                        }

                        if (dtVctoTitulo > dataBaixa)
                        {
                            resultado.vl_multa_baixa = 0;
                            resultado.vl_juros_baixa = 0;
                        }
                        else
                        {
                            resultado.vl_juros_baixa = calculo.vlJuros + baixasParciais.Sum(b => Convert.ToDecimal(b["vl_juros_calculado"] ?? 0));
                        }

                        resultado.vl_multa_calculada = resultado.vl_multa_baixa;
                        resultado.vl_juros_calculado = resultado.vl_juros_baixa;
                    }
                }
                else
                {
                    // Não há baixas parciais, calcular normalmente
                    var calculo = CalcularJurosMultaCompleto(vlSaldoTitulo, dtVctoTitulo, dataBaixa, parametros);
                    resultado.vl_juros_calculado = calculo.vlJuros;
                    resultado.vl_multa_calculada = calculo.vlMulta;
                    resultado.vl_juros_baixa = calculo.vlJuros;
                    resultado.vl_multa_baixa = calculo.vlMulta;
                }

                // Calcular descontos (completo)
                var descontoCalculado = await CalcularDescontosCompleto(titulo, dtVctoTitulo, dataBaixa, parametros, source);
                resultado.vl_desconto_baixa = descontoCalculado.valorDesconto;
                resultado.pc_pontualidade = descontoCalculado.percentualPontualidade;
                
                // Calcular valor final de liquidação
                resultado.vl_liquidacao_baixa = resultado.vl_principal_baixa + resultado.vl_juros_baixa + resultado.vl_multa_baixa - resultado.vl_desconto_baixa;
                
                return resultado;
            }
            catch (Exception)
            {
                // Em caso de erro, retornar valores conservadores
                resultado.vl_principal_baixa = Convert.ToDecimal(titulo["vl_saldo_titulo"] ?? titulo["vl_titulo"]);
                resultado.vl_liquidacao_baixa = resultado.vl_principal_baixa;
                resultado.obs_baixa = "Erro no cálculo - valores conservadores";
                return resultado;
            }
        }

        private (decimal vlJuros, decimal vlMulta) CalcularJurosMultaCompleto(decimal vlPrincipal, DateTime dtVencimento, DateTime dataBaixa, Dictionary<string, object> parametros)
        {
            // Se não está em atraso, não há juros nem multa
            if (dataBaixa <= dtVencimento)
            {
                return (0, 0);
            }

            // Calcular dias de atraso
            int diasAtraso = (dataBaixa.Date - dtVencimento.Date).Days;

            // Verificar carência
            int diasCarencia = 0;
            if (parametros.ContainsKey("nm_dias_carencia") && parametros["nm_dias_carencia"] != null)
            {
                diasCarencia = Convert.ToInt32(parametros["nm_dias_carencia"]);
            }

            // Se ainda está na carência, não cobrar juros/multa
            if (diasAtraso <= diasCarencia)
            {
                return (0, 0);
            }

            // Verificar se deve cobrar juros/multa
            bool cobrarJurosMulta = true;
            if (parametros.ContainsKey("id_cobrar_juros_multa") && parametros["id_cobrar_juros_multa"] != null)
            {
                cobrarJurosMulta = Convert.ToBoolean(parametros["id_cobrar_juros_multa"]);
            }

            if (!cobrarJurosMulta)
            {
                return (0, 0);
            }

            // Taxas dos parâmetros - se não estiverem configuradas, será zero
            decimal taxaJurosDia = 0;
            decimal taxaMulta = 0;

            if (parametros.ContainsKey("pc_juros_dia") && parametros["pc_juros_dia"] != null)
            {
                taxaJurosDia = Convert.ToDecimal(parametros["pc_juros_dia"]) / 100;
            }

            if (parametros.ContainsKey("pc_multa") && parametros["pc_multa"] != null)
            {
                taxaMulta = Convert.ToDecimal(parametros["pc_multa"]) / 100;
            }

            // Calcular juros simples
            decimal vlJuros = vlPrincipal * taxaJurosDia * diasAtraso;

            // Calcular multa (apenas após o primeiro dia de atraso)
            decimal vlMulta = diasAtraso > 0 ? vlPrincipal * taxaMulta : 0;

            return (Math.Round(vlJuros, 2), Math.Round(vlMulta, 2));
        }

        private async Task<(decimal valorDesconto, decimal percentualPontualidade)> CalcularDescontosCompleto(Dictionary<string, object> titulo, DateTime dtVencimento, DateTime dataBaixa, Dictionary<string, object> parametros, Source source)
        {
            try
            {
                decimal vlPrincipal = Convert.ToDecimal(titulo["vl_saldo_titulo"] ?? titulo["vl_titulo"]);
                decimal vlMaterial = Convert.ToDecimal(titulo["vl_material_titulo"] ?? 0);
                decimal vlLiquido = vlPrincipal;
                
                // Descontar material do valor principal se houver
                if (vlMaterial > 0)
                {
                    if ((vlPrincipal - vlMaterial) < 0)
                        vlLiquido = 0;
                    else
                        vlLiquido -= vlMaterial;
                }

                // Se está em atraso ou no vencimento, não há desconto por antecipação
                if (dataBaixa >= dtVencimento)
                {
                    return (0, 0);
                }

                int cdTitulo = Convert.ToInt32(titulo["cd_titulo"]);
                int cdPessoaEmpresa = Convert.ToInt32(titulo["cd_pessoa_empresa"]);
                int cd_contrato = titulo.ContainsKey("cd_origem_titulo") && titulo["cd_origem_titulo"] != null ? Convert.ToInt32(titulo["cd_origem_titulo"]) : 0;
                int cd_aluno = titulo.ContainsKey("cd_aluno") && titulo["cd_aluno"] != null ? Convert.ToInt32(titulo["cd_aluno"]) : 0;
                // 1. Buscar descontos do contrato que incidem na baixa
                decimal percentualDescontoContrato = 0;
                var descontosContrato = await BuscarDescontosContrato(cdTitulo, source);
                
                foreach (var desconto in descontosContrato)
                {
                    if (Convert.ToBoolean(desconto["id_desconto_ativo"] ?? false) && 
                        Convert.ToBoolean(desconto["id_incide_baixa"] ?? false))
                    {
                        decimal vlDescontoContrato = Convert.ToDecimal(desconto["vl_desconto_contrato"] ?? 0);
                        decimal pcDescontoContrato = Convert.ToDecimal(desconto["pc_desconto_contrato"] ?? 0);
                        
                        // Calcular percentual baseado no valor ou usar o percentual direto
                        decimal percentualValor = vlDescontoContrato > 0 && vlLiquido > 0 
                            ? (vlDescontoContrato / vlLiquido * 100) 
                            : pcDescontoContrato;
                        
                        // Aplicar desconto conforme parâmetro id_somar_descontos_financeiros
                        bool somarDescontos = parametros != null && 
                            parametros.ContainsKey("id_somar_descontos_financeiros") && 
                            Convert.ToBoolean(parametros["id_somar_descontos_financeiros"]);
                        
                        if (somarDescontos)
                        {
                            percentualDescontoContrato += percentualValor;
                        }
                        else
                        {
                            // Fórmula: 100 - ((1 - desc1/100) * (1 - desc2/100)) * 100
                            percentualDescontoContrato = 100 - ((1 - percentualValor / 100) * (1 - percentualDescontoContrato / 100)) * 100;
                        }
                    }
                }
                //busca turma do aluno vinculado ao contrato
                //TODO: mover para método
                var aluno_turma = new Dictionary<string, object>();
                var cd_turma = 0;
                IEnumerable<Dictionary<string, object>> feriadosEscola = null;
                var turmas_aluno_get = await SQLServerService.GetList("T_ALUNO_TURMA",null,"[cd_contrato],[cd_aluno]",$"[{cd_contrato}],[{cd_aluno}]", source);
                if( turmas_aluno_get.success && turmas_aluno_get.data != null && turmas_aluno_get.data.Count > 0) cd_turma = turmas_aluno_get.data[0].ContainsKey("cd_turma") && turmas_aluno_get.data[0]["cd_turma"] != null ? Convert.ToInt32(turmas_aluno_get.data[0]["cd_turma"]) : 0;

                var cd_politica = 0;
                var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};";
                var query_politica_aluno_turma = @"
                    SELECT TOP 1 p.*
                    FROM T_POLITICA_DESCONTO p
                    INNER JOIN T_POLITICA_ALUNO pa ON pa.cd_politica_desconto = p.cd_politica_desconto
                    INNER JOIN T_POLITICA_TURMA pt ON pt.cd_politica_desconto = p.cd_politica_desconto
                    WHERE pa.cd_aluno = @cd_aluno
                        AND pt.cd_turma = @cd_turma
                        AND p.id_ativo = 1
                        AND CAST(p.dt_inicial_politica AS DATE) <= @dt_vcto_titulo
                    ORDER BY p.dt_inicial_politica DESC";

                using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new System.Data.SqlClient.SqlCommand(query_politica_aluno_turma, connection))
                    {
                        command.Parameters.AddWithValue("@cd_aluno", cd_aluno);
                        command.Parameters.AddWithValue("@cd_turma", cd_turma);
                        command.Parameters.AddWithValue("@dt_vcto_titulo", dtVencimento);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                cd_politica = reader["cd_politica_desconto"] != DBNull.Value ? Convert.ToInt32(reader["cd_politica_desconto"]) : 0;
                            }
                        }
                    }
                }
                if (cd_politica == 0 && cd_turma >0)
                {
                    var query_politica_turma = @"
                    SELECT TOP 1 p.*
                    FROM T_POLITICA_DESCONTO p
                    INNER JOIN T_POLITICA_TURMA pt ON pt.cd_politica_desconto = p.cd_politica_desconto
                    WHERE pt.cd_turma = @cd_turma
                        AND p.id_ativo = 1
                        AND CAST(p.dt_inicial_politica AS DATE) <= @dt_vcto_titulo
                    ORDER BY p.dt_inicial_politica DESC";

                    using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();

                        using (var command = new System.Data.SqlClient.SqlCommand(query_politica_turma, connection))
                        {                           
                            command.Parameters.AddWithValue("@cd_turma", cd_turma);
                            command.Parameters.AddWithValue("@dt_vcto_titulo", dtVencimento);

                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    cd_politica = reader["cd_politica_desconto"] != DBNull.Value ? Convert.ToInt32(reader["cd_politica_desconto"]) : 0;
                                }
                            }
                        }
                    }

                }
                if (cd_politica == 0)
                {
                    var query_politica_aluno = @"
                    SELECT TOP 1 p.*
                    FROM T_POLITICA_DESCONTO p
                    INNER JOIN T_POLITICA_ALUNO pa ON pa.cd_politica_desconto = p.cd_politica_desconto
                    WHERE pa.cd_aluno = @cd_aluno
                        AND p.id_ativo = 1
                        AND CAST(p.dt_inicial_politica AS DATE) <= @dt_vcto_titulo
                    ORDER BY p.dt_inicial_politica DESC";

                    using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();

                        using (var command = new System.Data.SqlClient.SqlCommand(query_politica_aluno, connection))
                        {
                            command.Parameters.AddWithValue("@cd_aluno", cd_aluno);
                            command.Parameters.AddWithValue("@dt_vcto_titulo", dtVencimento);

                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    cd_politica = reader["cd_politica_desconto"] != DBNull.Value ? Convert.ToInt32(reader["cd_politica_desconto"]) : 0;
                                }
                            }
                        }
                    }

                }
                if(cd_politica == 0)
                {
                    var query_politica_escola = @"
                    SELECT TOP 1
                        p.cd_politica_desconto,
                        p.dt_inicial_politica
                    FROM T_POLITICA_DESCONTO p
                    WHERE p.cd_pessoa_escola = @cd_escola
                      AND p.id_ativo = 1
                      AND NOT EXISTS (
                            SELECT 1
                            FROM T_POLITICA_ALUNO pa
                            WHERE pa.cd_politica_desconto = p.cd_politica_desconto
                          )
                      AND NOT EXISTS (
                            SELECT 1
                            FROM T_POLITICA_TURMA pt
                            WHERE pt.cd_politica_desconto = p.cd_politica_desconto
                          )
                      AND CAST(p.dt_inicial_politica AS DATE) <= @dt_vcto_titulo
                    ORDER BY p.dt_inicial_politica DESC;";

                    using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();

                        using (var command = new System.Data.SqlClient.SqlCommand(query_politica_escola, connection))
                        {
                            command.Parameters.AddWithValue("@cd_escola", cdPessoaEmpresa);
                            command.Parameters.AddWithValue("@dt_vcto_titulo", dtVencimento);

                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    cd_politica = reader["cd_politica_desconto"] != DBNull.Value ? Convert.ToInt32(reader["cd_politica_desconto"]) : 0;
                                }
                            }
                        }
                    }
                }
                decimal percentual_anterior = percentualDescontoContrato;
                decimal percentual_politica = 0;
                percentual_politica = percentualDescontoContrato;
                if(cd_politica > 0)
                {
                    var dias_politica_get = await SQLServerService.GetList("T_DIAS_POLITICA", null, "[cd_politica_desconto]",$"[{cd_politica}]", source);
                    var dias_politica = new List<Dictionary<string, object>>();
                    if(dias_politica_get.success && dias_politica_get.data != null && dias_politica_get.data.Count>0) dias_politica = dias_politica_get.data;
                    if (dias_politica.Any())
                    {
                        var dias = dias_politica;
                        bool encontrou_politica = false;
                        //decimal percentual_politica = 0;
                        for (int i = 0; i < dias.Count && (!encontrou_politica); i++)
                        {
                            DateTime data_desconto = new DateTime();

                            //Caso não exista o dia, por exemplo, dia 31, tenta ainda o dia 30, 29 e 28:
                            bool encontrou_dia = false;
                            for (int k = 0; k < 3 && !encontrou_dia; k++)
                            {
                                try
                                {
                                    data_desconto = new DateTime(dtVencimento.Year,
                                       dtVencimento.Month, int.Parse(dias[i]["nm_dia_limite_politica"]?.ToString()??"0") - k);
                                    encontrou_dia = true;
                                }
                                catch (System.ArgumentOutOfRangeException)
                                {
                                    encontrou_dia = false;
                                }
                            }

                            if (parametros != null && bool.Parse(parametros["id_alterar_venc_final_semana"].ToString()))
                                pulaFeriadoEFinalSemana(ref data_desconto, cdPessoaEmpresa, ref feriadosEscola, false, connectionString);

                            //Se achar a política com percentual diferente de zero e a data da baixa for menor ou igual a data da política, sempre vai considerar o desconto do contrato e o desconto da política.
                            //Se achar a política com percentual diferente de zero e a data da baixa for maior que a data da política e tiver marcado vai considerar o desconto do contrato.
                            if (dias[i]["pc_desconto"] != null && int.Parse(dias[i]["pc_desconto"].ToString()) > 0)
                            {
                                    //percentual_politica = System.Convert.ToDouble(dias[i].pc_desconto.Value);
                                    if (dataBaixa.CompareTo(data_desconto) <= 0)
                                    {
                                        var pc_pontualidade = System.Convert.ToDouble(dias[i]["pc_desconto"]??0);

                                        //Aplica o percentual de pontualidade com o percentual de desconto:
                                        if (bool.Parse(parametros["id_somar_descontos_financeiros"]?.ToString() ?? "0"))
                                            percentual_politica += (decimal)pc_pontualidade;
                                        else
                                            percentual_politica =
                                                100 - (((1 - percentualDescontoContrato / 100) *
                                                        (1 - ((decimal)pc_pontualidade) / 100))) * 100;

                                        encontrou_politica = true;                                     

                                    }
                                    else
                                        //Se encontrar uma política com percentual igual a zero, sempre considerar o desconto do contrato.
                                        //Se achar a política com percentual diferente de zero e a data da baixa for maior que a data da política e tiver desmarcado vai zerar todos os descontos do contrato e da política. 
                                        if ((!encontrou_politica) && percentual_politica != 0 &&
                                            !bool.Parse(parametros["id_permitir_desc_apos_politica"]?.ToString()))
                                    {
                                        percentual_politica = 0;
                                    }
                                }
                        }
                    
                }

                }
                percentualDescontoContrato = percentual_politica;
                if (percentualDescontoContrato > 0)
                {
                    if (percentualDescontoContrato > 100)
                        percentualDescontoContrato = 100;
                        
                    decimal valorDesconto = Math.Round(percentualDescontoContrato * vlLiquido / 100, 2);
                    return (valorDesconto, 0); // pc_pontualidade = 0 pois não houve política
                }
                
                return (0, 0);
            }
            catch (Exception)
            {
                return (0, 0);
            }
        }

        private async Task<List<Dictionary<string, object>>> BuscarDescontosContrato(int cdTitulo, Source source)
        {
            try
            {
                // Baseado na lógica do método simularBaixaTitulo original
                // 1. Verificar se título é de origem "Contrato" 
                // 2. Buscar descontos usando cd_origem_titulo como cd_contrato
                
                var queryTitulo = @"
                    SELECT cd_origem_titulo, id_origem_titulo, cd_pessoa_empresa
                    FROM T_TITULO 
                    WHERE cd_titulo = @cd_titulo";

                var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};";

                using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    
                    // Buscar dados do título
                    using (var command = new System.Data.SqlClient.SqlCommand(queryTitulo, connection))
                    {
                        command.Parameters.AddWithValue("@cd_titulo", cdTitulo);
                        
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var cdOrigemTitulo = reader.IsDBNull(0) ? (int?)null : reader.GetInt32(0);
                                var idOrigemTitulo = reader.IsDBNull(1) ? (int?)null : reader.GetInt32(1);
                                var cdEscola = reader.GetInt32(2);
                                
                                // Verificar se é de origem "Contrato" 
                                // (baseado no código original: titulo.id_origem_titulo == Int32.Parse(db.LISTA_ORIGEM_LOGS["Contrato"]))
                                // 22 representa Contrato
                                if (!cdOrigemTitulo.HasValue || idOrigemTitulo != 22)
                                {
                                    return new List<Dictionary<string, object>>();
                                }
                                
                                await reader.CloseAsync();
                                
                                // Agora buscar descontos seguindo a lógica do getContratoBaixa
                                var queryDescontos = @"
                                    DECLARE @cd_contrato INT = @cd_origem_titulo;
                                    DECLARE @cd_escola INT = @cd_pessoa_escola;
                                    
                                    -- Verificar se há aditamentos de desconto (igual ao código original)
                                    IF EXISTS (
                                        SELECT 1 FROM T_ADITAMENTO a
                                        INNER JOIN T_CONTRATO c ON a.cd_contrato = c.cd_contrato
                                        WHERE a.cd_contrato = @cd_contrato 
                                        AND c.cd_pessoa_escola = @cd_escola
                                        AND a.id_tipo_aditamento IN (3, 4) -- CONCESSAO_DESCONTO = 3, PERDA_DESCONTO = 4
                                    )
                                    BEGIN
                                        -- Buscar descontos do último aditamento
                                        SELECT dc.* FROM T_DESCONTO_CONTRATO dc
                                        INNER JOIN T_ADITAMENTO a ON dc.cd_aditamento = a.cd_aditamento
                                        INNER JOIN T_CONTRATO c ON a.cd_contrato = c.cd_contrato
                                        WHERE c.cd_pessoa_escola = @cd_escola
                                        AND dc.cd_aditamento = (
                                            SELECT MAX(a2.cd_aditamento) 
                                            FROM T_ADITAMENTO a2
                                            INNER JOIN T_CONTRATO c2 ON a2.cd_contrato = c2.cd_contrato
                                            WHERE a2.cd_contrato = @cd_contrato 
                                            AND c2.cd_pessoa_escola = @cd_escola
                                            AND a2.id_tipo_aditamento IN (3, 4)
                                        )
                                    END
                                    ELSE
                                    BEGIN
                                        -- Buscar descontos diretos do contrato (fallback)
                                        SELECT dc.* FROM T_DESCONTO_CONTRATO dc
                                        INNER JOIN T_CONTRATO c ON dc.cd_contrato = c.cd_contrato
                                        WHERE dc.cd_contrato = @cd_contrato 
                                        AND c.cd_pessoa_escola = @cd_escola
                                    END";

                                var descontos = new List<Dictionary<string, object>>();
                                
                                using (var commandDescontos = new System.Data.SqlClient.SqlCommand(queryDescontos, connection))
                                {
                                    commandDescontos.Parameters.AddWithValue("@cd_origem_titulo", cdOrigemTitulo);
                                    commandDescontos.Parameters.AddWithValue("@cd_pessoa_escola", cdEscola);
                                    
                                    using (var readerDescontos = await commandDescontos.ExecuteReaderAsync())
                                    {
                                        while (await readerDescontos.ReadAsync())
                                        {
                                            var desconto = new Dictionary<string, object>();
                                            for (int i = 0; i < readerDescontos.FieldCount; i++)
                                            {
                                                desconto[readerDescontos.GetName(i)] = readerDescontos.IsDBNull(i) ? null : readerDescontos.GetValue(i);
                                            }
                                            descontos.Add(desconto);
                                        }
                                    }
                                }
                                
                                return descontos;
                            }
                        }
                    }
                }
                
                return new List<Dictionary<string, object>>();
            }
            catch (Exception)
            {
                return new List<Dictionary<string, object>>();
            }
        }

        private void pulaFeriadoEFinalSemana(ref DateTime data_opcao, int cd_escola,
            ref IEnumerable<Dictionary<string,object>> feriadosEscola, bool addDias,string connetionString)
        {
            Dictionary<string, object>? proximo_feriado = null;
            do
            {
                //Pula a data de feriado não financeiro:
                if (proximo_feriado != null)
                {
                    if (addDias)
                    {
                        data_opcao = new DateTime(
                            int.Parse(proximo_feriado["aa_feriado_fim"]?.ToString()),
                            int.Parse(proximo_feriado["mm_feriado_fim"]?.ToString()),
                            proximo_feriado["dd_feriado_fim"] != null
                                ? int.Parse(proximo_feriado["dd_feriado_fim"].ToString())
                                : data_opcao.Year
                        );
                        data_opcao = data_opcao.AddDays(1);
                    }
                    else
                    {
                        data_opcao = new DateTime(
                            int.Parse(proximo_feriado["aa_feriado"]?.ToString()),
                            int.Parse(proximo_feriado["mm_feriado"]?.ToString()),
                            proximo_feriado["dd_feriado"] != null
                                ? int.Parse(proximo_feriado["dd_feriado"].ToString())
                                : data_opcao.Year
                        );
                        data_opcao = data_opcao.AddDays(-1);
                    }
                }

                proximo_feriado = getFeriadosDentroOuAposData(cd_escola, data_opcao, true, feriadosEscola, addDias, connetionString).Result;
                // Enquanto tiver interceção da data com o feriado financeiro:
            } while (proximo_feriado != null
                    && (
                        (proximo_feriado.ContainsKey("aa_feriado") && proximo_feriado["aa_feriado"] != null
                         && proximo_feriado.ContainsKey("aa_feriado_fim") && proximo_feriado["aa_feriado_fim"] != null
                         && DateTime.Compare(data_opcao,
                                new DateTime(Convert.ToInt32(proximo_feriado["aa_feriado"]),
                                             Convert.ToInt32(proximo_feriado["mm_feriado"]),
                                             Convert.ToInt32(proximo_feriado["dd_feriado"]))) >= 0
                         && DateTime.Compare(data_opcao,
                                new DateTime(Convert.ToInt32(proximo_feriado["aa_feriado_fim"]),
                                             Convert.ToInt32(proximo_feriado["mm_feriado_fim"]),
                                             Convert.ToInt32(proximo_feriado["dd_feriado_fim"]))) <= 0)
                        ||
                        (!proximo_feriado.ContainsKey("aa_feriado") && !proximo_feriado.ContainsKey("aa_feriado_fim")
                         && DateTime.Compare(data_opcao,
                                new DateTime(data_opcao.Year,
                                             Convert.ToInt32(proximo_feriado["mm_feriado"]),
                                             Convert.ToInt32(proximo_feriado["dd_feriado"]))) >= 0
                         && DateTime.Compare(data_opcao,
                                new DateTime(data_opcao.Year,
                                             Convert.ToInt32(proximo_feriado["mm_feriado_fim"]),
                                             Convert.ToInt32(proximo_feriado["dd_feriado_fim"]))) <= 0)
                    )
);

            if (data_opcao.DayOfWeek == DayOfWeek.Saturday || data_opcao.DayOfWeek == DayOfWeek.Sunday)
            {
                while (data_opcao.DayOfWeek == DayOfWeek.Saturday || data_opcao.DayOfWeek == DayOfWeek.Sunday)
                    if (addDias)
                        data_opcao = data_opcao.AddDays(1);
                    else
                        data_opcao = data_opcao.AddDays(-1);
                pulaFeriadoEFinalSemana(ref data_opcao, cd_escola, ref feriadosEscola, addDias,connetionString);
            }
        }


        private async Task<Dictionary<string, object>> getFeriadosDentroOuAposData(int cd_escola, DateTime ultima_data, bool feriado_financeiro,IEnumerable<Dictionary<string, object>> feriadosEscola, bool addDias, string connectionString)
        {
            Dictionary<string, object> retorno = null;

            if (feriadosEscola == null)
                feriadosEscola = await GetFeriadoByEscolaAsync(cd_escola, feriado_financeiro, connectionString);

            if (feriadosEscola.Count() > 0)
            {
                IEnumerable<Dictionary<string, object>> cloneFeriadosEscola = feriadosEscola.ToList();

                cloneFeriadosEscola = cloneFeriadosEscola.Select(x => new Dictionary<string, object>
                {
                    ["aa_feriado"] = x.ContainsKey("aa_feriado") && x["aa_feriado"] != null ? x["aa_feriado"] : short.Parse(ultima_data.Year.ToString()),
                    ["aa_feriado_fim"] = x.ContainsKey("aa_feriado_fim") && x["aa_feriado_fim"] != null ? x["aa_feriado_fim"] : short.Parse(ultima_data.Year.ToString()),
                    ["dd_feriado"] = x.ContainsKey("dd_feriado") ? x["dd_feriado"] : null,
                    ["dd_feriado_fim"] = x.ContainsKey("dd_feriado_fim") ? x["dd_feriado_fim"] : null,
                    ["mm_feriado"] = x.ContainsKey("mm_feriado") ? x["mm_feriado"] : null,
                    ["mm_feriado_fim"] = x.ContainsKey("mm_feriado_fim") ? x["mm_feriado_fim"] : null,
                    ["dc_feriado"] = x.ContainsKey("dc_feriado") ? x["dc_feriado"] : null,
                    ["cod_feriado"] = x.ContainsKey("cod_feriado") ? x["cod_feriado"] : null
                });

                List<Dictionary<string, object>> listaAuxiliar = new List<Dictionary<string, object>>();
                List<Dictionary<string, object>> listFeriadoSemAno = cloneFeriadosEscola.ToList();
                for (int i = listFeriadoSemAno.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        var dict = listFeriadoSemAno[i];
                        int aa = Convert.ToInt32(dict["aa_feriado_fim"]);
                        int mm = Convert.ToInt32(dict["mm_feriado_fim"]);
                        int dd = Convert.ToInt32(dict["dd_feriado_fim"]);
                        DateTime data = new DateTime(aa, mm, dd);
                        if (addDias)
                        {
                            if (ultima_data.CompareTo(data) <= 0)
                                listaAuxiliar.Add(dict);
                        }
                        else
                        {
                            if (ultima_data.CompareTo(data) >= 0)
                                listaAuxiliar.Add(dict);
                        }
                    }
                    catch (Exception e)
                    {
                       
                    }
                }

                var listaResultante = listaAuxiliar.OrderBy(feriado => Convert.ToInt32(feriado["aa_feriado_fim"]))
                                   .ThenBy(feriado => Convert.ToInt32(feriado["mm_feriado_fim"]))
                                   .ThenBy(feriado => Convert.ToInt32(feriado["dd_feriado_fim"]));               
                if (addDias)
                    retorno = listaResultante.FirstOrDefault();
                else
                    retorno = listaResultante.LastOrDefault();

            }
            return retorno;
        }


        private async Task<IEnumerable<Dictionary<string, object>>> GetFeriadoByEscolaAsync(int cd_escola, bool feriado_financeiro, string connectionString)
        {
            var result = new Dictionary<string, object>();
            var query = @"
            SELECT *
            FROM T_FERIADO
            WHERE id_feriado_ativo = 1
            AND (@feriado_financeiro = 0 OR id_feriado_financeiro = 1)
            AND (cd_pessoa_escola IS NULL OR cd_pessoa_escola = @cd_escola)";

            var feriados = new List<Dictionary<string, object>>();

            using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new System.Data.SqlClient.SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@feriado_financeiro", feriado_financeiro ? 1 : 0);
                    command.Parameters.AddWithValue("@cd_escola", cd_escola);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var dict = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                dict[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            }
                            feriados.Add(dict);
                        }
                    }
                }
            }
            return feriados;
        }
        //BUSCA DE DESCONTO CONFORME O SGF ANTIGO
        //private async Task<List<Dictionary<string, object>>> BuscarPoliticasDesconto(int cdPessoaEmpresa, DateTime dtVencimento, Source source)
        //{
        //    try
        //    {
        //        var query = @"
        //            SELECT pd.*, dp.nm_dia_limite_politica, dp.pc_desconto
        //            FROM T_POLITICA_DESCONTO pd
        //            INNER JOIN T_DIAS_POLITICA dp ON pd.cd_politica_desconto = dp.cd_politica_desconto
        //            WHERE pd.cd_pessoa_escola = @cd_pessoa_escola
        //            AND pd.dt_inicial_politica <= @dt_vencimento
        //            AND (pd.dt_final_politica IS NULL OR pd.dt_final_politica >= @dt_vencimento)
        //            AND dp.pc_desconto > 0
        //            ORDER BY dp.nm_dia_limite_politica DESC, dp.pc_desconto DESC";

        //        var politicas = new List<Dictionary<string, object>>();
        //        var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};";

        //        using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
        //        {
        //            await connection.OpenAsync();
        //            using (var command = new System.Data.SqlClient.SqlCommand(query, connection))
        //            {
        //                command.Parameters.AddWithValue("@cd_pessoa_escola", cdPessoaEmpresa);
        //                command.Parameters.AddWithValue("@dt_vencimento", dtVencimento);

        //                using (var reader = await command.ExecuteReaderAsync())
        //                {
        //                    while (await reader.ReadAsync())
        //                    {
        //                        var politica = new Dictionary<string, object>();
        //                        for (int i = 0; i < reader.FieldCount; i++)
        //                        {
        //                            politica[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
        //                        }
        //                        politicas.Add(politica);
        //                    }
        //                }
        //            }
        //        }

        //        return politicas;
        //    }
        //    catch (Exception)
        //    {
        //        return new List<Dictionary<string, object>>();
        //    }
        //}

        public class SimulacaoBaixaResult
        {
            public decimal vl_liquidacao_baixa { get; set; }
            public decimal vl_juros_calculado { get; set; }
            public decimal vl_multa_calculada { get; set; }
            public decimal vl_juros_baixa { get; set; }
            public decimal vl_multa_baixa { get; set; }
            public decimal vl_principal_baixa { get; set; }
            public decimal vl_desconto_baixa { get; set; }
            public decimal pc_pontualidade { get; set; }
            public string obs_baixa { get; set; }
        }

        public class SimularBaixaModel
        {
            public List<int> cd_titulos { get; set; }
            public DateTime dt_baixa { get; set; }
            public string cd_empresa { get; set; }
        }
    }
}