using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Models.Module;
using Simjob.Framework.Infra.Identity.Models.ModuleIdentity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Interfaces
{
    public interface IModuleIdentityService
    {
        public void Register(CreateModuleIdentityModel model);
        public void Update(UpdateModuleIdentityModel model);
        public ModuleIdentity GetById(string id);
        public void Disable(string id);
        public void Enable(string id);
    }
}
