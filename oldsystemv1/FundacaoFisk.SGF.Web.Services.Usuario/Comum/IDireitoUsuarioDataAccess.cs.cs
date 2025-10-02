using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Usuario.Comum
{
    public interface IDireitoUsuarioDataAccess : IGenericRepository<SysDireitoUsuario>
    {
        IEnumerable<SysDireitoUsuario> findAllDireitosUsuarioByUsuario(int cd_usuario);
    }
}
