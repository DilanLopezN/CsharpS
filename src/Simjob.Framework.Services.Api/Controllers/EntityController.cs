using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using MongoDB.Driver.Linq;
using MoreLinq;
using MoreLinq.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Simjob.Framework.Application.Controllers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Core.Utils;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Domain.Models;
using Simjob.Framework.Domain.Models.WeeklyScheduleModels;
using Simjob.Framework.Domain.Util;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Data.Models;
using Simjob.Framework.Infra.Identity.Commands.AccessGroup;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Infra.Schemas.Commands.Entities;
using Simjob.Framework.Infra.Schemas.Entities;
using Simjob.Framework.Infra.Schemas.Interfaces;
using Simjob.Framework.Services.Api.Enums;
using Simjob.Framework.Services.Api.Interfaces;
using Simjob.Framework.Services.Api.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using static Simjob.Framework.Domain.Models.Searchdefs;
using static Simjob.Framework.Infra.Identity.Entities.Notification;
using ApiEntities = Simjob.Framework.Services.Api.Entities;

namespace Simjob.Framework.Services.Api.Controllers
{
    [ExcludeFromCodeCoverage]
    [Authorize(Policy = "ApiKeyPolicy")]
    public class EntityController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IEntityService _entityService;
        private readonly IConfiguration _configuration;
        private readonly IGeneratorsService _generatorsService;
        private readonly ISegmentationService _segmentationService;
        private readonly INotificationService _notificationService;
        private readonly IApproveLogService _approveLogService;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;
        private readonly IRepository<MongoDbContext, ApiEntities.Action> _actionRepository;

        private readonly IPermissionService _permissionService;
        private readonly ISearchdefsService _searchdefsService;
        private readonly IViewService _viewService;
        private readonly IUserAccessService _userAccessService;
        private readonly ITokenService _tokenService;
        private readonly IStatusFlowService _statusFlowService;
        private readonly IStatusFlowItemService _statusFlowItemService;
        private readonly ISharedSchemaRecordService _sharedSchemaRecordService;
        private readonly ISchemaBuilder _schemaBuilder;
        private readonly IRepository<SourceContext, Source> _sourceRepository;

        [ExcludeFromCodeCoverage]
        public EntityController(IViewService viewService, IPermissionService permissionService, IGeneratorsService generatorsService, ISegmentationService segmentationService, INotificationService notificationService, IApproveLogService approveLogService, IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, ITokenService tokenService, IEntityService entityService, IRepository<MongoDbContext, Schema> schemaRepository, IRepository<MongoDbContext, ApiEntities.Action> actionRepository, IUserService userService, IUserAccessService userAccessService, ISearchdefsService searchdefsService, IStatusFlowService statusFlowService, IStatusFlowItemService statusFlowItemService, ISharedSchemaRecordService sharedSchemaRecordService, IConfiguration configuration, ISchemaBuilder schemaBuilder, IRepository<SourceContext, Source> sourceRepository) : base(bus, notifications)
        {
            _tokenService = tokenService;
            _entityService = entityService;
            _schemaRepository = schemaRepository;
            _actionRepository = actionRepository;
            _userService = userService;
            _viewService = viewService;
            _permissionService = permissionService;
            _searchdefsService = searchdefsService;
            _userAccessService = userAccessService;
            _segmentationService = segmentationService;
            _generatorsService = generatorsService;
            _notificationService = notificationService;
            _approveLogService = approveLogService;
            _statusFlowService = statusFlowService;
            _statusFlowItemService = statusFlowItemService;
            _sharedSchemaRecordService = sharedSchemaRecordService;
            _configuration = configuration;
            _schemaBuilder = schemaBuilder;
            _sourceRepository = sourceRepository;
        }

        /// <summary>
        /// Get Relation Property
        /// </summary>
        /// <returns>Return result</returns>
        /// <response code="200">Return result paginated</response>
        [HttpGet("{schemaName}/relation-property")]
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> GetRelationProperty(string schemaName, string id, string property, int? page, int? limit, string sortField = null, bool sortDesc = false)
        {
            var fields = "";
            var values = "";
            var acesstoken = Request.Headers[HeaderNames.Authorization];
            var userId = _userService.ClaimUserId(acesstoken);
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            if (schema == null) return ResponseDefault();
            var json = schema.JsonValue;
            var permissionUser = _permissionService.RetornPermission(acesstoken.ToString());
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(json);

            string userField = null;
            var count = schemaModel.Properties.Where(x => x.Value.Type == "user").Count();
            if (count > 0)
            {
                userField = schemaModel.Properties.Where(x => x.Value.Type == "user").FirstOrDefault().Key;
            }
            // se usuario tem permissão de Read no schema.
            var permissaoReferenteAoSchema = permissionUser.Schemas.Where(x => x.SchemaName == schemaName).FirstOrDefault();
            if (permissaoReferenteAoSchema != null)
            {
                var isPermissionRead = false;
                var isPermissionOwner = false;

                foreach (var p in permissionUser.Schemas)
                {
                    if (p.SchemaName == schemaName)
                    {
                        if (p.Permissions.Contains("Read"))
                        {
                            isPermissionRead = true;
                        }
                        if (p.Permissions.Contains("Owner"))
                        {
                            isPermissionOwner = true;
                        }
                    }
                }

                if (isPermissionRead == true)
                {   //se tem permissão verificar se tem owner.
                    if (isPermissionOwner == true)
                    {
                        //verificar se tem acesso segmentado.
                        if (permissaoReferenteAoSchema.Segmentations != null)
                        {
                            if (permissaoReferenteAoSchema.Segmentations.Count > 0)
                            {
                                foreach (var s in permissaoReferenteAoSchema.Segmentations)
                                {
                                    if (s.values.Count > 1)
                                    {
                                        values += "[" + string.Join(",", s.values) + "]" + ",";
                                        fields += "[" + s.field + "],";
                                    }
                                    else
                                    {
                                        values += "[" + s.values[0] + "],";
                                        fields += "[" + s.field + "],";
                                    }
                                }

                                //pegar campos de segmentação e valores buscados e owner (obs: concatenar todos os valores com vírgula ex: status,nome,owner)
                                // pegar valor de segmentação e valores buscados e owner (obs: concatenar todos os valores com vírgula ex: ativo,matheus,id)

                                fields += "owners";
                                values += permissionUser.UserID;

                                var getSearch = await _entityService.SerachFields(userId, schemaName, fields, values, "", page, limit, null, new List<bool> { sortDesc }, sortField, null, id);
                                var getSearchOwner = _userService.ConvertOwnerId(getSearch, userField);
                                return ResponseDefault(getSearchOwner);
                            }
                        }

                        //senao tem acesso segmentado
                        //pega valores buscados e owner
                        fields += "owners";
                        values += permissionUser.UserID;
                        var getOwnersValues = await _entityService.SerachFields(userId, schemaName, fields, values, "", page, limit, null, new List<bool> { sortDesc }, sortField, null, id);
                        var getSearchOwnerConv = _userService.ConvertOwnerId(getOwnersValues, userField);
                        return ResponseDefault(getSearchOwnerConv);
                    }
                    //senão tem acesso a owner
                    //verificar se tem acesso segmentado.
                    if (permissaoReferenteAoSchema.Segmentations != null)
                    {
                        if (permissaoReferenteAoSchema.Segmentations.Count > 0)
                        {
                            foreach (var s in permissaoReferenteAoSchema.Segmentations)
                            {
                                if (s.values.Count > 1)
                                {
                                    values += "[" + string.Join(",", s.values) + "]" + ",";
                                    fields += "[" + s.field + "],";
                                }
                                else
                                {
                                    values += "[" + s.values[0] + "],";
                                    fields += "[" + s.field + "],";
                                }
                            }

                            //pegar valores de segmentação e valores buscados.
                            values = values.Substring(0, values.Length - 1);
                            fields = fields.Substring(0, fields.Length - 1);

                            var getSegValues = await _entityService.SerachFields(userId, schemaName, fields, values, "", page, limit, null, new List<bool> { sortDesc }, sortField, null, id);
                            var getSegConv = _userService.ConvertOwnerId(getSegValues, userField);
                            return ResponseDefault(getSegConv);
                        }
                    }

                    //senão tem acesso segmentado
                    // pegar valores buscados
                    //var getSearchBuscado = await _entityService.SerachFields(schemaName, fields, values, page, limit, null, sortField, sortDesc, id);
                    //var getBuscadoConv = _userService.ConvertOwnerId(getSearchBuscado);
                    //return ResponseDefault(getSearchBuscado);
                    return ResponseDefault(await _entityService.GetRelationProperty(schemaName, id, property, page, limit, sortField, sortDesc));
                }
            }
            return BadRequest($"Você não possui acesso ao schema {schemaName}");
        }

        /// <summary>
        /// Get results from schemaname or especifc fields
        /// </summary>
        /// <returns>Return result paginated</returns>
        /// <response code="200">Return result paginated</response>

        [HttpGet("{schemaName}/search-fields")]
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> SearchFields(string schemaName, string value, SearchModeEnum mode, int? page, int? limit, string sortField = null, string groupBy = null, bool sortDesc = false, string ids = "", string searchFields = null, string companySiteId = null)
        {
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            if (schema == null) return ResponseDefault();
            var json = schema.JsonValue;
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(json);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true && schemaModel.Alias != null)
            {
                if (sortField == null && schemaModel.PrimaryKey != null) sortField = schemaModel.PrimaryKey;
                var result = await SQLServerService.GetListEntity(schemaModel.Alias, page, limit, sortField, sortDesc, ids, searchFields, value, source, mode, schemaModel.CompanySiteId, companySiteId,schemaModel);
                if(!result.success) return BadRequest(result.error);
                var resultReturn = new
                {
                    result.data,
                    result.total,
                    page,
                    limit,
                    pages = limit != null ? (int)Math.Ceiling((double)result.total / limit.Value) : 0
                };
                return ResponseDefault(resultReturn);
            }

            var fields = "";
            var values = "";
            if (ids == null) return ResponseDefault(new PaginationData<object>(null, page, limit, 0));
            var acesstoken = Request.Headers[HeaderNames.Authorization];
            var userId = _userService.ClaimUserId(acesstoken);
            var permissionUser = _permissionService.RetornPermission(acesstoken.ToString());

            var companySiteIds = _userService.ClaimCompanySiteIds(acesstoken);
            if (schemaModel.Multi_Company && Request.Headers.ContainsKey("companySiteId"))
            {
                var companyHeaderId = Request.Headers["companySiteId"].ToString();
                if (!string.IsNullOrEmpty(companyHeaderId))
                {
                    if (!string.IsNullOrEmpty(companySiteId)) companySiteId += "," + companyHeaderId;
                    else companySiteId = companyHeaderId;
                }
            }
            if (!string.IsNullOrEmpty(companySiteId) && schemaModel.Multi_Company)
            {
                var companySiteIdsSplit = companySiteId.Split(',');
                bool containsAll = companySiteIdsSplit.All(id => companySiteIds.Contains(id));
                if (!containsAll)
                {
                    return ResponseDefault("Usuario não tem permissão para company");
                }
            }
            else if (!string.IsNullOrEmpty(companySiteId)) return ResponseDefault("Usuario não tem permissão para company");
            string userField = null;
            var count = schemaModel.Properties.Where(x => x.Value.Type == "user").Count();
            if (count > 0)
            {
                userField = schemaModel.Properties.Where(x => x.Value.Type == "user").FirstOrDefault().Key;
            }

            // se usuario tem permissão de Read no schema.
            var permissaoReferenteAoSchema = permissionUser.Schemas?.Where(x => x.SchemaName == schemaName).FirstOrDefault();
            if (permissaoReferenteAoSchema != null)
            {
                var isPermissionRead = false;
                var isPermissionOwner = false;

                foreach (var p in permissionUser.Schemas)
                {
                    if (p.SchemaName == schemaName)
                    {
                        if (p.Permissions.Contains("Read"))
                        {
                            isPermissionRead = true;
                        }
                        if (p.Permissions.Contains("Owner"))
                        {
                            isPermissionOwner = true;
                        }
                    }
                }

                if (isPermissionRead == true)
                {   //se tem permissão verificar se tem owner.
                    if (searchFields != null && value != null)
                    {
                        fields += "[" + searchFields + "],";
                        values += "[" + value + "],";
                    }
                    if (searchFields == null && value != null)
                    {
                        searchFields = await _entityService.GetListSearchFields(schemaName);

                        fields += "[" + searchFields + "],";
                        values += "[" + value + "],";
                    }
                    if (searchFields != null && value == null && mode.ToString() != "Contains")
                    {
                        value = "";
                        fields += "[" + searchFields + "],";
                        values += "[" + value + "],";
                    }

                    if (isPermissionOwner == true)
                    {
                        //verificar se tem acesso segmentado.
                        if (permissaoReferenteAoSchema.Segmentations != null)
                        {
                            if (permissaoReferenteAoSchema.Segmentations.Count > 0)
                            {
                                foreach (var s in permissaoReferenteAoSchema.Segmentations)
                                {
                                    if (s.values.Count > 1)
                                    {
                                        values += "[" + string.Join(",", s.values) + "]" + ",";
                                        fields += "[" + s.field + "],";
                                    }
                                    else
                                    {
                                        values += "[" + s.values[0] + "],";
                                        fields += "[" + s.field + "],";
                                    }
                                }
                                //items compartilhados somente owner
                                var sharedSchemaRecords = _sharedSchemaRecordService.GetBySchemaNameAndUserIdReceive(schemaName, userId);
                                dynamic getSharedValues1 = null;
                                List<object> listResult1 = new List<dynamic>();
                                if (sharedSchemaRecords.Count() > 0)
                                {
                                    string schemaRecordIds = null;

                                    foreach (var item in sharedSchemaRecords)
                                    {
                                        if (schemaRecordIds == null) schemaRecordIds = item.SchemaRecordId;
                                        else schemaRecordIds += "," + item.SchemaRecordId;
                                    }
                                    getSharedValues1 = await _entityService.SerachFields(userId, schemaName, fields, values, mode.ToString(), page, limit, null, new List<bool> { sortDesc }, sortField, groupBy, schemaRecordIds, companySiteId);
                                    listResult1.AddRange(getSharedValues1.Data);
                                }

                                //pegar campos de segmentação e valores buscados e owner (obs: concatenar todos os valores com vírgula ex: status,nome,owner)
                                // pegar valor de segmentação e valores buscados e owner (obs: concatenar todos os valores com vírgula ex: ativo,matheus,id)

                                fields += "owners";
                                values += permissionUser.UserID;

                                dynamic getSearch = await _entityService.SerachFields(userId, schemaName, fields, values, mode.ToString(), page, limit, null, new List<bool> { sortDesc }, sortField, groupBy, ids, companySiteId);
                                listResult1.AddRange(getSearch.Data);
                                object getSearchOwnerConv1 = null;
                                if (sharedSchemaRecords.Count > 0)
                                {
                                    listResult1 = listResult1.Distinct().ToList();
                                    getSearchOwnerConv1 = _userService.ConvertOwnerId(new PaginationData<object>(listResult1, page, limit, getSharedValues1.Total + getSearch.Total), userField);
                                    return ResponseDefault(getSearchOwnerConv1);
                                }
                                else
                                {
                                    getSearchOwnerConv1 = _userService.ConvertOwnerId(getSearch, userField);
                                    return ResponseDefault(getSearchOwnerConv1);
                                }
                            }
                        }

                        //items compartilhados somente owner
                        var sharedSchemaRecordsList = _sharedSchemaRecordService.GetBySchemaNameAndUserIdReceive(schemaName, userId);
                        dynamic getSharedValues = null;
                        List<object> listResult = new List<object>();
                        if (sharedSchemaRecordsList.Count() > 0)
                        {
                            string schemaRecordIds = null;

                            foreach (var item in sharedSchemaRecordsList)
                            {
                                if (schemaRecordIds == null) schemaRecordIds = item.SchemaRecordId;
                                else schemaRecordIds += "," + item.SchemaRecordId;
                            }
                            getSharedValues = await _entityService.SerachFields(userId, schemaName, fields, values, mode.ToString(), page, limit, null, new List<bool> { sortDesc }, sortField, groupBy, schemaRecordIds, companySiteId);
                            listResult.AddRange(getSharedValues.Data);
                        }

                        //senao tem acesso segmentado
                        //pega valores buscados e owner

                        fields += "owners";
                        values += permissionUser.UserID;
                        dynamic getOwnersValues = await _entityService.SerachFields(userId, schemaName, fields, values, mode.ToString(), page, limit, null, new List<bool> { sortDesc }, sortField, groupBy, ids, companySiteId);
                        listResult.AddRange(getOwnersValues.Data);
                        object getSearchOwnerConv = null;
                        if (sharedSchemaRecordsList.Count > 0)
                        {
                            listResult = listResult.Distinct().ToList();
                            getSearchOwnerConv = _userService.ConvertOwnerId(new PaginationData<object>(listResult, page, limit, getSharedValues.Total + getOwnersValues.Total), userField);
                            return ResponseDefault(getSearchOwnerConv);
                        }
                        else
                        {
                            getSearchOwnerConv = _userService.ConvertOwnerId(getOwnersValues, userField);
                            return ResponseDefault(getSearchOwnerConv);
                        }
                    }
                    //senão tem acesso a owner
                    //verificar se tem acesso segmentado.
                    if (permissaoReferenteAoSchema.Segmentations != null)
                    {
                        if (permissaoReferenteAoSchema.Segmentations.Count > 0)
                        {
                            foreach (var s in permissaoReferenteAoSchema.Segmentations)
                            {
                                if (s.values.Count > 1)
                                {
                                    values += "[" + string.Join(",", s.values) + "]" + ",";
                                    fields += "[" + s.field + "],";
                                }
                                else
                                {
                                    values += "[" + s.values[0] + "],";
                                    fields += "[" + s.field + "],";
                                }
                            }

                            //pegar valores de segmentação e valores buscados.
                            values = values.Substring(0, values.Length - 1);
                            fields = fields.Substring(0, fields.Length - 1);

                            var getSegValues = await _entityService.SerachFields(userId, schemaName, fields, values, mode.ToString(), page, limit, null, new List<bool> { sortDesc }, sortField, groupBy, ids, companySiteId);
                            var getSegConv = _userService.ConvertOwnerId(getSegValues, userField);
                            return ResponseDefault(getSegConv);
                        }
                    }

                    //senão tem acesso segmentado
                    // pegar valores buscados
                    if (values != "")
                    {
                        values = values.Substring(0, values.Length - 1);
                    }
                    if (fields != "")
                    {
                        fields = fields.Substring(0, fields.Length - 1);
                    }

                    var getSearchBuscado = await _entityService.SerachFields(userId, schemaName, fields, values, mode.ToString(), page, limit, null, new List<bool> { sortDesc }, sortField, groupBy, ids, companySiteId);
                    var getBuscadoConv = _userService.ConvertOwnerId(getSearchBuscado, userField);
                    return ResponseDefault(getBuscadoConv);
                }
            }

            //senao tem permisão read nem item compartilhado
            // response default

            return BadRequest($"Você não possui acesso ao schema {schemaName}");

            // se usuario tem permissão de Read no schema.
            //se tem permissão verificar se tem owner.
            //verificar se tem acesso segmentado.
            //pegar campos de segmentação e valores buscados e owner (obs: concatenar todos os valores com vírgula ex: status,nome,owner)
            // pegar valor de segmentação e valores buscados e owner (obs: concatenar todos os valores com vírgula ex: ativo,matheus,id)

            //senao tem acesso segmentado
            //pega valores buscados e owner
            //senão tem acesso a owner
            //verificar se tem acesso segmentado.
            //pegar valores de segmentação e valores buscados.
            //senão tem acesso segmentado
            // pegar valores buscados
            //senao tem permisão read
            // response default
        }

        /// <summary>
        /// Search Items
        /// </summary>
        /// <returns>Return result paginated</returns>
        /// <response code="200">Return result paginated</response>
        [HttpPost("{schemaName}/schemas-join")]
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> SchemasJoin(string schemaName, [FromBody] JoinsModel model)
        {
            var acesstoken = Request.Headers[HeaderNames.Authorization];
            var userId = _userService.ClaimUserId(acesstoken);
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            if (schema == null) return ResponseDefault();
            var json = schema.JsonValue;
            var permissionUser = _permissionService.RetornPermission(acesstoken.ToString());
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(json);

            string userField = null;
            var count = schemaModel.Properties.Where(x => x.Value.Type == "user").Count();
            if (count > 0)
            {
                userField = schemaModel.Properties.Where(x => x.Value.Type == "user").FirstOrDefault().Key;
            }
            var modelSchemas = model.Joins.Select(x => x.Schema).ToList();
            // se usuario tem permissão de Read no schema.
            var permissaoReferenteAoSchema = permissionUser.Schemas?.Where(x => x.SchemaName == schemaName).FirstOrDefault();
            var permissaoReferenteAosSchemas = permissionUser.Schemas?.Where(x => modelSchemas.Contains(x.SchemaName) && x.Permissions.Contains("Read")).Select(x => x.SchemaName).ToList();

            if (permissaoReferenteAoSchema != null)
            {
                var isPermissionRead = false;
                //var isPermissionOwner = false;
                if (permissaoReferenteAoSchema.Permissions.Contains("Read")) isPermissionRead = true;

                if (isPermissionRead == true)
                {
                    dynamic getSearchBuscado = null;
                    //model for joinSearch
                    var newWhereModel = new JoinsModel
                    {
                        Page = model.Page,
                        Limit = model.Limit,
                        Joins = model.Joins,
                        Order = model.Order,
                        Where = !model.Where.IsNullOrEmpty() ? model.Where.Where(x => x.Field.Contains(".")).ToList() : model.Where,
                        WhereOr = !model.WhereOr.IsNullOrEmpty() ? model.WhereOr.Where(x => x.Field.Contains(".")).ToList() : model.WhereOr
                    };
                    if (!model.Joins.IsNullOrEmpty()) getSearchBuscado = await _entityService.SerachFieldsJoin(schemaName, model, 1, 999999);
                    bool sortDesc = false;
                    string? sortfield = null;
                    if (model.Order != null)
                    {
                        if (model.Order.Field != null) sortfield = model.Order.Field;
                        if (model.Order.Sort == "desc") sortDesc = true;
                    }
                    getSearchBuscado = await _entityService.SerachFieldsJoin(schemaName, model, model.Page, model.Limit, sortfield, sortDesc);
                    return ResponseDefault(_userService.ConvertOwnerId(getSearchBuscado, userField));
                    //if (model.Joins.IsNullOrEmpty() && (model.Order == null || model.Order.Sort == null || model.Order.Field == null)) return ResponseDefault(getBuscadoConv);
                    //List<dynamic> items = new List<dynamic>();
                    //if (model.Order != null && model.Order.Field != null && model.Order.Sort != null)
                    //{
                    //    items.AddRange(getBuscadoConv.Data);

                    //    if (model.Order.Sort == "desc")
                    //    {
                    //        items = items.OrderByDescending(item => GetPropertyValue(item, model.Order.Field)).ToList();
                    //    }
                    //    else
                    //    {
                    //        items = items.OrderBy(item => GetPropertyValue(item, model.Order.Field)).ToList();
                    //    }

                    //}

                    //int skipCount = (int)((model.Page - 1) * model.Limit);
                    //int takeCount = (int)model.Limit;
                    //IEnumerable<object> enumerableList = items;
                    //var limitedList = enumerableList.Skip(skipCount).Take(takeCount).ToList();

                    //return ResponseDefault(new PaginationData<object>(limitedList, model.Page, model.Limit, enumerableList.Count()));
                }
            }
            //senao tem permisão read
            // response default

            return BadRequest($"Você não possui acesso ao schema {schemaName}");
        }

        public static object GetPropertyValue(IDictionary<string, object> expandoObject, string propertyName)
        {
            // Check if the property exists as a standalone field
            var properties = propertyName.Split('.');
            var value = expandoObject;
            foreach (var property in properties)
            {
                if (value.TryGetValue(property, out var nestedValue))
                {
                    if (nestedValue is List<IDictionary<string, object>> nestedObject)
                    {
                        nestedObject[0].TryGetValue(properties[1], out var arrayfieldValue);

                        return arrayfieldValue;
                    }
                    else
                    {
                        return nestedValue; // If it's a single value, return it
                    }
                }
                else
                {
                    // If any part of the property path doesn't exist, return null or an appropriate default value
                    return null;
                }
            }
            return value; // If it's an object (nested property), return the last nested object
        }

        /// <summary>
        /// Search Items
        /// </summary>
        /// <returns>Return result paginated</returns>
        /// <response code="200">Return result paginated</response>
        [HttpGet("{schemaName}/search-items")]
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> SearchItem(string schemaName, string relationSchema, string value, string id, int? page, int? limit, string sortField = null, bool sortDesc = false, string searchFields = null)
        {
            if (searchFields == null) searchFields = string.Empty;
            string fields = searchFields;
            string values = value;
            var acesstoken = Request.Headers[HeaderNames.Authorization];
            var userId = _userService.ClaimUserId(acesstoken);
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            if (schema == null) return ResponseDefault();
            var json = schema.JsonValue;
            var permissionUser = _permissionService.RetornPermission(acesstoken.ToString());
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(json);
            var property = schemaModel.Properties.FirstOrDefault(x => x.Value.RelationSchema != null && x.Value.RelationSchema.Equals(relationSchema, StringComparison.OrdinalIgnoreCase));
            string arrayField = property.Equals(default(KeyValuePair<string, Infra.Domain.Models.SchemaModelProperty>)) ? null : property.Key;
            if (arrayField == null) return ResponseDefault(new PaginationData<object>(new List<object>() { }, page, limit, 0));
            string ids = await _entityService.GetItems(schemaName, arrayField, id);
            if (ids == null) return ResponseDefault(new PaginationData<object>(new List<object>() { }, page, limit, 0));
            string userField = null;
            var count = schemaModel.Properties.Where(x => x.Value.Type == "user").Count();
            if (count > 0)
            {
                userField = schemaModel.Properties.Where(x => x.Value.Type == "user").FirstOrDefault().Key;
            }

            // se usuario tem permissão de Read no schema.
            var permissaoReferenteAoSchema = permissionUser.Schemas?.Where(x => x.SchemaName == relationSchema).FirstOrDefault();
            if (permissaoReferenteAoSchema != null)
            {
                var isPermissionRead = false;
                //var isPermissionOwner = false;

                foreach (var p in permissionUser.Schemas)
                {
                    if (p.SchemaName == schemaName)
                    {
                        if (p.Permissions.Contains("Read"))
                        {
                            isPermissionRead = true;
                        }
                        //if (p.Permissions.Contains("Owner"))
                        //{
                        //    isPermissionOwner = true;
                        //}
                    }
                }
                if (isPermissionRead == true)
                {
                    var getSearchBuscado = await _entityService.SerachFields(userId, relationSchema, fields, values, "", page, limit, null, new List<bool> { sortDesc }, sortField, null, ids);
                    var getBuscadoConv = _userService.ConvertOwnerId(getSearchBuscado, userField);
                    return ResponseDefault(getBuscadoConv);
                }
            }
            //senao tem permisão read
            // response default

            return BadRequest($"Você não possui acesso ao schema {schemaName}");
        }

        /// <summary>
        /// Get results based on search field defs
        /// </summary>
        /// <returns>Return result paginated</returns>
        /// <response code="200">Return result paginated</response>
        [HttpPost("search-fieldsDefs")]
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> SearchFieldsDefs(string idSearchDefs, string schemaNameDef, string value, [FromBody] Searchdefs.Def defs, int? page, int? limit, string searchFields = null, string companySiteId = null)
        {
            var searchDef = _searchdefsService.GetSearchdefsById(idSearchDefs);
            if (searchDef == null)
            {
                searchDef = new Searchdefs
                {
                    SchemaName = schemaNameDef,
                    Defs = defs
                };
            }
            var fields = "";
            var values = "";
            var schemaName = "";
            var acesstoken = Request.Headers[HeaderNames.Authorization];
            var userId = _userService.ClaimUserId(acesstoken);
            var schema = _schemaRepository.GetSchemaByField("name", searchDef.SchemaName);
            if (schema == null) return ResponseDefault();
            else
                schemaName = searchDef.SchemaName;
            var json = schema.JsonValue;
            var permissionUser = _permissionService.RetornPermission(acesstoken.ToString());
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(json);

            // companySite

            var companySiteIds = _userService.ClaimCompanySiteIds(acesstoken);

            if (schemaModel.Multi_Company && Request.Headers.ContainsKey("companySiteId"))
            {
                var companyHeaderId = Request.Headers["companySiteId"].ToString();
                if (!string.IsNullOrEmpty(companyHeaderId))
                {
                    if (!string.IsNullOrEmpty(companySiteId)) companySiteId += "," + companyHeaderId;
                    else companySiteId = companyHeaderId;
                }
            }
            if (!string.IsNullOrEmpty(companySiteId) && schemaModel.Multi_Company)
            {
                var companySiteIdsSplit = companySiteId.Split(',');
                bool containsAll = companySiteIdsSplit.All(id => companySiteIds.Contains(id));
                if (!containsAll)
                {
                    return ResponseDefault("Usuario não tem permissão para company");
                }
            }
            else if (!string.IsNullOrEmpty(companySiteId)) return ResponseDefault("Usuario não tem permissão para company");

            string userField = null;
            var count = schemaModel.Properties.Where(x => x.Value.Type == "user").Count();
            if (count > 0)
            {
                userField = schemaModel.Properties.Where(x => x.Value.Type == "user").FirstOrDefault().Key;
            }

            List<Filter> filtros = new();
            for (int i = 0; i < searchDef.Defs.filters.Count; i++)
            {
                filtros.Add(searchDef.Defs.filters[i]);
            }
            string orderField = null;
            List<bool> orderByList = new List<bool>();

            if (searchDef.Defs.orderBy.Count > 0)
            {
                foreach (var filter in searchDef.Defs.orderBy)
                {
                    if (orderField == null) orderField = filter.field;
                    else orderField += "," + filter.field;
                    if (filter.direction == "desc") orderByList.Add(true);
                    else orderByList.Add(false);
                }
            }

            // se usuario tem permissão de Read no schema.
            var permissaoReferenteAoSchema = permissionUser.Schemas.Where(x => x.SchemaName == schemaName).FirstOrDefault();
            if (permissaoReferenteAoSchema != null)
            {
                var isPermissionRead = false;
                var isPermissionOwner = false;

                foreach (var p in permissionUser.Schemas)
                {
                    if (p.SchemaName == schemaName)
                    {
                        if (p.Permissions.Contains("Read"))
                        {
                            isPermissionRead = true;
                        }
                        if (p.Permissions.Contains("Owner"))
                        {
                            isPermissionOwner = true;
                        }
                    }
                }

                if (isPermissionRead == true)
                {   //se tem permissão verificar se tem owner.
                    if (searchFields != null && value != null)
                    {
                        fields += "[" + searchFields + "],";
                        values += "[" + value + "],";
                    }
                    if (searchFields == null && value != null)
                    {
                        searchFields = await _entityService.GetListSearchFields(schemaName);

                        fields += "[" + searchFields + "],";
                        values += "[" + value + "],";
                    }

                    if (isPermissionOwner == true)
                    {
                        //verificar se tem acesso segmentado.
                        if (permissaoReferenteAoSchema.Segmentations != null)
                        {
                            if (permissaoReferenteAoSchema.Segmentations.Count > 0)
                            {
                                foreach (var s in permissaoReferenteAoSchema.Segmentations)
                                {
                                    if (s.values.Count > 1)
                                    {
                                        values += "[" + string.Join(",", s.values) + "]" + ",";
                                        fields += "[" + s.field + "],";
                                    }
                                    else
                                    {
                                        values += "[" + s.values[0] + "],";
                                        fields += "[" + s.field + "],";
                                    }
                                }

                                //pegar campos de segmentação e valores buscados e owner (obs: concatenar todos os valores com vírgula ex: status,nome,owner)
                                // pegar valor de segmentação e valores buscados e owner (obs: concatenar todos os valores com vírgula ex: ativo,matheus,id)

                                fields += "owners";
                                values += permissionUser.UserID;

                                var getSearch = await _entityService.SerachFields(userId, schemaName, fields, values, "", page, limit, filtros, orderByList, orderField, null, null, companySiteId);
                                var getSearchOwner = _userService.ConvertOwnerId(getSearch, userField);
                                return ResponseDefault(getSearchOwner);
                            }
                        }

                        //senao tem acesso segmentado
                        //pega valores buscados e owner
                        fields += "owners";
                        values += permissionUser.UserID;
                        var getOwnersValues = await _entityService.SerachFields(userId, schemaName, fields, values, "", page, limit, filtros, orderByList, orderField, null, null, companySiteId);
                        var getSearchOwnerConv = _userService.ConvertOwnerId(getOwnersValues, userField);
                        return ResponseDefault(getSearchOwnerConv);
                    }
                    //senão tem acesso a owner
                    //verificar se tem acesso segmentado.
                    if (permissaoReferenteAoSchema.Segmentations != null)
                    {
                        if (permissaoReferenteAoSchema.Segmentations.Count > 0)
                        {
                            foreach (var s in permissaoReferenteAoSchema.Segmentations)
                            {
                                if (s.values.Count > 1)
                                {
                                    values += "[" + string.Join(",", s.values) + "]" + ",";
                                    fields += "[" + s.field + "],";
                                }
                                else
                                {
                                    values += "[" + s.values[0] + "],";
                                    fields += "[" + s.field + "],";
                                }
                            }

                            //pegar valores de segmentação e valores buscados.
                            values = values.Substring(0, values.Length - 1);
                            fields = fields.Substring(0, fields.Length - 1);

                            var getSegValues = await _entityService.SerachFields(userId, schemaName, fields, values, "", page, limit, filtros, orderByList, orderField, null, null, companySiteId);
                            var getSegConv = _userService.ConvertOwnerId(getSegValues, userField);
                            return ResponseDefault(getSegConv);
                        }
                    }

                    //senão tem acesso segmentado
                    // pegar valores buscados
                    if (values != "")
                    {
                        values = values.Substring(0, values.Length - 1);
                    }
                    if (fields != "")
                    {
                        fields = fields.Substring(0, fields.Length - 1);
                    }

                    var getSearchBuscado = await _entityService.SerachFields(userId, schemaName, fields, values, "", page, limit, filtros, orderByList, orderField, null, null, companySiteId);
                    var getBuscadoConv = _userService.ConvertOwnerId(getSearchBuscado, userField);
                    return ResponseDefault(getSearchBuscado);
                }
            }

            //senao tem permisão read
            // response default

            return BadRequest($"Você não possui acesso ao schema {schemaName}");

            //var search = _entityService.SerachFields(searchDef.SchemaName,null,null,page,limit,filtros,orderField,orderBy,null);
            //return ResponseDefault( await search);
        }

        /// <summary>
        /// Get all from schemaname
        /// </summary>
        /// <returns>Return result paginated</returns>
        /// <response code="200">Return result paginated</response>
        [HttpGet("{schemaName}")]
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> GetAll(string schemaName, int? page, int? limit, string sortField = null, bool sortDesc = false, string ids = null, string companySiteId = null)
        {
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            if (schema == null) return ResponseDefault();
            var json = schema.JsonValue;
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(json);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true && schemaModel.Alias != null)
            {
                var result = await SQLServerService.GetList(schemaModel.Alias, page, limit, sortField, sortDesc, ids, null, null, source, SearchModeEnum.Equals, schemaModel.CompanySiteId, companySiteId);
                var resultReturn = new
                {
                    result.data,
                    result.total,
                    page,
                    limit,
                    pages = limit != null ? (int)Math.Ceiling((double)result.total / limit.Value) : 0
                };

                return ResponseDefault(resultReturn);
            }

            var fields = "";
            var values = "";
            var acesstoken = Request.Headers[HeaderNames.Authorization];
            var userId = _userService.ClaimUserId(acesstoken);
            var permissionUser = _permissionService.RetornPermission(acesstoken.ToString());
            var companySiteIds = _userService.ClaimCompanySiteIds(acesstoken);
            if (schemaModel.Multi_Company && Request.Headers.ContainsKey("companySiteId"))
            {
                var companyHeaderId = Request.Headers["companySiteId"].ToString();
                if (!string.IsNullOrEmpty(companyHeaderId))
                {
                    if (!string.IsNullOrEmpty(companySiteId)) companySiteId += "," + companyHeaderId;
                    else companySiteId = companyHeaderId;
                }
            }
            if (!string.IsNullOrEmpty(companySiteId) && schemaModel.Multi_Company)
            {
                var companySiteIdsSplit = companySiteId.Split(',');
                bool containsAll = companySiteIdsSplit.All(id => companySiteIds.Contains(id));
                if (!containsAll)
                {
                    return ResponseDefault("Usuario não tem permissão para company");
                }
            }
            else if (!string.IsNullOrEmpty(companySiteId)) return ResponseDefault("Usuario não tem permissão para company");
            string userField = null;
            var count = schemaModel.Properties.Where(x => x.Value.Type == "user").Count();
            if (count > 0)
            {
                userField = schemaModel.Properties.Where(x => x.Value.Type == "user").FirstOrDefault().Key;
            }

            // se usuario tem permissão de Read no schema.
            var permissaoReferenteAoSchema = permissionUser.Schemas?.Where(x => x.SchemaName == schemaName).FirstOrDefault();
            if (permissaoReferenteAoSchema != null)
            {
                var isPermissionRead = false;
                var isPermissionOwner = false;

                foreach (var p in permissionUser.Schemas)
                {
                    if (p.SchemaName == schemaName)
                    {
                        if (p.Permissions.Contains("Read"))
                        {
                            isPermissionRead = true;
                        }
                        if (p.Permissions.Contains("Owner"))
                        {
                            isPermissionOwner = true;
                        }
                    }
                }
                if (isPermissionRead == true)
                {   //se tem permissão verificar se tem owner.
                    if (isPermissionOwner == true)
                    {
                        //verificar se tem acesso segmentado.
                        if (permissaoReferenteAoSchema.Segmentations != null)
                        {
                            if (permissaoReferenteAoSchema.Segmentations.Count > 0)
                            {
                                foreach (var s in permissaoReferenteAoSchema.Segmentations)
                                {
                                    if (s.values.Count > 1)
                                    {
                                        values += "[" + string.Join(",", s.values) + "]" + ",";
                                        fields += "[" + s.field + "],";
                                    }
                                    else
                                    {
                                        values += "[" + s.values[0] + "],";
                                        fields += "[" + s.field + "],";
                                    }
                                }
                                //items compartilhados somente owner
                                var sharedSchemaRecords = _sharedSchemaRecordService.GetBySchemaNameAndUserIdReceive(schemaName, userId);
                                dynamic getSharedValuesOwner = null;
                                List<object> listResultOwner = new List<object>();
                                if (sharedSchemaRecords.Count() > 0)
                                {
                                    string schemaRecordIds = null;

                                    foreach (var item in sharedSchemaRecords)
                                    {
                                        if (schemaRecordIds == null) schemaRecordIds = item.SchemaRecordId;
                                        else schemaRecordIds += "," + item.SchemaRecordId;
                                    }
                                    getSharedValuesOwner = await _entityService.SerachFields(userId, schemaName, fields, values, "", page, limit, null, new List<bool> { sortDesc }, sortField, null, schemaRecordIds, companySiteId);
                                    listResultOwner.AddRange(getSharedValuesOwner.Data);
                                }

                                //pegar campos de segmentação e valores buscados e owner (obs: concatenar todos os valores com vírgula ex: status,nome,owner)
                                // pegar valor de segmentação e valores buscados e owner (obs: concatenar todos os valores com vírgula ex: ativo,matheus,id)

                                fields += "owners";
                                values += permissionUser.UserID;

                                dynamic getSearch1 = await _entityService.SerachFields(userId, schemaName, fields, values, "", page, limit, null, new List<bool> { sortDesc }, sortField, null, ids, companySiteId);

                                listResultOwner.AddRange(getSearch1.Data);
                                object getSearchOwnerConvList1 = null;
                                if (sharedSchemaRecords.Count > 0)
                                {
                                    listResultOwner = listResultOwner.Distinct().ToList();
                                    getSearchOwnerConvList1 = _userService.ConvertOwnerId(new PaginationData<object>(listResultOwner, page, limit, getSharedValuesOwner.Total + getSearch1.Total), userField);
                                    return ResponseDefault(getSearchOwnerConvList1);
                                }
                                else
                                {
                                    getSearchOwnerConvList1 = _userService.ConvertOwnerId(getSearch1, userField);
                                    return ResponseDefault(getSearchOwnerConvList1);
                                }
                            }
                        }
                        //items compartilhados somente owner
                        var sharedSchemaRecordsList = _sharedSchemaRecordService.GetBySchemaNameAndUserIdReceive(schemaName, userId);
                        dynamic getSharedValues = null;
                        List<object> listResult = new List<object>();
                        if (sharedSchemaRecordsList.Count() > 0)
                        {
                            string schemaRecordIds = null;

                            foreach (var item in sharedSchemaRecordsList)
                            {
                                if (schemaRecordIds == null) schemaRecordIds = item.SchemaRecordId;
                                else schemaRecordIds += "," + item.SchemaRecordId;
                            }
                            getSharedValues = await _entityService.SerachFields(userId, schemaName, fields, values, "", page, limit, null, new List<bool> { sortDesc }, sortField, null, schemaRecordIds, companySiteId);
                            listResult.AddRange(getSharedValues.Data);
                        }

                        //senao tem acesso segmentado
                        //pega valores buscados e owner e itensCompartilhados

                        fields += "owners";
                        values += permissionUser.UserID;

                        dynamic getSearch = await _entityService.SerachFields(userId, schemaName, fields, values, "", page, limit, null, new List<bool> { sortDesc }, sortField, null, ids, companySiteId);

                        listResult.AddRange(getSearch.Data);
                        object getSearchOwnerConvList = null;
                        if (sharedSchemaRecordsList.Count > 0)
                        {
                            listResult = listResult.Distinct().ToList();
                            getSearchOwnerConvList = _userService.ConvertOwnerId(new PaginationData<object>(listResult, page, limit, getSharedValues.Total + getSearch.Total), userField);
                            return ResponseDefault(getSearchOwnerConvList);
                        }
                        else
                        {
                            getSearchOwnerConvList = _userService.ConvertOwnerId(getSearch, userField);
                            return ResponseDefault(getSearchOwnerConvList);
                        }
                    }
                    //senão tem acesso a owner
                    //verificar se tem acesso segmentado.
                    if (permissaoReferenteAoSchema.Segmentations != null)
                    {
                        if (permissaoReferenteAoSchema.Segmentations.Count > 0)
                        {
                            foreach (var s in permissaoReferenteAoSchema.Segmentations)
                            {
                                if (s.values.Count > 1)
                                {
                                    values += "[" + string.Join(",", s.values) + "]" + ",";
                                    fields += "[" + s.field + "],";
                                }
                                else
                                {
                                    values += "[" + s.values[0] + "],";
                                    fields += "[" + s.field + "],";
                                }
                            }

                            //pegar valores de segmentação e valores buscados.

                            values = values.Substring(0, values.Length - 1);
                            fields = fields.Substring(0, fields.Length - 1);

                            var getSegValues = await _entityService.SerachFields(userId, schemaName, fields, values, "", page, limit, null, new List<bool> { sortDesc }, sortField, null, ids, companySiteId);
                            var getSegConv = _userService.ConvertOwnerId(getSegValues, userField);
                            return ResponseDefault(getSegValues);
                        }
                    }

                    //senão tem acesso segmentado
                    // pegar valores buscados
                    var getSearchBuscado = await _entityService.SerachFields(userId, schemaName, fields, values, "", page, limit, null, new List<bool> { sortDesc }, sortField, null, ids, companySiteId);
                    var getBuscadoConv = _userService.ConvertOwnerId(getSearchBuscado, userField);
                    return ResponseDefault(getBuscadoConv);
                }
            }
            //senao tem permisão read
            // response default

            return BadRequest($"Você não possui acesso ao schema {schemaName}");
        }

        ///// <summary>
        ///// Get value auto increment from  a field.
        ///// </summary>
        ///// <returns>Return Dictionary string</returns>
        ///// <response code="200">Return string Dictionary(field,autoIncValue)</response>
        //[HttpGet("AutoInc")]
        //[ExcludeFromCodeCoverage]
        //public async Task<Dictionary<string,string>> GetAutoInc(string schemaName,string fieldAutoInc) {
        //    var fields = fieldAutoInc.Split(",");

        //    return await _entityService.GetAutoinc(schemaName,fields);

        //}

        /// <summary>
        /// Get value auto increment from  a field
        /// </summary>
        /// <returns>Return a list from Generators</returns>
        /// <response code="200">Return a list from Generators</response>
        [HttpGet("AutoInc")]
        public async Task<IActionResult> GetAutoincAsync(string schemaName, string field, string mask)
        {
            if (schemaName == null || field == null) return BadRequest("schemaName e field não podem ser nulos");

            return Ok(await _generatorsService.GetAutoincAsync(schemaName, field, mask));
        }

        /// <summary>
        /// Get result from schemaname and id
        /// </summary>
        /// <returns>Return result paginated</returns>
        /// <response code="200">Return result paginated</response>
        [HttpGet("{schemaName}/{id}")]
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> GetById(string schemaName, string id)
        {
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            if (schema == null) return ResponseDefault();
            var json = schema.JsonValue;
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(json);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true && schemaModel.Alias != null)
            {

                var result = await SQLServerService.GetById(schemaModel.Alias, schemaModel.PrimaryKey, id, source);
                var resultReturn = result.data;
                return ResponseDefault(resultReturn);
            }

            string userField = null;
            var count = schemaModel.Properties.Where(x => x.Value.Type == "user").Count();
            if (count > 0)
            {
                userField = schemaModel.Properties.Where(x => x.Value.Type == "user").FirstOrDefault().Key;
            }
            var item = await _entityService.GetById(schemaName, id);
            if (item == null) return ResponseDefault("Não encontrado");
            var getSearchConvert = _userService.ConvertOwnerIdSingle(item, userField);
            return ResponseDefault(getSearchConvert);
        }

        [HttpGet("WeeklySchedule")]
        public async Task<IActionResult> GetSchedule(string? fornecedorId, DateTime? dataInicio, DateTime? dataFim, int? page = null, int? limit = null, string? sortField = null, bool sortDesc = false)
        {
            var acesstoken = Request.Headers[HeaderNames.Authorization];
            var permissionUser = _permissionService.RetornPermission(acesstoken.ToString());
            string? ownerId = permissionUser.UserID;
            var schedules = await _entityService.GetAllWeeklySchedule("Weeklyschedule", fornecedorId, ownerId, dataInicio, dataFim, sortField, sortDesc);
            if (page == null) page = 1;
            if (limit == null) limit = 30;
            int skipCount = (int)((page - 1) * limit);
            var limitedList = schedules.Skip(skipCount).Take((int)limit).ToList();
            var usuarioIds = limitedList.SelectMany(x => x.Schedules.Select(x => x.UsuarioId)).ToList();
            if (!usuarioIds.IsNullOrEmpty())
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
                var usuarios = _userService.GetUsersByUserIds(usuarioIds, tenanty);
                if (usuarios.Count > 0)
                {
                    var fornecedorIds = limitedList.SelectMany(x => x.Schedules).Select(x => x.FornecedorId).Distinct().ToList();
                    var accounts = await _entityService.GetByIdsList("Account", fornecedorIds);
                    List<AccountModel> accountsList = new List<AccountModel>();

                    if (accounts != null && accounts.Count > 0)
                    {
                        var jsonAccounts = JsonConvert.SerializeObject(accounts);
                        accountsList = JsonConvert.DeserializeObject<List<AccountModel>>(jsonAccounts);
                    }

                    var localCityIds = accountsList.Select(x => x.LocalCity).ToList();
                    localCityIds.RemoveAll(string.IsNullOrWhiteSpace);
                    var cities = await _entityService.GetByIdsList("LocalCity", localCityIds);

                    List<LocalCityModel> localCityList = new List<LocalCityModel>();
                    if (cities != null && cities.Count > 0)
                    {
                        var jsonLocalCities = JsonConvert.SerializeObject(cities);
                        localCityList = JsonConvert.DeserializeObject<List<LocalCityModel>>(jsonLocalCities);
                    }
                    var statesList = new List<LocalStateModel>();

                    var localStateIds = accountsList.Select(x => x.LocalState).ToList();
                    var states = await _entityService.GetByIdsList("LocalState", localStateIds);
                    var jsonLocalStates = JsonConvert.SerializeObject(states);
                    statesList = JsonConvert.DeserializeObject<List<LocalStateModel>>(jsonLocalStates);

                    foreach (var model in limitedList)
                    {
                        foreach (var schedule in model.Schedules)
                        {
                            if (schedule.UsuarioId != null)
                            {
                                schedule.UsuarioNome = usuarios.Where(x => x.Id == schedule.UsuarioId).FirstOrDefault().Name;
                                model.UsuarioNome = schedule.UsuarioNome;
                            }
                            if (schedule.FornecedorNome != null)
                            {
                                var fornecedorNomeSplit = schedule.FornecedorNome.Split(" ");
                                if (fornecedorNomeSplit.Count() > 2)
                                {
                                    if (fornecedorNomeSplit[2].Length > 2)
                                    {
                                        schedule.FornecedorNome = $"{fornecedorNomeSplit[0]} {fornecedorNomeSplit[1]} {fornecedorNomeSplit[2]}";
                                    }
                                    else
                                    {
                                        schedule.FornecedorNome = $"{fornecedorNomeSplit[0]} {fornecedorNomeSplit[1]}";
                                    }
                                }
                            }

                            var currentAccount = accountsList.Where(x => x.Id == schedule.FornecedorId).FirstOrDefault();
                            if (currentAccount != null)
                            {
                                var localCity = localCityList.Where(x => x.Id == currentAccount.LocalCity).FirstOrDefault();

                                if (localCity != null)
                                {
                                    if (currentAccount.LocalState != null)
                                    {
                                        var localState = statesList.Where(x => x.Id == currentAccount.LocalState).FirstOrDefault();
                                        schedule.FornecedorNome += !string.IsNullOrEmpty(localState.Code) ? $"/{localCity.Description}-{localState.Code}" : $"/{localCity.Description}";
                                    }
                                    else if (localCity.LocalState != null)
                                    {
                                        var localState = statesList.Where(x => x.Id == localCity.LocalState).FirstOrDefault();
                                        schedule.FornecedorNome += !string.IsNullOrEmpty(localState.Code) ? $"/{localCity.Description}-{localState.Code}" : $"/{localCity.Description}";
                                    }
                                }
                                //else if (localCity == null && currentAccount.LocalState != null)
                                //{
                                //    var localState = statesList.Where(x => x.Id == currentAccount.LocalState).FirstOrDefault();
                                //    schedule.FornecedorNome += $" {localState.Code}";
                                //}
                            }
                        }
                    }
                }
            }
            return ResponseDefault(new PaginationData<dynamic>(limitedList, page, limit, schedules.Count()));
        }

        [ExcludeFromCodeCoverage]
        private async Task RunActionForSchema(string schemaName, string entityId = "")
        //#pragma warning restore CS8632
        {
            ActionController x = new(null, null, _schemaRepository, _entityService, _actionRepository, null, _userService);
            ApiEntities.Action[] ac = x.GetBySchemaName(schemaName);

            var userHelper = _schemaRepository.GetUserHelper();
            var user = _userService.GetByUserName(userHelper.GetTenanty(), userHelper.GetUserName());

            string auth = Request.Headers["Authorization"];

            object entity = null;
            if (!string.IsNullOrEmpty(entityId))
            {
                entity = await _entityService.GetById(schemaName, entityId);
                if (entity == null)
                {
                    SendNotification("Run", "Entity not found by the provided entityId: " + entityId);
                }
            }

            foreach (var action in ac)
            {
                if (action.CallType == ApiEntities.ActionCallType.EventSchema)
                    await x.Run(action, new Dictionary<string, object>()
                    {
                        { "params", null }
                    }, auth, user, entity);
            }
        }

        /// <summary>
        /// Update entity object
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPut("{schemaName}")]
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> Update(string schemaName, string latitude, string longitude, string ip, string userAgent, string description, [FromBody] Dictionary<string, object> data)
        {
            if (data.Count == 0) return BadRequest();

            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            if (schema == null) return ResponseDefault();
            var json = schema.JsonValue;
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(json);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true && schemaModel.Alias != null)
            {
                //pega a chave primaria configurada
                var key = schemaModel.PrimaryKey;
                if (key.IsNullOrEmpty()) return ResponseDefault("chave primaria não configurada");

                //pega o valor da chave primaria no body
                var value = data.Where(x => x.Key == key).ToList();
                if (value.IsNullOrEmpty()) return ResponseDefault("chave primaria não informada no corpo do objeto.");
                //remove do data a chave primaria
                if (data.ContainsKey(value.First().Key))
                {
                    data.Remove(value.First().Key);
                }
                //realiza o update
                var dataSql = new Dictionary<string, object>(data);
                //add mirrors sql
                var dataSqlWithMirros = await _entityService.AddMirrorsSql(schemaName, dataSql);
                if (dataSqlWithMirros == null) return BadRequest();
                var result = await SQLServerService.UpdateWithResult(schemaModel.Alias, dataSqlWithMirros, source, value.First().Key, value.First().Value);
                if (!result.success) return BadRequest(result.error);
                return ResponseDefault(result.updatedData);
            }

            var entityOld = await _entityService.GetById(schemaName, data["id"].ToString());

            var schemaModelData = JsonConvert.DeserializeObject<SchemaModel>(schema.JsonValue);

            string id = schema.Id;

            var schemaProp = JsonConvert.DeserializeObject<SchemaModel>(json).Properties;

            string quote = "\"";
            schema.JsonValue = schema.JsonValue.Replace($"{quote}enum{quote}:[", $"{quote}enum{quote}:[{quote + quote},");
            //schema.JsonValue = schema.JsonValue.Replace($"{quote}enum{quote}:[{quote},]", $"{quote}enum{quote}:[]");

            List<string> ignoreKeys = new List<string>() { "id", "createAt", "createBy", "updateAt", "updateBy", "deleteAt", "deleteBy", "isDeleted" };

            var jsonOld = JsonConvert.SerializeObject(entityOld);
            var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonOld);
            var updateAtValue = dict["UpdateAt"];
            if (updateAtValue != null && data.ContainsKey("updateAt"))
            {
                var dataAtual = (DateTime?)data["updateAt"];
                var dataAnterior = (DateTime?)updateAtValue;
                if (dataAtual != dataAnterior) return BadRequest(new { Success = false, Data = new List<dynamic>() { new { key = "Server Error", value = "Registro não pode ser gravado por conflito de versão" } } });
            }
            foreach (var key in data.Keys) if (!ignoreKeys.Contains(key) && !schemaProp.ContainsKey(key)) return BadRequest($"schema não possui campo {key}");

            var beforeSave = JsonConvert.DeserializeObject<SchemaModel>(json).BeforeSave;

            if (beforeSave != null)
            {
                var action = _actionRepository.GetByField("name", beforeSave);

                if (action != null)
                {
                    ActionController x = new(null, null, _schemaRepository, _entityService, _actionRepository, null, _userService);
                    ApiEntities.Action[] ac = x.GetBySchemaName(schemaName);

                    var userHelper = _schemaRepository.GetUserHelper();
                    var user = _userService.GetByUserName(userHelper.GetTenanty(), userHelper.GetUserName());

                    string auth = Request.Headers["Authorization"];

                    var result = await x.Run(action, new Dictionary<string, object>()
                    {
                        { "params", null }
                    }, auth, user, entityOld);
                }
            }
            var statusF = false;
            var field = "";
            foreach (var prop in schemaProp)
            {
                if (prop.Value.uniqueCompany == true && data.ContainsKey("companySiteId"))
                {
                    var FromData = data[prop.Key];

                    var fromDataCompany = data["companySiteId"];

                    dynamic existe = await _entityService.GetByFieldNameCompany(schemaName, prop.Key, FromData.ToString(), fromDataCompany.ToString());

                    statusF = prop.Value.statusF;
                    if (existe != null && existe.Id.ToString() != data["id"].ToString())
                    {
                        return BadRequest("field " + prop.Key + " already exists");
                    }
                }

                if (prop.Value.Type == "date" && data.ContainsKey(prop.Key))
                {
                    if (data[prop.Key] != null && data[prop.Key].ToString() == "") data[prop.Key] = null;
                    if (data[prop.Key] != null)
                    {
                        if (DateTime.TryParse(data[prop.Key].ToString(), out _)) data[prop.Key] = DateTime.Parse(data[prop.Key].ToString()).AddHours(3);
                        else data[prop.Key] = null;
                    }
                }
            }

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
            if (data.Keys.FirstOrDefault(key => key == "updateBy") == null) data.Add("updateBy", userName);
            var command = new UpdateEntityCommand()
            {
                Id = id,
                SchemaName = schemaName,
                SchemaJson = schema.JsonValue,
                Data = data
            };

            await SendCommand(command);
            var schemaModelDataVerify = JsonConvert.DeserializeObject<Simjob.Framework.Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var properties = schemaModelDataVerify.Properties;
            var fileFields = properties.Where(x => x.Value.file != null).ToList();
            if (fileFields.Count > 0) Thread.Sleep(2000);
            await RunActionForSchema(schemaName, data["id"].ToString());

            //var userAOriginal = _userAccessService.GetBySchemaRecordId(data["id"].ToString());

            //var values = new List<object>(data.Values);
            //var keys = new List<string>(data.Keys);
            //var Lvalues = new List<string>();
            //Dictionary<string, string> dict = new Dictionary<string, string>();

            //foreach (var v in values)
            //{
            //    if (v != null)
            //    {
            //        Lvalues.Add(v.ToString());
            //    }
            //    else
            //    {
            //        Lvalues.Add(null);
            //    }
            //}

            //for (int i = 0; i < keys.Count; i++)
            //{
            //    dict.Add(keys[i], Lvalues[i]);
            //}

            ////Se userAccess == null > insert userAccess
            ////Se userAccess.ValorAlterado == null > valorAlterado == dict  valorOriginal continua o mesmo
            ////Se userAccess.ValorAlterado != null > valorOriginal = valorAlterado   valorAlterado = dict

            //if (userAOriginal != null)
            //{
            //    if (userAOriginal.ValorAlterado != null)
            //    {
            //        var newUserAccess = new UserAccess(userId, tenanty, userName, latitude, longitude, ip, userAgent, schemaName, description, userAOriginal.SchemaRecordId, userAOriginal.ValorAlterado, dict);
            //        _userAccessService.UpdateUserAccess(newUserAccess, userAOriginal.Id);
            //    }
            //    else
            //    {
            //        var newUserAccess = new UserAccess(userId, tenanty, userName, latitude, longitude, ip, userAgent, schemaName, description, userAOriginal.SchemaRecordId, userAOriginal.ValorOriginal, dict);
            //        _userAccessService.UpdateUserAccess(newUserAccess, userAOriginal.Id);
            //    }
            //}
            //else
            //{
            //    var newUserAccess = new UserAccess(userId, tenanty, userName, latitude, longitude, ip, userAgent, schemaName, description, id, dict, null);
            //    await _userAccessService.Register(newUserAccess);
            //}

            var statusFF = false;
            var fieldF = "";
            foreach (var prop in schemaProp)
            {
                if (prop.Value.statusF)
                {
                    fieldF = prop.Key;
                    statusFF = true;
                }
            }

            if (statusFF)
            {
                var statusFlow = _statusFlowService.GetBySchemaAndField(schemaName, fieldF, tenanty);
                if (statusFlow != null)
                {
                    int? aprovMin = null;
                    var listEmailAprov = new List<AprovEmailProperties>();
                    var entityJson = JsonConvert.SerializeObject(entityOld);
                    var entityDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(entityJson);
                    if (entityDict[statusFlow.Field].ToString() != data[statusFlow.Field].ToString())
                    {
                        foreach (var prop in statusFlow.Properties)
                        {
                            var dataAprov = DateTime.Now;
                            if (prop.Status == data[statusFlow.Field].ToString())
                            {
                                List<string> aprovadoresList = new List<string>();
                                if (!prop.Action.IsNullOrEmpty() && prop.Type == "Action")
                                {
                                    var statusFlowAction = _actionRepository.GetByField("name", prop.Action);

                                    if (statusFlowAction != null)
                                    {
                                        ActionController x = new(null, null, _schemaRepository, _entityService, _actionRepository, null, _userService);
                                        ApiEntities.Action[] ac = x.GetBySchemaName(schemaName);

                                        var userHelper = _schemaRepository.GetUserHelper();
                                        var user = _userService.GetByUserName(userHelper.GetTenanty(), userHelper.GetUserName());

                                        string auth = Request.Headers["Authorization"];

                                        dynamic result = await x.Run(statusFlowAction, new Dictionary<string, object>()
                                    {
                                        { "params", new { entity = data } }
                                    }, auth, user, null);

                                        aprovadoresList = result.aprovadores.ToObject<List<string>>();
                                        if (!aprovadoresList.Contains("admin@admin.com") && !aprovadoresList.IsNullOrEmpty()) aprovadoresList.Add("admin@admin.com");
                                        aprovadoresList = aprovadoresList.Distinct().ToList();
                                        var statusFlowItem = _statusFlowItemService.GetBySchemaRecordId(data["id"].ToString());
                                        if (statusFlowItem != null)
                                        {
                                            statusFlowItem.StatusFlowId = statusFlow.Id;
                                            statusFlowItem.Status = prop.Status;
                                            statusFlowItem.AprovEmail = aprovadoresList.ToArray();
                                            _statusFlowItemService.UpdateStatusFlowItem(statusFlowItem, statusFlowItem.Id);
                                        }
                                        else
                                        {
                                            _statusFlowItemService.RegisterAsync(new StatusFlowItem(statusFlow.Id, data["id"].ToString(), prop.Status, aprovadoresList.ToArray()));
                                        }
                                    }
                                    else
                                    {
                                        return BadRequest($"StatusFlow não encontrou a Action: {prop.Action}");
                                    }
                                    aprovMin = prop.AprovMin;

                                    foreach (var email in aprovadoresList) listEmailAprov.Add(new AprovEmailProperties { email = email, aprov = null, view = false, data = null });
                                }
                                else
                                {
                                    aprovMin = prop.AprovMin;
                                    var aprovEmails = prop.AprovEmail;
                                    var users = _userService.GetUsersByUserName(aprovEmails, tenanty);
                                    if (!users.IsNullOrEmpty())
                                    {
                                        var userEmails = users.Select(x => x.UserName).ToList();
                                        if (!userEmails.Contains("admin@admin.com") && !userEmails.IsNullOrEmpty()) userEmails.Add("admin@admin.com");
                                        foreach (var email in userEmails) listEmailAprov.Add(new AprovEmailProperties { email = email, aprov = null, view = false, data = null });
                                        var statusFlowItem = _statusFlowItemService.GetBySchemaRecordId(data["id"].ToString());
                                        if (statusFlowItem != null)
                                        {
                                            statusFlowItem.StatusFlowId = statusFlow.Id;
                                            statusFlowItem.Status = prop.Status;
                                            statusFlowItem.AprovEmail = userEmails.ToArray();
                                            _statusFlowItemService.UpdateStatusFlowItem(statusFlowItem, statusFlowItem.Id);
                                        }
                                        else
                                        {
                                            _statusFlowItemService.RegisterAsync(new StatusFlowItem(statusFlow.Id, data["id"].ToString(), prop.Status, userEmails.ToArray()));
                                        }
                                    }
                                }
                            }
                        }

                        if (aprovMin != null && !listEmailAprov.IsNullOrEmpty())
                        {
                            //var aprov = listEmail.Where(x => x.aprov == true).Count() >= aprovMin ? true : false;
                            var newNotification = new Notification(data["id"].ToString(), schemaName, statusFlow.Field, data[statusFlow.Field].ToString(), entityDict[statusFlow.Field].ToString(), "Mudança de status para " + data[statusFlow.Field].ToString(), null, null, false, (int)aprovMin, listEmailAprov.ToArray(), null)
                            {
                                CreateBy = userName
                            };
                            _notificationService.RegisterAsync(newNotification);

                            //sendgrid
                            foreach (var e in newNotification.AprovEmail)
                            {
                                var sendEmailCommand = new SendEmailCommand
                                {
                                    Subject = "Notificação",
                                    To = e.email,
                                    PlainTextContent = newNotification.Msg,
                                    HtmlContent = newNotification.Msg
                                };
                                _userAccessService.SendEmail(sendEmailCommand);
                            }
                            if (listEmailAprov.Count > 1)
                            {
                                var newApproveLog = new ApproveLog(data["id"].ToString(), schemaName, statusFlow.Field, data[statusFlow.Field].ToString(), "Mudança de status para " + data[statusFlow.Field].ToString());
                                _approveLogService.Register(newApproveLog);
                            }
                        }
                    }
                }
            }
            var schemaPropRelations = schemaProp.Where(x => !string.IsNullOrEmpty(x.Value.RelationSchema)).ToList();
            if (schemaPropRelations.Any())
            {
                var sendObject = schemaPropRelations
                .Select(x =>
                {
                    var schemaChildren = _schemaRepository.GetSchemaByField("name", x.Value.RelationSchema);
                    var jsonValue = schemaChildren.JsonValue; // Store JSON schema for access later
                    string quoteChildren = "\"";
                    jsonValue = jsonValue.Replace($"{quoteChildren}enum{quoteChildren}:[", $"{quoteChildren}enum{quoteChildren}:[{quoteChildren + quoteChildren},");
                    var schemaPropChildren = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(jsonValue).Properties;

                    if (!schemaPropChildren.ContainsKey("parentId"))
                        return null; // Return null to filter later

                    var matchingData = data.FirstOrDefault(y => y.Key == x.Key).Value;
                    var childrenIds = ((JArray)matchingData)?.ToObject<List<string>>();

                    return new
                    {
                        fieldName = x.Key,
                        schemaName = x.Value.RelationSchema,
                        childrenIds = childrenIds,
                        jsonValue = jsonValue,
                        parentId = id,
                    };
                })
                .Where(x => x != null) // Remove null entries
                .ToList();

                if (sendObject.Any())
                {
                    foreach (var item in sendObject)
                    {
                        dynamic entitiesOld = await _entityService.GetByIdsList(item.schemaName, item.childrenIds);

                        List<Dictionary<string, object>> datas = ((IEnumerable<dynamic>)entitiesOld)
                        .Cast<object>() // Cast to a non-dynamic type to resolve the issue
                        .Select(entityUpdate =>
                        {
                            var properties = entityUpdate.GetType()
                                .GetProperties(BindingFlags.Instance | BindingFlags.Public);

                            // Create dictionary with properties
                            var dictionary = properties.ToDictionary(
                               prop => char.ToLowerInvariant(prop.Name[0]) + prop.Name.Substring(1), // Convert to camelCase
                               prop => prop.GetValue(entityUpdate, null)
                           );

                            // Add parentId property
                            dictionary["parentId"] = item.parentId; // Ensure `item.parentId` is accessible
                            dictionary["updateAt"] = DateTime.Now;

                            return dictionary;
                        }).ToList();

                        var commandChildrenUpdate = new UpdateManyEntityCommand()
                        {
                            Id = id,
                            SchemaName = item.schemaName,
                            SchemaJson = item.jsonValue,
                            Datas = datas
                        };

                        await SendCommand(commandChildrenUpdate);
                    }
                }
            }
            var entity = await _entityService.GetById(schemaName, data["id"].ToString());
            //var afterSave = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(json).AfterSave;
            //if (afterSave != null)
            //{
            //    var action = _actionRepository.GetByField("name", afterSave);

            //    if (action != null)
            //    {
            //        ActionController x = new(null, null, _schemaRepository, _entityService, _actionRepository, null, _userService);
            //        ApiEntities.Action[] ac = x.GetBySchemaName(schemaName);

            //        var userHelper = _schemaRepository.GetUserHelper();
            //        var user = _userService.GetByUserName(userHelper.GetTenanty(), userHelper.GetUserName());

            //        string auth = Request.Headers["Authorization"];

            //        var result = await x.Run(action, new Dictionary<string, object>()
            //        {
            //            { "params", null }
            //        }, auth, user, entity);
            //    }

            //}

            return ResponseDefault(entity);
        }

        /// <summary>
        /// Register new object
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPut("UpdateOwner")]
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> UpdateOwner(string schemaName, string schemaRecordId, string userId)
        {
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            if (schema == null) return ResponseDefault();
            var json = schema.JsonValue;
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(json);
            if (!schemaModel.Owner_Schema) return ResponseDefault("Schema deve ser owner");
            string userField = null;
            var count = schemaModel.Properties.Where(x => x.Value.Type == "user").Count();
            if (count > 0)
            {
                userField = schemaModel.Properties.Where(x => x.Value.Type == "user").FirstOrDefault().Key;
            }
            var entities = await _entityService.GetByIdsList(schemaName, schemaRecordId.Split(',').ToList());
            if (entities == null) return ResponseDefault("Schema Não encontrado");
            var user = _userService.GetUserById(userId);
            if (user == null) return ResponseDefault("User Não encontrado");
            var jsonRecord = JsonConvert.SerializeObject(entities);
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var dicts = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonRecord, settings);
            foreach (var dict in dicts)
            {
                if (dict.ContainsKey("owners"))
                {
                    dict["owners"] = new List<string>() { userId };
                }
                else
                {
                    dict.Add("owners", new List<string>() { userId });
                }
            }
            var command = new UpdateManyEntityCommand()
            {
                Id = schema.Id,
                SchemaName = schemaName,
                SchemaJson = schema.JsonValue,
                Datas = dicts
            };

            await SendCommand(command);

            return ResponseDefault();
        }

        /// <summary>
        /// Register new object
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPut("RemoveOwner")]
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> DeleteOwner(string schemaName, string schemaRecordId, string userId)
        {
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            if (schema == null) return ResponseDefault();
            var json = schema.JsonValue;
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(json);
            string userField = null;
            var count = schemaModel.Properties.Where(x => x.Value.Type == "user").Count();
            if (count > 0)
            {
                userField = schemaModel.Properties.Where(x => x.Value.Type == "user").FirstOrDefault().Key;
            }
            var entities = await _entityService.GetByIdsList(schemaName, schemaRecordId.Split(',').ToList());
            if (entities == null) return ResponseDefault("Schema Não encontrado");
            var user = _userService.GetUserById(userId);
            if (user == null) return ResponseDefault("User Não encontrado");
            var jsonRecord = JsonConvert.SerializeObject(entities);
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var dicts = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonRecord, settings);

            foreach (var dict in dicts)
            {
                if (dict.ContainsKey("owners"))
                {
                    var ownersArray = dict["owners"] as JArray;  // Cast dict["owners"] to JArray
                    if (ownersArray != null)
                    {
                        var listOwners = ownersArray.ToObject<List<string>>();  // Convert JArray to List<string>
                        dict["owners"] = listOwners.Where(x => !x.Contains(userId)).ToList();
                    }
                }
            }
            var command = new UpdateManyEntityCommand()
            {
                Id = schema.Id,
                SchemaName = schemaName,
                SchemaJson = schema.JsonValue,
                Datas = dicts
            };
            await SendCommand(command);
            return ResponseDefault();
        }

        /// <summary>
        /// Register new object
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPost("{schemaName}")]
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> Insert(string schemaName, string latitude, string longitude, string ip, string userAgent, string description, [FromBody] Dictionary<string, object> data)
        {
            if (data.Count() == 0) return BadRequest();

            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            if (schema == null) return ResponseDefault();
            var json = schema.JsonValue;
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(json);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true && schemaModel.Alias != null)
            {
                var dataSql = new Dictionary<string, object>(data);
                //add mirrors sql
                var dataSqlWithMirros = await _entityService.AddMirrorsSql(schemaName, dataSql);
                if (dataSqlWithMirros == null) return BadRequest();
                var result = await SQLServerService.InsertWithResult(schemaModel.Alias, dataSqlWithMirros, source);
                if (!result.success) return BadRequest(result.error);
                return ResponseDefault(result.inserted);
            }

            string quote = "\"";
            schema.JsonValue = schema.JsonValue.Replace($"{quote}enum{quote}:[", $"{quote}enum{quote}:[{quote + quote},");
            var schemaProp = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(json).Properties;

            data = data.Where(kv => schemaProp.ContainsKey(kv.Key)).ToDictionary(kv => kv.Key, kv => kv.Value);
            string id = Guid.NewGuid().ToString();

            if (data.Keys.FirstOrDefault(key => key == "id") == null) data.Add("id", null);

            data["id"] = id;

            var beforeSave = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(json).BeforeSave;
            if (beforeSave != null)
            {
                var type = await _schemaBuilder.GetSchemaType(schemaName);
                string jsonSchema = JsonConvert.SerializeObject(data, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                var entityAction = JsonConvert.DeserializeObject(jsonSchema, type);
                var action = _actionRepository.GetByField("name", beforeSave);

                if (action != null)
                {
                    ActionController x = new(null, null, _schemaRepository, _entityService, _actionRepository, null, _userService);
                    ApiEntities.Action[] ac = x.GetBySchemaName(schemaName);

                    var userHelper = _schemaRepository.GetUserHelper();
                    var user = _userService.GetByUserName(userHelper.GetTenanty(), userHelper.GetUserName());

                    string auth = Request.Headers["Authorization"];

                    var result = await x.Run(action, new Dictionary<string, object>()
                {
                    { "params", null }
                }, auth, user, entityAction);
                }
            }
            var statusF = false;
            var field = "";

            foreach (var prop in schemaProp)
            {
                if (prop.Value.unique == true)
                {
                    var FromData = data[prop.Key];
                    var existe = _entityService.GetByFieldName(schemaName, prop.Key, FromData.ToString());
                    statusF = prop.Value.statusF;
                    if (existe.Result == true)
                    {
                        return BadRequest("field " + prop.Key + " already exists");
                    }
                }
                if (prop.Value.uniqueCompany == true && data.ContainsKey("companySiteId"))
                {
                    var FromData = data[prop.Key];

                    var fromDataCompany = data["companySiteId"];

                    dynamic existe = await _entityService.GetByFieldNameCompany(schemaName, prop.Key, FromData.ToString(), fromDataCompany.ToString());

                    statusF = prop.Value.statusF;

                    if (existe != null)
                    {
                        return BadRequest("field " + prop.Key + " already exists");
                    }
                }
                if (prop.Value.statusF)
                {
                    field = prop.Key;
                    statusF = true;
                }
                if (prop.Value.@enum != null)
                {
                    if (!data.ContainsKey(prop.Key))
                    {
                        data.Add(prop.Key, "");
                    }
                }
                if (prop.Value.Type == "date" && data.ContainsKey(prop.Key))
                {
                    if (data[prop.Key] != null && data[prop.Key].ToString() == "") data[prop.Key] = null;
                    if (data[prop.Key] != null)
                    {
                        if (DateTime.TryParse(data[prop.Key].ToString(), out _)) data[prop.Key] = DateTime.Parse(data[prop.Key].ToString()).AddHours(3);
                        else data[prop.Key] = null;
                    }
                }
                if (prop.Value.AutoInc)
                {
                    var autoIncField = prop.Key.Replace(prop.Key[0], char.ToUpper(prop.Key[0]));
                    if (!data.ContainsKey(prop.Key))
                    {
                        var generatedAutoInc = await _generatorsService.GetAutoincAsync(schemaName, autoIncField, "");
                        if (generatedAutoInc != null) data.Add(prop.Key, generatedAutoInc);
                    }
                }
            }
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
            if (data.Keys.FirstOrDefault(key => key == "createBy") == null) data.Add("createBy", userName);
            var command = new InsertEntityCommand()
            {
                SchemaName = schemaName,
                SchemaJson = schema.JsonValue,
                Data = data
            };
            await SendCommand(command);
            var schemaModelData = JsonConvert.DeserializeObject<Simjob.Framework.Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var properties = schemaModelData.Properties;
            var fileFields = properties.Where(x => x.Value.file != null).ToList();
            if (fileFields.Count > 0) Thread.Sleep(3000);
            else
            {
                Thread.Sleep(1000);
            }

            var values = new List<object>(data.Values);
            var keys = new List<string>(data.Keys);
            var Lvalues = new List<string>();
            //Dictionary<string, string> dict = new Dictionary<string, string>();
            Dictionary<string, string> convertedDictionary = data
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value != null ? kvp.Value.ToString() : null
            );

            var newUserAccess = new UserAccess(userId, tenanty, userName, latitude, longitude, ip, userAgent, schemaName, description, id, convertedDictionary, null);
            await _userAccessService.Register(newUserAccess);

            await RunActionForSchema(schemaName, data["id"].ToString());

            if (statusF)
            {
                int? aprovMin = null;

                var listEmailAprov = new List<AprovEmailProperties>();
                var statusFlow = _statusFlowService.GetBySchemaAndField(schemaName, field, tenanty);
                if (statusFlow != null)
                {
                    foreach (var prop in statusFlow.Properties)
                    {
                        var dataAprov = DateTime.Now;
                        if (prop.Status == data[statusFlow.Field].ToString())
                        {
                            List<string> aprovadoresList = new List<string>();
                            if (!prop.Action.IsNullOrEmpty() && prop.Type == "Action")
                            {
                                var statusFlowAction = _actionRepository.GetByField("name", prop.Action);

                                if (statusFlowAction != null)
                                {
                                    ActionController x = new(null, null, _schemaRepository, _entityService, _actionRepository, null, _userService);
                                    ApiEntities.Action[] ac = x.GetBySchemaName(schemaName);

                                    var userHelper = _schemaRepository.GetUserHelper();
                                    var user = _userService.GetByUserName(userHelper.GetTenanty(), userHelper.GetUserName());

                                    string auth = Request.Headers["Authorization"];

                                    dynamic result = await x.Run(statusFlowAction, new Dictionary<string, object>()
                                    {
                                        { "params", new { entity = data } }
                                    }, auth, user, null);

                                    aprovadoresList = result.aprovadores.ToObject<List<string>>();
                                    if (!aprovadoresList.IsNullOrEmpty())
                                    {
                                        if (!aprovadoresList.Contains("admin@admin.com")) aprovadoresList.Add("admin@admin.com");
                                        aprovadoresList = aprovadoresList.Distinct().ToList();
                                        _statusFlowItemService.RegisterAsync(new StatusFlowItem(statusFlow.Id, data["id"].ToString(), prop.Status, aprovadoresList.ToArray()));
                                    }
                                    //_statusFlowService.UpdateStatusFlowStatus(statusFlow, aprovadoresList, prop.Status);
                                }
                                else
                                {
                                    return BadRequest();
                                }
                                aprovMin = prop.AprovMin;
                                foreach (var email in aprovadoresList) listEmailAprov.Add(new AprovEmailProperties { email = email, aprov = null, view = false, data = null });
                            }
                            else
                            {
                                aprovMin = prop.AprovMin;
                                var aprovEmails = prop.AprovEmail;
                                var users = _userService.GetUsersByUserName(aprovEmails, tenanty);
                                if (!users.IsNullOrEmpty())
                                {
                                    var userEmails = users.Select(x => x.UserName).ToList();
                                    foreach (var email in userEmails) listEmailAprov.Add(new AprovEmailProperties { email = email, aprov = null, view = false, data = null });
                                    if (!userEmails.IsNullOrEmpty())
                                    {
                                        if (!userEmails.Contains("admin@admin.com")) userEmails.Add("admin@admin.com");
                                        _statusFlowItemService.RegisterAsync(new StatusFlowItem(statusFlow.Id, data["id"].ToString(), prop.Status, userEmails.ToArray()));
                                    }
                                }
                            }
                        }
                    }
                    if (aprovMin != null && !listEmailAprov.IsNullOrEmpty())
                    {
                        //var aprov = listEmail.Where(x => x.aprov == true).Count() >= aprovMin ? true : false;
                        var newNotification = new Notification(id, schemaName, statusFlow.Field, data[statusFlow.Field].ToString(), null, "Mudança de status para " + data[statusFlow.Field].ToString(), null, null, false, (int)aprovMin, listEmailAprov.ToArray(), null)
                        {
                            CreateBy = userName
                        };
                        _notificationService.RegisterAsync(newNotification);

                        // sendgrid
                        foreach (var e in newNotification.AprovEmail)
                        {
                            var sendEmailCommand = new SendEmailCommand
                            {
                                Subject = "Notificação",
                                To = e.email,
                                PlainTextContent = newNotification.Msg,
                                HtmlContent = newNotification.Msg
                            };
                            _userAccessService.SendEmail(sendEmailCommand);
                        }
                        if (listEmailAprov.Count > 1)
                        {
                            var newApproveLog = new ApproveLog(id, schemaName, statusFlow.Field, data[statusFlow.Field].ToString(), "Mudança de status para " + data[statusFlow.Field].ToString());
                            _approveLogService.Register(newApproveLog);
                        }
                    }
                }
            }
            var schemaPropRelations = schemaProp.Where(x => !string.IsNullOrEmpty(x.Value.RelationSchema)).ToList();
            if (schemaPropRelations.Any())
            {
                var sendObject = schemaPropRelations
                .Select(x =>
                {
                    var schemaChildren = _schemaRepository.GetSchemaByField("name", x.Value.RelationSchema);
                    var jsonValue = schemaChildren.JsonValue; // Store JSON schema for access later
                    string quoteChildren = "\"";
                    jsonValue = jsonValue.Replace($"{quoteChildren}enum{quoteChildren}:[", $"{quoteChildren}enum{quoteChildren}:[{quoteChildren + quoteChildren},");
                    var schemaPropChildren = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(jsonValue).Properties;

                    if (!schemaPropChildren.ContainsKey("parentId"))
                        return null; // Return null to filter later

                    var matchingData = data.FirstOrDefault(y => y.Key == x.Key).Value;
                    var childrenIds = ((JArray)matchingData)?.ToObject<List<string>>();

                    return new
                    {
                        fieldName = x.Key,
                        schemaName = x.Value.RelationSchema,
                        childrenIds = childrenIds,
                        jsonValue = jsonValue,
                        parentId = id,
                    };
                })
                .Where(x => x != null) // Remove null entries
                .ToList();

                if (sendObject.Any())
                {
                    foreach (var item in sendObject)
                    {
                        dynamic entitiesOld = await _entityService.GetByIdsList(item.schemaName, item.childrenIds);

                        List<Dictionary<string, object>> datas = ((IEnumerable<dynamic>)entitiesOld)
                        .Cast<object>() // Cast to a non-dynamic type to resolve the issue
                        .Select(entityUpdate =>
                        {
                            var properties = entityUpdate.GetType()
                                .GetProperties(BindingFlags.Instance | BindingFlags.Public);

                            // Create dictionary with properties
                            var dictionary = properties.ToDictionary(
                               prop => char.ToLowerInvariant(prop.Name[0]) + prop.Name.Substring(1), // Convert to camelCase
                               prop => prop.GetValue(entityUpdate, null)
                           );

                            // Add parentId property
                            dictionary["parentId"] = item.parentId; // Ensure `item.parentId` is accessible
                            dictionary["updateAt"] = DateTime.Now;

                            return dictionary;
                        }).ToList();

                        var commandChildrenUpdate = new UpdateManyEntityCommand()
                        {
                            Id = id,
                            SchemaName = item.schemaName,
                            SchemaJson = item.jsonValue,
                            Datas = datas
                        };

                        await SendCommand(commandChildrenUpdate);
                    }
                }
            }

            var entity = await _entityService.GetById(schemaName, id);
            //var afterSave = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(json).AfterSave;
            //if (afterSave != null)
            //{
            //    var action = _actionRepository.GetByField("name", afterSave);

            //    if (action != null)
            //    {
            //        ActionController x = new(null, null, _schemaRepository, _entityService, _actionRepository, null, _userService);
            //        ApiEntities.Action[] ac = x.GetBySchemaName(schemaName);

            //        var userHelper = _schemaRepository.GetUserHelper();
            //        var user = _userService.GetByUserName(userHelper.GetTenanty(), userHelper.GetUserName());

            //        string auth = Request.Headers["Authorization"];

            //        var result = await x.Run(action, new Dictionary<string, object>()
            //    {
            //        { "params", null }
            //    }, auth, user, entity);
            //    }

            //}
            return ResponseDefault(entity);
        }

        /// <summary>
        /// Register new objects
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPost("insertMany/{schemaName}")]
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> InsertMany(string schemaName, [FromBody] List<Dictionary<string, object>> datas)
        {
            if (datas.Count() == 0) return BadRequest();
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);

            var json = schema.JsonValue;
            string quote = "\"";
            schema.JsonValue = schema.JsonValue.Replace($"{quote}enum{quote}:[", $"{quote}enum{quote}:[{quote + quote},");
            var schemaProp = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(json).Properties;

            //datas = datas.SelectMany(x => x.Where(kv => schemaProp.ContainsKey(kv.Key)).ToDictionary(kv => kv.Key, kv => kv.Value);

            foreach (var data in datas)
            {
                foreach (var key in data.Keys) if (!schemaProp.ContainsKey(key)) return BadRequest($"schema não possui campo {key}");

                string id = Guid.NewGuid().ToString();
                if (data.Keys.FirstOrDefault(key => key == "id") == null)
                {
                    data.Add("id", null);
                    data["id"] = id;
                }

                foreach (var prop in schemaProp)
                {
                    if (prop.Value.unique == true)
                    {
                        var FromData = data[prop.Key];
                        var existe = _entityService.GetByFieldName(schemaName, prop.Key, FromData.ToString());
                        if (existe.Result == true)
                        {
                            return BadRequest("field " + prop.Key + " already exists");
                        }
                    }

                    if (prop.Value.@enum != null)
                    {
                        if (!data.ContainsKey(prop.Key))
                        {
                            data.Add(prop.Key, "");
                        }
                    }
                    if (prop.Value.Type == "date" && data.ContainsKey(prop.Key))
                    {
                        if (data[prop.Key] != null && data[prop.Key].ToString() == "") data[prop.Key] = null;
                        if (data[prop.Key] != null)
                        {
                            if (DateTime.TryParse(data[prop.Key].ToString(), out _)) data[prop.Key] = DateTime.Parse(data[prop.Key].ToString()).AddHours(3);
                            else data[prop.Key] = null;
                        }
                    }
                    if (prop.Value.AutoInc)
                    {
                        var autoIncField = prop.Key.Replace(prop.Key[0], char.ToUpper(prop.Key[0]));
                        if (!data.ContainsKey(prop.Key))
                        {
                            var generatedAutoInc = await _generatorsService.GetAutoincAsync(schemaName, autoIncField, "");
                            if (generatedAutoInc != null) data.Add(prop.Key, generatedAutoInc);
                        }
                    }
                }
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
                if (data.Keys.FirstOrDefault(key => key == "createBy") == null) data.Add("createBy", userName);
            }

            var command = new InsertManyEntityCommand()
            {
                SchemaName = schemaName,
                SchemaJson = schema.JsonValue,
                Datas = datas
            };
            await SendCommand(command);
            Thread.Sleep(1000);
            return ResponseDefault(datas);
        }

        /// <summary>
        /// UpdateMany entity objects
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPut("updateMany/{schemaName}")]
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> UpdateMany(string schemaName, [FromBody] List<Dictionary<string, object>> datas)
        {
            if (datas.Count == 0) return BadRequest();
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModelData = JsonConvert.DeserializeObject<SchemaModel>(schema.JsonValue);

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
            string id = schema.Id;

            var json = schema.JsonValue;

            var schemaProp = JsonConvert.DeserializeObject<SchemaModel>(json).Properties;

            string quote = "\"";
            schema.JsonValue = schema.JsonValue.Replace($"{quote}enum{quote}:[", $"{quote}enum{quote}:[{quote + quote},");
            List<string> ids = datas.Select(data => data["id"].ToString()).ToList();
            dynamic entitiesOld = await _entityService.GetByIdsList(schemaName, ids);
            List<string> entityIds = new List<string>();

            foreach (var e in entitiesOld)
            {
                string entityId = e.Id.ToString();
                entityIds.Add(entityId);

                datas = datas.Select(x =>
                {
                    if (x.TryGetValue("id", out var id) && id.ToString() == e.Id)
                    {
                        x.Add("createAt", e.CreateAt);
                        x.Add("createBy", e.CreateBy);
                        x.Add("updateBy", userName);
                    }
                    return x;
                }).ToList();
            }
            datas = datas.Where(x => entityIds.Contains(x["id"])).ToList();

            foreach (var data in datas)
            {
                List<string> ignoreKeys = new List<string>() { "id", "createAt", "createBy", "updateAt", "updateBy", "deleteAt", "deleteBy", "isDeleted" };

                foreach (var key in data.Keys) if (!ignoreKeys.Contains(key) && !schemaProp.ContainsKey(key)) return BadRequest($"schema não possui campo {key}");

                foreach (var prop in schemaProp)
                {
                    if (prop.Value.Type == "date" && data.ContainsKey(prop.Key))
                    {
                        if (data[prop.Key] != null && data[prop.Key].ToString() == "") data[prop.Key] = null;
                        if (data[prop.Key] != null)
                        {
                            if (DateTime.TryParse(data[prop.Key].ToString(), out _)) data[prop.Key] = DateTime.Parse(data[prop.Key].ToString()).AddHours(3);
                            else data[prop.Key] = null;
                        }
                    }
                }

                //dynamic entityForFields = entityOld;
                //if (data.Keys.FirstOrDefault(key => key == "createAt") == null) data.Add("createAt", entityForFields.CreateAt);
                //if (data.Keys.FirstOrDefault(key => key == "createBy") == null) data.Add("createBy", entityForFields.CreateBy);
                //if (data.Keys.FirstOrDefault(key => key == "updateBy") == null) data.Add("updateBy", userName);
            }

            var command = new UpdateManyEntityCommand()
            {
                Id = id,
                SchemaName = schemaName,
                SchemaJson = schema.JsonValue,
                Datas = datas
            };

            await SendCommand(command);

            return ResponseDefault(datas);
        }

        internal async Task<List<string>> RegisterChildrens(Dictionary<string, Infra.Domain.Models.SchemaModelProperty> schemaProp, List<string> schemasSplit, Dictionary<string, object> data, string createBy)
        {
            List<Dictionary<string, object>> schemaInserts = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> listSchemaObj = new List<Dictionary<string, object>>();
            List<string> childrenIds = new List<string>();
            var propFields = schemaProp.Where(x => schemasSplit.Contains(x.Value.RelationSchema) && x.Value.Type == "array").OrderBy(x => x.Value.RelationSchema).ToList();
            for (int i = 0; i < schemasSplit.Count(); i++)
            {
                var jsonData = JsonConvert.SerializeObject(data[propFields[i].Key]);
                schemaInserts = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonData);
                foreach (var childrenInsert in schemaInserts)
                {
                    childrenInsert.Add("id", Guid.NewGuid().ToString());
                    childrenInsert.Add("nameofSchema", schemasSplit[i]);
                }

                data.Remove(propFields[i].Key);
                listSchemaObj.AddRange(schemaInserts);
            }

            //cadastra childrenSchemas
            //foreach (var sObj in listSchemaObj)
            //{
            foreach (var childrenSchemaData in listSchemaObj)
            {
                Dictionary<string, object> insert = childrenSchemaData;
                var schemaChildrenName = insert["nameofSchema"].ToString();
                var childrenSchema = _schemaRepository.GetSchemaByField("name", schemaChildrenName);
                //pega os ids de children
                insert.Remove("nameofSchema");
                childrenIds.Add(insert["id"].ToString());
                if (insert.Keys.FirstOrDefault(key => key == "createBy") == null) insert.Add("createBy", createBy);
                var childrenCommand = new InsertEntityCommand()
                {
                    SchemaName = schemaChildrenName,
                    SchemaJson = childrenSchema.JsonValue,
                    Data = insert
                };
                await SendCommand(childrenCommand);
                //Thread.Sleep(500);
            }
            //}
            return childrenIds;
        }

        /// <summary>
        /// Register new object
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPost("{schemasChildren}/Fields")]
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> InsertFields(string schemaName, string schemasChildren, string latitude, string longitude, string ip, string userAgent, string description, [FromBody] Dictionary<string, object> data)
        {
            if (data.Count() == 0) return BadRequest();
            string id = Guid.NewGuid().ToString();
            if (data.Keys.FirstOrDefault(key => key == "id") == null) data.Add("id", null);
            data["id"] = id;

            List<string> schemasSplit = schemasChildren.Split(',').ToList();

            schemasSplit = schemasSplit.OrderBy(c => c).ToList();

            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var json = schema.JsonValue;
            var schemaProp = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(json).Properties;
            string quote = "\"";
            schema.JsonValue = schema.JsonValue.Replace($"{quote}enum{quote}:[", $"{quote}enum{quote}:[{quote + quote},");
            //schema.JsonValue = schema.JsonValue.Replace($"{quote}enum{quote}:[{quote},]", $"{quote}enum{quote}:[]");
            foreach (var key in data.Keys)
            {
                if (key != "id" && !schemaProp.ContainsKey(key)) return BadRequest($"schema não possui campo {key}");
            }

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

            var childrenIds = await RegisterChildrens(schemaProp, schemasSplit, data, userName);
            Thread.Sleep(1000);
            var beforeSave = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(json).BeforeSave;
            if (beforeSave != null)
            {
                var action = _actionRepository.GetByField("name", beforeSave);

                if (action != null)
                {
                    ActionController x = new(null, null, _schemaRepository, _entityService, _actionRepository, null, _userService);
                    ApiEntities.Action[] ac = x.GetBySchemaName(schemaName);

                    var userHelper = _schemaRepository.GetUserHelper();
                    var user = _userService.GetByUserName(userHelper.GetTenanty(), userHelper.GetUserName());

                    string auth = Request.Headers["Authorization"];

                    var result = await x.Run(action, new Dictionary<string, object>()
                {
                    { "params", null }
                }, auth, user, null);
                }
                else
                {
                    return BadRequest();
                }
            }
            var statusF = false;
            var field = "";
            var relationField = "";
            foreach (var prop in schemaProp)
            {
                if (prop.Value.unique == true)
                {
                    var FromData = data[prop.Key];
                    var existe = _entityService.GetByFieldName(schemaName, prop.Key, FromData.ToString());
                    statusF = prop.Value.statusF;
                    if (existe.Result == true)
                    {
                        return BadRequest("field " + prop.Key + " already exists");
                    }
                }
                if (prop.Value.statusF)
                {
                    field = prop.Key;
                    statusF = true;
                }
                if (prop.Value.@enum != null)
                {
                    if (!data.ContainsKey(prop.Key))
                    {
                        data.Add(prop.Key, "");
                    }
                }
                if (prop.Value.Type == "date" && data.ContainsKey(prop.Key))
                {
                    if (data[prop.Key] != null && data[prop.Key].ToString() == "") data[prop.Key] = null;
                    if (data[prop.Key] != null)
                    {
                        if (DateTime.TryParse(data[prop.Key].ToString(), out _)) data[prop.Key] = DateTime.Parse(data[prop.Key].ToString()).AddHours(3);
                        else data[prop.Key] = null;
                    }
                }
                if (schemasSplit.Contains(prop.Value.RelationSchema) && prop.Value.Type == "array")
                {
                    relationField = prop.Key;
                }

                if (prop.Value.AutoInc)
                {
                    var autoIncField = prop.Key.Replace(prop.Key[0], char.ToUpper(prop.Key[0]));
                    if (!data.ContainsKey(prop.Key))
                    {
                        var generatedAutoInc = await _generatorsService.GetAutoincAsync(schemaName, autoIncField, "");
                        if (generatedAutoInc != null) data.Add(prop.Key, generatedAutoInc);
                    }
                }
            }
            //Adiciona ids no Schema
            data.Add(relationField, childrenIds);
            if (data.Keys.FirstOrDefault(key => key == "createBy") == null) data.Add("createBy", userName);
            var command = new InsertEntityCommand()
            {
                SchemaName = schemaName,
                SchemaJson = schema.JsonValue,
                Data = data
            };
            await SendCommand(command);
            Thread.Sleep(1000);
            //RunActionForSchema(schemaName, data["id"].ToString());
            //var entity = await _entityService.GetById(schemaName, id);

            var values = new List<object>(data.Values);
            var keys = new List<string>(data.Keys);
            var Lvalues = new List<string>();
            Dictionary<string, string> dict = new Dictionary<string, string>();

            foreach (var v in values)
            {
                if (v != null)
                {
                    Lvalues.Add(v.ToString());
                }
                else
                {
                    Lvalues.Add(null);
                }
            }

            for (int i = 0; i < keys.Count; i++)
            {
                dict.Add(keys[i], Lvalues[i]);
            }

            var newUserAccess = new UserAccess(userId, tenanty, userName, latitude, longitude, ip, userAgent, schemaName, description, id, dict, null);
            await _userAccessService.Register(newUserAccess);

            await RunActionForSchema(schemaName, data["id"].ToString());

            if (statusF)
            {
                int? aprovMin = null;

                var listEmailAprov = new List<AprovEmailProperties>();
                var statusFlow = _statusFlowService.GetBySchemaAndField(schemaName, field, tenanty);
                if (statusFlow != null)
                {
                    foreach (var prop in statusFlow.Properties)
                    {
                        var dataAprov = DateTime.Now;
                        if (prop.Status == data[statusFlow.Field].ToString())
                        {
                            List<string> aprovadoresList = new List<string>();
                            if (!prop.Action.IsNullOrEmpty() && prop.Type == "Action")
                            {
                                var statusFlowAction = _actionRepository.GetByField("name", prop.Action);

                                if (statusFlowAction != null)
                                {
                                    ActionController x = new(null, null, _schemaRepository, _entityService, _actionRepository, null, _userService);
                                    ApiEntities.Action[] ac = x.GetBySchemaName(schemaName);

                                    var userHelper = _schemaRepository.GetUserHelper();
                                    var user = _userService.GetByUserName(userHelper.GetTenanty(), userHelper.GetUserName());

                                    string auth = Request.Headers["Authorization"];

                                    dynamic result = await x.Run(statusFlowAction, new Dictionary<string, object>()
                                    {
                                        { "params", new { entity = data } }
                                    }, auth, user, null);

                                    aprovadoresList = result.aprovadores.ToObject<List<string>>();
                                    if (!aprovadoresList.IsNullOrEmpty())
                                    {
                                        if (!aprovadoresList.Contains("admin@admin.com")) aprovadoresList.Add("admin@admin.com");

                                        _statusFlowItemService.RegisterAsync(new StatusFlowItem(statusFlow.Id, data["id"].ToString(), prop.Status, aprovadoresList.ToArray()));
                                        //_statusFlowService.UpdateStatusFlowStatus(statusFlow, aprovadoresList, prop.Status);
                                    }
                                }
                                else
                                {
                                    return BadRequest();
                                }
                                aprovMin = prop.AprovMin;
                                foreach (var email in aprovadoresList) listEmailAprov.Add(new AprovEmailProperties { email = email, aprov = null, view = false, data = null });
                            }
                            else
                            {
                                aprovMin = prop.AprovMin;
                                var aprovEmails = prop.AprovEmail;
                                var users = _userService.GetUsersByUserName(aprovEmails, tenanty);
                                if (!users.IsNullOrEmpty())
                                {
                                    var userEmails = users.Select(x => x.UserName).ToList();
                                    foreach (var email in userEmails) listEmailAprov.Add(new AprovEmailProperties { email = email, aprov = null, view = false, data = null });
                                    if (!userEmails.IsNullOrEmpty())
                                    {
                                        if (!userEmails.Contains("admin@admin.com")) userEmails.Add("admin@admin.com");

                                        _statusFlowItemService.RegisterAsync(new StatusFlowItem(statusFlow.Id, data["id"].ToString(), prop.Status, userEmails.ToArray()));
                                    }
                                }
                            }
                        }
                    }
                    if (aprovMin != null && !listEmailAprov.IsNullOrEmpty())
                    {
                        //var aprov = listEmail.Where(x => x.aprov == true).Count() >= aprovMin ? true : false;
                        var newNotification = new Notification(id, schemaName, statusFlow.Field, data[statusFlow.Field].ToString(), null, "Mudança de status para " + data[statusFlow.Field].ToString(), null, null, false, (int)aprovMin, listEmailAprov.ToArray(), null)
                        {
                            CreateBy = userName
                        };
                        _notificationService.RegisterAsync(newNotification);

                        // sendgrid
                        foreach (var e in newNotification.AprovEmail)
                        {
                            var sendEmailCommand = new SendEmailCommand
                            {
                                Subject = "Notificação",
                                To = e.email,
                                PlainTextContent = newNotification.Msg,
                                HtmlContent = newNotification.Msg
                            };
                            _userAccessService.SendEmail(sendEmailCommand);
                        }
                        if (listEmailAprov.Count > 1)
                        {
                            var newApproveLog = new ApproveLog(id, schemaName, statusFlow.Field, data[statusFlow.Field].ToString(), "Mudança de status para " + data[statusFlow.Field].ToString());
                            _approveLogService.Register(newApproveLog);
                        }
                    }
                }
            }
            var entity = await _entityService.GetById(schemaName, id);

            var afterSave = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(json).AfterSave;
            if (afterSave != null)
            {
                var action = _actionRepository.GetByField("name", afterSave);

                if (action != null)
                {
                    ActionController x = new(null, null, _schemaRepository, _entityService, _actionRepository, null, _userService);
                    ApiEntities.Action[] ac = x.GetBySchemaName(schemaName);

                    var userHelper = _schemaRepository.GetUserHelper();
                    var user = _userService.GetByUserName(userHelper.GetTenanty(), userHelper.GetUserName());

                    string auth = Request.Headers["Authorization"];

                    var result = await x.Run(action, new Dictionary<string, object>()
                {
                    { "params", null }
                }, auth, user, entity);
                }
                else
                {
                    return BadRequest();
                }
            }
            return ResponseDefault(entity);
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <returns>Return result paginated</returns>
        /// <response code="200">Return result paginated</response>
        [HttpDelete("{schemaName}")]
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> Delete(string schemaName, string id)
        {
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            if (schema == null) return ResponseDefault();
            var json = schema.JsonValue;
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(json);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            if (source != null && source.Active != null && source.Active == true && schemaModel.Alias != null)
            {
                var resultDelete = await SQLServerService.Delete(schemaModel.Alias, id, source);
                var resultReturn = new
                {
                    result = resultDelete.deleted
                };
                return ResponseDefault(resultReturn);
            }

            var beforeSave = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(json).BeforeSave;

            if (beforeSave != null)
            {
                var action = _actionRepository.GetByField("name", beforeSave);

                if (action != null)
                {
                    ActionController x = new(null, null, _schemaRepository, _entityService, _actionRepository, null, _userService);
                    ApiEntities.Action[] ac = x.GetBySchemaName(schemaName);

                    var userHelper = _schemaRepository.GetUserHelper();
                    var user = _userService.GetByUserName(userHelper.GetTenanty(), userHelper.GetUserName());

                    string auth = Request.Headers["Authorization"];

                    var result = await x.Run(action, new Dictionary<string, object>()
                {
                    { "params", null }
                }, auth, user, null);
                }
            }

            var command = new DeleteEntityCommand { Id = id, SchemaName = schemaName };

            var entityDeleted = _entityService.GetById(command.SchemaName, command.Id);

            if (entityDeleted.Result == null) return BadRequest("Id não existe");

            await RunActionForSchema(schemaName, id.ToString());
            await SendCommand(command);
            var userA = _userAccessService.GetBySchemaRecordId(id);

            if (userA != null) _userAccessService.DeleteUserAccess(userA.Id);

            var afterSave = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(json).AfterSave;
            if (afterSave != null)
            {
                var action = _actionRepository.GetByField("name", afterSave);

                if (action != null)
                {
                    ActionController x = new(null, null, _schemaRepository, _entityService, _actionRepository, null, _userService);
                    ApiEntities.Action[] ac = x.GetBySchemaName(schemaName);

                    var userHelper = _schemaRepository.GetUserHelper();
                    var user = _userService.GetByUserName(userHelper.GetTenanty(), userHelper.GetUserName());

                    string auth = Request.Headers["Authorization"];

                    var result = await x.Run(action, new Dictionary<string, object>()
                    {
                        { "params", null }
                    }, auth, user, null);
                }
            }
            return ResponseDefault(entityDeleted);
        }
    }
}