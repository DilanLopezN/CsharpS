using MongoDB.Driver;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Infra.Schemas.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Simjob.Framework.Infra.Identity.Services
{
    public class ProfileService : IProfileService
    {
        protected readonly ProfileContext Context;
        protected readonly IMongoCollection<Profile> Collection;
        private readonly IMediatorHandler _bus;
        private readonly IProfileRepository _ProfileRepository;
        //private readonly ISchemaRepository<ProfileContext, Profile> _ProfileRepository;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;

        public ProfileService(ProfileContext context, IMediatorHandler bus,
            IProfileRepository ProfileRepository, IRepository<MongoDbContext, Schema> schemaRepository)
        {
            Context = context;
            Collection = context.GetUserCollection();
            _bus = bus;
            _ProfileRepository = ProfileRepository;
            _schemaRepository = schemaRepository;
        }

        public void DeleteProfile(string id)
        {
            _ProfileRepository.Delete(id);
        }

        public Profile GetByTenanty(string tenanty)
        {
            var profile = _ProfileRepository.GetByTenanty(tenanty);
            return profile;
        }

        public Profile GetProfileById(string id)
        {
            var profile = _ProfileRepository.GetById(id);
            return profile;
        }

        public List<Profile> GetProfiles()
        {
            var profiles = Collection.Find(Builders<Profile>.Filter.Eq(u => u.IsDeleted, false)).ToList();

            return profiles;
        }

        public void Register(Profile profile)
        {
            _ProfileRepository.Insert(profile);
        }

        public void UpdateProfile(Profile profile, string id)
        {
            var profileIdFilter = Builders<Profile>.Filter.Eq(u => u.Id, id);
            var profileUpdate = Builders<Profile>.Update.Set(u => u.NomeEmpresa, profile.NomeEmpresa)
                                                          .Set(u => u.Color, profile.Color)
                                                          .Set(u => u.Banner, profile.Banner)
                                                          .Set(u => u.Tenanty, profile.Tenanty)
                                                          .Set(u => u.Dominio, profile.Dominio);


            Collection.UpdateOne(profileIdFilter, profileUpdate);
        }
        public void UpdateProfileLogo(Profile profile, string id)
        {
            var profileIdFilter = Builders<Profile>.Filter.Eq(u => u.Id, id);
            var profileUpdate = Builders<Profile>.Update.Set(u => u.Logo, profile.Logo).Set(u => u.Tenanty, profile.Tenanty);

            Collection.UpdateOne(profileIdFilter, profileUpdate);
        }
    }
}
