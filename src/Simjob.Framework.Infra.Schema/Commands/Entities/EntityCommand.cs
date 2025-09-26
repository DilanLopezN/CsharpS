using MongoDB.Driver;
using Newtonsoft.Json;
using Simjob.Framework.Domain.Core.Commands;
using System.Collections.Generic;
using System.Linq;


namespace Simjob.Framework.Infra.Schemas.Commands.Entities
{
    public abstract class EntityCommand : Command
    {
        public EntityCommand()
        {

        }

        public EntityCommand(string schemaName, Dictionary<string, object> data)
        {
            SchemaName = schemaName;
            Data = data;
        }

        public string Id { get; set; }
        public string SchemaName { get; set; }
        public string SchemaJson { get; set; }
        public Dictionary<string, object> Data { get; set; }


        public override bool IsValid()
        {
            var jsonSchema = NJsonSchema.JsonSchema.
              FromJsonAsync(SchemaJson)
              .GetAwaiter()
              .GetResult();

            string json = JsonConvert.SerializeObject(Data);
            var resValidation = jsonSchema.Validate(json);

            if (resValidation.Any())
                resValidation.ToList()
                    .ForEach(v => ValidationResult
                                .Errors
                                .Add(new FluentValidation.Results.ValidationFailure(v.Property, v.Path)));


            return base.IsValid();
        }
    }
}
