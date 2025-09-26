using Simjob.Framework.Domain.Core.Events;

namespace Simjob.Framework.Infra.Schemas.Events.Entities
{
    public class DeleteEntityEvent : Event
    {
        public DeleteEntityEvent(string id, string schemaName)
        {
            Id = id;
            SchemaName = schemaName;
        }

        public string Id { get; set; }
        public string SchemaName { get; set; }
    }
}
