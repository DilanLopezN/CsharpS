using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel {
    public partial class TipoFinanceiro {

        public enum TiposFinanceiro
        {
            BOLETO = 1,
            CARNE = 2,
            TITULO = 3,
            CHEQUE = 4,
            CARTAO = 5
        }

        public string tipo_financeiro_ativo
        {
            get {
                return this.id_tipo_financeiro_ativo ? "Sim" : "Não";
            }
        }

        /// <summary>
        /// Função que monta as colunos no relatório do Tipo Financeiro
        /// </summary>
        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();
                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_tipo_financeiro", "Código"));
                retorno.Add(new DefinicaoRelatorio("dc_tipo_financeiro", "Tipo Financeiro"));
                retorno.Add(new DefinicaoRelatorio("tipo_financeiro_ativo", "Ativo", AlinhamentoColuna.Center));
                return retorno;
            }
        }
    }
}
