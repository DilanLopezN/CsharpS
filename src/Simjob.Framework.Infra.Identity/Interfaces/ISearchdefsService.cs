using System.Collections.Generic;
using Simjob.Framework.Domain.Models;

namespace Simjob.Framework.Infra.Identity.Interfaces
{
    public interface ISearchdefsService
    {
        public void Register(Searchdefs search);
        public Searchdefs GetSearchdefsById(string id);
        public void UpdateSearchdefs(Searchdefs search,string id);
        public void DeleteSearchdefs(string id);
        List<Searchdefs> GetSearchdefs(string accessToken);
    }
}
