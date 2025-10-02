using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Usuario.Model
{
    using Componentes.GenericModel;
    public class SysMenuUI : TO
    {
        public int cd_menu { get; set; }
        public string no_menu { get; set; }
        public string dc_url_menu { get; set; }
        public bool id_separador { get; set; }

        public virtual ICollection<SysMenuUI> MenusFilhos { get; set; }
    }
}
