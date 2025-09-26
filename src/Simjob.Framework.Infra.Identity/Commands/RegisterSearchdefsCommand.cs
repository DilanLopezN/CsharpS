using Simjob.Framework.Domain.Core.Commands;
using Simjob.Framework.Infra.Identity.Entities;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static Simjob.Framework.Domain.Models.Searchdefs;

namespace Simjob.Framework.Infra.Identity.Commands
{
    [ExcludeFromCodeCoverage]
    public class RegisterSearchdefsCommand :Command
    {
        public string SchemaName { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public Def Def { get; set; }



        public override bool IsValid()
        {
            //ValidationResult = new RegisterUserValidation().Validate(this);
            return base.IsValid();
        }
    }
}
