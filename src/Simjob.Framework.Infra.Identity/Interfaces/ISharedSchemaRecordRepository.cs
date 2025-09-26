using Simjob.Framework.Infra.Identity.Entities;
using System;
using System.Linq.Expressions;

namespace Simjob.Framework.Infra.Identity.Interfaces
{
    public interface ISharedSchemaRecordRepository
    {
        SharedSchemaRecord GetById(string id);
        void Insert(SharedSchemaRecord obj);

        void Update(string id, SharedSchemaRecord obj);

        void Delete(string id);

        bool Exists(Expression<Func<SharedSchemaRecord, bool>> predicate);


    }
}
