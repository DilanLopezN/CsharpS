using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Domain.Models;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Infra.Schemas.Entities;
using Simjob.Framework.Infra.Schemas.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;

namespace Simjob.Framework.Infra.Identity.Services
{
    public class ExternalTokensService : IExternalTokensService
    {

        protected readonly ExternalTokensContext Context;
        protected readonly IMongoCollection<Tokens> _collection;
        private readonly DomainNotificationHandler _notifications;
        private readonly IMediatorHandler _bus;
        private readonly IRepository<ExternalTokensContext, Tokens> _externalTokensRepository;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;
        private readonly ISchemaBuilder _schemaBuilder;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IUserHelper _userHelper;
        private static readonly Encoding rawEncoder = Encoding.UTF8;
        private readonly ExternalTokensConfigurations _externalTokensConfigurations;



        public ExternalTokensService(ExternalTokensContext context, INotificationHandler<DomainNotification> notifications, IMediatorHandler bus,
            IRepository<ExternalTokensContext, Tokens> externalTokensRepository, IRepository<MongoDbContext, Schema> schemaRepository, ISchemaBuilder schemaBuilder, IUserService userService, ExternalTokensConfigurations externalTokensConfigurations, IUserHelper userHelper, IConfiguration configuration)
        {
            _externalTokensConfigurations = externalTokensConfigurations;
            Context = context;
            _notifications = (DomainNotificationHandler)notifications;
            _collection = context.GetUserCollection();
            _bus = bus;
            _externalTokensRepository = externalTokensRepository;
            _schemaRepository = schemaRepository;
            _schemaBuilder = schemaBuilder;
            _userService = userService;
            _configuration = configuration;
            _userHelper = userHelper;

        }
        public TokenResponse GerenerateToken(string userId)
        {
            var user = _userService.GetUserById(userId);
            var claims = user.Claims;

            //  ClaimsIdentity identity = new ClaimsIdentity(

            //    new GenericIdentity(user.UserName, "Login"),
            //    new[] { new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            //              new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            //              new Claim("userid", user.Id),
            //              new Claim("tenanty", user.Tenanty),

            //    }
            //);

            // identity.Claims.ToList().AddRange(claims);

            DateTime dtCreation = DateTime.Now;
            DateTime dtExpiration;
            //if (_externalTokensConfigurations.Seconds != 0)
            //{
            //    dtExpiration = dtCreation +
            //    TimeSpan.FromSeconds(_externalTokensConfigurations.Seconds);
            //}

            dtExpiration = dtCreation +
            TimeSpan.FromSeconds(Int32.MaxValue);
            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _externalTokensConfigurations.Issuer,
                Audience = _externalTokensConfigurations.Audience,
                NotBefore = dtCreation,
                Expires = dtExpiration

            });
            var token = handler.WriteToken(securityToken);
            //var version = Assembly.GetExecutingAssembly().GetName().Version;            
            //  var version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            return new TokenResponse(

                user.Id,
                dtCreation,
                dtExpiration,
                token);
        }



        public void DeleteToken(string id)
        {
            _externalTokensRepository.Delete(id);
        }

        public List<Tokens> GetAllTokens()
        {
            var tokens = _collection.Find(Builders<Tokens>.Filter.Eq(u => u.IsDeleted, false)).ToList();

            return tokens;
        }

        public Tokens GetById(string id)
        {
            var token = _externalTokensRepository.GetById(id);
            return token;
        }

        public Tokens GetByUserToken(string userToken)
        {
            var filterId = Builders<Tokens>.Filter.Eq(e => e.UserToken, userToken);
            var filterIsDeleted = Builders<Tokens>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterId & filterIsDeleted).FirstOrDefault();
        }

        public Tokens Register(Tokens tokens)
        {
            _externalTokensRepository.Insert(tokens);
            return tokens;
        }

        public Tokens GetByUserId(string userId)
        {
            var filterId = Builders<Tokens>.Filter.Eq(e => e.UserId, userId);
            var filterIsDeleted = Builders<Tokens>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterId & filterIsDeleted).FirstOrDefault();
        }

        public Tokens Update(string id, Tokens tokens)
        {
            var entity = GetById(id);
            if (entity == null) return null;

            tokens.CreateAt = entity.CreateAt;
            tokens.CreateBy = entity.CreateBy;
            tokens.DeleteAt = entity.DeleteAt;
            tokens.DeleteBy = entity.DeleteBy;
            tokens.UpdateBy = _userHelper.GetUserName();
            tokens.UpdateAt = DateTime.Now;

            // var filter = Builders<Tokens>.Filter.Eq(e => e.Id, id);
            _collection.ReplaceOne<Tokens>(o => o.Id == id, tokens);

            return tokens;
        }
    }
}
