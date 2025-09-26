using Simjob.Framework.Domain.Core.Commands;
using System.Diagnostics.CodeAnalysis;

namespace Simjob.Framework.Infra.Identity.Commands
{
    [ExcludeFromCodeCoverage]
    public class RegisterGroupCommand : Command
    {
        public string GroupName { get; set; }
        public int Cd_empresa { get; set; }

        public override bool IsValid()
        {
            //ValidationResult = new RegisterUserValidation().Validate(this);
            return base.IsValid();
        }
    }
}
