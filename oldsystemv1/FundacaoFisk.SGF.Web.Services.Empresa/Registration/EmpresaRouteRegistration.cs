using System.Web.Http;
using System.Web.Routing;
using MvcTurbine.Routing;

namespace FundacaoFisk.SGF.Web.Services.Empresa.Registration {
    public class EmpresaRouteRegistration : IRouteRegistrator {
        public void Register(RouteCollection routes) {
            routes.MapHttpRoute(
               name: "EmpresaApi",
               routeTemplate: "api/empresa/{action}",
               defaults: new { controller = "empresa"});
            routes.MapHttpRoute(
             name: "FuncionarioApi",
             routeTemplate: "api/funcionario/{action}",
             defaults: new { controller = "funcionario"});
        }
    }
}
