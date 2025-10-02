using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Usuario.Model {
    public class UsuarioPermissao {
        public UsuarioWebSGF usuarioWeb { get; set; }
        public List<Permissao> permissoes = new List<Permissao>();
    }
}
