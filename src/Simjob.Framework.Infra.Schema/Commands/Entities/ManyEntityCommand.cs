using Simjob.Framework.Domain.Core.Commands;
using System.Collections.Generic;

namespace Simjob.Framework.Infra.Schemas.Commands.Entities
{
    public abstract class ManyEntityCommand : Command
    {
        public ManyEntityCommand()
        {

        }

        public ManyEntityCommand(string schemaName, List<Dictionary<string, object>> datas)
        {
            SchemaName = schemaName;
            Datas = datas;
        }

        public string SchemaName { get; set; }
        public string SchemaJson { get; set; }
        public List<Dictionary<string, object>> Datas { get; set; }
    }
}
