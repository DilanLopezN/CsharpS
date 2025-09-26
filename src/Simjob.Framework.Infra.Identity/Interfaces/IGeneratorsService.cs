using Simjob.Framework.Infra.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Interfaces
{
    public interface IGeneratorsService
    {
        public void Register(Generators generators);
        public Generators GetById(string id);
        Task<object> GetAll(int? page, int? limit, string sortField = null, bool sortDesc = false);
        Generators GetGeneratorByName(string schemaName);
        Task<string> GetAutoincAsync(string schemaName, string field, string mask);
        object GetRepository(Type schemaType);
    }
}
