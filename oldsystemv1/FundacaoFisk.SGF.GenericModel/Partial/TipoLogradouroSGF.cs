using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    using Componentes.GenericModel;
    public partial class TipoLogradouroSGF
    {
        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();
                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
               // retorno.Add(new DefinicaoRelatorio("cd_tipo_logradouro", "Código"));
                retorno.Add(new DefinicaoRelatorio("no_tipo_logradouro", "Tipo de Logradouro", AlinhamentoColuna.Left, "3.8000in"));
                retorno.Add(new DefinicaoRelatorio("sg_tipo_logradouro", "Sigla", AlinhamentoColuna.Center));
                return retorno;
            }

        }
    }
}
