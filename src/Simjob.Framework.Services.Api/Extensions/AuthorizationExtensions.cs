using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Simjob.Framework.Services.Api.Middleware;

namespace WorkTiming.Services.Api.Configurations
{
    public static class AuthorizationExtensions
    {
        public static void AddAuthorizationConfiguration(this IServiceCollection services)
        {
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build());

                auth.AddPolicy("ApiKeyPolicy", policy =>
                {
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                    policy.Requirements.Add(new ApiKeyRequirement());
                });

                auth.AddPolicy("OAuth2", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes("OAuth2")
                    .RequireAuthenticatedUser().Build());

            });
        }
    }
}
