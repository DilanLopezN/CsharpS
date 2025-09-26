using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Simjob.Framework.Domain.Core.Notifications
{
    public class DomainNotificationHandler : INotificationHandler<DomainNotification>
    {
        public List<DomainNotification> Notifications { get; set; }

        public DomainNotificationHandler()
        {
            Notifications = new List<DomainNotification>();
        }

            public void Dispose() => Notifications = new List<DomainNotification>();

        public bool HasNotification() => Notifications.Any();

        public Task Handle(DomainNotification notification, CancellationToken cancellationToken)
        {
            Notifications.Add(notification);
            return Task.CompletedTask;
        }
    }
}
