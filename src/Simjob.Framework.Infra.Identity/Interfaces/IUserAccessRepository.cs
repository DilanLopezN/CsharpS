using Simjob.Framework.Infra.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Interfaces
{
    public interface IUserAccessRepository
    {
        UserAccess GetById(string id);
        void Insert(UserAccess obj);

        void Update(string id, UserAccess obj);

        void Delete(string id);

        bool Exists(Expression<Func<UserAccess, bool>> predicate);
    }
}
