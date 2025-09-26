using MongoDB.Driver;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Test.Services.Api;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Simjob.Framework.Infra.Identity.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private const string UserCollectionName = "profile";
        protected readonly ProfileContext _context;
        protected readonly IMongoCollection<Profile> _collection;
        protected readonly IUserHelper _userHelper;
        protected bool UserOnly;
        protected FilterDefinition<Profile> UserOnlyFilter = Builders<Profile>.Filter.Empty;

        public ProfileRepository(ProfileContext context, IUserHelper userHelper)
        {
            _context = context;
            _collection = _context.GetDatabase().GetCollection<Profile>(UserCollectionName);
            _userHelper = userHelper;
            var claims = _userHelper.GetClaims();
            if (claims.Any()) UserOnly = claims.Exists(c => c.Value == "useronly" && c.Type == typeof(User).Name);
            if (UserOnly)
                UserOnlyFilter = Builders<Profile>.Filter.Eq("CreateBy", _userHelper.GetUserName());
        }

        public ProfileRepository(TDDContext context, IUserHelper userHelper, string test)
        {
            _collection = context.GetDatabase().GetCollection<Profile>(UserCollectionName);
            _userHelper = userHelper;
            if (UserOnly)
                UserOnlyFilter = Builders<Profile>.Filter.Eq("CreateBy", _userHelper.GetUserName());
        }

        public TDDContext MockContext { get; }
        public IUserHelper Object { get; }
        public string V { get; }

        public void Delete(string id)
        {
            var username = _userHelper.GetUserName();

            var entity = this.GetById(id);
            if (entity == null) return;
            var filter = Builders<Profile>.Filter.Eq(e => e.Id, id);
            var updateIsDeleted = Builders<Profile>.Update.Set(o => o.IsDeleted, true)
                .Set(o => o.DeleteAt, DateTime.Now)
                .Set(o => o.DeleteBy, username);

            _collection.UpdateOne(filter, updateIsDeleted);
        }

        public bool Exists(Expression<Func<Profile, bool>> predicate)
        {
            var filterPredicate = Builders<Profile>.Filter.Where(predicate);
            var filterIsDeleted = Builders<Profile>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterPredicate & filterIsDeleted & UserOnlyFilter).CountDocuments() > 0;
        }

        public Profile GetById(string id)
        {
            var filterId = Builders<Profile>.Filter.Eq(e => e.Id, id);
            var filterIsDeleted = Builders<Profile>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterId & filterIsDeleted & UserOnlyFilter).FirstOrDefault();
        }

        public Profile GetByTenanty(string tenanty)
        {
            var filterTenanty = Builders<Profile>.Filter.Eq(e => e.Tenanty, tenanty);
            var filterIsDeleted = Builders<Profile>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterTenanty & filterIsDeleted & UserOnlyFilter).FirstOrDefault();
        }

        public void Insert(Profile obj)
        {
            if (string.IsNullOrEmpty(obj.Id)) obj.Id = Guid.NewGuid().ToString();
            obj.CreateBy = _userHelper.GetUserName();
            obj.CreateAt = DateTime.Now;
            _collection.InsertOne(obj);
        }

        public void Update(string id, Profile obj)
        {
            var entity = GetById(id);
            if (entity == null) return;

            obj.CreateAt = entity.CreateAt;
            obj.CreateBy = entity.CreateBy;
            obj.DeleteAt = entity.DeleteAt;
            obj.DeleteBy = entity.DeleteBy;
            obj.UpdateBy = _userHelper.GetUserName();
            obj.UpdateAt = DateTime.Now;

            var filter = Builders<Profile>.Filter.Eq(e => e.Id, id);
            _collection.ReplaceOne<Profile>(o => o.Id == id, obj);
        }
    }
}