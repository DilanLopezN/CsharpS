using Simjob.Framework.Domain.Core.Commands;
using Simjob.Framework.Infra.Identity.Entities;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Simjob.Framework.Infra.Identity.Commands
{
    [ExcludeFromCodeCoverage]
    public class DeletePermissionCommand : Command
    {
        // A principio será utilizado para atualizar apenas um schema por vez, alterar para list
        // quando for utilizar to many.

        public string SchemaID { get; set; }
        public string ActionID { get; set; }
        public string UserID { get; set; }
        public List<string> Permissions { get; set; } // 

        public override bool IsValid()
        {
            //ValidationResult = new RegisterUserValidation().Validate(this);
            return base.IsValid();
        }
    }
}
