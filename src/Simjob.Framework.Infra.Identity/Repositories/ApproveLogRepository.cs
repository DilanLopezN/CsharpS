using MongoDB.Driver;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Repositories
{
    public class ApproveLogRepository : IApproveLogRepository
    {

        private const string UserCollectionName = "approvelog";
        protected readonly ApproveLogContext _context;
        protected readonly IMongoCollection<ApproveLog> _collection;
        protected readonly IUserHelper _userHelper;
        protected bool UserOnly;
        protected FilterDefinition<ApproveLog> UserOnlyFilter = Builders<ApproveLog>.Filter.Empty;
        public virtual void Delete(string id)
        {
            var username = _userHelper.GetUserName();

            var entity = this.GetById(id);
            if (entity == null) return;
            var filter = Builders<ApproveLog>.Filter.Eq(e => e.Id, id);
            var updateIsDeleted = Builders<ApproveLog>.Update.Set(o => o.IsDeleted, true)
                .Set(o => o.DeleteAt, DateTime.Now)
                .Set(o => o.DeleteBy, username);

            _collection.UpdateOne(filter, updateIsDeleted);
        }

        public bool Exists(Expression<Func<ApproveLog, bool>> predicate)
        {
            var filterPredicate = Builders<ApproveLog>.Filter.Where(predicate);
            var filterIsDeleted = Builders<ApproveLog>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterPredicate & filterIsDeleted & UserOnlyFilter).CountDocuments() > 0;
        }

        public virtual ApproveLog GetById(string id)
        {
            var filterId = Builders<ApproveLog>.Filter.Eq(e => e.Id, id);
            var filterIsDeleted = Builders<ApproveLog>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterId & filterIsDeleted & UserOnlyFilter).FirstOrDefault();
        }

        public virtual void Insert(ApproveLog obj)
        {
            if (string.IsNullOrEmpty(obj.Id)) obj.Id = Guid.NewGuid().ToString();
            obj.CreateBy = _userHelper.GetUserName();
            obj.CreateAt = DateTime.Now;
            _collection.InsertOne(obj);
        }

        public virtual void Update(string id, ApproveLog obj)
        {
            var entity = GetById(id);
            if (entity == null) return;

            obj.CreateAt = entity.CreateAt;
            obj.CreateBy = entity.CreateBy;
            obj.DeleteAt = entity.DeleteAt;
            obj.DeleteBy = entity.DeleteBy;
            obj.UpdateBy = _userHelper.GetUserName();
            obj.UpdateAt = DateTime.Now;

            var filter = Builders<ApproveLog>.Filter.Eq(e => e.Id, id);
            _collection.ReplaceOne<ApproveLog>(o => o.Id == id, obj);
        }
    }
}
