using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Services.Api.Enums;
using Simjob.Framework.Services.Api.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

            // Calcular juros
            decimal vlJuros = vlPrincipal * taxaJurosDia * diasAtraso;

            // Calcular multa (apenas após o primeiro dia de atraso)
            decimal vlMulta = diasAtraso > 0 ? vlPrincipal * taxaMulta : 0;

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
                if (titulo["dc_tipo_titulo"]?.ToString() == "TX")
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
                var turmas_aluno_get = await SQLServerService.GetList("T_ALUNO_TURMA", null, "[cd_contrato],[cd_aluno]", $"[{cd_contrato}],[{cd_aluno}]", source);
                if (turmas_aluno_get.success && turmas_aluno_get.data != null && turmas_aluno_get.data.Count > 0) cd_turma = turmas_aluno_get.data[0].ContainsKey("cd_turma") && turmas_aluno_get.data[0]["cd_turma"] != null ? Convert.ToInt32(turmas_aluno_get.data[0]["cd_turma"]) : 0;

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
                if (cd_politica == 0 && cd_turma > 0)
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
                if (cd_politica == 0)
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
                if (cd_politica > 0)
                {
                    var dias_politica_get = await SQLServerService.GetList("T_DIAS_POLITICA", null, "[cd_politica_desconto]", $"[{cd_politica}]", source);
                    var dias_politica = new List<Dictionary<string, object>>();
                    if (dias_politica_get.success && dias_politica_get.data != null && dias_politica_get.data.Count > 0) dias_politica = dias_politica_get.data;
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
                                       dtVencimento.Month, int.Parse(dias[i]["nm_dia_limite_politica"]?.ToString() ?? "0") - k);
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
                                    var pc_pontualidade = System.Convert.ToDouble(dias[i]["pc_desconto"] ?? 0);

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

        /// <summary>
        /// Busca descontos do contrato
        /// </summary>
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
            ref IEnumerable<Dictionary<string, object>> feriadosEscola, bool addDias, string connetionString)
        {
            Dictionary<string, object> proximo_feriado = null;
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
                pulaFeriadoEFinalSemana(ref data_opcao, cd_escola, ref feriadosEscola, addDias, connetionString);
            }
        }

        private async Task<Dictionary<string, object>> getFeriadosDentroOuAposData(int cd_escola, DateTime ultima_data, bool feriado_financeiro, IEnumerable<Dictionary<string, object>> feriadosEscola, bool addDias, string connectionString)
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
                    catch (Exception)
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