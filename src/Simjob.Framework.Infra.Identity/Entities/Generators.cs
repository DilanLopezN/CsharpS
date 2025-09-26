using Simjob.Framework.Domain.Core.Entities;
using Simjob.Framework.Infra.Schemas.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Entities
{
    public class Generators : Entity
    {
        public Generators(string schema, string code, string sequencia)
        {
            Schema = schema;
            Code = code;
            Sequencia = sequencia;
        }
        public Generators(string id,string schema, string code, string sequencia)
        {
            Id = id;
            Schema = schema;
            Code = code;
            Sequencia = sequencia;
        }

        public string Schema { get; set; }
        public string Code { get; set; }
        public string Sequencia { get; set; }
    }
    
}
