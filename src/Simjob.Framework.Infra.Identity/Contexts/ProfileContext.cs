using MongoDB.Driver;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Entities;

namespace Simjob.Framework.Infra.Identity.Contexts
{
    public class ProfileContext : MongoDbContext
    {
        private const string USERDATABASENAME = "identity";
        private const string USERCOLLECTIONNAME = "profile";
        protected readonly IUserHelper _userHelper;

        public ProfileContext(IUserHelper userHelper) : base(userHelper)
        {
        }
        public IMongoCollection<Profile> GetUserCollection()
        {
            return GetDatabase().GetCollection<Profile>(USERCOLLECTIONNAME);
        }
        public override IMongoDatabase GetDatabase()
        {
            return Client.GetDatabase(USERDATABASENAME);
        }

    }
}
