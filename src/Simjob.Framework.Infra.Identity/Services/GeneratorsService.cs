using MongoDB.Driver;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Infra.Schemas.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Services
{
    public class GeneratorsService : IGeneratorsService
    {
        protected readonly GeneratorsContext Context;
        protected readonly IMongoCollection<Generators> Collection;
        private readonly IRepository<GeneratorsContext, Generators> _generatorsRepository;
        private readonly ISchemaBuilder _schemaBuilder;
        private readonly IServiceProvider _serviceProvider;





        public GeneratorsService(ISchemaBuilder schemaBuilder, IServiceProvider serviceProvider, GeneratorsContext context, IRepository<GeneratorsContext, Generators> generatorsRepository)
        {
            Context = context;
            Collection = context.GetUserCollection();
            _generatorsRepository = generatorsRepository;
            _schemaBuilder = schemaBuilder;
            _serviceProvider = serviceProvider;


        }

        public async Task<object> GetAll(int? page, int? limit, string sortField = null, bool sortDesc = false)
        {
            return _generatorsRepository.GetAll(page, limit, sortField, sortDesc, false);
        }

        public async Task<string> GetAutoincAsync(string schemaName, string field, string mask)
        {
            // Type type = null;
            var code = field;
            var sequencia = "";
            var generator = _generatorsRepository.GetByField("schema", schemaName);
            //var generator = _generatorsRepository.SearchLikeInFieldAutoComplete("schema", schemaName);
            // try { type = await _schemaBuilder.GetSchemaType(schemaName); } catch { return null; }
            if (generator == null)
            {
                if (!string.IsNullOrEmpty(mask))
                {
                    var newMask = mask.Replace("#", "0");
                    sequencia = newMask.Substring(0, mask.Length - 1) + "1";
                    var newGenerator2 = new Generators(schemaName, code, sequencia);
                    _generatorsRepository.Insert(newGenerator2);
                    return sequencia;
                }
                else
                {
                    var newGenerator2 = new Generators(schemaName, code, "1");
                    _generatorsRepository.Insert(newGenerator2);
                    return sequencia;
                }

            }
            else
            {
                var sequencia2 = int.Parse(generator.Sequencia) + 1;
                var pad = sequencia2.ToString().PadLeft(generator.Sequencia.Length, '0');
                var newGenerator2 = new Generators(schemaName, code, pad);
                Update(newGenerator2, generator.Id);
                return pad;
            }



        }
        public Generators GetById(string id)
        {
            var filterId = Builders<Generators>.Filter.Eq(u => u.Id, id);
            var filterIsDeleted = Builders<Generators>.Filter.Eq(u => u.IsDeleted, false);
            return Collection.Find(filterId & filterIsDeleted).FirstOrDefault();
        }

        public Generators GetGeneratorByName(string schemaName)
        {
            return _generatorsRepository.GetByField("name", schemaName);
        }



        public object GetRepository(Type schemaType)
        {
            var typeRepo = typeof(IRepository<,>).MakeGenericType(typeof(MongoDbContext), schemaType);
            return _serviceProvider.GetService(typeRepo);
        }

        public void Register(Generators generators)
        {
            _generatorsRepository.Insert(generators);
        }
        public void Update(Generators generators, string id)
        {
            var generatorsIdFilter = Builders<Generators>.Filter.Eq(u => u.Id, id);
            var generatorsUpdate = Builders<Generators>.Update.Set(u => u.Schema, generators.Schema)
                                                          .Set(u => u.Code, generators.Code)
                                                          .Set(u => u.Sequencia, generators.Sequencia);

            Collection.UpdateOne(generatorsIdFilter, generatorsUpdate);
        }
    }
}
