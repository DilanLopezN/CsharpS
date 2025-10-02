using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel {
    public partial class MovimentacaoFinanceira {
         
        public enum TipoMovimentacaoFinanceira
         {
             PAGAMENTO = 1,
             RECEBIMENTO = 2,
             TRANSFERENCIA = 3,
             RETIRADA_CAIXA_DEPOSITO = 4,
             DEPOSITO_CONTA_CORRENTE = 5,
             ABERTURA_SALDO = 6,
             ACERTO_FECHAMENTO_CAIXA = 9
         }

        public string mov_financeira_ativa
        {
            get {
                return this.id_mov_financeira_ativa ? "Sim" : "Não";
            }
        }
        /// <summary>
        /// Função que monta as colunos no relatório da Movimentação Financeira
        /// </summary>
        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();
                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_movimentacao_financeira", "Código"));
                retorno.Add(new DefinicaoRelatorio("dc_movimentacao_financeira", "Movimentação Financeira"));
                retorno.Add(new DefinicaoRelatorio("mov_financeira_ativa", "Ativo", AlinhamentoColuna.Center));
                return retorno;
            }
        }

    }
}
