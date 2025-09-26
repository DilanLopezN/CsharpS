using Azure.Storage.Blobs.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using MongoDB.Bson;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using Simjob.Framework.Application.Controllers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Entities;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Core.Utils;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Commands.AccessGroup;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Infra.Identity.Models;
using Simjob.Framework.Infra.Schemas.Commands.Entities;
using Simjob.Framework.Infra.Schemas.Entities;
using Simjob.Framework.Infra.Schemas.Interfaces;
using Simjob.Framework.Services.Api.ViewModels.Asap;
using Simjob.Framework.Services.Api.ViewModels.User;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Schema;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using static MongoDB.Driver.WriteConcern;

namespace Simjob.Framework.Services.Api.Controllers
{
    [Authorize(Policy = "ApiKeyPolicy")]
    public class AsapTaskController : BaseController
    {
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;
        private readonly IEntityService _entityService;
        private readonly IUserService _userService;
        private readonly IUserAccessService _userAccessService;

        public AsapTaskController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, IRepository<MongoDbContext, Schema> schemaRepository, IEntityService entityService, IUserService userService, IUserAccessService userAccessService) : base(bus, notifications)
        {
            _schemaRepository = schemaRepository;
            _entityService = entityService;
            _userService = userService;
            _userAccessService = userAccessService;
        }

        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] InsertAsapTaskModel model)
        {
            var usuario = _userService.GetUserById(model.UserId);
            if (usuario == null) return NotFound("Usuario não encontrado");
            //Insert
            var schemaName = "AsapTask";
            var data = model.ToDictionary();
            if (data.Count() == 0) return BadRequest();
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            if (schema == null) return BadRequest();
            var command = new InsertEntityCommand()
            {
                SchemaName = schemaName,
                SchemaJson = schema.JsonValue,
                Data = data
            };
            await SendCommand(command);

            //Disparo de notificação

            //AsapType
            var asapTypeSchemaName = "AsapType";
            var asapType = await _entityService.SerachFields(null, asapTypeSchemaName, "code", model.AsapTypeCode, null, 1, 1, null, new List<bool> { false });
            if (asapType == null) return BadRequest();
            Type type = asapType.GetType();
            PropertyInfo info = type.GetProperty("Data");
            var obj = (dynamic)info.GetValue(asapType, null);
            List<string> plataformas = ((IEnumerable<object>)obj[0].Platform).Cast<string>().ToList();

            //AsapConfigSender
            var asapConfigSenderSchemaName = "AsapConfigSender";
            foreach (var p in plataformas)
            {
                dynamic configSender = await _entityService.GetById(asapConfigSenderSchemaName, p);
                string platform = configSender.Platform.ToString();
                switch (platform)
                {
                    case "EMAIL":
                        var email = usuario.UserName;
                        if (email.IsNullOrEmpty()) continue;
                        var sendEmailCommand = new SendEmailCommand()
                        {
                            To = email,
                            Subject = model.Subject,
                            HtmlContent = model.Body
                        };
                        if (configSender.Config != null)
                        {
                            var config = JsonConvert.DeserializeObject<ConfigSendGrid>(configSender.Config);
                            _userAccessService.SendEmail(config, sendEmailCommand);
                        }
                        break;

                    case "SMS":
                        if (configSender.Config != null)
                        {
                            var configSms = JsonConvert.DeserializeObject<ConfigSMS>(configSender.Config);
                            EnviaSMS(configSms, usuario.Telefone);
                        }
                        break;
                }
            }

            return ResponseDefault();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int? page, int? limit, string sortField = null, bool sortDesc = false)
        {

            return ResponseDefault();
            string userId = "";
            var campo = "userId";
            var schemaName = "AsapTask";
            try
            {
                var accessToken = Request.Headers[HeaderNames.Authorization];
                var tokenInfo = Util.GetUserInfoFromToken(accessToken);

                if (tokenInfo.Count > 0)
                {
                    userId = tokenInfo["userid"];
                }
            }
            catch (Exception)
            {
                throw;
            }

            dynamic asapTasks = await _entityService.SerachFields(null, schemaName, campo, userId, null, page, limit, null, new List<bool> { sortDesc }, sortField: sortField);
            if (asapTasks == null || asapTasks.Data == null) return NotFound();
            List<AsapTaskModel> asapTaskList = JsonConvert.DeserializeObject<List<AsapTaskModel>>(JsonConvert.SerializeObject(asapTasks.Data));

            var schemaNameAsapType = "AsapType";
            var asapTypeIds = asapTaskList.Where(x => x.AsapTypeId != null).Select(x => x.AsapTypeId).Distinct().ToList();
            dynamic asapTypeDefault = await _entityService.SerachFields(null, schemaNameAsapType, "Default", "true", null, page, limit, null, new List<bool> { sortDesc }, sortField: sortField);
            if (asapTypeDefault == null || asapTypeDefault.Data == null) return NotFound("asapType ");
            AsapTypeModel asapTypeDefaultModel = JsonConvert.DeserializeObject<AsapTypeModel>(JsonConvert.SerializeObject(asapTypeDefault.Data[0]));

            dynamic asapTypeByIds = await _entityService.GetAll(schemaNameAsapType, null, null, ids: string.Join(",", asapTypeIds));
            List<AsapTypeModel> asapTypesModel = JsonConvert.DeserializeObject<List<AsapTypeModel>>(JsonConvert.SerializeObject(asapTypeByIds.Data));

            var schemaLog = "AsapLogSender";
            var asapLogIds = asapTaskList.Where(x => x.AsapLogSender != null).Select(x => x.AsapLogSender).SelectMany(x => x).ToList();
            dynamic asaplogResult = await _entityService.GetAll(schemaLog, null, null, ids: string.Join(",", asapLogIds));
            var asapSendLogsModel = new List<AsapLogSenderModel>();
            if (asaplogResult != null && asaplogResult.Data != null)
            {
                asapSendLogsModel = JsonConvert.DeserializeObject<List<AsapLogSenderModel>>(JsonConvert.SerializeObject(asaplogResult.Data));
            }

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

            var usersIds = asapSendLogsModel.Select(x => x.UserId);
            var usersResult = _userService.SearchFieldsByTenanty(null, null, tenanty, null, null, null, false, string.Join(",", usersIds));

            var users = new List<UserModel>();
            if (usersResult != null && usersResult.Data != null)
            {
                users = JsonConvert.DeserializeObject<List<UserModel>>(JsonConvert.SerializeObject(usersResult.Data));
            }

            try
            {
                var retornoData = asapTaskList.Select(x =>
                {
                    AsapTypeModel? asapType = x.AsapTypeId == null ? asapTypeDefaultModel : asapTypesModel.FirstOrDefault(y => y.Id == x.AsapTypeId);

                    var logs = new List<AsapLogSenderModel>();
                    if (x.AsapLogSender != null && x.AsapLogSender.Any())
                    {
                        logs = asapSendLogsModel.Where(l => x.AsapLogSender.Contains(l.Id)).ToList();
                    }
                    var asapLogsUser = logs.Select(l =>
                    {
                        var userLog = users.FirstOrDefault(u => u.Id == l.UserId);

                        return new UserLogAsapModel
                        {
                            IdAsapLogSender = l.Id,
                            SentDate = l.SentDate,
                            UserId = l.UserId,
                            UserName = userLog.UserName,
                            Name = userLog.Name
                        };
                    });

                    var icon = "";
                    if (asapType != null) icon = asapType.Icon;
                    x.AsapTypeIcon = icon;
                    x.UserLogs = asapLogsUser.ToList();
                    return x;
                }
                ).ToList();

                var retorno = new
                {
                    Data = retornoData.Select(x => new
                    {
                        x.Id,
                        x.AsapTypeId,
                        x.AsapTypeIcon,
                        x.Code,
                        x.Subject,
                        x.Body,
                        x.UserId,
                        x.Priority,
                        x.SentDate,
                        x.Status,
                        x.View,
                        x.CreateAt,
                        x.CreateBy,
                        x.UpdateAt,
                        x.UpdateBy,
                        x.DeleteAt,
                        x.DeleteBy,
                        x.IsDeleted,
                        AsapLogSender = x.UserLogs,
                    }),
                    Limit = asapTasks.Limit,
                    Page = asapTasks.Page,
                    Pages = asapTasks.Pages,
                    Total = asapTasks.Total,
                };

                return ResponseDefault(retorno);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPatch]
        [Route("status")]
        public async Task<IActionResult> UpdateStatus(string id, int? value)
        {
            var schemaName = "AsapTask";

            var asaptask = await _entityService.GetById(schemaName, id);

            var asapDict = asaptask.GetType().GetProperties()
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(asaptask));

            var newValue = 1;
            if (value != null && (value == 1 || value == 0)) newValue = value.Value;

            asapDict["status"] = newValue;

            await _entityService.Update(schemaName, asapDict);

            return Ok();
        }

        [HttpPatch]
        [Route("view")]
        public async Task<IActionResult> UpdateView(string id, int? value)
        {
            var schemaName = "AsapTask";

            var asaptask = await _entityService.GetById(schemaName, id);

            var asapDict = asaptask.GetType().GetProperties()
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(asaptask));

            var newValue = 1;
            if (value != null && (value == 1 || value == 0)) newValue = value.Value;

            asapDict["view"] = newValue;

            await _entityService.Update(schemaName, asapDict);

            return Ok();
        }

        [Route("Log")]
        [HttpPost]
        public async Task<IActionResult> InsertLog(string userId, string asapTaskId)
        {
            var schemaName = "AsapLogSender";
            string idLog = Guid.NewGuid().ToString();
            var model = new TaskLogModel
            {
                UserId = userId,
                SentDate = DateTime.Now,
                Id = idLog
            };

            var data = model.ToDictionary();
            if (data.Count() == 0) return BadRequest();
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            if (schema == null) return BadRequest();
            var command = new InsertEntityCommand()
            {
                SchemaName = schemaName,
                SchemaJson = schema.JsonValue,
                Data = data
            };
            await SendCommand(command);

            var schemaNameAsaptask = "AsapTask";

            var asaptask = await _entityService.GetById(schemaNameAsaptask, asapTaskId);

            var asapDict = asaptask.GetType().GetProperties()
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(asaptask));

            dynamic logExistentes = asapDict["AsapLogSender"];
            if (logExistentes != null)
            {
                logExistentes.Add(idLog);
                asapDict["AsapLogSender"] = logExistentes;
            }
            else asapDict["AsapLogSender"] = new List<string> { idLog };

            await _entityService.Update(schemaNameAsaptask, asapDict);

            return Ok();
        }

        private void EnviaSMS(ConfigSMS config, string numero)
        {
            TwilioClient.Init(config.AccountSid, config.AuthToken);

            // Envia a mensagem
            var message = MessageResource.Create(
                to: new PhoneNumber(numero),
                from: new PhoneNumber(config.TwilioPhoneNumber),
                body: "Teste Twilio e C#."
            );
        }
    }
}