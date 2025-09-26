using MediatR;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Schemas.Entities;
using Simjob.Framework.Infra.Schemas.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Schemas.Services
{
    public class ViewService : IViewService
    {
        private readonly ISchemaBuilder _schemaBuilder;
        private readonly IServiceProvider _serviceProvider;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;
        private readonly IRepository<MongoDbContext, Views> _viewsRepository;
        private readonly MongoDbContext _context;
        private readonly DomainNotificationHandler _notifications;
        private readonly IMediatorHandler _bus;

        public ViewService(ISchemaBuilder schemaBuilder, IServiceProvider serviceProvider,
            IRepository<MongoDbContext, Schema> schemaRepository, IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, IRepository<MongoDbContext, Views> viewsRepository, MongoDbContext context)
        {
            _schemaBuilder = schemaBuilder;
            _serviceProvider = serviceProvider;
            _schemaRepository = schemaRepository;
            _viewsRepository = viewsRepository;
            _bus = bus;
            _context = context;
            _notifications = (DomainNotificationHandler)notifications;

        }

        public Views GetViewByName(string name)
        {
            return _viewsRepository.GetByField("name", name);
        }

        public object GetRepository(Type schemaType)
        {
            var typeRepo = typeof(IRepository<,>).MakeGenericType(typeof(MongoDbContext), schemaType);
            return _serviceProvider.GetService(typeRepo);
        }

        public async Task<object> GetAll(int? page, int? limit, string sortField = null, bool sortDesc = false, string ids = null)
        {
            return _viewsRepository.GetAll(page, limit, sortField, sortDesc, false, ids);
        }

        public object ExecuteByViewId(Views view)
        {
            return _viewsRepository.ViewFilter(view.Query);
        }

        public async Task<object> SerachFields(string searchFields, string value, int? page, int? limit, string sortField = null, bool sortDesc = false, string ids = null)
        {
            Views teste = new(null, null, null, null,null, null);
            var propertiesToSearch = teste.GetType().GetProperties();
            List<string> properties = new();

            if (searchFields == null)
            {
                foreach (var obj in propertiesToSearch)
                {
                    properties.Add(obj.Name.ToString());
                }

                searchFields = string.Join(',', properties);
            }

            return _viewsRepository.SearchRegexByFields(searchFields, value, "", null, page, limit, sortField, null, sortDesc, ids);
        }

        public Views GetById(string id)
        {
            return _viewsRepository.GetById(id);
        }

        public bool Insert(Views view)
        {
            var exists = _viewsRepository.Exists(x => x.Name == view.Name);
            if (exists)
            {
                _bus.RaiseEvent(new DomainNotification("ViewService", "ViewName already exists"));
                return false;
            }

            try
            {
                _viewsRepository.Insert(view);
                return true;
            }
            catch
            {
                _bus.RaiseEvent(new DomainNotification("ViewService", "Error on insert"));
                return false;
            }
        }

        public bool Update(string id, Views view)
        {
            var entity = _viewsRepository.GetById(id);
            if (entity == null)
            {
                _bus.RaiseEvent(new DomainNotification("ViewService", "View not found"));
                return false;
            }

            var exists = _viewsRepository.Exists(x => x.Name == view.Name);

            if (entity.Name != view.Name && exists)
            {
                _bus.RaiseEvent(new DomainNotification("ViewService", "ViewName already exists"));
                return false;
            }

            try
            {
                _viewsRepository.Update(view);
                return true;
            }
            catch
            {
                _bus.RaiseEvent(new DomainNotification("ViewService", "Error on insert"));
                return false;
            }
        }

        public bool Delete(string id)
        {
            var entity = _viewsRepository.GetById(id);
            if (entity == null)
            {
                _bus.RaiseEvent(new DomainNotification("ViewService", "View not found"));
                return false;
            }

            try
            {
                _viewsRepository.Delete(id);
                return true;
            }
            catch
            {
                _bus.RaiseEvent(new DomainNotification("ViewService", "Error on delete view"));
                return false;
            }
        }

        public Views GetViewByNameExternal(string name, string tenanty)
        {
            return _viewsRepository.GetByFieldExternal("name", name, tenanty);
        }

        public Views GetByIdExternal(string id, string tenanty)
        {
            return _viewsRepository.GetByIdExternal(id, tenanty);
        }
    }
}