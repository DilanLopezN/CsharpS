using Simjob.Framework.Domain.Core.Commands;
using Simjob.Framework.Infra.Identity.Entities;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Simjob.Framework.Infra.Identity.Commands
{
    [ExcludeFromCodeCoverage]
    public class RegisterProfileCommand : Command
    {
        public string NomeEmpresa { get; set; }
        public string Color { get; set; }     

        public override bool IsValid()
        {
            //ValidationResult = new RegisterUserValidation().Validate(this);
            return base.IsValid();
        }
    }
}
