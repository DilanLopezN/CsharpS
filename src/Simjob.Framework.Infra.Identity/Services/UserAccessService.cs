using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using SendGrid;
using SendGrid.Helpers.Mail;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Infra.Identity.Commands.AccessGroup;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Infra.Identity.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Services
{
    public class UserAccessService : IUserAccessService
    {
        protected readonly UserAccessContext Context;
        protected readonly IMongoCollection<UserAccess> Collection;

        //  private readonly DomainNotificationHandler _notifications;
        //  private readonly IMediatorHandler _bus;
        private readonly IRepository<UserAccessContext, UserAccess> _userAccessRepository;

        //  private readonly IRepository<MongoDbContext, Schema> _schemaRepository;
        //  private readonly ISchemaBuilder _schemaBuilder;
        //   private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        //  private static readonly Encoding rawEncoder = Encoding.UTF8;

        public UserAccessService(UserAccessContext context, IRepository<UserAccessContext, UserAccess> userAccessRepository, IConfiguration configuration)
        {
            Context = context;
            Collection = context.GetUserCollection();
            _userAccessRepository = userAccessRepository;
            _configuration = configuration;
        }

        public async Task<object> GetAll(int? page, int? limit, string sortField = null, bool sortDesc = false)
        {
            return _userAccessRepository.GetAll(page, limit, sortField, sortDesc, false);
        }

        public void DeleteUserAccess(string id)
        {
            _userAccessRepository.Delete(id);
        }

        public void UpdateUserAccess(UserAccess userAccess, string id)
        {
            var userAccessIdFilter = Builders<UserAccess>.Filter.Eq(u => u.Id, id);
            var userAccessUpdate = Builders<UserAccess>.Update.Set(u => u.UserId, userAccess.UserId)
                                                          .Set(u => u.Tenanty, userAccess.Tenanty)
                                                          .Set(u => u.UserName, userAccess.UserName)
                                                          .Set(u => u.Latitude, userAccess.Latitude)
                                                          .Set(u => u.Longitude, userAccess.Longitude)
                                                          .Set(u => u.Ip, userAccess.Ip)
                                                          .Set(u => u.UserAgent, userAccess.UserAgent)
                                                          .Set(u => u.SchemaName, userAccess.SchemaName)
                                                          .Set(u => u.Description, userAccess.Description)
                                                          .Set(u => u.SchemaRecordId, userAccess.SchemaRecordId)
                                                          .Set(u => u.ValorOriginal, userAccess.ValorOriginal)
                                                          .Set(u => u.ValorAlterado, userAccess.ValorAlterado);

            Collection.UpdateOne(userAccessIdFilter, userAccessUpdate);
        }

        public UserAccess GetById(string id)
        {
            var filterId = Builders<UserAccess>.Filter.Eq(u => u.Id, id);
            var filterIsDeleted = Builders<UserAccess>.Filter.Eq(u => u.IsDeleted, false);
            return Collection.Find(filterId & filterIsDeleted).FirstOrDefault();
        }

        public UserAccess GetByUserId(string userId)
        {
            var filterId = Builders<UserAccess>.Filter.Eq(u => u.UserId, userId);
            var filterIsDeleted = Builders<UserAccess>.Filter.Eq(u => u.IsDeleted, false);
            return Collection.Find(filterId & filterIsDeleted).FirstOrDefault();
        }

        public UserAccess GetBySchemaRecordId(string schemaRecordId)
        {
            var filterId = Builders<UserAccess>.Filter.Eq(u => u.SchemaRecordId, schemaRecordId);
            var filterIsDeleted = Builders<UserAccess>.Filter.Eq(u => u.IsDeleted, false);
            return Collection.Find(filterId & filterIsDeleted).FirstOrDefault();
        }

        public async Task Register(UserAccess userAccess)
        {
            //bool isFirstuserAccess = !_userAccessRepository.Exists(u => u.UserName == userAccess.UserName);
            //bool userAccessExists = false;

            //if (!isFirstuserAccess)
            //    userAccessExists = _userAccessRepository.Exists(u => u.UserName == userAccess.UserName && u.UserName == userAccess.UserName);

            //if (userAccessExists)
            //{
            //    _bus.RaiseEvent(new DomainNotification("UserAccessService", "UserName already exists"));
            //    return;
            //}

            _userAccessRepository.Insert(userAccess);
        }

        public bool SendEmail(SendEmailCommand command)
        {
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("admin@simjob.net", "Admin");
            var subject = command.Subject;
            var to = new EmailAddress(command.To, command.NameTo);
            var plainTextContent = command.PlainTextContent;
            var htmlContent = command.HtmlContent;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = client.SendEmailAsync(msg);
            return response.Result.IsSuccessStatusCode;
        }

        public bool SendEmail(ConfigSendGrid configSendGrid, SendEmailCommand command)
        {
            var apiKey = configSendGrid.ApiKey;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(configSendGrid.Email, configSendGrid.Name);
            var subject = command.Subject;
            var to = new EmailAddress(command.To, command.NameTo);
            var plainTextContent = command.PlainTextContent;
            var htmlContent = command.HtmlContent;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = client.SendEmailAsync(msg);
            return response.Result.IsSuccessStatusCode;
        }
    }
}