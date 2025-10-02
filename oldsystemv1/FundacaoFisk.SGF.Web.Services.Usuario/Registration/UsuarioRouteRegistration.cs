using System.Web.Http;
using System.Web.Routing;
using MvcTurbine.Routing;

namespace FundacaoFisk.SGF.Web.Services.Usuario.Registration {
    public class UsuarioRouteRegistration : IRouteRegistrator {
        public void Register(RouteCollection routes) {
            routes.MapHttpRoute(
               name: "UsuarioApi",
               routeTemplate: "api/usuario/{action}",
               defaults: new { controller = "usuario" }
               );
            routes.MapHttpRoute(
               name: "UsuarioSenhaApi",
               routeTemplate: "api/usuariosenha/{action}",
               defaults: new { controller = "usuariosenha" }
               );
            routes.MapHttpRoute(
               name: "PermissaoApi",
               routeTemplate: "api/permissao/{action}",
               defaults: new { controller = "permissao" }
               );
        }
    }
}
