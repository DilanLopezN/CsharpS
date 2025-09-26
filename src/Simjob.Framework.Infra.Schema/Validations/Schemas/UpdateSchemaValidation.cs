using Simjob.Framework.Infra.Schemas.Commands;

namespace Simjob.Framework.Infra.Schemas.Validations.Schemas
{
    public class UpdateSchemaValidation : SchemaValidation<UpdateSchemaCommand>
    {
        public UpdateSchemaValidation()
        {
            NameValidation();
            JsonValueValidation();
        }
    }

}
