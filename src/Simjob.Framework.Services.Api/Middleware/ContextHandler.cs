using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simjob.Framework.Services.Api.Middleware
{
    public static class ContextHandler
    {
        public static async Task JSONResponse(HttpContext context, object jsonObject, int statusCode = StatusCodes.Status200OK)
        {
            string jsonError = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObject);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(jsonError);
        }

        public static async Task HandleIncomingRequests(HttpContext context, Func<Task> next)
        {
            #pragma warning disable CS8632 // A anotação para tipos de referência anuláveis deve ser usada apenas em código em um contexto de anotações '#nullable'.
            string? controller = context.Request.RouteValues["controller"]?.ToString();
            string? action = context.Request.RouteValues["actionName"]?.ToString();
            #pragma warning restore CS8632
            await next.Invoke();
        }
    }
}
