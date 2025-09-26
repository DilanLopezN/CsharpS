

using Simjob.Framework.Infra.Schemas.Validations.Entities;

namespace Simjob.Framework.Infra.Schemas.Commands.Entities
{
    public class InsertEntityCommand : EntityCommand
    {
        public override bool IsValid()
        {
            ValidationResult = new InsertEntityValidation().Validate(this);

            return base.IsValid();
        }
    }
}
