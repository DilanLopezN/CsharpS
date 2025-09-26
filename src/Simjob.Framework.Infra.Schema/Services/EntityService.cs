using MediatR;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Entities;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Domain.Models;
using Simjob.Framework.Domain.Models.WeeklyScheduleModels;
using Simjob.Framework.Domain.Util;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Schemas.Entities;
using Simjob.Framework.Infra.Schemas.Interfaces;
using Simjob.Framework.Infra.Schemas.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Simjob.Framework.Domain.Models.JoinsModel;

namespace Simjob.Framework.Infra.Schemas.Services
{
    [ExcludeFromCodeCoverage]
    public class EntityService : IEntityService
    {
        private readonly ISchemaBuilder _schemaBuilder;
        private readonly IServiceProvider _serviceProvider;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;
        private readonly IAzureStorageService _azureStorageService;
        private readonly IHuaweiStorageService _huaweiStorageService;
        private readonly IAWSStorageService _awsStorageService;
        private readonly DomainNotificationHandler _notifications;
        private readonly IMediatorHandler _bus;


        public EntityService(ISchemaBuilder schemaBuilder, IServiceProvider serviceProvider,
            IRepository<MongoDbContext, Schema> schemaRepository, IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, IAzureStorageService azureStorageService, IHuaweiStorageService huaweiStorageService, IAWSStorageService awsStorageService)
        {
            _schemaBuilder = schemaBuilder;
            _serviceProvider = serviceProvider;
            _schemaRepository = schemaRepository;
            _bus = bus;
            _notifications = (DomainNotificationHandler)notifications;
            _azureStorageService = azureStorageService;
            _huaweiStorageService = huaweiStorageService;
            _awsStorageService = awsStorageService;
        }

        public Schema GetSchemaByName(string schemaName)
        {
            return _schemaRepository.GetSchemaByField("name", schemaName);
        }

        public async Task<bool> GetByFieldName(string schemaName, string field, string value)
        {
            var type = await _schemaBuilder.GetSchemaType(schemaName);
            var repository = GetRepository(type);

            var teste = repository
                .GetType()
                .GetMethod("GetByField")
                .Invoke(repository, new object[] { field, value });

            if (teste != null)
            {
                return true;
            }
            return false;
        }

        public object GetRepository(Type schemaType)
        {
            var typeRepo = typeof(IRepository<,>).MakeGenericType(typeof(MongoDbContext), schemaType);
            return _serviceProvider.GetService(typeRepo);
        }

        [ExcludeFromCodeCoverage]
        private async Task AddMirrors(string schemaName, Dictionary<string, object> obj)
        {
            var schema = GetSchemaByName(schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Domain.Models.SchemaModel>(schema.JsonValue);
            if (schemaModel.Properties == null) return;
            var propertiesRelation = schemaModel.Properties.Where(p => p.Value != null && p.Value.RelationSchema != null);
            if (!propertiesRelation.Any()) return;
            var propertiesMirror = schemaModel.Properties.Where(p => p.Value != null && p.Value.Mirror != null);

            foreach (var prop in propertiesMirror)
            {
                string[] propertiesInMirror = prop.Value.Mirror.Split('.');

                string propertyMirror = propertiesInMirror[0];

                var propValue = obj.FirstOrDefault(v => v.Key.ToLower() == propertyMirror.ToLower()).Value;
                if (propValue == null || string.IsNullOrEmpty(propValue.ToString())) continue;

                string propertyInMirror = propertiesInMirror[1];

                var propertyMirrorRelationSchemaName = propertiesRelation.FirstOrDefault(pr => pr.Key.ToLower() == propertyMirror.ToLower()).Value.RelationSchema;
                var propertyMirrorRelationType = await _schemaBuilder.GetSchemaType(propertyMirrorRelationSchemaName);
                var propertyMirrorRelationRepository = GetRepository(propertyMirrorRelationType);
                var type = propValue.GetType().Name;

                var schemaRelation = GetSchemaByName(propertyMirrorRelationSchemaName);
                var schemaRelationModel = JsonConvert.DeserializeObject<Domain.Models.SchemaModel>(schemaRelation.JsonValue);
      
                if (type == "JArray")
                {
                    List<string> propertyValues = ((IEnumerable)propValue).Cast<string>().Select(x => x).ToList();

                    dynamic itens = string.IsNullOrEmpty(schemaRelationModel.PrimaryKey) ? propertyMirrorRelationRepository.GetType().GetMethod("GetByIdsList").Invoke(propertyMirrorRelationRepository, new object[] { propertyValues }) : propertyMirrorRelationRepository.GetType().GetMethod("GetByFieldsIntList").Invoke(propertyMirrorRelationRepository, new object[] { "primaryKey",propertyValues.Select(p => int.Parse(p)).ToList()});

                    if (itens == null || itens.Count == 0)
                    {
                        if (!string.IsNullOrEmpty(schemaRelationModel.PrimaryKey))
                        {
                            await _bus.RaiseEvent(new DomainNotification(this.GetType().Name, $"Item in property mirror not found {propertyMirror}({propertyMirrorRelationSchemaName})  {schemaRelationModel.PrimaryKey}: {propValue}"));
                            return;
                        }

                        await _bus.RaiseEvent(new DomainNotification(this.GetType().Name, $"Item in property mirror not found {propertyMirror}({propertyMirrorRelationSchemaName})  id: {propValue}"));
                        return;
                    };
                    List<string> itemValues = new List<string>();
                    foreach (var i in itens)
                    {
                        object item = i;
                        var itemProperties = item.GetType().GetProperties();
                        var itemProperty = itemProperties.FirstOrDefault(p => p.Name.ToLower() == propertyInMirror.ToLower());
                        var itemValue = itemProperty.GetValue(item);
                        itemValues.Add(itemValue.ToString());

                    }

                    if (obj.Keys.FirstOrDefault(key => key == prop.Key) == null) obj.Add(prop.Key, null);

                    if (!itemValues.IsNullOrEmpty())
                    {
                        string itemValuesString = string.Join(",", itemValues);
                        obj[prop.Key] = itemValuesString;

                    }
                }
                else
                {
                    var item = string.IsNullOrEmpty(schemaRelationModel.PrimaryKey) ? propertyMirrorRelationRepository.GetType().GetMethod("GetById").Invoke(propertyMirrorRelationRepository, new object[] { propValue }) : propertyMirrorRelationRepository.GetType().GetMethod("GetByFieldInt").Invoke(propertyMirrorRelationRepository, new object[] { schemaRelationModel.PrimaryKey,Convert.ToInt32(propValue) });

                    if (item == null)
                    {
                        if (!string.IsNullOrEmpty(schemaRelationModel.PrimaryKey))
                        {
                            await _bus.RaiseEvent(new DomainNotification(this.GetType().Name, $"Item in property mirror not found {propertyMirror}({propertyMirrorRelationSchemaName})  {schemaRelationModel.PrimaryKey}: {propValue}"));
                            return;
                        }
                        await _bus.RaiseEvent(new DomainNotification(this.GetType().Name, $"Item in property mirror not found {propertyMirror}({propertyMirrorRelationSchemaName})  id: {propValue}"));
                        return;
                    };

                    var itemProperties = item.GetType().GetProperties();
                    var itemProperty = itemProperties.FirstOrDefault(p => p.Name.ToLower() == propertyInMirror.ToLower());
                    var itemValue = itemProperty.GetValue(item);
                    if (obj.Keys.FirstOrDefault(key => key == prop.Key) == null) obj.Add(prop.Key, null);
 
                    obj[prop.Key] = itemValue;
                }
            }

            return;
        }

        [ExcludeFromCodeCoverage]
        public async Task<object> GetRelationProperty(string schemaName, string id, string property, int? page, int? limit, string sortField = null, bool sortDesc = false)
        {
            var entity = await this.GetById(schemaName, id);

            if (entity == null)
            {
                await _bus.RaiseEvent(new DomainNotification(this.GetType().Name, "entity not found"));
                return null;
            }

            var propValue = entity.GetType().GetProperties().FirstOrDefault(p => p.Name.ToLower() == property.ToLower()).GetValue(entity);

            var schema = _schemaRepository.GetByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Domain.Models.SchemaModel>(schema.JsonValue);
            var propertyModel = schemaModel.Properties.FirstOrDefault(p => p.Key.ToLower() == property.ToLower());

            string relationSchema = propertyModel.Value.RelationSchema;
            var relationType = await _schemaBuilder.GetSchemaType(relationSchema);
            var relationRepo = GetRepository(relationType);

            bool isArray = propertyModel.Value.Type.ToLower() == "array";

            if (isArray)
            {
                var array = JsonConvert.DeserializeObject<string[]>(JsonConvert.SerializeObject(propValue));
                var items = relationRepo.GetType().GetMethod("GetByIds").Invoke(relationRepo, new object[] { array, page, limit, sortField, sortDesc, false });
                return items;
            }

            var value = propValue as string;
            var item = relationRepo.GetType().GetMethod("GetById").Invoke(relationRepo, new object[] { value });

            return item;
        }

        [ExcludeFromCodeCoverage]
        public async Task<object> GetAll(string schemaName, int? page, int? limit, string sortField = null, bool sortDesc = false, string ids = null)
        {
            var type = await _schemaBuilder.GetSchemaType(schemaName);
            var repository = GetRepository(type);
            return repository
                .GetType()
                .GetMethod("GetAll")
                .Invoke(repository, new object[] { page, limit, sortField, sortDesc, false, ids });
        }
        #region stock 
        public async Task<dynamic> GetAllItems(string schemaName, string? idItem, string? codigoItem)
        {
            var type = await _schemaBuilder.GetSchemaType(schemaName);
            var repository = GetRepository(type);
            return repository
                .GetType()
                .GetMethod("GetAllItems")
                .Invoke(repository, new object[] { idItem, codigoItem });
        }
        public async Task<dynamic> GetByIdsList(string schemaName, List<string> ids)
        {
            var type = await _schemaBuilder.GetSchemaType(schemaName);
            var repository = GetRepository(type);
            return repository
                .GetType()
                .GetMethod("GetByIdsList")
                .Invoke(repository, new object[] { ids });
        }
        public async Task<dynamic> GetAllPicking(string schemaName, string? codigo, string? localDestDescription, string? status, DateTime? dtInicio, DateTime? dtFinal, string sortField = null, bool sortDesc = false, string ids = null)
        {
            var type = await _schemaBuilder.GetSchemaType(schemaName);
            var repository = GetRepository(type);
            return repository
                .GetType()
                .GetMethod("GetAllPicking")
                .Invoke(repository, new object[] { codigo, localDestDescription, status, dtInicio, dtFinal, sortField, sortDesc, false, ids });
        }
        public async Task<dynamic> GetAllStockBalance(string schemaName, string? idItem, string? idLocal, string? codLocal, DateTime? dataInicioSaldo, DateTime? dataFinalSaldo)
        {
            var type = await _schemaBuilder.GetSchemaType(schemaName);
            var repository = GetRepository(type);
            return repository
                .GetType()
                .GetMethod("GetAllStockBalance")
                .Invoke(repository, new object[] { idItem, idLocal, codLocal, dataInicioSaldo, dataFinalSaldo });
        }
        public async Task<dynamic> GetAllStockBalanceForItemRepo(string schemaName, List<string> itemIds)
        {
            var type = await _schemaBuilder.GetSchemaType(schemaName);
            var repository = GetRepository(type);
            return repository
                .GetType()
                .GetMethod("GetAllStockBalanceForItemRepo")
                .Invoke(repository, new object[] { itemIds });
        }
        public async Task<dynamic> GetStockBalanceByLocalCodes(string schemaName, string[] localCodes)
        {
            var type = await _schemaBuilder.GetSchemaType(schemaName);
            var repository = GetRepository(type);
            return repository
                .GetType()
                .GetMethod("GetStockBalanceByLocalCodes")
                .Invoke(repository, new object[] { localCodes });
        }

        public async Task<dynamic> GetStockBalanceByItemId(string schemaName, string itemId)
        {
            var type = await _schemaBuilder.GetSchemaType(schemaName);
            var repository = GetRepository(type);
            return repository
                .GetType()
                .GetMethod("GetStockBalanceByItemId")
                .Invoke(repository, new object[] { itemId });
        }
        public async Task<dynamic> GetByIdsPickingItem(string schemaName, List<string> ids, string? lote)
        {
            var type = await _schemaBuilder.GetSchemaType(schemaName);
            var repository = GetRepository(type);
            return repository
                .GetType()
                .GetMethod("GetByIdsPickingItem")
                .Invoke(repository, new object[] { ids, lote });
        }
        public async Task<dynamic> GetByIdStockOperation(string schemaName, string? id, string? code, string? status)
        {
            var type = await _schemaBuilder.GetSchemaType(schemaName);
            var repository = GetRepository(type);
            return repository
                .GetType()
                .GetMethod("GetByIdStockOperation")
                .Invoke(repository, new object[] { id, code, status });
        }

        public async Task<dynamic> GetByIdNotaFiscal(string schemaName, string? id, string? numero, DateTime? dtInicio, DateTime? dtFinal, string? status)
        {
            var type = await _schemaBuilder.GetSchemaType(schemaName);
            var repository = GetRepository(type);
            return repository
                .GetType()
                .GetMethod("GetByIdNotaFiscal")
                .Invoke(repository, new object[] { id, numero, dtInicio, dtFinal, status });
        }
        public async Task<dynamic> GetAllNf(string schemaName, string nota, DateTime? dtInicio, DateTime? dtFinal, string status, string sortField = null, bool sortDesc = false, bool addIsDeleted = false, string ids = null)
        {
            var type = await _schemaBuilder.GetSchemaType(schemaName);
            var repository = GetRepository(type);
            return repository
                .GetType()
                .GetMethod("GetAllNf")
                .Invoke(repository, new object[] { nota, dtInicio, dtFinal, status, sortField, sortDesc, addIsDeleted, ids });
        }

        public async Task<dynamic> GetByNfConfirmacaoEntrega(string schemaName, string numero, DateTime? dtInicio, DateTime? dtFinal, string status)
        {
            var type = await _schemaBuilder.GetSchemaType(schemaName);
            var repository = GetRepository(type);
            return repository
                .GetType()
                .GetMethod("GetByNfConfirmacaoEntrega")
                .Invoke(repository, new object[] { numero, dtInicio, dtFinal, status });
        }
        public async Task<dynamic> GetByIdsStockMov(string schemaName, List<string> ids, string? codeExt, string? volCode)
        {
            var type = await _schemaBuilder.GetSchemaType(schemaName);
            var repository = GetRepository(type);
            return repository
                .GetType()
                .GetMethod("GetByIdsStockMov")
                .Invoke(repository, new object[] { ids, codeExt, volCode });
        }


        #endregion

        #region weeklySchedule

        public async Task<List<WeeklyScheduleReturnModel>> GetAllWeeklySchedule(string schemaName, string fornecedorId, string? ownerId, DateTime? dataInicio, DateTime? dataFim, string sortField = null, bool sortDesc = false)
        {
            var type = await _schemaBuilder.GetSchemaType(schemaName);
            var repository = GetRepository(type);
            dynamic weeklySchedules = repository
                .GetType()
                .GetMethod("GetWeeklySchedulePaginated")
                .Invoke(repository, new object[] { fornecedorId, ownerId, dataInicio, dataFim, sortDesc, sortField });

            if (weeklySchedules == null)
            {
                await _bus.RaiseEvent(new DomainNotification("Schema", "WeeklySchedule not found"));
                return null;
            }
            var jsonWeeklySchedule = JsonConvert.SerializeObject(weeklySchedules);
            List<WeeklyScheduleModel> weeklySchedulesList = JsonConvert.DeserializeObject<List<WeeklyScheduleModel>>(jsonWeeklySchedule);
            var groupedSchedules = weeklySchedulesList.Where(x => x.UsuarioId != null).GroupBy(x => x.UsuarioId).ToList();


            var newRetorno = groupedSchedules.Select(x => new WeeklyScheduleReturnModel
            {
                UsuarioId = x.Key,
                UsuarioNome = "",
                Schedules = x.ToList()
            }).ToList();
            return newRetorno;

        }
        #endregion

        [ExcludeFromCodeCoverage]
        public async Task<string> GetListSearchFields(string schemaName)
        {
            var schema = _schemaRepository.GetByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Domain.Models.SchemaModel>(schema.JsonValue);
            var propertiesNotRelation = schemaModel.Properties
                                            .Where(p => string.IsNullOrEmpty(p.Value.RelationSchema))
                                            .Select(p => p.Key)
                                            .ToList();

            var searchFields = string.Join(',', propertiesNotRelation);

            return searchFields;
        }


        public async Task<object> SerachFieldsJoin(string schemaName, JoinsModel model, int? page = null, int? limit = null, string sortField = null, bool sortDesc = false)
        {
            var type = await _schemaBuilder.GetSchemaType(schemaName);
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModelData = JsonConvert.DeserializeObject<Simjob.Framework.Infra.Domain.Models.SchemaModel>(schema.JsonValue);

            var repository = GetRepository(type);
            return repository
                .GetType()
                .GetMethod("SearchRegexByFieldsJoin")
                .Invoke(repository, new object[] { schemaModelData, model, page, limit, sortField, sortDesc });
        }

        [ExcludeFromCodeCoverage]
        public async Task<object> SerachFields(string userId, string schemaName, string searchFields, string value, string mode, int? page, int? limit, List<Framework.Domain.Models.Searchdefs.Filter> defs, List<bool> sortDesc, string sortField = null, string groupBy = null, string ids = null, string companySiteIds = null)
        {
            if (value == null) value = "";
            if (companySiteIds == null) companySiteIds = "";
            var type = await _schemaBuilder.GetSchemaType(schemaName);
            var repository = GetRepository(type);
            var schemas = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModelData = JsonConvert.DeserializeObject<Simjob.Framework.Infra.Domain.Models.SchemaModel>(schemas.JsonValue);
            var relationField = "";
            List<string> relationIds = new List<string>();
            //comentario
            if (schemaModelData != null)
            {
                if (schemaModelData.segmentationRelationSchema != null)
                {
                    relationField = schemaModelData.segmentationRelationSchema.searchField;
                    if (schemaModelData.Properties.ContainsKey(relationField))
                    {
                        if (schemaModelData.segmentationRelationSchema.filter == "userLogin")
                        {
                            var relationSchemaName = schemaModelData.Properties.Where(x => x.Key == relationField).FirstOrDefault().Value.RelationSchema;

                            var fieldUser = schemaModelData.segmentationRelationSchema.fieldSchema;
                            var typeRelation = await _schemaBuilder.GetSchemaType(relationSchemaName);
                            var repositoryRelation = GetRepository(typeRelation);
                            dynamic listRelationSchema = repositoryRelation
                            .GetType()
                            .GetMethod("GetAllRelationSchema")
                            .Invoke(repositoryRelation, new object[] { userId, fieldUser, page, limit, sortField, sortDesc[0], false, null });

                            foreach (var relationSchema in listRelationSchema.Data)
                            {
                                relationIds.Add(relationSchema.Id);
                            }
                            if (relationIds.Count() == 0) return repository;
                        }
                    }

                }
            }



            if (string.IsNullOrEmpty(searchFields) && value.Length > 0)
            {
                var schema = _schemaRepository.GetByField("name", schemaName);
                var schemaModel = JsonConvert.DeserializeObject<Domain.Models.SchemaModel>(schema.JsonValue);
                var propertiesNotRelation = schemaModel.Properties
                                                .Where(p => string.IsNullOrEmpty(p.Value.RelationSchema))
                                                .Select(p => p.Key)
                                                .ToList();

                searchFields = string.Join(',', propertiesNotRelation);
            }

            if (defs != null)
                return repository
                    .GetType()
                    .GetMethod("SearchRegexByFieldsDefs")
                    .Invoke(repository, new object[] { defs, sortDesc, searchFields, value, schemaModelData, page, limit, sortField, ids, companySiteIds });


            //else if (value.Contains(";"))
            //{
            //    return repository
            //        .GetType()
            //        .GetMethod("SearchRegexByFieldsSeg")
            //        .Invoke(repository, new object[] { searchFields, value, page, limit, sortField, sortDesc, ids });
            //}

            //else if (value.Contains(","))
            //{
            //    return repository
            //        .GetType()
            //        .GetMethod("SearchRegexByMultiFields")
            //        .Invoke(repository, new object[] { searchFields, value, page, limit, sortField, sortDesc, ids });
            //}


            else
            {
                return repository
                    .GetType()
                    .GetMethod("SearchRegexByFields")
                    .Invoke(repository, new object[] { searchFields, value, mode, schemaModelData, page, limit, sortField, groupBy, sortDesc[0], ids, relationIds, relationField, companySiteIds });
            }

            //foreach (dynamic v in lista)
            //{

            //    var getValue = v.GetType().GetProperty("Owners").GetValue(v, null);
            //    var ownersIds = String.Join(",", getValue);
            //}
        }

        [ExcludeFromCodeCoverage]
        public async Task<object> GetById(string schemaName, string id)
        {
            var type = await _schemaBuilder.GetSchemaType(schemaName);
            var repository = GetRepository(type);

            return repository
                .GetType()
                .GetMethod("GetById")
                .Invoke(repository, new object[] { id });
        }

        [ExcludeFromCodeCoverage]
        private async Task<bool> RelationPropertiesValidation(string schemaName, Dictionary<string, object> obj)
        {
            var schema = GetSchemaByName(schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Domain.Models.SchemaModel>(schema.JsonValue);
            if (schemaModel.Properties == null) return true;
            var propertiesRelation = schemaModel.Properties.Where(p => p.Value != null && p.Value.RelationSchema != null);
            if (!propertiesRelation.Any()) return true;

            foreach (var prop in propertiesRelation)
            {
                var propValue = obj.FirstOrDefault(v => v.Key == prop.Key);
                if (propValue.Value == null) continue;
                string relationSchema = prop.Value.RelationSchema;
                var type = await _schemaBuilder.GetSchemaType(relationSchema);
                var repo = GetRepository(type);

                List<string> teste = new();

                bool multiType = false;
                try
                {
                    multiType = prop.Value?.Type?.Type?.ToString() == "Array";
                }
                catch { };

                bool isArray = multiType ? ((JArray)prop.Value?.Type)
                                                .FirstOrDefault(t => t.Value<string>() == "array") != null :
                                            prop.Value?.Type.ToString() == "array";

                if (isArray)
                {
                    var array = JsonConvert.DeserializeObject<string[]>(JsonConvert.SerializeObject(propValue.Value));
                    var items = repo.GetType().GetMethod("GetByIds").Invoke(repo, new object[] { array, null, null, null, null, true });
                    //if (items.Data.ToList().Count != array.ToList().Count)
                    //{
                    //    await _bus.RaiseEvent(new DomainNotification(this.GetType().Name, $"Relation error \"{prop.Key}\", item(s) not found"));
                    //    return false;
                    //};

                    continue;
                }

                string value = propValue.Value as string;
                var res = repo.GetType().GetMethod("GetById").Invoke(repo, new object[] { value });
                if (res == null)
                {
                    await _bus.RaiseEvent(new DomainNotification(this.GetType().Name, $"Relation error \"{prop.Key}\", item(s) not found"));
                    return false;
                }
                continue;
            }

            return true;
        }


        public async Task<List<string>> UploadAttachs(List<dynamic> data, string bucketName = "")
        {
            var enc = new EncryptorDecryptor();
            var bucket = String.IsNullOrEmpty(bucketName) ? await SerachFields(null, "Bucket", "default", "Yes", null, 1, 1, null, new List<bool> { false }) : await SerachFields(null, "Bucket", "name", bucketName, null, 1, 1, null, new List<bool> { false });

            if (bucket == null)
            {
                await _bus.RaiseEvent(new DomainNotification(this.GetType().Name, $"Bucket Configuration not found"));
                return new List<string>();
            }

            try
            {
                Type type = bucket.GetType();
                PropertyInfo info = type.GetProperty("Data");
                var obj = (dynamic)info.GetValue(bucket, null);
                //var bucketConfigs = JsonConvert.DeserializeObject(enc.Decrypt(obj[0].Config));
                var bucketConfigs = JsonConvert.DeserializeObject(obj[0].Config);
                var typeBucket = Convert.ToString(obj[0].TypeBucket);
                var bcktName = Convert.ToString(obj[0].Name);
                var folder = Convert.ToString(bucketConfigs["Folder"]);
                var urls = new List<string>();
                var files = new List<StreamFileToUpload>();
                foreach (var file in data)
                {
                    try
                    {
                        var base64 = file.base64.ToString().Split(",");
                        var extesion = base64[0].Split(";");
                        var ext = extesion[0].Split("/")[1];
                        var stream = StorageService.Base64ToStream(base64[1]);
                        var fileName = folder + file.fileName;
                        var metadata = base64[0].Split(";");
                        var mimeType = metadata[0].Split(":")[1];
                        files.Add(new StreamFileToUpload() { FilePath = fileName, File = stream,  ContentType = mimeType});
                    }
                    catch (Exception ex)
                    {
                        Logs.AddLog("[StorageService] - " + ex.Message);
                        await _bus.RaiseEvent(new DomainNotification("StorageService", "Invalid file format"));
                        return default;
                    }
                }

                switch (typeBucket)
                {

                    case "Azure_BlobStorage":
                        var azureConfig = new AzureModel(bcktName, typeBucket, Convert.ToString(bucketConfigs["AzureConnectionString"]), Convert.ToString(bucketConfigs["AzureContainer"]), "10");

                        //foreach (var file in files)
                        //{
                        //    var res = await _azureStorageService.Upload(file.File, file.FilePath, azureConfig);
                        //    if (String.IsNullOrEmpty(res))
                        //    {
                        //        await _bus.RaiseEvent(new DomainNotification("StorageService", "Error to upload file"));
                        //        return default;
                        //    }
                        //    else
                        //    {
                        //        urls.Add(res);
                        //        Logs.AddLog("Urls uploadAttach - " + urls);
                        //    }


                        //}
                        //return urls;


                        var uploadTasks = files.Select(async file =>
                        {
                            var res = await _azureStorageService.Upload(file.File, file.FilePath, azureConfig,file.ContentType);
                            if (String.IsNullOrEmpty(res))
                            {
                                await _bus.RaiseEvent(new DomainNotification("StorageService", "Error to upload file"));
                                return null;
                            }
                            else
                            {
                                Logs.AddLog("Urls uploadAttach - " + res);
                                return res;
                            }
                        });

                        // Wait for all uploads to complete
                        var results = await Task.WhenAll(uploadTasks);
                        // Filter out any null results (failed uploads)
                        urls.AddRange(results.Where(result => result != null));

                        return urls;


                    case "Huawei_Cloud":
                        var huaweiConfig = new HuaweiModel(bcktName, typeBucket, Convert.ToString(bucketConfigs["EndPoint"]), Convert.ToString(bucketConfigs["Accesskey"]), Convert.ToString(bucketConfigs["SecretAccessKey"]), Convert.ToString(bucketConfigs["Folder"]));
                        //foreach (var file in files)
                        //{
                        //    var res = _huaweiStorageService.Upload(huaweiConfig, file.FilePath, file.File);
                        //    if (String.IsNullOrEmpty(res))
                        //    {
                        //        await _bus.RaiseEvent(new DomainNotification("StorageService", "Error to upload file"));
                        //        return default;
                        //    }
                        //    else
                        //    {
                        //        urls.Add(res);
                        //    }
                        //}
                        var uploadTasksHuawei = files.Select(async file =>
                        {
                            var res = _huaweiStorageService.Upload(huaweiConfig, file.FilePath, file.File);
                            if (String.IsNullOrEmpty(res))
                            {
                                await _bus.RaiseEvent(new DomainNotification("StorageService", "Error to upload file"));
                                return null;
                            }
                            else
                            {
                                Logs.AddLog("Urls uploadAttach - " + res);
                                return res;
                            }
                        });

                        // Wait for all uploads to complete
                        var resultsHuawei = await Task.WhenAll(uploadTasksHuawei);
                        // Filter out any null results (failed uploads)
                        urls.AddRange(resultsHuawei.Where(result => result != null));
                        return urls;

                    case "Amazon_S3":
                        var awsConfig = new AWSModel(bcktName, typeBucket, Convert.ToString(bucketConfigs["AccessKey"]), Convert.ToString(bucketConfigs["SecretAccessKey"]), Convert.ToString(bucketConfigs["RegionEndPoint"]), Convert.ToString(bucketConfigs["Folder"]));
                        //foreach (var file in files)
                        //{
                        //    var res = await _awsStorageService.Upload(awsConfig, file.FilePath, file.File);
                        //    if (String.IsNullOrEmpty(res))
                        //    {
                        //        await _bus.RaiseEvent(new DomainNotification("StorageService", "Error to upload file"));
                        //        return default;
                        //    }
                        //    else
                        //    {
                        //        urls.Add(res);

                        //    }
                        //}
                        var uploadTasksAWS = files.Select(async file =>
                        {
                            var res = await _awsStorageService.Upload(awsConfig, file.FilePath, file.File,file.ContentType);
                            if (String.IsNullOrEmpty(res))
                            {
                                await _bus.RaiseEvent(new DomainNotification("StorageService", "Error to upload file"));
                                return null;
                            }
                            else
                            {
                                Logs.AddLog("Urls uploadAttach - " + res);
                                return res;
                            }
                        });

                        // Wait for all uploads to complete
                        var resultsAWS = await Task.WhenAll(uploadTasksAWS);
                        // Filter out any null results (failed uploads)
                        urls.AddRange(resultsAWS.Where(result => result != null));

                        return urls;

                    default:
                        return default;
                }

            }
            catch (Exception ex)
            {
                Logs.AddLog("[EntityService] - " + ex.Message);
                await _bus.RaiseEvent(new DomainNotification(this.GetType().Name, $"Bucket configuration format error"));
                return new List<string>();
            }
        }

        [ExcludeFromCodeCoverage]
        public async Task<Entity> Insert(string schemaName, Dictionary<string, object> data)
        {
            var type = await _schemaBuilder.GetSchemaType(schemaName);
            var repository = GetRepository(type);

            //if (schemaName == "Bucket")
            //{
            //    var enc = new EncryptorDecryptor();
            //    var d = data.Where(x => x.Key == "config").FirstOrDefault();
            //    data["config"] = (object)enc.Encrypt(data["config"].ToString());
            //}

            var schemas = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModelData = JsonConvert.DeserializeObject<Simjob.Framework.Infra.Domain.Models.SchemaModel>(schemas.JsonValue);

            //Index
            //var indexFields = new List<string>();
            //if (!schemaModelData.SearchFields.IsNullOrEmpty()) indexFields.AddRange(schemaModelData.SearchFields.Split(',').ToList());
            //if (!schemaModelData.FilterFields.IsNullOrEmpty()) indexFields.AddRange(schemaModelData.FilterFields.Split(',').ToList());
            //if (!indexFields.IsNullOrEmpty()) indexFields = indexFields.Distinct().ToList();
            var properties = schemaModelData.Properties;
            var fileFields = properties.Where(x => x.Value.file != null).ToList();

            //var schemaProp = JsonConvert.DeserializeObject<Simjob.Framework.Infra.Domain.Models.SchemaModel>(schemas.JsonValue).Properties;

            

            await AddMirrors(schemaName, data);

            // Remove todos os campos que estão passando valor null
            data.Where(k => k.Value == null).ToList().ForEach(k => data.Remove(k.Key));
            List<string> ownerIds = new List<string>();
            if (data.ContainsKey("owners"))
            {
                if (data["owners"].ToString().Contains("userName") || data["owners"].ToString().Contains("userId"))
                {
                    var ownerValues = JsonConvert.SerializeObject(data["owners"]);
                    var ownerValuesDes = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(ownerValues);

                    ownerIds = ownerValuesDes
                        .Select(owner => owner.TryGetValue("userId", out string userId) ? userId : null)
                        .Where(userId => userId != null)
                        .ToList();

                    data["owners"] = ownerIds;
                }

            }
            //Valida se existem campos do tipo file, se sim, busca o storage e realiza o upload dos arquivos
            if (fileFields != null && fileFields.Count > 0)
            {
                Logs.AddLog("fileFieldsCount" + fileFields.Count);


                foreach (var field in fileFields)
                {

                    if (data.ContainsKey(field.Key))
                    {
                        var z = field.Value.file;
                        var bucketInfosJson = JsonConvert.SerializeObject(z);
                        var bucketInfosDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(bucketInfosJson);
                        var bucketName = bucketInfosDic["bucketName"];
                        var dataValue = data[field.Key];
                        var files = JsonConvert.DeserializeObject<List<dynamic>>(dataValue.ToString());
                        var urls = await UploadAttachs(files, bucketName);
                        data[field.Key] = (object)urls;
                    }
                }
            }

            string json = JsonConvert.SerializeObject(data, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            try
            {
                var entity = JsonConvert.DeserializeObject(json, type);

                //return (Entity)repository
                //     .GetType()
                //     .GetMethod("Insert")
                //     .Invoke(repository, new object[] { entity, indexFields, true });

                var result = await Task.Run(() =>
                     repository
                     .GetType()
                     .GetMethod("Insert")
                     .Invoke(repository, new object[] { entity })
                   );

                // Do something with the result if needed

                return (Entity)result;

            }
            catch (Exception ex)
            {
                Logs.AddLog("[EntityService] - " + ex.Message);
                return default;
            }

            // Comentado para testes
            //if (_notifications.HasNotification()) return null;
        }
        public async Task<List<Entity>> InsertManyBalance(string schemaName, List<Dictionary<string, object>> datas)
        {
            var type = await _schemaBuilder.GetSchemaType(schemaName);
            var repository = GetRepository(type);
            //foreach (var data in datas)
            //{
            //    await AddMirrors(schemaName, data);
            //    data.Where(k => k.Value == null).ToList().ForEach(k => data.Remove(k.Key));
            //    List<string> ownerIds = new List<string>();
            //    if (data.ContainsKey("owners"))
            //    {
            //        if (data["owners"].ToString().Contains("userName") || data["owners"].ToString().Contains("userId"))
            //        {
            //            var ownerValues = JsonConvert.SerializeObject(data["owners"]);
            //            var ownerValuesDes = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(ownerValues);

            //            ownerIds = ownerValuesDes
            //                .Select(owner => owner.TryGetValue("userId", out string userId) ? userId : null)
            //                .Where(userId => userId != null)
            //                .ToList();

            //            data["owners"] = ownerIds;
            //        }

            //    }
            //}

            try
            {
                string json = JsonConvert.SerializeObject(datas, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                var entityType = type; // Assume 'type' is the actual type you want to use
                var listType = typeof(List<>).MakeGenericType(entityType);

                var entities = JsonConvert.DeserializeObject(json, listType);
                //return (Entity)repository
                //     .GetType()
                //     .GetMethod("Insert")
                //     .Invoke(repository, new object[] { entity, indexFields, true });

                var result = await Task.Run(() =>
                     repository
                     .GetType()
                     .GetMethod("InsertManyEntity")
                     .Invoke(repository, new object[] { entities })
                   );

                // Do something with the result if needed

                return (List<Entity>)result;

            }
            catch (Exception ex)
            {
                Logs.AddLog("[EntityService] - " + ex.Message);
                return default;
            }
        }
        public async Task<List<Entity>> InsertMany(string schemaName, List<Dictionary<string, object>> datas)
        {
            var type = await _schemaBuilder.GetSchemaType(schemaName);
            var repository = GetRepository(type);
            foreach (var data in datas)
            {
                await AddMirrors(schemaName, data);
                data.Where(k => k.Value == null).ToList().ForEach(k => data.Remove(k.Key));
                List<string> ownerIds = new List<string>();
                if (data.ContainsKey("owners"))
                {
                    if (data["owners"].ToString().Contains("userName") || data["owners"].ToString().Contains("userId"))
                    {
                        var ownerValues = JsonConvert.SerializeObject(data["owners"]);
                        var ownerValuesDes = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(ownerValues);

                        ownerIds = ownerValuesDes
                            .Select(owner => owner.TryGetValue("userId", out string userId) ? userId : null)
                            .Where(userId => userId != null)
                            .ToList();

                        data["owners"] = ownerIds;
                    }

                }
            }

            try
            {
                string json = JsonConvert.SerializeObject(datas, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                var entityType = type; // Assume 'type' is the actual type you want to use
                var listType = typeof(List<>).MakeGenericType(entityType);

                var entities = JsonConvert.DeserializeObject(json, listType);
                //return (Entity)repository
                //     .GetType()
                //     .GetMethod("Insert")
                //     .Invoke(repository, new object[] { entity, indexFields, true });

                var result = await Task.Run(() =>
                     repository
                     .GetType()
                     .GetMethod("InsertManyEntity")
                     .Invoke(repository, new object[] { entities })
                   );

                // Do something with the result if needed

                return (List<Entity>)result;

            }
            catch (Exception ex)
            {
                Logs.AddLog("[EntityService] - " + ex.Message);
                return default;
            }

        }

        public async Task<List<Entity>> UpdateMany(string schemaName, List<Dictionary<string, object>> datas)
        {
            var type = await _schemaBuilder.GetSchemaType(schemaName);
            var repository = GetRepository(type);


            var schemas = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModelData = JsonConvert.DeserializeObject<Simjob.Framework.Infra.Domain.Models.SchemaModel>(schemas.JsonValue);

            foreach (var data in datas)
            {
                await AddMirrors(schemaName, data);

                data.Where(k => k.Value == null).ToList().ForEach(k => data.Remove(k.Key));
                List<string> ownerIds = new List<string>();
                if (data.ContainsKey("owners"))
                {
                    if (data["owners"].ToString().Contains("userName") || data["owners"].ToString().Contains("userId"))
                    {
                        var ownerValues = JsonConvert.SerializeObject(data["owners"]);
                        var ownerValuesDes = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(ownerValues);

                        ownerIds = ownerValuesDes
                            .Select(owner => owner.TryGetValue("userId", out string userId) ? userId : null)
                            .Where(userId => userId != null)
                            .ToList();

                        data["owners"] = ownerIds;
                    }
                }

            }
            try
            {
                string json = JsonConvert.SerializeObject(datas, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                var entityType = type; // Assume 'type' is the actual type you want to use
                var listType = typeof(List<>).MakeGenericType(entityType);

                var entities = JsonConvert.DeserializeObject(json, listType);


                var result = await Task.Run(() =>
                repository
                .GetType()
                .GetMethod("UpdateManyEntity")
                .Invoke(repository, new object[] { entities })
              );

                // Do something with the result if needed

                return (List<Entity>)result;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }
        [ExcludeFromCodeCoverage]
        public async Task<Entity> Update(string schemaName, Dictionary<string, object> data)
        {
            var type = await _schemaBuilder.GetSchemaType(schemaName);
            var repository = GetRepository(type);


            var schemas = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModelData = JsonConvert.DeserializeObject<Simjob.Framework.Infra.Domain.Models.SchemaModel>(schemas.JsonValue);

            var properties = schemaModelData.Properties;
            var fileFields = properties.Where(x => x.Value.file != null).ToList();
            await AddMirrors(schemaName, data);

            data.Where(k => k.Value == null).ToList().ForEach(k => data.Remove(k.Key));
            List<string> ownerIds = new List<string>();
            if (data.ContainsKey("owners"))
            {
                if (data["owners"].ToString().Contains("userName") || data["owners"].ToString().Contains("userId"))
                {
                    var ownerValues = JsonConvert.SerializeObject(data["owners"]);
                    var ownerValuesDes = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(ownerValues);

                    ownerIds = ownerValuesDes
                        .Select(owner => owner.TryGetValue("userId", out string userId) ? userId : null)
                        .Where(userId => userId != null)
                        .ToList();

                    data["owners"] = ownerIds;
                }
            }
            //TO DO : Refatorar este trecho
            //Verifica se existem campos do tipo file
            if (fileFields.Count > 0)
            {
                foreach (var field in fileFields)
                {
                    if (data.ContainsKey(field.Key))
                    {
                        var z = field.Value.file;
                        var bucketInfosJson = JsonConvert.SerializeObject(z);
                        var bucketInfosDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(bucketInfosJson);
                        var bucketName = bucketInfosDic["bucketName"];
                        var dataValue = data[field.Key];
                        var currentUrls = new List<string>();
                        var urls = new List<string>();
                        if (dataValue.ToString().Contains("data:"))
                        {
                            var files = JsonConvert.DeserializeObject<List<dynamic>>(dataValue.ToString());

                            var filesToUpload = files.Where(x => x.ToString().Contains("data:")).ToList();
                            files = files.Where(x => x.ToString().Contains("http")).ToList();

                            List<string> httpFiles = files
                            .Where(item => item is string)
                            .Select(item => (string)item)
                            .ToList();

                            currentUrls.AddRange(httpFiles);
                            urls = await UploadAttachs(filesToUpload, bucketName);
                            currentUrls.AddRange(urls);
                            data[field.Key] = (object)currentUrls;
                        }
                    }
                }


            }
            string json = JsonConvert.SerializeObject(data, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            try
            {
                var entity = JsonConvert.DeserializeObject(json, type);
                //       Logs.AddLog("entity -" + entity);
                return (Entity)repository
                        .GetType()
                        .GetMethod("Update")
                        .Invoke(repository, new object[] { entity });
            }
            catch (Exception e)
            {
                string msg = e.Message;
                return null;
            }
        }



        [ExcludeFromCodeCoverage]
        public async Task Delete(string id, string schemaName)
        {
            var type = await _schemaBuilder.GetSchemaType(schemaName);
            var repository = GetRepository(type);
            repository
                .GetType()
                .GetMethod("Delete")
                .Invoke(repository, new object[] { id });
        }


        public async Task DeleteAll(string schemaName)
        {
            var type = await _schemaBuilder.GetSchemaType(schemaName);
            var repository = GetRepository(type);
            repository
                .GetType()
                .GetMethod("DeleteAll")
                .Invoke(repository, new object[] { });
        }
        [ExcludeFromCodeCoverage]
        public async Task<Dictionary<string, string>> GetAutoinc(string schemaName, string[] fieldAutoInc)
        {

            var type = await _schemaBuilder.GetSchemaType(schemaName);
            var repository = GetRepository(type);
            var dicti = new Dictionary<string, string>();

            foreach (var field in fieldAutoInc)
            {

                var schemaObj = repository
                .GetType()
                .GetMethod("GetAutoInc")
                .Invoke(repository, new object[] { schemaName, field });

                var valueNumAutoInc = 0;
                if (schemaObj != null)
                {
                    dynamic valueAutoinc = schemaObj.GetType().GetProperty(field).GetValue(schemaObj);
                    var fieldAutoI = Convert.ToString(valueAutoinc);
                    var valueStr = Regex.Replace(fieldAutoI, @"\d", "");
                    var valueNumber = Regex.Replace(fieldAutoI, @"\D", "");

                    if (valueNumber.Length > 0)
                    {
                        valueNumAutoInc = Convert.ToInt32(valueNumber) + 1;
                    }
                    else
                    {
                        valueNumAutoInc += 1;
                    }

                    var newAutoInc = valueStr + valueNumAutoInc.ToString();

                    dicti.Add(field, newAutoInc);
                }
                else
                {
                    return null;
                }

            }

            return dicti;

        }

        public async Task<string> GetItems(string schemaName, string arrayField, string id = null)
        {
            string ids = null;
            if (id != null)
            {
                dynamic itemSchema = await GetById(schemaName, id);
                if (itemSchema != null)
                {
                    PropertyInfo propertyInfo = itemSchema.GetType().GetProperty(arrayField, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo != null)
                    {
                        var fieldValue = propertyInfo.GetValue(itemSchema, null);
                        if (fieldValue != null && fieldValue.Count > 0)
                        {
                            var fieldValues = ((IEnumerable<object>)fieldValue).Select(x => x.ToString());
                            ids = string.Join(",", fieldValues);
                        }
                    }
                }

            }
            return ids;
        }
        public async Task<dynamic> GetByField(string schemaName, string field, string value)
        {
            var type = await _schemaBuilder.GetSchemaType(schemaName);
            var repository = GetRepository(type);

            dynamic schema = repository
                .GetType()
                .GetMethod("GetByField")
                .Invoke(repository, new object[] { field, value });

            return schema;
        }


        public async Task<dynamic> GetListByField(string schemaName, string field, string value)
        {
            var type = await _schemaBuilder.GetSchemaType(schemaName);
            var repository = GetRepository(type);

            dynamic schema = repository
                .GetType()
                .GetMethod("GetListByField")
                .Invoke(repository, new object[] { field, value });

            return schema;
        }
        public async Task<dynamic> GetItemsJoin(JoinsModel model, dynamic data)
        {


            //else
            //{
            //    foreach (var item in data.Data)
            //    {
            //        var dynamicObject = new ExpandoObject() as IDictionary<string, object>;
            //        foreach (var property in item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            //        {
            //            dynamicObject[ToCamelCase(property.Name)] = property.GetValue(item);
            //        }


            //        foreach (var join in model.Joins)
            //        {
            //            var itemObject = (object)item;
            //            var type = itemObject.GetType();
            //            var property = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            //                               .FirstOrDefault(p => string.Equals(p.Name, join.ForeignKey, StringComparison.OrdinalIgnoreCase));
            //            object foreignFieldValue = null;
            //            if (property != null)
            //            {
            //                foreignFieldValue = property.GetValue(itemObject);
            //            }
            //            if (join.Schema != null)
            //            {
            //                if (schemasPermission != null && schemasPermission.Contains(join.Schema))
            //                {
            //                    dynamic itemSchemas = await GetListByField(join.Schema, join.Key, (string)foreignFieldValue);
            //                    if (itemSchemas.Count > 0 && string.Equals(join.ForeignKey, property.Name, StringComparison.OrdinalIgnoreCase))
            //                    {

            //                        List<IDictionary<string, object>> dynamicItems = new List<IDictionary<string, object>>();
            //                        foreach (var itemSchema in itemSchemas)
            //                        {
            //                            var dynamicItemSchema = new ExpandoObject() as IDictionary<string, object>;
            //                            foreach (var propertyItem in itemSchema.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            //                            {
            //                                if (join.Fields != null)
            //                                {
            //                                    if (join.Fields.ToLower().Contains(propertyItem.Name.ToLower()))

            //                                    {
            //                                        dynamicItemSchema[ToCamelCase(propertyItem.Name)] = propertyItem.GetValue(itemSchema);
            //                                    }

            //                                }
            //                                else
            //                                {
            //                                    dynamicItemSchema[ToCamelCase(propertyItem.Name)] = propertyItem.GetValue(itemSchema);
            //                                }

            //                            }
            //                            dynamicItems.Add(dynamicItemSchema);

            //                        }
            //                        dynamicObject[ToCamelCase(property.Name)] = dynamicItems;


            //                    }
            //                }
            //            }
            //        }

            //        items.Add(dynamicObject);


            //    }
            //}
            List<dynamic> items = new List<dynamic>();
            items.AddRange(data.Data);
            if (!model.Where.IsNullOrEmpty() && !model.WhereOr.IsNullOrEmpty()) items = items.Where(item => CheckSearchConditions(item, model.Where, model.WhereOr)).ToList();

            if (!model.Where.IsNullOrEmpty() && model.WhereOr.IsNullOrEmpty()) items = items.Where(item => CheckSearchConditions(item, model.Where)).ToList();

            if (!model.WhereOr.IsNullOrEmpty() && model.Where.IsNullOrEmpty()) items = items.Where(item => CheckSearchConditionsOr(item, model.WhereOr)).ToList();


            if (model.Order != null)
            {
                if (model.Order.Sort == "desc")
                {
                    items = items.OrderByDescending(item => GetPropertyValue(item, model.Order.Field)).ToList();
                }
                else
                {
                    items = items.OrderBy(item => GetPropertyValue(item, model.Order.Field)).ToList();
                }
            }
            return items;
        }
        #region OrderGetItems
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
        #endregion
        #region SearchGetItems
        public static bool CheckSearchConditions(IDictionary<string, object> expandoObject, IEnumerable<WhereCondition> searchConditions, IEnumerable<WhereCondition> searchOrCondition)
        {
            bool andConditions = searchConditions.All(condition => CheckProperty(expandoObject, condition.Field, condition.Operation, condition.Value));
            bool orCondition = searchOrCondition.Any(condition => CheckProperty(expandoObject, condition.Field, condition.Operation, condition.Value));
            return andConditions && orCondition;
        }

        public static bool CheckSearchConditions(IDictionary<string, object> expandoObject, IEnumerable<WhereCondition> searchConditions)
        {
            return searchConditions.All(condition => CheckProperty(expandoObject, condition.Field, condition.Operation, condition.Value));
        }

        public static bool CheckSearchConditionsOr(IDictionary<string, object> expandoObject, IEnumerable<WhereCondition> searchOrCondition)
        {
            return searchOrCondition.Any(condition => CheckProperty(expandoObject, condition.Field, condition.Operation, condition.Value));
        }

        // Custom function to check if the value of a property (either standalone or nested) of a dynamic object (ExpandoObject) matches the search condition
        public static bool CheckProperty(IDictionary<string, object> expandoObject, string propertyName, string operation, object searchValue)
        {
            var properties = propertyName.Split('.');
            var value = expandoObject;
            string[] properties2 = null;
            if (searchValue.ToString().StartsWith("$"))
            {
                var fieldString2 = searchValue.ToString();
                properties2 = fieldString2.Split("$")[1].Split(".");
            }
            var count = 0;
            foreach (var property in properties)
            {
                if (value.TryGetValue(property, out var nestedValue))
                {
                    if (nestedValue is IDictionary<string, object> nestedObject)
                    {
                        value = nestedObject;
                        value.TryGetValue(properties[1], out var arrayfieldValue);
                        return CompareValues(arrayfieldValue, operation, searchValue);

                    }
                    else
                    {
                        if (properties2 != null)
                        {
                            if (value.TryGetValue(properties2[count], out var nestedValue2))
                            {
                                return CompareValues(nestedValue, operation, nestedValue2);
                            }
                        }
                        return CompareValues(nestedValue, operation, searchValue); // Compare the single value
                    }
                }

                else
                {
                    // If any part of the property path doesn't exist, return false
                    return false;
                }
                count++;
            }
            return false; // If it's an object (nested property), return false
        }

        // Custom function to compare two values based on the operation
        public static bool CompareValues(object value1, string operation, object value2)
        {
            if ((operation != "==" && operation != "!=") && ((value1 == null || value1.ToString() == "") || (value2 == null || value2.ToString() == ""))) return false;
            switch (operation)
            {
                case "like":

                    return value1.ToString().ToLower().Contains(value2.ToString().ToLower());
                case "==":
                    if (value1 == "" && value2 == "")
                    {
                        // Your additional conditions or logic here
                        return Equals(value1, value2) || Equals(null, null);
                    }
                    return Equals(value1, value2);
                // Other code for the "== case" if the above condition is not met


                case "!=":
                    if (value1 == "" && value2 == "")
                    {
                        // Your additional conditions or logic here
                        return !Equals(value1, value2) || !Equals(null, null);
                    }
                    return !Equals(value1, value2);
                // Other code for the "!=" case" if the above condition is not met

                case "<":
                    try
                    {
                        return Convert.ToDateTime(value1).Date < Convert.ToDateTime(value2).Date;
                    }
                    catch
                    {
                        return Convert.ToDouble(value1) < Convert.ToDouble(value2);
                    }
                case ">":
                    try
                    {

                        return Convert.ToDateTime(value1).Date > Convert.ToDateTime(value2).Date;
                    }
                    catch
                    {
                        return Convert.ToDouble(value1) > Convert.ToDouble(value2);
                    }

                case "<=":
                    try
                    {
                        return Convert.ToDateTime(value1).Date <= Convert.ToDateTime(value2).Date;
                    }
                    catch
                    {
                        return Convert.ToDouble(value1) <= Convert.ToDouble(value2);
                    }
                case ">=":
                    try
                    {
                        return Convert.ToDateTime(value1).Date >= Convert.ToDateTime(value2).Date;
                    }
                    catch
                    {
                        return Convert.ToDouble(value1) >= Convert.ToDouble(value2);
                    }
                case "between":
                    try
                    {
                        var dateTimeValue1 = Convert.ToDateTime(value1);
                        List<string> listSearchDates = value2.ToString().Split(',').ToList();
                        // Check if the first datetime value is between the second and third datetime values
                        return dateTimeValue1 >= Convert.ToDateTime(listSearchDates[0]).Date && dateTimeValue1 <= Convert.ToDateTime(listSearchDates[1]).Date;
                    }
                    catch
                    {
                        return false;
                    }

                case "in":
                    List<string> listSearchIn = value2.ToString().Split(',').ToList();
                    return listSearchIn.Contains(value1.ToString());
                default:
                    throw new ArgumentException("Invalid operation");
            }
        }

        #endregion
        private string ToCamelCase(string name)
        {
            return char.ToLowerInvariant(name[0]) + name.Substring(1);
        }

        public async Task<object> GetByFieldNameCompany(string schemaName, string field, string value, string companySiteId)
        {
            var type = await _schemaBuilder.GetSchemaType(schemaName);

            var repository = GetRepository(type);
            return repository
                .GetType()
                .GetMethod("GetByFieldCompany")
                .Invoke(repository, new object[] { field, value, companySiteId });
        }

        public async Task<Dictionary<string, object>> AddMirrorsSql(string schemaName, Dictionary<string, object> obj)
        {
            var schema = GetSchemaByName(schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Domain.Models.SchemaModel>(schema.JsonValue);
            if (schemaModel.Properties == null) return obj;
            var propertiesRelation = schemaModel.Properties.Where(p => p.Value != null && p.Value.RelationSchema != null);
            if (!propertiesRelation.Any()) return obj;
            var propertiesMirror = schemaModel.Properties.Where(p => p.Value != null && p.Value.Mirror != null);

            foreach (var prop in propertiesMirror)
            {
                string[] propertiesInMirror = prop.Value.Mirror.Split('.');

                string propertyMirror = propertiesInMirror[0];

                var propValue = obj.FirstOrDefault(v => v.Key.ToLower() == propertyMirror.ToLower()).Value;
                if (propValue == null || string.IsNullOrEmpty(propValue.ToString())) continue;

                string propertyInMirror = propertiesInMirror[1];

                var propertyMirrorRelationSchemaName = propertiesRelation.FirstOrDefault(pr => pr.Key.ToLower() == propertyMirror.ToLower()).Value.RelationSchema;
                var propertyMirrorRelationType = await _schemaBuilder.GetSchemaType(propertyMirrorRelationSchemaName);
                var propertyMirrorRelationRepository = GetRepository(propertyMirrorRelationType);
                var type = propValue.GetType().Name;

                var schemaRelation = GetSchemaByName(propertyMirrorRelationSchemaName);
                var schemaRelationModel = JsonConvert.DeserializeObject<Domain.Models.SchemaModel>(schemaRelation.JsonValue);

                if (type == "JArray")
                {
                    List<string> propertyValues = ((IEnumerable)propValue).Cast<string>().Select(x => x).ToList();

                    dynamic itens = string.IsNullOrEmpty(schemaRelationModel.PrimaryKey) ? propertyMirrorRelationRepository.GetType().GetMethod("GetByIdsList").Invoke(propertyMirrorRelationRepository, new object[] { propertyValues }) : propertyMirrorRelationRepository.GetType().GetMethod("GetByFieldsIntList").Invoke(propertyMirrorRelationRepository, new object[] { "primaryKey", propertyValues.Select(p => int.Parse(p)).ToList() });

                    if (itens == null || itens.Count == 0)
                    {
                        if (!string.IsNullOrEmpty(schemaRelationModel.PrimaryKey))
                        {
                            await _bus.RaiseEvent(new DomainNotification(this.GetType().Name, $"Item in property mirror not found {propertyMirror}({propertyMirrorRelationSchemaName})  {schemaRelationModel.PrimaryKey}: {propValue}"));
                            return null;
                        }

                        await _bus.RaiseEvent(new DomainNotification(this.GetType().Name, $"Item in property mirror not found {propertyMirror}({propertyMirrorRelationSchemaName})  id: {propValue}"));
                        return null;
                    }
                    ;
                    List<string> itemValues = new List<string>();
                    foreach (var i in itens)
                    {
                        object item = i;
                        var itemProperties = item.GetType().GetProperties();
                        var itemProperty = itemProperties.FirstOrDefault(p => p.Name.ToLower() == propertyInMirror.ToLower());
                        var itemValue = itemProperty.GetValue(item);
                        itemValues.Add(itemValue.ToString());

                    }

                    if (obj.Keys.FirstOrDefault(key => key == prop.Key) == null) obj.Add(prop.Key, null);

                    if (!itemValues.IsNullOrEmpty())
                    {
                        string itemValuesString = string.Join(",", itemValues);
                        obj[prop.Key] = itemValuesString;

                    }
                }
                else
                {
                    var item = string.IsNullOrEmpty(schemaRelationModel.PrimaryKey) ? propertyMirrorRelationRepository.GetType().GetMethod("GetById").Invoke(propertyMirrorRelationRepository, new object[] { propValue }) : propertyMirrorRelationRepository.GetType().GetMethod("GetByFieldInt").Invoke(propertyMirrorRelationRepository, new object[] { schemaRelationModel.PrimaryKey, Convert.ToInt32(propValue) });

                    if (item == null)
                    {
                        if (!string.IsNullOrEmpty(schemaRelationModel.PrimaryKey))
                        {
                            await _bus.RaiseEvent(new DomainNotification(this.GetType().Name, $"Item in property mirror not found {propertyMirror}({propertyMirrorRelationSchemaName})  {schemaRelationModel.PrimaryKey}: {propValue}"));
                            return null;
                        }
                        await _bus.RaiseEvent(new DomainNotification(this.GetType().Name, $"Item in property mirror not found {propertyMirror}({propertyMirrorRelationSchemaName})  id: {propValue}"));
                        return null;
                    }
                    ;

                    var itemProperties = item.GetType().GetProperties();
                    var itemProperty = itemProperties.FirstOrDefault(p => p.Name.ToLower() == propertyInMirror.ToLower());
                    var itemValue = itemProperty.GetValue(item);
                    if (obj.Keys.FirstOrDefault(key => key == prop.Key) == null) obj.Add(prop.Key, null);

                    obj[prop.Key] = itemValue;
                }
            }

            return obj;
        }
    }
}