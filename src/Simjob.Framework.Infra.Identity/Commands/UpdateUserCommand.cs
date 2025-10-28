using Simjob.Framework.Domain.Core.Commands;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Simjob.Framework.Infra.Identity.Commands
{
    [ExcludeFromCodeCoverage]
    public class UpdateUserCommand : Command
    {
        public string UserId { get; set; }
        public string Tenanty { get; set; }
        public string Name { get; set; }
        public string Telefone { get; set; }
        public string CompanySiteIdDefault { get; set; }
        public string[] CompanySiteIds { get; set; }
        public string UserName { get; set; }
        public string GroupId { get; set; }
        public bool Root { get; set; }
        public bool ControlAccess { get; set; } = false;
        public bool LogonAzure { get; set; }
        [JsonIgnore]
        public string UpdateBy { get; set; }
        public string cd_pessoa { get; set; }
        public override bool IsValid()
        {
            //ValidationResult = new SignInUserValidation().Validate(this);
            return base.IsValid();
        }
    }
}
