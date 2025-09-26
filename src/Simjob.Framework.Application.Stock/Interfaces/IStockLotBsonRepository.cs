using MongoDB.Bson;
using Simjob.Framework.Domain.Interfaces.Repositories;

namespace Simjob.Framework.Application.Stock.Interfaces
{
    public interface IStockLotBsonRepository : IBsonDocumentRepository
    {
        BsonDocument GetByCode(string code);
    }
}
