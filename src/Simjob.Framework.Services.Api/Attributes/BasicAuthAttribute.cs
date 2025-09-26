using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Simjob.Framework.Infra.Identity.Interfaces;
using System;
using System.Security.Claims;
using System.Text;

namespace Simjob.Framework.Services.Api.Attributes
{
    public class BasicAuthAttribute : Attribute, IAuthorizationFilter
    {
        private readonly IUserService _userService;


        public BasicAuthAttribute(IUserService userService)
        {
            _userService = userService;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authHeader = context.HttpContext.Request.Headers["Authorization"].ToString();

            if (authHeader != null && authHeader.StartsWith("Basic "))
            {
                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));

                var tenanty = decodedUsernamePassword.Split(':')[0].Split(';')[0];
                var username = decodedUsernamePassword.Split(':')[0].Split(';')[1];
                var password = decodedUsernamePassword.Split(':')[1];


                var user = _userService.GetByUserName(tenanty,username);
                bool login = _userService.Login(tenanty, username, password);
                if (user != null) 
                {
                    if (username == user.UserName && tenanty == user.Tenanty && login)
                    {
                        var claims = new[] { new Claim(ClaimTypes.Name, username) };
                        var identity = new ClaimsIdentity(claims, "Basic");
                        context.HttpContext.User = new ClaimsPrincipal(identity);
                        return;
                    }
                }
               
            }

            context.Result = new UnauthorizedResult();
        }
    }
}

