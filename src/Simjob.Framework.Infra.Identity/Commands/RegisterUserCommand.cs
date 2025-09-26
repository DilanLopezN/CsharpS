
using Simjob.Framework.Domain.Core.Commands;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Simjob.Framework.Infra.Identity.Commands
{

    [ExcludeFromCodeCoverage]
    public class RegisterUserCommand : Command
    {
        public string Tenanty { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Telefone { get; set; }
        public string GrupoId { get; set; }
        public string CompanySiteIdDefault { get; set; }
        public string[] CompanySiteIds { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public bool Root { get; set; }
        public bool ControlAccess { get; set; } = false;
        [JsonIgnore]
        public string CreateBy { get; set; }

        public string cd_pessoa { get; set; }
        public string? cd_usuario { get; set; }
        public override bool IsValid()
        {
            //ValidationResult = new RegisterUserValidation().Validate(this);
            return base.IsValid();
        }
    }
}
