using MongoDB.Driver;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Services
{
    public class NotificationService : INotificationService
    {
        protected readonly NotificationContext Context;
        protected readonly IMongoCollection<Notification> Collection;
        private readonly IRepository<NotificationContext, Notification> _notificationRepository;
        private readonly INotificationRepository _notificationRepository2;
        private readonly IUserHelper _userHelper;

        public NotificationService(NotificationContext context, IUserHelper userHelper, IRepository<NotificationContext, Notification> notificationRepository, INotificationRepository notificationRepository2)
        {
            Context = context;
            Collection = context.GetUserCollection();

            _notificationRepository = notificationRepository;
            _userHelper = userHelper;
            _notificationRepository2 = notificationRepository2;
        }
        public void DeleteNotification(string id)
        {
            _notificationRepository.Delete(id);
        }

        public async Task<object> GetAll(int? page, int? limit, string sortField = null, bool sortDesc = false)
        {
            return _notificationRepository.GetAll(page, limit, sortField, sortDesc, false);
        }

        //public async Task<object> GetAllByUserIdAndName(string userId, string userName, int? page, int? limit, string sortField = null, bool sortDesc = false)
        //{
        //    return _notificationRepository2.GetAllByUserIdAndName(userId, userName, page, limit, sortField, sortDesc, false);
        //}

        public Notification GetById(string id)
        {
            var filterId = Builders<Notification>.Filter.Eq(u => u.Id, id);
            var filterIsDeleted = Builders<Notification>.Filter.Eq(u => u.IsDeleted, false);
            var filterIsAprovNull = Builders<Notification>.Filter.Eq(u => u.Aprov, null);
            return Collection.Find(filterId & filterIsDeleted & filterIsAprovNull).FirstOrDefault();
        }

        public Notification GetByIdView(string id)
        {
            var filterId = Builders<Notification>.Filter.Eq(u => u.Id, id);
            var filterIsDeleted = Builders<Notification>.Filter.Eq(u => u.IsDeleted, false);
            return Collection.Find(filterId & filterIsDeleted).FirstOrDefault();
        }

        public Notification GetBySchemaRecordId(string schemaRecordId)
        {
            var filterId = Builders<Notification>.Filter.Eq(u => u.SchemaRecordId, schemaRecordId);
            var filterIsDeleted = Builders<Notification>.Filter.Eq(u => u.IsDeleted, false);
            var filterIsAprovNull = Builders<Notification>.Filter.Eq(u => u.Aprov, null);
            return Collection.Find(filterId & filterIsDeleted & filterIsAprovNull).FirstOrDefault();
        }

        //public Notification GetByUserId(string userId)
        //{
        //    var filterId = Builders<Notification>.Filter.Eq(u => u.UserId, userId);
        //    var filterIsDeleted = Builders<Notification>.Filter.Eq(u => u.IsDeleted, false);


        //    //var filterIsAprovNull = Builders<Notification>.Filter.Eq(u => u.Aprov, null);
        //    return Collection.Find(filterId & filterIsDeleted).FirstOrDefault();
        //}

        public async Task<object> GetNotificationsByUserId(string userId, string userName, int? page, int? limit, string sortField = null, bool sortDesc = false)
        {
            return _notificationRepository2.GetAllByUserIdAndName(userId, userName, page, limit, sortField, sortDesc, false);
        }

        //public Notification GetByUserIdAndUserName(string userId, string userName)
        //{
        //    var filterUserName = Builders<Notification>.Filter
        //                             .ElemMatch(t => t.AprovEmail,
        //                             x => x.email == userName);

        //    var subFilter = Builders<Notification>.Filter.Eq(x => x.AprovEmail.FirstOrDefault().email, userName);

        //    var filterId = Builders<Notification>.Filter.Eq(u => u.UserId, userId);
        //    //Builders<RootEntity>.Filter( => .CarsData.Where(x => x.Car.UUID.Equals("SOMETHING").Count() > 0)
        //    //var filteruserName = Builders<Notification>.Filter.Lt(u => u.AprovEmail.Where());
        //    var filterIsDeleted = Builders<Notification>.Filter.Eq(u => u.IsDeleted, false);
        //    var filterIsAprovNull = Builders<Notification>.Filter.Eq(u => u.Aprov, null);
        //    var teste = Collection.Find(filterUserName).FirstOrDefault();
        //    return Collection.Find((filterId | filterUserName) & filterIsDeleted & filterIsAprovNull).Limit(10).FirstOrDefault();
        //}



        public Notification RegisterAsync(Notification notification)
        {
            var obj = _notificationRepository.Insert(notification);
            return obj;
        }

        public void UpdateNotification(Notification notification, string id)
        {
            //var userName = _userHelper.GetUserName();
            var notificationIdFilter = Builders<Notification>.Filter.Eq(u => u.Id, id);
            var notificationUpdate = Builders<Notification>.Update.Set(u => u.SchemaRecordId, notification.SchemaRecordId)
                                                          .Set(u => u.SchemaName, notification.SchemaName)
                                                          .Set(u => u.Field, notification.Field)
                                                          .Set(u => u.Value, notification.Value)
                                                          .Set(u => u.ValueOld, notification.ValueOld)
                                                          .Set(u => u.Msg, notification.Msg)
                                                          .Set(u => u.Obs, notification.Obs)
                                                          .Set(u => u.UserId, notification.UserId)
                                                          .Set(u => u.AprovMin, notification.AprovMin)
                                                          .Set(u => u.AprovEmail, notification.AprovEmail)
                                                          .Set(u => u.Aprov, notification.Aprov)
                                                          .Set(u => u.View, notification.View)
                                                          .Set(u => u.UpdateAt, DateTime.Now);

            Collection.UpdateOne(notificationIdFilter, notificationUpdate);
        }
    }

}
