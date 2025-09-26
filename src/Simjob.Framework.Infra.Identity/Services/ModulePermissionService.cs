using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MoreLinq;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Infra.Identity.Models.ModulePermission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static MongoDB.Driver.WriteConcern;

namespace Simjob.Framework.Infra.Identity.Services
{
    public class ModulePermissionService : IModulePermissionService
    {
        private readonly IRepository<ModulePermissionContext, Entities.ModulePermission> _modulePermissionRepository;
        private readonly IRepository<ModuleContext, Entities.Module> _moduleRepository;
        protected readonly IMongoCollection<ModulePermission> _collection;
        private readonly IUserService _userService;
        protected readonly ModulePermissionContext Context;
        private readonly IMediatorHandler _bus;

   

        public ModulePermissionService(IRepository<ModulePermissionContext, Entities.ModulePermission> modulePermissionRepository, IRepository<ModuleContext, Entities.Module> moduleRepository, IMediatorHandler bus, ModulePermissionContext context,IUserService userService)
        {
            _modulePermissionRepository = modulePermissionRepository;
            _moduleRepository = moduleRepository;
            Context = context;
            _collection = context.GetUserCollection();
            _bus = bus;
            _userService = userService;
        }
        //private bool ValidaModulePermission(string userId)
        //{
        //    var user = _userService.GetUserById(userId);
        //    if(user == null)
        //    {
        //        _bus.RaiseEvent(new DomainNotification("ModulePermissionService", "User Not Found"));
        //        return false;
        //    }

        //    return true;
        //}

        public List<CreateModulePermissionModel> InsertMany(List<CreateModulePermissionModel> models)
        {
            var moduleIds = models.Select(x => x.ModuleId).Distinct().ToList();
            var userIds = models.Select(x => x.UserId).Distinct().ToList();
            var modulesPermissionToInsert = new List<ModulePermission>();
            var modulesPermissionToUpdate = new List<ModulePermission>();
            string commaSeparatedIds = string.Join(",", moduleIds);
            string commaSeparatedUserIds = string.Join(",", userIds);
            var modulesList = _moduleRepository.GetAll(1,10000000,ids: commaSeparatedIds);
            var modulesPermissionList = _modulePermissionRepository.GetAllModulePermissions(null,1,100000,null,false,false,null, commaSeparatedIds, commaSeparatedUserIds);
            foreach (var model in models)
            {
                var modulePermissionExists = modulesPermissionList.Data.Where(x => x.ModuleId == model.ModuleId && x.UserId == model.UserId).FirstOrDefault();
                var module = modulesList.Data.Where(x => x.Id == model.ModuleId).FirstOrDefault();
                if (modulePermissionExists != null)
                {
                    modulePermissionExists.ModuleName = module.Name;
                    modulePermissionExists.Read = model.Read;
                    modulePermissionExists.Create = model.Create;
                    modulePermissionExists.Update = model.Update;
                    modulePermissionExists.Delete = model.Delete;

                    modulesPermissionToUpdate.Add(modulePermissionExists);
                }
                else
                {

                    modulesPermissionToInsert.Add(new ModulePermission
                    {
                        UserId = model.UserId,
                        ModuleId = model.ModuleId,
                        GroupId = null,
                        ModuleName = module.Name,
                        Read = model.Read,
                        Create = model.Create,
                        Update = model.Update,
                        Delete = model.Delete,

                    });
                }
            }
            modulesPermissionToUpdate = modulesPermissionToUpdate.DistinctBy(x => x.Id).ToList();
            modulesPermissionToInsert = modulesPermissionToInsert
                .DistinctBy(x => new { x.UserId, x.ModuleId })
                .ToList();

            if (!modulesPermissionToUpdate.IsNullOrEmpty()) _modulePermissionRepository.UpdateManyEntity(modulesPermissionToUpdate);

            if(!modulesPermissionToInsert.IsNullOrEmpty()) _modulePermissionRepository.InsertMany(modulesPermissionToInsert);
            return models;


        }
        public List<ModulePermission> InsertManyGroup(List<CreateModulePermissionGroupModel> models)
        {
            var moduleIds = models.Select(x => x.ModuleId).Distinct().ToList();
            var groupIds = models.Select(x => x.GroupId).Distinct().ToList();
            var users = _userService.GetUsersByGroupIds(groupIds);
            var userIds = users.Select(x => x.Id).ToList();
            var modulesPermissionToInsert = new List<ModulePermission>();
            var modulesPermissionGroupToInsert = new List<ModulePermission>();
            var modulesPermissionGroupToUpdate = new List<ModulePermission>();
            var modulesPermissionToUpdate = new List<ModulePermission>();
            string commaSeparatedIds = string.Join(",", moduleIds);
            string commaSeparatedUserIds = string.Join(",", userIds);
            string commaSeparatedGroupIds = string.Join(",", groupIds);
            var modulesList = _moduleRepository.GetAll(1, 10000000, ids: commaSeparatedIds);
            var modulesPermissionList = _modulePermissionRepository.GetAllModulePermissions(null, 1, 100000, null, false, false, null, commaSeparatedIds, commaSeparatedUserIds);

            var modulesPermissionGroupList = _modulePermissionRepository.GetAllModulePermissions(null, 1, 100000, null, false, false, null, commaSeparatedIds, null, commaSeparatedGroupIds);
            foreach (var model in models)
            {
                var module = modulesList.Data.Where(x => x.Id == model.ModuleId).FirstOrDefault();
                foreach (var userId in userIds)
                {
                    var modulePermissionExists = modulesPermissionList.Data.Where(x => x.ModuleId == model.ModuleId && x.UserId == userId).FirstOrDefault();
              
                    if (modulePermissionExists != null)
                    {
                        modulePermissionExists.ModuleName = module.Name;
                        modulePermissionExists.Read = model.Read;
                        modulePermissionExists.Create = model.Create;
                        modulePermissionExists.Update = model.Update;
                        modulePermissionExists.Delete = model.Delete;

                        modulesPermissionToUpdate.Add(modulePermissionExists);
                    }
                    else
                    {

                        modulesPermissionToInsert.Add(new ModulePermission
                        {
                            UserId = userId,
                            ModuleId = model.ModuleId,
                            GroupId = null,
                            ModuleName = module.Name,
                            Read = model.Read,
                            Create = model.Create,
                            Update = model.Update,
                            Delete = model.Delete,

                        });
                    }
                }
                var moduleGroupPermissionExists = modulesPermissionGroupList.Data.Where(x => x.GroupId == model.GroupId && x.ModuleId == model.ModuleId).FirstOrDefault();

                if (moduleGroupPermissionExists != null)
                {
                    moduleGroupPermissionExists.ModuleName = module.Name;
                    moduleGroupPermissionExists.Read = model.Read;
                    moduleGroupPermissionExists.Create = model.Create;
                    moduleGroupPermissionExists.Update = model.Update;
                    moduleGroupPermissionExists.Delete = model.Delete;

                    modulesPermissionGroupToUpdate.Add(moduleGroupPermissionExists);
                }

                else
                {

                    modulesPermissionGroupToInsert.Add(new ModulePermission
                    {
                     
                        ModuleId = model.ModuleId,
                        GroupId = model.GroupId,
                        ModuleName = module.Name,
                        Read = model.Read,
                        Create = model.Create,
                        Update = model.Update,
                        Delete = model.Delete,
                    });
                }


            }
            modulesPermissionToUpdate = modulesPermissionToUpdate.DistinctBy(x => x.Id).ToList();
            modulesPermissionToInsert = modulesPermissionToInsert
                .DistinctBy(x => new { x.UserId, x.ModuleId })
                .ToList();

            modulesPermissionGroupToInsert = modulesPermissionGroupToInsert.DistinctBy(x => new { x.GroupId, x.ModuleId }).ToList();
            modulesPermissionGroupToUpdate = modulesPermissionGroupToUpdate.DistinctBy(x => x.Id).ToList();

            if (!modulesPermissionGroupToUpdate.IsNullOrEmpty()) _modulePermissionRepository.UpdateManyEntity(modulesPermissionGroupToUpdate);

            if (!modulesPermissionGroupToInsert.IsNullOrEmpty()) _modulePermissionRepository.InsertMany(modulesPermissionGroupToInsert);

            if (!modulesPermissionToUpdate.IsNullOrEmpty()) _modulePermissionRepository.UpdateManyEntity(modulesPermissionToUpdate);

            if (!modulesPermissionToInsert.IsNullOrEmpty()) _modulePermissionRepository.InsertMany(modulesPermissionToInsert);
            var listReturn = new List<ModulePermission>();

            listReturn.AddRange(modulesPermissionGroupToUpdate);
            listReturn.AddRange(modulesPermissionGroupToInsert);
            listReturn.AddRange(modulesPermissionToUpdate);
            listReturn.AddRange(modulesPermissionToInsert);

            listReturn = listReturn.DistinctBy(x => x.Id).ToList();
            return listReturn;
        }


        public void RegisterRecursive(CreateModulePermissionModel model, List<ModulePermission> modulePermissionsDatabaseList,List<Entities.Module> modulesDatabaseList, List<Entities.ModulePermission> listCreateModulesPermission, List<Entities.ModulePermission> listUpdateModulesPermission)
        {
            //var isValid = ValidaModulePermission(model.UserId);
            //if (!isValid) return;
            var module = modulesDatabaseList.Where(x=> x.Id == model.ModuleId).FirstOrDefault();
            

            if (module == null)
            {
                _bus.RaiseEvent(new DomainNotification("ModulePermissionService", "Module Not Found"));
                return;
            }
         
            try
            {
    
               var modulePermissionExists = modulePermissionsDatabaseList.Where(x=> x.UserId == model.UserId && x.ModuleId == model.ModuleId).FirstOrDefault();
               if(modulePermissionExists == null)
               {
                    var modulePermission = new Entities.ModulePermission
                    {
                        UserId = model.UserId,
                        ModuleId = model.ModuleId,
                        ModuleName = module.Name,
                        Read = model.Read,
                        Create = model.Create,
                        Update = model.Update,
                        Delete = model.Delete,
                    };
                    listCreateModulesPermission.Add(modulePermission);
               }
               else
               {
                    modulePermissionExists.UserId = model.UserId;
                    modulePermissionExists.ModuleId = model.ModuleId;
                    modulePermissionExists.ModuleName = module.Name;
                    modulePermissionExists.Read = model.Read;
                    modulePermissionExists.Create = model.Create;
                    modulePermissionExists.Update = model.Update;
                    modulePermissionExists.Delete = model.Delete;
                    listUpdateModulesPermission.Add(modulePermissionExists);
               }
               if (module.ModuleId != null && module.ModuleId != "")
               {
                    RegisterRecursive(new CreateModulePermissionModel { Create = model.Create, Read = model.Read, Delete = model.Delete, Update = model.Update,ModuleId = module.ModuleId, UserId = model.UserId}, modulePermissionsDatabaseList,modulesDatabaseList,listCreateModulesPermission,listUpdateModulesPermission);
               }

                if (listCreateModulesPermission.Any()) 
                {
                    _modulePermissionRepository.InsertMany(listCreateModulesPermission);
                    listCreateModulesPermission.Clear();
                }
                if (listUpdateModulesPermission.Any()) 
                {
                    _modulePermissionRepository.UpdateManyEntity(listUpdateModulesPermission);
                    listUpdateModulesPermission.Clear();
                } 
            }   
            
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification("ModulePermissionService", ex.Message));
                return;
            }
        }

       
        public void Delete(string id)
        {
            var modulePermission = _modulePermissionRepository.GetById(id);
            if (modulePermission == null)
            {
                _bus.RaiseEvent(new DomainNotification("ModulePermissionService", "ModulePermission Not Found"));
                return;
            }
            modulePermission.IsDeleted = true;

            try
            {
                _modulePermissionRepository.Update(modulePermission);

            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification("ModulePermissionService", ex.Message));
                return;
            }
        }
        public ModulePermission GetById(string id)
        {
            return _modulePermissionRepository.GetById(id);
        }

        public  object GetAll(string groupId, string userId,int? page, int? limit, string sortField = null, bool sortDesc = false)
        {
            return _modulePermissionRepository.GetAllModulePermissions(userId,page, limit, sortField, sortDesc,groupIds: groupId);
        }

     
    }
}
