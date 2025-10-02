using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class TipoLocalidadeSGF
    {
        public enum TipoLocalidadeSGFEnum
        {
            PAIS = 1,
            ESTADO = 2,
            CIDADE = 3,
            BAIRRO = 4,
            DISTRITO = 5,
            LOGRADOURO = 6
        }
    }
}
