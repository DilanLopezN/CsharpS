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
    public class ExternalTokensContext : MongoDbContext
    {
        public string USERDATABASENAME = "identity";
        private const string USERCOLLECTIONNAME = "tokens";
        public ExternalTokensContext(IUserHelper userHelper) : base(userHelper)
        {
        }
        public IMongoCollection<Tokens> GetUserCollection()
        {
            return GetDatabase().GetCollection<Tokens>(USERCOLLECTIONNAME);
        }
        public override IMongoDatabase GetDatabase()
        {
            return Client.GetDatabase(USERDATABASENAME);
        }
    }
}

