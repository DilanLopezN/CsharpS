using System.Web.Http;
using System.Web.Routing;
using MvcTurbine.Routing;

namespace FundacaoFisk.SGF.Web.Services.CNAB.Registration
{
    class CNABRouteRegistration :IRouteRegistrator
    {
        public void Register(RouteCollection routes)
        {
            routes.MapHttpRoute(
               name: "CNABApi",
               routeTemplate: "api/cnab/{action}",
               defaults: new { controller = "cnab"});
        }
    }
}
