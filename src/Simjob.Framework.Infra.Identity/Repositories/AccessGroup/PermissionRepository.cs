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
    public class PermissionRepository : IPermissionRepository
    {
        private const string UserCollectionName = "permission";
        protected readonly PermissionContext _context;
        protected readonly IMongoCollection<Permission> _collection;
        protected readonly IUserHelper _userHelper;
        protected bool UserOnly;
        protected FilterDefinition<Permission> UserOnlyFilter = Builders<Permission>.Filter.Empty;

        [ExcludeFromCodeCoverage]
        public PermissionRepository(PermissionContext context, IUserHelper userHelper)
        {
            _context = context;
            _collection = _context.GetDatabase().GetCollection<Permission>(UserCollectionName);
            _userHelper = userHelper;
            var claims = _userHelper.GetClaims();
            if (claims.Any()) UserOnly = claims.Exists(c => c.Value == "useronly" && c.Type == typeof(User).Name);
            if (UserOnly)
                UserOnlyFilter = Builders<Permission>.Filter.Eq("CreateBy", _userHelper.GetUserName());
        }

        public PermissionRepository(TDDContext context, IUserHelper userHelper, string test) // tdd
        {
            _collection = context.GetDatabase().GetCollection<Permission>(UserCollectionName);
            _userHelper = userHelper;
            if (UserOnly)
                UserOnlyFilter = Builders<Permission>.Filter.Eq("CreateBy", _userHelper.GetUserName());
        }


        public virtual void DeletePermanent(string id)
        {
            var user = this.GetById(id);
            if (user == null) return;
            var filter = Builders<Permission>.Filter.Eq(e => e.Id, id);
            _collection.DeleteOne(filter);
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
            var filter = Builders<Permission>.Filter.Eq(e => e.Id, id);
            var updateIsDeleted = Builders<Permission>.Update.Set(o => o.IsDeleted, true)
                .Set(o => o.DeleteAt, DateTime.Now)
                .Set(o => o.DeleteBy, username);

            _collection.UpdateOne(filter, updateIsDeleted);

        }
        public virtual PaginationData<Permission> GetAll(int? page = null, int? limit = null, string sortField = null, bool sortDesc = false, bool addIsDeleted = false)
        {
            SortDefinition<Permission> sort = null;
            var filterIsDeleted = addIsDeleted ? Builders<Permission>.Filter.Empty : Builders<Permission>.Filter.Where(e => e.IsDeleted == false);
            if (!string.IsNullOrEmpty(sortField))
                sort = sortDesc ? Builders<Permission>.Sort.Descending(sortField) : Builders<Permission>.Sort.Ascending(sortField);
            var count = _collection.CountDocuments(filterIsDeleted & UserOnlyFilter);

            if (!page.HasValue || !limit.HasValue)
            {
                var resNoLimit = string.IsNullOrEmpty(sortField) ? _collection.Find(filterIsDeleted & UserOnlyFilter) : _collection.Find(filterIsDeleted & UserOnlyFilter).Sort(sort);
                return new PaginationData<Permission>(resNoLimit.ToList(), page, limit, count);
            }

            var res = string.IsNullOrEmpty(sortField) ? _collection.Find(filterIsDeleted & UserOnlyFilter).Skip((page - 1) * limit).Limit(limit) : _collection.Find(filterIsDeleted & UserOnlyFilter).Sort(sort).Skip((page - 1) * limit).Limit(limit);
            return new PaginationData<Permission>(res.ToList(), page, limit, count);
        }
        public virtual Permission GetById(string id)
        {
            var filterId = Builders<Permission>.Filter.Eq(e => e.Id, id);
            var filterIsDeleted = Builders<Permission>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterId & filterIsDeleted & UserOnlyFilter).FirstOrDefault();
        }
        public virtual List<Permission> GetById(string[] ids)
        {
            var filter = Builders<Permission>.Filter.In("Id", ids);
            var filterIsDeleted = Builders<Permission>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filter & filterIsDeleted & UserOnlyFilter).ToList();
        }
        public virtual void Insert(Permission obj)
        {
            if (string.IsNullOrEmpty(obj.Id)) obj.Id = Guid.NewGuid().ToString();
            obj.CreateBy = _userHelper.GetUserName();
            obj.CreateAt = DateTime.Now;
            _collection.InsertOne(obj);
        }
        public virtual void InsertMany(List<Permission> objs)
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
        public virtual PaginationData<Permission> Search(Expression<Func<Permission, bool>> predicate, int? page = null, int? limit = null, string sortField = null, bool sortDesc = false)
        {
            SortDefinition<Permission> sort = null;
            var filter = Builders<Permission>.Filter.Where(predicate);
            var filterIsDeleted = Builders<Permission>.Filter.Eq(e => e.IsDeleted, false);
            var count = _collection.CountDocuments(filter & filterIsDeleted & UserOnlyFilter);

            if (!page.HasValue || !limit.HasValue)
            {
                var resNoLimit = string.IsNullOrEmpty(sortField) ? _collection.Find(filter & filterIsDeleted & UserOnlyFilter) : _collection.Find(filter & filterIsDeleted & UserOnlyFilter).Sort(sort);
                return new PaginationData<Permission>(resNoLimit.ToList(), page, limit, count);
            }

            if (!string.IsNullOrEmpty(sortField))
                sort = sortDesc ? Builders<Permission>.Sort.Descending(sortField) : Builders<Permission>.Sort.Ascending(sortField);
            var res = string.IsNullOrEmpty(sortField) ? _collection.Find(predicate & filterIsDeleted & UserOnlyFilter).Skip((page - 1) * limit).Limit(limit) : _collection.Find(predicate & filterIsDeleted & UserOnlyFilter).Sort(sort).Skip((page - 1) * limit).Limit(limit);
            return new PaginationData<Permission>(res.ToList(), page, limit, count);
        }
        public List<Permission> SearchLikeInFieldAutoComplete(string field, string value, int limit = 10, Expression<Func<Permission, bool>> predicate = null)
        {
            var fieldsSplit = field.Split(',');
            var filters = fieldsSplit.Select(f => Builders<Permission>.Filter.Regex(f, new BsonRegularExpression($"/^.*{value}.*$/i"))).ToList();
            var filterIsDeleted = Builders<Permission>.Filter.Eq(u => u.IsDeleted, false);
            var filterAnyEq = filters.Count > 1 ? Builders<Permission>.Filter.Or(filters) : Builders<Permission>.Filter.Regex(field, new BsonRegularExpression($"/^.*{value}.*$/i"));
            var filterWhere = predicate == null ? Builders<Permission>.Filter.Empty : Builders<Permission>.Filter.Where(predicate);

            return _collection.Find(filterAnyEq & filterIsDeleted & UserOnlyFilter & filterWhere).Limit(limit).ToList();
        }
        public virtual void Update(string id, Permission obj)
        {
            var entity = GetById(id);
            if (entity == null) return;

            obj.CreateAt = entity.CreateAt;
            obj.CreateBy = entity.CreateBy;
            obj.DeleteAt = entity.DeleteAt;
            obj.DeleteBy = entity.DeleteBy;
            obj.UpdateBy = _userHelper.GetUserName();
            obj.UpdateAt = DateTime.Now;

            var filter = Builders<Permission>.Filter.Eq(e => e.Id, id);
            _collection.ReplaceOne<Permission>(o => o.Id == id, obj);
        }
        public Permission SearchFirstByField(string field, string value, bool isDeleted = false)
        {
            var eqIsDeleted = Builders<Permission>.Filter.Eq(e => e.IsDeleted, isDeleted);
            var eqFilter = Builders<Permission>.Filter.Eq(field, value);
            return _collection.Find(eqFilter & eqIsDeleted & UserOnlyFilter).FirstOrDefault();
        }
        public Permission GetByField(string field, string value)
        {
            var filterField = Builders<Permission>.Filter.Eq(field, value);
            var filterIsDeleted = Builders<Permission>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterField & filterIsDeleted & UserOnlyFilter).FirstOrDefault();
        }
        public bool Exists(Expression<Func<Permission, bool>> predicate)
        {
            var filterPredicate = Builders<Permission>.Filter.Where(predicate);
            var filterIsDeleted = Builders<Permission>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterPredicate & filterIsDeleted & UserOnlyFilter).CountDocuments() > 0;
        }
    }
}

