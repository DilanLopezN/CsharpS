using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simjob.Framework.Application.Controllers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Commands;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Infra.Schemas.Entities;
using Simjob.Framework.Services.Api.Interfaces;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace Simjob.Framework.Services.Api.Controllers
{
    [Authorize]
    public class AccessGroupController : BaseController
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly ISegmentationService _segmentationService;
        private readonly IGroupService _groupService;
        private readonly IPermissionService _permissionService;
        private readonly IRepository<MongoDbContext, Group> _repository;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;

        public AccessGroupController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications,
                                ITokenService tokenService, IRepository<MongoDbContext, Group> repository, IUserService userService, ISegmentationService segmentationService, IGroupService groupService, IPermissionService permissionService, IRepository<MongoDbContext, Schema> schemaRepository) : base(bus, notifications)
        {
            _tokenService = tokenService;
            _userService = userService;
            _groupService = groupService;
            _permissionService = permissionService;
            _repository = repository;
            _segmentationService = segmentationService;
            _schemaRepository = schemaRepository;
        }

        /// <summary>
        /// Register new Group
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [AllowAnonymous]
        [HttpPost("insertGroup")]
        public IActionResult InsertGroup([FromBody] RegisterGroupCommand command)
        {
            var newGroup = new Group(command.GroupName, null,command.Cd_empresa);
            _groupService.Register(newGroup, null);
            return ResponseDefault(_groupService.GetGroupById(newGroup.Id)?.Id);
        }

        /// <summary>
        /// Insert permissions for all users in group
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPost("insertPermissionsByGroup")]
        public IActionResult InsertPermissionGroup(RegisterPermissionCommand command)
        {
            var usersGroup = _userService.GetUsersByGroupId(groupId: command.GroupId);
            var permissions = new Permission(command.Schema, null, null, null);
            _permissionService.InsertPermissionsInGroup(usersGroup, permissions);

            return ResponseDefault(_permissionService.GetPermissionsByGroup(usersGroup));
        }

        /// <summary>
        /// Update groupname 
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPut("updateGroup")]
        public IActionResult UpdateGroup([FromBody] RegisterGroupCommand command, string id)
        {
            var group = _groupService.GetGroupById(id);
            _groupService.UpdateGroupName(group, command.GroupName);
            return ResponseDefault(_groupService.GetGroupById(id));
        }

        /// <summary>
        /// Delete group by id
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpDelete("deleteGroupById/{groupId}")]
        public IActionResult DeleteGroup(string groupId)
        {
            var group = _groupService.GetGroupById(groupId);

            _groupService.DeleteGroup(group);
            return ResponseDefault(_groupService.GetGroupById(groupId));

        }

        /// <summary>
        /// Get all groups active 
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>  
        [HttpGet("getAll/groups")]
        public IActionResult GetGroups(string name, int? page = null, int? limit = null, string sortField = null, bool sortDesc = false,int? companySiteId = null)
        {
            var res = _groupService.GetGroupsPaginada(name, page, limit, companySiteId,sortField,sortDesc);
            foreach (var group in res.Data)
            {
                group.TotalUsersInGroup = _userService.GetUsersByGroupId(group.Id).Count;
            }
            return ResponseDefault(res);
        }

        /// <summary>
        /// Insert invidual permission by user
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response> 
        [AllowAnonymous]
        [HttpPost("insertPermission")]
        public IActionResult InsertPermission([FromBody] RegisterPermissionCommand command)
        {
            var user = _userService.GetUserById(command.UserID);

            var newPermission = new Permission(command.Schema, command.UserID, user.Name, user.UserName);
            _permissionService.Register(newPermission);

            var permission = _permissionService.GetPermissionById(newPermission.UserID);

            foreach (var schema in command.Schema)
            {
                if (schema.Segmentations != null)
                {
                    foreach (var seg in schema.Segmentations)
                    {

                        var newSeg = new Segmentation(schema.SchemaName, seg.field, seg.values.ToArray(), command.UserID);
                        var getSegmentation = _segmentationService.GetSegmentationByFields(command.UserID, schema.SchemaName, seg.field);
                        if (getSegmentation == null)
                        {

                            _segmentationService.Register(newSeg);
                        }
                        else
                        {
                            _segmentationService.UpdateSegmentation(newSeg, getSegmentation.Id);

                        }


                    }
                }


            }
            // _segmentationService.Register()
            return ResponseDefault(permission);
        }


        ///// <summary>
        ///// Insert All Users Permissions
        ///// </summary>
        ///// <returns>Return success</returns>
        ///// <response code="200">Return success</response> 
        //[AllowAnonymous]
        //[HttpPost("insertUsersPermissionByGroupId")]
        //public IActionResult InsertAllUsersPermission(string groupId)
        //{
        //    var group = _groupService.GetGroupById(groupId);
        //    var users = _userService.GetUsersByGroupId(groupId);
        //    if (group.GroupName != "Administradores")
        //    {
        //        _permissionService.InsertPermissionsInGroup(users, new Permission(group.Schema, null, null, null));
        //    }
        //    // _segmentationService.Register()
        //    return ResponseDefault(group);
        //}


        /// <summary>
        /// Update invidual permission by user
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response> 
        [HttpPut("update/permission")]
        public IActionResult UpdatePermission([FromBody] UpdatePermissionCommand command)
        {
            List<SchemasGroup.Segmentation> segmentations = new List<SchemasGroup.Segmentation>();
            var permissions = _permissionService.GetPermissionById(command.UserID);
            if (permissions == null) return BadRequest();
            var schemaName = permissions.Schemas.Where(x => x.SchemaID == command.SchemaID).FirstOrDefault();
            foreach (var schemaSegmentations in command.Segmentations)
            {
                if (schemaSegmentations != null)
                {
                    var a = schemaSegmentations.values.ToArray();
                    var newSeg = new Segmentation(schemaName.SchemaName, schemaSegmentations.field, schemaSegmentations.values.ToArray(), command.UserID);

                    var getSegmentation = _segmentationService.GetSegmentationByFields(command.UserID, schemaName.SchemaName, schemaSegmentations.field);
                    if (getSegmentation == null)
                    {

                        _segmentationService.Register(newSeg);

                    }
                    else
                    {
                        _segmentationService.UpdateSegmentation(newSeg, getSegmentation.Id);

                    }
                    segmentations.Add(new SchemasGroup.Segmentation { field = newSeg.Field, values = schemaSegmentations.values });
                }
            }
            if (command.SchemaID != null && command.SchemaID != "") _permissionService.UpdatePermission(permissions, command.SchemaID, command.PermissionsSchemas, command.Actions, segmentations);
            return ResponseDefault(_permissionService.GetPermissionById(permissions.UserID));
        }



        /// <summary>
        /// Get all permission from all users
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response> 
        [HttpGet("getAll/permission")]
        public IActionResult GetPermissions()
        {
            return ResponseDefault(_permissionService.GetPermissions());
        }

        /// <summary>
        /// Get all permission by user
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>         
        [HttpGet("searchByUserId/{userId}")]
        public IActionResult GetPermissionsByUserId(string userId)
        {
            var permission = _permissionService.GetPermissionById(userId); // find permissions
            if (permission == null) // if null and first user tenanty
            {
                var user = _userService.GetUserById(userId); // get user by id
                if (user != null) // user find
                {
                    if (_userService.GetUsersByTenanty(user.Tenanty).Count == 1) // first user
                    {
                        _groupService.Register(new Group("Administradores", null), null); // register group adm
                        var group = _groupService.GetGroupByName("Administradores");
                        user.GroupId = group.Id;
                        user.Root = true;
                        _userService.UpdateGroup(user, group.Id); // update user group

                        List<SchemasGroup> schemas = new();
                        Permission emptyPermission = new(schemas, user.Id, user.Name, user.UserName);
                        _permissionService.Register(emptyPermission); // register empty permission
                        return ResponseDefault(emptyPermission); // return this permission
                    }
                }

                return ResponseDefault(); // user not found, return default.
            }
            return ResponseDefault(permission); // return exist permission.
        }

        /// <summary>
        /// Get all permission by user
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>         
        [HttpGet("searchByUserId/{userId}/{module}")]
        public IActionResult GetPermissionsByUserIdModule(string userId,string module)
        {
            var permission = _permissionService.GetPermissionById(userId); // find permissions

      

            var schemasByModule = _schemaRepository.GetSchemasByModuleRegex(module);
            var schemaNamesToSearch = schemasByModule.Select(x => x.Name).ToList();
            permission.Schemas = permission.Schemas.Where(x => schemaNamesToSearch.Contains(x.SchemaName)).ToList();
            if (permission == null) // if null and first user tenanty
            {
                var user = _userService.GetUserById(userId); // get user by id
                if (user != null) // user find
                {
                    if (_userService.GetUsersByTenanty(user.Tenanty).Count == 1) // first user
                    {
                        _groupService.Register(new Group("Administradores", null), null); // register group adm
                        var group = _groupService.GetGroupByName("Administradores");
                        user.GroupId = group.Id;
                        user.Root = true;
                        _userService.UpdateGroup(user, group.Id); // update user group

                        List<SchemasGroup> schemas = new();
                        Permission emptyPermission = new(schemas, user.Id, user.Name, user.UserName);
                        _permissionService.Register(emptyPermission); // register empty permission
                        return ResponseDefault(emptyPermission); // return this permission
                    }
                }

                return ResponseDefault(); // user not found, return default.
            }
            if (permission.Schemas.Any()) return ResponseDefault(permission.Schemas);
            return ResponseDefault(); // return exist permission.
        }

        /// <summary>
        /// Get all permission by group
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>        
        [HttpGet("searchByGroup/{groupId}")]
        //[HttpGet("searchByGroup")]
        public IActionResult GetGroupUsersPermission(string nameUser, string groupId, int page, int limit)
        {
            if (nameUser == null)
            {
                var usersGroup = _userService.GetUsersByGroupId(groupId);
                if (usersGroup.Count <= 0) return BadRequest();
                //return ResponseDefault(_permissionService.GetPermissionsByGroup(usersGroup)); //  fun. Anteriores
                return ResponseDefault(_permissionService.GetPermissionsByGroupPaginada(usersGroup, page, limit)); // nova
            }
            else
            {
                var usersGroup = _userService.GetByUserNameRegex(groupId, nameUser);
                return ResponseDefault(_permissionService.GetPermissionByNamePaginada(usersGroup, page, limit));
            }
        }

        /// <summary>
        /// Copy a permission from other user
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response> 
        [HttpPost("CopyPermission")]
        public IActionResult CopyPermission(string idUser, string idUserCopy)
        {
            var permissionOrigin = _permissionService.GetPermissionById(idUser);
            var permissionDesti = _permissionService.GetPermissionById(idUserCopy);
            var user = _userService.GetUserById(idUserCopy);

            var newpermission = new Permission(permissionOrigin.Schemas, idUserCopy, user.Name, user.UserName);
            _permissionService.Register(newpermission);

            return ResponseDefault(_permissionService.GetPermissionById(idUserCopy));

        }

        #region codigo comentado

        // Analisar se está em uso, se não, pode deletar...
        //[HttpPut("update/DadosUser")]
        //public async Task<IActionResult> UpdateDadosUser(string userId,string nome,string email,string telefone)
        //{
        //    var user = _permissionService.GetPermissionById(userId);

        //    _permissionService.UpdateDadosUser(user, nome, email, telefone);


        //    return ResponseDefault(_permissionService.GetPermissionById(user.UserID));
        //}

        // Deleta um schema ou action e suas permissões de um usuário especifico
        //[HttpDelete("deletePermission")]
        //public async Task<IActionResult> DeletePermission([FromBody] DeletePermissionCommand command)
        //{
        //    var permission = _permissionService.GetPermissionById(command.UserID);
        //    try
        //    {
        //        if (command.SchemaID != null) _permissionService.DeletePermissionSchema(permission, command.SchemaID);
        //        if (command.ActionID != null) _permissionService.DeletePermissionAction(permission, command.ActionID);
        //        return ResponseDefault(_permissionService.GetPermissionById(command.UserID));
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        ///// <summary> // ANALISE - REATIVAR SE ESTIVER EM TUDO
        ///// Delete all permision from user
        ///// </summary>
        ///// <returns>Return success</returns>
        ///// <response code="200">Return success</response> 
        //[HttpDelete("deletePermissions")]
        //public async Task<IActionResult> DeletePermissions([FromBody] DeletePermissionCommand command)
        //{
        //    var permissions = _permissionService.GetPermissionById(command.UserID);
        //    try
        //    {
        //        _permissionService.DeletePermissions(permissions);
        //        return ResponseDefault(_permissionService.GetPermissionById(command.UserID));
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}
        #endregion
    }
}
