using System.Web.Http;
using System.Web.Routing;
using MvcTurbine.Routing;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Registration
{
    public class SecretariaRouteRegistration : IRouteRegistrator
    {
        public void Register(RouteCollection routes) {
            routes.MapHttpRoute(
               name: "SecretariaApi",
               routeTemplate: "api/secretaria/{action}",
               defaults: new { controller = "secretaria" }
               );

            routes.MapHttpRoute(
               name: "AlunoApi",
               routeTemplate: "api/aluno/{action}",
               defaults: new { controller = "aluno" }
               );
        }
    }
}
