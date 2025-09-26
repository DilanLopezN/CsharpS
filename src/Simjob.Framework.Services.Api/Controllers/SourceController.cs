using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Simjob.Framework.Application.Controllers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Util;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Infra.Identity.Models.Module;
using System.Collections.Generic;
using System;
using Simjob.Framework.Infra.Identity.Models.Source;
using Simjob.Framework.Infra.Identity.Entities;
using DotLiquid;
using Simjob.Framework.Infra.Identity.Contexts;
using System.Linq;

namespace Simjob.Framework.Services.Api.Controllers
{
    [Authorize(Policy = "ApiKeyPolicy")]
    public class SourceController : BaseController
    {
        private readonly ISourceService _sourceService;
        protected readonly IMongoCollection<Source> _collection;
        protected readonly SourceContext Context;

        public SourceController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, ISourceService sourceService, SourceContext context) : base(bus, notifications)
        {
            _sourceService = sourceService;
            _collection = context.GetUserCollection();
            Context = context;
        }

        /// <summary>
        /// Insert new Module
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPost()]
        public IActionResult Insert([FromBody] CreateSourceModel model)
        {
            var source = _sourceService.Register(model);

            return ResponseDefault(source);
        }


        /// <summary>
        /// Update Module
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPut()]
        public IActionResult Update([FromBody] UpdateSourceModel model)
        {
            var source = _sourceService.Update(model);

            return ResponseDefault(source);
        }


        /// <summary>
        /// get  Module
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var source = _sourceService.GetById(id);

            return ResponseDefault(source);
        }

        /// <summary>
        /// Enable a module
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPatch("Enable/{id}")]
        public IActionResult Enable(string id)
        {
            _sourceService.Enable(id);

            return ResponseDefault();
        }

        /// <summary>
        /// Disable a module
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPatch("Disable/{id}")]
        public IActionResult Disable(string id)
        {
            _sourceService.Disable(id);

            return ResponseDefault();
        }


        /// <summary>
        /// get  Module
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpGet("GetPaginated")]
        public IActionResult GetAllPaginated(int? page, int? limit, string? name)
        {
            if (page == null) page = 1;
            if (limit == null) limit = 10;
            var filterName = string.IsNullOrEmpty(name) ? Builders<Source>.Filter.Empty : Builders<Source>.Filter.Regex(u => u.User, new BsonRegularExpression($"/^.*{name}.*$/i"));
            var filterIsDeleted = Builders<Source>.Filter.Eq(u => u.IsDeleted, false);

            var listBusca = _collection.Find(filterIsDeleted & filterName).ToList();
            var res = listBusca.Skip((int)((page - 1) * limit)).Take((int)limit);

            List<ReturnModuleModel> schemasReturn = new List<ReturnModuleModel>();

            var retornoMontado = res.Select(x => new
            {
                x.Id,
                x.Host,
                x.User,
                x.Password,
                x.Port,
                x.DbName,
                x.Dialect,
                x.Active
            }).ToList();

            long count = Convert.ToInt64(listBusca.Count());
            return ResponseDefault(new PaginationData<dynamic>(retornoMontado.OrderBy(x => x.User).ToList(), page, limit, count));
        }

    }
}
