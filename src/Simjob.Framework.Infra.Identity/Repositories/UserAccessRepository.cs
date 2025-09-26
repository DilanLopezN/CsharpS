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
    public class UserAccessRepository : IUserAccessRepository
    {
        private const string UserCollectionName = "useraccess";
        protected readonly UserAccessContext _context;
        protected readonly IMongoCollection<UserAccess> _collection;
        protected readonly IUserHelper _userHelper;
        protected bool UserOnly;
        protected FilterDefinition<UserAccess> UserOnlyFilter = Builders<UserAccess>.Filter.Empty;

        [ExcludeFromCodeCoverage]

        public UserAccessRepository(UserAccessContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
            var claims = _userHelper.GetClaims();
            if (claims.Any()) UserOnly = claims.Exists(c => c.Value == "useronly" && c.Type == typeof(User).Name);
            if (UserOnly)
                UserOnlyFilter = Builders<UserAccess>.Filter.Eq("CreateBy", _userHelper.GetUserName());
        }


        [ExcludeFromCodeCoverage]
        protected FilterDefinition<J> GetUsertOnlyFilter<J>(string property = null)
        {
            return UserOnly ? Builders<J>.Filter.Eq(property ?? "CreateBy", _userHelper.GetUserName()) : Builders<J>.Filter.Empty;
        }

        // ok
        public virtual void Delete(string id)
        {
            var username = _userHelper.GetUserName();

            var entity = this.GetById(id);
            if (entity == null) return;
            var filter = Builders<UserAccess>.Filter.Eq(e => e.Id, id);
            var updateIsDeleted = Builders<UserAccess>.Update.Set(o => o.IsDeleted, true)
                .Set(o => o.DeleteAt, DateTime.Now)
                .Set(o => o.DeleteBy, username);

            _collection.UpdateOne(filter, updateIsDeleted);
        }
        public virtual UserAccess GetById(string id)
        {
            var filterId = Builders<UserAccess>.Filter.Eq(e => e.Id, id);
            var filterIsDeleted = Builders<UserAccess>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterId & filterIsDeleted & UserOnlyFilter).FirstOrDefault();
        }
        public virtual void Insert(UserAccess obj)
        {
            if (string.IsNullOrEmpty(obj.Id)) obj.Id = Guid.NewGuid().ToString();
            obj.CreateBy = _userHelper.GetUserName();
            obj.CreateAt = DateTime.Now;
            _collection.InsertOne(obj);
        }
        public virtual void Update(string id, UserAccess obj)
        {
            var entity = GetById(id);
            if (entity == null) return;

            obj.CreateAt = entity.CreateAt;
            obj.CreateBy = entity.CreateBy;
            obj.DeleteAt = entity.DeleteAt;
            obj.DeleteBy = entity.DeleteBy;
            obj.UpdateBy = _userHelper.GetUserName();
            obj.UpdateAt = DateTime.Now;

            var filter = Builders<UserAccess>.Filter.Eq(e => e.Id, id);
            _collection.ReplaceOne<UserAccess>(o => o.Id == id, obj);
        }
        public bool Exists(Expression<Func<UserAccess, bool>> predicate)
        {
            var filterPredicate = Builders<UserAccess>.Filter.Where(predicate);
            var filterIsDeleted = Builders<UserAccess>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterPredicate & filterIsDeleted & UserOnlyFilter).CountDocuments() > 0;
        }
    }
}
