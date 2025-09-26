using Simjob.Framework.Infra.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Interfaces
{
    public interface IExternalTokensRepository
    {
        Tokens GetById(string id);

        Tokens GetByUserToken(string userToken);
        Tokens Insert(Tokens obj);

        Tokens Update(string id, Tokens obj);

        void Delete(string id);

        bool Exists(Expression<Func<Tokens, bool>> predicate);
    }
}
