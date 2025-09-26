using Simjob.Framework.Domain.Core.Commands;

namespace Simjob.Framework.Infra.Identity.Commands.AccessGroup
{
    public class SendEmailCommand : Command
    {
        //public string NameFrom { get; set; }
        //public string From { get; set; }
        public string Subject { get; set; }
        public string NameTo { get; set; } = null;
        public string To { get; set; }
        public string PlainTextContent { get; set; }
        public string HtmlContent { get; set; }

    }
}
