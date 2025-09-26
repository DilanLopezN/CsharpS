using MongoDB.Driver;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Repositories
{
    public class SegmentationRepository : ISegmentationRepository
    {
        private const string UserCollectionName = "segmentation";
        protected readonly SegmentationContext _context;
        protected readonly IMongoCollection<Segmentation> _collection;
        protected readonly IUserHelper _userHelper;
        protected bool UserOnly;
        protected FilterDefinition<Segmentation> UserOnlyFilter = Builders<Segmentation>.Filter.Empty;

        [ExcludeFromCodeCoverage]

        public SegmentationRepository(SegmentationContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
            var claims = _userHelper.GetClaims();
            if (claims.Any()) UserOnly = claims.Exists(c => c.Value == "useronly" && c.Type == typeof(User).Name);
            if (UserOnly)
                UserOnlyFilter = Builders<Segmentation>.Filter.Eq("CreateBy", _userHelper.GetUserName());
        }


        public void Delete(string id)
        {
            var username = _userHelper.GetUserName();

            var entity = this.GetById(id);
            if (entity == null) return;
            var filter = Builders<Segmentation>.Filter.Eq(e => e.Id, id);
            var updateIsDeleted = Builders<Segmentation>.Update.Set(o => o.IsDeleted, true)
                .Set(o => o.DeleteAt, DateTime.Now)
                .Set(o => o.DeleteBy, username);

            _collection.UpdateOne(filter, updateIsDeleted);
        }

        public bool Exists(Expression<Func<Segmentation, bool>> predicate)
        {
            var filterPredicate = Builders<Segmentation>.Filter.Where(predicate);
            var filterIsDeleted = Builders<Segmentation>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterPredicate & filterIsDeleted & UserOnlyFilter).CountDocuments() > 0;
        }

        public Segmentation GetById(string id)
        {
            var filterId = Builders<Segmentation>.Filter.Eq(e => e.Id, id);
            var filterIsDeleted = Builders<Segmentation>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterId & filterIsDeleted & UserOnlyFilter).FirstOrDefault();
        }

        public void Insert(Segmentation obj)
        {
            if (string.IsNullOrEmpty(obj.Id)) obj.Id = Guid.NewGuid().ToString();
            obj.CreateBy = _userHelper.GetUserName();
            obj.CreateAt = DateTime.Now;
            _collection.InsertOne(obj);
        }

        public void Update(string id, Segmentation obj)
        {
            var entity = GetById(id);
            if (entity == null) return;

            obj.CreateAt = entity.CreateAt;
            obj.CreateBy = entity.CreateBy;
            obj.DeleteAt = entity.DeleteAt;
            obj.DeleteBy = entity.DeleteBy;
            obj.UpdateBy = _userHelper.GetUserName();
            obj.UpdateAt = DateTime.Now;

            var filter = Builders<Segmentation>.Filter.Eq(e => e.Id, id);
            _collection.ReplaceOne<Segmentation>(o => o.Id == id, obj);
        }
    }
}
