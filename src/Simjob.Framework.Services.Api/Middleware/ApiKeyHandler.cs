using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Simjob.Framework.Domain.Core.Utils;
using Simjob.Framework.Domain.Models.PublishModels;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Services.Api.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Simjob.Framework.Services.Api.Middleware
{
    public class ApiKeyHandler : AuthorizationHandler<IAuthorizationRequirement>
    {
        private IHttpContextAccessor _httpContextAccessor;
        private readonly IApiKeyValidation _apiKeyValidation;
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;

        public ApiKeyHandler(
            IHttpContextAccessor httpContextAccessor,
            IApiKeyValidation apiKeyValidation,
            ITokenService tokenService,
            IUserService userService)
        {
            _httpContextAccessor = httpContextAccessor;
            _apiKeyValidation = apiKeyValidation;
            _tokenService = tokenService;
            _userService = userService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, IAuthorizationRequirement requirement)
        {
            var httpContext = _httpContextAccessor?.HttpContext;

            if (httpContext == null)
            {
                context.Fail();
                return;
            }

            string authorizationHeader = httpContext.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                if (authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    var token = authorizationHeader.Substring("Bearer ".Length).Trim();
                    if (_tokenService.ValidateToken(token))
                    {
                        var handler = new JwtSecurityTokenHandler();
                        var tokenPermission = handler.ReadJwtToken(token);
                        var claims = tokenPermission.Claims.ToList(); // Convert claims to a list

                        var identity = new ClaimsIdentity(claims, "custom", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                        var principal = new ClaimsPrincipal(identity);

                        httpContext.Items["currentClaims"] = principal;
                        context.Succeed(requirement);
                        return;
                    }
                }
                
            }

            if (httpContext.Request.Headers.ContainsKey("X-API-Key") && requirement is ApiKeyRequirement)
            {
                string apiKeyHeader = httpContext.Request.Headers["X-API-Key"].ToString();
                if (_apiKeyValidation.IsValidApiKey(apiKeyHeader))
                {
                    var apiKey = apiKeyHeader.Substring("X-API-Key ".Length).Trim();
                    var user = _userService.GetByApiKey(apiKey);
                    if (user != null)
                    {
                        var responseToken = _tokenService.GerenerateToken(user.Id);
                        if (responseToken != null)
                        {
                            var updatedAuthorizationHeader = $"Bearer {responseToken.AccessToken}";
                            httpContext.Request.Headers[HeaderNames.Authorization] = updatedAuthorizationHeader;
                            var handler = new JwtSecurityTokenHandler();
                            var tokenPermission = handler.ReadJwtToken(responseToken.AccessToken);
                            var claims = tokenPermission.Claims.ToList(); // Convert claims to a list

                            var identity = new ClaimsIdentity(claims, "custom", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                            var principal = new ClaimsPrincipal(identity);

                            httpContext.Items["currentClaims"] = principal;
                            context.Succeed(requirement);
                            return;
                        }
                    }
                }

                context.Fail();
                await Task.CompletedTask;
            }
        }
    }
}
