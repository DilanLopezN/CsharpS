using Simjob.Framework.Domain.Core.Entities;
using System.Collections.Generic;

namespace Simjob.Framework.Infra.Identity.Entities
{
    public class Group : Entity
    {
        public Group(string groupName, List<SchemasGroup> schema, int? cd_pessoa = null)
        {
            GroupName = groupName;
            Schema = schema;
            Cd_empresa = cd_pessoa;
        }

        public string GroupName { get; set; }
        public int? Cd_empresa { get; set; }
        public int TotalUsersInGroup { get; set; }
        public List<SchemasGroup> Schema { get; set; }
    }
}
