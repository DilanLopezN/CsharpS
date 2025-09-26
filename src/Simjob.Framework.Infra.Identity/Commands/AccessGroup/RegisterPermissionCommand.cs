using Simjob.Framework.Domain.Core.Commands;
using Simjob.Framework.Infra.Identity.Entities;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Simjob.Framework.Infra.Identity.Commands
{
    [ExcludeFromCodeCoverage]
    public class RegisterPermissionCommand : Command
    {
        public string UserID { get; set; }
        public string GroupId { get; set; }
        public List<SchemasGroup> Schema { get; set; }
        
        public override bool IsValid()
        {
            //ValidationResult = new RegisterUserValidation().Validate(this);
            return base.IsValid();
        }
  
    }
}
