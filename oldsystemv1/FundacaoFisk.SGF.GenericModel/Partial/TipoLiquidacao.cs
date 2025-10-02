using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel {
    public partial class TipoLiquidacao {

        public enum TipoLiqui
        {
            CHEQUE_PRE_DATADO = 4,
            LIQUIDACAO_BANCARIA = 5,
            CANCELAMENTO = 6,
            CHEQUE_AVISTA = 10,
            MOTIVO_BOLSA = 100,
            DESCONTO_FOLHA_PAGAMENTO = 101,
            MOTIVO_BOLSA_ADITIVO= 102,
            TROCA_FINANCEIRA = 110,
            CARTAO_CREDITO = 2,
            CARTAO_DEBITO = 3
        }

        public string tipo_liquidacao_ativa
        {
            get {
                return this.id_tipo_liquidacao_ativa ? "Sim" : "Não";
            }
        }

        /// <summary>
        /// Função que monta as colunos no relatório do TIpo liquidação
        /// </summary>
        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();
                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_tipo_liquidacao", "Código"));
                retorno.Add(new DefinicaoRelatorio("dc_tipo_liquidacao", "Tipo de Liquidação"));
                retorno.Add(new DefinicaoRelatorio("tipo_liquidacao_ativa", "Ativo", AlinhamentoColuna.Center));
                return retorno;
            }
        }
    }
}
