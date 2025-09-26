//using MediatR;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json;
//using Simjob.Framework.Application.Controllers;
//using Simjob.Framework.Domain.Core.Bus;
//using Simjob.Framework.Domain.Core.Notifications;
//using Simjob.Framework.Domain.Interfaces.Repositories;
//using Simjob.Framework.Infra.Data.Context;
//using Simjob.Framework.Infra.Identity.Entities;
//using Simjob.Framework.Infra.Identity.Interfaces;
//using Simjob.Framework.Infra.Identity.Services;
//using Simjob.Framework.Infra.Schemas.Entities;
//using Simjob.Framework.Infra.Schemas.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Simjob.Framework.Services.Api.Controllers
//{
//    public class GeneratorsController : BaseController
//    {
//        private readonly IGeneratorsService _generatorsService;
//        private readonly IRepository<MongoDbContext, Generators> _repository;
//        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;

//        public GeneratorsController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, IRepository<MongoDbContext, Generators> repository, IGeneratorsService generatorsService, IRepository<MongoDbContext, Schema> schemaRepository) : base(bus, notifications)
//        {
//            _generatorsService = generatorsService;
//            _repository = repository;
//            _schemaRepository = schemaRepository;

//        }

//        /// <summary>
//        /// Get Generators By ID
//        /// </summary>
//        /// <returns>Return a Generator</returns>
//        /// <response code="200">Return a Generator</response>
//        [Authorize]
//        [HttpGet("getGeneratorsById")]
//        public IActionResult GetGeneratorsById(string id)
//        {
//            var generator = _generatorsService.GetById(id);
//            if (generator != null) return ResponseDefault(generator);
//            return BadRequest();
//        }



//        /// <summary>
//        /// Get all Generators pagined from database
//        /// </summary>
//        /// <returns>Return a list from Generators</returns>
//        /// <response code="200">Return a list from Generators</response>
//        [Authorize]
//        [HttpGet("getAllGenerators")]
//        public async Task<IActionResult> Get(int? page = null, int? limit = null, string sortField = null, bool sortDesc = false)
//        {
//            return ResponseDefault(await _generatorsService.GetAll(page, limit, sortField, sortDesc));
//        }

//        /// <summary>
//        /// Get all Generators pagined from database
//        /// </summary>
//        /// <returns>Return a list from Generators</returns>
//        /// <response code="200">Return a list from Generators</response>
//        [Authorize]
//        [HttpGet("getGeneratorAutoInc")]
//        public async Task<IActionResult> GetAutoincAsync(string schemaName, string field, string mask)
//        {
//            if (schemaName == null || field == null) return BadRequest("schemaName e field não podem ser nulos");

            
//            return Ok( await _generatorsService.GetAutoincAsync(schemaName, field, mask));
//        }
//        /// <summary>
//        /// Get all Generators pagined from database
//        /// </summary>
//        /// <returns>Return a list from Generators</returns>
//        /// <response code="200">Return a list from Generators</response>
//        [Authorize]
//        [HttpPost("RegisterGenerator")]
//        public IActionResult Register(string schema, string code, string sequencia)
//        {
//            var autoInc = false;
//            var getschema = _schemaRepository.GetByField("name", schema);
//            if (getschema == null) return NotFound("schema not found");
//            var json = getschema.JsonValue;
//            var schemaProp = JsonConvert.DeserializeObject<SchemaModel>(json).Properties;
//            foreach (var prop in schemaProp)
//            {
//                if (prop.Value.AutoInc == true)
//                {
//                    autoInc = true;
//                    break;
//                }
//            }
//            if(autoInc == true)
//            {

//            }
            
            
//            var newGenerator = new Generators(schema, code, sequencia);
//            _generatorsService.Register(newGenerator);
//            return Ok(newGenerator);

//        }
//    }
    
//}
