using System.Web.Http;
using System.Web.Routing;
using MvcTurbine.Routing;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Registration
{
    class CoordenacaoRouteRegistration :IRouteRegistrator
    {
        public void Register(RouteCollection routes)
        {
            routes.MapHttpRoute(
               name: "CoordenacaoApi",
               routeTemplate: "api/coordenacao/{action}",
               defaults: new { controller = "coordenacao" });
            routes.MapHttpRoute(
               name: "CursoApi",
               routeTemplate: "api/curso/{action}",
               defaults: new { controller = "curso" }
               );
            routes.MapHttpRoute(
               name: "TurmaApi",
               routeTemplate: "api/turma/{action}",
               defaults: new { controller = "turma" });
            routes.MapHttpRoute(
               name: "ProfessorApi",
               routeTemplate: "api/professor/{action}",
               defaults: new { controller = "professor" });
            
        }
    }
}
