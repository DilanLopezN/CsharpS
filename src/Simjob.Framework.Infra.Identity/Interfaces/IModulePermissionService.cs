using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Models.Module;
using Simjob.Framework.Infra.Identity.Models.ModulePermission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Interfaces
{
    public interface IModulePermissionService  
    {
        public void RegisterRecursive(CreateModulePermissionModel model, List<ModulePermission> modulePermissionsDatabaseList, List<Entities.Module> modulesDatabaseList, List<Entities.ModulePermission> listCreateModulesPermission, List<Entities.ModulePermission> listUpdateModulesPermission);

        public List<CreateModulePermissionModel> InsertMany(List<CreateModulePermissionModel> models);
        public List<ModulePermission> InsertManyGroup(List<CreateModulePermissionGroupModel> models);
        public ModulePermission GetById(string id);
        public object GetAll(string groupId, string userId,int? page, int? limit, string sortField = null, bool sortDesc = false);
        public void Delete(string id);

    }
}
