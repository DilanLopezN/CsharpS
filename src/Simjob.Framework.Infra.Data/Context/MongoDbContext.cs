using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Simjob.Framework.Domain.Data;
using Simjob.Framework.Domain.Interfaces.Users;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Data.Context
{
    [ExcludeFromCodeCoverage]
    public class MongoDbContext : DbContext
    {

        protected readonly MongoClient Client;
        private readonly IUserHelper _userHelper;
        public MongoDbContext(IUserHelper userHelper)
        {
            var config = new ConfigurationBuilder()
               .AddEnvironmentVariables()
               .AddJsonFile("appsettings.json")
               .Build();

            AddConvetionsPack();
            var mongoUrl = new MongoUrl(config.GetConnectionString("MongoDb"));
            Client = new MongoClient(mongoUrl);
            _userHelper = userHelper;
        }

        public override IMongoDatabase GetDatabase()
        {
            return Client.GetDatabase(_userHelper.GetTenanty() ?? "_empty");
        }




        [ExcludeFromCodeCoverage]
        private void AddConvetionsPack()
        {
            var conventionPack = new ConventionPack {
                new CamelCaseElementNameConvention(),
                new EnumRepresentationConvention(MongoDB.Bson.BsonType.String),
                };

            ConventionRegistry.Register("camelCase", conventionPack, t => true);

            ConventionRegistry.Register("IgnoreExtraElements", new ConventionPack { new IgnoreExtraElementsConvention(true) }, type => true);

        }

        [ExcludeFromCodeCoverage]
        // Executa View
        public dynamic RunCommand(string command, string tenanty)
        {
            IMongoDatabase database;
            if (tenanty != null)
            {
                database = GetDatabaseByTenanty(tenanty);
            }
            else
            {
                database = GetDatabase();
            }


            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.CanonicalExtendedJson };
            var commandJson = new JsonCommand<BsonDocument>(command);

            // teste //
            //var response = database.RunCommand(commandJson);
            // teste //
            //var response = database.RunCommand(commandJson);
            //response.Remove("Timestamp");
            //response.Remove("operationTime");
            //response.Remove("$clusterTime");
            //var res = BsonSerializer.Deserialize<dynamic>(response);

            //var teste = new JsonCommand<BsonDocument>(command);
            //var col = database.GetCollection<BsonDocument>("localcity");
            //var docs = col.Find(new BsonDocument((BsonDocument)command)).Limit(200).ToList();
            var response =  database.RunCommand(new JsonCommand<BsonDocument>(command));

            response.Remove("Timestamp");
            response.Remove("operationTime");
            response.Remove("$clusterTime");
            var res = BsonSerializer.Deserialize<dynamic>(response);
            var des = res;
            var desDic = (IDictionary<string, object>)(ExpandoObject)des;
            var cursorExists = desDic.ContainsKey("cursor");
            if (cursorExists && des.cursor.firstBatch != null)
                des = (dynamic)des.cursor.firstBatch;

            return des;

        }

        [ExcludeFromCodeCoverage]
        public async Task<dynamic> RunCommandAsync(string command)
        {
            var database = GetDatabase();
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.CanonicalExtendedJson };
            var commandJson = new JsonCommand<BsonDocument>(command);

            var res = BsonSerializer.Deserialize<dynamic>(await database.RunCommandAsync(new JsonCommand<BsonDocument>(command)));//.ToJson(jsonWriterSettings);
            var des = res;

            var desDic = (IDictionary<string, object>)(ExpandoObject)des;
            var cursorExists = desDic.ContainsKey("cursor");

            if (cursorExists && des.cursor.firstBatch != null)
                des = (dynamic)des.cursor.firstBatch;

            return des;
        }

        public async Task<List<T>> RunCommandAsync<T>(string command)
        {
            var database = GetDatabase();
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.CanonicalExtendedJson };
            var commandJson = new JsonCommand<BsonDocument>(command);

            var res = BsonSerializer.Deserialize<dynamic>(await database.RunCommandAsync(new JsonCommand<BsonDocument>(command)));
            var des = res;

            var desDic = (IDictionary<string, object>)(ExpandoObject)des;
            var cursorExists = desDic.ContainsKey("cursor");

            if (cursorExists && des.cursor.firstBatch != null)
                des = (dynamic)des.cursor.firstBatch;

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(des);

            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(json);
        }

        public List<T> RunCommandNew<T>(string command)
        {
            var database = GetDatabase();
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.CanonicalExtendedJson };
            var commandJson = new JsonCommand<BsonDocument>(command);

            var res = BsonSerializer.Deserialize<dynamic>(database.RunCommand(new JsonCommand<BsonDocument>(command)));
            var des = res;

            var desDic = (IDictionary<string, object>)(ExpandoObject)des;
            var cursorExists = desDic.ContainsKey("cursor");

            if (cursorExists && des.cursor.firstBatch != null)
                des = (dynamic)des.cursor.firstBatch;

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(des);

            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(json);
        }

        public override IMongoDatabase GetDatabaseByTenanty(string tenanty)
        {
            return Client.GetDatabase(tenanty);
        }
    }
}
