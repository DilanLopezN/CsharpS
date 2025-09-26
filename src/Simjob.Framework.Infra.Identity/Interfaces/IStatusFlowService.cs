using Simjob.Framework.Infra.Identity.Commands;
using Simjob.Framework.Infra.Identity.Entities;
using System.Collections.Generic;

namespace Simjob.Framework.Infra.Identity.Interfaces
{
    public interface IStatusFlowService
    {
        public void Register(StatusFlowCommand statusFlow);
        public void UpdateStatusFlowStatus(StatusFlow statusFlow, List<string> aprovadores, string status);
        public StatusFlow GetById(string id);
        public StatusFlow GetBySchemaAndField(string schemaName, string field, string tenanty);
        public void DeleteStatusFlow(string id);
        //public void UpdateStatusFlow(StatusFlowCommand statusFlow);

        //Task<object> GetAll(int? page, int? limit, string sortField = null, bool sortDesc = false);
    }
}
