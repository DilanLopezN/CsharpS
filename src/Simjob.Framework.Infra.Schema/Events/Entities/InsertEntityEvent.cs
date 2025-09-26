using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Schemas.Events.Entities
{
    public class InsertEntityEvent : EntityEvent
    {
        public InsertEntityEvent(string id, string schemaName, string schemaJson, Dictionary<string, object> data) : base(id, schemaName, schemaJson, data)
        {
        }
    }
}
