using MongoDB.Bson;
using MongoDB.Driver;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Domain.Util;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Test.Services.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace Simjob.Framework.Infra.Identity.Repositories
{
    public class TwoFactorAuthRepository : ITwoFactorAuthRepository
    {
        private const string UserCollectionName = "usertwofactorauth";
        protected readonly TwoFactorAuthContext _context;
        protected readonly IMongoCollection<UserTwoFactorAuth> _collection;
        protected bool UserOnly;
        protected FilterDefinition<UserTwoFactorAuth> UserOnlyFilter = Builders<UserTwoFactorAuth>.Filter.Empty;
        [ExcludeFromCodeCoverage]
        public TwoFactorAuthRepository(TwoFactorAuthContext context)
        {
            _context = context;
            _collection = _context.GetDatabase().GetCollection<UserTwoFactorAuth>(UserCollectionName);
        }

        public TwoFactorAuthRepository(TDDContext context, string test)
        {
            _collection = context.GetDatabase().GetCollection<UserTwoFactorAuth>(UserCollectionName);
        }
        [ExcludeFromCodeCoverage]
        public virtual void Delete(string id)
        {
            throw new NotImplementedException();

        }
        public virtual void Insert(UserTwoFactorAuth obj)
        {
            if (string.IsNullOrEmpty(obj.Id)) obj.Id = Guid.NewGuid().ToString();
            obj.CreateAt = DateTime.Now;
            _collection.InsertOne(obj);
        }
        [ExcludeFromCodeCoverage]
        public virtual void Update(string id, User obj)
        {
            throw new NotImplementedException();
        }
        [ExcludeFromCodeCoverage]
        public User GetByField(string field, string value)
        {
            throw new NotImplementedException();
        }
        public bool Exists(Expression<Func<UserTwoFactorAuth, bool>> predicate)
        {
            var filterPredicate = Builders<UserTwoFactorAuth>.Filter.Where(predicate);
            return _collection.Find(filterPredicate & UserOnlyFilter).CountDocuments() > 0;
        }
    }
}

