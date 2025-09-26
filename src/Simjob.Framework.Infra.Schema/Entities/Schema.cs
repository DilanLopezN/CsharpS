using Simjob.Framework.Domain.Core.Entities;

namespace Simjob.Framework.Infra.Schemas.Entities
{
    public class Schema : Entity
    {
        public Schema(string name, bool strongEntity, string jsonValue)
        {
            Name = name;
            StrongEntity = strongEntity;
            JsonValue = jsonValue;
        }

        public string Name { get; set; }
        public bool StrongEntity { get; set; }
        public string JsonValue { get; set; }
        
    }
}
