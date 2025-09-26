using MongoDB.Bson;

namespace Simjob.Framework.Application.Stock.Interfaces
{
    public interface IStockLotBalanceBsonRepository
    {
        BsonDocument GetByStockLotIdLastDiffToday(string stockLotId);
        BsonDocument GetByStockLotIdToday(string stockLotId);
    }
}
