using System.Web.Http;
using System.Web.Routing;
using MvcTurbine.Routing;

namespace FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Registration
{
    class EscolaRouteRegistration :IRouteRegistrator
    {
        public void Register(RouteCollection routes)
        {
            routes.MapHttpRoute(
               name: "EscolaApi",
               routeTemplate: "api/escola/{action}/{id}",
               defaults: new { controller = "escola",id = RouteParameter.Optional });
        }
    }
}
