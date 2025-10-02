using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    using Componentes.GenericModel;
    public partial class TipoTelefoneSGF
    {

        public enum TipoTelefoneSGFEnum { TELEFONE = 1, EMAIL = 4, SITE = 5, CELULAR = 3 }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();
                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_tipo_telefone", "Código"));
                retorno.Add(new DefinicaoRelatorio("no_tipo_telefone", "Tipo de Contato", AlinhamentoColuna.Left, "3.8000in"));
                return retorno;
            }
        }
    }
}
