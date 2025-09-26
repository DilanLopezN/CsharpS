using Simjob.Framework.Infra.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Interfaces
{
    public interface IGeneratorsRepository
    {
        Generators GetById(string id);
        void Insert(Generators obj);

        bool Exists(Expression<Func<Generators, bool>> predicate);
    }
}
