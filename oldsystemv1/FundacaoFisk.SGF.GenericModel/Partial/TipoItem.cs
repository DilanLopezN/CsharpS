using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class TipoItem
    {
        public enum TipoItemEnum
        {
            MATERIAL_DIDATICO = 1,
            ITEM_VENDA = 2,
            ITEM_BIBLIOTECA = 3,
            IMOBILIZADO = 4,
            SERVICO = 5,
            CUSTOSDESPESAS = 6,
            VENDA = 7
        } 
    }
}
