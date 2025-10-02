using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class Movimento
    {
        public bool gerar_titulos  {get;set;}
        public enum TipoImportacaoXML { 
            ATIVO = 1,
            INATIVO = 0
        }
        public enum TipoVinculoMovimento
        {
            PESQUISA_ITEM_MOVIMENTO = 1,
            PESQUISA_ITEM_CADASTRO_MOVIMENTO = 2,
            HAS_PESQ_NOTA_MATERIAL = 3,
            PESQUISA_VENDA_MATERIAL = 4
        }
        public enum TipoMovimentoEnum
        {
            ENTRADA = 1,
            SAIDA = 2,
            DESPESA = 3,
            SERVICO = 4,
            DEVOLUCAO = 5
        }
        public enum TipoFinanceiroEnum
        {
            TITULO = 3
        }
        public enum StatusNFEnum
        {
            ABERTO = 1,
            FECHADO = 2,
            CANCELADO = 3
        }
        public enum CfOPEnum
        {
            SAIDAFORADOESTADO = 6,
            SAIDADENTROESTADO = 5,
            ENTRADAFORAESTADO = 2,
            ENTRADADENTROESTADO = 1
        }
        public enum TipoMeioPagamento
        {

            DINHEIRO = 01,
            CHEQUE = 02,
            CARTAO_DE_CREDITO = 03,
            CARTAO_DE_DEBITO = 04,
            CREDITO_LOJA = 05,
            VALE_ALIMENTACAO = 10,
            VALE_REFEICAO = 11,
            VALE_PRESENTE = 12,
            VALE_COMBUSTIVEL = 13,
            BOLETO_BANCARIO = 15,
            SEM_PAGAMENTO = 90,
            OUTROS = 99
        }

        public List<TipoFinanceiro> tiposFinan { get; set; }
        public List<LocalMovto> bancos { get; set; }
        public List<Banco> bancosCheque { get; set; }
        public List<Titulo> titulos { get; set; }
        public List<SituacaoTributaria> situacoesTributariaICMS { get; set; }
        public List<SituacaoTributaria> situacoesTributariaPIS { get; set; }
        public List<SituacaoTributaria> situacoesTributariaCOFINS { get; set; }
        public int? cd_pessoa_aluno { get; set; }
        public string no_pessoa { get; set; }
        public string no_aluno { get; set; }
        public string dc_politica_comercial { get; set; }
        public string dc_tipo_nota { get; set; }
        public double pc_reduzido_nf { get; set; }
        public Cheque cheque { get; set; }
        public bool id_bloquear_venda_sem_estoque { get; set; }
        public int? cd_sit_trib_ICMS {get;set;}
        public bool id_nf_movimento_antigo { get; set; }
        public string dc_futura
        {
            get
            {
                string descFutura = "";
                if (id_venda_futura == true)
                    descFutura = "Sim";
                else
                    descFutura = "Não";
                return descFutura;
            }
        }
        public string dc_nm_movimento
        {
            get
            {
                string numero = "";
                numero = nm_movimento.ToString() + dc_serie_movimento == null ? "" : "-" + dc_serie_movimento;
                return numero;
            }
        }

        public static Movimento changeValuesMovimento(Movimento mvtoContext, Movimento movtoView)
        {
            mvtoContext.cd_pessoa = movtoView.cd_pessoa;
            mvtoContext.cd_aluno = movtoView.cd_aluno;
            mvtoContext.cd_politica_comercial = movtoView.cd_politica_comercial;
            mvtoContext.cd_tipo_financeiro = movtoView.cd_tipo_financeiro;
            mvtoContext.nm_movimento = movtoView.nm_movimento;
            mvtoContext.dc_serie_movimento = movtoView.dc_serie_movimento;
            mvtoContext.dt_emissao_movimento = movtoView.dt_emissao_movimento;
            mvtoContext.dt_vcto_movimento = movtoView.dt_vcto_movimento;
            mvtoContext.dt_mov_movimento = movtoView.dt_mov_movimento;
            mvtoContext.pc_acrescimo = movtoView.pc_acrescimo;
            mvtoContext.vl_acrescimo = movtoView.vl_acrescimo;
            mvtoContext.pc_desconto = movtoView.pc_desconto;
            mvtoContext.vl_desconto = movtoView.vl_desconto;
            mvtoContext.tx_obs_movimento = movtoView.tx_obs_movimento;
            switch (mvtoContext.id_tipo_movimento)
            {
                //case (int)Movimento.TipoMovimentoEnum.COMPRA:
                //case (int)Movimento.TipoMovimentoEnum.VENDAPRODUTO:
                //    break;
                case (int)Movimento.TipoMovimentoEnum.SERVICO:
                    mvtoContext.cd_tipo_nota_fiscal = movtoView.cd_tipo_nota_fiscal;
                    mvtoContext.vl_base_calculo_ISS_nf = movtoView.vl_base_calculo_ISS_nf;
                    mvtoContext.vl_ISS_nf = movtoView.vl_ISS_nf;
                    mvtoContext.dc_cfop_nf = movtoView.dc_cfop_nf;
                    mvtoContext.cd_cfop_nf = movtoView.cd_cfop_nf;
                    mvtoContext.tx_obs_fiscal = movtoView.tx_obs_fiscal;
                    mvtoContext.id_status_nf = movtoView.id_status_nf;
                    mvtoContext.id_nf = movtoView.id_nf;
                    mvtoContext.id_nf_escola = movtoView.id_nf_escola;
                    mvtoContext.vl_aproximado = movtoView.vl_aproximado;
                    mvtoContext.pc_aliquota_aproximada = movtoView.pc_aliquota_aproximada;
                    if (mvtoContext.id_status_nf == null)
                        mvtoContext.id_status_nf = (int)Movimento.StatusNFEnum.ABERTO;
                    break;
                case (int)Movimento.TipoMovimentoEnum.ENTRADA: 
                case (int)Movimento.TipoMovimentoEnum.SAIDA:
                    mvtoContext.cd_tipo_nota_fiscal = movtoView.cd_tipo_nota_fiscal;
                    mvtoContext.cd_cfop_nf = movtoView.cd_cfop_nf;
                    mvtoContext.dc_cfop_nf = movtoView.dc_cfop_nf;
                    mvtoContext.tx_obs_fiscal = movtoView.tx_obs_fiscal;
                    mvtoContext.vl_base_calculo_ICMS_nf = movtoView.vl_base_calculo_ICMS_nf;
                    mvtoContext.vl_base_calculo_COFINS_nf = movtoView.vl_base_calculo_COFINS_nf;
                    mvtoContext.vl_base_calculo_PIS_nf = movtoView.vl_base_calculo_PIS_nf;
                    mvtoContext.vl_base_calculo_IPI_nf = movtoView.vl_base_calculo_IPI_nf;
                    mvtoContext.vl_ICMS_nf = movtoView.vl_ICMS_nf;
                    mvtoContext.vl_PIS_nf = movtoView.vl_PIS_nf;
                    mvtoContext.vl_COFINS_nf = movtoView.vl_COFINS_nf;
                    mvtoContext.vl_IPI_nf = movtoView.vl_IPI_nf;
                    mvtoContext.id_nf = movtoView.id_nf;
                    mvtoContext.vl_aproximado = movtoView.vl_aproximado;
                    mvtoContext.pc_aliquota_aproximada = movtoView.pc_aliquota_aproximada;
                    mvtoContext.id_nf_escola = movtoView.id_nf_escola;
                    if (mvtoContext.id_status_nf == null)
                        mvtoContext.id_status_nf = (int)Movimento.StatusNFEnum.ABERTO;
                    mvtoContext.dc_key_nfe = movtoView.dc_key_nfe;
                    mvtoContext.dc_meio_pagamento = movtoView.dc_meio_pagamento;
                    
                    break;
                case (int)Movimento.TipoMovimentoEnum.DEVOLUCAO:
                    mvtoContext.cd_tipo_nota_fiscal = movtoView.cd_tipo_nota_fiscal;
                    mvtoContext.cd_cfop_nf = movtoView.cd_cfop_nf;
                    mvtoContext.dc_cfop_nf = movtoView.dc_cfop_nf;
                    mvtoContext.tx_obs_fiscal = movtoView.tx_obs_fiscal;
                    mvtoContext.vl_base_calculo_ICMS_nf = movtoView.vl_base_calculo_ICMS_nf;
                    mvtoContext.vl_base_calculo_COFINS_nf = movtoView.vl_base_calculo_COFINS_nf;
                    mvtoContext.vl_base_calculo_PIS_nf = movtoView.vl_base_calculo_PIS_nf;
                    mvtoContext.vl_base_calculo_IPI_nf = movtoView.vl_base_calculo_IPI_nf;
                    mvtoContext.vl_ICMS_nf = movtoView.vl_ICMS_nf;
                    mvtoContext.vl_PIS_nf = movtoView.vl_PIS_nf;
                    mvtoContext.vl_COFINS_nf = movtoView.vl_COFINS_nf;
                    mvtoContext.vl_IPI_nf = movtoView.vl_IPI_nf;
                    mvtoContext.id_nf = movtoView.id_nf;
                    mvtoContext.vl_aproximado = movtoView.vl_aproximado;
                    mvtoContext.pc_aliquota_aproximada = movtoView.pc_aliquota_aproximada;
                    mvtoContext.id_nf_escola = movtoView.id_nf_escola;
                    if (mvtoContext.id_status_nf == null)
                        mvtoContext.id_status_nf = (int)Movimento.StatusNFEnum.ABERTO;
                    mvtoContext.cd_nota_fiscal = movtoView.cd_nota_fiscal;
                    break;
            }
            return mvtoContext;
        }

        public static string retornarTipoMovimento(byte id_tipo_movimento)
        {
            string nomeTpMovto = "";
            switch (id_tipo_movimento)
            {
                case (int)Movimento.TipoMovimentoEnum.ENTRADA:
                    {
                        nomeTpMovto = "Entrada ";
                        break;
                    }
                case (int)Movimento.TipoMovimentoEnum.SAIDA:
                    {
                        nomeTpMovto = "Saída";
                        break;
                    }
                case (int)Movimento.TipoMovimentoEnum.SERVICO:
                    {
                        nomeTpMovto = "Serviço";
                        break;
                    }
                case (int)Movimento.TipoMovimentoEnum.DEVOLUCAO:
                    {
                        nomeTpMovto = "Devolução";
                        break;
                    }
            }
            return nomeTpMovto;
        }

        public static string gerarObservacaoKardex(Movimento movimento)
        {
            var retorno = "";
            string nomeTpMovto = Movimento.retornarTipoMovimento(movimento.id_tipo_movimento);
            string descOperacao = "movimento";
            if (movimento.id_nf)
                descOperacao = "nota fiscal";
            retorno = nomeTpMovto + " conforme " + descOperacao + " nro. " + movimento.nm_movimento + " de " + movimento.no_pessoa + ".";
            return retorno;
        }

        public string status_nf {
            get {
                var retorno = "";
                if(id_status_nf != null)
                    switch((byte) id_status_nf) {
                        case (byte) Movimento.StatusNFEnum.ABERTO: 
                            retorno = "aberta";
                            break;
                        case (byte) Movimento.StatusNFEnum.FECHADO:
                            retorno = "fechada";
                            break;
                        case (byte) Movimento.StatusNFEnum.CANCELADO: 
                            retorno = "cancelada";
                            break;
                    }

                return retorno;
            }
        }
        public string dc_nota_devolucao { get; set; }
        

        //Propriedades para composição da nota fiscal de serviço:
        public short nm_cfop { get; set; }
        public int cd_dados_nf { get; set; }
        public decimal? vl_liquido_itens { get; set; }

        public int? nm_contrato { get; set; }

        public Nullable<int> nm_matricula_contrato { get; set; }
        

        public string no_contrato
        {
            get
            {
                if ((nm_contrato != null && nm_matricula_contrato != null && nm_contrato == nm_matricula_contrato) || (nm_contrato != null && nm_matricula_contrato == null))
                {
                    return String.Format("Contrato: {0}", nm_contrato);
                }
                else if (nm_contrato != null && nm_matricula_contrato != null && nm_contrato != nm_matricula_contrato)
                {
                    return String.Format("Contrato: {0}, Número: {1}", nm_contrato, nm_matricula_contrato);
                }
                else
                {
                    return null;
                }

            }
        }

        public List<ContratoComboUI> contratos_combo_material_didatico { get; set; }

        public string no_curso { get; set; }

        public string vl_total_geral {
            get {
                return string.Format(CultureInfo.InvariantCulture, "{0:0.00}", this.vl_liquido_itens);
            }
        }
        public string vlISS_NF {
            get {
                return string.Format(CultureInfo.InvariantCulture, "{0:0.00}", this.vl_ISS_nf);
            }
        }
        public string dta_emissao_movimento {
            get {
                if(dt_emissao_movimento != null)
                    return String.Format("{0:yyyy-MM-dd}", dt_emissao_movimento);
                else
                    return String.Empty;
            }
        }
        public string dta_autorizacao_nfe
        {
            get
            {
                if (dt_autorizacao_nfe != null)
                    return String.Format("{0:dd/MM/yyyy HH:mm:ss}", dt_autorizacao_nfe);
                else
                    return String.Empty;
            }
        }
        public string dta_nfe_cancel
        {
            get
            {
                if (dt_nfe_cancel != null)
                    return String.Format("{0:dd/MM/yyyy HH:mm:ss}", dt_nfe_cancel);
                else
                    return String.Empty;
            }
        }
        
        //Propriedades para composição da nota fiscal de produto:
        public string dta_emissao_movimento_produto {
            get {
                if(dt_emissao_movimento != null)
                    return String.Format("{0:yyyy-MM-ddT00:00:00-03:00}", dt_emissao_movimento);
                else
                    return String.Empty;
            }
        }
        public string dta_mov_movimento_produto {
            get {
                if(dt_mov_movimento != null)
                    return String.Format("{0:yyyy-MM-ddT00:00:00-03:00}", dt_mov_movimento);
                else
                    return String.Empty;
            }
        }
        public string vlBaseCalculoICMS_NFInvariante {
            get {
                return string.Format(CultureInfo.InvariantCulture, "{0:0.00}", this.vl_base_calculo_ICMS_nf);
            }
        }
        public string vlICMS_NFInvariante {
            get {
                return string.Format(CultureInfo.InvariantCulture, "{0:0.00}", this.vl_ICMS_nf);
            }
        }
        public string vlDescontoInvariante {
            get {
                return string.Format(CultureInfo.InvariantCulture, "{0:0.00}", this.vl_desconto);
            }
        }
        public string vl_IPI_Nf_Invariante {
            get {
                return string.Format(CultureInfo.InvariantCulture, "{0:0.00}", this.vl_IPI_nf);
            }
        }
        public string vl_PIS_Nf_Invariante {
            get {
                return string.Format(CultureInfo.InvariantCulture, "{0:0.00}", this.vl_PIS_nf);
            }
        }
        public string vl_COFINS_Nf_Invariante {
            get {
                return string.Format(CultureInfo.InvariantCulture, "{0:0.00}", this.vl_COFINS_nf);
            }
        }
        public string vlTotTribute {
            get {
                return string.Format(CultureInfo.InvariantCulture, "{0:0.00}", this.vl_ICMS_nf + this.vl_PIS_nf + this.vl_COFINS_nf + this.vl_IPI_nf);
            }
        }
        public string vlAproximado {
            get {
                return string.Format(CultureInfo.InvariantCulture, "{0:0.00}", this.vl_aproximado);
            }
        }
        public string pcAliquotaAproximada {
            get {
                return string.Format(CultureInfo.InvariantCulture, "{0:0.0000}", this.pc_aliquota_aproximada);
            }
        }

    }
}
