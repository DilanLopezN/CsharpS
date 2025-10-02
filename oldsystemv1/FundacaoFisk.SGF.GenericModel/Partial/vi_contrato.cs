using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class vi_contrato
    {
        public ICollection<DescontoContrato> DescontoContrato { get; set; }

        public string desc_descontos_contrato { get; set; }
        //{
        //    get
        //    {
        //        string retorno = "";
        //        if (DescontoContrato != null && DescontoContrato.Count() > 0)
        //            foreach (DescontoContrato d in DescontoContrato)
        //                if (retorno == "")
        //                    retorno += d.dc_tipo_desconto;
        //                else
        //                    retorno += ", " + d.dc_tipo_desconto;
        //        return retorno;
        //    }
        //    set { }
        //}
    }
}
