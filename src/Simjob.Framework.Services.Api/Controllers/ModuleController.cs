using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using MongoDB.Bson;
using MongoDB.Driver;
using MoreLinq;
using Newtonsoft.Json;
using Simjob.Framework.Application.Controllers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Domain.Util;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Domain.Models;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Infra.Identity.Models.Module;
using Simjob.Framework.Infra.Schemas.Entities;
using Simjob.Framework.Services.Api.Models.Modules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Simjob.Framework.Services.Api.Controllers
{
    [Authorize(Policy = "ApiKeyPolicy")]
    public class ModuleController : BaseController
    {
        private readonly IModuleService _moduleService;
        protected readonly IMongoCollection<Module> _collection;
        protected readonly ModuleContext Context;
        private readonly IPermissionService _permissionService;
        private readonly IRepository<ModulePermissionContext, ModulePermission> _modulePermissionRepository;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;
        private readonly IRepository<MongoDbContext, Entities.Action> _actionRepository;
        private readonly IUserService _userService;
        private readonly IGroupService _groupService;

        public ModuleController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, IModuleService moduleService, IUserService userService, IPermissionService permissionService, ModuleContext context, IRepository<MongoDbContext, Schema> schemaRepository, IRepository<MongoDbContext, Entities.Action> actionRepository, IRepository<ModulePermissionContext, ModulePermission> modulePermission, IGroupService groupService) : base(bus, notifications)
        {
            _moduleService = moduleService;
            Context = context;
            _collection = context.GetUserCollection();
            _schemaRepository = schemaRepository;
            _actionRepository = actionRepository;
            _userService = userService;
            _permissionService = permissionService;
            _modulePermissionRepository = modulePermission;
            _groupService = groupService;
        }


        /// <summary>
        /// Insert new Module
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPost()]
        public IActionResult Insert([FromBody] CreateModuleModel model)
        {
            var module = _moduleService.Register(model);

            return ResponseDefault(module);
        }


        /// <summary>
        /// Update Module
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPut()]
        public IActionResult Update([FromBody] UpdateModuleModel model)
        {
            var module = _moduleService.Update(model);

            return ResponseDefault(module);
        }


        /// <summary>
        /// get  Module
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var module = _moduleService.GetById(id);

            return ResponseDefault(module);
        }

        /// <summary>
        /// Enable a module
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPatch("Enable/{id}")]
        public IActionResult Enable(string id)
        {
            _moduleService.Enable(id);

            return ResponseDefault();
        }

        /// <summary>
        /// Disable a module
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPatch("Disable/{id}")]
        public IActionResult Disable(string id)
        {
            _moduleService.Disable(id);

            return ResponseDefault();
        }



        private dynamic ModuleRecursive(List<Module> modules, List<Module> fullModuleList, List<string> schemasPermissionList, List<Entities.Action> actionsReturnList)
        {
            var listChildrenReturn = new List<dynamic>();
            foreach (var module in modules)
            { 
                if (module.Type == "Schema")
                {
                    var moduleName = module.Name;
                    List<ReturnModuleModel> schemasReturn = new List<ReturnModuleModel>();
                    var schemas = _schemaRepository.GetSchemasByModuleRegex(moduleName);
                    schemas = schemas.Where(x => schemasPermissionList.Contains(x.Name)).ToList();
                    schemasReturn.AddRange(schemas.Select(x =>
                    {
                        var schemaModel = JsonConvert.DeserializeObject<SchemaModel>(x.JsonValue);
                        return new ReturnModuleModel
                        {
                            SchemaName = x.Name,
                            Description = schemaModel.Description,
                            Module = schemaModel.Module,
                            Redirect = schemaModel.Redirect,
                            Icon = schemaModel.Icon,
                            Path = $"/entity/{x.Name}",
                        };
                    }));

                    var schemaReturn = schemasReturn.Where(y => y.Module == module.Name).ToList();
                    if (schemaReturn != null)
                    {
                        listChildrenReturn.Add(new
                        {
                            module.Id,
                            module.Name,
                            module.Description,
                            module.Type,
                            module.Icon,
                            module.Active,
                            module.ModuleId,
                            module.ActionId,
                            module.Price,
                            module.Path,
                            module.Order,
                            SubModules = schemaReturn.DistinctBy(x => x.Path).Select(w => new
                            {
                                w.Icon,
                                Name = w.Description,
                                Path = w.Path,
                                Redirect = w.Redirect,
                                SchemaName = w.SchemaName

                            }).ToList(),
                        });

                    }
                }
                if (module.Type == "Action")
                {

                    if (!string.IsNullOrEmpty(module.ActionId))
                    {
                        var actionReturn = actionsReturnList.Where(y => y.Id == module.ActionId).FirstOrDefault();
                        if (actionReturn != null)
                        {

                            listChildrenReturn.Add(new
                            {
                                module.Id,
                                Name = actionReturn.Name,
                                Description = actionReturn.Description,
                                module.Type,
                                Icon = actionReturn.Icon,
                                module.Active,
                                module.ModuleId,
                                module.Price,
                                module.ActionId,
                                Path = module.Path,
                                module.Order,
                                SubModules = new List<dynamic>()
                            });
                        }
                    }
                    else
                    {
                        var actionsReturnSubModules = actionsReturnList.Where(y => y.Module == module.Name).ToList();
                        if (!actionsReturnSubModules.IsNullOrEmpty())
                        {

                            listChildrenReturn.Add(new
                            {
                                module.Id,
                                module.Name,
                                module.Description,
                                module.Type,
                                module.Icon,
                                module.Active,
                                module.ModuleId,
                                module.Price,
                                module.ActionId,
                                Path = module.Path,
                                module.Order,
                                SubModules = actionsReturnSubModules.Select(x => new
                                {
                                    ActionId = x.Id,
                                    Icon = x.Icon,
                                    Name = x.Name,
                                    Description = x.Description,
                                    Path = module.Path
                                }).ToList()
                            });
                        }

                    }
                }



                if (module.Type == "Component" || module.Type == "Addon")
                    listChildrenReturn.Add(new
                    {
                        module.Id,
                        module.Name,
                        module.Description,
                        module.Type,
                        module.Icon,
                        module.Active,
                        module.ModuleId,
                        module.Price,
                        module.ActionId,
                        module.Path,
                        module.Order,
                        SubModules = new List<dynamic>()
                    });


                if (module.Type == "None")
                {
                    var filterIsDeletedChildren = Builders<Module>.Filter.Eq(u => u.IsDeleted, false);
                    var filterInIds = Builders<Module>.Filter.Eq(u => u.Id, module.ModuleId);
                    var filters = filterInIds & filterIsDeletedChildren;
                    var childrenModules = fullModuleList.Where(x => x.ModuleId == module.Id).ToList();

                         
             
                        listChildrenReturn.Add(new
                        {
                            module.Id,
                            module.Name,
                            module.Description,
                            module.Type,
                            module.Icon,
                            module.Active,
                            module.ModuleId,
                            module.Price,
                            module.ActionId,
                            module.Path,
                            module.Order,
                            SubModules = !childrenModules.IsNullOrEmpty() ?  ModuleRecursive(childrenModules, fullModuleList, schemasPermissionList, actionsReturnList) : null,
                        });
                    
                }

            }
            return listChildrenReturn;
        }

        /// <summary>
        /// get  Module
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpGet("GetPaginated")]
        public IActionResult GetAllPaginated(int? page, int? limit, string? name)
        {
            if (page == null) page = 1;
            if (limit == null) limit = 10;
            var filterName = string.IsNullOrEmpty(name) ? Builders<Module>.Filter.Empty : Builders<Module>.Filter.Regex(u => u.Name, new BsonRegularExpression($"/^.*{name}.*$/i"));
            var filterIsDeleted = Builders<Module>.Filter.Eq(u => u.IsDeleted, false);

            var listBusca = _collection.Find(filterIsDeleted & filterName).ToList();
            var res = listBusca.Skip((int)((page - 1) * limit)).Take((int)limit);

            List<ReturnModuleModel> schemasReturn = new List<ReturnModuleModel>();
            //schemasPermission

            var retornoMontado = res.Select(x => new
            {
                x.Id,
                x.Name,
                x.Description,
                x.Type,
                x.Icon,
                x.Active,
                x.ModuleId,
                x.ActionId,
                x.Path,
                x.Price,
                x.Order,
            }).ToList();

            long count = Convert.ToInt64(listBusca.Count());
            return ResponseDefault(new PaginationData<dynamic>(retornoMontado.OrderBy(x => x.Order).ToList(), page, limit, count));
        }



        /// <summary>
        /// get  Module
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpGet()]
        public IActionResult GetAll(int? page, int? limit)
        {
            var acesstoken = Request.Headers[HeaderNames.Authorization];
            var userId = _userService.ClaimUserId(acesstoken);
            if (userId == null) return BadRequest("user is null");
            var user = _userService.GetUserById(userId);
            if (user == null) return BadRequest("user is null");
 
            var permissions = _permissionService.GetPermissionById(userId);
            var modulesPermissionUser = _modulePermissionRepository.GetListByField("userId", userId);
            var modulesPermissionGroup = _modulePermissionRepository.GetListByField("groupId", user.GroupId);
            var group = _groupService.GetGroupById(user.GroupId);
            var modulesPermission = modulesPermissionUser?.Where(x => x.Read == true).ToList();

            modulesPermission.AddRange(modulesPermissionGroup?.Where(x => x.Read == true).ToList());

            modulesPermission = modulesPermission.Distinct().ToList();
            var moduleIdsToFilter = modulesPermission.Select(x => x.ModuleId).Distinct().ToList();
            if (!moduleIdsToFilter.Any()) return BadRequest("No permission found");
            if (page == null) page = 1;
            if (limit == null) limit = 10;
            var filterIds = Builders<Module>.Filter.In(u => u.Id, moduleIdsToFilter);
            var filterIsDeleted = Builders<Module>.Filter.Eq(u => u.IsDeleted, false);

            var listBusca = _collection.Find(filterIsDeleted & filterIds).ToList();
            //permissionAction
            var actionPermissionList = new List<string>();
            var permissionActions = permissions?.Schemas?.Where(x => x.Actions != null && x.Actions.Any(y => y.Permissions == true)).SelectMany(x => x.Actions).DistinctBy(x => x.ActionId).Select(x => x.ActionId).ToList();

            if(permissionActions != null) actionPermissionList.AddRange(permissionActions);

            var groupActions = group?.Schema?.Where(x => x.Actions != null && x.Actions.Any(y => y.Permissions == true)).SelectMany(x => x.Actions).DistinctBy(x => x.ActionId).Select(x => x.ActionId).ToList();
            if(groupActions != null) actionPermissionList?.AddRange(groupActions);


            actionPermissionList = actionPermissionList.Distinct().ToList();
            if (actionPermissionList == null) actionPermissionList = new List<string>();
            listBusca.RemoveAll(x => x.Type == "Action" && x.ActionId != null && !actionPermissionList.Contains(x.ActionId));

            var moduleNamesForAction = listBusca.Where(x => x.Type == "Action" || x.Type == "None").Select(x => x.Name).ToList();
            //filterAction
            var actionsReturnList = _actionRepository.GetAllActionsByModules(moduleNamesForAction);
            actionsReturnList = actionsReturnList.Where(x => actionPermissionList.Contains(x.Id)).ToList();
            var res = listBusca.Where(x => string.IsNullOrEmpty(x.ModuleId)).Skip((int)((page - 1) * limit)).Take((int)limit);
            var moduleNames = res.Where(x => x.Type == "Schema" || x.Type == "None").Select(x => x.Name).ToList();
            List<ReturnModuleModel> schemasReturn = new List<ReturnModuleModel>();
            //schemasPermission
            var schemasPermissionList = new List<string>();

            var permissionsUser = permissions?.Schemas?.Where(x => x.SchemaName != null && x.Permissions != null && x.Permissions.Contains("Menu")).Select(x => x.SchemaName).Distinct().ToList();

            if (permissionsUser != null) schemasPermissionList.AddRange(permissionsUser);

            var permissionsGroup = group?.Schema?.Where(x => x.SchemaName != null && x.Permissions != null && x.Permissions.Contains("Menu")).Select(x => x.SchemaName).Distinct().ToList();
            if(permissionsGroup != null) schemasPermissionList.AddRange(permissionsGroup);

            schemasPermissionList = schemasPermissionList.Distinct().ToList();
            foreach (var moduleName in moduleNames)
            {
                var schemas = _schemaRepository.GetSchemasByModuleRegex(moduleName);

                schemas = schemas.Where(x => schemasPermissionList.Contains(x.Name)).ToList();
                schemasReturn.AddRange(schemas.Select(x =>
                {
                    var schemaModel = JsonConvert.DeserializeObject<SchemaModel>(x.JsonValue);
                    return new ReturnModuleModel
                    {
                        SchemaName = x.Name,
                        Description = schemaModel.Description,
                        Module = schemaModel.Module,
                        Icon = schemaModel.Icon,
                        Path = $"/entity/{x.Name}",
                        Redirect = schemaModel.Redirect

                    };
                }));
            }
            var moduleIds = listBusca.Where(x => x.ModuleId != null).Select(x => x.Id).ToList();
            var filterIsDeletedChildren = Builders<Module>.Filter.Eq(u => u.IsDeleted, false);
            var filterInIds = Builders<Module>.Filter.In(u => u.Id, moduleIds);
            var filters = filterInIds & filterIsDeletedChildren;
            var fullModuleList = _collection.Find(filters).ToList();

            var retornoMontado = new List<dynamic>();
            foreach (var item in res)
            {

                ////buscar módulos relacionados ao id do pai, verificar os tipos
                if (item.Type == "None")
                {
                    var childrenModulesList = fullModuleList.Where(cm => cm.ModuleId == item.Id).ToList();

                    List<RetornoSubModules> returnSubModuleModel = new List<RetornoSubModules>();


                    var schemaReturn = schemasReturn.Where(y => y.Module == item.Name).ToList();

                    var subModulesRecursive = ModuleRecursive(childrenModulesList.OrderBy(x => x.Order).ToList(), fullModuleList, schemasPermissionList, actionsReturnList);

                    if (subModulesRecursive != null && subModulesRecursive.Count > 0)
                    {
                        foreach (var subModule in subModulesRecursive)
                        {

                            returnSubModuleModel.Add(new RetornoSubModules
                            {
                                Id = subModule.Id,
                                Name = subModule.Name,
                                Description = subModule.Description,
                                Type = subModule.Type,
                                Icon = subModule.Icon,
                                Active = subModule.Active,
                                ModuleId = subModule.ModuleId,
                                Price = subModule.Price,
                                ActionId = subModule.ActionId,
                                Path = subModule.Path,
                                Order = subModule.Order,
                                SubModules = subModule.SubModules
                            });
                        }
                    }

                    if (!schemaReturn.IsNullOrEmpty())
                    {
                        returnSubModuleModel.AddRange(schemaReturn.DistinctBy(x=> x.Path).Select(x=> new RetornoSubModules
                        {
                            Icon = x.Icon,
                            Name = x.Description,
                            Path = x.Path,
                            Redirect = x.Redirect,
                            SchemaName = x.SchemaName
                        }));
                    }



                        retornoMontado.Add(new
                        {
                            item.Id,
                            item.Name,
                            item.Description,
                            item.Type,
                            item.Icon,
                            item.Active,
                            item.ModuleId,
                            item.Price,
                            item.ActionId,
                            item.Path,
                            item.Order,
                            SubModules = returnSubModuleModel
                            //SubModules = !childrenModulesList.IsNullOrEmpty() ? ModuleRecursive(childrenModulesList.OrderBy(x => x.Order).ToList(), fullModuleList, schemasPermissionList, actionsReturnList) : null,
                            //SubModulesSchema = !schemaReturn.IsNullOrEmpty() ? schemaReturn.DistinctBy(x => x.Path).Select(w => new
                            //{
                            //    w.Icon,
                            //    Name = w.Description,
                            //    Path = w.Path,
                            //    Redirect = w.Redirect,
                            //    SchemaName = w.SchemaName

                            //}).ToList() : null

                        });
                    
                }
                if (item.Type == "Schema")
                {
                    var schemaReturn = schemasReturn.Where(y => y.Module == item.Name).ToList();
                   
                        retornoMontado.Add(new
                        {
                            item.Id,
                            item.Name,
                            item.Description,
                            item.Type,
                            item.Icon,
                            item.Active,
                            item.ModuleId,
                            item.Price,
                            item.ActionId,
                            item.Path,
                            item.Order,
                            SubModules =  schemaReturn.DistinctBy(x => x.Path).Select(w => new
                            {
                                w.Icon,
                                Name = w.Description,
                                Path = w.Path,
                                Redirect = w.Redirect,
                                SchemaName = w.SchemaName

                            }).ToList()
                        });
                    

                }
                if (item.Type == "Action")
                {
                    if (!string.IsNullOrEmpty(item.ActionId))
                    {
                        var actionReturn = actionsReturnList.Where(y => y.Id == item.ActionId).FirstOrDefault();
                        if (actionReturn != null)
                        {

                            retornoMontado.Add(new
                            {
                                item.Id,
                                Name = actionReturn.Name,
                                Description = actionReturn.Description,
                                item.Type,
                                Icon = actionReturn.Icon,
                                item.Active,
                                item.ModuleId,
                                item.Price,
                                item.ActionId,
                                Path = item.Path,
                                item.Order,
                                SubModules = new List<dynamic>()
                            });
                        }
                    }
                    else
                    {
                        var actionsReturnSubModules = actionsReturnList.Where(y => y.Module == item.Name).ToList();
                        if (!actionsReturnSubModules.IsNullOrEmpty())
                        {

                            retornoMontado.Add(new
                            {
                                item.Id,
                                item.Name,
                                item.Description,
                                item.Type,
                                item.Icon,
                                item.Active,
                                item.ModuleId,
                                item.Price,
                                item.ActionId,
                                Path = item.Path,
                                item.Order,
                                SubModules = actionsReturnSubModules.Select(x => new
                                {
                                    ActionId = x.Id,
                                    Icon = x.Icon,
                                    Name = x.Name,
                                    Description = x.Description,
                                    Path = item.Path
                                }).ToList()
                            });
                        }

                    }

                }
                if (item.Type == "Component" || item.Type == "Addon")
                {
                    retornoMontado.Add(new
                    {
                        item.Id,
                        item.Name,
                        item.Description,
                        item.Type,
                        item.Icon,
                        item.Active,
                        item.ModuleId,
                        item.Price,
                        item.ActionId,
                        item.Path,
                        item.Order,
                        SubModules = new List<dynamic>()
                    });
                }
            }
            long count = Convert.ToInt64(listBusca.Where(x => x.ModuleId == null || x.ModuleId == "").Count());
            return ResponseDefault(new PaginationData<dynamic>(retornoMontado.OrderBy(x => x.Order).ToList(), page, limit, count));
        }
    }
}