using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    using Componentes.GenericModel;
    public partial class ClasseTelefoneSGF
    {
        public const int TIPO_COMERCIAL = 1;
        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();
                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
               // retorno.Add(new DefinicaoRelatorio("cd_classe_telefone", "Código"));
                retorno.Add(new DefinicaoRelatorio("dc_classe_telefone", "Classe de Telefone", AlinhamentoColuna.Left, "3.8000in"));
                return retorno;                             
            }

        }
    }
}
