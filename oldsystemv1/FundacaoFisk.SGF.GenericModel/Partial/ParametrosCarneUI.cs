using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class ParametrosCarneUI
    {
        public int cd_contrato { get; set; }
        public int parcIniCarne { get; set; }
        public int parcFimCarne { get; set; }
        public bool imprimirCapaCarne { get; set; }
        public int cd_escola { get; set; }
        public bool contaSegura { get; set; }
        public bool sgfNew { get; set; }
    }
}
