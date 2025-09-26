using Simjob.Framework.Infra.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Interfaces
{
    public interface IApproveLogRepository
    {
        ApproveLog GetById(string id);
        void Insert(ApproveLog obj);

        void Update(string id, ApproveLog obj);

        void Delete(string id);

        bool Exists(Expression<Func<ApproveLog, bool>> predicate);
    }
}
