

using Simjob.Framework.Infra.Schemas.Validations.Entities;

namespace Simjob.Framework.Infra.Schemas.Commands.Entities
{
    public class UpdateEntityCommand : EntityCommand
    {
        public override bool IsValid()
        {
            ValidationResult = new UpdateEntityValidation().Validate(this);

            return base.IsValid();
        }
    }
}
