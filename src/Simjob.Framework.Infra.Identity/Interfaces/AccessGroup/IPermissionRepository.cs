using Simjob.Framework.Infra.Identity.Entities;
using System;
using System.Linq.Expressions;
namespace Simjob.Framework.Infra.Identity.Interfaces
{
    public interface IPermissionRepository
    {
        Permission GetByField(string field, string value);
        void Delete(string id);
        void Insert(Permission obj);
        void Update(string id, Permission obj);
        bool Exists(Expression<Func<Permission, bool>> predicate);
        void DeletePermanent(string id);
    }
}
