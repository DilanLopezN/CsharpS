using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class TipoLog {
        public enum TipoLogEnum {
            INCLUSAO = 1,
            ALTERACAO = 2,
            DELECAO = 3,
            LOGIN = 4,
            LOGOUT = 5
        }
    }
}
