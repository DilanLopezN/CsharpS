using DotLiquid;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using MongoDB.Driver;
using Simjob.Framework.Application.Controllers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Util;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Infra.Identity.Models.Module;
using Simjob.Framework.Infra.Identity.Models.ModulePermission;
using Simjob.Framework.Infra.Identity.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simjob.Framework.Services.Api.Controllers
{
    public class ModulePermissionController : BaseController
    {
        private readonly IModulePermissionService _modulePermissionService;
        protected readonly IMongoCollection<Module> _collectionModule;
        protected readonly ModuleContext ContextModule;

        protected readonly IMongoCollection<ModulePermission> _collection;
        protected readonly ModulePermissionContext Context;
        private readonly IUserService _userService;
        public ModulePermissionController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications,IModulePermissionService modulePermissionService, ModuleContext contextModule, ModulePermissionContext context, IUserService userService) : base(bus, notifications)
        {
            _modulePermissionService = modulePermissionService;
            ContextModule = contextModule;
            _collectionModule = contextModule.GetUserCollection();

            Context = context;
            _collection = context.GetUserCollection();
            _userService = userService;
        }


        /// <summary>
        /// Insert or Update ModulePermission
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPost()]
        public IActionResult Insert([FromBody] CreateModulePermissionModel model)
        {

            var filterIsDeletedModule = Builders<Module>.Filter.Eq(u => u.IsDeleted, false);

            var modulesDatabaseList = _collectionModule.Find(filterIsDeletedModule).ToList();

            var filterIsDeleted = Builders<ModulePermission>.Filter.Eq(u => u.IsDeleted, false);

            var modulesPermissionDatabaseList = _collection.Find(filterIsDeleted).ToList();
            _modulePermissionService.RegisterRecursive(model, modulesPermissionDatabaseList, modulesDatabaseList,new System.Collections.Generic.List<ModulePermission>(), new System.Collections.Generic.List<ModulePermission>());

            return ResponseDefault();
        }

        /// <summary>
        /// Insert or Update ModulePermission
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPost("InsertMany")]
        public IActionResult InsertMany([FromBody] List<CreateModulePermissionModel> models)
        {
            var acesstoken = Request.Headers[HeaderNames.Authorization];

            var userId = _userService.ClaimUserId(acesstoken);

            if (userId == null) return BadRequest("user é null");

            var user = _userService.GetUserById(userId);

            if (user == null) return BadRequest("user é null");

            if (user.Root == false && user.ControlAccess == false) return BadRequest("user nao tem permissao para cadastrar permissões");           

            _modulePermissionService.InsertMany(models);
            return ResponseDefault();
        }

        /// <summary>
        /// Insert or Update ModulePermission
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPost("InsertManyGroup")]
        public IActionResult InsertManyGroup([FromBody] List<CreateModulePermissionGroupModel> models)
        {
            var acesstoken = Request.Headers[HeaderNames.Authorization];

            var userId = _userService.ClaimUserId(acesstoken);

            if (userId == null) return BadRequest("user é null");

            var user = _userService.GetUserById(userId);

            if (user == null) return BadRequest("user é null");

            if (user.Root == false && user.ControlAccess == false) return BadRequest("user nao tem permissao para cadastrar permissões");

            var groupToRegister = models.Select(x => x.GroupId).FirstOrDefault();


            PaginationData<ModulePermission> modulePermissions = (PaginationData<ModulePermission>)_modulePermissionService.GetAll(null, user.Id, null, null);

            var moduleIds = modulePermissions.Data.Select(x => x.ModuleId).Distinct().ToList();

            var moduleIdsToInsert = models.Select(x => x.ModuleId).Distinct().ToList();

            foreach (var module in moduleIdsToInsert)
            {
                if (!moduleIds.Contains(module)) return BadRequest($"user nao tem permissão para módulo {module}");
            }

            return ResponseDefault(_modulePermissionService.InsertManyGroup(models));
        }





        /// <summary>
        /// get  ModulePermission
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var module = _modulePermissionService.GetById(id);

            return ResponseDefault(module);
        }

        /// <summary>
        /// Get all UserAccess pagined from database
        /// </summary>
        /// <returns>Return a list from UserAccess</returns>
        /// <response code="200">Return a list from UserAccess</response>
        [Authorize]
        [HttpGet]
        public IActionResult GetAll(string groupId = null,string userId = null,int? page = null, int? limit = null, string sortField = null, bool sortDesc = false)
        {
            return ResponseDefault( _modulePermissionService.GetAll(groupId,userId,page, limit, sortField, sortDesc));
        }

        /// <summary>
        /// Delete a ModulePermission
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(string id)
        {
            _modulePermissionService.Delete(id);

            return ResponseDefault();
        }

    }
}
