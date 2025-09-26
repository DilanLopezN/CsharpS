using MongoDB.Driver;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Infra.Identity.Models.ModuleIdentity;
using Simjob.Framework.Infra.Schemas.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Services
{
    public class ModuleIdentityService : IModuleIdentityService
    {
        private readonly IRepository<ModuleIdentityContext, Entities.ModuleIdentity> _moduleIdentityRepository;
        private readonly IMediatorHandler _bus;
       
        public ModuleIdentityService(IRepository<ModuleIdentityContext, Entities.ModuleIdentity> moduleIdentityRepository, IMediatorHandler bus)
        {
            _moduleIdentityRepository = moduleIdentityRepository;
            _bus = bus;
        }

        public void Register(CreateModuleIdentityModel model)
        {
            var moduleIdentity = new Entities.ModuleIdentity
            {
                Name = model.Name,
                Description = model.Description,
                Type = model.Type,
                Icon = model.Icon,
                Active = true,
                Tenanty = model.Tenanty,
                ModuleId = model.ModuleId ?? null,
                Price = model.Price,
                ActionId = model.ActionId ?? null,
                Path = model.Path ?? ""
            };
            try
            {
                _moduleIdentityRepository.Insert(moduleIdentity);
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification("ModuleIdentityService", ex.Message));
                return;
            }
        }

        public void Update(UpdateModuleIdentityModel model)
        {
            var moduleIdentity = _moduleIdentityRepository.GetById(model.Id);
            if (moduleIdentity == null)
            {
                _bus.RaiseEvent(new DomainNotification("ModuleIdentityService", "ModuleIdentity Not Found"));
                return;
            }

            moduleIdentity.Name = model.Name;
            moduleIdentity.Description = model.Description;
            moduleIdentity.Type = model.Type;
            moduleIdentity.Icon = model.Icon;
            moduleIdentity.Tenanty = model.Tenanty;
            moduleIdentity.ModuleId = model.ModuleId ?? null;
            moduleIdentity.Price = model.Price;
            moduleIdentity.ActionId = model.ActionId ?? null;
            moduleIdentity.Path = model.Path ?? "";

            try
            {
                _moduleIdentityRepository.Update(moduleIdentity);

            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification("ModuleIdentityService", ex.Message));
                return;
            }
        }

        public ModuleIdentity GetById(string id)
        {
            return _moduleIdentityRepository.GetById(id);
        }

        public void Disable(string id)
        {
            var moduleIdentity = _moduleIdentityRepository.GetById(id);
            if (moduleIdentity == null)
            {
                _bus.RaiseEvent(new DomainNotification("ModuleIdentityService", "ModuleIdentity Not Found"));
                return;
            }
            moduleIdentity.Active = false;

            try
            {
                _moduleIdentityRepository.Update(moduleIdentity);

            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification("ModuleService", ex.Message));
                return;
            }
        }

        public void Enable(string id)
        {
            var moduleIdentity = _moduleIdentityRepository.GetById(id);
            if (moduleIdentity == null)
            {
                _bus.RaiseEvent(new DomainNotification("ModuleIdentityService", "ModuleIdentity Not Found"));
                return;
            }
            moduleIdentity.Active = true;

            try
            {
                _moduleIdentityRepository.Update(moduleIdentity);
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification("ModuleService", ex.Message));
                return;
            }
        }
    }
}
