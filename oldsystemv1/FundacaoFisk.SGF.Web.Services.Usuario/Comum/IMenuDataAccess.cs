using System.Collections.Generic;
using FundacaoFisk.SGF.Web.Services.Usuario.Model;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Usuario.Comum
{
    public interface IMenuDataAccess : IGenericRepository<SysMenu>
    {
        IEnumerable<SysMenuUI> GetMenusUsuario(int? codEscola, int? codUsuario, bool administrador);
        List<SysMenu> GetFuncionalidadesUsuario(int? codEscola, int codUsuario, bool administrador);
        List<SysMenu> GetFuncionalidadesUsuarioArvore(int? codUsuario);
        List<SysMenu> GetFuncionalidadesGrupoArvore(int? codGrupo);
        int GetMenuEspecial();
    }
}
