using Microsoft.AspNetCore.Mvc;

namespace Simjob.Framework.Services.Api.Middleware
{
    public class ApiKeyAttribute : ServiceFilterAttribute
    {
        public ApiKeyAttribute() : base(typeof(ApiKeyAuthFilter))
        {
        }
    }
}
