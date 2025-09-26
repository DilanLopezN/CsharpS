using Simjob.Framework.Domain.Util;
using Simjob.Framework.Infra.Identity.Commands;
using Simjob.Framework.Infra.Identity.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Interfaces
{
    public interface ISharedSchemaRecordService
    {
        public SharedSchemaRecord RegisterAsync(SharedSchemaRecordCommand command);
        public SharedSchemaRecord GetById(string id);
        public Task<PaginationData<SharedSchemaRecord>> GetBySchemaRecordIdAndUserIdSender(int? page, int? limit, string sortField = null, bool sortDesc = false, string schemaRecordId = null, string userId = null);
        public Task<PaginationData<SharedSchemaRecord>> GetByUserIdReceive(int? page, int? limit, string sortField = null, bool sortDesc = false, string userId = null);



        public SharedSchemaRecord Update(SharedSchemaRecordCommand sharedschemarecord, string id);
        public void Delete(string id);
        public void DeleteByUserIdReceive(string schemaRecordId, string userId);


        Task<object> GetAll(int? page, int? limit, string sortField = null, bool sortDesc = false);

        public List<SharedSchemaRecord> GetBySchemaNameAndUserIdReceive(string schemaName, string userId);
    }
}
