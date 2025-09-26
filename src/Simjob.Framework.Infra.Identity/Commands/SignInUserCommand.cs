

using Simjob.Framework.Domain.Core.Commands;
using System.Diagnostics.CodeAnalysis;

namespace Simjob.Framework.Infra.Identity.Commands
{
    [ExcludeFromCodeCoverage]
    public class SignInUserCommand : Command
    {
        public string Tenanty { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }


        public override bool IsValid()
        {
            //ValidationResult = new SignInUserValidation().Validate(this);
            return base.IsValid();
        }
    }
}
