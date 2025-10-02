using System.Collections.Generic;
using FundacaoFisk.SGF.Web.Services.Usuario.Model;
using Componentes.Utils;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Usuario.Comum
{
    using System;
    public interface ISysGrupoUsuarioDataAccess : IGenericRepository<SysGrupoUsuario>
    {
        SysGrupoUsuario findGrupoUsuario(int cdGrupo, int cdUsuario);
        IEnumerable<SysGrupoUsuario> findAllGrupoUsuarioByUsuario(int cd_usuario);
    }
}
