using Simjob.Framework.Domain.Core.Commands;
using Simjob.Framework.Infra.Identity.Entities;

namespace Simjob.Framework.Infra.Identity.Commands
{
    public class StatusFlowCommand : Command
    {
        //public string Id { get; set; }
        public string SchemaName { get; set; }
        public string Tenanty { get; set; }
        public string Field { get; set; }
        public StatusFlow.ListProperties[] Properties { get; set; }

    }
}
