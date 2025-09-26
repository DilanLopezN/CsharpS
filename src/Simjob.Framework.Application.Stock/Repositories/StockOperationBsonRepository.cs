using Simjob.Framework.Application.Stock.Interfaces;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Data.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace Simjob.Framework.Application.Stock.Repositories
{
    [ExcludeFromCodeCoverage]
    public class StockOperationBsonRepository : BsonDocumentRepository, IStockOperationBsonRepository
    {

        public StockOperationBsonRepository(MongoDbContext context) : base(context, "stockoperation")
        {

        }


    }
}
