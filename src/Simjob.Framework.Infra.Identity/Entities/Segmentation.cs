using Simjob.Framework.Domain.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Entities
{
    public class Segmentation : Entity
    {
        public Segmentation(string schemaName, string field, string[] values, string userId)
        {
            SchemaName = schemaName;
            Field = field;
            Values = values;
            UserId = userId;
        }

        public Segmentation()
        {
            
        }
        public string SchemaName { get; set; }
        public string Field { get; set; }
        public string[] Values { get; set; }
        public string UserId { get; set; }

    }
}
