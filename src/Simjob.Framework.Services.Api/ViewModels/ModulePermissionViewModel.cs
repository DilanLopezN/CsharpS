using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Schemas.Models;
using System.Collections.Generic;

namespace Simjob.Framework.Services.Api.ViewModels
{
    public class ModulePermissionViewModel
    {
        public string Icon { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public IEnumerable<ModulePermissionModel> Groups { get; set; }
        public IEnumerable<string> Permissions { get; set; }
        public List<Entities.Action> Actions { get; set; }
    }
}