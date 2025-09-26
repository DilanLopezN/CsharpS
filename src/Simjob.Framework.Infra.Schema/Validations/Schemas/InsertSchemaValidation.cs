using Simjob.Framework.Infra.Schemas.Commands;

namespace Simjob.Framework.Infra.Schemas.Validations.Schemas
{
    public class InsertSchemaValidation : SchemaValidation<InsertSchemaCommand>
    {
        public InsertSchemaValidation()
        {
            NameValidation();
            JsonValueValidation();
        }
    }
}
