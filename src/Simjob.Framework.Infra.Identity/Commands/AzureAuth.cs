using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Commands
{
    public class AzureAuth
    {
        public string Tenanty { get; set; }
        public string Email { get; set; }
        public string TokenAzure { get; set; }
    }
}
