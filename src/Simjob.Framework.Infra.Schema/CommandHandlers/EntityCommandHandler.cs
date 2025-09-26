using MediatR;
using Simjob.Framework.Domain.CommandHandlers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Infra.Schemas.Commands.Entities;
using Simjob.Framework.Infra.Schemas.Events.Entities;
using Simjob.Framework.Infra.Schemas.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Schemas.CommandHandlers
{
    public class EntityCommandHandler : CommandHandler, IRequestHandler<InsertEntityCommand, bool>,
        IRequestHandler<UpdateEntityCommand, bool>, IRequestHandler<DeleteEntityCommand, bool>, IRequestHandler<InsertManyEntityCommand, bool>, IRequestHandler<UpdateManyEntityCommand, bool>
    {

        private readonly IEntityService _entityService;
        //private readonly EventStoreRepository<MongoDbContext> _eventStore;
        private readonly IUserHelper _userHelper;
        public EntityCommandHandler(INotificationHandler<DomainNotification> notifications, IMediatorHandler bus, IEntityService schemaService, IUserHelper userHelper) : base(notifications, bus)
        {
            _entityService = schemaService;
            _userHelper = userHelper;
        }

        public async Task<bool> Handle(InsertEntityCommand request, CancellationToken cancellationToken)
        {

            if (!CommandIsValid(request)) return false;
            if (request.Data.ContainsKey("_id")) request.Data.Add("id", request.Data["_id"]);
            var entity = await _entityService.Insert(request.SchemaName, request.Data);

            if (!HasNotifications())
            {
                try
                {
                    await Bus.RaiseEvent(new InsertEntityEvent(entity.Id, request.SchemaName, request.SchemaJson, request.Data));
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            else
            {
                return false;
            }

            return !HasNotifications();
        }

        public async Task<bool> Handle(UpdateEntityCommand request, CancellationToken cancellationToken)
        {
            if (!CommandIsValid(request)) return false;
            if (request.Data.ContainsKey("_id")) request.Data.Add("id", request.Data["_id"]);
            var entity = await _entityService.Update(request.SchemaName, request.Data);
            if (!HasNotifications())
                await Bus.RaiseEvent(new UpdateEntityEvent(entity.Id, request.SchemaName, request.SchemaJson, request.Data));

            return !HasNotifications();
        }

        public async Task<bool> Handle(DeleteEntityCommand request, CancellationToken cancellationToken)
        {
            if (!CommandIsValid(request)) return false;

            await _entityService.Delete(request.Id, request.SchemaName);
            if (!HasNotifications())
                await Bus.RaiseEvent(new DeleteEntityEvent(request.Id, request.SchemaName));

            return !HasNotifications();
        }

        public async Task<bool> Handle(InsertManyEntityCommand request, CancellationToken cancellationToken)
        {
            await _entityService.InsertMany(request.SchemaName, request.Datas);
            if (!HasNotifications()) return true;
            return false;
        }

        public async Task<bool> Handle(UpdateManyEntityCommand request, CancellationToken cancellationToken)
        {
            await _entityService.UpdateMany(request.SchemaName, request.Datas);
            if (!HasNotifications()) return true;
            return false;
        }
    }
}
