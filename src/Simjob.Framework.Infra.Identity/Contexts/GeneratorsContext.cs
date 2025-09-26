using MongoDB.Driver;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Contexts
{
    public class GeneratorsContext : MongoDbContext
    {
        public string USERDATABASENAME = "identity";
        private const string USERCOLLECTIONNAME = "generators";
        public GeneratorsContext(IUserHelper userHelper) : base(userHelper)
        {
        }
        public IMongoCollection<Generators> GetUserCollection()
        {
            return GetDatabase().GetCollection<Generators>(USERCOLLECTIONNAME);
        }
        public override IMongoDatabase GetDatabase()
        {
            return Client.GetDatabase(USERDATABASENAME);
        }
    }
}
