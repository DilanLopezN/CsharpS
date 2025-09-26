using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Simjob.Framework.Application.Controllers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Models;
using Simjob.Framework.Infra.Identity.Commands;
using Simjob.Framework.Infra.Identity.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Simjob.Framework.Services.Api.Controllers
{
    [Authorize]
    public class SearchdefsController : BaseController
    {
        private readonly ISearchdefsService _searchdefsService;

        public SearchdefsController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, ISearchdefsService searchdefsService) : base(bus, notifications)
        {
            _searchdefsService = searchdefsService;
        }

        /// <summary>
        /// Register a new SearchDefs
        /// </summary>
        /// <response code="200">Return success</response>
        [HttpPost("insert")]
        public IActionResult InsertSearchDefs([FromBody] RegisterSearchdefsCommand command)
        {
            var newSearch = new Searchdefs(command.SchemaName, command.UserId, command.Description, command.Def);
            _searchdefsService.Register(newSearch);
            return ResponseDefault(newSearch.Id);
        }

        /// <summary>
        /// Update searchdefs
        /// </summary>
        /// <response code="200">Return SearchDefs updated </response>
        [HttpPut("update")]
        public IActionResult UpdateSearchDefs([FromBody] RegisterSearchdefsCommand command, string id)
        {
            var searchd = _searchdefsService.GetSearchdefsById(id);
            if (searchd != null)
            {
                Searchdefs newSearch = new Searchdefs(command.SchemaName, command.UserId, command.Description, command.Def);
                _searchdefsService.UpdateSearchdefs(newSearch, id);
                return ResponseDefault(id);
            }
            return BadRequest();
        }

        /// <summary>
        /// Delete searchdefs
        /// </summary>
        /// <response code="200">Return success</response>
        [HttpDelete("deleteById")]
        public IActionResult DeleteSearchDefs(string id)
        {

            var searchdefs = _searchdefsService.GetSearchdefsById(id);
            if (searchdefs != null)
            {
                _searchdefsService.DeleteSearchdefs(id);
                return ResponseDefault(searchdefs);
            }
            return BadRequest();


        }

        /// <summary>
        /// Get all searchdefs
        /// </summary>
        /// <response code="200">Return success</response>
        [HttpGet("getAll")]
        [ExcludeFromCodeCoverage]
        public IActionResult Getsearchdefs()
        {
            var acesstoken = Request.Headers[HeaderNames.Authorization];
            return ResponseDefault(_searchdefsService.GetSearchdefs(acesstoken));
        }

        /// <summary>
        /// Get searchdefs by Id
        /// </summary>
        /// <response code="200">Return a object searchdefs</response>
        [HttpGet("getById")]
        public IActionResult Getsearchdefs(string id)
        {
            var objSearch = _searchdefsService.GetSearchdefsById(id);
            if (objSearch != null) return ResponseDefault(objSearch);
            return BadRequest();
        }
    }
}