using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class LogGeral
    {
        public string no_login { get; set; }
        public string dc_tipo_log { get; set; }

        internal static void Add(LogGeral x)
        {
            throw new NotImplementedException();
        }
    }
}
