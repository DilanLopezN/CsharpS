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

namespace Simjob.Framework.Infra.Identity.Repositories
{
    public class StatusFlowRepository : IStatusFlowRepository
    {

        private const string UserCollectionName = "statusflow";
        protected readonly StatusFlowContext _context;
        protected readonly IMongoCollection<StatusFlow> _collection;
        protected readonly IUserHelper _userHelper;
        protected bool UserOnly;
        protected FilterDefinition<StatusFlow> UserOnlyFilter = Builders<StatusFlow>.Filter.Empty;

        public StatusFlowRepository(StatusFlowContext context, IUserHelper userHelper, string collectionName = null)
        {
            _context = context;
            _collection = _context.GetDatabase().GetCollection<StatusFlow>(collectionName ?? typeof(StatusFlow).Name.ToLower());
            _userHelper = userHelper;
            var claims = _userHelper.GetClaims();
            if (claims != null && claims.Any()) UserOnly = claims.Exists(c => c.Value == "useronly" && c.Type == typeof(StatusFlow).Name);
            if (UserOnly)
                UserOnlyFilter = Builders<StatusFlow>.Filter.Eq("CreateBy", _userHelper.GetUserName());
        }

        public virtual void Delete(string id)
        {
            var username = _userHelper.GetUserName();

            var entity = this.GetById(id);
            if (entity == null) return;
            var filter = Builders<StatusFlow>.Filter.Eq(e => e.Id, id);
            var updateIsDeleted = Builders<StatusFlow>.Update.Set(o => o.IsDeleted, true)
                .Set(o => o.DeleteAt, DateTime.Now)
                .Set(o => o.DeleteBy, username);

            _collection.UpdateOne(filter, updateIsDeleted);
        }

        public bool Exists(Expression<Func<StatusFlow, bool>> predicate)
        {
            var filterPredicate = Builders<StatusFlow>.Filter.Where(predicate);
            var filterIsDeleted = Builders<StatusFlow>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterPredicate & filterIsDeleted & UserOnlyFilter).CountDocuments() > 0;
        }

        public PaginationData<StatusFlow> GetAll(int? page = null, int? limit = null, string sortField = null, bool sortDesc = false, bool addIsDeleted = false, string ids = null)
        {
            if (ids != null && !ids.Any()) return new PaginationData<StatusFlow>(new List<StatusFlow>() { }, page, limit, 0);

            SortDefinition<StatusFlow> sort = null;

            var filterIsDeleted = addIsDeleted ? Builders<StatusFlow>.Filter.Empty : Builders<StatusFlow>.Filter.Where(e => e.IsDeleted == false);

            var filters = UserOnlyFilter & filterIsDeleted;
            if (!string.IsNullOrEmpty(sortField))
                sort = sortDesc ? Builders<StatusFlow>.Sort.Descending(sortField) : Builders<StatusFlow>.Sort.Ascending(sortField);
            var count = _collection.CountDocuments(filters);

            if (!page.HasValue || !limit.HasValue)
            {
                var resNoLimit = string.IsNullOrEmpty(sortField) ? _collection.Find(filters) : _collection.Find(filters).Sort(sort);
                return new PaginationData<StatusFlow>(resNoLimit.ToList(), page, limit, count);
            }

            var res = string.IsNullOrEmpty(sortField) ? _collection.Find(filters).Skip((page - 1) * limit).Limit(limit) : _collection.Find(filters).Sort(sort).Skip((page - 1) * limit).Limit(limit);
            return new PaginationData<StatusFlow>(res.ToList(), page, limit, count);
        }

        public virtual StatusFlow GetById(string id)
        {
            var filterId = Builders<StatusFlow>.Filter.Eq(e => e.Id, id);
            var filterIsDeleted = Builders<StatusFlow>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterId & filterIsDeleted & UserOnlyFilter).FirstOrDefault();
        }

        public virtual void Insert(StatusFlow obj)
        {
            if (string.IsNullOrEmpty(obj.Id)) obj.Id = Guid.NewGuid().ToString();
            obj.CreateBy = _userHelper.GetUserName();
            obj.CreateAt = DateTime.Now;
            _collection.InsertOne(obj);
        }

        public virtual void Update(string id, StatusFlow obj)
        {
            var entity = GetById(id);
            if (entity == null) return;

            obj.CreateAt = entity.CreateAt;
            obj.CreateBy = entity.CreateBy;
            obj.DeleteAt = entity.DeleteAt;
            obj.DeleteBy = entity.DeleteBy;
            obj.UpdateBy = _userHelper.GetUserName();
            obj.UpdateAt = DateTime.Now;

            var filter = Builders<StatusFlow>.Filter.Eq(e => e.Id, id);
            _collection.ReplaceOne<StatusFlow>(o => o.Id == id, obj);
        }
    }
}
