using MongoDB.Bson;
using MongoDB.Driver;
using Simjob.Framework.Application.Stock.Interfaces;
using Simjob.Framework.Infra.Data.Context;

namespace Simjob.Framework.Application.Stock.Repositories
{
    public class StockBalanceBsonRepository : IStockBalanceBsonRepository
    {
        private readonly MongoDbContext _context;
        private readonly IMongoCollection<BsonDocument> _collection;
        private readonly FilterDefinition<BsonDocument> _isDeletedFilter = Builders<BsonDocument>.Filter.Eq("isDeleted", new BsonBoolean(false));
        public StockBalanceBsonRepository(MongoDbContext context)
        {
            _context = context;
            _collection = _context.GetDatabase().GetCollection<BsonDocument>("stockbalance");
        }


        public BsonDocument GetByFields(string itemId, string stockLotId, string stockLocId)
        {

            var stockitemIdFilter = Builders<BsonDocument>.Filter.Eq("itemId", itemId);
            var stocklotIdFilter = Builders<BsonDocument>.Filter.Eq("stockLotId", stockLotId);
            var stocklocIdFilter = Builders<BsonDocument>.Filter.Eq("stockLocId", stockLocId);
            //var dateFilter = Builders<BsonDocument>.Filter.Gt("date", new BsonDateTime(date.AddHours(-date.Hour).Date));
            var filter = _isDeletedFilter & stockitemIdFilter & stocklotIdFilter & stocklocIdFilter;
            var sort = Builders<BsonDocument>.Sort.Descending("date");
            var options = new FindOptions<BsonDocument>();

            return _collection.Find(filter).Sort(sort).FirstOrDefault();
        }
    }
}
