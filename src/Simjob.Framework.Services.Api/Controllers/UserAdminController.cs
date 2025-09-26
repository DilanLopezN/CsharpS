using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Simjob.Framework.Application.Controllers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Core.Utils;
using Simjob.Framework.Infra.Identity.Commands;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Infra.Identity.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Simjob.Framework.Services.Api.Interfaces;

namespace Simjob.Framework.Services.Api.Controllers
{
    public class UserAdminController : BaseController
    {
        private readonly IUserAdminService _userAdminService;
        private readonly ITokenService _tokenService;
        public UserAdminController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, IUserAdminService userAdminService,ITokenService tokenService) : base(bus, notifications)
        {
            _userAdminService = userAdminService;
            _tokenService = tokenService;
        }



        /// <summary>
        /// Register a new userAdmin in Tenanty
        /// </summary>
        /// <returns>True or false</returns>
        /// <response code="200">Returning a boolean when success or failure</response>
        [AllowAnonymous]
        [HttpPost("register")]
        public  IActionResult Register([FromBody] RegisterUserCommand command)
        {
            string userName = "";
            try
            {
                var accessToken = Request.Headers[HeaderNames.Authorization];
                var tokenInfo = Util.GetUserInfoFromToken(accessToken);

                if (tokenInfo.Count > 0)
                {
                    userName = tokenInfo["username"];
                }
            }
            catch (Exception)
            {
                throw;
            }

            command.CreateBy = userName;
            command.Root = false;
            //var group = _groupService.GetGroupById(command.GrupoId);
            //if (command.GrupoId != null)
            //{
            //    if (group != null)
            //    {
            //        if (group.GroupName == "Administradores")
            //        {
            //            command.Root = true;
            //        }
            //    }
            //}
            var entity = new UserAdmin()
            {
                UserName = command.Email,
                Tenanty = command.Tenanty,
                Name = command.Name,
                Telefone = command.Telefone,
                CompanySiteIdDefault = command.CompanySiteIdDefault,
                CompanySiteIds = command.CompanySiteIds,
                GroupId = command.GrupoId,
                Root = command.Root,
                FirstLogin = true,
                //NivelId = command.NivelId,
                //RevendaId = command.RevendaId,
                CreateBy = command.CreateBy
            };

            _userAdminService.Register(entity, command.Password);
            

            var user = _userAdminService.GetByUserName(command.Tenanty, command.Email);

            List<SchemasGroup> schemas = new();
            List<ActionsGroup> actions = new();
            //if (user != null)
            //{
            //    if (group != null)
            //    {
            //        var newPermission = new Permission(group.Schema, user.Id, user.Name, user.UserName);
            //        _permissionService.Register(newPermission);
            //    }
            //}

            return ResponseDefault(user.Id);
        }


        /// <summary>
        /// Update userAdmin
        /// </summary>
        /// <returns>Return a User</returns>
        /// <response code="200">Return a object User</response>
        [AllowAnonymous]
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateUserCommand command)
        {
            string userName = "";
            try
            {
                var accessToken = Request.Headers[HeaderNames.Authorization];
                var tokenInfo = Util.GetUserInfoFromToken(accessToken);

                if (tokenInfo.Count > 0)
                {
                    userName = tokenInfo["username"];
                }
            }
            catch (Exception)
            {
                throw;
            }

            command.UpdateBy = userName;
            command.Root = false;
            //Permission permission = null;
            //if (command.GroupId != null)
            //{
            //    var group = _groupService.GetGroupById(command.GroupId);

            //    if (group != null)
            //    {
            //        var user = _userService.GetUserById(command.UserId);

            //        var newPermission = new Permission(group.Schema, command.UserId, user.Name, user.UserName);
            //        _permissionService.Register(newPermission);

            //        //permission = _permissionService.GetPermissionById(newPermission.UserID);
            //        if (group.GroupName == "Administradores")
            //        {
            //            command.Root = true;
            //        }
            //    }
            //}
            await SendCommand(command);
            return ResponseDefault(_userAdminService.GetUserAdminById(command.UserId));
        }



        /// <summary>
        /// Get all usersAdmin from Tenanty
        /// </summary>
        /// <returns>Return a List of Users</returns>
        /// <response code="200">Return a List of Users</response>
        [Authorize]
        [HttpGet("getUsersAdmin/{tenanty}")]
        public async Task<IActionResult> GetAllUsers(string tenanty, string value, int? page, int? limit, string sortField = null, bool sortDesc = false, string ids = null)
        {
            //return ResponseDefault(_userService.GetUsersByTenanty(tenanty)); // Func. Antes
            return ResponseDefault(await _userAdminService.SearchFields(tenanty, value, page, limit, sortField, sortDesc, ids));
        }


        /// <summary>
        /// Get UserAdmin By ID
        /// </summary>
        /// <returns>Return a User</returns>
        /// <response code="200">Return a User</response>
        [Authorize]
        [HttpGet("getUserAdminById")]
        public IActionResult GetUserById(string userId)
        {
            var user = _userAdminService.GetUserAdminById(userId);
            if (user != null) return ResponseDefault(user);
            return BadRequest();
        }


        /// <summary>
        /// Generate a token access for admin user
        /// </summary>
        /// <returns>Return a TokenResponse</returns>
        /// <response code="200">Returns Token Response</response>
        /// <response code="400">User not found</response>
        [AllowAnonymous]
        [HttpPost("tokenAdmin")]
        public async Task<IActionResult> GenerateTokenAdmin([FromBody] SignInUserCommand command)
        {

            var validateLogin = _userAdminService.LoginAdmin(command.Tenanty, command.Email, command.Password);
            if(!validateLogin) return BadRequest("credenciais inválidas");


            if (HasNotifications()) return ResponseDefault();

            var user = _userAdminService.GetByUserName(command.Tenanty, command.Email);

            var tokenResponse = _tokenService.GerenerateTokenAdmin(user.Id);

            return ResponseDefault(tokenResponse);
        }
    }
}
