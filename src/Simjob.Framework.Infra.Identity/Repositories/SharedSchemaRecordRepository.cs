using MongoDB.Driver;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace Simjob.Framework.Infra.Identity.Repositories
{
    public class SharedSchemaRecordRepository : ISharedSchemaRecordRepository
    {
        private const string UserCollectionName = "sharedschemarecord";
        protected readonly SharedSchemaRecordContext _context;
        protected readonly IMongoCollection<SharedSchemaRecord> _collection;
        protected readonly IUserHelper _userHelper;
        protected bool UserOnly;
        protected FilterDefinition<SharedSchemaRecord> UserOnlyFilter = Builders<SharedSchemaRecord>.Filter.Empty;

        public SharedSchemaRecordRepository(SharedSchemaRecordContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
            var claims = _userHelper.GetClaims();
            if (claims.Any()) UserOnly = claims.Exists(c => c.Value == "useronly" && c.Type == typeof(User).Name);
            if (UserOnly)
                UserOnlyFilter = Builders<SharedSchemaRecord>.Filter.Eq("CreateBy", _userHelper.GetUserName());
        }

        [ExcludeFromCodeCoverage]


        public void Delete(string id)
        {
            var username = _userHelper.GetUserName();

            var entity = this.GetById(id);
            if (entity == null) return;
            var filter = Builders<SharedSchemaRecord>.Filter.Eq(e => e.Id, id);
            var updateIsDeleted = Builders<SharedSchemaRecord>.Update.Set(o => o.IsDeleted, true)
                .Set(o => o.DeleteAt, DateTime.Now)
                .Set(o => o.DeleteBy, username);

            _collection.UpdateOne(filter, updateIsDeleted);
        }

        public bool Exists(Expression<Func<SharedSchemaRecord, bool>> predicate)
        {
            var filterPredicate = Builders<SharedSchemaRecord>.Filter.Where(predicate);
            var filterIsDeleted = Builders<SharedSchemaRecord>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterPredicate & filterIsDeleted & UserOnlyFilter).CountDocuments() > 0;
        }



        public SharedSchemaRecord GetById(string id)
        {
            var filterId = Builders<SharedSchemaRecord>.Filter.Eq(e => e.Id, id);
            var filterIsDeleted = Builders<SharedSchemaRecord>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterId & filterIsDeleted & UserOnlyFilter).FirstOrDefault();
        }

        public virtual void Insert(SharedSchemaRecord obj)
        {
            if (string.IsNullOrEmpty(obj.Id)) obj.Id = Guid.NewGuid().ToString();
            obj.CreateBy = _userHelper.GetUserName();
            obj.CreateAt = DateTime.Now;
            _collection.InsertOne(obj);
        }

        public virtual void Update(string id, SharedSchemaRecord obj)
        {
            var entity = GetById(id);
            if (entity == null) return;

            obj.CreateAt = entity.CreateAt;
            obj.CreateBy = entity.CreateBy;
            obj.DeleteAt = entity.DeleteAt;
            obj.DeleteBy = entity.DeleteBy;
            obj.UpdateBy = _userHelper.GetUserName();
            obj.UpdateAt = DateTime.Now;

            var filter = Builders<SharedSchemaRecord>.Filter.Eq(e => e.Id, id);
            _collection.ReplaceOne<SharedSchemaRecord>(o => o.Id == id, obj);
        }
    }
}
