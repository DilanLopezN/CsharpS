using Simjob.Framework.Infra.Identity.Entities;
using System;
using System.Linq.Expressions;
namespace Simjob.Framework.Infra.Identity.Interfaces
{
    public interface ITwoFactorAuthRepository
    {
        User GetByField(string field, string value);
        void Delete(string id);
        void Insert(UserTwoFactorAuth obj);
        void Update(string id, User obj);
        bool Exists(Expression<Func<UserTwoFactorAuth, bool>> predicate);
    }
}
