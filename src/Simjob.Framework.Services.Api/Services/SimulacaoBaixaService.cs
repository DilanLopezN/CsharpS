using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Services.Api.Enums;
using Simjob.Framework.Services.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simjob.Framework.Services.Api.Services
{
    public class SimulacaoBaixaService
    {
        /// <summary>
        /// Simula a baixa de um título calculando juros, multa e descontos
        /// </summary>
        /// <param name="titulo">Dados do título</param>
        /// <param name="dataBaixa">Data da baixa</param>
        /// <param name="parametros">Parâmetros da escola</param>
        /// <param name="source">Fonte de dados</param>
        /// <returns>Resultado da simulação</returns>
        public async Task<SimulacaoBaixaResult> SimularBaixaTitulo(Dictionary<string, object> titulo, DateTime dataBaixa, Dictionary<string, object> parametros, Source source)
        {
            var resultado = new SimulacaoBaixaResult();
            
            try
            {
                int cdTitulo = Convert.ToInt32(titulo["cd_titulo"]);
                
                // Detectar campos de acordo com o contexto (ContaReceber vs Matricula)
                DateTime dtVctoTitulo;
                decimal vlSaldoTitulo;
                decimal vlMaterialTitulo = 0;
                string obsCliente = "";

                // Verificar se é do contexto de ContaReceber ou Matricula
                if (titulo.ContainsKey("dt_vcto_titulo"))
                {
                    // Contexto ContaReceber
                    dtVctoTitulo = Convert.ToDateTime(titulo["dt_vcto_titulo"]);
                    vlSaldoTitulo = Convert.ToDecimal(titulo["vl_saldo_titulo"] ?? titulo["vl_titulo"]);
                    vlMaterialTitulo = Convert.ToDecimal(titulo["vl_material_titulo"] ?? 0);
                    obsCliente = titulo["no_cliente"]?.ToString() ?? "";
                }
                else
                {
                    // Contexto Matricula
                    dtVctoTitulo = Convert.ToDateTime(titulo["dt_vencimento"]);
                    vlSaldoTitulo = Convert.ToDecimal(titulo["vl_saldo"] ?? titulo["vl_titulo"]);
                    vlMaterialTitulo = Convert.ToDecimal(titulo["vl_material"] ?? 0);
                    obsCliente = titulo["no_aluno"]?.ToString() ?? "";
                }
                
                resultado.vl_principal_baixa = vlSaldoTitulo;
                resultado.obs_baixa = $"Simulação baixa - {obsCliente}";

                // Buscar baixas parciais existentes
                var baixasParciais = await BuscarBaixasParciais(cdTitulo, source);
                
                if (baixasParciais.Any())
                {
                    // Lógica para baixas parciais
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
                decimal vlSaldoConservador = 0;
                if (titulo.ContainsKey("vl_saldo_titulo"))
                    vlSaldoConservador = Convert.ToDecimal(titulo["vl_saldo_titulo"] ?? titulo["vl_titulo"]);
                else
                    vlSaldoConservador = Convert.ToDecimal(titulo["vl_saldo"] ?? titulo["vl_titulo"]);

                resultado.vl_principal_baixa = vlSaldoConservador;
                resultado.vl_liquidacao_baixa = vlSaldoConservador;
                resultado.obs_baixa = "Erro no cálculo - valores conservadores";
                return resultado;
            }
        }

        /// <summary>
        /// Calcula juros e multa com base nos parâmetros da escola
        /// </summary>
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
            if (parametros != null && parametros.ContainsKey("nm_dias_carencia") && parametros["nm_dias_carencia"] != null)
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
            if (parametros != null && parametros.ContainsKey("id_cobrar_juros_multa") && parametros["id_cobrar_juros_multa"] != null)
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

            if (parametros != null && parametros.ContainsKey("pc_juros_dia") && parametros["pc_juros_dia"] != null)
            {
                taxaJurosDia = Convert.ToDecimal(parametros["pc_juros_dia"]) / 100;
            }

            if (parametros != null && parametros.ContainsKey("pc_multa") && parametros["pc_multa"] != null)
            {
                taxaMulta = Convert.ToDecimal(parametros["pc_multa"]) / 100;
            }

            // Calcular juros
            decimal vlJuros = vlPrincipal * taxaJurosDia * diasAtraso;

            // Calcular multa
            decimal vlMulta = vlPrincipal * taxaMulta;

            return (Math.Round(vlJuros, 2), Math.Round(vlMulta, 2));
        }

        /// <summary>
        /// Busca baixas parciais existentes para o título
        /// </summary>
        private async Task<List<Dictionary<string, object>>> BuscarBaixasParciais(int cdTitulo, Source source)
        {
            try
            {
                var filtros = new List<(string campo, object valor)> { new("cd_titulo", cdTitulo) };
                var baixas = await SQLServerService.GetList("T_BAIXA_TITULO", null, "[cd_titulo]", $"[{cdTitulo}]", source, SearchModeEnum.Equals);
                return baixas.success ? baixas.data : new List<Dictionary<string, object>>();
            }
            catch (Exception)
            {
                return new List<Dictionary<string, object>>();
            }
        }

        /// <summary>
        /// Calcula descontos aplicáveis ao título - versão completa baseada no ContaReceberController
        /// </summary>
        private async Task<(decimal valorDesconto, decimal percentualPontualidade)> CalcularDescontosCompleto(Dictionary<string, object> titulo, DateTime dtVencimento, DateTime dataBaixa, Dictionary<string, object> parametros, Source source)
        {
            try
            {
                decimal vlPrincipal;
                decimal vlMaterial = 0;
                
                // Detectar campos de acordo com o contexto
                if (titulo.ContainsKey("vl_saldo_titulo"))
                {
                    // Contexto ContaReceber
                    vlPrincipal = Convert.ToDecimal(titulo["vl_saldo_titulo"] ?? titulo["vl_titulo"]);
                    vlMaterial = Convert.ToDecimal(titulo["vl_material_titulo"] ?? 0);
                }
                else
                {
                    // Contexto Matricula
                    vlPrincipal = Convert.ToDecimal(titulo["vl_saldo"] ?? titulo["vl_titulo"]);
                    vlMaterial = Convert.ToDecimal(titulo["vl_material"] ?? 0);
                }

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

                // 2. Buscar descontos por antecipação
                decimal percentualDescontoAntecipacao = 0;
                if (parametros != null && parametros.ContainsKey("pc_desconto_pontualidade") && parametros["pc_desconto_pontualidade"] != null)
                {
                    percentualDescontoAntecipacao = Convert.ToDecimal(parametros["pc_desconto_pontualidade"]);
                }

                // 3. Calcular desconto total
                decimal percentualTotal = percentualDescontoContrato + percentualDescontoAntecipacao;
                
                // Limitar a 100%
                if (percentualTotal > 100)
                    percentualTotal = 100;

                decimal valorDesconto = vlLiquido * (percentualTotal / 100);

                return (Math.Round(valorDesconto, 2), percentualTotal);
            }
            catch (Exception)
            {
                return (0, 0);
            }
        }

        /// <summary>
        /// Busca descontos do contrato
        /// </summary>
        private async Task<List<Dictionary<string, object>>> BuscarDescontosContrato(int cdTitulo, Source source)
        {
            try
            {
                // Buscar através do título para encontrar o contrato
                var titulo = await SQLServerService.GetFirstByFields(source, "T_TITULO", new List<(string campo, object valor)> { new("cd_titulo", cdTitulo) });
                if (titulo == null || !titulo.ContainsKey("cd_origem_titulo"))
                    return new List<Dictionary<string, object>>();

                int cdContrato = Convert.ToInt32(titulo["cd_origem_titulo"]);
                
                var descontos = await SQLServerService.GetList("T_DESCONTO_CONTRATO", null, "[cd_contrato]", $"[{cdContrato}]", source, SearchModeEnum.Equals);
                return descontos.success ? descontos.data : new List<Dictionary<string, object>>();
            }
            catch (Exception)
            {
                return new List<Dictionary<string, object>>();
            }
        }
    }

    /// <summary>
    /// Resultado da simulação de baixa de título
    /// </summary>
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
}