using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simjob.Framework.Application.Controllers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Util;
using Simjob.Framework.Infra.Identity.Commands;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Services.Api.Interfaces;
using System;
using System.Threading.Tasks;

namespace Simjob.Framework.Services.Api.Controllers
{
    [Authorize]
    public class TwoFactorAuthController : BaseController
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly ITwoFactorAuthService _twoFactorAuth;

        public TwoFactorAuthController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications,
                                ITokenService tokenService, IUserService userService, ITwoFactorAuthService twoFactorAuth) : base(bus, notifications)
        {
            _tokenService = tokenService;
            _userService = userService;
            _twoFactorAuth = twoFactorAuth;
        }

        /// <summary>
        /// Generate a random code for use in A2F
        /// </summary>
        /// <response code="200">Return a a2f code</response>
        [HttpPost("2fa")]
        public async Task<IActionResult> GenerateA2F([FromBody] TwoFactorAuthCommand command)
        {
            await SendCommand(command);

            if (HasNotifications()) return ResponseDefault();

            UserTwoFactorAuth twoFactorAuth = new UserTwoFactorAuth
            (
                command.UserId,
                "",
                command.Email,
                command.PublicIP,
                command.Hash, 
                false
            );

            try
            {
                var user = _userService.GetUserById(command.UserId);
                var userName = _userService.GetByUserName(command.Tenanty, command.Email);
                if (user != null && userName != null)
                {
                    var a2fsucess = await _twoFactorAuth.SendCodeVerification(twoFactorAuth);
                    if (a2fsucess == null) return ResponseDefault("error sending email");
                    return ResponseDefault(a2fsucess);
                }
                else
                {
                    return ResponseDefault("invalid user/email");
                }
            }
            catch (Exception ex)
            {
                Logs.AddLog("[TwoFactorAuthController] - " + ex.Message);
                return BadRequest();
            }
        }

        /// <summary>
        /// Check if a2f code is valid
        /// </summary>
        /// <response code="200">Return authenticated user</response>
        [HttpPost("validation2fa")]
        public async Task<IActionResult> validation2fa([FromBody] TwoFactorAuthCommand command)
        {
            await SendCommand(command);
            if (HasNotifications()) return ResponseDefault();
            var authenticated = _twoFactorAuth.CodeIsValid(command.UserId, command.Code, command.Hash, command.PublicIP);
            if (authenticated == null) return ResponseDefault(authenticated);
            return ResponseDefault(authenticated);
        }

        /// <summary>
        /// Enable or disabled Two Factor Authentication Option
        /// </summary>
        /// <response code="200">Return Enabled or Disabled</response>
        [HttpPost("enable_disable_2fa")]
        public async Task<IActionResult> EnableOrDisable2FA([FromBody] TwoFactorAuthCommand command)
        {
            await SendCommand(command);
            if (HasNotifications()) return ResponseDefault();            
            var user = _userService.GetUserById(command.UserId);
            if (user != null)
            {
                var option = _userService.EnableOrDisableA2F(user, command.a2f);
                if (option == 1)
                    return ResponseDefault("enabled");
                else
                    return ResponseDefault("disabled");
            } else
            {
                return ResponseDefault("invalid userID");
            }
        }
    }
}
