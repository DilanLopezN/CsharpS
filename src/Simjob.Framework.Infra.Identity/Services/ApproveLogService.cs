using MongoDB.Driver;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Services
{
    public class ApproveLogService : IApproveLogService
    {
        protected readonly ApproveLogContext Context;
        protected readonly IMongoCollection<ApproveLog> Collection;
        private readonly IRepository<ApproveLogContext, ApproveLog> _approveLogRepository;

        public ApproveLogService(ApproveLogContext context, IRepository<ApproveLogContext, ApproveLog> approveLogRepository)
        {
            Context = context;
            Collection = context.GetUserCollection();
            _approveLogRepository = approveLogRepository;
        }

        public void DeleteApproveLog(string id)
        {
            _approveLogRepository.Delete(id);
        }

        public async Task<object> GetAll(int? page, int? limit, string sortField = null, bool sortDesc = false)
        {
            return _approveLogRepository.GetAll(page, limit, sortField, sortDesc, false);
        }

        public ApproveLog GetById(string id)
        {
            var filterId = Builders<ApproveLog>.Filter.Eq(u => u.Id, id);
            var filterIsDeleted = Builders<ApproveLog>.Filter.Eq(u => u.IsDeleted, false);
            return Collection.Find(filterId & filterIsDeleted).FirstOrDefault();
        }

        public ApproveLog GetBySchemaRecordId(string schemaRecordId)
        {
            var filterId = Builders<ApproveLog>.Filter.Eq(u => u.SchemaRecordId, schemaRecordId);
            var filterIsDeleted = Builders<ApproveLog>.Filter.Eq(u => u.IsDeleted, false);
            return Collection.Find(filterId & filterIsDeleted).FirstOrDefault();
        }

        public void Register(ApproveLog approveLog)
        {
            _approveLogRepository.Insert(approveLog);
        }

        public void UpdateApproveLog(ApproveLog approveLog, string id)
        {
            var approveLogIdFilter = Builders<ApproveLog>.Filter.Eq(u => u.Id, id);
            var approveLogUpdate = Builders<ApproveLog>.Update.Set(u => u.SchemaRecordId, approveLog.SchemaRecordId)
                                                          .Set(u => u.SchemaName, approveLog.SchemaName)
                                                          .Set(u => u.Field, approveLog.Field)
                                                          .Set(u => u.Value, approveLog.Value)
                                                          .Set(u => u.Msg, approveLog.Msg);
                                                         

            Collection.UpdateOne(approveLogIdFilter, approveLogUpdate);
        }
    }
}
