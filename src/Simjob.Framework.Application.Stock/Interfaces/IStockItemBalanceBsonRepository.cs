using MongoDB.Bson;
using Simjob.Framework.Domain.Interfaces.Repositories;

namespace Simjob.Framework.Application.Stock.Interfaces
{
    public interface IStockItemBalanceBsonRepository : IBsonDocumentRepository
    {
        public BsonDocument GetByItemIdLastDiffToday(string itemId);

        public BsonDocument GetByItemIdToday(string itemId);
    }
}
