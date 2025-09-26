

using Simjob.Framework.Domain.Core.Commands;
using Simjob.Framework.Infra.Schemas.Validations.Entities;

namespace Simjob.Framework.Infra.Schemas.Commands.Entities
{
    public class DeleteEntityCommand : Command
    {
        public string Id { get; set; }
        public string SchemaName { get; set; }
        public override bool IsValid()
        {
            ValidationResult = new DeleteEntityValidation().Validate(this);

            return base.IsValid();
        }
    }
}
