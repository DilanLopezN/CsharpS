using Simjob.Framework.Domain.Util;
using Simjob.Framework.Infra.Identity.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Interfaces
{
    public interface IPermissionService
    {
        public void Register(Permission group);
        Task RegisterAsync(List<Permission> permissions);
        public Permission GetPermissionById(string id);
        public Permission GetPermissionByIdExternal(string id, string tenanty);
        public Permission GetPermissionByName(string name);
        public void UpdatePermission(Permission obj, string schemaId, List<string> permissions, List<ActionsGroup> actions, List<SchemasGroup.Segmentation> segmentations);
        public void DeletePermissions(Permission obj);
        public void InsertPermissionsInGroup(List<User> users, Permission permissions);
        List<Permission> GetPermissions();
        List<Permission> GetPermissionsByGroup(List<User> users);
        public Permission RetornPermission(string acessToken);
        public void UpdateDadosUser(Permission obj, string name, string email, string telefone); //usando?
        PaginationData<Permission> GetPermissionsByGroupPaginada(List<User> users, int page, int limit);
        public PaginationData<Permission> GetPermissionByNamePaginada(List<User> users, int page, int limit);
    }
}
