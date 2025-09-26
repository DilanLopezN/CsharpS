using MongoDB.Bson;
using MongoDB.Driver;
using Simjob.Framework.Application.Stock.Interfaces;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Data.Repositories;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Simjob.Framework.Application.Stock.Repositories
{
    [ExcludeFromCodeCoverage]
    public class StockItemBalanceBsonRepository : BsonDocumentRepository, IStockItemBalanceBsonRepository
    {
        public StockItemBalanceBsonRepository(MongoDbContext context) : base(context, "stockitembalance")
        {
        }


        public BsonDocument GetByItemIdLastDiffToday(string itemId)
        {
            var findStockItemBalanceLast = Collection.Find(
                                        Builders<BsonDocument>.Filter.Eq("itemId", itemId) &
                                        Builders<BsonDocument>.Filter.Lt("date", new BsonDateTime(DateTime.Now.Date)))
                                        .Sort(Builders<BsonDocument>.Sort.Descending("date"));

            return findStockItemBalanceLast.FirstOrDefault();
        }

        public BsonDocument GetByItemIdToday(string itemId)
        {
            var findStockItemBalanceToDay = Collection.Find(
               Builders<BsonDocument>.Filter.Eq("itemId", itemId) &
               Builders<BsonDocument>.Filter.Gte("date", new BsonDateTime(DateTime.Now.Date)) &
               Builders<BsonDocument>.Filter.Lt("date", new BsonDateTime(DateTime.Now.AddDays(1).Date))
               ).Sort(Builders<BsonDocument>.Sort.Ascending("date"));

            return  findStockItemBalanceToDay.FirstOrDefault();
        }
    }
}
