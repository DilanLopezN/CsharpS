using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel {
    public partial class PlanoConta {

        /* Propriedades do Relatório de Balancete */
        private decimal _vl_saldo_anterior = 0;
        public decimal vl_saldo_anterior {
            get {
                return _vl_saldo_anterior;
            }
            set {
                this._vl_saldo_anterior = value;
            }
        }
        public decimal vl_saldo {
            get {
                return vl_saldo_anterior + vl_debito_conta - vl_credito_conta;
            }
        }
        public decimal vl_debito_conta {
            get {
                decimal retorno = 0;
                if(this.ContaCorrente != null)
                    foreach(ContaCorrente cc in this.ContaCorrente)
                        if(cc.vl_conta_corrente.HasValue) {
                            if(cc.id_tipo_movimento == (int) FundacaoFisk.SGF.GenericModel.ContaCorrente.Tipo.ENTRADA)
                                retorno += cc.vl_conta_corrente.Value;
                            if(cc.cd_local_destino.HasValue && cc.id_tipo_movimento == (int) FundacaoFisk.SGF.GenericModel.ContaCorrente.Tipo.SAIDA)
                                retorno += cc.vl_conta_corrente.Value;
                        }
                        
                return retorno;
            }
        }
        public decimal vl_credito_conta {
            get {
                decimal retorno = 0;
                if(this.ContaCorrente != null)
                    foreach(ContaCorrente cc in this.ContaCorrente)
                        if(cc.vl_conta_corrente.HasValue) {
                            if(cc.id_tipo_movimento == (int) FundacaoFisk.SGF.GenericModel.ContaCorrente.Tipo.SAIDA)
                                retorno += cc.vl_conta_corrente.Value;
                            if(cc.cd_local_destino.HasValue && cc.id_tipo_movimento == (int) FundacaoFisk.SGF.GenericModel.ContaCorrente.Tipo.ENTRADA)
                                retorno += cc.vl_conta_corrente.Value;
                        }
                return retorno;
            }
        }
    }
}
