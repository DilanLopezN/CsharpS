using MongoDB.Driver;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Entities;

namespace Simjob.Framework.Infra.Identity.Contexts
{
    public class StatusFlowItemContext : MongoDbContext
    {
        public string USERDATABASENAME = "";
        private const string USERCOLLECTIONNAME = "statusflowitem";
        protected readonly IUserHelper _userHelper;
        public StatusFlowItemContext(IUserHelper userHelper) : base(userHelper)
        {
        }
        public IMongoCollection<StatusFlowItem> GetUserCollection()
        {
            return GetDatabase().GetCollection<StatusFlowItem>(USERCOLLECTIONNAME);
        }
    }
}
