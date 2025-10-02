using Componentes.GenericModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
    public class DescontoTituloCarne : TO
    {
        public string dc_desconto { get; set; }
        public System.DateTime dt_desconto { get; set; }
        public decimal vl_desconto { get; set; }
    }
}
