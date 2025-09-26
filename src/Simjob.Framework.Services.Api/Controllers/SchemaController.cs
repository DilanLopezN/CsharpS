using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Simjob.Framework.Application.Controllers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Domain.Models.PublishModels;
using Simjob.Framework.Domain.Util;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Schemas.Commands;
using Simjob.Framework.Infra.Schemas.Entities;
using Simjob.Framework.Infra.Schemas.Interfaces;
using Simjob.Framework.Services.Api.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Simjob.Framework.Services.Api.Controllers
{
    [Authorize]
    public class SchemaController : BaseController
    {
        private readonly IRepository<MongoDbContext, Schema> _repository;        
        private readonly IRepository<SourceContext, Source> _sourceRepository;

        private readonly ISchemaBuilder _schemaBuilder;
        private readonly IEntityService _entityService;

        //public SchemaController(IRepository<MongoDbContext, Schema> repository, IMediatorHandler bus, INotificationHandler<DomainNotification> notifications) : base(bus, notifications)
        //{
        //    _repository = repository;

        //}

        public SchemaController(IRepository<MongoDbContext, Schema> @object, ISchemaBuilder schemaBuilder, IEntityService entityService, IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, IRepository<SourceContext, Source> sourceRepository) : base(bus, notifications)
        {
            _repository = @object;
            _schemaBuilder = schemaBuilder;
            _entityService = entityService;
            _sourceRepository = sourceRepository;
        }

        /// <summary>
        /// Run a MQL command in mongodb 
        /// </summary>
        /// <response code="200">Return result from mql</response>
        [HttpPost("run-mql")]
        public async Task<IActionResult> RunMQL(Dictionary<string, object> data)
        {
            var context = _repository.GetDbContext();
            var response = await context.GetDatabase().RunCommandAsync<BsonDocument>(data["mqlQuery"].ToString());
            response.Remove("Timestamp");
            response.Remove("operationTime");
            response.Remove("$clusterTime");

            var res = BsonSerializer.Deserialize<dynamic>(response);//.ToJson(jsonWriterSettings);
            var des = res;

            var desDic = (IDictionary<string, object>)(ExpandoObject)des;
            var cursorExists = desDic.ContainsKey("cursor");

            if (cursorExists && des.cursor.firstBatch != null)
                des = (dynamic)des.cursor.firstBatch;

            return ResponseDefault(des);
        }

        /// <summary>
        /// Get all schemas pagined
        /// </summary>
        /// <response code="200">Return list of schemas pagined</response>
        [HttpGet]
        public IActionResult GetAll(string? module, int? page, int? limit, string sortField = null, bool sortDesc = false)
        {
            return ResponseDefault(_repository.GetAllModules(module,page, limit, sortField, sortDesc));
        }


        /// <summary>
        /// Get schema by Id(name)
        /// <paramref name="schemaName"/>
        /// </summary>
        /// <response code="200">Return schema</response>
        [HttpGet("{schemaName}")]
        public IActionResult GetById(string schemaName)
        {
            var schema = _repository.GetByField("name", schemaName);
            if (schema != null)
            {
                return ResponseDefault(schema);
            }
            return BadRequest();

        }


        /// <summary>
        /// Update schema json
        /// </summary>
        /// <response code="200">Return success</response>
        [HttpPut]
        public async Task<IActionResult> Update(string name, [FromBody] Dictionary<string, object> jsonSchema)
        {

            var command = new UpdateSchemaCommand() { Name = name, JsonValue = JsonConvert.SerializeObject(jsonSchema) };
            await SendCommand(command);

            var schema = _repository.GetByField("name", name);
            var schemaModelData = JsonConvert.DeserializeObject<Simjob.Framework.Infra.Domain.Models.SchemaModel>(schema.JsonValue);

            var indexFields = new List<string>();
            if (!schemaModelData.SearchFields.IsNullOrEmpty()) indexFields.AddRange(schemaModelData.SearchFields.Split(',').ToList());
            if (!schemaModelData.FilterFields.IsNullOrEmpty()) indexFields.AddRange(schemaModelData.FilterFields.Split(',').ToList());
            if (!indexFields.IsNullOrEmpty()) indexFields = indexFields.Distinct().ToList();
            var type = await _schemaBuilder.GetSchemaType(schema.Name);
            var repository = _entityService.GetRepository(type);

            try
            {

                var insertIndexes = repository
                .GetType()
                .GetMethod("SchemaIndexSync")
                .Invoke(repository, new object[] { indexFields });

            }
            catch (Exception ex)
            {
                Logs.AddLog("[SchemaController] - " + ex.Message);
            }
            return ResponseDefault();
        }

        /// <summary>
        /// Insert new schema from designer 
        /// </summary>
        /// <response code="200">Return success</response>
        [HttpPost]
        public async Task<IActionResult> Insert(string name, [FromBody] Dictionary<string, object> jsonSchema, bool strongEntity = true)
        {
            
            if (jsonSchema.ContainsKey("source"))
            {
                var source = _sourceRepository.GetByField("description", jsonSchema["source"].ToString());
                if(source == null) return NotFound($"Source({jsonSchema["source"].ToString()}) not found");
                var tableName = name;
                if(jsonSchema.ContainsKey("alias")) tableName = jsonSchema["alias"].ToString();
               
                //passar o nome da tabela com o nome do alias
                //remover a obrigatoriedade do autoinc ao gerar tabela
                if (source.Active == true) await CreateTable(tableName, jsonSchema, source);
            }

            var command = new InsertSchemaCommand() { Name = name, StrongEntity = strongEntity, JsonValue = JsonConvert.SerializeObject(jsonSchema) };
        await SendCommand(command);

            var schema = _repository.GetByField("name", name);
            var schemaModelData = JsonConvert.DeserializeObject<Simjob.Framework.Infra.Domain.Models.SchemaModel>(schema.JsonValue);

            var indexFields = new List<string>();
            if (!schemaModelData.SearchFields.IsNullOrEmpty()) indexFields.AddRange(schemaModelData.SearchFields.Split(',').ToList());
            if (!schemaModelData.FilterFields.IsNullOrEmpty()) indexFields.AddRange(schemaModelData.FilterFields.Split(',').ToList());
            if (!indexFields.IsNullOrEmpty()) indexFields = indexFields.Distinct().ToList();
            var type = await _schemaBuilder.GetSchemaType(schema.Name);
            var repository = _entityService.GetRepository(type);

            try
            {

                var insertIndexes = repository
                .GetType()
                .GetMethod("SchemaIndexSync")
                .Invoke(repository, new object[] { indexFields });

            }
            catch (Exception ex)
            {
                Logs.AddLog("[SchemaController] - " + ex.Message);
            }

            return ResponseDefault(schema.Id);

        }

        private async Task CreateTable(string tableName, Dictionary<string, object> jsonSchema,Source source)
        {      
            switch (source.Dialect)
            {
                case "SQLSERVER":
                    var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};";
                    var tableExists = await SQLServerService.CheckIfTableExists(connectionString,tableName);
                    if (!tableExists)
                    {
                        var sql = SQLServerService.GenerateCreateTableSql(tableName, jsonSchema);
                        await SQLServerService.RunSQLServerCommand(connectionString, sql);
                    }                   
                    break;
                case "MYSQL":
                    break;
                default:
                    break;
            }
           

        }

        private async Task UpdateTable(string tableName, Dictionary<string, object> jsonSchema, Source source)
        {
            var schema = _repository.GetByField("name", tableName);
            //pegar schema aqui
            switch (source.Dialect)
            {
                case "SQLSERVER":
                    var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};";
                    await SQLServerService.AlterTableSqlServer(connectionString,tableName,jsonSchema,jsonSchema);
                    break;
                case "MYSQL":
                    break;
                default:
                    break;
            }
        }

        

        

        /// <summary>
        /// Delete schema by Id
        /// </summary>
        /// <response code="200">Return success</response>
        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            var schema = _repository.GetByField("_id", id);
            if (schema != null)
            {
                var command = new DeleteSchemaCommand { Id = id };
                await SendCommand(command);
                return ResponseDefault();
            }
            return BadRequest();
        }


        /// <summary>
        /// sync all schemas
        /// </summary>
        /// <response code="200">Return success</response>
        [Authorize]
        [HttpPost("schema-index-sync")]
        public async Task<IActionResult> SchemaIndexSync()
        {
            var schemas = await _repository.GetAllList();

            foreach (var schema in schemas)
            {
                var schemaModelData = JsonConvert.DeserializeObject<Simjob.Framework.Infra.Domain.Models.SchemaModel>(schema.JsonValue);

                var indexFields = new List<string>();
                if (!schemaModelData.SearchFields.IsNullOrEmpty()) indexFields.AddRange(schemaModelData.SearchFields.Split(',').ToList());
                if (!schemaModelData.FilterFields.IsNullOrEmpty()) indexFields.AddRange(schemaModelData.FilterFields.Split(',').ToList());
                if (!indexFields.IsNullOrEmpty()) indexFields = indexFields.Distinct().ToList();
                var type = await _schemaBuilder.GetSchemaType(schema.Name);
                var repository = _entityService.GetRepository(type);

                try
                {

                    var insertIndexes = repository
                    .GetType()
                    .GetMethod("SchemaIndexSync")
                    .Invoke(repository, new object[] { indexFields });

                }
                catch (Exception ex)
                {
                    Logs.AddLog("[SchemaController] - " + ex.Message);
                    continue;
                }
            }
            return ResponseDefault();
        }

        /// <summary>
        /// Publish Schema in any tenanty database
        /// </summary>
        /// <response code="200">Return result from mql</response>
        [HttpPost("publish")]
        public async Task<IActionResult> Publish([Required] string url, [Required] string tenanty, [Required] string email, [Required] string password, [Required] string schemaName, [FromBody] Dictionary<string, object> schemaData, bool strongEntity = true)
        {
            HttpClient client = new()
            {
                Timeout = TimeSpan.FromMilliseconds(5000)
            };

            dynamic user = new { Tenanty = tenanty, Email = email, password };

            var json = JsonConvert.SerializeObject(user);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var res = await client.PostAsync(url + "/User/token", data);

            var content = await res.Content.ReadAsStringAsync();
            if (res.IsSuccessStatusCode)
            {
                var accessToken = JsonConvert.DeserializeObject<Token>(content).data.accessToken;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);


                res = await client.GetAsync(url + $"/Schema/{schemaName}");

                content = await res.Content.ReadAsStringAsync();


                var schema = JsonConvert.DeserializeObject<GetSchemaResult>(content);
                bool success = false;
                if (schema.data != null)
                {
                    //updateSchema
                    dynamic body = schemaData;
                    json = JsonConvert.SerializeObject(body);
                    data = new StringContent(json, Encoding.UTF8, "application/json");
                    try { res = await client.PutAsync(url + $"/Schema?name={schemaName}", data); success = res.IsSuccessStatusCode; } catch (Exception) { success = false; }
                    content = await res.Content.ReadAsStringAsync();
                }

                else
                {
                    //createSchema
                    dynamic body = schemaData;
                    json = JsonConvert.SerializeObject(body);
                    data = new StringContent(json, Encoding.UTF8, "application/json");
                    try { res = await client.PostAsync(url + $"/Schema?name={schemaName}&strongEntity={strongEntity}", data); success = res.IsSuccessStatusCode; } catch (Exception) { success = false; }

                    content = await res.Content.ReadAsStringAsync();
                }
                if (!success) return BadRequest();
            }
            return ResponseDefault();
        }
    }
}
