using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simjob.Framework.Application.Controllers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Commands;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Simjob.Framework.Services.Api.Controllers
{
    [Authorize]
    public class SharedSchemaRecordController : BaseController
    {


        private readonly ISharedSchemaRecordService _sharedSchemaRecordService;
        private readonly IRepository<MongoDbContext, SharedSchemaRecord> _repository;

        public SharedSchemaRecordController(ISharedSchemaRecordService sharedSchemaRecordService, IRepository<MongoDbContext, SharedSchemaRecord> repository, IMediatorHandler bus, INotificationHandler<DomainNotification> notifications) : base(bus, notifications)
        {
            _sharedSchemaRecordService = sharedSchemaRecordService;
            _repository = repository;
        }

        /// <summary>
        /// Register new SharedSchemaRecord
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>

        [HttpPost("insert")]
        public IActionResult InsertSharedSchemaRecord([FromBody] SharedSchemaRecordCommand command)
        {

            return ResponseDefault(_sharedSchemaRecordService.RegisterAsync(command));

        }


        /// <summary>
        /// Update  SharedSchemaRecord
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>

        [HttpPut("update")]
        public IActionResult UpdateSharedSchemaRecord([FromBody] SharedSchemaRecordCommand command, string id)
        {

            return ResponseDefault(_sharedSchemaRecordService.Update(command, id));

        }

        /// <summary>
        /// Get  SharedSchemaRecord
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>

        [HttpGet("getById")]
        public IActionResult GetSharedSchemaRecordById(string id)
        {

            return ResponseDefault(_sharedSchemaRecordService.GetById(id));

        }



        [HttpGet("{schemaName},{userIdReceive}/getBySchemaNameAndUserIdReceive")]
        public IActionResult GetBySchemaNameAndUserIdReceive(string schemaName, string userIdReceive)
        {

            return ResponseDefault(_sharedSchemaRecordService.GetBySchemaNameAndUserIdReceive(schemaName, userIdReceive));

        }

        /// <summary>
        /// Get all SharedSchemaRecord pagined from database
        /// </summary>
        /// <returns>Return a list from SharedSchemaRecord</returns>
        /// <response code="200">Return a list from SharedSchemaRecord</response>

        [HttpGet("getAll")]
        public async Task<IActionResult> Get(int? page = null, int? limit = null, string sortField = null, bool sortDesc = false)
        {
            return ResponseDefault(await _sharedSchemaRecordService.GetAll(page, limit, sortField, sortDesc));
        }


        /// <summary>
        /// Get all SharedSchemaRecord pagined from database
        /// </summary>
        /// <returns>Return a list from SharedSchemaRecord</returns>
        /// <response code="200">Return a list from SharedSchemaRecord</response>

        [HttpGet("getAllByUserIdReceive")]
        public async Task<IActionResult> GetByUserIdReceive([Required] string userIdReceive, int? page, int? limit, string sortField = null, bool sortDesc = false)
        {
            return ResponseDefault(await _sharedSchemaRecordService.GetByUserIdReceive(page, limit, sortField, sortDesc, userIdReceive));
        }


        /// <summary>
        /// Get all SharedSchemaRecord pagined from database
        /// </summary>
        /// <returns>Return a list from SharedSchemaRecord</returns>
        /// <response code="200">Return a list from SharedSchemaRecord</response>

        [HttpGet("getAllBySchemaRecordIdAndUserIdSender")]
        public async Task<IActionResult> GetBySchemaRecordIdAndUserIdSender([Required] string schemaRecordId, [Required] string userIdSender, int? page, int? limit, string sortField = null, bool sortDesc = false)
        {
            return ResponseDefault(await _sharedSchemaRecordService.GetBySchemaRecordIdAndUserIdSender(page, limit, sortField, sortDesc, schemaRecordId, userIdSender));
        }


        /// <summary>
        /// Delete  SharedSchemaRecord
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>

        [HttpDelete("delete")]
        public IActionResult Delete(string id)
        {
            _sharedSchemaRecordService.Delete(id);

            return ResponseDefault(_sharedSchemaRecordService.GetById(id));
        }

        /// <summary>
        /// Delete  SharedSchemaRecord
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>

        [HttpDelete("deleteByUserIdReceive")]
        public IActionResult DeleteByUserId([Required] string id, [Required] string userIdReceive)
        {
            _sharedSchemaRecordService.DeleteByUserIdReceive(id, userIdReceive);

            return ResponseDefault(_sharedSchemaRecordService.GetById(id));
        }
    }
}
