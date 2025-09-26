using Simjob.Framework.Infra.Schemas.Commands.Entities;

namespace Simjob.Framework.Infra.Schemas.Validations.Entities
{
    public class InsertEntityValidation : EntityValidation<InsertEntityCommand>
    {
        public InsertEntityValidation()
        {
            SchemaJsonValidation();
            SchemaNameValidation();
            DataValidation();
        }
    }
}
