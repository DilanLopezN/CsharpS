using System.Web.Http;
using System.Web.Routing;
using MvcTurbine.Routing;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Registration {
    public class PessoaRouteRegistration : IRouteRegistrator {
        public void Register(RouteCollection routes) {
            routes.MapHttpRoute(
               name: "PessoaApi",
               routeTemplate: "api/pessoa/{action}",
               defaults: new { controller = "pessoa" }
               );
            routes.MapHttpRoute(
               name: "LocalidadeApi",
               routeTemplate: "api/localidade/{action}",
               defaults: new { controller = "localidade" }
               );
        }
    }
}
