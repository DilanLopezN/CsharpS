using Simjob.Framework.Infra.Schemas.Validations.Schemas;

namespace Simjob.Framework.Infra.Schemas.Commands
{
    public class InsertSchemaCommand : SchemaCommand
    {
        public override bool IsValid()
        {
            ValidationResult = new InsertSchemaValidation().Validate(this);
            return base.IsValid();
        }
    }
}
