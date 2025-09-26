using Simjob.Framework.Domain.Util;
using Simjob.Framework.Infra.Identity.Entities;
using System;
using System.Linq.Expressions;

namespace Simjob.Framework.Infra.Identity.Interfaces
{
    public interface INotificationRepository
    {
        Notification GetById(string id);
        void Insert(Notification obj);

        void Update(string id, Notification obj);

        void Delete(string id);

        bool Exists(Expression<Func<Notification, bool>> predicate);

        //PaginationData<Notification> GetAllByUserIdAndName(string userId, string userName, int? page = null, int? limit = null, string sortField = null, bool sortDesc = false, bool addIsDeleted = false, string ids = null);

        PaginationData<Notification> GetAllByUserIdAndName(string userId, string userName, int? page = null, int? limit = null, string sortField = null, bool sortDesc = false, bool addIsDeleted = false, string ids = null);
    }
}
