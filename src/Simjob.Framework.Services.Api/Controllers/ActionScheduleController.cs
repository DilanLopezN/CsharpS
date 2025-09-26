using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simjob.Framework.Application.Controllers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Infra.Data.Context;
using System;
using ApiEntities = Simjob.Framework.Services.Api.Entities;

namespace Simjob.Framework.Services.Api.Controllers
{
    [Authorize]
    public class ActionScheduleController : BaseController
    {
        private readonly IRepository<MongoDbContext, ApiEntities.ActionSchedule> _actionScheduleRepository;
        public ActionScheduleController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, IRepository<MongoDbContext, ApiEntities.ActionSchedule> actionScheduleRepository) : base(bus, notifications)
        {
            _actionScheduleRepository = actionScheduleRepository;
        }


        /// <summary>
        /// Update Action
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPut("{id}")]
        public IActionResult Update([FromRoute] string id)
        {
            var action = _actionScheduleRepository.GetById(id);
            action.IsExecuted = true;
            _actionScheduleRepository.Update(action);
            //action.Timer = 10000;
            action.IsExecuted = false;
            action.NextActionRun = DateTime.Now.AddMinutes(action.Timer);
            action.Id = null;
            action.UpdateAt = null;
            action.UpdateBy = null;

            //var exists = _actionScheduleRepository.GetByIdSchedule(action.ActionId);
            //if (exists == null)
            _actionScheduleRepository.Insert(action);
            return ResponseDefault(action);
        }

        /// <summary>
        /// Get All ActionSchedule
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpGet]
        public IActionResult GetAllSchedule()
        {
            return ResponseDefault(_actionScheduleRepository.GetAllSchedule());
        }
    }
}
