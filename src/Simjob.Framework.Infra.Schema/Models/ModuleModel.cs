using System.Collections.Generic;

namespace Simjob.Framework.Infra.Schemas.Models
{
    public class ModuleModel
    {
        public ModuleModel()
        {
            Groups = new List<ModuleModel>();
        }

        public string Icon { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public IEnumerable<ModuleModel> Groups { get; set; }
    }
}
