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
   public class SegmentationContext : MongoDbContext
    {
        public string USERDATABASENAME = "identity";
        private const string USERCOLLECTIONNAME = "segmentation";
        public SegmentationContext(IUserHelper userHelper) : base(userHelper)
        {
        }
        public IMongoCollection<Segmentation> GetUserCollection()
        {
            return GetDatabase().GetCollection<Segmentation>(USERCOLLECTIONNAME);
        }
        public override IMongoDatabase GetDatabase()
        {
            return Client.GetDatabase(USERDATABASENAME);
        }
    }
}
