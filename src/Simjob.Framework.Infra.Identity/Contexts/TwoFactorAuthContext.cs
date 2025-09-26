using MongoDB.Driver;
using Simjob.Framework.Domain.Data;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Entities;

namespace Simjob.Framework.Infra.Identity.Contexts
{
    public class TwoFactorAuthContext : MongoDbContext
    {
        private const string USERDATABASENAME = "identity";
        private const string USERCOLLECTIONNAME = "usertwofactorauth";
        public TwoFactorAuthContext(IUserHelper userHelper) : base(userHelper)
        {
        }

        public IMongoCollection<UserTwoFactorAuth> GetUserCollection()
        {
            return GetDatabase().GetCollection<UserTwoFactorAuth>(USERCOLLECTIONNAME);
        }

        public override IMongoDatabase GetDatabase()
        {
            return Client.GetDatabase(USERDATABASENAME);
        }
    }
}
