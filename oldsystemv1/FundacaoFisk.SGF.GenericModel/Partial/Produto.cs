using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{

    public partial class Produto
    {
        public enum TipoProduto
        {
            INGLES = 1,
            ESPANHOL = 2
        }

        public string produtoAtivo
        {
            get
            {
                return this.id_produto_ativo ? "Sim" : "Não";
            }
        }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_produto", "Código"));
                retorno.Add(new DefinicaoRelatorio("no_produto", "Produto", AlinhamentoColuna.Left, "4.5000in"));
                retorno.Add(new DefinicaoRelatorio("no_produto_abreviado", "Abreviado", AlinhamentoColuna.Left, "0.9000in"));
                retorno.Add(new DefinicaoRelatorio("produtoAtivo", "Ativo", AlinhamentoColuna.Center, "0.9000in"));

                return retorno;
            }
        }
    }
}
