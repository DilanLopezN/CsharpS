using MongoDB.Driver;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Entities;

namespace Simjob.Framework.Infra.Identity.Contexts
{
    public class PermissionContext : MongoDbContext
    {
        private const string USERDATABASENAME = "";
        private const string USERCOLLECTIONNAME = "permission";
        protected readonly IUserHelper _userHelper;

        public PermissionContext(IUserHelper userHelper) : base(userHelper)
        {
        }

        public IMongoCollection<Permission> GetUserCollection()
        {
            return GetDatabase().GetCollection<Permission>(USERCOLLECTIONNAME);
        }


    }
}
