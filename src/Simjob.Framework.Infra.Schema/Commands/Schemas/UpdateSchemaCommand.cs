using Simjob.Framework.Infra.Schemas.Validations.Schemas;

namespace Simjob.Framework.Infra.Schemas.Commands
{
    public class UpdateSchemaCommand : SchemaCommand
    {
        public override bool IsValid()
        {
            ValidationResult = new UpdateSchemaValidation().Validate(this);
            return base.IsValid();
        }
    }
}
