using Simjob.Framework.Infra.Identity.Commands.AccessGroup;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Models;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Interfaces
{
    public interface IUserAccessService
    {
        public Task Register(UserAccess userAccess);

        public UserAccess GetById(string id);

        public UserAccess GetByUserId(string id);

        public void DeleteUserAccess(string id);

        public void UpdateUserAccess(UserAccess userAccess, string id);

        public UserAccess GetBySchemaRecordId(string id);

        Task<object> GetAll(int? page, int? limit, string sortField = null, bool sortDesc = false);

        public bool SendEmail(SendEmailCommand command);

        bool SendEmail(ConfigSendGrid configSendGrid, SendEmailCommand command);
    }
}