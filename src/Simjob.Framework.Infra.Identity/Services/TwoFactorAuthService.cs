using MediatR;
using MongoDB.Driver;
using Newtonsoft.Json;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Infra.Schemas.Entities;
using Simjob.Framework.Infra.Schemas.Models;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using MoreLinq;
using Simjob.Framework.Infra.Schemas.Interfaces;
using System.Threading.Tasks;
using Simjob.Framework.Domain.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using Simjob.Framework.Domain.Util;

namespace Simjob.Framework.Infra.Identity.Services
{
    public class TwoFactorAuthService : ITwoFactorAuthService
    {
        protected readonly TwoFactorAuthContext Context;
        protected readonly IMongoCollection<UserTwoFactorAuth> Collection;
        private readonly DomainNotificationHandler _notifications;
        private readonly IMediatorHandler _bus;
        private readonly IRepository<TwoFactorAuthContext, UserTwoFactorAuth> _twoFactorAuthRepository;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;
        private readonly ISchemaBuilder _schemaBuilder;
        static readonly HttpClient client = new();

        private static readonly Encoding rawEncoder = Encoding.UTF8;
        protected FilterDefinition<UserTwoFactorAuth> UserOnlyFilter = Builders<UserTwoFactorAuth>.Filter.Empty;

        public TwoFactorAuthService(TwoFactorAuthContext context, INotificationHandler<DomainNotification> notifications, IMediatorHandler bus,
            IRepository<TwoFactorAuthContext, UserTwoFactorAuth> twoFactorAuthRepository, IRepository<MongoDbContext, Schema> schemaRepository, ISchemaBuilder schemaBuilder)
        {
            Context = context;
            _notifications = (DomainNotificationHandler)notifications;
            Collection = context.GetUserCollection();
            _bus = bus;
            _twoFactorAuthRepository = twoFactorAuthRepository;
            _schemaRepository = schemaRepository;
            _schemaBuilder = schemaBuilder;
        }

        public void Register(UserTwoFactorAuth obj)
        {
            _twoFactorAuthRepository.Insert(obj);
        }

        public Task<string> getToken()
        {
            try
            {
                string token = "";
                var url = "https://asap.simjob.net:2053/api/token";
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "POST";
                httpRequest.Accept = "application/json";
                httpRequest.ContentType = "application/json";

                var data = @"{
                      ""email"": ""admin@admin.com"",
                      ""password"": ""admin123*""
                    }";

                var content = new StringContent(data, Encoding.UTF8, "application/json");
                var result = client.PostAsync(url, content).Result;

                token = JObject.Parse(result.Content.ReadAsStringAsync().Result).DescendantsAndSelf().OfType<JProperty>().Single(x => x.Name.Equals("accessToken")).Value.ToString();
                return Task.FromResult(token);
            } catch (Exception ex)
            {
                Logs.AddLog("[TwoFactorAuthService] - " + ex.ToString());
                throw;
            }
        }

        public async Task<string> getTokenToSendEmail(string token)
        {
            try
            {
                string tokenToSendEmail = "";
                var url = "https://asap.simjob.net:2053/api/TokenGen/generate";

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync(url);

                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                tokenToSendEmail = JObject.Parse(responseBody)
                    .DescendantsAndSelf()
                    .OfType<JProperty>()
                    .Single(x => x.Name.Equals("data"))
                    .Value.ToString();

                return tokenToSendEmail;
            } catch (Exception ex)
            {
                Logs.AddLog("[TwoFactorAuthService] - " + ex.ToString());
                throw;
            }
        }

        public async Task<UserTwoFactorAuth> SendCodeVerification(UserTwoFactorAuth a2f)
        {
            try
            {
                Random generator = new Random();
                String code = generator.Next(0, 1000000).ToString("D6");
                a2f.Code = code;

                string token = await getToken();
                string tokenToSendEmail = await getTokenToSendEmail(token);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                

                if (token != null)
                {
                    var url = "https://asap.simjob.net:2053/api/MailRequest";
                    string[] parameterNames = new string[] { a2f.Email };

                    JArray jarrayObj = new JArray();

                    foreach (string parameterName in parameterNames)
                    {
                        jarrayObj.Add(parameterName);
                    }

                    JObject UpdateAccProfile = (dynamic)new JObject(
                        new JProperty("token", tokenToSendEmail),
                        new JProperty("subject", "Simjob Login Request - Verification"),
                        new JProperty("body", code + " is your verification code."),
                        new JProperty("from", "admin@admin.com"),
                        new JProperty("recipients", jarrayObj)
                    );

                    var content = new StringContent(UpdateAccProfile.ToString(), Encoding.UTF8, "application/json");
                    var result = client.PostAsync(url, content).Result;
                }

                Register(a2f);
                return new UserTwoFactorAuth(a2f.UserId, "", a2f.Email, a2f.PublicIP, a2f.Hash, a2f.Authenticated);
            }
            catch (Exception ex)
            {
                Logs.AddLog("[TwoFactorAuthService] - " + ex.Message);
                return default;
            }
        }

        public UserTwoFactorAuth CodeIsValid(string id, string code, string hash, string ip)
        {
            var filterId = Builders<UserTwoFactorAuth>.Filter.Eq(e => e.UserId, id);
            var filterCode = Builders<UserTwoFactorAuth>.Filter.Eq(b => b.Code, code);
            var filterHash = Builders<UserTwoFactorAuth>.Filter.Eq(c => c.Hash, hash);
            var filterAuth = Builders<UserTwoFactorAuth>.Filter.Eq(c => c.Authenticated, true);

            var userAuthenticated = Collection.Find(filterId & filterHash & filterAuth).FirstOrDefault();
            if (userAuthenticated != null)
            {
                if(userAuthenticated.Authenticated == true) return userAuthenticated;
            }

            var userA2F = Collection.Find(filterId & filterCode & filterHash).FirstOrDefault();

            if (userA2F == null)
            {
                UserTwoFactorAuth user = new UserTwoFactorAuth(id, "", "", "", "", false);
                return user;
            }

            if (userA2F.Authenticated == true) return userA2F;
            if (Convert.ToDateTime(userA2F.ExpireAt) >= DateTime.Now)
            {
                userA2F.Authenticated = true;
                _twoFactorAuthRepository.Update(userA2F);
                return userA2F;
            }

            return default;
        }

    }
}

