using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class DescontoTituloCNAB
    {
        public string dta_desconto
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", this.dt_desconto);
            }
        }
        public string vlDesconto
        {
            get
            {
                return string.Format("{0:#,0.00}", this.vl_desconto);
            }
        }

    }
}
