using System.Collections.Generic;

namespace Simjob.Framework.Infra.Schemas.Models
{
    public class ModulePermissionModel
    {
        public ModulePermissionModel()
        {
            Groups = new List<ModulePermissionModel>();
            Permissions = new List<string>();
        }

        public string Icon { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public IEnumerable<ModulePermissionModel> Groups { get; set; }
        public IEnumerable<string> Permissions { get; set; }
    }
}