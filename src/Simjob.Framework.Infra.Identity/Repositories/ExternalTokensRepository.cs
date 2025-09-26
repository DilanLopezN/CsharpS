using MongoDB.Driver;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Test.Services.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Repositories
{
    public class ExternalTokensRepository : IExternalTokensRepository
    {

        private const string UserCollectionName = "tokens";
        protected readonly ExternalTokensContext _context;
        protected readonly IMongoCollection<Tokens> _collection;
        protected readonly IUserHelper _userHelper;
        protected bool UserOnly;
        protected FilterDefinition<Tokens> UserOnlyFilter = Builders<Tokens>.Filter.Empty;

        [ExcludeFromCodeCoverage]


        public ExternalTokensRepository(ExternalTokensContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
            var claims = _userHelper.GetClaims();
            if (claims.Any()) UserOnly = claims.Exists(c => c.Value == "useronly" && c.Type == typeof(User).Name);
            if (UserOnly)
                UserOnlyFilter = Builders<Tokens>.Filter.Eq("CreateBy", _userHelper.GetUserName());
        }

        public ExternalTokensRepository(TDDContext context, IUserHelper userHelper, string test) // tdd
        {
            _collection = context.GetDatabase().GetCollection<Tokens>(UserCollectionName);
            _userHelper = userHelper;
            if (UserOnly)
                UserOnlyFilter = Builders<Tokens>.Filter.Eq("CreateBy", _userHelper.GetUserName());
        }
        [ExcludeFromCodeCoverage]
        protected FilterDefinition<J> GetUsertOnlyFilter<J>(string property = null)
        {
            return UserOnly ? Builders<J>.Filter.Eq(property ?? "CreateBy", _userHelper.GetUserName()) : Builders<J>.Filter.Empty;
        }

        // ok
        public bool Exists(Expression<Func<Tokens, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public virtual void Delete(string id)
        {
            var username = _userHelper.GetUserName();

            var entity = this.GetById(id);
            if (entity == null) return;
            var filter = Builders<Tokens>.Filter.Eq(e => e.Id, id);
            var updateIsDeleted = Builders<Tokens>.Update.Set(o => o.IsDeleted, true)
                .Set(o => o.DeleteAt, DateTime.Now)
                .Set(o => o.DeleteBy, username);

            _collection.UpdateOne(filter, updateIsDeleted);
        }

        public Tokens GetById(string id)
        {
            var filterId = Builders<Tokens>.Filter.Eq(e => e.Id, id);
            var filterIsDeleted = Builders<Tokens>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterId & filterIsDeleted & UserOnlyFilter).FirstOrDefault();
        }

        public Tokens GetByUserToken(string userToken)
        {
            var filterId = Builders<Tokens>.Filter.Eq(e => e.UserToken, userToken);
            var filterIsDeleted = Builders<Tokens>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterId & filterIsDeleted & UserOnlyFilter).FirstOrDefault();
        }

        public Tokens Insert(Tokens obj)
        {
            if (string.IsNullOrEmpty(obj.Id)) obj.Id = Guid.NewGuid().ToString();
            obj.CreateBy = _userHelper.GetUserName();
            obj.CreateAt = DateTime.Now;
            _collection.InsertOne(obj);
            return obj;
        }

        public Tokens Update(string id, Tokens obj)
        {
            var entity = GetById(id);
            if (entity == null) return null;

            obj.CreateAt = entity.CreateAt;
            obj.CreateBy = entity.CreateBy;
            obj.DeleteAt = entity.DeleteAt;
            obj.DeleteBy = entity.DeleteBy;
            obj.UpdateBy = _userHelper.GetUserName();
            obj.UpdateAt = DateTime.Now;

            var filter = Builders<Tokens>.Filter.Eq(e => e.Id, id);
            _collection.ReplaceOne<Tokens>(o => o.Id == id, obj);

            return obj;
        }
    }
}
