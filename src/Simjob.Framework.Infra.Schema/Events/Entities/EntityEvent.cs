using Simjob.Framework.Domain.Core.Events;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Simjob.Framework.Infra.Schemas.Events.Entities
{
    [ExcludeFromCodeCoverage]
    public abstract class EntityEvent : Event
    {
        protected EntityEvent(string id, string schemaName, string schemaJson, Dictionary<string, object> data)
        {
            AggregateId = id;
            SchemaName = schemaName;
            SchemaJson = schemaJson;
            Data = data;
        }

        public string SchemaName { get; set; }
        public string SchemaJson { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }
}
