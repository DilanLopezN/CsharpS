using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class Item
    {
        public Nullable<int> qt_estoque { get; set; }
        public decimal vl_item { get; set; }
        public decimal vl_custo { get; set; }
        public byte id_categoria_grupo { get; set; }
        public bool id_movto_estoque { get; set; }
        public string desc_plano_conta { get; set; }
    }
}
