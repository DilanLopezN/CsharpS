using MediatR;
using System;

namespace Simjob.Framework.Domain.Core.Events
{
    public abstract class Event : Message, INotification
    {
        public DateTime Timespan { get; protected set; }
        public Event() 
        {
            Timespan = DateTime.Now;
        }
    }
}
