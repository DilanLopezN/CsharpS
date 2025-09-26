using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Simjob.Framework.Infra.Identity.Services;
using Simjob.Framework.Services.Api.Configurations;
using System;

namespace Simjob.Framework.Services.Api.Extensions
{
    public static class AuthenticationExtensions
    {
        public static void AddAuthenticationConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var signingConfigurations = new SigningConfigurations();
            services.AddSingleton(signingConfigurations);

            var tokenConfigurations = new TokenConfigurations();
            new ConfigureFromConfigurationOptions<TokenConfigurations>(
                configuration.GetSection("TokenConfigurations"))
                    .Configure(tokenConfigurations);
            services.AddSingleton(tokenConfigurations);

            var externaltokenConfigurations = new ExternalTokensConfigurations();
            new ConfigureFromConfigurationOptions<ExternalTokensConfigurations>(
                configuration.GetSection("ExternalTokensConfigurations"))
                    .Configure(externaltokenConfigurations);
            services.AddSingleton(externaltokenConfigurations);

            services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(bearerOptions =>
            {
                var paramsValidation = bearerOptions.TokenValidationParameters;
                paramsValidation.IssuerSigningKey = signingConfigurations.Key;
                paramsValidation.ValidAudience = tokenConfigurations.Audience;
                paramsValidation.ValidIssuer = tokenConfigurations.Issuer;
                paramsValidation.ValidateIssuerSigningKey = true;
                paramsValidation.ValidateLifetime = true;
                //  paramsValidation.ClockSkew = TimeSpan.Zero;
                paramsValidation.ClockSkew = TimeSpan.FromSeconds(tokenConfigurations.Seconds);

                paramsValidation.ValidAudience = externaltokenConfigurations.Audience;
                paramsValidation.ValidIssuer = externaltokenConfigurations.Issuer;
                paramsValidation.ClockSkew = TimeSpan.FromSeconds(externaltokenConfigurations.Seconds);
            });
        }
    }
}
