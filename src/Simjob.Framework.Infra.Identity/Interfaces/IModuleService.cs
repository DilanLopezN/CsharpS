using Simjob.Framework.Domain.Util;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Models.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Interfaces
{
    public interface IModuleService
    {
        public Entities.Module? Register(CreateModuleModel model);
        public Entities.Module? Update(UpdateModuleModel model);
        public Module GetById(string id);
        public void Disable(string id);
        public void Enable(string id);
    }
}
