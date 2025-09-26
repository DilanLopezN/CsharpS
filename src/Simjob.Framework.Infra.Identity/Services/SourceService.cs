using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Infra.Identity.Models.Source;
using System;
using System.Linq;

namespace Simjob.Framework.Infra.Identity.Services
{
    public class SourceService : ISourceService
    {
        private readonly IRepository<SourceContext, Entities.Source> _sourceRepository;
        private readonly IMediatorHandler _bus;
        public SourceService(IRepository<SourceContext, Entities.Source> sourceRepository, IMediatorHandler bus)
        {
            _sourceRepository = sourceRepository;
            _bus = bus;
        }
        public void Disable(string id)
        {
            var source = _sourceRepository.GetById(id);

            var sources = _sourceRepository.GetAll();
            var sourcesF = sources.Data.Where(x => x.Active == true);
            if (source == null)
            {
                _bus.RaiseEvent(new DomainNotification("sourceService", "source Not Found"));
                return;
            }
            source.Active = false;

            try
            {
                _sourceRepository.Update(source);

            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification("sourceService", ex.Message));
                return;
            }
        }

        public void Enable(string id)
        {
            var source = _sourceRepository.GetById(id);
            if (source == null)
            {
                _bus.RaiseEvent(new DomainNotification("sourceService", "source Not Found"));
                return;
            }
            source.Active = true;

            try
            {
                _sourceRepository.Update(source);

            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification("sourceService", ex.Message));
                return;
            }
        }

        public Source GetById(string id)
        {
            return _sourceRepository.GetById(id);
        }

        public Source Register(CreateSourceModel model)
        {
          
            var source = new Entities.Source
            {
                Host = model.Host ?? "",
                User = model.User ?? "",
                Password = model.Password ?? "",
                Port = model.Port,
                DbName = model.DbName ?? "",
                Dialect = model.Dialect ?? "",
                Active = true,
                Description = $"{model.Host}-{model.DbName}"
            };
            try
            {
                _sourceRepository.Insert(source);
                return source;
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification("sourceService", ex.Message));
                return null;
            }
        }

        public Source Update(UpdateSourceModel model)
        {

            var source = _sourceRepository.GetById(model.Id);
            if (source == null)
            {
                _bus.RaiseEvent(new DomainNotification("sourceService", "source Not Found"));
                return null;
            }

            source.Host = model.Host ?? "";
            source.User = model.User ?? "";
            source.Password = model.Password ?? "";
            source.Port = model.Port;
            source.DbName = model.DbName ?? "";
            source.Dialect = model.Dialect ?? "";
            source.Description = model.Host ?? "" + " - " + model.DbName ?? "";

            try
            {
                _sourceRepository.Update(source);
                return source;
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification("sourceService", ex.Message));
                return null;
            }
        }
    }
}
