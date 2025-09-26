using MongoDB.Driver;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Entities;

namespace Simjob.Framework.Infra.Identity.Contexts
{
    public class NotificationContext : MongoDbContext
    {
        public string USERDATABASENAME = "";
        private const string USERCOLLECTIONNAME = "notification";
        protected readonly IUserHelper _userHelper;

        public NotificationContext(IUserHelper userHelper) : base(userHelper)
        {
        }
        public IMongoCollection<Notification> GetUserCollection()
        {
            return GetDatabase().GetCollection<Notification>(USERCOLLECTIONNAME);
        }
        //public override IMongoDatabase GetDatabase()
        //{
        //    return Client.GetDatabase(USERDATABASENAME);
        //}

        //public override IMongoDatabase GetDatabase()
        //{
        //    return Client.GetDatabase(_userHelper.GetTenanty() ?? "_empty");
        //}
    }
}
