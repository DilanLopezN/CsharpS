using System.Web.Http;
using System.Web.Routing;
using MvcTurbine.Routing;

namespace FundacaoFisk.SGF.Web.Services.Biblioteca.Registration
{
    public class BibliotecaRouteRegistration :IRouteRegistrator
    {
        public void Register(RouteCollection routes)
        {
            routes.MapHttpRoute(
               name: "BibliotecaApi",
               routeTemplate: "api/biblioteca/{action}",
               defaults: new { controller = "biblioteca" });
        }
    }
}
