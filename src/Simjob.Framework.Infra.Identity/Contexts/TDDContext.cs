using MongoDB.Driver;
using Simjob.Framework.Domain.Core.Entities;
using Simjob.Framework.Domain.Data;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Entities;

namespace Simjob.Framework.Test.Services.Api
{
    public class TDDContext : MongoDbContext
    {
        private const string USERDATABASENAME = "";
        public TDDContext(IUserHelper userHelper) : base(userHelper)
        {
        }


        public IMongoCollection<User> GetUserCollection(string collection)
        {
            return GetDatabase().GetCollection<User>(collection);
        }

        public override IMongoDatabase GetDatabase()
        {
            return Client.GetDatabase("_empty");
        }
    }
}
