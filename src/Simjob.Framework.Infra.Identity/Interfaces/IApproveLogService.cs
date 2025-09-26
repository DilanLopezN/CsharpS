using Simjob.Framework.Infra.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Interfaces
{
    public interface IApproveLogService
    {
        public void Register(ApproveLog approveLog);
        public ApproveLog GetById(string id);
        public void DeleteApproveLog(string id);
        public void UpdateApproveLog(ApproveLog approveLog, string id);
        public ApproveLog GetBySchemaRecordId(string id);
        Task<object> GetAll(int? page, int? limit, string sortField = null, bool sortDesc = false);
    }
}
