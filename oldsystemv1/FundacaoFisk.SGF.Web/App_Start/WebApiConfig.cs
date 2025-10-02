using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace FundacaoFisk.SGF.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Servi�os e configura��o da API da Web

            // Rotas da API da Web
            //config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling .Ignore;
            config.Filters.Add(new AuthorizeAttribute());
        }
    }
}
