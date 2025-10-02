using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class TransacaoFinanceira
    {
        public enum TipoPesquisaTranFinan
        {
            CONTEXTO = 1,
            EDIT_VIEW = 2
        }

        public enum TipoCancelamento {
            CANCELAMENTO = 6
        }
        public List<LocalMovto> LocaisMovimento { get; set; }
        public List<TipoLiquidacao> TiposLiquidacao { get; set; }
        public List<Titulo> titulosBaixa { get; set; }
        public List<Banco> bancos { get; set; }
        public int cd_contrato { get; set; }
        public int cd_movimento { get; set; }
        public bool id_liquidacao_tit_ant_aberto { get; set; }
        public bool isSupervisor { get; set; }
        public int movimentoRetroativo { get; set; }
        public Cheque cheque { get; set; }
        public int cd_tipo_liquidacao_old { get; set; }

        public static TransacaoFinanceira changeValuesTransacaoFinanceira(TransacaoFinanceira tFinanContext, TransacaoFinanceira tFinanView)
        {
            tFinanContext.cd_local_movto = tFinanView.cd_local_movto;
            tFinanContext.cd_tipo_liquidacao = tFinanView.cd_tipo_liquidacao;
            tFinanContext.vl_total_baixa = tFinanView.vl_total_baixa;
            tFinanContext.vl_total_troco = tFinanView.vl_total_troco;
            tFinanContext.vl_troco = tFinanView.vl_troco;
            tFinanContext.dt_tran_finan = tFinanView.dt_tran_finan.HasValue ? tFinanView.dt_tran_finan.Value.ToLocalTime() : tFinanView.dt_tran_finan;
            if (tFinanContext.ChequeTransacaoFinanceira != null && tFinanContext.ChequeTransacaoFinanceira.Count() > 0)
            {
                if (tFinanView.cheque != null)
                {
                    ChequeTransacao cqtranContext = tFinanContext.ChequeTransacaoFinanceira.FirstOrDefault();
                    cqtranContext.dt_bom_para = (DateTime)tFinanView.cheque.dt_bom_para;
                    cqtranContext.nm_cheque = tFinanView.cheque.nm_primeiro_cheque;
                    cqtranContext.Cheque.no_emitente_cheque = tFinanView.cheque.no_emitente_cheque;
                    cqtranContext.Cheque.no_agencia_cheque = tFinanView.cheque.no_agencia_cheque;
                    cqtranContext.Cheque.nm_agencia_cheque = tFinanView.cheque.nm_agencia_cheque;
                    cqtranContext.Cheque.nm_digito_agencia_cheque = tFinanView.cheque.nm_digito_agencia_cheque;
                    cqtranContext.Cheque.nm_conta_corrente_cheque = tFinanView.cheque.nm_conta_corrente_cheque;
                    cqtranContext.Cheque.nm_digito_cc_cheque = tFinanView.cheque.nm_digito_cc_cheque;
                    cqtranContext.Cheque.nm_primeiro_cheque = tFinanView.cheque.nm_primeiro_cheque;
                    cqtranContext.Cheque.cd_banco = tFinanView.cheque.cd_banco;
                }
                else
                    tFinanContext.ChequeTransacaoFinanceira = new List<ChequeTransacao>();
            }
            else if (tFinanContext.ChequeTransacaoFinanceira != null)
                tFinanContext.ChequeTransacaoFinanceira = tFinanView.ChequeTransacaoFinanceira;

            return tFinanContext;
        }
    }
}
