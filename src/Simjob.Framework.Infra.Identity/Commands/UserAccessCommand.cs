using MongoDB.Driver;
using Simjob.Framework.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Commands
{
    public class UserAccessCommand
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Ip { get; set; }
        public string UserAgent { get; set; }
        public string SchemaName { get; set; }
        public string Description { get; set; }
        public string SchemaRecordId { get; set; }
    //    public override bool IsValid()
    //    {
            //ValidationResult = new SignInUserValidation().Validate(this);
      //      return base.IsValid();
       // }
    }
}
