using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Simjob.Framework.Application.Controllers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Core.Utils;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Commands;
using Simjob.Framework.Infra.Identity.Commands.AccessGroup;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Infra.Schemas.Commands.Entities;
using Simjob.Framework.Infra.Schemas.Entities;
using Simjob.Framework.Infra.Schemas.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using static Simjob.Framework.Infra.Identity.Entities.Notification;
using ApiEntities = Simjob.Framework.Services.Api.Entities;

namespace Simjob.Framework.Services.Api.Controllers
{
    public class NotificationsController : BaseController
    {
        private readonly INotificationService _notificationService;
        private readonly IApproveLogService _approveLogService;
        private readonly IEntityService _entityService;
        private readonly IStatusFlowService _statusFlowService;
        private readonly IStatusFlowItemService _statusFlowItemService;
        private readonly IUserService _userService;
        private readonly IUserAccessService _userAccessService;
        //private readonly IUserService _userService;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;
        private readonly IRepository<MongoDbContext, ApiEntities.Action> _actionRepository;
        private readonly IConfiguration _configuration;
        public NotificationsController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, INotificationService notificationService, IApproveLogService approveLogService, IEntityService entityService, IStatusFlowService statusFlowService, IUserService userService, IUserAccessService userAccessService, IConfiguration configuration, IRepository<MongoDbContext, Schema> schemaRepository, IRepository<MongoDbContext, ApiEntities.Action> actionRepository, IStatusFlowItemService statusFlowItemService) : base(bus, notifications)
        {
            _notificationService = notificationService;
            _approveLogService = approveLogService;
            _entityService = entityService;
            _statusFlowService = statusFlowService;
            _schemaRepository = schemaRepository;
            _actionRepository = actionRepository;
            _configuration = configuration;
            _userService = userService;
            _userAccessService = userAccessService;
            _statusFlowItemService = statusFlowItemService;
        }
        ///// <summary>
        ///// Get Notification
        ///// </summary>
        ///// <returns>Return success</returns>
        ///// <response code="200">Return success</response>
        //[HttpGet("getNotification")]
        //[ExcludeFromCodeCoverage]
        //[Authorize]
        //public async Task<IActionResult> GetNotificationByUserId(int? page = null, int? limit = null, string sortField = null, bool sortDesc = false)
        //{
        //    var userName = "";
        //    var userId = "";

        //    try
        //    {
        //        var accessToken = Request.Headers[HeaderNames.Authorization];

        //        var tokenInfo = Util.GetUserInfoFromToken(accessToken);

        //        if (tokenInfo.Count > 0)
        //        {
        //            userName = tokenInfo["username"];
        //            userId = tokenInfo["userid"];
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }


        //    //Notification notification = null;
        //    //notification = await _notificationService.GetAllByUserIdAndName(userId,userName);
        //    return ResponseDefault(await _notificationService.GetAllByUserIdAndName(userId, userName, page, limit, sortField, sortDesc));

        //}


        /// <summary>
        /// Get Notification
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpGet("getNotifications")]
        [ExcludeFromCodeCoverage]
        [Authorize]
        public async Task<IActionResult> GetNotificationsByUserId(int? page = null, int? limit = null, string sortField = null, bool sortDesc = false)
        {
            var userName = "";
            var userId = "";

            try
            {
                var accessToken = Request.Headers[HeaderNames.Authorization];

                var tokenInfo = Util.GetUserInfoFromToken(accessToken);

                if (tokenInfo.Count > 0)
                {
                    userName = tokenInfo["username"];
                    userId = tokenInfo["userid"];
                }
            }
            catch (Exception)
            {
                throw;
            }



            return ResponseDefault(await _notificationService.GetNotificationsByUserId(userId, userName, page, limit, sortField, sortDesc));

        }


        /// <summary>
        /// Verify Notification
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpGet("verifyNotification")]
        [ExcludeFromCodeCoverage]
        [Authorize]
        public IActionResult Verify(string schemaRecordId)
        {
            Notification notification = null;
            notification = _notificationService.GetBySchemaRecordId(schemaRecordId);
            return ResponseDefault(notification);

        }
        /// <summary>
        /// Register Notification
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPost("notificationsRegister")]
        [Authorize]
        public async Task<IActionResult> RegisterNotifications([FromBody] RegisterNotificationCommand notificationCommand)
        {
            var userName = "";
            try
            {
                var accessToken = Request.Headers[HeaderNames.Authorization];

                var tokenInfo = Util.GetUserInfoFromToken(accessToken);

                if (tokenInfo.Count > 0)
                {
                    userName = tokenInfo["username"];
                }

            }
            catch (Exception)
            {
                throw;
            }

            var newNotification = new Notification(notificationCommand.Msg, notificationCommand.Obs, notificationCommand.UserId, false);
            newNotification.CreateBy = userName;
            var notification = _notificationService.RegisterAsync(newNotification);
            return ResponseDefault(notification);
        }
        /// <summary>
        /// Update Notification
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPut("notificationsUpdateAprov{schemaRecordId}")]
        [Authorize]
        public IActionResult UpdateNotificationsAprov(string schemaRecordId)
        {
            Notification notificationOriginal = null;
            notificationOriginal = _notificationService.GetBySchemaRecordId(schemaRecordId);
            if (notificationOriginal == null) return ResponseDefault("Notificação não existe");
            var dataAtual = DateTime.Now;
            notificationOriginal.View = false;
            notificationOriginal.Aprov = null;
            notificationOriginal.AprovEmail = notificationOriginal.AprovEmail.Select(x =>
            {
                x.aprov = null;
                x.view = false;
                return x;
            }).ToArray();
            _notificationService.UpdateNotification(notificationOriginal, notificationOriginal.Id);

            return ResponseDefault(notificationOriginal);
        }


        /// <summary>
        /// Update Notification
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPut("notificationsUpdate")]
        [Authorize]
        public async Task<IActionResult> UpdateNotifications([FromBody] UpdateNotificationCommand notificationCommand)
        {
            StatusFlow.ListProperties existeProximo = null;
            Notification notificationOriginal = null;
            if (notificationCommand.NotificationId == null)
            {
                notificationOriginal = _notificationService.GetBySchemaRecordId(notificationCommand.SchemaRecordId);
            }
            else
            {
                notificationOriginal = _notificationService.GetById(notificationCommand.NotificationId);
            }



            if (notificationOriginal == null) return ResponseDefault("Notificação não existe");

            var userName = "";
            var tenanty = "";
            try
            {
                var accessToken = Request.Headers[HeaderNames.Authorization];

                var tokenInfo = Util.GetUserInfoFromToken(accessToken);

                if (tokenInfo.Count > 0)
                {
                    userName = tokenInfo["username"];
                    tenanty = tokenInfo["tenanty"];
                }

            }
            catch (Exception)
            {
                throw;
            }

            var searchAprov = notificationOriginal.AprovEmail.Where(x => x.email == userName).FirstOrDefault();
            if (searchAprov != null)
            {
                notificationOriginal.AprovEmail.Where(x => x.email == userName).FirstOrDefault().aprov = notificationCommand.Aprov;
                notificationOriginal.AprovEmail.Where(x => x.email == userName).FirstOrDefault().view = true;
            }

            var userAprovTrue = notificationOriginal.AprovEmail.Where(x => x.aprov == true).Count();
            var userAprovFalse = notificationOriginal.AprovEmail.Where(x => x.aprov == false).Count();
            var checkAprovs = notificationOriginal.AprovEmail.Where(x => x.aprov == null).Count();

            if (userAprovTrue >= notificationOriginal.AprovMin)
            {
                notificationOriginal.Aprov = true;
                notificationOriginal.AprovEmail.ToList().ForEach(x => x.view = true);

                //Atualizar o Entity para o próximo status e gerar notificação com base no proximoStatus
                var schemaRepository = _schemaRepository.GetByField("name", notificationOriginal.SchemaName);


                var statusFlow = _statusFlowService.GetBySchemaAndField(notificationOriginal.SchemaName, notificationOriginal.Field, tenanty);
                if (statusFlow != null)
                {
                    var proximoStatus = statusFlow.Properties.Where(x => x.Status == notificationOriginal.Value).FirstOrDefault();
                    if (proximoStatus != null)
                    {
                        existeProximo = statusFlow.Properties.Where(x => x.Status == proximoStatus.ProximoStatus).FirstOrDefault();
                    }

                    var entity = await _entityService.GetById(notificationOriginal.SchemaName, notificationOriginal.SchemaRecordId);
                    var json = JsonConvert.SerializeObject(entity);
                    var settings = new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    };

                    var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json, settings);
                    dict[notificationOriginal.Field] = proximoStatus.ProximoStatus;
                    var obsMensagem = $"{DateTime.Now} - {notificationCommand.Justificativa} ({userName})";
                    if (!dict.ContainsKey("justificativa")) dict.Add("justificativa", obsMensagem);
                    else dict["justificativa"] = $"{obsMensagem}";
                    var command = new UpdateEntityCommand()
                    {
                        Id = schemaRepository.Id,
                        SchemaName = notificationOriginal.SchemaName,
                        SchemaJson = schemaRepository.JsonValue,
                        Data = dict
                    };
                    await SendCommand(command);
                    var camelCaseDict = new Dictionary<string, object>();
                    foreach (var kvp in dict)
                    {
                        string camelCaseKey = kvp.Key.Replace(kvp.Key[0], char.ToLower(kvp.Key[0]));
                        camelCaseDict[camelCaseKey] = kvp.Value;
                    }
                    if (existeProximo != null)
                    {
                        var proximoStatusFlow = statusFlow.Properties.Where(x => x.Status == existeProximo.Status).FirstOrDefault();
                        List<string> aprovadoresList = new List<string>();
                        var listEmailAprov = new List<AprovEmailProperties>();
                        if (!proximoStatusFlow.Action.IsNullOrEmpty() && proximoStatusFlow.Type == "Action")
                        {
                            var statusFlowAction = _actionRepository.GetByField("name", proximoStatusFlow.Action);

                            if (statusFlowAction != null)
                            {
                                ActionController x = new(null, null, _schemaRepository, _entityService, _actionRepository, null, _userService);
                                ApiEntities.Action[] ac = x.GetBySchemaName(notificationOriginal.SchemaName);

                                var userHelper = _schemaRepository.GetUserHelper();
                                var user = _userService.GetByUserName(userHelper.GetTenanty(), userHelper.GetUserName());

                                string auth = Request.Headers["Authorization"];

                                dynamic result = await x.Run(statusFlowAction, new Dictionary<string, object>()
                                        {
                                            { "params", new { entity = camelCaseDict } }
                                        }, auth, user, null);

                                aprovadoresList = result.aprovadores.ToObject<List<string>>();

                                if (!aprovadoresList.IsNullOrEmpty())
                                {
                                    if (!aprovadoresList.Contains("admin@admin.com")) aprovadoresList.Add("admin@admin.com");
                                    aprovadoresList = aprovadoresList.Distinct().ToList();

                                    foreach (var email in aprovadoresList)
                                    {
                                        listEmailAprov.Add(new AprovEmailProperties { email = email, aprov = null, view = false, data = null });
                                    }
                                }



                            }
                            else
                            {
                                return BadRequest();
                            }
                        }
                        else
                        {
                            var aprovEmails = proximoStatusFlow.AprovEmail;

                            var users = _userService.GetUsersByUserName(aprovEmails, tenanty);
                            if (!users.IsNullOrEmpty())
                            {
                                var userEmails = users.Select(x => x.UserName).ToList();
                                if (!userEmails.IsNullOrEmpty())
                                {
                                    if (!userEmails.Contains("admin@admin.com")) userEmails.Add("admin@admin.com");

                                    foreach (var email in userEmails)
                                    {
                                        listEmailAprov.Add(new AprovEmailProperties { email = email, aprov = null, view = false, data = null });
                                    }
                                }

                            }
                        }
                        var statusFlowItemOriginal = _statusFlowItemService.GetBySchemaRecordId(notificationCommand.SchemaRecordId);
                        if (statusFlowItemOriginal != null)
                        {

                            statusFlowItemOriginal.StatusFlowId = statusFlow.Id;
                            statusFlowItemOriginal.Status = existeProximo.Status;
                            statusFlowItemOriginal.AprovEmail = listEmailAprov.Select(x => x.email).ToArray();
                            _statusFlowItemService.UpdateStatusFlowItem(statusFlowItemOriginal, statusFlowItemOriginal.Id);

                        }
                        if (listEmailAprov.Count() > 0)
                        {
                            var newNotification = new Notification(notificationOriginal.SchemaRecordId, notificationOriginal.SchemaName, notificationOriginal.Field, existeProximo.Status, notificationOriginal.Value, "Mudança de status para " + existeProximo.Status, null, notificationOriginal.UserId, false, notificationOriginal.AprovMin, listEmailAprov.ToArray(), null);
                            newNotification.CreateBy = userName;

                            _notificationService.RegisterAsync(newNotification);
                        }

                    }
                }
            }

            //if (userAprovFalse >= notificationOriginal.AprovMin)
            //{
            //    notificationOriginal.Aprov = false;
            //}
            if ((checkAprovs == 0 || userAprovFalse >= notificationOriginal.AprovMin || (checkAprovs + userAprovTrue < notificationOriginal.AprovMin)) && notificationOriginal.AprovMin > userAprovTrue)
            {
                var statusFlow = _statusFlowService.GetBySchemaAndField(notificationOriginal.SchemaName, notificationOriginal.Field, tenanty);
                if (statusFlow != null)
                {
                    var status = statusFlow.Properties.Where(x => x.Status == notificationOriginal.Value).FirstOrDefault();
                    var schemaRepository = _schemaRepository.GetByField("name", notificationOriginal.SchemaName);

                    notificationOriginal.Aprov = false;
                    notificationOriginal.AprovEmail.ToList().ForEach(x => x.view = true);

                    var entity = await _entityService.GetById(notificationOriginal.SchemaName, notificationOriginal.SchemaRecordId);
                    var json = JsonConvert.SerializeObject(entity);
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    //dict[notificationOriginal.Field] = notificationOriginal.ValueOld != null ? notificationOriginal.ValueOld : notificationOriginal.Value;
                    dict[notificationOriginal.Field] = notificationOriginal.Value;
                    var obsMensagem = $"{DateTime.Now} - {notificationCommand.Justificativa} ({userName})";
                    if (!dict.ContainsKey("justificativa")) dict.Add("justificativa", obsMensagem);
                    else dict["justificativa"] = $"{obsMensagem}";


                    var command = new UpdateEntityCommand()
                    {
                        Id = schemaRepository.Id,
                        SchemaName = notificationOriginal.SchemaName,
                        SchemaJson = schemaRepository.JsonValue,
                        Data = dict
                    };
                    await SendCommand(command);

                    statusFlow = _statusFlowService.GetBySchemaAndField(notificationOriginal.SchemaName, status.ReprovaStatus, tenanty);
                    if (statusFlow != null)
                    {
                        var dataAprov = DateTime.Now;
                        var listEmailAprov = new List<AprovEmailProperties>();

                        //var aprovEmails = statusFlow.Properties.Where(x => x.Status == notificationOriginal.Value).FirstOrDefault().AprovEmail;
                        var emailReprova = notificationOriginal.CreateBy;
                        foreach (var aprovs in notificationOriginal.AprovEmail)
                        {
                            listEmailAprov.Add(new AprovEmailProperties { email = aprovs.email, aprov = null, view = false, data = null });
                        }


                        var getUser = _userService.GetByUserName(tenanty, emailReprova);
                        var newNotification = new Notification(notificationOriginal.SchemaRecordId, notificationOriginal.SchemaName, notificationOriginal.Field, notificationOriginal.ValueOld, notificationOriginal.Value, "Mudança de status reprovada", obsMensagem, getUser.Id, false, notificationOriginal.AprovMin, listEmailAprov.ToArray(), null);
                        newNotification.CreateBy = userName;
                        _notificationService.RegisterAsync(newNotification);

                    }


                }


            }
            var dataAtual = DateTime.Now;
            notificationOriginal.AprovEmail.Where(x => x.email == userName).FirstOrDefault().data = dataAtual;
            _notificationService.UpdateNotification(notificationOriginal, notificationOriginal.Id);
            string aprovacao = notificationCommand.Aprov ? "aprovou" : "reprovou";
            string mensagem = $"Usuário {userName} {aprovacao} o status {notificationOriginal.Value}";

            //Sendgrid
            var sendEmailCommand = new SendEmailCommand();
            sendEmailCommand.Subject = aprovacao;
            sendEmailCommand.To = notificationOriginal.CreateBy;
            sendEmailCommand.PlainTextContent = mensagem;
            sendEmailCommand.HtmlContent = mensagem;
            _userAccessService.SendEmail(sendEmailCommand);
            if (notificationOriginal.AprovEmail.Length > 1)
            {
                if (existeProximo != null)
                {
                    var log = new ApproveLog(notificationOriginal.SchemaRecordId, notificationOriginal.SchemaName, notificationOriginal.Field, existeProximo.Status, mensagem);
                    _approveLogService.Register(log);
                }
                if (notificationOriginal.Aprov == false)
                {
                    var log = new ApproveLog(notificationOriginal.SchemaRecordId, notificationOriginal.SchemaName, notificationOriginal.Field, notificationOriginal.Value, mensagem);
                    _approveLogService.Register(log);
                }
            }

            return ResponseDefault(notificationOriginal);

        }

        /// <summary>
        /// Update Notification
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPut("notificationsUpdateMany")]
        [Authorize]
        public async Task<IActionResult> UpdateManyNotifications([FromBody] List<UpdateNotificationCommand> notificationCommandList)
        {
            foreach (var notificationCommand in notificationCommandList)
            {
                StatusFlow.ListProperties existeProximo = null;
                Notification notificationOriginal = null;
                if (notificationCommand.NotificationId == null)
                {
                    notificationOriginal = _notificationService.GetBySchemaRecordId(notificationCommand.SchemaRecordId);
                }
                else
                {
                    notificationOriginal = _notificationService.GetById(notificationCommand.NotificationId);
                }



                if (notificationOriginal == null) return ResponseDefault("Notificação não existe");

                var userName = "";
                var tenanty = "";
                try
                {
                    var accessToken = Request.Headers[HeaderNames.Authorization];

                    var tokenInfo = Util.GetUserInfoFromToken(accessToken);

                    if (tokenInfo.Count > 0)
                    {
                        userName = tokenInfo["username"];
                        tenanty = tokenInfo["tenanty"];
                    }

                }
                catch (Exception)
                {
                    throw;
                }

                var searchAprov = notificationOriginal.AprovEmail.Where(x => x.email == userName).FirstOrDefault();
                if (searchAprov != null)
                {
                    notificationOriginal.AprovEmail.Where(x => x.email == userName).FirstOrDefault().aprov = notificationCommand.Aprov;
                    notificationOriginal.AprovEmail.Where(x => x.email == userName).FirstOrDefault().view = true;
                }

                var userAprovTrue = notificationOriginal.AprovEmail.Where(x => x.aprov == true).Count();
                var userAprovFalse = notificationOriginal.AprovEmail.Where(x => x.aprov == false).Count();
                var checkAprovs = notificationOriginal.AprovEmail.Where(x => x.aprov == null).Count();

                if (userAprovTrue >= notificationOriginal.AprovMin)
                {
                    notificationOriginal.Aprov = true;
                    notificationOriginal.AprovEmail.ToList().ForEach(x => x.view = true);

                    //Atualizar o Entity para o próximo status e gerar notificação com base no proximoStatus
                    var schemaRepository = _schemaRepository.GetByField("name", notificationOriginal.SchemaName);


                    var statusFlow = _statusFlowService.GetBySchemaAndField(notificationOriginal.SchemaName, notificationOriginal.Field, tenanty);
                    if (statusFlow != null)
                    {
                        var proximoStatus = statusFlow.Properties.Where(x => x.Status == notificationOriginal.Value).FirstOrDefault();
                        if (proximoStatus != null)
                        {
                            existeProximo = statusFlow.Properties.Where(x => x.Status == proximoStatus.ProximoStatus).FirstOrDefault();
                        }

                        var entity = await _entityService.GetById(notificationOriginal.SchemaName, notificationOriginal.SchemaRecordId);
                        var json = JsonConvert.SerializeObject(entity);
                        var settings = new JsonSerializerSettings
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        };

                        var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json, settings);
                        dict[notificationOriginal.Field] = proximoStatus.ProximoStatus;

                        var obsMensagem = $"{DateTime.Now} - {notificationCommand.Justificativa} ({userName})";
                        if (!dict.ContainsKey("justificativa")) dict.Add("justificativa", obsMensagem);
                        else dict["justificativa"] = $"{obsMensagem}";

                        var command = new UpdateEntityCommand()
                        {
                            Id = schemaRepository.Id,
                            SchemaName = notificationOriginal.SchemaName,
                            SchemaJson = schemaRepository.JsonValue,
                            Data = dict
                        };
                        await SendCommand(command);
                        var camelCaseDict = new Dictionary<string, object>();
                        foreach (var kvp in dict)
                        {
                            string camelCaseKey = kvp.Key.Replace(kvp.Key[0], char.ToLower(kvp.Key[0]));
                            camelCaseDict[camelCaseKey] = kvp.Value;
                        }
                        if (existeProximo != null)
                        {
                            var proximoStatusFlow = statusFlow.Properties.Where(x => x.Status == existeProximo.Status).FirstOrDefault();
                            List<string> aprovadoresList = new List<string>();
                            var listEmailAprov = new List<AprovEmailProperties>();
                            var dataAprov = DateTime.Now;
                            if (!proximoStatusFlow.Action.IsNullOrEmpty() && proximoStatusFlow.Type == "Action")
                            {
                                var statusFlowAction = _actionRepository.GetByField("name", proximoStatusFlow.Action);

                                if (statusFlowAction != null)
                                {
                                    ActionController x = new(null, null, _schemaRepository, _entityService, _actionRepository, null, _userService);
                                    ApiEntities.Action[] ac = x.GetBySchemaName(notificationOriginal.SchemaName);

                                    var userHelper = _schemaRepository.GetUserHelper();
                                    var user = _userService.GetByUserName(userHelper.GetTenanty(), userHelper.GetUserName());

                                    string auth = Request.Headers["Authorization"];

                                    dynamic result = await x.Run(statusFlowAction, new Dictionary<string, object>()
                                    {
                                        { "params", new { entity = camelCaseDict } }
                                    }, auth, user, null);

                                    aprovadoresList = result.aprovadores.ToObject<List<string>>();

                                    if (!aprovadoresList.IsNullOrEmpty())
                                    {
                                        if (!aprovadoresList.Contains("admin@admin.com")) aprovadoresList.Add("admin@admin.com");

                                        foreach (var email in aprovadoresList)
                                        {
                                            listEmailAprov.Add(new AprovEmailProperties { email = email, aprov = null, view = false, data = null });
                                        }
                                    }



                                }
                                else
                                {
                                    return BadRequest();
                                }
                            }
                            else
                            {
                                var aprovEmails = proximoStatusFlow.AprovEmail;

                                var users = _userService.GetUsersByUserName(aprovEmails, tenanty);
                                if (!users.IsNullOrEmpty())
                                {
                                    var userEmails = users.Select(x => x.UserName).ToList();
                                    if (!userEmails.IsNullOrEmpty())
                                    {
                                        if (!userEmails.Contains("admin@admin.com")) userEmails.Add("admin@admin.com");

                                        foreach (var email in userEmails)
                                        {
                                            listEmailAprov.Add(new AprovEmailProperties { email = email, aprov = null, view = false, data = null });
                                        }
                                    }

                                }
                            }
                            var statusFlowItemOriginal = _statusFlowItemService.GetBySchemaRecordId(notificationCommand.SchemaRecordId);
                            if (statusFlowItemOriginal != null)
                            {

                                statusFlowItemOriginal.StatusFlowId = statusFlow.Id;
                                statusFlowItemOriginal.Status = existeProximo.Status;
                                statusFlowItemOriginal.AprovEmail = listEmailAprov.Select(x => x.email).ToArray();
                                _statusFlowItemService.UpdateStatusFlowItem(statusFlowItemOriginal, statusFlowItemOriginal.Id);

                            }
                            if (listEmailAprov.Count() > 0)
                            {
                                var newNotification = new Notification(notificationOriginal.SchemaRecordId, notificationOriginal.SchemaName, notificationOriginal.Field, existeProximo.Status, notificationOriginal.Value, "Mudança de status para " + existeProximo.Status, null, notificationOriginal.UserId, false, notificationOriginal.AprovMin, listEmailAprov.ToArray(), null);
                                newNotification.CreateBy = userName;

                                _notificationService.RegisterAsync(newNotification);
                            }

                        }
                    }
                }

                //if (userAprovFalse >= notificationOriginal.AprovMin)
                //{
                //    notificationOriginal.Aprov = false;
                //}
                if ((checkAprovs == 0 || userAprovFalse >= notificationOriginal.AprovMin || (checkAprovs + userAprovTrue < notificationOriginal.AprovMin)) && notificationOriginal.AprovMin > userAprovTrue)
                {
                    var statusFlow = _statusFlowService.GetBySchemaAndField(notificationOriginal.SchemaName, notificationOriginal.Field, tenanty);
                    if (statusFlow != null)
                    {
                        var status = statusFlow.Properties.Where(x => x.Status == notificationOriginal.Value).FirstOrDefault();

                        var schemaRepository = _schemaRepository.GetByField("name", notificationOriginal.SchemaName);

                        notificationOriginal.Aprov = false;
                        notificationOriginal.AprovEmail.ToList().ForEach(x => x.view = true);

                        var entity = await _entityService.GetById(notificationOriginal.SchemaName, notificationOriginal.SchemaRecordId);
                        var json = JsonConvert.SerializeObject(entity);
                        var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                        //dict[notificationOriginal.Field] = notificationOriginal.ValueOld != null ? notificationOriginal.ValueOld : notificationOriginal.Value;
                        dict[notificationOriginal.Field] = status.ReprovaStatus;
                        var obsMensagem = $"{DateTime.Now} - {notificationCommand.Justificativa} ({userName})";
                        if (!dict.ContainsKey("justificativa")) dict.Add("justificativa", obsMensagem);
                        else dict["justificativa"] = $"{obsMensagem}";


                        var command = new UpdateEntityCommand()
                        {
                            Id = schemaRepository.Id,
                            SchemaName = notificationOriginal.SchemaName,
                            SchemaJson = schemaRepository.JsonValue,
                            Data = dict
                        };
                        await SendCommand(command);

                        statusFlow = _statusFlowService.GetBySchemaAndField(notificationOriginal.SchemaName, status.ReprovaStatus, tenanty);
                        if (statusFlow != null)
                        {
                            var listEmailAprov = new List<AprovEmailProperties>();

                            //var aprovEmails = statusFlow.Properties.Where(x => x.Status == notificationOriginal.Value).FirstOrDefault().AprovEmail;
                            var emailReprova = notificationOriginal.CreateBy;
                            var dataAprov = DateTime.Now;
                            foreach (var aprovs in notificationOriginal.AprovEmail)
                            {
                                listEmailAprov.Add(new AprovEmailProperties { email = aprovs.email, aprov = null, view = false, data = null });
                            }


                            var getUser = _userService.GetByUserName(tenanty, emailReprova);
                            var newNotification = new Notification(notificationOriginal.SchemaRecordId, notificationOriginal.SchemaName, notificationOriginal.Field, notificationOriginal.ValueOld, notificationOriginal.Value, "Mudança de status reprovada", obsMensagem, getUser.Id, false, notificationOriginal.AprovMin, listEmailAprov.ToArray(), null);
                            newNotification.CreateBy = userName;
                            _notificationService.RegisterAsync(newNotification);

                        }

                    }


                }
                var dataAtual = DateTime.Now;

                notificationOriginal.AprovEmail.Where(x => x.email == userName).FirstOrDefault().data = dataAtual;

                _notificationService.UpdateNotification(notificationOriginal, notificationOriginal.Id);
                string aprovacao = notificationCommand.Aprov ? "aprovou" : "reprovou";
                string mensagem = $"Usuário {userName} {aprovacao} o status {notificationOriginal.Value}";

                //Sendgrid
                var sendEmailCommand = new SendEmailCommand();
                sendEmailCommand.Subject = aprovacao;
                sendEmailCommand.To = notificationOriginal.CreateBy;
                sendEmailCommand.PlainTextContent = mensagem;
                sendEmailCommand.HtmlContent = mensagem;
                _userAccessService.SendEmail(sendEmailCommand);
                if (notificationOriginal.AprovEmail.Length > 1)
                {
                    if (existeProximo != null)
                    {
                        var log = new ApproveLog(notificationOriginal.SchemaRecordId, notificationOriginal.SchemaName, notificationOriginal.Field, existeProximo.Status, mensagem);
                        _approveLogService.Register(log);
                    }
                    if (notificationOriginal.Aprov == false)
                    {
                        var log = new ApproveLog(notificationOriginal.SchemaRecordId, notificationOriginal.SchemaName, notificationOriginal.Field, notificationOriginal.Value, mensagem);
                        _approveLogService.Register(log);
                    }
                }
            }
            return ResponseDefault();

        }




        /// <summary>
        /// Update notificationsView
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPut("notificationsViewUpdate")]
        [ExcludeFromCodeCoverage]
        public IActionResult UpdateView(string notificationId)
        {

            Notification notificationOriginal = null;

            notificationOriginal = _notificationService.GetByIdView(notificationId);


            //if (data.Keys.FirstOrDefault(key => key == "id") == null) data.Add("id", null);
            //data["id"] = id;

            if (notificationOriginal == null) return ResponseDefault("Notificação não existe");


            var userName = "";

            try
            {
                var accessToken = Request.Headers[HeaderNames.Authorization];
                var tokenInfo = Util.GetUserInfoFromToken(accessToken);

                if (tokenInfo.Count > 0) userName = tokenInfo["username"];
            }
            catch (Exception)
            {
                throw;
            }


            //var userAprovTrue = notificationOriginal.AprovEmail.Where(x => x.aprov == true).Count();
            //var userAprovFalse = notificationOriginal.AprovEmail.Where(x => x.aprov == false).Count();
            if (notificationOriginal.AprovEmail is not null)
            {
                var searchView = notificationOriginal.AprovEmail.Where(x => x.email == userName && x.view == false).Count();
                if (searchView > 0)
                {
                    notificationOriginal.AprovEmail.Where(x => x.email == userName && x.view == false).FirstOrDefault().view = true;
                }
            }

            notificationOriginal.View = true;
            _notificationService.UpdateNotification(notificationOriginal, notificationId);
            return ResponseDefault(notificationOriginal);
        }


    }
}
