using MediatR;
using Simjob.Framework.Domain.CommandHandlers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Infra.Identity.Commands;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.CommandHandlers
{
    [ExcludeFromCodeCoverage]
    public class UserCommandHandler : CommandHandler, IRequestHandler<RegisterUserCommand, bool>, IRequestHandler<SignInUserCommand, bool>, IRequestHandler<UpdateUserCommand, bool>
    {
        private readonly IUserService _userService;
        public UserCommandHandler(INotificationHandler<DomainNotification> notifications, IMediatorHandler bus, IUserService userService) : base(notifications, bus)
        {
            _userService = userService;
        }

        public Task<bool> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            if (!CommandIsValid(request)) return Task.FromResult(false);

            var entity = new User()
            {
                UserName = request.Email,
                Tenanty = request.Tenanty,
                Name = request.Name,
                Telefone = request.Telefone,
                CompanySiteIdDefault = request.CompanySiteIdDefault,
                CompanySiteIds = request.CompanySiteIds,
                GroupId = request.GrupoId,
                Root = request.Root,
                ControlAccess = request.ControlAccess,
                FirstLogin = true,
                CreateBy = request.CreateBy,
                Cd_pessoa = request.cd_pessoa,
                Cd_usuario = request.cd_usuario
            };

            _userService.Register(entity, request.Password);

            return Task.FromResult(!HasNotifications());
        }

        public Task<bool> Handle(SignInUserCommand request, CancellationToken cancellationToken)
        {
            if (!CommandIsValid(request)) return Task.FromResult(false);

            _userService.Login(request.Tenanty, request.Email, request.Password);

            return Task.FromResult(!HasNotifications());
        }

        public Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            if (!CommandIsValid(request)) return Task.FromResult(false);

            var entity = new User()
            {
                Id = request.UserId,
                Name = request.Name,
                UserName = request.Email,
                Telefone = request.Telefone,
                CompanySiteIdDefault = request.CompanySiteIdDefault,
                CompanySiteIds = request.CompanySiteIds,
                Tenanty = request.Tenanty,
                GroupId = request.GroupId,
                Root = request.Root,
                ControlAccess = request.ControlAccess,
                LogonAzure = request.LogonAzure,
                UpdateBy = request.UpdateBy
            };

            _userService.UpdateGroup(entity, request.GroupId);

            return Task.FromResult(!HasNotifications());
        }
    }
}
