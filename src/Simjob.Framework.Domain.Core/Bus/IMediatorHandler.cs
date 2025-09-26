using Simjob.Framework.Domain.Core.Commands;
using Simjob.Framework.Domain.Core.Events;
using System.Threading.Tasks;

namespace Simjob.Framework.Domain.Core.Bus
{
    public interface IMediatorHandler
    {
        Task RaiseEvent<T>(T @event) where T : Event;
        Task SendCommand<T>(T command) where T : Command;

    }

}
