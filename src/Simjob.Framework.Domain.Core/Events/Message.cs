using MediatR;

namespace Simjob.Framework.Domain.Core.Events
{
    public abstract class Message : IRequest<bool>
    {
        public string MessageType { get;  protected set; }
        public string AggregateId { get; protected set; }

        public Message(string messageType = null, string aggregateId = null)
        {
            MessageType = messageType ?? GetType().Name;
            AggregateId = aggregateId;
        }

        
    }
}
