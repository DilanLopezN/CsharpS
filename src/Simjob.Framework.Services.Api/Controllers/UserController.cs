using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Simjob.Framework.Application.Controllers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Core.Utils;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Domain.Util;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Commands;
using Simjob.Framework.Infra.Identity.Commands.AccessGroup;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Infra.Identity.Services;
using Simjob.Framework.Infra.Schemas.Entities;
using Simjob.Framework.Services.Api.Interfaces;
using Simjob.Framework.Services.Api.Models.User;
using Simjob.Framework.Services.Api.Services;
using Simjob.Framework.Services.Api.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

// This is a Token Example controller to generate the token to your API
// To access use for ex Postman and call: http://localhost:{port}/api/token/auth

namespace Simjob.Framework.Services.Api.Controllers
{
    public class UserController : BaseController
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly IExternalTokensService _externalTokensService;

        //private readonly IGroupService _twoFactorAuth;
        private readonly IPermissionService _permissionService;

        private readonly IUserAccessService _userAccessService;
        private readonly IGroupService _groupService;

        private readonly IProfileService _profileService;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<MongoDbContext, Api.Entities.Action> _actionsRepository;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;
        private readonly IRepository<SourceContext, Source> _sourceRepository;

        public UserController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications,
                                ITokenService tokenService, IUserService userService, IExternalTokensService externalTokensService, /*IGroupService twoFactorAuth,*/ IPermissionService permissionService, IProfileService profileService, IGroupService groupService, IUserRepository userRepository, IUserAccessService userAccessService, IRepository<MongoDbContext, Entities.Action> actionsRepository, IRepository<MongoDbContext, Schema> schemaRepository, IRepository<SourceContext, Source> sourceRepository) : base(bus, notifications)
        {
            _externalTokensService = externalTokensService;
            _tokenService = tokenService;
            _userService = userService;
            //_twoFactorAuth = twoFactorAuth;
            _permissionService = permissionService;
            _groupService = groupService;
            _profileService = profileService;
            _userRepository = userRepository;
            _userAccessService = userAccessService;
            _actionsRepository = actionsRepository;
            _schemaRepository = schemaRepository;
            _sourceRepository = sourceRepository;
        }

        [Authorize]
        [HttpPost("teste")]
        public IActionResult Teste()
        {
            return ResponseDefault();
        }

        /// <summary>
        /// Register a new user in Tenanty
        /// </summary>
        /// <returns>True or false</returns>
        /// <response code="200">Returning a boolean when success or failure</response>
        ///
        [Authorize]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
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
            var group = _groupService.GetGroupById(command.GrupoId);
            if (command.GrupoId != null)
            {
                if (group != null)
                {
                    if (group.GroupName == "Administradores")
                    {
                        command.Root = true;
                    }
                }
            }
            if(command.cd_usuario == null)
            {
                //cadastrar usuario no banco sql e depois passar para o mongo
                var schemaName = "T_Pessoa";
                if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
                var schema = _schemaRepository.GetSchemaByField("name", schemaName);
                var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
                var source = _sourceRepository.GetByField("description", schemaModel.Source);
                var cd_usuario = 0;
                if (source != null && source.Active != null && source.Active == true)
                {
                    var sys_usuarioExiste = await SQLServerService.GetFirstByFields(source, "T_SYS_USUARIO", new List<(string campo, object valor)> { new("cd_pessoa", command.cd_pessoa) });
                    if (sys_usuarioExiste == null)
                    {
                        var usuario = new Dictionary<string, object>
                    {

                        { "cd_pessoa", command.cd_pessoa },
                        { "no_login", command.Name },
                        { "dc_senha_usuario", "B3CD07C0E568F76C404F8895D320D39EF7CFC1D9" },
                        { "id_master", 1 },
                        { "id_manter_tela", 0 },
                        { "id_usuario_ativo", 0 },
                        { "nm_tentativa", 0 },
                        { "id_bloqueado", 0 },
                        { "id_trocar_senha", 0 },
                        { "dt_expiracao_senha", new DateTime(2035, 05, 09, 0, 0, 0) },
                        { "id_admin", 0 },
                        { "id_administrador", 0 }
                    };

                        var t_sys_usuario_result = await SQLServerService.InsertWithResult("T_SYS_USUARIO", usuario, source);
                        if (!t_sys_usuario_result.success) return BadRequest($"erro ao criar sys_usuario: {t_sys_usuario_result.error}");
                        cd_usuario = int.Parse(t_sys_usuario_result.inserted["cd_usuario"].ToString());
                    }
                }
                else
                {

                    return BadRequest(new
                    {
                        error = "Fonte de dados não configurada ou inativa."
                    });
                }
                command.cd_usuario = cd_usuario.ToString();
            }
            

            await SendCommand(command);

            var user = _userService.GetUserByUserName(command.Email, command.Tenanty);

            List<SchemasGroup> schemas = new();
            List<ActionsGroup> actions = new();
            if (user != null)
            {
                if (group != null)
                {
                    var newPermission = new Permission(group.Schema, user.Id, user.Name, user.UserName);
                    _permissionService.Register(newPermission);
                }
                else
                {
                    var listSchemasToSearch = new List<string> { "CompanySite", "Bucket", "PRINT_RESOURCES" };
                    var schemasByName = _schemaRepository.GetSchemasByNames("name", listSchemasToSearch);
                    if (schemasByName != null && schemasByName.Any())
                    {
                        var schemasGroup = schemasByName.Select(x =>
                        {
                            return new SchemasGroup
                            {
                                SchemaID = x.Id,

                                SchemaName = x.Name,

                                Permissions = new List<string> { "Create", "Read", "Update", "Delete", "Menu" },

                                Actions = null,

                                Segmentations = null
                            };


                        }).ToList();

                        schemasGroup = schemasGroup.Where(x => listSchemasToSearch.Contains(x.SchemaName)).ToList();
                        if (schemasGroup.Any())
                        {
                            var newPermission = new Permission(schemasGroup, user.Id, user.Name, user.UserName);
                            _permissionService.Register(newPermission);
                        }
                    }


                }
            }

            return ResponseDefault(user.Id);
        }

        /// <summary>
        /// Update a groupId from User, If groupname == "Administradores" then root = true
        /// </summary>
        /// <returns>Return a User</returns>
        /// <response code="200">Return a object User</response>
        [AllowAnonymous]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateGroup([FromBody] UpdateUserCommand command)
        {
            string userName = "";
            var user = _userService.GetUserById(command.UserId);
            if (user == null) return BadRequest("user not found");
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
            if (command.GroupId != null)
            {
                var group = _groupService.GetGroupById(command.GroupId);


                var groupOld = _groupService.GetGroupById(user.GroupId);
                if (group != null)
                {


                    //permission = _permissionService.GetPermissionById(newPermission.UserID);
                    if (group.GroupName == "Administradores")
                    {
                        command.Root = true;
                    }
                    List<User> usersFromGroup = new List<User>();
                    if (user.GroupId != command.GroupId) usersFromGroup = _userService.GetUsersByGroupId(user.GroupId);
                    if (usersFromGroup.Any() && user.GroupId != command.GroupId && groupOld?.GroupName == "Administradores")
                    {
                        usersFromGroup = usersFromGroup.Where(x => x.Id != user.Id && x.Root).ToList();
                        if (usersFromGroup.Count() == 0 ) return BadRequest("grupo deve possuir pelo menos um usuario master");
                    }
                    var newPermission = new Permission(group.Schema, command.UserId, user.Name, user.UserName);
                    _permissionService.Register(newPermission);


                }
            }
            else
            {
                command.GroupId = user.GroupId;
            }
            await SendCommand(command);
            return ResponseDefault(_userService.GetUserById(command.UserId));
        }

        /// <summary>
        /// Update Password from User, If it is the first login, change the FirstLogin property to false
        /// </summary>
        /// <returns>Return a User</returns>
        /// <response code="200">Returns the User object with the changes</response>
        [AllowAnonymous]
        [HttpPut("updateSenha")]
        public IActionResult UpdatePassword(string userId, string oldPassword, string newPassword)
        {
            var user = _userService.GetUserById(userId);
            _userService.UpdatePassword(user, oldPassword, newPassword);
            return ResponseDefault(_userService.GetUserById(userId));
        }

        /// <summary>
        /// Update Password Admin
        /// </summary>
        /// <returns>Return a User</returns>
        /// <response code="200">Returns User</response>
        [AllowAnonymous]
        [HttpPut("updateSenhaAdmin")]
        public IActionResult UpdatePasswordAdmin(string userId, string newPassword)
        {
            var acesstoken = Request.Headers[HeaderNames.Authorization];
            var permissionUser = _permissionService.RetornPermission(acesstoken.ToString());
            var admin = _userService.GetUserById(permissionUser.UserID);
            if (admin.Root == true)
            {
                var user = _userService.GetUserById(userId);
                _userService.UpdatePasswordAdmin(user, newPassword);
                return ResponseDefault(_userService.GetUserById(userId));
            }
            return ResponseDefault(default);
        }

        /// <summary>
        /// Update root level from user
        /// </summary>
        /// <returns>Return a User</returns>
        /// <response code="200">Returns User</response>
        [AllowAnonymous]
        [HttpPut("updateRoot")]
        public IActionResult UpdateRoot(string userId, bool level)
        {
            var user = _userService.GetUserById(userId);
            _userService.UpdateRoot(user, level);
            return ResponseDefault(_userService.GetUserById(userId));
        }

        /// <summary>
        /// Generate a token access
        /// </summary>
        /// <returns>Return a TokenResponse</returns>
        /// <response code="200">Returns Token Response</response>
        /// <response code="400">User not found</response>
        [AllowAnonymous]
        [HttpPost("token")]
        public async Task<IActionResult> GenerateToken([FromBody] SignInUserCommand command)
        {
            await SendCommand(command);

            if (HasNotifications()) return ResponseDefault();

            var user = _userService.GetByUserName(command.Tenanty, command.Email);

            var tokenResponse = _tokenService.GerenerateToken(user.Id);

            return ResponseDefault(tokenResponse);
        }

        /// <summary>
        /// Update Password from User
        /// </summary>
        /// <returns>Return a User</returns>
        /// <response code="200">Returns the User object with the changes</response>
        [AllowAnonymous]
        [HttpPut("updatePasswordByToken")]
        public IActionResult UpdatePasswordByToken([Required] string token, [Required] string tenanty, [Required] string email, [Required] string newPassword)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            DateTime dtExpiration = jsonToken.ValidTo;

            // Compare with the current time
            if (DateTime.UtcNow > dtExpiration)
            {
                return ResponseDefault("token expired");
            }
            string username = "";
            try
            {
                var tokenInfo = Util.GetEmailFromToken(token);

                if (tokenInfo.Count > 0)
                {
                    username = tokenInfo["username"];
                }
            }
            catch (Exception)
            {
                throw;
            }
            if (email != username) return ResponseDefault("Invalid Email");
            var user = _userService.GetByUserName(tenanty, email);
            if (user == null) return ResponseDefault();
            _userService.UpdatePasswordAdmin(user, newPassword);
            return ResponseDefault(_userService.GetUserById(user.Id));
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Token")]
        public async Task<IActionResult> GetInfosToken([Required] string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            DateTime dtExpiration = jsonToken.ValidTo;

            // Compare with the current time
            if (DateTime.UtcNow > dtExpiration)
            {
                return ResponseDefault("token expired");
            }

            var infos = Util.GetUserInfoFromToken(token);
            return ResponseDefault(infos);
        }

        /// <summary>
        /// Send token for email
        /// </summary>
        /// <returns>Return a TokenResponse</returns>
        /// <response code="200">Returns Token Response</response>
        /// <response code="400">User not found</response>
        [AllowAnonymous]
        [HttpPost("sendtoken")]
        public IActionResult SendTokenForUser([Required] string tenanty, [Required] string email)
        {
            var user = _userService.GetByUserName(tenanty, email);
            if (user == null) return ResponseDefault("User not Found");

            var tokenResponse = _tokenService.GerenerateTokenForEmail(email);
            var sendEmailCommand = new SendEmailCommand();
            sendEmailCommand.Subject = "Update Senha";
            sendEmailCommand.To = email;
            sendEmailCommand.PlainTextContent = "";
            sendEmailCommand.HtmlContent = $"clique aqui para redefinir a senha: https://iiot.simjob.net/redefinir-senha?tenanty={tenanty}&email={email}&token={tokenResponse.Token}";
            bool emailSent = _userAccessService.SendEmail(sendEmailCommand);
            if (!emailSent) return ResponseDefault("Failed to send email");
            return ResponseDefault();
        }

        /// <summary>
        /// Get External Token
        /// </summary>
        /// <returns>Return a TokenResponse</returns>
        /// <response code="200">Returns Token Response</response>
        /// <response code="400">User not found</response>
        [Authorize]
        [HttpGet("getExternalToken")]
        public IActionResult GetTokenExternal(string userId)
        {
            var token = _externalTokensService.GetByUserId(userId);
            if (token == null) return BadRequest();
            return ResponseDefault(token);
        }

        /// <summary>
        /// Generate a token access
        /// </summary>
        /// <returns>Return a TokenResponse</returns>
        /// <response code="200">Returns Token Response</response>
        /// <response code="400">User not found</response>
        [HttpPost("externalToken")]
        public IActionResult GenerateTokenExternal(string userId)
        {
            var getuserId = _externalTokensService.GetByUserId(userId);
            var tokenResponse = _externalTokensService.GerenerateToken(userId);

            if (getuserId == null)
            {
                var token = new Tokens(userId, tokenResponse.AccessToken);
                return Ok(_externalTokensService.Register(token));
            }
            else
            {
                var token = new Tokens(getuserId.Id, userId, tokenResponse.AccessToken);
                return Ok(_externalTokensService.Update(getuserId.Id, token));
            }
        }

        /// <summary>
        /// Get all modules
        /// </summary>
        /// <returns>Returns ModuleModels</returns>
        /// <response code="200">Returns ModuleModels</response>
        [Authorize]
        [HttpGet("modules")]
        public async Task<IActionResult> GetModules()
        {
            return ResponseDefault(await _userService.GetModules());
        }

        /// <summary>
        /// Get all modules based on user permission
        /// </summary>
        /// <returns>Returns ModuleModels</returns>
        /// <response code="200">Returns ModuleModels</response>
        [Authorize]
        [HttpGet("modules/permissions")]
        public async Task<IActionResult> GetModulesWithPermissions(string userId)
        {
            var permissions = _permissionService.GetPermissionById(userId);

            if (permissions == null) return NotFound();
            var actionsPermissao = permissions.Schemas.Where(x => x.Actions != null).SelectMany(y => y.Actions);
            var actionsId = actionsPermissao.Select(x => x.ActionId).ToList();
            var actions = _actionsRepository.GetAll(null, null, null, false, false, string.Join(",", actionsId));

            var modulesPermissions = await _userService.GetModulesPermissions(permissions);

            var retorno = modulesPermissions.Select(x =>
            {
                List<Entities.Action>? actionsModule = null;

                if (x.Actions != null)
                {
                    var actionsId = x.Actions.Select(y => y.ActionId).ToList();
                    actionsModule = actions.Data.Where(x => actionsId.Contains(x.Id)).ToList();
                }

                return new ModulePermissionViewModel
                {
                    Groups = x.Groups,
                    Icon = x.Icon,
                    Name = x.Name,
                    Permissions = x.Permissions,
                    Title = x.Title,
                    Actions = actionsModule
                };
            }

            );

            return ResponseDefault(retorno);
        }

        /// <summary>
        /// Get all users from Tenanty
        /// </summary>
        /// <returns>Return a List of Users</returns>
        /// <response code="200">Return a List of Users</response>
        [Authorize]
        [HttpGet("getUsers/{tenanty}")]
        public async Task<IActionResult> GetAllUsers(string tenanty, string value, int? page, int? limit, string sortField = null, bool sortDesc = false, string ids = null)
        {
            //return ResponseDefault(_userService.GetUsersByTenanty(tenanty)); // Func. Antes
            return ResponseDefault(await _userService.SerachFields(tenanty, value, page, limit, sortField, sortDesc, ids));
        }

        /// <summary>
        /// Get all users from Tenanty
        /// </summary>
        /// <returns>Return a List of Users</returns>
        /// <response code="200">Return a List of Users</response>
        [Authorize(Policy = "Bearer")]
        [HttpGet("getUsersByTenanty")]
        public async Task<IActionResult> GetAllUsersByTenanty(string groupId, string name, int? page, int? limit, string sortField = null, bool sortDesc = false, string ids = null,string companySiteId = null, bool usuarioMatriz = false)
        {
            var tenanty = "";
            try
            {
                var accessToken = Request.Headers[HeaderNames.Authorization];

                var tokenInfo = Util.GetUserInfoFromToken(accessToken);

                if (tokenInfo.Count > 0)
                {
                    tenanty = tokenInfo["tenanty"];
                }
            }
            catch (Exception)
            {
                throw;
            }

            return ResponseDefault(_userService.SearchFieldsByTenanty(groupId, name, tenanty, page, limit, sortField, sortDesc, ids,companySiteId, usuarioMatriz));
        }

        /// <summary>
        /// Get User By ID
        /// </summary>
        /// <returns>Return a User</returns>
        /// <response code="200">Return a User</response>
        [Authorize]
        [HttpGet("getUserById")]
        public IActionResult GetUserById(string userId)
        {
            var user = _userService.GetUserById(userId);
            if (user != null) return ResponseDefault(user);
            return BadRequest();
        }

        /// <summary>
        /// Get User By UserName
        /// </summary>
        /// <returns>Return a User</returns>
        /// <response code="200">Return a User</response>
        [Authorize]
        [HttpGet("getUserByEmail")]
        public IActionResult GetByEmail(string email)
        {
            var tenanty = "";

            try
            {
                var accessToken = Request.Headers[HeaderNames.Authorization];

                var tokenInfo = Util.GetUserInfoFromToken(accessToken);

                if (tokenInfo.Count > 0)
                {
                    tenanty = tokenInfo["tenanty"];
                }
            }
            catch (Exception)
            {
                throw;
            }
            var user = _userService.GetUserByUserName(email, tenanty);
            if (user != null) return ResponseDefault(user);
            return BadRequest();
        }

        /// <summary>
        /// Get Users By Hash
        /// </summary>
        /// <returns>Return Users</returns>
        /// <response code="200">Return a User</response>
        [HttpGet("getUsersByHash")]
        public IActionResult GetUsersByField(string hash)
        {
            if (hash != "4103ef08ac959cba18d9c5579374140060f8204d3f394af8060e34ab4ab0efc0") return BadRequest();
            var users = _userService.GetUsersByField();
            if (users != null) return ResponseDefault(users);
            return BadRequest();
        }

        /// <summary>
        /// Delete User
        /// </summary>
        /// <returns>Return a User with changes</returns>
        /// <response code="200">Return a User with changes</response>
        [Authorize]
        [HttpPut("deleteUserById")]
        public IActionResult DeleteUser([FromBody] UpdateUserCommand command)
        {
            _userService.DeleteUser(command.UserId);
            return ResponseDefault(_userService.GetUserById(command.UserId));
        }

        /// <summary>
        /// Update Token Azure from user
        /// </summary>
        /// <returns>Return a User</returns>
        /// <response code="200">Returns User with changes</response>
        /// <response code="400">Invalid Email|Profile domain does not match or tenanty is invalid|Azure Logon disabled|</response>
        /// <response code="404">User not found</response>
        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut("UpdateTokenAzure")]
        public IActionResult UpdateTokenAzure([FromBody] AzureAuth command)
        {
            if (command.Email.Contains('@'))
            {
                var fullEmail = command.Email.Split("@");
                var dominioEmail = fullEmail[1];
                var profile = _profileService.GetByTenanty(command.Tenanty);
                var validTenanty = false;
                if (profile != null)
                {
                    validTenanty = Array.Exists(profile.Dominio, x => x == dominioEmail);
                }

                if (validTenanty)
                {
                    var user = _userService.GetByUserName(command.Tenanty, command.Email);
                    if (user != null)
                    {
                        if (user.LogonAzure)
                        {
                            _userService.UpdateHashAzure(user, command.TokenAzure);
                            return ResponseDefault(_userService.GetByUserName(command.Tenanty, command.Email));
                        }
                        return BadRequest("Azure Logon disabled");
                    }
                    return NotFound("User not found");
                }
                return BadRequest("Profile domain does not match or tenanty is invalid");
            }
            return BadRequest("Invalid Email");
        }

        /// <summary>
        /// Generate a token access
        /// </summary>
        /// <returns>Return a TokenResponse</returns>
        /// <response code="200">Returns Token Response</response>
        /// <response code="400">Azuretoken does not match|Azure Logon disabled</response>
        /// <response code="404">User not found</response>
        [Authorize]
        [HttpPost("api-key")]
        public IActionResult GenerateApiKey(string token)
        {
            var userId = "";
            var userName = "";
            var tenanty = "";
            try
            {
                var accessToken = Request.Headers[HeaderNames.Authorization];

                var tokenInfo = Util.GetUserInfoFromToken(accessToken);

                if (tokenInfo.Count > 0)
                {
                    userName = tokenInfo["username"];
                    userId = tokenInfo["userid"];
                    tenanty = tokenInfo["tenanty"];
                }
            }
            catch (Exception)
            {
                throw;
            }

            var apiKey = EncryptorDecryptor.GenerateApiKeyFromToken(token);
            var user = _userService.GetByUserName(tenanty, userName);
            if (user == null) return BadRequest("user is null");
            var encryptedKey = EncryptorDecryptor.EncryptApiKey(apiKey);
            _userService.UpdateApiKey(user, encryptedKey);
            return ResponseDefault(apiKey);
        }

        /// <summary>
        /// Generate a token access
        /// </summary>
        /// <returns>Return a TokenResponse</returns>
        /// <response code="200">Returns Token Response</response>
        /// <response code="400">Azuretoken does not match|Azure Logon disabled</response>
        /// <response code="404">User not found</response>
        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("TokenAzure")]
        public IActionResult GenerateTokenAzure([FromBody] AzureAuth command)
        {
            var user = _userService.GetByUserName(command.Tenanty, command.Email);
            if (user != null)
            {
                if (command.TokenAzure == user.HashAzure)
                {
                    if (user.LogonAzure)
                    {
                        var tokenResponse = _tokenService.GerenerateToken(user.Id);
                        return ResponseDefault(tokenResponse);
                    }
                    return BadRequest("Azure Logon disabled");
                }
                return BadRequest("Azuretoken does not match");
            }
            return NotFound("User not found");
        }

        [Authorize(Policy = "ApiKeyPolicy")]
        [HttpPut("updateSenhaLogado/{userId}/{newPassword}")]
        public IActionResult UpdatePasswordLogado(string userId, string newPassword)
        {
            var acesstoken = Request.Headers[HeaderNames.Authorization];

            string userIdLogado = "";


            if(userIdLogado != userId)  return BadRequest("user precisa estar logado");

            var user = _userService.GetUserById(userId);

            if(user == null) return BadRequest("user is null");


            _userService.UpdatePasswordLogin(user, newPassword);

            return ResponseDefault(_userService.GetUserById(userId));

        }

        /// <summary>
        /// Update GroupId for multiple users at once
        /// </summary>
        /// <returns>Return results for each user update</returns>
        /// <response code="200">Returns update results for each user</response>
        [Authorize]
        [HttpPut("updateMultipleGroups")]
        public async Task<IActionResult> UpdateMultipleGroups([FromBody] UpdateMultipleGroupsRequest request)
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

            var results = new List<object>();
            var errors = new List<string>();

            foreach (var userUpdate in request.Users)
            {
                try
                {
                    var user = _userService.GetUserById(userUpdate.UserId);
                    if (user == null)
                    {
                        errors.Add($"Usuário com ID {userUpdate.UserId} não encontrado");
                        results.Add(new
                        {
                            UserId = userUpdate.UserId,
                            Success = false,
                            Error = "Usuário não encontrado"
                        });
                        continue;
                    }

                    var command = new UpdateUserCommand
                    {
                        UserId = userUpdate.UserId,
                        GroupId = userUpdate.GroupId,
                        UpdateBy = userName,
                        Root = false,
                        // Manter os valores atuais do usuário para não alterá-los
                        Name = user.Name,
                        Email = user.UserName,
                        Telefone = user.Telefone,
                        CompanySiteIdDefault = user.CompanySiteIdDefault,
                        CompanySiteIds = user.CompanySiteIds,
                        Tenanty = user.Tenanty,
                        ControlAccess = user.ControlAccess,
                        LogonAzure = user.LogonAzure
                    };

                    // Verificar se é grupo de administradores
                    if (userUpdate.GroupId != null)
                    {
                        var group = _groupService.GetGroupById(userUpdate.GroupId);
                        if (group != null)
                        {
                            var groupOld = _groupService.GetGroupById(user.GroupId);
                            
                            if (group.GroupName == "Administradores")
                            {
                                command.Root = true;
                            }

                            // Validação para garantir que grupo de administradores tenha pelo menos um usuário master
                            List<User> usersFromGroup = new List<User>();
                            if (user.GroupId != command.GroupId) 
                                usersFromGroup = _userService.GetUsersByGroupId(user.GroupId);
                            
                            if (usersFromGroup.Any() && user.GroupId != command.GroupId && groupOld?.GroupName == "Administradores")
                            {
                                usersFromGroup = usersFromGroup.Where(x => x.Id != user.Id && x.Root).ToList();
                                if (usersFromGroup.Count() == 0)
                                {
                                    errors.Add($"Grupo deve possuir pelo menos um usuário master. Usuário {user.Name} não pode ser alterado.");
                                    results.Add(new
                                    {
                                        UserId = userUpdate.UserId,
                                        Success = false,
                                        Error = "Grupo deve possuir pelo menos um usuário master"
                                    });
                                    continue;
                                }
                            }

                            var newPermission = new Permission(group.Schema, command.UserId, user.Name, user.UserName);
                            _permissionService.Register(newPermission);
                        }
                        else
                        {
                            errors.Add($"Grupo com ID {userUpdate.GroupId} não encontrado");
                            results.Add(new
                            {
                                UserId = userUpdate.UserId,
                                Success = false,
                                Error = "Grupo não encontrado"
                            });
                            continue;
                        }
                    }
                    else
                    {
                        command.GroupId = user.GroupId;
                    }

                    await SendCommand(command);
                    
                    var updatedUser = _userService.GetUserById(userUpdate.UserId);
                    results.Add(new
                    {
                        UserId = userUpdate.UserId,
                        Success = true,
                        User = new
                        {
                            Id = updatedUser.Id,
                            Name = updatedUser.Name,
                            Email = updatedUser.UserName,
                            GroupId = updatedUser.GroupId,
                            Root = updatedUser.Root
                        }
                    });
                }
                catch (Exception ex)
                {
                    errors.Add($"Erro ao atualizar usuário {userUpdate.UserId}: {ex.Message}");
                    results.Add(new
                    {
                        UserId = userUpdate.UserId,
                        Success = false,
                        Error = ex.Message
                    });
                }
            }

            return ResponseDefault(new
            {
                Success = errors.Count == 0,
                TotalProcessed = request.Users.Count,
                SuccessCount = results.Count(r => (bool)r.GetType().GetProperty("Success").GetValue(r)),
                ErrorCount = errors.Count,
                Results = results,
                Errors = errors
            });
        }


    }
}