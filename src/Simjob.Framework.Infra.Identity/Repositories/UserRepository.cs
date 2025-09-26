using MongoDB.Bson;
using MongoDB.Driver;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Domain.Util;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Simjob.Framework.Test;
using Simjob.Framework.Test.Services.Api;
using System.Diagnostics.CodeAnalysis;

namespace Simjob.Framework.Infra.Identity.Repositories
{
    public class UserRepository : IUserRepository
    {
        public const string UserCollectionName = "user";
        protected readonly IdentityContext _context;
        protected readonly IMongoCollection<User> _collection;
        protected readonly IUserHelper _userHelper;
        protected bool UserOnly;
        protected FilterDefinition<User> UserOnlyFilter = Builders<User>.Filter.Empty;
        [ExcludeFromCodeCoverage]
        public UserRepository(IdentityContext context, IUserHelper userHelper)
        {
            _context = context;
            _collection = _context.GetDatabase().GetCollection<User>(UserCollectionName);
            _userHelper = userHelper;
            var claims = _userHelper.GetClaims();
            if (claims.Any()) UserOnly = claims.Exists(c => c.Value == "useronly" && c.Type == typeof(User).Name);
            if (UserOnly)
                UserOnlyFilter = Builders<User>.Filter.Eq("CreateBy", _userHelper.GetUserName());
        }

        public UserRepository(TDDContext context, IUserHelper userHelper, string test) // tdd
        {
            _collection = context.GetDatabase().GetCollection<User>(UserCollectionName);
            _userHelper = userHelper;
            if (UserOnly)
                UserOnlyFilter = Builders<User>.Filter.Eq("CreateBy", _userHelper.GetUserName());
        }

        [ExcludeFromCodeCoverage]
        protected FilterDefinition<J> GetUsertOnlyFilter<J>(string property = null)
        {
            return UserOnly ? Builders<J>.Filter.Eq(property ?? "CreateBy", _userHelper.GetUserName()) : Builders<J>.Filter.Empty;
        }
        public virtual void Delete(string id)
        {
            var username = _userHelper.GetUserName();

            var entity = this.GetById(id);
            if (entity == null) return;
            var filter = Builders<User>.Filter.Eq(e => e.Id, id);
            var updateIsDeleted = Builders<User>.Update.Set(o => o.IsDeleted, true)
                .Set(o => o.DeleteAt, DateTime.Now)
                .Set(o => o.DeleteBy, username);

            _collection.UpdateOne(filter, updateIsDeleted);

        }
        public virtual PaginationData<User> SearchRegexByFields(string fields, string search, int? page = null, int? limit = null, string sortField = null, bool sortDesc = false)
        {
            var fieldsSplit = fields.Split(',');
            var regexFilters = fieldsSplit.Select(field => Builders<User>.Filter.Regex(field, new BsonRegularExpression($"/^.*{search}.*$/i")));
            var orFilters = Builders<User>.Filter.Or(regexFilters);

            SortDefinition<User> sort = null;
            var filterIsDeleted = Builders<User>.Filter.Eq(e => e.IsDeleted, false);

            FilterDefinition<User> filters = filterIsDeleted & orFilters & UserOnlyFilter;

            if (!string.IsNullOrEmpty(sortField))
                sort = sortDesc ? Builders<User>.Sort.Descending(sortField) : Builders<User>.Sort.Ascending(sortField);
            var count = _collection.CountDocuments(filters);

            if (!page.HasValue || !limit.HasValue)
            {
                var resNoLimit = string.IsNullOrEmpty(sortField) ? _collection.Find(filters) : _collection.Find(filters).Sort(sort);
                return new PaginationData<User>(resNoLimit.ToList(), page, limit, count);
            }

            var res = string.IsNullOrEmpty(sortField) ? _collection.Find(filters).Skip((page - 1) * limit).Limit(limit) : _collection.Find(filters).Sort(sort).Skip((page - 1) * limit).Limit(limit);
            return new PaginationData<User>(res.ToList(), page, limit, count);
        }
        public virtual PaginationData<User> GetAll(int? page = null, int? limit = null, string sortField = null, bool sortDesc = false, bool addIsDeleted = false)
        {

            SortDefinition<User> sort = null;
            var filterIsDeleted = addIsDeleted ? Builders<User>.Filter.Empty : Builders<User>.Filter.Where(e => e.IsDeleted == false);
            if (!string.IsNullOrEmpty(sortField))
                sort = sortDesc ? Builders<User>.Sort.Descending(sortField) : Builders<User>.Sort.Ascending(sortField);
            var count = _collection.CountDocuments(filterIsDeleted & UserOnlyFilter);

            if (!page.HasValue || !limit.HasValue)
            {
                var resNoLimit = string.IsNullOrEmpty(sortField) ? _collection.Find(filterIsDeleted & UserOnlyFilter) : _collection.Find(filterIsDeleted & UserOnlyFilter).Sort(sort);
                return new PaginationData<User>(resNoLimit.ToList(), page, limit, count);
            }

            var res = string.IsNullOrEmpty(sortField) ? _collection.Find(filterIsDeleted & UserOnlyFilter).Skip((page - 1) * limit).Limit(limit) : _collection.Find(filterIsDeleted & UserOnlyFilter).Sort(sort).Skip((page - 1) * limit).Limit(limit);
            return new PaginationData<User>(res.ToList(), page, limit, count);
        }
        public virtual User GetById(string id)
        {
            var filterId = Builders<User>.Filter.Eq(e => e.Id, id);
            var filterIsDeleted = Builders<User>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterId & filterIsDeleted & UserOnlyFilter).FirstOrDefault();
        }
        public virtual List<User> GetById(string[] ids)
        {
            var filter = Builders<User>.Filter.In("Id", ids);
            var filterIsDeleted = Builders<User>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filter & filterIsDeleted & UserOnlyFilter).ToList();
        }
        public virtual void Insert(User obj)
        {
            if (string.IsNullOrEmpty(obj.Id)) obj.Id = Guid.NewGuid().ToString();
            obj.CreateBy = _userHelper.GetUserName();
            obj.CreateAt = DateTime.Now;
            _collection.InsertOne(obj);
        }
        public virtual void InsertMany(List<User> objs)
        {
            var now = DateTime.Now;
            string username = _userHelper.GetUserName();

            foreach (var e in objs)
            {
                if (string.IsNullOrEmpty(e.Id))
                    e.Id = Guid.NewGuid().ToString();

                e.CreateAt = now;
                e.CreateBy = username;
            }

            _collection.InsertMany(objs);
        }
        public virtual PaginationData<User> Search(Expression<Func<User, bool>> predicate, int? page = null, int? limit = null, string sortField = null, bool sortDesc = false)
        {
            SortDefinition<User> sort = null;
            var filter = Builders<User>.Filter.Where(predicate);
            var filterIsDeleted = Builders<User>.Filter.Eq(e => e.IsDeleted, false);
            var count = _collection.CountDocuments(filter & filterIsDeleted & UserOnlyFilter);

            if (!page.HasValue || !limit.HasValue)
            {
                var resNoLimit = string.IsNullOrEmpty(sortField) ? _collection.Find(filter & filterIsDeleted & UserOnlyFilter) : _collection.Find(filter & filterIsDeleted & UserOnlyFilter).Sort(sort);
                return new PaginationData<User>(resNoLimit.ToList(), page, limit, count);
            }

            if (!string.IsNullOrEmpty(sortField))
                sort = sortDesc ? Builders<User>.Sort.Descending(sortField) : Builders<User>.Sort.Ascending(sortField);
            var res = string.IsNullOrEmpty(sortField) ? _collection.Find(predicate & filterIsDeleted & UserOnlyFilter).Skip((page - 1) * limit).Limit(limit) : _collection.Find(predicate & filterIsDeleted & UserOnlyFilter).Sort(sort).Skip((page - 1) * limit).Limit(limit);
            return new PaginationData<User>(res.ToList(), page, limit, count);
        }
        public List<User> SearchLikeInFieldAutoComplete(string field, string value, int limit = 10, Expression<Func<User, bool>> predicate = null)
        {
            var fieldsSplit = field.Split(',');
            var filters = fieldsSplit.Select(f => Builders<User>.Filter.Regex(f, new BsonRegularExpression($"/^.*{value}.*$/i"))).ToList();
            var filterIsDeleted = Builders<User>.Filter.Eq(u => u.IsDeleted, false);
            var filterAnyEq = filters.Count > 1 ? Builders<User>.Filter.Or(filters) : Builders<User>.Filter.Regex(field, new BsonRegularExpression($"/^.*{value}.*$/i"));
            var filterWhere = predicate == null ? Builders<User>.Filter.Empty : Builders<User>.Filter.Where(predicate);

            return _collection.Find(filterAnyEq & filterIsDeleted & UserOnlyFilter & filterWhere).Limit(limit).ToList();
        }
        public virtual void Update(string id, User obj)
        {
            var entity = GetById(id);
            if (entity == null) return;

            obj.CreateAt = entity.CreateAt;
            obj.CreateBy = entity.CreateBy;
            obj.DeleteAt = entity.DeleteAt;
            obj.DeleteBy = entity.DeleteBy;
            obj.UpdateBy = _userHelper.GetUserName();
            obj.UpdateAt = DateTime.Now;

            var filter = Builders<User>.Filter.Eq(e => e.Id, id);
            _collection.ReplaceOne<User>(o => o.Id == id, obj);
        }
        public User SearchFirstByField(string field, string value, bool isDeleted = false)
        {
            var eqIsDeleted = Builders<User>.Filter.Eq(e => e.IsDeleted, isDeleted);
            var eqFilter = Builders<User>.Filter.Eq(field, value);
            return _collection.Find(eqFilter & eqIsDeleted & UserOnlyFilter).FirstOrDefault();
        }
        public User GetByField(string field, string value)
        {
            var filterField = Builders<User>.Filter.Eq(field, value);
            var filterIsDeleted = Builders<User>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterField & filterIsDeleted & UserOnlyFilter).FirstOrDefault();
        }
        public bool Exists(Expression<Func<User, bool>> predicate)
        {
            var filterPredicate = Builders<User>.Filter.Where(predicate);
            var filterIsDeleted = Builders<User>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterPredicate & filterIsDeleted & UserOnlyFilter).CountDocuments() > 0;
        }
    }
}

