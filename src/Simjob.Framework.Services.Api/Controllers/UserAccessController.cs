using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Simjob.Framework.Application.Controllers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Commands.AccessGroup;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Services.Api.Interfaces;
using System.Threading.Tasks;

namespace Simjob.Framework.Services.Api.Controllers
{
    public class UserAccessController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly IUserAccessService _userAccessService;
        private readonly IRepository<MongoDbContext, UserAccess> _repository;

        public UserAccessController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications,
                                ITokenService tokenService, IRepository<MongoDbContext, UserAccess> repository, IUserService userService, IUserAccessService userAccessService, IConfiguration configuration) : base(bus, notifications)
        {
            _tokenService = tokenService;
            _userService = userService;
            _userAccessService = userAccessService;
            _repository = repository;
            _configuration = configuration;
        }

        /// <summary>
        /// Get UserAccess By ID
        /// </summary>
        /// <returns>Return a User</returns>
        /// <response code="200">Return a User</response>
        [Authorize]
        [HttpGet("getUserAccessById")]
        public IActionResult GetUserAccessById(string id)
        {
            var user = _userAccessService.GetById(id);
            if (user != null) return ResponseDefault(user);
            return BadRequest();
        }

        /// <summary>
        /// GetByUserId
        /// </summary>
        /// <returns>Return a User</returns>
        /// <response code="200">Return a User</response>
        [Authorize]
        [HttpGet("getByUserId")]
        public IActionResult GetByUserId(string userId)
        {
            var user = _userAccessService.GetByUserId(userId);
            if (user != null) return ResponseDefault(user);
            return BadRequest();
        }

        /// <summary>
        /// GetBySchemaRecordId
        /// </summary>
        /// <returns>Return a User</returns>
        /// <response code="200">Return a User</response>
        [Authorize]
        [HttpGet("getBySchemaRecordId")]
        public IActionResult GetBySchemaRecordId(string schemaRecordId)
        {
            var user = _userAccessService.GetBySchemaRecordId(schemaRecordId);
            if (user != null) return ResponseDefault(user);
            return BadRequest();
        }


        /// <summary>
        /// Get all UserAccess pagined from database
        /// </summary>
        /// <returns>Return a list from UserAccess</returns>
        /// <response code="200">Return a list from UserAccess</response>
        [Authorize]
        [HttpGet("getAllUserAccess")]
        public async Task<IActionResult> Get(int? page = null, int? limit = null, string sortField = null, bool sortDesc = false)
        {
            return ResponseDefault(await _userAccessService.GetAll(page, limit, sortField, sortDesc));
        }
        /// <summary>
        /// Register new Sendgrid
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [Authorize]
        [HttpPost("SendEmail")]
        public IActionResult SendEmail([FromBody] SendEmailCommand command)
        {
            if (_userAccessService.SendEmail(command)) return Ok();
            return BadRequest();
        }
    }
}
