using Simjob.Framework.Domain.Core.Commands;
using System.Collections.Generic;

namespace Simjob.Framework.Infra.Identity.Commands
{
    public class SharedSchemaRecordCommand : Command
    {
        public string SchemaRecordId { get; set; }
        public string SchemaName { get; set; }
        public string UserIdSender { get; set; }
        public string UserIdReceive { get; set; }
        public string UserNameSender { get; set; }
        public string UserNameReceive { get; set; }
        public List<string> Permissions { get; set; }
    }
}
