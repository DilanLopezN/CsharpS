using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Models.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Interfaces
{
    public interface IUserAdminService
    {
        public bool LoginAdmin(string tenanty, string username, string password);
        public UserAdmin GetUserAdminById(string id);
        public UserAdmin GetByUserName(string tenanty, string username);
        public bool Register(UserAdmin user, string password);
        public void Update(UserAdmin user, string groupId);

        public void DeleteUserAdmin(string id);
        Task<object> SearchFields(string tenanty, string value, int? page, int? limit, string sortField = null, bool sortDesc = false, string ids = null);
    }
}
