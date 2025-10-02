using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class UsuarioWebSGF
    {
        public bool selecionado = false;
        public bool isMasterUserLogado { get; set; }
        public string usuario_ativo
        {
            get
            {
                return this.id_usuario_ativo ? "Sim" : "Não";
            }
        }
        public ICollection<SysGrupoUsuario> SysGrupo { get; set; }
        public int qtdHorarios { get; set; }
        public int qtdPermissao { get; set; }
        public virtual ICollection<Escola> EmpresasUsuario { get; set; }
        public List<string> menusAreaRestrita { get; set; }
        public string passwordAreaRestrita { get; set; }
        public string emailAreaRestrita { get; set; }
        public string nameAreaRestrita { get; set; }
        public bool isAddedAreaRestrita { get; set; }

    }
}
