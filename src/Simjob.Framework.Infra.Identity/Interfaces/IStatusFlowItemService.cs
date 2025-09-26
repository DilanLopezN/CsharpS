using Simjob.Framework.Infra.Identity.Entities;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Interfaces
{
    public interface IStatusFlowItemService
    {
        public StatusFlowItem RegisterAsync(StatusFlowItem statusFlowItem);
        public StatusFlowItem GetById(string id);
        public void DeleteStatusFlowItem(string id);
        public void UpdateStatusFlowItem(StatusFlowItem statusFlowItem, string id);
        public StatusFlowItem GetBySchemaRecordId(string schemaRecordId);

        Task<object> GetAll(int? page, int? limit, string sortField = null, bool sortDesc = false);
    }
}
