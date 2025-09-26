using Simjob.Framework.Infra.Identity.Entities;
using System;
using System.Linq.Expressions;
namespace Simjob.Framework.Infra.Identity.Interfaces
{
    public interface IGroupRepository
    {
        User GetByField(string field, string value);
        void Delete(string id);
        void Insert(Group obj);
        void Update(string id, Group obj);
        bool Exists(Expression<Func<Group, bool>> predicate);
    }
}
