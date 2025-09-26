using MediatR;
using Microsoft.AspNetCore.Mvc;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Commands;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Simjob.Framework.Application.Controllers
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]

    public abstract class BaseController : ControllerBase
    {
        private readonly IMediatorHandler _bus;
        private readonly DomainNotificationHandler _notifications;

        protected BaseController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications)
        {
            _bus = bus;
            _notifications = (DomainNotificationHandler)notifications;
        }

        protected IActionResult ResponseDefault(object obj = null)
        {
            SendNotificationsByModelStateErrors();

            if (HasNotifications())
                return BadRequest(new ApiResponse(GetNotificationsToKeyValuePair(), false));

            return Ok(new ApiResponse(obj));
        }

        private List<KeyValuePair<string, string>> GetNotificationsToKeyValuePair()
        {
            return _notifications.Notifications.Select(n => new KeyValuePair<string, string>(n.Type, n.Value)).ToList();
        }

        private void SendNotificationsByModelStateErrors()
        {
            if (ModelState.IsValid) return;
            var errors = ModelState.SelectMany(ms => ms.Value.Errors).Select(e => e.ErrorMessage);
            foreach (var error in errors)
                SendNotification("ModelState", error);
        }

        protected bool HasNotifications()
        {
            return _notifications.HasNotification();
        }

        protected async Task SendCommand<T>(T command) where T : Command
        {
            await _bus.SendCommand(command);
        }

        protected void SendNotification(string key, string value)
        {
            _bus.RaiseEvent(new DomainNotification(key, value));
        }




    }
}
