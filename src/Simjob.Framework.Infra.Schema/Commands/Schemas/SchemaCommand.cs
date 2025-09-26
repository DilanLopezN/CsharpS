using Simjob.Framework.Domain.Core.Commands;

namespace Simjob.Framework.Infra.Schemas.Commands
{
    public abstract class SchemaCommand : Command
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool StrongEntity { get; set; }       


        public string JsonValue { get; set; }
    }
}
