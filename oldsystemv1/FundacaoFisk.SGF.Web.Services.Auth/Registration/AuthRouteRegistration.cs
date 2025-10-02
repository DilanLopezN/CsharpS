using System.Web.Http;
using System.Web.Routing;
using MvcTurbine.Routing;

namespace FundacaoFisk.SGF.Web.Services.Auth.Registration {
    public class AuthRouteRegistration : IRouteRegistrator {
        public void Register(RouteCollection routes) {
            routes.MapHttpRoute(
               name: "AuthApi",
               routeTemplate: "api/auth/{action}",
               defaults: new { controller = "auth" }
               );
        }
    }
}
