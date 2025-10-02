using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model
{
    public class TaxaMatriculaSearchUI
    {
        public int cd_taxa_matricula { get; set; }
        public int cd_contrato { get; set; }
        public Nullable<decimal> vl_matricula_taxa { get; set; }
        public System.DateTime dt_vcto_taxa { get; set; }
        public int nm_parcelas_taxa { get; set; }
        public decimal pc_responsavel_taxa { get; set; }
        public int cd_pessoa_responsavel_taxa { get; set; }
        public int cd_tipo_financeiro_taxa { get; set; }
        public Nullable<int> cd_plano_conta_taxa { get; set; }
        public String no_pessoa_responsavel { get; set; }
        public String dc_plano_conta_taxa { get; set; }


        public string pec_responsavel_taxa
        {
            get
            {
                if (this.pc_responsavel_taxa < 0)
                    return "";
                return string.Format("{0,00}", this.pc_responsavel_taxa);
            }
        }

        public string val_matricula_taxa
        {
            get
            {
                if (this.vl_matricula_taxa == null)
                    return "";
                return string.Format("{0:#,0.00}", this.vl_matricula_taxa);
            }
        }

        public string dta_vcto_taxa
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_vcto_taxa);
            }
        }
    }
}
