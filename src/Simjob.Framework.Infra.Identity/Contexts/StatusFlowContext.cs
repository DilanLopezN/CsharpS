using MongoDB.Driver;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Entities;

namespace Simjob.Framework.Infra.Identity.Contexts
{
    public class StatusFlowContext : MongoDbContext
    {
        public string USERDATABASENAME = "identity";
        private const string USERCOLLECTIONNAME = "statusflow";
        public StatusFlowContext(IUserHelper userHelper) : base(userHelper)
        {
        }
        public IMongoCollection<StatusFlow> GetUserCollection()
        {
            return GetDatabase().GetCollection<StatusFlow>(USERCOLLECTIONNAME);
        }
        public override IMongoDatabase GetDatabase()
        {
            return Client.GetDatabase(USERDATABASENAME);
        }
    }
}
