using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class UsuarioEmpresaSGF
    {
        public string no_pessoa { get; set; }
        public string dc_reduzido_pessoa { get; set; }
        public int cd_pessoa { get; set; }
        public List<SysGrupo> Grupos { get; set; }
    }
}
