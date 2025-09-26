using Simjob.Framework.Domain.Core.Commands;

namespace Simjob.Framework.Infra.Identity.Commands
{
    public class UpdateNotificationCommand : Command
    {
        public string NotificationId { get; set; }
        public string SchemaRecordId { get; set; }
        public string Justificativa { get; set; }
        public bool Aprov { get; set; }
        public override bool IsValid()
        {
            //ValidationResult = new SignInUserValidation().Validate(this);
            return base.IsValid();
        }
    }
}
