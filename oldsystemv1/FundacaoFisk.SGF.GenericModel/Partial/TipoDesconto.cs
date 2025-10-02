using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel {
    public partial class TipoDesconto {
        
        /// <summary>
        /// Função que monta as colunos no relatório do Tipo de Desconto
        /// </summary>
        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();
                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_tipo_desconto", "Código"));
                retorno.Add(new DefinicaoRelatorio("dc_tipo_desconto", "Tipo Desconto"));
                retorno.Add(new DefinicaoRelatorio("incide_baixa", "Baixa", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("incide_parcela_1", "1ºParcela", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("pc_desc", "Percentual", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("tipo_desconto_ativo", "Ativo", AlinhamentoColuna.Center));
                return retorno;
            }
        }
    }
}
