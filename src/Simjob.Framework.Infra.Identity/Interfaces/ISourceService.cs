using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Models.Module;
using Simjob.Framework.Infra.Identity.Models.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Interfaces
{
   public interface ISourceService
    {
        public Source? Register(CreateSourceModel model);
        public Source? Update(UpdateSourceModel model);
        public Source GetById(string id);
        public void Disable(string id);
        public void Enable(string id);
    }
}
