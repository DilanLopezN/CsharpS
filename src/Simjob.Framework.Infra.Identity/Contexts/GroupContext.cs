using MongoDB.Driver;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Entities;

namespace Simjob.Framework.Infra.Identity.Contexts
{
    public class GroupContext : MongoDbContext
    {
        public string USERDATABASENAME = "";
        private const string USERCOLLECTIONNAME = "group";
        protected readonly IUserHelper _userHelper;

        public GroupContext(IUserHelper userHelper) : base(userHelper)
        {
        }

        public IMongoCollection<Group> GetUserCollection()
        {
            return GetDatabase().GetCollection<Group>(USERCOLLECTIONNAME);
        }

        //public override IMongoDatabase GetDatabase()
        //{
        //    return Client.GetDatabase(_userHelper.GetTenanty() ?? "_empty");
        //}
    }
}
