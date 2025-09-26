using MongoDB.Driver;

namespace Simjob.Framework.Domain.Data
{
    public abstract class DbContext
    {
        public abstract IMongoDatabase GetDatabase();
        public abstract IMongoDatabase GetDatabaseByTenanty(string tenanty);
    }
}
