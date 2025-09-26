using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Simjob.Framework.Domain.Models;
using Simjob.Framework.Domain.Util;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Services.Api.Configurations;
using Simjob.Framework.Services.Api.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;

namespace Simjob.Framework.Services.Api.Services
{
    public class TokenService : ITokenService
    {
        private readonly TokenConfigurations _tokenConfigurations;
        private readonly SigningConfigurations _signingConfigurations;
        private readonly IUserService _userService;
        private readonly IUserAdminService _userAdminService;
        private readonly IHttpContextAccessor _accessor;
        private readonly IServiceProvider _serviceProvider;
        public TokenService(TokenConfigurations tokenConfigurations, SigningConfigurations signingConfigurations, IUserService userService, IHttpContextAccessor accessor, IServiceProvider serviceProvider, IUserAdminService userAdminService)
        {
            _tokenConfigurations = tokenConfigurations;
            _signingConfigurations = signingConfigurations;
            _userService = userService;
            _accessor = accessor;
            _serviceProvider = serviceProvider;
            _userAdminService = userAdminService;
        }

        public TokenResponse GerenerateToken(string userId)
        {
            var user = _userService.GetUserById(userId);
            var claims = user.Claims;

            ClaimsIdentity identity = new ClaimsIdentity(

              new GenericIdentity(user.UserName, "Login"),
              new[] { new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                        new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                        new Claim("userid", user.Id),
                        new Claim("tenanty", user.Tenanty),
                        new Claim("cd_pessoa", user.Cd_pessoa??""),
                        new Claim("cd_usuario", user.Cd_usuario??""),
                        new Claim("companySiteIdDefault",user.CompanySiteIdDefault ?? ""),
                        //new Claim("companySiteIds",user.CompanySiteIds != null ? string.Join(",", user.CompanySiteIds) : ""),
                        new Claim("root",user.Root.ToString()),

              }
          );

            identity.Claims.ToList().AddRange(claims);

            DateTime dtCreation = DateTime.Now;
            DateTime dtExpiration;
            if (_tokenConfigurations.Seconds != 0)
            {
                dtExpiration = dtCreation +
                TimeSpan.FromSeconds(_tokenConfigurations.Seconds);
            }
            else
            {
                dtExpiration = dtCreation +
                TimeSpan.FromSeconds(86400);

            }

            var handler = new JwtSecurityTokenHandler();

            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _tokenConfigurations.Issuer,
                Audience = _tokenConfigurations.Audience,
                SigningCredentials = _signingConfigurations.SigningCredentials,
                Subject = identity,
                NotBefore = dtCreation,
                Expires = dtExpiration
            });
            var token = handler.WriteToken(securityToken);
            //var version = Assembly.GetExecutingAssembly().GetName().Version;            
            var version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            return new TokenResponse(
                user.Tenanty,
                user.Id,
                user.UserName,
                user.Name,
                user.Claims,
                dtCreation,
                dtExpiration,
                token,
                user.A2f,
                user.GroupId,
                user.Telefone,
                user.Root,
                user.ControlAccess,
                version.ToString(),
                user.FirstLogin,
                user.RevendaId,
                user.NivelId,
                user.ApiKey != null ? EncryptorDecryptor.DecryptApiKey(user.ApiKey) : null,
                user.CompanySiteIdDefault ?? null,
                user.CompanySiteIds != null ? string.Join(",", user.CompanySiteIds) : null,
                user.Cd_pessoa,
                user.Cd_usuario,
                user.UsuarioMatriz);

        }

        public TokenResponse GerenerateTokenAdmin(string userId)
        {
            var user = _userAdminService.GetUserAdminById(userId);
            var claims = user.Claims;

            ClaimsIdentity identity = new ClaimsIdentity(

              new GenericIdentity(user.UserName, "Login"),
              new[] { new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                        new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                        new Claim("userid", user.Id),
                        new Claim("tenanty", user.Tenanty),
                        new Claim("companySiteIdDefault",user.CompanySiteIdDefault ?? ""),
                        new Claim("companySiteIds",user.CompanySiteIds != null ? string.Join(",", user.CompanySiteIds) : "")
              }
          );

            identity.Claims.ToList().AddRange(claims);

            DateTime dtCreation = DateTime.Now;
            DateTime dtExpiration;
            if (_tokenConfigurations.Seconds != 0)
            {
                dtExpiration = dtCreation +
                TimeSpan.FromSeconds(_tokenConfigurations.Seconds);
            }
            else
            {
                dtExpiration = dtCreation +
                TimeSpan.FromSeconds(86400);

            }

            var handler = new JwtSecurityTokenHandler();

            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _tokenConfigurations.Issuer,
                Audience = _tokenConfigurations.Audience,
                SigningCredentials = _signingConfigurations.SigningCredentials,
                Subject = identity,
                NotBefore = dtCreation,
                Expires = dtExpiration
            });
            var token = handler.WriteToken(securityToken);
            //var version = Assembly.GetExecutingAssembly().GetName().Version;            
            var version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            return new TokenResponse(
                user.Tenanty,
                user.Id,
                user.UserName,
                user.Name,
                user.Claims,
                dtCreation,
                dtExpiration,
                token,
                user.A2f,
                user.GroupId,
                user.Telefone,
                user.Root,
                false,
                version.ToString(),
                user.FirstLogin,
                user.RevendaId,
                user.NivelId,
                user.ApiKey != null ? EncryptorDecryptor.DecryptApiKey(user.ApiKey) : null,
                user.CompanySiteIdDefault ?? null,
                user.CompanySiteIds != null ? string.Join(",", user.CompanySiteIds) : null,
                "",
                "",
                true);

        }

        public dynamic GerenerateTokenForEmail(string email)
        {
            ClaimsIdentity identity = new ClaimsIdentity(

           new GenericIdentity(email, "Login"),
           new[] { new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                        new Claim(JwtRegisteredClaimNames.UniqueName, email)

           });


            DateTime dtCreation = DateTime.Now;
            DateTime dtExpiration;


            dtExpiration = dtCreation +
            TimeSpan.FromSeconds(300);

            var handler = new JwtSecurityTokenHandler();

            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _tokenConfigurations.Issuer,
                Audience = _tokenConfigurations.Audience,
                SigningCredentials = _signingConfigurations.SigningCredentials,
                Subject = identity,
                NotBefore = dtCreation,
                Expires = dtExpiration
            });
            var token = handler.WriteToken(securityToken);
            //var version = Assembly.GetExecutingAssembly().GetName().Version;            
            var version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            return new
            {
                DtCreation = dtCreation,
                DtExpiration = dtExpiration,
                Token = token
            };
        }

        public bool ValidateToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _tokenConfigurations.Issuer,
                    ValidAudience = _tokenConfigurations.Audience,
                    IssuerSigningKey = _signingConfigurations.SigningCredentials.Key,
                }, out SecurityToken validatedToken);

                // If no exceptions are thrown, the token is valid
                return true;
            }
            catch (SecurityTokenException)
            {
                // Token validation failed due to security token issues
                return false;
            }
            catch (Exception)
            {
                // Other exceptions occurred (e.g., malformed token)
                return false;
            }

        }

    }
}
