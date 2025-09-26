using FluentValidation;
using Simjob.Framework.Infra.Schemas.Commands.Entities;

namespace Simjob.Framework.Infra.Schemas.Validations.Entities
{
    public class EntityValidation<T> : AbstractValidator<T> where T : EntityCommand
    {
        public EntityValidation()
        {

        }

        public void IdValidation()
        {
            RuleFor(e => e.Id).NotNull().NotEmpty();
        }

        public void DataValidation()
        {
            RuleFor(e => e.Data).NotNull();
        }
        public void SchemaJsonValidation()
        {
            RuleFor(e => e.SchemaJson).NotNull().NotEmpty();
        }
        public void SchemaNameValidation()
        {
            RuleFor(e => e.SchemaName).NotNull().NotEmpty();
        }



    }
}
