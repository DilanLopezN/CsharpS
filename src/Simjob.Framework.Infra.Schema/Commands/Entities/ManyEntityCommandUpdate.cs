using Simjob.Framework.Domain.Core.Commands;
using System.Collections.Generic;

namespace Simjob.Framework.Infra.Schemas.Commands.Entities
{
    public abstract class ManyEntityCommandUpdate : Command
    {
        public ManyEntityCommandUpdate() { }
        public ManyEntityCommandUpdate(string id, string schemaName, string schemaJson, List<Dictionary<string, object>> datas)
        {
            Id = id;
            SchemaName = schemaName;
            SchemaJson = schemaJson;
            Datas = datas;
        }

        public string Id { get; set; }
        public string SchemaName { get; set; }
        public string SchemaJson { get; set; }
        public List<Dictionary<string, object>> Datas { get; set; }
    }


}
