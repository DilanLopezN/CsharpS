using MediatR;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Commands;
using Simjob.Framework.Domain.Core.Events;
using Simjob.Framework.Domain.Core.Notifications;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Simjob.Framework.Domain.CommandHandlers
{

    [ExcludeFromCodeCoverage]
    public abstract class CommandHandler
    {
        private readonly DomainNotificationHandler _notifications;
        protected readonly IMediatorHandler Bus;

        protected CommandHandler(INotificationHandler<DomainNotification> notifications, IMediatorHandler bus)
        {
            _notifications = (DomainNotificationHandler)notifications;
            Bus = bus;
        }


        protected void SendEvent<T>(T @event) where T : Event
        {
            Bus.RaiseEvent(@event);
        }


        protected void SendDomainNotification(string key, string value)
        {
            Bus.RaiseEvent(new DomainNotification(key, value));
        }

        protected bool HasNotifications()
        {
            return _notifications.HasNotification();
        }

        protected bool CommandIsValid<T>(T command) where T : Command
        {
            if (command.IsValid()) return true;

            var errors = command.GetValidationResultErrors().Select(c => c.ErrorMessage);
            foreach (var error in errors)
                Bus.RaiseEvent(new DomainNotification("CommandHandler", error));

            return false;
        }
    }
}
