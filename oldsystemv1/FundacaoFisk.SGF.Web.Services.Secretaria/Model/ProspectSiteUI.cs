using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model
{
    public class ProspectSiteUI : TO
    {
        public int cd_prospect_site { get; set; }
        public int cd_prospect { get; set; }
        public int id_teste_online { get; set; }
        public Nullable<double> pc_acerto_teste { get; set; }
        public string dc_acerto_teste { get; set; }
        public int cd_produto { get; set; }
        public string no_produto { get; set; }
        public string dc_produto_online { get; set; }
    }
}