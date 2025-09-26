using Simjob.Framework.Domain.Core.Entities;

namespace Simjob.Framework.Infra.Identity.Entities
{
    public class StatusFlow : Entity
    {
        public StatusFlow(string schemaName, string tenanty, string field, ListProperties[] properties)
        {
            SchemaName = schemaName;
            Tenanty = tenanty;
            Field = field;
            Properties = properties;

        }
        public string SchemaName { get; set; }
        public string Tenanty { get; set; }
        public string Field { get; set; }

        public ListProperties[] Properties { get; set; }


        public class ListProperties
        {
            public string Status { get; set; }
            public string Action { get; set; }
            public string Type { get; set; }
            public string ProximoStatus { get; set; }
            public string ReprovaStatus { get; set; }
            public string[] AprovEmail { get; set; }
            public int AprovMin { get; set; }
        }
    }
}
