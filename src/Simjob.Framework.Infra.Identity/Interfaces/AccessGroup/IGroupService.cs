using Simjob.Framework.Domain.Util;
using Simjob.Framework.Infra.Identity.Commands.AccessGroup;
using Simjob.Framework.Infra.Identity.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Interfaces
{
    public interface IGroupService
    {
        public void Register(Group group, string tenanty);
        public Task UpdateGroupPermission(Group group, RegisterGroupPermissionCommand command, string tenanty);
        public Group GetGroupById(string id);
        public Group GetGroupByName(string name);
        public Group GetGroupByNameEQ(string name);
        public void UpdateGroupName(Group user, string newUserName);
        public void DeleteGroup(Group user);
        List<Group> GetGroups();


        public PaginationData<Group> GetGroupByNamePaginada(string groupName, int? page = null, int? limit = null, int? companySiteId = null);

        public PaginationData<GetGroupsCommand> GetGroupsPaginada(string? groupName = null, int? page = null, int? limit = null, int? companySiteId = null, string sortField = null, bool sortDesc = false);


    }
}
