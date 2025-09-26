using Microsoft.Extensions.DependencyInjection;
using Simjob.Framework.Application.Stock.Controllers;
using Simjob.Framework.Application.Stock.Interfaces;
using Simjob.Framework.Application.Stock.Repositories;
using Simjob.Framework.Application.Stock.Services;

namespace Simjob.Framework.Application.Stock.Extensions
{
    public static class ServiceCollectionConfigurationExtension
    {
        public static void AddStockModule(this IServiceCollection services)
        {
            services.AddMvc().AddApplicationPart(typeof(StockController).Assembly);

            services.AddScoped<IStockOperationService, StockOperationService>();

            services.AddScoped<IStockOperationBsonRepository, StockOperationBsonRepository>();
            services.AddScoped<IStockMovBsonRepository, StockMovBsonRepository>();
            services.AddScoped<IStockItemBalanceBsonRepository, StockItemBalanceBsonRepository>();
            services.AddScoped<IStockLotBsonRepository, StockLotBsonRepository>();
            services.AddScoped<IStockLotBalanceBsonRepository, StockLotBalanceBsonRepository>();
            services.AddScoped<IStockBalanceBsonRepository, StockBalanceBsonRepository>();
        }
    }
}
