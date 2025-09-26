using Simjob.Framework.Infra.Identity.Entities;
using System;
using System.Linq.Expressions;

namespace Simjob.Framework.Infra.Identity.Interfaces
{
    public interface IProfileRepository
    {
        void Insert(Profile obj);

        Profile GetById(string id);

        Profile GetByTenanty(string tenanty);

        void Update(string id, Profile obj);

        void Delete(string id);

        bool Exists(Expression<Func<Profile, bool>> predicate);
    }
}