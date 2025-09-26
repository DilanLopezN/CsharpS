using Simjob.Framework.Domain.Core.Commands;

namespace Simjob.Framework.Infra.Identity.Commands
{
    public class RegisterNotificationCommand : Command
    {
        public string Msg { get; set; }
        public string Obs { get; set; }

        public string UserId { get; set; }
    }

}

