using Simjob.Framework.Infra.Identity.Entities;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Interfaces
{
    public interface INotificationService
    {
        public Notification RegisterAsync(Notification notification);
        public Notification GetById(string id);
        //public Notification GetByUserId(string id);
        public Notification GetByIdView(string id);
        public Task<object> GetNotificationsByUserId(string userId, string userName, int? page, int? limit, string sortField = null, bool sortDesc = false);
        //public Notification GetByUserIdAndUserName(string id, string userName);
        public void DeleteNotification(string id);
        public void UpdateNotification(Notification notification, string id);
        public Notification GetBySchemaRecordId(string id);

        Task<object> GetAll(int? page, int? limit, string sortField = null, bool sortDesc = false);

        //Task<object> GetAllByUserIdAndName(string userId, string userName, int? page, int? limit, string sortField = null, bool sortDesc = false);
    }
}
