using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class ItemPolitica
    {

        public string nro_dia
        {
            get
            {
                if (nm_dias_politica == null)
                    return " - ";
                else
                    return nm_dias_politica + "";
            }
        }
    }
}
