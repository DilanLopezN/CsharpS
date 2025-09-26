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
    public class ModulePermissionContext : MongoDbContext
    {
        public string USERDATABASENAME = "";
        private const string USERCOLLECTIONNAME = "modulepermission";
        protected readonly IUserHelper _userHelper;
        public ModulePermissionContext(IUserHelper userHelper) : base(userHelper)
        {
        }

        public IMongoCollection<ModulePermission> GetUserCollection()
        {
            return GetDatabase().GetCollection<ModulePermission>(USERCOLLECTIONNAME);
        }
    }
}
