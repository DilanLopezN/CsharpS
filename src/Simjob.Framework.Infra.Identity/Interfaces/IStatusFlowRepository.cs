using Simjob.Framework.Domain.Util;
using Simjob.Framework.Infra.Identity.Entities;
using System;
using System.Linq.Expressions;

namespace Simjob.Framework.Infra.Identity.Interfaces
{
    public interface IStatusFlowRepository
    {
        StatusFlow GetById(string id);
        void Insert(StatusFlow obj);

        void Update(string id, StatusFlow obj);

        void Delete(string id);

        bool Exists(Expression<Func<StatusFlow, bool>> predicate);

        PaginationData<StatusFlow> GetAll(int? page = null, int? limit = null, string sortField = null, bool sortDesc = false, bool addIsDeleted = false, string ids = null);
    }
}
