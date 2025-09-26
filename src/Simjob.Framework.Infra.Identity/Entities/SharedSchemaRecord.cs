using Simjob.Framework.Domain.Core.Entities;
using System.Collections.Generic;

namespace Simjob.Framework.Infra.Identity.Entities
{
    public class SharedSchemaRecord : Entity
    {
        public SharedSchemaRecord(string schemaRecordId, string schemaName, string userIdSender, string userIdReceive, string userNameSender, string userNameReceive, List<string> permissions)
        {
            SchemaRecordId = schemaRecordId;
            SchemaName = schemaName;
            UserIdSender = userIdSender;
            UserIdReceive = userIdReceive;
            UserNameSender = userNameSender;
            UserNameReceive = userNameReceive;
            Permissions = permissions;
        }

        public string SchemaRecordId { get; set; }
        public string SchemaName { get; set; }
        public string UserIdSender { get; set; }
        public string UserIdReceive { get; set; }
        public string UserNameSender { get; set; }
        public string UserNameReceive { get; set; }
        public List<string> Permissions { get; set; }


    }
}
