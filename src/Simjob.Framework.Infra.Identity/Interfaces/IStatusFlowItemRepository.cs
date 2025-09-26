using Simjob.Framework.Infra.Identity.Entities;
using System;
using System.Linq.Expressions;

namespace Simjob.Framework.Infra.Identity.Interfaces
{
    public interface IStatusFlowItemRepository
    {
        StatusFlowItem GetById(string id);
        StatusFlowItem GetBySchemaRecordId(string schemaRecordId);

        void Update(string id, StatusFlowItem obj);

        bool Exists(Expression<Func<StatusFlowItem, bool>> predicate);
    }
}
