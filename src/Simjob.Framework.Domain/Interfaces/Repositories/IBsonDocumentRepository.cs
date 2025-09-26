using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;

namespace Simjob.Framework.Domain.Interfaces.Repositories
{
    public interface IBsonDocumentRepository
    {
        public BsonDocument GetById(string id);
        public List<BsonDocument> Search(FilterDefinition<BsonDocument> filterDefinition);
    }
}
