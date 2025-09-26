using MediatR;
using Simjob.Framework.Domain.CommandHandlers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Infra.Schemas.Commands.Views;
using Simjob.Framework.Infra.Schemas.Entities;
using Simjob.Framework.Infra.Schemas.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Schemas.CommandHandlers
{
    public class ViewCommandHandler : CommandHandler, IRequestHandler<InsertViewCommand, bool>,
        IRequestHandler<UpdateViewCommand, bool>, IRequestHandler<DeleteViewCommand, bool>
    {

        private readonly IEntityService _entityService;
        private readonly IUserHelper _userHelper;
        private readonly IViewService _viewService;
        public ViewCommandHandler(INotificationHandler<DomainNotification> notifications, IMediatorHandler bus, IEntityService schemaService, IUserHelper userHelper, IViewService viewService) : base(notifications, bus)
        {
            _entityService = schemaService;
            _userHelper = userHelper;
            _viewService = viewService;
        }

        public Task<bool> Handle(InsertViewCommand request, CancellationToken cancellationToken)
        {
            if (!CommandIsValid(request)) return Task.FromResult(false);
            //string quote = "\"";

            //foreach (var param in request.Parameters)
            //{
            //    var parametro = "@" + param.Name;
            //    if (request.Query.Contains(param.Name))
            //    {
            //        if(param.DataType == ViewParameterType.String || param.DataType == ViewParameterType.Date)
            //            request.Query = request.Query.Replace(parametro, quote + param.Value + quote);
            //        else
            //        {
            //            request.Query = request.Query.Replace(parametro, param.Value);
            //        }
            //    }
            //}

            var entity = new Views(request.Name, request.Description, request.Query, request.SchemaName,request.Type, request.Parameters);
            _viewService.Insert(entity);

            return Task.FromResult(!HasNotifications());
        }

        public Task<bool> Handle(UpdateViewCommand request, CancellationToken cancellationToken)
        {
            if (!CommandIsValid(request)) return Task.FromResult(false);

            var entity = ViewsFactory.Full(request.Id, request.Name, request.Description, request.Query, request.SchemaName,request.Type, request.Parameters);
            _viewService.Update(request.Id, entity);

            return Task.FromResult(!HasNotifications());
        }

        public Task<bool> Handle(DeleteViewCommand request, CancellationToken cancellationToken)
        {
            if (!CommandIsValid(request)) return Task.FromResult(false);
            _viewService.Delete(request.Id);
            return Task.FromResult(!HasNotifications());
        }
    }
}
