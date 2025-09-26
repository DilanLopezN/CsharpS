

using FluentValidation;
using Simjob.Framework.Infra.Schemas.Commands.Entities;
using System.Diagnostics.CodeAnalysis;

namespace Simjob.Framework.Infra.Schemas.Validations.Entities
{
    [ExcludeFromCodeCoverage]
    public class DeleteEntityValidation : AbstractValidator<DeleteEntityCommand>
    {
        public DeleteEntityValidation()
        {
            IdValidation();
        }

        protected void IdValidation()
        {
            RuleFor(e => e.Id).NotNull().NotEmpty();
        }

        protected void SchemaNameValidation()
        {
            RuleFor(e => e.SchemaName).NotNull().NotEmpty();
        }
    }
}
