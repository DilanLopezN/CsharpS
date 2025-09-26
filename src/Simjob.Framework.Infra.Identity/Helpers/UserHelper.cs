using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using Simjob.Framework.Domain.Interfaces.Users;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;

namespace Simjob.Framework.Infra.Identity.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly IHttpContextAccessor _accessor;
        public UserHelper(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public List<Claim> GetClaims()
        {

            var httpContext = _accessor?.HttpContext;
            if (!httpContext.User.Claims.Any())
            {
                ClaimsPrincipal claimsPrincipal = (ClaimsPrincipal)(_accessor?.HttpContext.Items["currentClaims"]);
                if (claimsPrincipal != null)
                {
                    return claimsPrincipal.Claims?.ToList();
                }
            }
            return _accessor.HttpContext?.User?.Claims?.ToList();
        }

        [ExcludeFromCodeCoverage]
        public string GetId()
        {
            var httpContext = _accessor?.HttpContext;
            if (!httpContext.User.Claims.Any())
            {
                ClaimsPrincipal claimsPrincipal = (ClaimsPrincipal)(_accessor?.HttpContext.Items["currentClaims"]);
                if (claimsPrincipal != null)
                {
                    return claimsPrincipal.Claims?.FirstOrDefault(c => c.Type == "userid")?.Value;

                }
            }
            return _accessor.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == "userid")?.Value;
        }

        public string GetTenanty()
        {
            var httpContext = _accessor?.HttpContext;
            if (!httpContext.User.Claims.Any())
            {
                ClaimsPrincipal claimsPrincipal = (ClaimsPrincipal)(_accessor?.HttpContext.Items["currentClaims"]);
                if (claimsPrincipal != null)
                {
                    return claimsPrincipal.Claims?.FirstOrDefault(c => c.Type == "tenanty")?.Value;
                }

            }
            return _accessor.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == "tenanty")?.Value;
        }

        public string GetUserName()
        {
            var httpContext = _accessor?.HttpContext;
            if (!httpContext.User.Claims.Any())
            {
                ClaimsPrincipal claimsPrincipal = (ClaimsPrincipal)(_accessor?.HttpContext.Items["currentClaims"]);
                if (claimsPrincipal != null)
                {
                    return claimsPrincipal.Claims?.FirstOrDefault(c => c.Type == "unique_name")?.Value;
                }
            }

            //return _accessor.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == "unique_name")?.Value;
            return _accessor.HttpContext.User?.Identity?.Name;
        }

        [ExcludeFromCodeCoverage]
        public bool IsAuthenticated()
        {
            var httpContext = _accessor?.HttpContext;
            if (!httpContext.User.Claims.Any())
            {
                ClaimsPrincipal claimsPrincipal = (ClaimsPrincipal)(_accessor?.HttpContext.Items["currentClaims"]);
                return claimsPrincipal.Identity.IsAuthenticated;
            }
            return _accessor.HttpContext.User.Identity.IsAuthenticated;
        }
    }
}
