

using Simjob.Framework.Domain.Core.Commands;
using System.Diagnostics.CodeAnalysis;

namespace Simjob.Framework.Infra.Identity.Commands
{
    [ExcludeFromCodeCoverage]
    public class TwoFactorAuthCommand : Command
    {
        public string UserId { get; set; }
        public string Tenanty { get; set; }
        public string Email { get; set; }
        public string PublicIP { get; set; }
        public string Hash { get; set; }
        public string Code { get; set; }
        public int a2f { get; set; }

        public override bool IsValid()
        {
            //ValidationResult = new SignInUserValidation().Validate(this);
            return base.IsValid();
        }
    }
}
