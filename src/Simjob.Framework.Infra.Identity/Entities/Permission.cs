using Simjob.Framework.Domain.Core.Entities;
using System.Collections.Generic;

namespace Simjob.Framework.Infra.Identity.Entities
{
    public class Permission : Entity
    {
        public Permission(List<SchemasGroup> schema, string userId, string name, string email)
        {
            UserID = userId;
            Name = name;
            Email = email;            
            Schemas = schema;
        }


        public string UserID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }        
        public List<SchemasGroup> Schemas { get; set; }
    }
}
