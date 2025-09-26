using Microsoft.Extensions.DependencyInjection;
using Simjob.Framework.Infra.Schemas.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Schemas.Extensions
{
    public static class ServiceCollectionSchemaConfigurationExtensions
    {
        public static void AddInternSchemas(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var schemaBuilder = serviceProvider.GetService<ISchemaBuilder>();
            schemaBuilder.InsertInternSchemas();
        }


    }
}
