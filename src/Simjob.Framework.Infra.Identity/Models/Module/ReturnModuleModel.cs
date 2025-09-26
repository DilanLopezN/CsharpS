using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Models.Module
{
  public  class ReturnModuleModel
    {
        public string SchemaName { get; set; }
        public string Description { get; set; }
        public string Module { get; set; }
        public string? Redirect { get; set; }
        public string Icon { get; set; }
        public string Path { get; set; }
    }
}
