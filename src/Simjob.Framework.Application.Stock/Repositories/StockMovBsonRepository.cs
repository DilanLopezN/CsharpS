using MongoDB.Bson;
using MongoDB.Driver;
using Simjob.Framework.Application.Stock.Interfaces;
using Simjob.Framework.Infra.Data.Context;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Simjob.Framework.Application.Stock.Repositories
{
    [ExcludeFromCodeCoverage]
    public class StockMovBsonRepository : IStockMovBsonRepository
    {
        private readonly MongoDbContext _context;
        private readonly IMongoCollection<BsonDocument> _collection;
        private readonly FilterDefinition<BsonDocument> _isDeletedFilter = Builders<BsonDocument>.Filter.Eq("isDeleted", new BsonBoolean(false));
        public StockMovBsonRepository(MongoDbContext context)
        {
            _context = context;
            _collection = _context.GetDatabase().GetCollection<BsonDocument>("stockmov");
        }

        public List<BsonDocument> GetByIds(List<string> ids)
        {
            var stockOperationIdFilter = Builders<BsonDocument>.Filter.In("_id", ids);
            var filter = _isDeletedFilter & stockOperationIdFilter;
            return _collection.Find(filter).ToList();
        }

        public List<BsonDocument> GetByStockOperationId(string stockOperationId)
        {
            var stockOperationIdFilter = Builders<BsonDocument>.Filter.Eq("stockOperationId", stockOperationId);
            var filter = _isDeletedFilter & stockOperationIdFilter;
            return _collection.Find(filter).ToList();
        }
    }
}
