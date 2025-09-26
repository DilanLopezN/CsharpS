using MongoDB.Driver;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Domain.Models;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Infra.Schemas.Entities;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Simjob.Framework.Infra.Identity.Services
{
    public class SearchdefsService : ISearchdefsService
    {
        protected readonly SearchdefsContext Context;
        protected readonly IMongoCollection<Searchdefs> Collection;
        private readonly IMediatorHandler _bus;
        private readonly IRepository<SearchdefsContext, Searchdefs> _searchdefsRepository;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;

        public SearchdefsService(SearchdefsContext context, IMediatorHandler bus,
            IRepository<SearchdefsContext, Searchdefs> searchdefsRepository, IRepository<MongoDbContext, Schema> schemaRepository)
        {
            Context = context;
            Collection = context.GetUserCollection();
            _bus = bus;
            _searchdefsRepository = searchdefsRepository;
            _schemaRepository = schemaRepository;
        }

        public void DeleteSearchdefs(string id)
        {
            _searchdefsRepository.Delete(id);
        }
        
        [ExcludeFromCodeCoverage]
        public List<Searchdefs> GetSearchdefs(string accessToken)
        {
            var idUser = "";
            if (accessToken != "")
            {                
                var token = accessToken.ToString().Split(" ");
                var onlyToken = "";
                foreach (var itens in token) {if (itens.Length > 10) onlyToken = itens;}                
                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(onlyToken);
                var claims = jwtSecurityToken.Claims.ToList();

                foreach (var claim in claims)
                {
                    if (claim.Type == "userid")
                    {
                        idUser = claim.Value;
                        break;
                    }
                }
            }

            var Seachdefs = Collection.Find(Builders<Searchdefs>.Filter.Eq(u => u.IsDeleted, false) & Builders<Searchdefs>.Filter.Eq(u => u.UserId, idUser)).ToList();

            return Seachdefs;
        }

        public Searchdefs GetSearchdefsById(string id)
        {
            var searchdefs = _searchdefsRepository.GetById(id);

            return searchdefs;
        }

        public void Register(Searchdefs search)
        {                    
            _searchdefsRepository.Insert(search);
        }

        public void UpdateSearchdefs(Searchdefs search, string id)
        {           
            var SearchIdFilter = Builders<Searchdefs>.Filter.Eq(u => u.Id, id);
            var SearchUpdate = Builders<Searchdefs>.Update.Set(u => u.SchemaName, search.SchemaName)
                                                          .Set(u => u.UserId, search.UserId)
                                                          .Set(u => u.Description, search.Description)
                                                          .Set(u => u.Defs, search.Defs);

            Collection.UpdateOne(SearchIdFilter, SearchUpdate);
        }
    }
}