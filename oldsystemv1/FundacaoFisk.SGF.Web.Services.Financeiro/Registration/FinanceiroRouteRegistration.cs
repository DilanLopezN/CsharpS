using System.Web.Http;
using System.Web.Routing;
using MvcTurbine.Routing;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Registration
{
    public class FinanceiroRouteRegistration : IRouteRegistrator
    {
        public void Register(RouteCollection routes) {
            routes.MapHttpRoute(
               name: "FinanceiroApi",
               routeTemplate: "api/financeiro/{action}",
               defaults: new { controller = "financeiro" }
               );
            routes.MapHttpRoute(
               name: "FiscalApi",
               routeTemplate: "api/fiscal/{action}",
               defaults: new { controller = "fiscal" }
               );
        }
    }
}
