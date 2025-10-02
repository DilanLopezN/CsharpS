using System.Data;
using MvcTurbine.ComponentModel;
using FundacaoFisk.SGF.Web.Services.Usuario.Comum;
using FundacaoFisk.SGF.Web.Services.Usuario.Business;
using FundacaoFisk.SGF.Web.Services.Usuario.DataAccess;

namespace FundacaoFisk.SGF.Web.Services.Usuario.Registration {
    public class UsuarioServiceRegistration : IServiceRegistration {
        public void Register(IServiceLocator locator) {
            locator.Register<IUsuarioBusiness, UsuarioBusiness>();
            locator.Register<IPermissaoBusiness, PermissaoBusiness>();
            locator.Register<IUsuarioDataAccess, UsuarioDataAccess>();
            locator.Register<IGrupoDataAccess, GrupoDataAccess>();
            locator.Register<IMenuDataAccess, MenuDataAccess>();
            locator.Register<ISysDireitoGrupoDataAccess, SysDireitoGrupoDataAccess>();
            locator.Register<ISysGrupoUsuarioDataAccess, SysGrupoUsuarioDataAccess>();
            locator.Register<IUsuarioEmpresaDataAccess, UsuarioEmpresaDataAccess>();
            locator.Register<IDireitoUsuarioDataAccess, DireitoUsuarioDataAccess>();
        }
    }
}