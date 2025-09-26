using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;
using MoreLinq;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Domain.Util;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Commands.AccessGroup;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Infra.Schemas.Entities;
using Simjob.Framework.Infra.Schemas.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Services
{
    public class GroupService : IGroupService
    {
        protected readonly GroupContext Context;
        protected readonly IMongoCollection<Group> Collection;
        private readonly DomainNotificationHandler _notifications;
        private readonly IMediatorHandler _bus;
        private readonly IRepository<GroupContext, Group> _groupRepository;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;
        private readonly ISchemaBuilder _schemaBuilder;
        private readonly IUserService _userService;
        private readonly IPermissionService _permissionService;
        private static readonly Encoding rawEncoder = Encoding.UTF8;

        public GroupService(GroupContext context, INotificationHandler<DomainNotification> notifications, IMediatorHandler bus,
            IRepository<GroupContext, Group> groupRepository, IRepository<MongoDbContext, Schema> schemaRepository, ISchemaBuilder schemaBuilder, IUserService userService, IPermissionService permissionService)
        {
            Context = context;
            _notifications = (DomainNotificationHandler)notifications;
            Collection = context.GetUserCollection();
            _bus = bus;
            _groupRepository = groupRepository;
            _schemaRepository = schemaRepository;
            _schemaBuilder = schemaBuilder;
            _userService = userService;
            _permissionService = permissionService;
            _permissionService = permissionService;
        }

        public void Register(Group Group, string tenanty)
        {
            bool isFirstGroup = !_groupRepository.Exists(u => u.GroupName == Group.GroupName);
            bool GroupExists = false;

            if (!isFirstGroup)
                GroupExists = _groupRepository.Exists(u => u.GroupName == Group.GroupName && u.Cd_empresa == Group.Cd_empresa);

            if (GroupExists)
            {
                _bus.RaiseEvent(new DomainNotification("GroupService", "Groupname already exists"));
                return;
            }

            var newGroup = _groupRepository.Insert(Group);

        }

        public Group GetGroupById(string id)
        {
            var filterId = Builders<Group>.Filter.Eq(u => u.Id, id);
            var filterIsDeleted = Builders<Group>.Filter.Eq(u => u.IsDeleted, false);
            return Collection.Find(filterId & filterIsDeleted).FirstOrDefault();
        }

        public Group GetGroupByName(string groupName)
        {
            var filterRegexLike = Builders<Group>.Filter.Regex(u => u.GroupName, new BsonRegularExpression($"/^.*{groupName}.*$/i"));
            var filterIsDeleted = Builders<Group>.Filter.Eq(u => u.IsDeleted, false);
            return Collection.Find(filterRegexLike & filterIsDeleted).FirstOrDefault();
        }

        public Group GetGroupByNameEQ(string name)
        {
            var filterEq = Builders<Group>.Filter.Eq(u => u.GroupName, name);
            var filterIsDeleted = Builders<Group>.Filter.Eq(u => u.IsDeleted, false);
            return Collection.Find(filterEq & filterIsDeleted).FirstOrDefault();
        }

        public PaginationData<Group> GetGroupByNamePaginada(string groupName, int? page = null, int? limit = null, int? companySiteId = null)
        {
            if (page == null) page = 1;
            if (limit == null) limit = 10;
            var filterIsDeleted = Builders<Group>.Filter.Eq(u => u.IsDeleted, false);
            var filterEq = Builders<Group>.Filter.Eq(u => u.Cd_empresa, companySiteId);
            var filterRegex = Builders<Group>.Filter.Regex(u => u.GroupName, new BsonRegularExpression($"/^.*{groupName}.*$/i"));


            var listBusca = Collection.Find(filterIsDeleted & filterRegex & filterEq).ToList();
            var res = listBusca.Skip((int)((page - 1) * limit)).Take((int)limit);

            foreach (var group in res)
            {
                group.TotalUsersInGroup = _userService.GetUsersByGroupId(group.Id).Count();
            }

            long count = Convert.ToInt64(listBusca.Count());
            return new PaginationData<Group>(res.ToList(), page, limit, count);
        }


        public PaginationData<GetGroupsCommand> GetGroupsPaginada(string? groupName = null, int? page = null, int? limit = null, int? companySiteId = null, string sortField = null, bool sortDesc = false)
        {
            if (page == null) page = 1;
            if (limit == null) limit = 10;

            var listFilter = new List<FilterDefinition<Group>>();
            if(groupName != null)
            {
                var filterRegex = Builders<Group>.Filter.Regex(u => u.GroupName, new BsonRegularExpression($"/^.*{groupName}.*$/i"));
                listFilter.Add(filterRegex);
            }
            if(companySiteId != null)
            {
                var filterEq = Builders<Group>.Filter.Eq(u => u.Cd_empresa, companySiteId);
                listFilter.Add(filterEq);
            }
            var filterIsDeleted = Builders<Group>.Filter.Eq(u => u.IsDeleted, false);
            listFilter.Add(filterIsDeleted);

            var combinedFilter = Builders<Group>.Filter.And(listFilter);

            var listBusca = Collection.Find(combinedFilter).ToList();



            if (sortField != null) {
                var prop = typeof(Group).GetProperties().FirstOrDefault(x => x.Name.ToLower() == sortField.ToLower());
                if (!sortDesc)
                {
                    listBusca = listBusca.OrderBy(x => prop == null ? "Id" : prop.GetValue(x, null)).ToList();
                }
                else
                {
                    listBusca = listBusca.OrderByDescending(x => prop == null ? "Id" : prop.GetValue(x, null)).ToList();
                }

            }

            var res = listBusca.Skip((int)((page - 1) * limit)).Take((int)limit);
            List<GetGroupsCommand> groupsList = new List<GetGroupsCommand>();
            foreach (var group in res)
            {
                group.TotalUsersInGroup = _userService.GetUsersByGroupId(group.Id).Count();

                groupsList.Add(new GetGroupsCommand() { Id = group.Id, GroupName = group.GroupName, TotalUsersInGroup = group.TotalUsersInGroup,Cd_empresa = group.Cd_empresa });
            }

            long count = Convert.ToInt64(listBusca.Count());
            return new PaginationData<GetGroupsCommand>(groupsList, page, limit, count);
        }


        public void UpdateGroupName(Group Group, string newGroupName)
        {
            var exists = GetGroupByNameEQ(newGroupName) != null;

            if (exists)
            {
                _bus.RaiseEvent(new DomainNotification("GroupService", "Groupname already exists"));
                return;
            }

            var GroupIdFilter = Builders<Group>.Filter.Eq(u => u.Id, Group.Id);
            var GroupGroupNameUpdate = Builders<Group>.Update.Set(u => u.GroupName, newGroupName);
            Collection.UpdateOne(GroupIdFilter, GroupGroupNameUpdate);
        }

        public void DeleteGroup(Group group)
        {
            _groupRepository.Delete(group.Id);
        }

        [ExcludeFromCodeCoverage]
        public List<Group> GetGroups()
        {
            var groups = Collection.Find(Builders<Group>.Filter.Eq(u => u.IsDeleted, false)).ToList();

            foreach (var group in groups)
            {
                group.TotalUsersInGroup = _userService.GetUsersByGroupId(group.Id).Count();


            }

            return groups;
        }

        public async Task UpdateGroupPermission(Group group, RegisterGroupPermissionCommand command, string tenanty)
        {
            var groupExists = GetGroupByNameEQ(command.GroupName);

            if (groupExists != null && groupExists.Id != group.Id && groupExists.Cd_empresa == group.Cd_empresa)
            {
                await _bus.RaiseEvent(new DomainNotification("GroupService", "Groupname already exists"));
                return;
            }

            var GroupIdFilter = Builders<Group>.Filter.Eq(u => u.Id, group.Id);
            var GroupUpdate = Builders<Group>.Update.Set(u => u.GroupName, command.GroupName)
                                                    .Set(u => u.Schema, command.Schema);
            await Collection.UpdateOneAsync(GroupIdFilter, GroupUpdate);


            var newGroup = GetGroupById(group.Id);

            if (newGroup != null)
            {
                var users = _userService.GetUsersByTenantyAndGroupID(tenanty, newGroup.Id);
                if (users.Count > 0)
                {
                    List<Permission> permissions = new List<Permission>();
                    foreach (var user in users)
                    {
                        var newPermission = new Permission(newGroup.Schema, user.Id, user.Name, user.UserName);
                        permissions.Add(newPermission);
                    }
                    await _permissionService.RegisterAsync(permissions);
                }

            }
        }
    }
}

