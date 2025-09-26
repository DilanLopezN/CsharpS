using MongoDB.Driver;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Entities;

namespace Simjob.Framework.Infra.Identity.Contexts
{
    public class SharedSchemaRecordContext : MongoDbContext
    {
        public string USERDATABASENAME = "";
        private const string USERCOLLECTIONNAME = "sharedschemarecord";
        protected readonly IUserHelper _userHelper;

        public SharedSchemaRecordContext(IUserHelper userHelper) : base(userHelper)
        {
        }

        public IMongoCollection<SharedSchemaRecord> GetUserCollection()
        {
            return GetDatabase().GetCollection<SharedSchemaRecord>(USERCOLLECTIONNAME);
        }


    }
}
