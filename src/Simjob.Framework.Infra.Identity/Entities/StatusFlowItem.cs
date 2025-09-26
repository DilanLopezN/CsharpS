using Simjob.Framework.Domain.Core.Entities;

namespace Simjob.Framework.Infra.Identity.Entities
{
    public class StatusFlowItem : Entity
    {
        public string StatusFlowId { get; set; }
        public string SchemaRecordId { get; set; }
        public string Status { get; set; }
        public string[] AprovEmail { get; set; }

        public StatusFlowItem(string statusFlowId, string schemaRecordId, string status, string[] aprovEmail)
        {
            StatusFlowId = statusFlowId;
            SchemaRecordId = schemaRecordId;
            Status = status;
            AprovEmail = aprovEmail;
        }
    }
}
