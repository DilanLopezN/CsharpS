using MongoDB.Driver;
using Simjob.Framework.Domain.Core.Entities;
using Simjob.Framework.Domain.Data;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Entities;

namespace Simjob.Framework.Infra.Identity.Contexts
{
    public class IdentityContext : MongoDbContext
    {
        private const string USERDATABASENAME = "identity";
        private const string USERCOLLECTIONNAME = "user";
        public IdentityContext(IUserHelper userHelper) : base(userHelper)
        {
        }


        public IMongoCollection<User> GetUserCollection()
        {
            return GetDatabase().GetCollection<User>(USERCOLLECTIONNAME);
        }

        public override IMongoDatabase GetDatabase()
        {
            return Client.GetDatabase(USERDATABASENAME);
        }
    }
}
