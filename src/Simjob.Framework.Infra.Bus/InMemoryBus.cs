using MediatR;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Commands;
using Simjob.Framework.Domain.Core.Events;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Bus
{
    [ExcludeFromCodeCoverage]
    public class InMemoryBus : IMediatorHandler
    {
        private readonly IMediator _mediator;
        //private readonly EventStoreRepository<MongoDbContext> _eventStoreTenanty;
        //private readonly EventStoreRepository<IdentityContext> _eventStoreIdentity;
        public InMemoryBus(IMediator mediator)
        //EventStoreRepository<MongoDbContext> eventStoreTenanty,
        //EventStoreRepository<IdentityContext> eventStoreIdentity)
        {
            _mediator = mediator;
            //_eventStoreTenanty = eventStoreTenanty;
            //_eventStoreIdentity = eventStoreIdentity;
        }


        public Task RaiseEvent<T>(T @event) where T : Event
        {
            //if (@event.MessageType != "DomainNotification")
            //{
            //    if (@event.MessageType.Contains("User"))
            //        _eventStoreIdentity.Save(@event);
            //    else
            //        _eventStoreTenanty.Save(@event);
            //}

            _mediator.Publish(@event);
            return Task.CompletedTask;
        }

        public Task SendCommand<T>(T command) where T : Command
        {
            _mediator.Send(command);
            return Task.CompletedTask;
        }
    }
}
