

using Simjob.Framework.Infra.Schemas.Commands.Entities;

namespace Simjob.Framework.Infra.Schemas.Validations.Entities
{
    public class UpdateEntityValidation : EntityValidation<UpdateEntityCommand>
    {
        public UpdateEntityValidation()
        {
            IdValidation();
            SchemaJsonValidation();
            SchemaNameValidation();
            DataValidation();
        }
    }
}
