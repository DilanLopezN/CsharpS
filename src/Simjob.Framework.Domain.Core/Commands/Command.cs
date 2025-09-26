using FluentValidation.Results;
using Simjob.Framework.Domain.Core.Events;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Simjob.Framework.Domain.Core.Commands
{
    public abstract class Command : Message
    {
        protected ValidationResult ValidationResult { get; set; }
        [ExcludeFromCodeCoverage]
        public List<ValidationFailure> GetValidationResultErrors()
        {
            return ValidationResult?.Errors;
        }

        public virtual bool IsValid() => ValidationResult == null ? true : ValidationResult.IsValid;
    }
}
