namespace Simjob.Framework.Services.Api.Models
{
    public class EmailConfigUI
    {
        public string PrimaryDomain { get; set; }
        public int PrimaryPort { get; set; }
        public string UsernameEmail { get; set; }
        public string UsernamePassword { get; set; }
        public string FromEmail { get; set; }
        public string CcEmail { get; set; }
        public string ssl { get; set; }
        public string UseDefaultCredentials { get; set; }
        public string FromUser { get; set; }
    }
}