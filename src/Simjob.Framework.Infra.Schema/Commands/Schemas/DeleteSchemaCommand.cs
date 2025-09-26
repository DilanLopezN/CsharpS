using Simjob.Framework.Infra.Schemas.Validations.Schemas;

namespace Simjob.Framework.Infra.Schemas.Commands
{
    public class DeleteSchemaCommand : SchemaCommand
    {
        public override bool IsValid()
        {
            ValidationResult = new DeleteSchemaValidation().Validate(this);
            return base.IsValid();
        }
    }
}
