using MongoDB.Bson;
using MongoDB.Driver;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Domain.Models;
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
    public class SearchdefsRepository : ISearchdefsRepository
    {
        private const string UserCollectionName = "searchdefs";
        protected readonly SearchdefsContext _context;
        protected readonly IMongoCollection<Searchdefs> _collection;
        protected readonly IUserHelper _userHelper;
        protected bool UserOnly;
        protected FilterDefinition<Searchdefs> UserOnlyFilter = Builders<Searchdefs>.Filter.Empty;

        [ExcludeFromCodeCoverage]
        public SearchdefsRepository(SearchdefsContext context, IUserHelper userHelper)
        {
            _context = context;
            _collection = _context.GetDatabase().GetCollection<Searchdefs>(UserCollectionName);
            _userHelper = userHelper;
            var claims = _userHelper.GetClaims();
            if (claims.Any()) UserOnly = claims.Exists(c => c.Value == "useronly" && c.Type == typeof(User).Name);
            if (UserOnly)
                UserOnlyFilter = Builders<Searchdefs>.Filter.Eq("CreateBy", _userHelper.GetUserName());
        }

        public SearchdefsRepository(TDDContext context, IUserHelper userHelper, string test) // tdd
        {
            _collection = context.GetDatabase().GetCollection<Searchdefs>(UserCollectionName);
            _userHelper = userHelper;
            if (UserOnly)
                UserOnlyFilter = Builders<Searchdefs>.Filter.Eq("CreateBy", _userHelper.GetUserName());
        }

        public void Delete(string id)
        {
            var username = _userHelper.GetUserName();

            var entity = this.GetById(id);
            if (entity == null) return;
            var filter = Builders<Searchdefs>.Filter.Eq(e => e.Id, id);
            var updateIsDeleted = Builders<Searchdefs>.Update.Set(o => o.IsDeleted, true)
                .Set(o => o.DeleteAt, DateTime.Now)
                .Set(o => o.DeleteBy, username);

            _collection.UpdateOne(filter, updateIsDeleted);
        }

        public bool Exists(Expression<Func<Searchdefs, bool>> predicate)
        {
            var filterPredicate = Builders<Searchdefs>.Filter.Where(predicate);
            var filterIsDeleted = Builders<Searchdefs>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterPredicate & filterIsDeleted & UserOnlyFilter).CountDocuments() > 0;
        }

        public List<Searchdefs> GetAll()
        {
            var filterIsDeleted = Builders<Searchdefs>.Filter.Eq(u => u.IsDeleted, false);
            return _collection.Find(filterIsDeleted).ToList();
        }

        public Searchdefs GetById(string id)
        {
            var filterId = Builders<Searchdefs>.Filter.Eq(e => e.Id, id);
            var filterIsDeleted = Builders<Searchdefs>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterId & filterIsDeleted & UserOnlyFilter).FirstOrDefault();
        }

        public void Insert(Searchdefs obj)
        {
            if (string.IsNullOrEmpty(obj.Id)) obj.Id = Guid.NewGuid().ToString();
            obj.CreateBy = _userHelper.GetUserName();
            obj.CreateAt = DateTime.Now;
            _collection.InsertOne(obj);
        }

        public void Update(string id, Searchdefs obj)
        {
            var entity = GetById(id);
            if (entity == null) return;

            obj.CreateAt = entity.CreateAt;
            obj.CreateBy = entity.CreateBy;
            obj.DeleteAt = entity.DeleteAt;
            obj.DeleteBy = entity.DeleteBy;
            obj.UpdateBy = _userHelper.GetUserName();
            obj.UpdateAt = DateTime.Now;

            var filter = Builders<Searchdefs>.Filter.Eq(e => e.Id, id);
            _collection.ReplaceOne<Searchdefs>(o => o.Id == id, obj);
        }
    }
}
