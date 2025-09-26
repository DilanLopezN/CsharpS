using Simjob.Framework.Domain.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Entities
{
    public class ApproveLog : Entity
    {
        public ApproveLog(string schemaRecordId, string schemaName, string field, string value, string msg)
        {
            SchemaRecordId = schemaRecordId;
            SchemaName = schemaName;
            Field = field;
            Value = value;
            Msg = msg;
        }
        public string SchemaRecordId { get; set; }
        public string SchemaName { get; set; }
        public string Field { get; set; }
        public string Value { get; set; }
        public string Msg { get; set; }
    }
}
