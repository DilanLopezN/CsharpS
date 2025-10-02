using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
    public class ContaCorrenteUI : TO
    {

       
        public int cd_conta_corrente { get; set; }
        public string movimento { get; set; }
        public string planoConta { get; set; }
        public LocalMovto localDestino { get; set; }
        public LocalMovto localOrigem { get; set; }   

        public int cd_local_origem { get; set; }
        public int? cd_local_destino { get; set; }
        public int cd_movimentacao_financeira { get; set; }
        public int? cd_plano_conta { get; set; }
        public string dc_obs_conta_corrente { get; set; }
        public int cd_tipo_liquidacao { get; set; }
        public string dc_tipo_liquidacao { get; set; }

        public short nm_digito_conta_correnteOri { get; set; }
        public short nm_digito_conta_correnteDes { get; set; }

        public Nullable<decimal> vl_conta_corrente { get; set; }
        public Nullable<System.DateTime> dta_conta_corrente { get; set; }
        public byte id_tipo_movimento { get; set; }

        public int cd_pessoa_escola { get; set; }
        public int cd_local_movimento { get; set; }
        public string des_local_movto { get; set; }
        public string des_local_ori { get; set; }
        public string des_local_dest { get; set; }

        public decimal? vl_entrada { get; set; }
        public decimal? vl_saida { get; set; }
        public decimal  saldo_inicial { get; set; }
        public decimal saldo_final { get; set; }
     
        public string no_pessoa_local { get; set; }
        public byte nm_tipo_local { get; set; }
        public ObsSaldoCaixa obsSaldoCaixa { get; set; }
        
        //propriedades para relatório
        public List<LocalMovimentoWithContaUI> localMovimento { get; set; }
        public List<TipoLiquidacao> tipoLiquidacao { get; set; }
        public string tx_obs_saldo_caixa { get; set; }

        public String destino
        {
            get
            {
                string retorno = "";
                if (localDestino != null)
                    retorno = localDestino.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                        localDestino.no_local_movto + " | ag.:" + localDestino.nm_agencia + " | c/c:" + localDestino.nm_conta_corrente + "-" + localDestino.nm_digito_conta_corrente : localDestino.no_local_movto;
                return retorno;
            }
            set { }
        }

        public String origem
        {
            get
            {
                string retorno = "";
                if (localOrigem != null)
                    retorno = localOrigem.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                        localOrigem.no_local_movto + " | ag.:" + localOrigem.nm_agencia + " | c/c:" + localOrigem.nm_conta_corrente + "-" + localOrigem.nm_digito_conta_corrente : localOrigem.no_local_movto;
                return retorno;
            }
            set { }
        }    

        public string dt_conta_corrente
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dta_conta_corrente);
            }
        }

        public string tipo_movimento {

            get {
                if (id_tipo_movimento == 1) return "E";
                else return "S";
            
            }
            set { }
        }

        public string vlConta_corrente
        {
            get
            {
                if (this.vl_conta_corrente == null)
                    return "";
                return string.Format("{0:#,0.00}", this.vl_conta_corrente);
            }
        }

        public static ContaCorrenteUI fromContaCorrenteUI(ContaCorrenteUI contaCcUI)
        {
            ContaCorrenteUI retornContaCorrenteUI = new ContaCorrenteUI
            {
                cd_conta_corrente = contaCcUI.cd_conta_corrente,
                cd_local_destino = contaCcUI.cd_local_destino,
                cd_local_origem = contaCcUI.cd_local_origem,
                cd_movimentacao_financeira = contaCcUI.cd_movimentacao_financeira,
                cd_plano_conta = contaCcUI.cd_plano_conta,
                localDestino = contaCcUI.localDestino,
                localOrigem = contaCcUI.localOrigem,
                planoConta = contaCcUI.planoConta,
                movimento = contaCcUI.movimento,
                dta_conta_corrente = contaCcUI.dta_conta_corrente,
                vl_conta_corrente = contaCcUI.vl_conta_corrente,
                dc_obs_conta_corrente = contaCcUI.dc_obs_conta_corrente,
                id_tipo_movimento = contaCcUI.id_tipo_movimento,
                cd_tipo_liquidacao = contaCcUI.cd_tipo_liquidacao
            };
            return retornContaCorrenteUI;
        }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                retorno.Add(new DefinicaoRelatorio("dt_conta_corrente", "Data", AlinhamentoColuna.Left, "0.8000in"));
                retorno.Add(new DefinicaoRelatorio("origem", "Origem", AlinhamentoColuna.Left, "1.2000in"));
                retorno.Add(new DefinicaoRelatorio("destino", "Destino", AlinhamentoColuna.Left, "1.2000in"));
                retorno.Add(new DefinicaoRelatorio("tipo_movimento", "E/S", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("movimento", "Movimento", AlinhamentoColuna.Left, "1.3000in"));
                retorno.Add(new DefinicaoRelatorio("vlConta_corrente", "Valor", AlinhamentoColuna.Right, "1.0000in"));
                retorno.Add(new DefinicaoRelatorio("planoConta", "Plano", AlinhamentoColuna.Left, "1.2000in"));
                return retorno;
            }
        }
    }
}
