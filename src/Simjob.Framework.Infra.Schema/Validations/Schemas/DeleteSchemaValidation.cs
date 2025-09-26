using Simjob.Framework.Infra.Schemas.Commands;

namespace Simjob.Framework.Infra.Schemas.Validations.Schemas
{
    public class DeleteSchemaValidation : SchemaValidation<DeleteSchemaCommand>
    {
        public DeleteSchemaValidation()
        {
            IdValidation();
        }
    }
}
