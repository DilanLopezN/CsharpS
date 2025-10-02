using System.Web.Http;
using System.Web.Routing;
using MvcTurbine.Routing;

namespace FundacaoFisk.SGF.Web.Services.EmailMarketing.Registration
{
    public class EmailMarketingRouteRegistration :IRouteRegistrator
    {
        public void Register(RouteCollection routes)
        {
            routes.MapHttpRoute(
               name: "EmailMarketingApi",
               routeTemplate: "api/emailMarketing/{action}",
               defaults: new { controller = "emailMarketing" });
        }
    }
}
