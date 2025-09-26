using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Schemas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Models
{
    public class ModulePermissionActionModel
    {
        public string Icon { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public IEnumerable<ModulePermissionModel> Groups { get; set; }
        public IEnumerable<string> Permissions { get; set; }
        public List<ActionsGroup> Actions { get; set; } = new List<ActionsGroup>();
    }
}