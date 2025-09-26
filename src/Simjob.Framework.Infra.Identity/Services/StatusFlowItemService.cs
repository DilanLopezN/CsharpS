using MongoDB.Driver;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Services
{
    public class StatusFlowItemService : IStatusFlowItemService
    {
        protected readonly StatusFlowItemContext Context;
        protected readonly IMongoCollection<StatusFlowItem> Collection;
        private readonly IRepository<StatusFlowItemContext, StatusFlowItem> _repository;
        private readonly IStatusFlowItemRepository _statusflowItemRepository;
        private readonly IUserHelper _userHelper;


        public StatusFlowItemService(StatusFlowItemContext context, IUserHelper userHelper, IRepository<StatusFlowItemContext, StatusFlowItem> repository, IStatusFlowItemRepository statusFlowItemRepository)
        {
            Context = context;
            Collection = context.GetUserCollection();
            _repository = repository;
            _userHelper = userHelper;
            _statusflowItemRepository = statusFlowItemRepository;
        }

        public void DeleteStatusFlowItem(string id)
        {
            _repository.Delete(id);
        }

        public async Task<object> GetAll(int? page, int? limit, string sortField = null, bool sortDesc = false)
        {
            return _repository.GetAll(page, limit, sortField, sortDesc, false);
        }

        public StatusFlowItem GetById(string id)
        {
            return _statusflowItemRepository.GetById(id);
        }

        public StatusFlowItem GetBySchemaRecordId(string schemaRecordId)
        {
            return _statusflowItemRepository.GetBySchemaRecordId(schemaRecordId);
        }

        public StatusFlowItem RegisterAsync(StatusFlowItem statusFlowItem)
        {
            return _repository.Insert(statusFlowItem);
        }

        public void UpdateStatusFlowItem(StatusFlowItem statusFlowItem, string id)
        {
            _statusflowItemRepository.Update(id, statusFlowItem);
        }
    }
}
