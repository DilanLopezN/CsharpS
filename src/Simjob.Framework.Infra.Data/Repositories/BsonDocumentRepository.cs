using MongoDB.Bson;
using MongoDB.Driver;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Infra.Data.Context;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Simjob.Framework.Infra.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    public class BsonDocumentRepository : IBsonDocumentRepository
    {
        protected readonly MongoDbContext Context;
        protected readonly IMongoCollection<BsonDocument> Collection;
        protected readonly FilterDefinition<BsonDocument> IsDeletedFilter = Builders<BsonDocument>.Filter.Eq("isDeleted", new BsonBoolean(false));
        public BsonDocumentRepository(MongoDbContext context,string collectionName)
        {
            Context = context;
            Collection = Context.GetDatabase().GetCollection<BsonDocument>(collectionName);
        }

        public BsonDocument GetById(string id)
        {
            var idFilter = Builders<BsonDocument>.Filter.Eq("_id", id);
            return Collection.Find(idFilter).FirstOrDefault();
        }

        public List<BsonDocument> Search(FilterDefinition<BsonDocument> filterDefinition)
        {
            var filter = IsDeletedFilter & filterDefinition;
            return Collection.Find(filter).ToList();
        }
    }
}
