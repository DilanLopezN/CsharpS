using Microsoft.Extensions.Primitives;
using Simjob.Framework.Domain.Util;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Models;
using Simjob.Framework.Infra.Schemas.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Interfaces
{
    public interface IUserService
    {
        public bool Register(User user, string password);

        public User GetUserById(string id);

        public User GetUserByUserName(string userName, string tenanty);

        public List<User> GetUsersByUserName(string[] userName, string tenanty);

        public int EnableOrDisableA2F(User user, int a2f); // new

        public void UpdateRoot(User user, bool level);

        public void UpdateUserName(User user, string newUserName);

        public void UpdateApiKey(User user, string encryptedKey);

        public void UpdateGroup(User user, string groupId);

        public void UpdatePassword(User user, string oldPassword, string newPassword);

        public void UpdatePasswordAdmin(User user, string newPassword);
        public void UpdatePasswordLogin(User user, string newPassword);
        public void ResetPassword(User user, string newPassword);

        public bool Login(string tenanty, string username, string password);

        public User GetByUserName(string tenanty, string username);

        public User GetByApiKey(string apiKey);

        Task<IEnumerable<ModuleModel>> GetModules();

        List<User> GetUsersByGroupId(string groupId);
        List<User> GetUsersByGroupIds(List<string> groupIds);
        List<User> GetUsersByUserIds(List<string> userIds, string tenanty);

        List<User> GetUsersByTenanty(string tenanty);

        List<User> GetUsersByTenantyAndGroupID(string tenanty, string groupId);

        List<User> GetUsersByField();

        public void DeleteUser(string id);

        Task<IEnumerable<ModulePermissionActionModel>> GetModulesPermissions(Permission permission);

        Task<object> SerachFields(string tenanty, string value, int? page, int? limit, string sortField = null, bool sortDesc = false, string ids = null);

        PaginationData<User> SearchFieldsByTenanty(string groupId, string name, string value, int? page, int? limit, string sortField = null, bool sortDesc = false, string ids = null, string companySiteId = null, bool usuarioMatriz = false);

        List<User> GetByUserNameRegex(string id, string name);

        public void UpdateHashAzure(User user, string newHashAzure);

        public void UpdateLogonAzure(User user, bool logonAzure);

        public object ConvertOwnerId(object response, string userField);

        public object ConvertOwnerIdSingle(object response, string userField);

        public string ClaimUserId(StringValues accessToken);

        public string ClaimCompanySiteIds(StringValues accessToken);
    }
}