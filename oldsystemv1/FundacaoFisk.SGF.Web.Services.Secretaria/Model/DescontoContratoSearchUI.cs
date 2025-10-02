using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model
{
    public class DescontoContratoSearchUI
    {
        public int cd_desconto_contrato { get; set; }
        public int cd_tipo_desconto { get; set; }
        public string dc_tipo_desconto { get; set; }
        public decimal? pc_desconto { get; set; }
        public decimal? vl_desconto_contrato { get; set; }
        public bool id_incide_parcela_1 { get; set; }
        public bool id_incide_baixa { get; set; }
        public bool id_desconto_ativo { get; set; }
        public bool desc_incluso_valor { get; set; }
        public string pc_desc
        {
            get
            {
                if (this.pc_desconto == null)
                    return "";
                return string.Format("{0,00}", this.pc_desconto);
            }
        }

        public string vl_desconto
        {
            get
            {
                if (this.vl_desconto_contrato == null)
                    return "";
                return string.Format("{0:#,0.00}", this.vl_desconto_contrato);
            }
        }

    }
}
