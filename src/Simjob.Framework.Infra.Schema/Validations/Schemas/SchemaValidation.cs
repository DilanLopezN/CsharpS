using FluentValidation;
using Simjob.Framework.Infra.Schemas.Commands;

namespace Simjob.Framework.Infra.Schemas.Validations.Schemas
{
    public class SchemaValidation<T> : AbstractValidator<T> where T : SchemaCommand
    {
        public SchemaValidation()
        {
        }

        protected void IdValidation()
        {
            RuleFor(s => s.Id).NotNull().NotEmpty();
        }

        protected void NameValidation()
        {
            RuleFor(s => s.Name).NotNull().NotEmpty();
        }

        protected void JsonValueValidation()
        {
            RuleFor(s => s.JsonValue).NotNull().NotEmpty();
        }
    }
}
