using System.Collections.Generic;

namespace Simjob.Framework.Infra.Schemas.Events.Entities
{
    public class UpdateEntityEvent : EntityEvent
    {
        public UpdateEntityEvent(string id, string schemaName, string schemaJson, Dictionary<string, object> data) : base(id, schemaName, schemaJson, data)
        {
        }
    }
}
