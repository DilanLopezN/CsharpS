using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class Cheque
    {
        public DateTime? dt_bom_para { get; set; }
        public bool existe_cheque { get; set; }
        public int? cd_cheque_trans { get; set; }
        public int? cd_tran_finan { get; set; }
        public string nm_cheque { get; set; }

        public static void changeValuesCheque(Cheque chequeContext, Cheque chequeView)
        {
            chequeContext.no_emitente_cheque = chequeView.no_emitente_cheque;
            chequeContext.no_agencia_cheque = chequeView.no_agencia_cheque;
            chequeContext.nm_agencia_cheque = chequeView.nm_agencia_cheque;
            chequeContext.nm_digito_agencia_cheque = chequeView.nm_digito_agencia_cheque;
            chequeContext.nm_conta_corrente_cheque = chequeView.nm_conta_corrente_cheque;
            chequeContext.nm_digito_cc_cheque = chequeView.nm_digito_cc_cheque;
            chequeContext.nm_primeiro_cheque = chequeView.nm_primeiro_cheque;
            chequeContext.cd_banco = chequeView.cd_banco;
        }

        public static bool validarDadosCheque(Cheque cheque, bool dadosAdicionais)
        {
            bool valido = true;
            if (cheque != null)
            {
                if (string.IsNullOrEmpty(cheque.no_emitente_cheque) || string.IsNullOrEmpty(cheque.no_agencia_cheque) ||
                    cheque.nm_digito_agencia_cheque < 0 || cheque.nm_conta_corrente_cheque <= 0 ||
                    cheque.nm_digito_cc_cheque < 0 || string.IsNullOrEmpty(cheque.nm_primeiro_cheque) || cheque.cd_banco <= 0)
                    valido = false;
                if (dadosAdicionais && (!cheque.dt_bom_para.HasValue))
                    valido = false;
            }
            else
                valido = false;
            return valido;
        }
    }
}










