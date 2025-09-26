using MediatR;
using MongoDB.Driver;
using MoreLinq;
using MoreLinq.Extensions;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Domain.Util;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Infra.Schemas.Entities;
using Simjob.Framework.Infra.Schemas.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Services
{
    public class PermissionService : IPermissionService
    {
        protected readonly PermissionContext Context;
        protected readonly IMongoCollection<Permission> Collection;
        private readonly DomainNotificationHandler _notifications;
        private readonly IMediatorHandler _bus;
        private readonly IRepository<PermissionContext, Permission> _permissionRepository;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;
        private readonly ISchemaBuilder _schemaBuilder;
        private readonly ISegmentationService _segmentationService;
        private static readonly Encoding rawEncoder = Encoding.UTF8;


        public PermissionService(PermissionContext context, INotificationHandler<DomainNotification> notifications, IMediatorHandler bus,
            IRepository<PermissionContext, Permission> PermissionRepository, IRepository<MongoDbContext, Schema> schemaRepository, ISchemaBuilder schemaBuilder, ISegmentationService segmentationService)
        {
            Context = context;
            _notifications = (DomainNotificationHandler)notifications;
            Collection = context.GetUserCollection();
            _bus = bus;
            _permissionRepository = PermissionRepository;
            _schemaRepository = schemaRepository;
            _schemaBuilder = schemaBuilder;
            _segmentationService = segmentationService;
        }

        public void Register(Permission permission)
        {
            bool isFirstPermission = !_permissionRepository.Exists(u => u.UserID == permission.UserID);
            bool PermissionExists = false;

            if (!isFirstPermission)
                PermissionExists = _permissionRepository.Exists(u => u.UserID == permission.UserID && u.UserID == permission.UserID);

            if (PermissionExists)
            {
                var permissionUser = GetPermissionById(permission.UserID);
                var filter = Builders<Permission>.Filter.Eq(e => e.UserID, permission.UserID);
                Collection.DeleteMany(filter);
                //_permissionRepository.Delete(permission.UserID);
            }

            _permissionRepository.Insert(permission);

        }

        public async Task RegisterAsync(List<Permission> permissions)
        {
            foreach (var permission in permissions)
            {
                bool isFirstPermission = !_permissionRepository.Exists(u => u.UserID == permission.UserID);
                bool PermissionExists = false;

                if (!isFirstPermission)
                    PermissionExists = _permissionRepository.Exists(u => u.UserID == permission.UserID && u.UserID == permission.UserID);

                if (PermissionExists)
                {
                    var permissionUser = GetPermissionById(permission.UserID);
                    var filter = Builders<Permission>.Filter.Eq(e => e.UserID, permission.UserID);
                    Collection.DeleteMany(filter);
                    //_permissionRepository.Delete(permission.UserID);
                }
            }
            await _permissionRepository.InsertManyAsync(permissions);

        }




        public Permission GetPermissionById(string id)
        {
            var filterById = Builders<Permission>.Filter.Eq(u => u.UserID, id);
            var filterIsDeleted = Builders<Permission>.Filter.Eq(u => u.IsDeleted, false);
            return Collection.Find(filterById & filterIsDeleted).FirstOrDefault();
        }

        [ExcludeFromCodeCoverage]
        public Permission GetPermissionByName(string PermissionName)
        {
            return Collection.Find(Builders<Permission>.Filter.Eq(u => u.UserID, PermissionName.ToLower()) &
                Builders<Permission>.Filter.Eq(u => u.UserID, PermissionName.ToLower())
            ).FirstOrDefault();
        }

        public PaginationData<Permission> GetPermissionByNamePaginada(List<User> users, int page, int limit)
        {
            if (page == 0) page = 1;
            if (limit == 0) limit = 30;

            var listBusca = new List<Permission>();

            if (users.Count > 0)
            {
                foreach (var user in users)
                {

                    var perm = Collection.Find(Builders<Permission>.Filter.Eq(u => u.UserID, user.Id)).FirstOrDefault();
                    //var perm = Collection.Find(Builders<Permission>.Filter.Regex(u => u.Name, new BsonRegularExpression($"/^.{user.Name}.$/i"))).FirstOrDefault();
                    if (perm != null) listBusca.Add(perm);
                }
            }

            var res = listBusca.Skip((page - 1) * limit).Take(limit);
            long count = Convert.ToInt64(listBusca.Count());
            return new PaginationData<Permission>(res.ToList(), page, limit, count);
        }


        public void UpdatePermission(Permission perm, string schemaId, List<string> permissions, List<ActionsGroup> actions, List<SchemasGroup.Segmentation> segmentations)
        {
            var filter = Builders<Permission>.Filter.Where(x => x.UserID == perm.UserID && x.Schemas.Any(i => i.SchemaID == schemaId));
            var update = Builders<Permission>.Update.Set(x => x.Schemas[-1].Permissions, permissions)
                                                    .Set(x => x.Schemas[-1].Actions, actions)
                                                    .Set(x => x.Schemas[-1].Segmentations, segmentations);

            var result = Collection.UpdateOneAsync(filter, update).Result;
        }

        public void UpdateDadosUser(Permission per, string nome, string email, string telefone)
        {
            per.Name = nome ?? per.Name;
            per.Email = email ?? per.Email;


            _permissionRepository.Update(per);


        }

        public void DeletePermissions(Permission permission)
        {
            _permissionRepository.Delete(permission.UserID);
        }

        public List<Permission> GetPermissions()
        {
            return Collection.Find(Builders<Permission>.Filter.Eq(u => u.IsDeleted, false)).ToList();
        }

        // Insere múltiplas permissões baseado no groupId
        public void InsertPermissionsInGroup(List<User> users, Permission permission)
        {
            List<Permission> permissionsToAdd = new List<Permission>();
            foreach (var user in users)
            {
                // deleta
                var PermissionExists = _permissionRepository.Exists(u => u.UserID == user.Id);
                if (PermissionExists)
                {
                    var filter = Builders<Permission>.Filter.Eq(e => e.UserID, user.Id);

                    Collection.DeleteMany(filter);
                }

                foreach (var schema in permission.Schemas)
                {
                    if (schema.Segmentations != null)
                    {
                        foreach (var seg in schema.Segmentations)
                        {
                            var newSeg = new Segmentation(schema.SchemaName, seg.field, seg.values.ToArray(), user.Id);

                            var getSegmentation = _segmentationService.GetSegmentationByFields(user.Id, schema.SchemaName, seg.field);
                            if (getSegmentation == null)
                            {

                                _segmentationService.Register(newSeg);
                            }
                            else
                            {
                                _segmentationService.UpdateSegmentation(newSeg, getSegmentation.Id);

                            }


                        }
                    }


                }
                //insere
                var permissions = new Permission(permission.Schemas, user.Id, user.Name, user.UserName);

                permissionsToAdd.Add(permissions);
            }

            _permissionRepository.InsertMany(permissionsToAdd);
        }

        public List<Permission> GetPermissionsByGroup(List<User> users)
        {
            List<Permission> listPermissionsByUser = new List<Permission>();
            foreach (var user in users)
            {
                var permissions = GetPermissionById(user.Id);


                var newPermission = new Permission(null, user.Id, user.Name, user.UserName);

                if (permissions != null)
                {
                    listPermissionsByUser.Add(permissions);
                }
                else
                {
                    listPermissionsByUser.Add(newPermission);
                }
            }

            return listPermissionsByUser;

        }

        public Permission RetornPermission(string accessToken)
        {
            var userId = "";
            if (accessToken != "")
            {
                var token = accessToken.ToString().Split(" ");
                var onlyToken = "";
                foreach (var itens in token) { if (itens.Length > 10) onlyToken = itens; }
                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(onlyToken);
                var claims = jwtSecurityToken.Claims.ToList();

                foreach (var claim in claims)
                {
                    if (claim.Type == "userid")
                    {
                        userId = claim.Value;
                        break;
                    }
                }
            }

            return this.GetPermissionById(userId);

        }

        public PaginationData<Permission> GetPermissionsByGroupPaginada(List<User> users, int page, int limit)
        {
            if (page == 0) page = 1;
            if (limit == 0) limit = 30;
            List<Permission> listPermissionsByUser = new List<Permission>();
            foreach (var user in users)
            {
                var permissions = GetPermissionById(user.Id);
                var newPermission = new Permission(null, user.Id, user.Name, user.UserName);

                if (permissions != null)
                {
                    listPermissionsByUser.Add(permissions);
                }
                else
                {
                    listPermissionsByUser.Add(newPermission);
                }
            }
            var res = listPermissionsByUser.Skip((page - 1) * limit).Take(limit);
            long count = Convert.ToInt64(listPermissionsByUser.Count());
            return new PaginationData<Permission>(res.ToList(), page, limit, count);
        }

        public Permission GetPermissionByIdExternal(string id, string tenanty)
        {
            var coll = Context.GetDatabaseByTenanty(tenanty).GetCollection<Permission>("permission");
            var filterById = Builders<Permission>.Filter.Eq(u => u.UserID, id);
            var filterIsDeleted = Builders<Permission>.Filter.Eq(u => u.IsDeleted, false);
            return coll.Find(filterById & filterIsDeleted).FirstOrDefault();
        }


    }
}

