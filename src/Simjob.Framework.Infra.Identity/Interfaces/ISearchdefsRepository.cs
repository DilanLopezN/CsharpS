using System;
using System.Linq.Expressions;
using Simjob.Framework.Domain.Models;

namespace Simjob.Framework.Infra.Identity.Interfaces
{
    public interface ISearchdefsRepository
    {
        void Insert(Searchdefs obj);
        Searchdefs GetById(string id);
        //List<Searchdefs> GetAll();
        void Update(string id, Searchdefs obj);
        void Delete(string id);      
        
        bool Exists(Expression<Func<Searchdefs, bool>> predicate);
    }
}
