using Simjob.Framework.Domain.Core.Commands;
using Simjob.Framework.Infra.Identity.Entities;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Simjob.Framework.Infra.Identity.Commands.AccessGroup
{
    [ExcludeFromCodeCoverage]
    public class RegisterGroupPermissionCommand : Command
    {
        public string GroupName { get; set; }

        public List<SchemasGroup> Schema { get; set; }

        public override bool IsValid()
        {
            //ValidationResult = new RegisterUserValidation().Validate(this);
            return base.IsValid();
        }
    }
}
