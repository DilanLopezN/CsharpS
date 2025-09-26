using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Schemas.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Schemas.Contexts
{
    [ExcludeFromCodeCoverage]
    public class ViewContext : MongoDbContext
    {
        private const string USERDATABASENAME = "";
        private const string USERCOLLECTIONNAME = "view";
        protected readonly IUserHelper _userHelper;

        public ViewContext(IUserHelper userHelper) : base(userHelper)
        {
        }        
        public new dynamic RunCommand(string command)
        {
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.CanonicalExtendedJson };
            var commandJson = new JsonCommand<BsonDocument>(command);

            var res = BsonSerializer.Deserialize<dynamic>(GetDatabase().RunCommand(new JsonCommand<BsonDocument>(command)).ToJson(jsonWriterSettings));//;
            var des = res;

            var desDic = (IDictionary<string, object>)(ExpandoObject)des;
            var cursorExists = desDic.ContainsKey("cursor");

            if (cursorExists && des.cursor.firstBatch != null)
                des = (dynamic)des.cursor.firstBatch;


            return des;
        }

        public object RunView(Views view)
        {
            //var commandJson = new JsonCommand<BsonDocument>(view.Query);
            //var command = @"{ aggregate : 'articles', 
            var command = new BsonDocument { { "account", "ARIES INDUSTRIA MECANICA LTDA." } };


            var result = GetDatabase().RunCommand<object>(command);
            //var res = GetDatabase().RunCommand<object>(view.Query.ToString());
            return result;
            //var des = res;


            //var desDic = (IDictionary<string, object>)(ExpandoObject)des;
            //var cursorExists = desDic.ContainsKey("cursor");

            //if (cursorExists && des.cursor.firstBatch != null)
            //    des = (dynamic)des.cursor.firstBatch;

            //return des;
        }

        public IMongoCollection<Views> GetUserCollection()
        {
            return GetDatabase().GetCollection<Views>(USERCOLLECTIONNAME);
        }

        public bool CreateView(PipelineDefinition<BsonDocument, BsonDocument> pipeline)
        {
            try
            {
                GetDatabase().CreateViewAsync("teste12345", "item", pipeline);
                return true;
                //GetDatabase().DropCollection("viewname");

            } catch
            {
                return false;
            }
        }

        public bool ViewExists(string viewname)
        {
            var collections = GetDatabase().ListCollections().ToList();
            var views = collections.Where(x => x.ElementAt(1).Value == "view").ToList();
            var exists = views.Where(x => x.ElementAt(0).Value == viewname).FirstOrDefault();
            if (exists == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override IMongoDatabase GetDatabase()
        {
            return Client.GetDatabase(_userHelper.GetTenanty() ?? "_empty");
        }
    }
}
