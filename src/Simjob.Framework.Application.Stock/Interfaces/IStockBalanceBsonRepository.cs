using MongoDB.Bson;

namespace Simjob.Framework.Application.Stock.Interfaces
{
    public interface IStockBalanceBsonRepository
    {
        BsonDocument GetByFields(string itemId, string stockLotId, string stockLocId);
    }
}
