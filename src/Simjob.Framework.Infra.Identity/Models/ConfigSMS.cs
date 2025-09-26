using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Models
{
    public class ConfigSMS
    {
        public string AccountSid { get; set; }
        public string AuthToken { get; set; }
        public string TwilioPhoneNumber { get; set; }
    }
}
