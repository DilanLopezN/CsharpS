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
    public class GeneratorsRepository : IGeneratorsRepository
    {
        private const string UserCollectionName = "generators";
        protected readonly GeneratorsContext _context;
        protected readonly IMongoCollection<Generators> _collection;
        protected readonly IUserHelper _userHelper;
        protected bool UserOnly;
        protected FilterDefinition<Generators> UserOnlyFilter = Builders<Generators>.Filter.Empty;

        [ExcludeFromCodeCoverage]

        public GeneratorsRepository(GeneratorsContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
            var claims = _userHelper.GetClaims();
            if (claims.Any()) UserOnly = claims.Exists(c => c.Value == "useronly" && c.Type == typeof(User).Name);
            if (UserOnly)
                UserOnlyFilter = Builders<Generators>.Filter.Eq("CreateBy", _userHelper.GetUserName());
        }

        public bool Exists(Expression<Func<Generators, bool>> predicate)
        {
            var filterPredicate = Builders<Generators>.Filter.Where(predicate);
            var filterIsDeleted = Builders<Generators>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterPredicate & filterIsDeleted & UserOnlyFilter).CountDocuments() > 0;
        }

        public Generators GetById(string id)
        {
            var filterId = Builders<Generators>.Filter.Eq(e => e.Id, id);
            var filterIsDeleted = Builders<Generators>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterId & filterIsDeleted & UserOnlyFilter).FirstOrDefault();
        }

        public void Insert(Generators obj)
        {
            if (string.IsNullOrEmpty(obj.Id)) obj.Id = Guid.NewGuid().ToString();
            obj.CreateBy = _userHelper.GetUserName();
            obj.CreateAt = DateTime.Now;
            _collection.InsertOne(obj);
        }
    }
}
