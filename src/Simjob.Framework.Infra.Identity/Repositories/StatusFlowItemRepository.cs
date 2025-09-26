using MongoDB.Driver;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Simjob.Framework.Infra.Identity.Repositories
{
    public class StatusFlowItemRepository : IStatusFlowItemRepository
    {
        protected readonly StatusFlowItemContext _context;
        protected readonly IMongoCollection<StatusFlowItem> _collection;
        protected readonly IUserHelper _userHelper;
        protected bool UserOnly;
        protected FilterDefinition<StatusFlowItem> UserOnlyFilter = Builders<StatusFlowItem>.Filter.Empty;


        public StatusFlowItemRepository(StatusFlowItemContext context, IUserHelper userHelper, string collectionName = null)
        {
            _context = context;
            _collection = _context.GetDatabase().GetCollection<StatusFlowItem>(collectionName ?? typeof(StatusFlowItem).Name.ToLower());
            _userHelper = userHelper;
            var claims = _userHelper.GetClaims();
            if (claims != null && claims.Any()) UserOnly = claims.Exists(c => c.Value == "useronly" && c.Type == typeof(StatusFlowItem).Name);
            if (UserOnly)
                UserOnlyFilter = Builders<StatusFlowItem>.Filter.Eq("CreateBy", _userHelper.GetUserName());

        }


        public bool Exists(Expression<Func<StatusFlowItem, bool>> predicate)
        {
            var filterPredicate = Builders<StatusFlowItem>.Filter.Where(predicate);
            var filterIsDeleted = Builders<StatusFlowItem>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterPredicate & filterIsDeleted & UserOnlyFilter).CountDocuments() > 0;
        }

        public StatusFlowItem GetById(string id)
        {
            var filterId = Builders<StatusFlowItem>.Filter.Eq(e => e.Id, id);
            var filterIsDeleted = Builders<StatusFlowItem>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterId & filterIsDeleted & UserOnlyFilter).FirstOrDefault();
        }

        public StatusFlowItem GetBySchemaRecordId(string schemaRecordId)
        {
            var filterSchemaRecordId = Builders<StatusFlowItem>.Filter.Eq(e => e.SchemaRecordId, schemaRecordId);
            var filterIsDeleted = Builders<StatusFlowItem>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterSchemaRecordId & filterIsDeleted & UserOnlyFilter).FirstOrDefault();
        }

        public void Update(string id, StatusFlowItem obj)
        {
            var statusflowItemIdFilter = Builders<StatusFlowItem>.Filter.Eq(u => u.Id, id);
            var statusflowItemUpdate = Builders<StatusFlowItem>.Update.Set(u => u.StatusFlowId, obj.StatusFlowId)
                                                          .Set(u => u.SchemaRecordId, obj.SchemaRecordId)
                                                          .Set(u => u.Status, obj.Status)
                                                          .Set(u => u.AprovEmail, obj.AprovEmail)
                                                          .Set(u => u.UpdateAt, DateTime.Now);

            _collection.UpdateOne(statusflowItemIdFilter, statusflowItemUpdate);
        }
    }
}
