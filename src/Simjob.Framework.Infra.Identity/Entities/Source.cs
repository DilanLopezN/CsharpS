using Simjob.Framework.Domain.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Entities
{
    public class Source : Entity
    {
        public string Description { get; set; }
        public string Host { get; set; }    
        public string User { get; set; }                        
        public string Password { get; set; }                    
        public int Port { get; set; }                             
        public string DbName { get; set; }  
        public string Dialect { get; set; }
        public bool? Active { get; set; }
    }
}
