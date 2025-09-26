using System;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Schemas.Interfaces
{
    public interface ISchemaBuilder
    {
        Task<Type> GetSchemaType(string schemaName, bool reloadSchema = false);
        Task InsertInternSchemas();
    }
}
