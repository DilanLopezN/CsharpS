using Simjob.Framework.Domain.Core.Entities;
using System.Collections.Generic;

namespace Simjob.Framework.Infra.Identity.Entities
{
    public class SchemasGroup
    {
        public SchemasGroup(string schemaname, string schema, List<string> permission, List<ActionsGroup>? actions)
        {
            SchemaName = schemaname;
            Permissions = permission;
            SchemaID = schema;
            Actions = actions;
        }
        public SchemasGroup()
        {
        }
        public string SchemaName { get; set; }
        public List<string> Permissions { get; set; }
        public string SchemaID { get; set; }
        public List<ActionsGroup>? Actions { get; set; }
        public List<Segmentation> Segmentations { get; set; }

        public class Segmentation
        {
            public string field { get; set; }
            public List<string> values { get; set; }
        }
    }
}