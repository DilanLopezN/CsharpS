using DotLiquid;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using MongoDB.Driver;
using Simjob.Framework.Application.Controllers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Domain.Util;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Infra.Identity.Models.Module;
using Simjob.Framework.Infra.Identity.Models.ModuleIdentity;
using Simjob.Framework.Infra.Identity.Services;
using Simjob.Framework.Infra.Schemas.Entities;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using System;
using Newtonsoft.Json;
using Simjob.Framework.Infra.Domain.Models;
using Simjob.Framework.Domain.Core.Utils;

namespace Simjob.Framework.Services.Api.Controllers
{
    [Authorize(Policy = "ApiKeyPolicy")]
    public class ModuleIdentityController : BaseController
    {
        private readonly IModuleIdentityService _moduleIdentityService;
        private readonly IUserAdminService _userAdminService;
        protected readonly IMongoCollection<ModuleIdentity> _collection;
        protected readonly ModuleIdentityContext Context;

        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;
        private readonly IRepository<MongoDbContext, Entities.Action> _actionRepository;
        public ModuleIdentityController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, IModuleIdentityService moduleIdentityService, ModuleIdentityContext context, IRepository<MongoDbContext, Schema> schemaRepository, IRepository<MongoDbContext, Entities.Action> actionRepository, IUserAdminService userAdminService) : base(bus, notifications)
        {
            _moduleIdentityService = moduleIdentityService;
            Context = context;
            _collection = context.GetUserCollection();
            _schemaRepository = schemaRepository;
            _actionRepository = actionRepository;
            _userAdminService = userAdminService;
        }
        private string VeirificaAdminReturnTenanty()
        {
            var acesstoken = Request.Headers[HeaderNames.Authorization];
            string userId = "";
            string tenanty = "";
            try
            {
                var accessToken = Request.Headers[HeaderNames.Authorization];
                var tokenInfo = Util.GetUserInfoFromToken(accessToken);

                if (tokenInfo.Count > 0)
                {
                    userId = tokenInfo["userid"];
                    tenanty = tokenInfo["tenanty"];
                }
            }
            catch (Exception)
            {

                throw;
            }
            if (string.IsNullOrEmpty(userId)) return "";
            var usuarioAdminLogado = _userAdminService.GetUserAdminById(userId);
            if (usuarioAdminLogado == null) return "";
            return tenanty;
        }
        /// <summary>
        /// Insert new ModuleIdentity
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPost()]
        public IActionResult Insert([FromBody] CreateModuleIdentityModel model)
        {
            string tenanty = VeirificaAdminReturnTenanty();
            if (string.IsNullOrEmpty(tenanty)) return Unauthorized();
            _moduleIdentityService.Register(model);

            return ResponseDefault();
        }


        /// <summary>
        /// Update ModuleIdentity
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPut()]
        public IActionResult Update([FromBody] UpdateModuleIdentityModel model)
        {
            string tenanty = VeirificaAdminReturnTenanty();
            if (string.IsNullOrEmpty(tenanty)) return Unauthorized();
            _moduleIdentityService.Update(model);

            return ResponseDefault();
        }

        /// <summary>
        /// get  ModuleIdentity
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            string tenanty = VeirificaAdminReturnTenanty();
            if (string.IsNullOrEmpty(tenanty)) return Unauthorized();
            var module = _moduleIdentityService.GetById(id);

            return ResponseDefault(module);
        }

        /// <summary>
        /// Enable a ModuleIdentity
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPatch("Enable/{id}")]
        public IActionResult Enable(string id)
        {
            string tenanty = VeirificaAdminReturnTenanty();
            if (string.IsNullOrEmpty(tenanty)) return Unauthorized();
            _moduleIdentityService.Enable(id);

            return ResponseDefault();
        }

        /// <summary>
        /// Disable a ModuleIdentity
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPatch("Disable/{id}")]
        public IActionResult Disable(string id)
        {
            string tenanty = VeirificaAdminReturnTenanty();
            if (string.IsNullOrEmpty(tenanty)) return Unauthorized();
            _moduleIdentityService.Disable(id);

            return ResponseDefault();
        }

        /// <summary>
        /// get  ModuleIdentities
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpGet()]
        public IActionResult GetAll(int? page, int? limit)
        {
            string tenanty = VeirificaAdminReturnTenanty();
            if (string.IsNullOrEmpty(tenanty)) return Unauthorized();

            if (page == null) page = 1;
            if (limit == null) limit = 10;
            var filterTenanty = Builders<ModuleIdentity>.Filter.Eq(u => u.Tenanty, tenanty);
            var filterIsDeleted = Builders<ModuleIdentity>.Filter.Eq(u => u.IsDeleted, false);

            var listBusca = _collection.Find(filterIsDeleted & filterTenanty).ToList();
            //permissionAction
            var actionIdsForPermission = listBusca.Where(x => x.ActionId != null && x.Type == "Action").Select(x => x.ActionId).ToList();

            var res = listBusca.Skip((int)((page - 1) * limit)).Take((int)limit);
            var moduleNames = res.Where(x => x.Type == "Schema").Select(x => x.Name).ToList();
            List<ReturnModuleModel> schemasReturn = new List<ReturnModuleModel>();

            foreach (var moduleName in moduleNames)
            {
                var schemas = _schemaRepository.GetSchemasByModuleRegex(moduleName);
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

                    };
                }));
            }
            var moduleIds = listBusca.Where(x => x.ModuleId != null).Select(x => x.Id).ToList();
            var filterIsDeletedChildren = Builders<ModuleIdentity>.Filter.Eq(u => u.IsDeleted, false);
            var filterInIds = Builders<ModuleIdentity>.Filter.In(u => u.Id, moduleIds);
            var filters = filterInIds & filterIsDeletedChildren;
            var fullModuleList = _collection.Find(filters).ToList();
            var actionIds = listBusca.Where(x => x.ActionId != null && x.Type == "Action").Select(x => x.ActionId).ToList();
            //filterAction
            var actionsReturnList = _actionRepository.GetAllActionsByIds(actionIds);
            var retornoMontado = new List<dynamic>();
            foreach (var item in res)
            {
                {
                    ////buscar módulos relacionados ao id do pai, verificar os tipos
                    if (item.Type == "None")
                    {
                        //var childrenModulesList = fullModuleList.Where(cm => cm.ModuleId == item.Id).ToList();
                        //if (childrenModulesList != null)
                        //{
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
                                item.Tenanty,
                                SubModules = new List<dynamic>()
                            });
                        //}
                    }
                    if (item.Type == "Schema")
                    {
                        var schemaReturn = schemasReturn.Where(y => y.Module == item.Name).ToList();
                        if (schemaReturn != null)
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
                                item.Tenanty,
                                SubModules = schemaReturn.Select(w => new
                                {
                                    w.Icon,
                                    Name = w.Description,
                                    Path = w.Path,
                                    SchemaName = w.SchemaName

                                }).ToList(),
                            });
                        }

                    }
                    if (item.Type == "Action")
                    {
                        var actionReturn = actionsReturnList.Where(y => y.Id == item.ActionId).FirstOrDefault();
                        if (actionReturn != null)
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
                                item.Tenanty,
                                SubModules = new List<dynamic>
                                {
                                    new
                                    {
                                        Icon = actionReturn.Icon,
                                        Name = actionReturn.Name,
                                        Description = actionReturn.Description,
                                        Path = ""
                                    }


                                }
                            });
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
                            item.Tenanty,
                            SubModules = new List<dynamic>()
                        });
                    }

                }
            }
            long count = Convert.ToInt64(listBusca.Count());
            return ResponseDefault(new PaginationData<dynamic>(retornoMontado.ToList(), page, limit, count));

        }


        /// <summary>
        /// get  ModuleIdentities
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpGet("Tenanty/{tenanty}")]
        public IActionResult GetAllByTenanty(int? page, int? limit,string tenanty)
        {
            string tenantyLogado = VeirificaAdminReturnTenanty();
            if (string.IsNullOrEmpty(tenantyLogado)) return Unauthorized();

            var acesstoken = Request.Headers[HeaderNames.Authorization];

            if (page == null) page = 1;
            if (limit == null) limit = 10;
            var filterTenanty = Builders<ModuleIdentity>.Filter.Eq(u => u.Tenanty, tenanty);
            var filterIsDeleted = Builders<ModuleIdentity>.Filter.Eq(u => u.IsDeleted, false);

            var listBusca = _collection.Find(filterIsDeleted & filterTenanty).ToList();
            //permissionAction
            var actionIdsForPermission = listBusca.Where(x => x.ActionId != null && x.Type == "Action").Select(x => x.ActionId).ToList();
        
            var res = listBusca.Skip((int)((page - 1) * limit)).Take((int)limit);
            var moduleNames = res.Where(x => x.Type == "Schema").Select(x => x.Name).ToList();
            List<ReturnModuleModel> schemasReturn = new List<ReturnModuleModel>();
     
            foreach (var moduleName in moduleNames)
            {
                var schemas = _schemaRepository.GetSchemasByModuleRegex(moduleName);
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

                    };
                }));
            }
            var moduleIds = listBusca.Where(x => x.ModuleId != null).Select(x => x.Id).ToList();
            var filterIsDeletedChildren = Builders<ModuleIdentity>.Filter.Eq(u => u.IsDeleted, false);
            var filterInIds = Builders<ModuleIdentity>.Filter.In(u => u.Id, moduleIds);
            var filters = filterInIds & filterIsDeletedChildren;
            var fullModuleList = _collection.Find(filters).ToList();
            var actionIds = listBusca.Where(x => x.ActionId != null && x.Type == "Action").Select(x => x.ActionId).ToList();
            //filterAction
            var actionsReturnList = _actionRepository.GetAllActionsByIds(actionIds);
            var retornoMontado = new List<dynamic>();
            foreach (var item in res)
            {
                {
                    ////buscar módulos relacionados ao id do pai, verificar os tipos
                    if (item.Type == "None")
                    {
                        //var childrenModulesList = fullModuleList.Where(cm => cm.ModuleId == item.Id).ToList();
                        //if (childrenModulesList != null)
                        //{
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
                                item.Tenanty,
                                SubModules = new List<dynamic>()
                            });
                        //}
                    }
                    if (item.Type == "Schema")
                    {
                        var schemaReturn = schemasReturn.Where(y => y.Module == item.Name).ToList();
                        if (schemaReturn != null)
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
                                item.Tenanty,
                                SubModules = schemaReturn.Select(w => new
                                {
                                    w.Icon,
                                    Name = w.Description,
                                    Path = w.Path,
                                    SchemaName = w.SchemaName

                                }).ToList(),
                            });
                        }

                    }
                    if (item.Type == "Action")
                    {
                        var actionReturn = actionsReturnList.Where(y => y.Id == item.ActionId).FirstOrDefault();
                        if (actionReturn != null)
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
                                item.Tenanty,
                                SubModules = new List<dynamic>
                                {
                                    new
                                    {
                                        Icon = actionReturn.Icon,
                                        Name = actionReturn.Name,
                                        Description = actionReturn.Description,
                                        Path = ""
                                    }


                                }
                            });
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
                            item.Tenanty,
                            SubModules = new List<dynamic>()
                        });
                    }

                }
            }
            long count = Convert.ToInt64(listBusca.Count());
            return ResponseDefault(new PaginationData<dynamic>(retornoMontado.ToList(), page, limit, count));

        }
    }
}
