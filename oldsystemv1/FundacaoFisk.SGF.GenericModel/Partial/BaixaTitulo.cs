using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel {
    public partial class BaixaTitulo {

        public List<LocalMovto> locaisMovtoTitulo { get; set; }
        public List<LocalMovto> locaisMovtoBaixa { get; set; }
        public List<TipoLiquidacao> tiposLiquidacao { get; set; }
        public decimal soma_valores_desconto { get; set; }
        public int? id_origem_titulo { get; set; }

        public string obsBaixaTitulo
        {
            get
            {
                string obsBaixa = "";
                if (id_natureza_titulo != null)
                    if (id_natureza_titulo == (int)Titulo.NaturezaTitulo.RECEBER)
                        obsBaixa = "Receber de " + this.Titulo.nomeResponsavel;
                    else
                        obsBaixa = "Pagar para " + this.Titulo.nomeResponsavel;
                return obsBaixa;
            }
            set { }
        }

        public string VlLiquidacaoBaixa {
            get {
                return string.Format("{0:#,0.00}", this.vl_liquidacao_baixa);
            }
        }
        public string VLMultaBaixa
        {
            get
            {
                return string.Format("{0:#,0.00}", this.vl_multa_baixa);
            }
        }
        public string VLPrincipalBaixa
        {
            get
            {
                return string.Format("{0:#,0.00}", this.vl_principal_baixa);
            }
        }
        public string VLLiquidacaoBaixa
        {
            get
            {
                return string.Format("{0:#,0.00}", this.vl_liquidacao_baixa);
            }
        }
        public string VLJurosBaixa
        {
            get
            {
                return string.Format("{0:#,0.00}", this.vl_juros_baixa);
            }
        }
        public string VLDescontoBaixa
        {
            get
            {
                return string.Format("{0:#,0.00}", this.vl_desconto_baixa);
            }
        }
        public string dta_baixa
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_baixa_titulo);
            }
        }
        public string dc_tipo_liqui { get; set; }
        public string no_banco_baixa { get; set; }

        public SortedList sl_politicas = new SortedList();
        public List<DiasPoliticaAntecipacao> diasPoliticaAntecipacao { get; set; }
        //Atributos do título para mostrar na grid de baixa:
        public Nullable<int> nm_baixa { get; set; }
        public Nullable<int> nm_titulo { get; set; }
        public Nullable<byte> nm_parcela_titulo { get; set; }
        public string des_desconto { get; set; }
        public string dt_vcto_titulo { get; set; }
        public Nullable<byte> id_natureza_titulo { get; set; }
        public double pc_juros_calc { get; set; }
        public double pc_multa_calc { get; set; }
        public decimal vl_acr { get; set; }
        public bool id_somar_descontos_financeiros { get; set; }
        public string natureza_titulo {
            get {
                string descNatureza = "";
                if(id_natureza_titulo != null)
                    if(id_natureza_titulo == (int) Titulo.NaturezaTitulo.RECEBER)
                        descNatureza = "R";
                    else
                        descNatureza = "P";
                return descNatureza;
            }
        }

        public static BaixaTitulo changeValuesBaixaTitulo(BaixaTitulo bxContext, BaixaTitulo bxView)
        {
            bxContext.dt_baixa_titulo = bxView.dt_baixa_titulo;
            bxContext.id_baixa_parcial = bxView.id_baixa_parcial;
            bxContext.vl_desconto_baixa = bxView.vl_desconto_baixa;
            bxContext.vl_liquidacao_baixa = bxView.vl_liquidacao_baixa;
            bxContext.vl_multa_calculada = bxView.vl_multa_calculada;
            bxContext.vl_juros_calculado = bxView.vl_juros_calculado;
            bxContext.vl_multa_baixa = bxView.vl_multa_baixa;
            bxContext.vl_juros_baixa = bxView.vl_juros_baixa;
            bxContext.vl_desc_juros_baixa = bxView.vl_desc_juros_baixa;
            bxContext.vl_desc_multa_baixa = bxView.vl_desc_multa_baixa;
            bxContext.cd_tipo_liquidacao = bxView.cd_tipo_liquidacao;
            bxContext.cd_local_movto = bxView.cd_local_movto;
            bxContext.vl_principal_baixa = bxView.vl_principal_baixa;
            bxContext.pc_pontualidade = bxView.pc_pontualidade;
            bxContext.tx_obs_baixa = bxView.tx_obs_baixa;

            if (bxContext.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.CHEQUE_AVISTA || bxContext.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.CHEQUE_PRE_DATADO)
            {

                if (bxContext.ChequeBaixa != null && bxContext.ChequeBaixa.Count() > 0)
                {
                    if (bxView.ChequeBaixa != null)
                    {
                        ChequeBaixa cqContext = bxContext.ChequeBaixa.FirstOrDefault();
                        ChequeBaixa cqView = bxView.ChequeBaixa.FirstOrDefault();
                        cqContext.dt_bom_para = (DateTime)cqView.Cheque.dt_bom_para;
                        cqContext.nm_cheque = cqView.Cheque.nm_primeiro_cheque;
                        cqContext.Cheque.no_emitente_cheque = cqView.Cheque.no_emitente_cheque;
                        cqContext.Cheque.no_agencia_cheque = cqView.Cheque.no_agencia_cheque;
                        cqContext.Cheque.nm_agencia_cheque = cqView.Cheque.nm_agencia_cheque;
                        cqContext.Cheque.nm_digito_agencia_cheque = cqView.Cheque.nm_digito_agencia_cheque;
                        cqContext.Cheque.nm_conta_corrente_cheque = cqView.Cheque.nm_conta_corrente_cheque;
                        cqContext.Cheque.nm_digito_cc_cheque = cqView.Cheque.nm_digito_cc_cheque;
                        cqContext.Cheque.nm_primeiro_cheque = cqView.Cheque.nm_primeiro_cheque;
                        cqContext.Cheque.cd_banco = cqView.Cheque.cd_banco;
                    }
                    else
                        bxContext.ChequeBaixa = new List<ChequeBaixa>();
                }
                else if (bxContext.ChequeBaixa != null)
                    bxContext.ChequeBaixa = bxView.ChequeBaixa;
            }

            return bxContext;
        }

        public string valor_desconto {
            get {
                string retorno = "";

                retorno = "\nDesconto por antecipação - " + this.pc_pontualidade + "%";
                return retorno;
            }
        }

        public class DiasPoliticaAntecipacao {
            public int cd_politica_desconto { get; set; }
            public DateTime Data_politica { get; set; }
            public byte nm_dia_limite_politica { get; set; }
            public decimal pc_pontualidade { get; set; }
            public decimal pc_pontualidade_total { get; set; }
            public decimal pc_desconto_baixa { get; set; }
            public int qtd_politica { get; set; }
            public int cd_titulo { get; set; }
            public decimal vl_titulo {get;set;}
            public int nm_coluna {get;set;}
        }
    }
}