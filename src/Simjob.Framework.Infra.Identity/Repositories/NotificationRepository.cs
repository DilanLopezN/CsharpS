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
    public class NotificationRepository : INotificationRepository
    {
        private const string UserCollectionName = "notification";
        protected readonly NotificationContext _context;
        protected readonly IMongoCollection<Notification> _collection;
        protected readonly IUserHelper _userHelper;
        protected bool UserOnly;
        protected FilterDefinition<Notification> UserOnlyFilter = Builders<Notification>.Filter.Empty;

        public NotificationRepository(NotificationContext context, IUserHelper userHelper, string collectionName = null)
        {
            _context = context;
            _collection = _context.GetDatabase().GetCollection<Notification>(collectionName ?? typeof(Notification).Name.ToLower());
            _userHelper = userHelper;
            var claims = _userHelper.GetClaims();
            if (claims != null && claims.Any()) UserOnly = claims.Exists(c => c.Value == "useronly" && c.Type == typeof(Notification).Name);
            if (UserOnly)
                UserOnlyFilter = Builders<Notification>.Filter.Eq("CreateBy", _userHelper.GetUserName());
        }
        public virtual void Delete(string id)
        {
            var username = _userHelper.GetUserName();

            var entity = this.GetById(id);
            if (entity == null) return;
            var filter = Builders<Notification>.Filter.Eq(e => e.Id, id);
            var updateIsDeleted = Builders<Notification>.Update.Set(o => o.IsDeleted, true)
                .Set(o => o.DeleteAt, DateTime.Now)
                .Set(o => o.DeleteBy, username);

            _collection.UpdateOne(filter, updateIsDeleted);
        }

        public bool Exists(Expression<Func<Notification, bool>> predicate)
        {
            var filterPredicate = Builders<Notification>.Filter.Where(predicate);
            var filterIsDeleted = Builders<Notification>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterPredicate & filterIsDeleted & UserOnlyFilter).CountDocuments() > 0;
        }

        //public PaginationData<Notification> GetAllByUserIdAndName(string userId, string userName, int? page = null, int? limit = null, string sortField = null, bool sortDesc = false, bool addIsDeleted = false, string ids = null)
        //{
        //    if (ids != null && !ids.Any()) return new PaginationData<Notification>(new List<Notification>() { }, page, limit, 0);

        //    SortDefinition<Notification> sort = null;
        //    var filterIds = string.IsNullOrEmpty(ids) ? Builders<Notification>.Filter.Empty : Builders<Notification>.Filter.In("Id", ids.Split(','));
        //    var filterIsDeleted = addIsDeleted ? Builders<Notification>.Filter.Empty : Builders<Notification>.Filter.Where(e => e.IsDeleted == false);
        //    var filterUserName = Builders<Notification>.Filter
        //                             .ElemMatch(t => t.AprovEmail,
        //                             x => x.email == userName);

        //    //var subFilter = Builders<Notification>.Filter.Eq(x => x.AprovEmail.FirstOrDefault().email, userName);
        //    var filterUserId = Builders<Notification>.Filter.Eq(u => u.UserId, userId);
        //    var filterIsAprovNull = Builders<Notification>.Filter.Eq(u => u.Aprov, null);

        //    var filters = (filterUserId | filterUserName) & filterIsDeleted & filterIsAprovNull;
        //    if (!string.IsNullOrEmpty(sortField))
        //        sort = sortDesc ? Builders<Notification>.Sort.Descending(sortField) : Builders<Notification>.Sort.Ascending(sortField);
        //    var count = _collection.CountDocuments(filters);

        //    if (!page.HasValue || !limit.HasValue)
        //    {
        //        var resNoLimit = string.IsNullOrEmpty(sortField) ? _collection.Find(filters) : _collection.Find(filters).Sort(sort);
        //        return new PaginationData<Notification>(resNoLimit.ToList(), page, limit, count);
        //    }

        //    var res = string.IsNullOrEmpty(sortField) ? _collection.Find(filters).Skip((page - 1) * limit).Limit(limit) : _collection.Find(filters).Sort(sort).Skip((page - 1) * limit).Limit(limit);
        //    return new PaginationData<Notification>(res.ToList(), page, limit, count);
        //}

        public PaginationData<Notification> GetAllByUserIdAndName(string userId, string userName, int? page = null, int? limit = null, string sortField = null, bool sortDesc = false, bool addIsDeleted = false, string ids = null)
        {
            if (ids != null && !ids.Any()) return new PaginationData<Notification>(new List<Notification>() { }, page, limit, 0);

            SortDefinition<Notification> sort = null;
            var filterIds = string.IsNullOrEmpty(ids) ? Builders<Notification>.Filter.Empty : Builders<Notification>.Filter.In("Id", ids.Split(','));
            var filterIsDeleted = addIsDeleted ? Builders<Notification>.Filter.Empty : Builders<Notification>.Filter.Where(e => e.IsDeleted == false);
            var filterUserName = Builders<Notification>.Filter
                                     .ElemMatch(t => t.AprovEmail,
                                     x => x.email == userName && x.view == false);

            //var subFilter = Builders<Notification>.Filter.Eq(x => x.AprovEmail.FirstOrDefault().email, userName);
           

            var filterView = Builders<Notification>.Filter.Where(u => u.View == false || (u.View == true && u.Aprov == null));

            var filters =  (filterUserName  | filterIsDeleted) & filterView;
            if (!string.IsNullOrEmpty(sortField))
                sort = sortDesc ? Builders<Notification>.Sort.Descending(sortField) : Builders<Notification>.Sort.Ascending(sortField);
            var count = _collection.CountDocuments(filters);

            if (!page.HasValue || !limit.HasValue)
            {
                var resNoLimit = string.IsNullOrEmpty(sortField) ? _collection.Find(filters) : _collection.Find(filters).Sort(sort);
                return new PaginationData<Notification>(resNoLimit.ToList(), page, limit, count);
            }

            var res = string.IsNullOrEmpty(sortField) ? _collection.Find(filters).Skip((page - 1) * limit).Limit(limit) : _collection.Find(filters).Sort(sort).Skip((page - 1) * limit).Limit(limit);
            return new PaginationData<Notification>(res.ToList(), page, limit, count);
        }

        public virtual Notification GetById(string id)
        {
            var filterId = Builders<Notification>.Filter.Eq(e => e.Id, id);
            var filterIsDeleted = Builders<Notification>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterId & filterIsDeleted & UserOnlyFilter).FirstOrDefault();
        }

        public virtual void Insert(Notification obj)
        {
            if (string.IsNullOrEmpty(obj.Id)) obj.Id = Guid.NewGuid().ToString();
            //obj.CreateBy = _userHelper.GetUserName();
            obj.CreateAt = DateTime.Now;
            _collection.InsertOne(obj);
        }

        public virtual void Update(string id, Notification obj)
        {
            var entity = GetById(id);
            if (entity == null) return;

            obj.CreateAt = entity.CreateAt;
            obj.CreateBy = entity.CreateBy;
            obj.DeleteAt = entity.DeleteAt;
            obj.DeleteBy = entity.DeleteBy;
            //obj.UpdateBy = _userHelper.GetUserName();
            obj.UpdateAt = DateTime.Now;

            var filter = Builders<Notification>.Filter.Eq(e => e.Id, id);
            _collection.ReplaceOne<Notification>(o => o.Id == id, obj);
        }
    }
}
