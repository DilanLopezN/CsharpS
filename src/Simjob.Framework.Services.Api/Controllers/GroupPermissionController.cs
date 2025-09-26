using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Simjob.Framework.Application.Controllers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Core.Utils;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Commands.AccessGroup;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Infra.Schemas.Entities;
using Simjob.Framework.Services.Api.Interfaces;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Simjob.Framework.Services.Api.Controllers
{
    [Authorize]
    public class GroupPermissionController : BaseController
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly ISegmentationService _segmentationService;
        private readonly IGroupService _groupService;
        private readonly IRepository<MongoDbContext, Group> _repository;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;

        public GroupPermissionController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications,
                               ITokenService tokenService, IRepository<MongoDbContext, Group> repository, IUserService userService, ISegmentationService segmentationService, IGroupService groupService, IRepository<MongoDbContext, Schema> schemaRepository) : base(bus, notifications)
        {
            _tokenService = tokenService;
            _userService = userService;
            _groupService = groupService;
            _schemaRepository = schemaRepository;
            _repository = repository;
            _segmentationService = segmentationService;
        }



        /// <summary>
        /// Register new Group
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>

        [HttpPost("insertGroupPermission")]
        public IActionResult InsertGroup([FromBody] RegisterGroupPermissionCommand command)
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
            var newGroup = new Group(command.GroupName, command.Schema);
            _groupService.Register(newGroup, tenanty);
            return ResponseDefault(_groupService.GetGroupById(newGroup.Id));
        }

        /// <summary>
        /// Update permission 
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPut("updateGroupPermission")]
        public async Task<IActionResult> UpdateGroup([FromBody] RegisterGroupPermissionCommand command, string id)
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
            var group = _groupService.GetGroupById(id);
            if (group == null) return ResponseDefault("groupId not Found");
            await _groupService.UpdateGroupPermission(group, command, tenanty);
            return ResponseDefault(_groupService.GetGroupById(id));
        }

        /// <summary>
        /// Get all groups active 
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>  
        [HttpGet("getAll/groups")]
        public IActionResult GetGroups(string name, int? page = null, int? limit = null, string sortField = null, bool sortDesc = false)
        {
            //if (name != null)
            //{
            //return ResponseDefault(_groupService.GetGroups());
            return ResponseDefault(_groupService.GetGroupsPaginada(name, page, limit));
            //}
            //else
            //{
            //    var res = _repository.GetAll(page, limit, sortField, sortDesc);
            //    foreach (var group in res.Data)
            //    {
            //        group.TotalUsersInGroup = _userService.GetUsersByGroupId(group.Id).Count;
            //    }
            //    return ResponseDefault(res);
            //}
        }


        /// <summary>
        /// Get group permissions by id
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>  
        [HttpGet("get/groupPermissionsById")]
        public IActionResult GetGroupPermissions(string id)
        {
            var group = _groupService.GetGroupById(id);
            if (group == null) return ResponseDefault("groupId not Found");
            group.TotalUsersInGroup = _userService.GetUsersByGroupId(group.Id).Count();
            return ResponseDefault(group);
        }

        /// <summary>
        /// Get group permissions by id e module
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>  
        [HttpGet("get/groupPermissionsById/{id}/{module}")]
        public IActionResult GetGroupPermissionsModule(string id,string module)
        {
            var group = _groupService.GetGroupById(id);
            if (group == null) return ResponseDefault("groupId not Found");
            var schemasByModule = _schemaRepository.GetSchemasByModuleRegex(module);
            var schemaNamesToSearch = schemasByModule.Select(x => x.Name).ToList();


            group.Schema = group.Schema.Where(x => schemaNamesToSearch.Contains(x.SchemaName)).ToList();
            group.TotalUsersInGroup = _userService.GetUsersByGroupId(group.Id).Count();

            var retorno = new
            {
                group.Id,
                group.GroupName,
                group.TotalUsersInGroup,
                group.CreateAt,
                group.UpdateAt,
                group.UpdateBy,
                group.DeleteAt,
                group.DeleteBy,
                group.IsDeleted,
                Schema = group.Schema.Select(x =>
                {
                    var schema = schemasByModule.Where(y => x.SchemaName == y.Name).FirstOrDefault(); // You can compute this here
                    var schemaModelData = JsonConvert.DeserializeObject<Simjob.Framework.Infra.Domain.Models.SchemaModel>(schema.JsonValue);
                    return new
                    {

                        SchemaDescription = schemaModelData.Description,
                        x.SchemaName,
                        x.Permissions,
                        x.SchemaID,
                        x.Actions,
                        x.Segmentations
                    };
                }).ToList()
            };

            return ResponseDefault(retorno);
        }
    }
}
