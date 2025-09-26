using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using MoreLinq;
using Newtonsoft.Json;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Entities;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Domain.Util;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Infra.Identity.Models.Module;
using Simjob.Framework.Infra.Schemas.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static MongoDB.Driver.WriteConcern;

namespace Simjob.Framework.Infra.Identity.Services
{
    public class ModuleService : IModuleService
    {
        private readonly IRepository<ModuleContext, Entities.Module> _moduleRepository;
        private readonly IMediatorHandler _bus;
        protected readonly IMongoCollection<Entities.Module> _collection;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;
        protected readonly ModuleContext Context;
        public ModuleService(IRepository<ModuleContext, Entities.Module> moduleRepository, IMediatorHandler bus, IRepository<MongoDbContext, Schema> schemaRepository)
        {
            _moduleRepository = moduleRepository;
            _bus = bus;
            _schemaRepository = schemaRepository;
        }
        public void Disable(string id)
        {
            var module = _moduleRepository.GetById(id);
            if (module == null)
            {
                _bus.RaiseEvent(new DomainNotification("ModuleService", "Module Not Found"));
                return;
            }
            module.Active = false;

            try
            {
                _moduleRepository.Update(module);

            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification("ModuleService", ex.Message));
                return;
            }
        }

        public void Enable(string id)
        {
            var module = _moduleRepository.GetById(id);
            if (module == null)
            {
                _bus.RaiseEvent(new DomainNotification("ModuleService", "Module Not Found"));
                return;
            }
            module.Active = true;

            try
            {
                _moduleRepository.Update(module);

            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification("ModuleService", ex.Message));
                return;
            }
        }
        

        public Entities.Module GetById(string id)
        {
            return _moduleRepository.GetById(id);
        }

        public Entities.Module? Register(CreateModuleModel model)
        {
            var moduleExiste = _moduleRepository.GetByField("Name",model.Name);
            if (moduleExiste != null) 
            {
                _bus.RaiseEvent(new DomainNotification("ModuleService","Name já existe"));
                return null;
            } 
            var module = new Entities.Module
            {
                Name = model.Name,
                Description = model.Description,
                Type = model.Type,
                Icon = model.Icon,
                Active = true,
                ModuleId = model.ModuleId ?? null,
                Price = model.Price,
                ActionId = model.ActionId ?? null,
                Order = model.Order,
                Path = model.Path ?? ""
            };
            try
            {
                _moduleRepository.Insert(module);
                return module;
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification("ModuleService", ex.Message));
                return null;
            }
        }

        public Entities.Module? Update(UpdateModuleModel model)
        {
            var module = _moduleRepository.GetById(model.Id);
            if (module == null)
            {
                _bus.RaiseEvent(new DomainNotification("ModuleService", "Module Not Found"));
                return null;
            }
            var moduleExiste = _moduleRepository.GetByField("Name", model.Name);
            if (moduleExiste != null && moduleExiste.Id != module.Id)
            {
                _bus.RaiseEvent(new DomainNotification("ModuleService", "Name já existe"));
                return null;
            }

            module.Name = model.Name;
            module.Description = model.Description;
            module.Type = model.Type;
            module.Icon = model.Icon;
            module.ModuleId = model.ModuleId ?? null;
            module.Price = model.Price;
            module.ActionId = model.ActionId ?? null;
            module.Order = model.Order;
            module.Path = model.Path ?? "";

            try
            {
                _moduleRepository.Update(module);
                return module;
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification("ModuleService", ex.Message));
                return null;
            }
        }
    }
}
