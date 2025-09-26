using MongoDB.Driver;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Infra.Identity.Commands;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Simjob.Framework.Infra.Identity.Services
{
    public class StatusFlowService : IStatusFlowService
    {

        protected readonly StatusFlowContext Context;
        protected readonly IMongoCollection<StatusFlow> Collection;
        private readonly IRepository<StatusFlowContext, StatusFlow> _repository;
        private readonly IStatusFlowRepository _statusFlowRepository;
        private readonly IUserHelper _userHelper;


        public StatusFlowService(StatusFlowContext context, IUserHelper userHelper, IRepository<StatusFlowContext, StatusFlow> repository, IStatusFlowRepository statusFlowRepository)
        {
            Context = context;
            Collection = context.GetUserCollection();
            _userHelper = userHelper;
            _statusFlowRepository = statusFlowRepository;
            _repository = repository;
        }
        public void DeleteStatusFlow(string id)
        {
            _repository.Delete(id);
        }

        //public async Task<object> GetAll(int? page, int? limit, string sortField = null, bool sortDesc = false)
        //{
        //    return _repository.GetAll(page, limit, sortField, sortDesc, false);
        //}

        public StatusFlow GetById(string id)
        {
            var filterId = Builders<StatusFlow>.Filter.Eq(u => u.Id, id);
            var filterIsDeleted = Builders<StatusFlow>.Filter.Eq(u => u.IsDeleted, false);
            return Collection.Find(filterId & filterIsDeleted).FirstOrDefault();
        }

        public StatusFlow GetBySchemaAndField(string schemaName, string field, string tenanty)
        {
            var filterSchema = Builders<StatusFlow>.Filter.Eq(u => u.SchemaName, schemaName);
            var filterField = Builders<StatusFlow>.Filter.Eq(u => u.Field, field);
            var filterTenanty = Builders<StatusFlow>.Filter.Eq(u => u.Tenanty, tenanty);
            var filterIsDeleted = Builders<StatusFlow>.Filter.Eq(u => u.IsDeleted, false);
            return Collection.Find(filterSchema & filterField & filterTenanty & filterIsDeleted).FirstOrDefault();
        }

        public void Register(StatusFlowCommand statusFlow)
        {
            var existe = GetBySchemaAndField(statusFlow.SchemaName, statusFlow.Field, statusFlow.Tenanty);
            if (existe != null)
            {
                var userName = _userHelper.GetUserName();
                var statusFlowSchemaFilter = Builders<StatusFlow>.Filter.Eq(u => u.Id, existe.Id);
                var statusFlowUpdate = Builders<StatusFlow>.Update.Set(u => u.Field, statusFlow.Field)
                                                              .Set(u => u.Tenanty, statusFlow.Tenanty)
                                                              .Set(u => u.Properties, statusFlow.Properties)
                                                              .Set(u => u.UpdateBy, userName);

                Collection.UpdateOne(statusFlowSchemaFilter, statusFlowUpdate);
            }
            else
            {
                var newStatusFlow = new StatusFlow(statusFlow.SchemaName, statusFlow.Tenanty, statusFlow.Field, statusFlow.Properties);
                _repository.Insert(newStatusFlow);
            }

        }

        public void UpdateStatusFlowStatus(StatusFlow statusFlow, List<string> aprovadores, string status)
        {
            statusFlow.Properties.Where(property => property.Status == status).FirstOrDefault().AprovEmail = aprovadores.ToArray();

            var statusFlowCommand = new StatusFlowCommand();
            statusFlowCommand.SchemaName = statusFlow.SchemaName;
            statusFlowCommand.Tenanty = statusFlow.Tenanty;
            statusFlowCommand.Field = statusFlow.Field;
            statusFlowCommand.Properties = statusFlow.Properties;

            Register(statusFlowCommand);
        }

        //public void UpdateStatusFlow(StatusFlowCommand statusFlow)
        //{
        //    var userName = _userHelper.GetUserName();
        //    var statusFlowSchemaFilter = Builders<StatusFlow>.Filter.Eq(u => u.SchemaName, statusFlow.SchemaName);
        //    var statusFlowUpdate = Builders<StatusFlow>.Update.Set(u => u.Field, statusFlow.Field)
        //                                                  .Set(u => u.Status, statusFlow.Status)
        //                                                  .Set(u => u.ProximoStatus, statusFlow.ProximoStatus)
        //                                                  .Set(u => u.AprovEmail, statusFlow.AprovEmail)
        //                                                  .Set(u => u.AprovMin, statusFlow.AprovMin)
        //                                                  .Set(u => u.UpdateBy, userName);

        //    Collection.UpdateOne(statusFlowSchemaFilter, statusFlowUpdate);

        //}
    }
}
