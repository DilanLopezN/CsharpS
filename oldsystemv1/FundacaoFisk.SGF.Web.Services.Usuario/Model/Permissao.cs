using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Web.Services.Usuario.Model {
    public class Permissao {

        public Permissao()
        {
            children = children = new List<Permissao>();
        }

        public int id { get; set; }
        public int cd_direito_grupo { get; set; }
        public string permissao { get; set; }
        public bool visualizar { get; set; }
        public bool incluir { get; set; }
        public bool alterar { get; set; }
        public bool excluir { get; set; }
        public int pai { get; set; }
        public bool ehPermitidoEditar { get; set; }
        public ICollection<Permissao> children { get; set; }
    }
}
