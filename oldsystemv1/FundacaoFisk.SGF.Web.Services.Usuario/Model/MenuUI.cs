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
    using FundacaoFisk.SGF.Web.Services.Empresa.Model;
    public class MenuUI : TO
    {
        public List<SysMenuUI> menuPrincipal { get; set; }
        public List<SysMenuUI> menuConfiguracao { get; set; }
        public List<SysMenuUI> menuCadastrosBasicos { get; set; }

        public string nomeUsuario { get; set; }
        public int codEscolaLogada { get; set; }
        public string nomeEscolaLogada { get; set; }
        public List<EmpresaUsuarioSession> empresasUsuario { get; set; }
        public bool empresaPropria { get; set; }
    }
}
