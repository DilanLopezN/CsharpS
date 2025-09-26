using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Simjob.Framework.Application.Controllers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Core.Utils;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Commands;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Infra.Schemas.Entities;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Simjob.Framework.Services.Api.Controllers
{
    public class StatusFlowController : BaseController
    {
        private readonly IStatusFlowService _statusFlowService;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;

        public StatusFlowController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, IStatusFlowService statusFlowService, IRepository<MongoDbContext, Schema> schemaRepository) : base(bus, notifications)
        {
            _statusFlowService = statusFlowService;
            _schemaRepository = schemaRepository;
        }

        /// <summary>
        /// Insert StatusFlow
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPost("insert")]
        [ExcludeFromCodeCoverage]
        [Authorize]
        public IActionResult InsertStatusFlow([FromBody] StatusFlowCommand command)
        {
            var schema = _schemaRepository.GetByField("name", command.SchemaName);
            if (schema == null) return BadRequest("schema não existe");
            _statusFlowService.Register(command);
            var statusFlow = _statusFlowService.GetBySchemaAndField(command.SchemaName, command.Field, command.Tenanty);
            return ResponseDefault(statusFlow);
        }
        /// <summary>
        /// GetBySchemaAndField StatusFlow
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpGet("getBySchemaAndField")]
        [ExcludeFromCodeCoverage]
        [Authorize]
        public IActionResult GetBySchemaAndField(string schemaName, string field)
        {
            var tenanty = "";
            try
            {
                var accessToken = Request.Headers[HeaderNames.Authorization];

                var tokenInfo = Util.GetUserInfoFromToken(accessToken);

                if (tokenInfo.Count > 0)
                {
                    tenanty = tokenInfo["tenanty"];
                }

            }
            catch (Exception)
            {
                throw;
            }
            return ResponseDefault(_statusFlowService.GetBySchemaAndField(schemaName, field, tenanty));

        }

        /// <summary>
        /// DeleteById StatusFlow
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPut("deleteById")]
        [ExcludeFromCodeCoverage]
        [Authorize]
        public IActionResult DeleteById(string id)
        {
            _statusFlowService.DeleteStatusFlow(id);

            return ResponseDefault(_statusFlowService.GetById(id));
        }


    }
}
