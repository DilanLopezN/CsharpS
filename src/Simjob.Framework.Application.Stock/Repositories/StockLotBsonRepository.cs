using MongoDB.Bson;
using MongoDB.Driver;
using Simjob.Framework.Application.Stock.Interfaces;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Data.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace Simjob.Framework.Application.Stock.Repositories
{
    [ExcludeFromCodeCoverage]
    public class StockLotBsonRepository : BsonDocumentRepository,IStockLotBsonRepository
    {
        public StockLotBsonRepository(MongoDbContext context) : base(context, "stocklot")
        {

        }

        public BsonDocument GetByCode(string code)
        {
            var codeFilter = Builders<BsonDocument>.Filter.Eq("code", code);
            var filter = IsDeletedFilter & codeFilter;
            return Collection.Find(filter).FirstOrDefault();
        }

   
    }
}
