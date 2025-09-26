using MongoDB.Driver;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Domain.Util;
using Simjob.Framework.Infra.Identity.Commands;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Services
{
    public class SharedSchemaRecordService : ISharedSchemaRecordService
    {

        protected readonly SharedSchemaRecordContext Context;
        protected readonly IMongoCollection<SharedSchemaRecord> Collection;

        private readonly IRepository<SharedSchemaRecordContext, SharedSchemaRecord> _sharedSchemaRecordRepository;
        private readonly INotificationService _notificationService;
        public SharedSchemaRecordService(SharedSchemaRecordContext context, IRepository<SharedSchemaRecordContext, SharedSchemaRecord> sharedSchemaRecordRepository, INotificationService notificationService)
        {
            Context = context;
            Collection = context.GetUserCollection();
            _sharedSchemaRecordRepository = sharedSchemaRecordRepository;
            _notificationService = notificationService;
        }

        public void Delete(string id)
        {
            _sharedSchemaRecordRepository.Delete(id);
        }
        public void DeleteByUserIdReceive(string id, string userId)
        {
            var sharedSchemaRecordFilter = Builders<SharedSchemaRecord>.Filter.Eq(u => u.Id, id);
            var userIdReceiveFilter = Builders<SharedSchemaRecord>.Filter.Eq(u => u.UserIdReceive, userId);
            var schemaRecordUpdate = Builders<SharedSchemaRecord>.Update.Set(u => u.IsDeleted, true);

            Collection.UpdateOne(sharedSchemaRecordFilter & userIdReceiveFilter, schemaRecordUpdate);
        }


        public async Task<object> GetAll(int? page, int? limit, string sortField = null, bool sortDesc = false)
        {
            return _sharedSchemaRecordRepository.GetAll(page, limit, sortField, sortDesc, false);
        }

        public SharedSchemaRecord GetById(string id)
        {
            var filterId = Builders<SharedSchemaRecord>.Filter.Eq(u => u.Id, id);
            var filterIsDeleted = Builders<SharedSchemaRecord>.Filter.Eq(u => u.IsDeleted, false);
            return Collection.Find(filterId & filterIsDeleted).FirstOrDefault();
        }

        public SharedSchemaRecord GetByUserIdSender(string userIdSender)
        {
            var filterId = Builders<SharedSchemaRecord>.Filter.Eq(u => u.UserIdSender, userIdSender);
            var filterIsDeleted = Builders<SharedSchemaRecord>.Filter.Eq(u => u.IsDeleted, false);
            return Collection.Find(filterId & filterIsDeleted).FirstOrDefault();
        }

        public List<SharedSchemaRecord> GetBySchemaNameAndUserIdReceive(string schemaName, string userId)
        {
            var filterId = Builders<SharedSchemaRecord>.Filter.Eq(u => u.UserIdReceive, userId);
            var filterSchemaName = Builders<SharedSchemaRecord>.Filter.Eq(u => u.SchemaName, schemaName);
            var filterIsDeleted = Builders<SharedSchemaRecord>.Filter.Eq(u => u.IsDeleted, false);
            return Collection.Find(filterId & filterSchemaName & filterIsDeleted).ToList();
        }

        public async Task<PaginationData<SharedSchemaRecord>> GetByUserIdReceive(int? page, int? limit, string sortField = null, bool sortDesc = false, string userId = null)
        {
            SortDefinition<SharedSchemaRecord> sort = null;
            if (page == null) page = 1;
            if (limit == null) limit = 10;
            var filterIsDeleted = Builders<SharedSchemaRecord>.Filter.Eq(u => u.IsDeleted, false);

            var filterId = Builders<SharedSchemaRecord>.Filter.Eq(u => u.UserIdReceive, userId);
            if (sortField != null) sort = sortDesc ? Builders<SharedSchemaRecord>.Sort.Descending(sortField) : Builders<SharedSchemaRecord>.Sort.Ascending(sortField);
            var filters = filterIsDeleted & filterId;
            var listBusca = !string.IsNullOrEmpty(sortField) ? Collection.Find(filters).Sort(sort).ToList() : Collection.Find(filters).ToList();
            var res = listBusca.Skip((int)((page - 1) * limit)).Take((int)limit);


            long count = Convert.ToInt64(listBusca.Count());
            return new PaginationData<SharedSchemaRecord>(res.ToList(), page, limit, count);


        }

        public async Task<PaginationData<SharedSchemaRecord>> GetBySchemaRecordIdAndUserIdSender(int? page, int? limit, string sortField = null, bool sortDesc = false, string schemaRecordId = null, string userId = null)
        {
            SortDefinition<SharedSchemaRecord> sort = null;
            if (page == null) page = 1;
            if (limit == null) limit = 10;
            var filterIsDeleted = Builders<SharedSchemaRecord>.Filter.Eq(u => u.IsDeleted, false);
            var filterSchemaRecordId = Builders<SharedSchemaRecord>.Filter.Eq(u => u.SchemaRecordId, schemaRecordId);
            var filterId = Builders<SharedSchemaRecord>.Filter.Eq(u => u.UserIdSender, userId);
            if (sortField != null) sort = sortDesc ? Builders<SharedSchemaRecord>.Sort.Descending(sortField) : Builders<SharedSchemaRecord>.Sort.Ascending(sortField);
            var filters = filterIsDeleted & filterId & filterSchemaRecordId;
            var listBusca = !string.IsNullOrEmpty(sortField) ? Collection.Find(filters).Sort(sort).ToList() : Collection.Find(filters).ToList();
            var res = listBusca.Skip((int)((page - 1) * limit)).Take((int)limit);


            long count = Convert.ToInt64(listBusca.Count());
            return new PaginationData<SharedSchemaRecord>(res.ToList(), page, limit, count);
        }

        public SharedSchemaRecord RegisterAsync(SharedSchemaRecordCommand command)
        {
            var newSharedSchemaRecord = new SharedSchemaRecord(command.SchemaRecordId, command.SchemaName, command.UserIdSender, command.UserIdReceive, command.UserNameSender, command.UserNameReceive, command.Permissions);


            var sharedSchemaRecordRepository = _sharedSchemaRecordRepository.Insert(newSharedSchemaRecord);

            var newNotification = new Notification(command.SchemaRecordId, command.SchemaName, null, null, null, $"{command.UserNameSender} compartilhou item do {command.SchemaName}", "Compartilhamento", command.UserIdReceive, false, 0, null, null);
            newNotification.CreateBy = command.UserNameSender;
            _notificationService.RegisterAsync(newNotification);

            return sharedSchemaRecordRepository;
        }

        public SharedSchemaRecord Update(SharedSchemaRecordCommand command, string id)
        {
            var segmentationIdFilter = Builders<SharedSchemaRecord>.Filter.Eq(u => u.Id, id);
            var segmentationUpdate = Builders<SharedSchemaRecord>.Update.Set(u => u.SchemaRecordId, command.SchemaRecordId)
                                                                        .Set(u => u.SchemaName, command.SchemaName)
                                                                        .Set(u => u.UserIdSender, command.UserIdSender)
                                                                        .Set(u => u.UserIdReceive, command.UserIdReceive)
                                                                        .Set(u => u.UserNameSender, command.UserNameSender)
                                                                        .Set(u => u.UserNameReceive, command.UserNameReceive)
                                                                        .Set(u => u.Permissions, command.Permissions);
            Collection.UpdateOne(segmentationIdFilter, segmentationUpdate);

            var newNotification = new Notification(command.SchemaRecordId, command.SchemaName, null, null, null, $"{command.UserNameSender} compartilhou item do {command.SchemaName}", "Compartilhamento", command.UserIdReceive, false, 0, null, null);
            newNotification.CreateBy = command.UserNameSender;
            _notificationService.RegisterAsync(newNotification);

            return GetById(id);
        }


    }
}
