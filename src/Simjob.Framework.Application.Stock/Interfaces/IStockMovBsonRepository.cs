using MongoDB.Bson;
using System.Collections.Generic;

namespace Simjob.Framework.Application.Stock.Interfaces
{
    public interface IStockMovBsonRepository
    {
        List<BsonDocument> GetByStockOperationId(string stockOperationId);

        List<BsonDocument> GetByIds(List<string> ids);

    }
}
