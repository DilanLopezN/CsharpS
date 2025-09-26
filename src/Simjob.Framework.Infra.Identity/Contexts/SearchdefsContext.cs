using MongoDB.Driver;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Domain.Models;
using Simjob.Framework.Infra.Data.Context;

namespace Simjob.Framework.Infra.Identity.Contexts
{
    public class SearchdefsContext : MongoDbContext
    {
        private const string USERDATABASENAME = "";
        private const string USERCOLLECTIONNAME = "searchdefs";
        protected readonly IUserHelper _userHelper;

        public SearchdefsContext(IUserHelper userHelper) : base(userHelper)
        {

        }
        public IMongoCollection<Searchdefs> GetUserCollection()
        {
            return GetDatabase().GetCollection<Searchdefs>(USERCOLLECTIONNAME);
        } 
      
    }
}
