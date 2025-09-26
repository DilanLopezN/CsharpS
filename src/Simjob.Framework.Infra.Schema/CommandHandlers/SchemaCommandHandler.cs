using MediatR;
using Simjob.Framework.Domain.CommandHandlers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Schemas.Commands;
using Simjob.Framework.Infra.Schemas.Entities;
using Simjob.Framework.Infra.Schemas.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Schemas.CommandHandlers
{
    public class SchemaCommandHandler : CommandHandler, IRequestHandler<InsertSchemaCommand, bool>,
        IRequestHandler<UpdateSchemaCommand, bool>,
        IRequestHandler<DeleteSchemaCommand, bool>
    {
        private readonly ISchemaBuilder _schemaBuilder;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;
        public SchemaCommandHandler(INotificationHandler<DomainNotification> notifications, IMediatorHandler bus, IRepository<MongoDbContext, Schema> schemaRepository, ISchemaBuilder schemaBuilder) : base(notifications, bus)
        {
            _schemaRepository = schemaRepository;
            _schemaBuilder = schemaBuilder;
        }

        public Task<bool> Handle(InsertSchemaCommand request, CancellationToken cancellationToken)
        {
            if (!CommandIsValid(request)) return Task.FromResult(false);
            var entity = new Schema(request.Name, request.StrongEntity, request.JsonValue);

            var isExist = _schemaRepository.GetByField("Name", entity.Name);

            if (isExist == null)
            {
                _schemaRepository.Insert(entity);
            }

            return Task.FromResult(!HasNotifications());
        }

        public Task<bool> Handle(UpdateSchemaCommand request, CancellationToken cancellationToken)
        {

            if (!CommandIsValid(request)) return Task.FromResult(false);
            var entity = new Schema(request.Name, request.StrongEntity, request.JsonValue) { Id = request.Id };
            _schemaBuilder.GetSchemaType(entity.Name, true);
            var schema = _schemaRepository.GetByField("name", request.Name);
            entity.StrongEntity = schema.StrongEntity;
            entity.Id = schema.Id;
            _schemaRepository.Update(entity);
            return Task.FromResult(!HasNotifications());
        }

        public Task<bool> Handle(DeleteSchemaCommand request, CancellationToken cancellationToken)
        {
            if (!CommandIsValid(request)) return Task.FromResult(false);
            _schemaRepository.Delete(request.Id);
            return Task.FromResult(!HasNotifications()); ;
        }
    }
}
