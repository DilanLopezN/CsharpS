using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Simjob.Framework.Application.Controllers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Domain.Models.PublishModels;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Infra.Schemas.Entities;
using Simjob.Framework.Infra.Schemas.Interfaces;
using Simjob.Framework.Services.Api.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ApiEntities = Simjob.Framework.Services.Api.Entities;

namespace Simjob.Framework.Services.Api.Controllers
{
    [Authorize(Policy = "ApiKeyPolicy")]
    public class ActionController : BaseController
    {
        private readonly IEntityService _entityService;
        private readonly IRepository<MongoDbContext, ApiEntities.Action> _actionsRepository;
        private readonly IRepository<MongoDbContext, ApiEntities.ActionSchedule> _actionScheduleRepository;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;
        private readonly IUserService _userService;

        public static TV GetValue<TK, TV>(IDictionary<TK, TV> dict, TK key, TV defaultValue = default(TV))
        {
            TV value;
            return dict.TryGetValue(key, out value) ? value : defaultValue;
        }

        public ActionController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, IRepository<MongoDbContext, Schema> schemaRepository, IEntityService entityService, IRepository<MongoDbContext, ApiEntities.Action> actionRepository, IRepository<MongoDbContext, ApiEntities.ActionSchedule> actionScheduleRepository, IUserService userService) : base(bus, notifications)
        {
            _entityService = entityService;
            _actionsRepository = actionRepository;
            _actionScheduleRepository = actionScheduleRepository;
            _schemaRepository = schemaRepository;
            _userService = userService;
        }

        [ExcludeFromCodeCoverage]
        internal async Task<object> Run(ApiEntities.Action action, Dictionary<string, object> data, string authorization, object user, object entity)
        {
            //var httpClientHandler = new HttpClientHandler();
            //httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
            //{
            //    return true;
            //};

            HttpClient client = new()
            {
                Timeout = TimeSpan.FromMilliseconds(action.Timeout >= 5000 ? action.Timeout : 5000)
                //Timeout = TimeSpan.FromMilliseconds(90000)

            };

            var values = new
            {
                code = action.JavascriptCode,
                @params = data["params"] ?? new { },
                context = new Dictionary<string, object>()
                {
                    { "user", user },
                    { action.CallConfig.SchemaName ?? "schema", entity },
                    { "action", action }
                }

            };

            var valuesJson = JsonConvert.SerializeObject(values, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize
                });

            var content = new StringContent(valuesJson, System.Text.Encoding.UTF8, "application/json");
            content.Headers.Add("Token", authorization.Replace("Bearer", "").Trim());

            //try { var r = await client.PostAsync(EnvironmentSettings.Get("ACTION_BACKEND_URL") + "/actions/run", content); } catch (Exception ex) { throw ex; }

            var response = await client.PostAsync(EnvironmentSettings.Get("ACTION_BACKEND_URL") + "/actions/run", content);
            string responseRawText = await response.Content.ReadAsStringAsync();
            var responseJson = Newtonsoft.Json.Linq.JObject.Parse(responseRawText == "" ? "{}" : responseRawText);

            return responseJson;
        }



        [ExcludeFromCodeCoverage]
        protected internal ApiEntities.Action[] GetBySchemaName([FromRoute] string schemaName)
        {
            var actions = _actionsRepository.GetAll().Data;

            List<ApiEntities.Action> result = new();
            foreach (ApiEntities.Action ac in actions)
            {
                if (ac.CallConfig.SchemaName == schemaName)
                {
                    result.Add(ac);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Execute Action
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPost("run")]
        [RequiresTwoStepAuthentication(true)]
        public async Task<IActionResult> Run([FromBody] Dictionary<string, object> data, [FromHeader] string authorization)
        {
            ApiEntities.Action action;
            if (data.ContainsKey("actionId") && !string.IsNullOrEmpty(data["actionId"].ToString()))
            {
                action = _actionsRepository.GetById(data["actionId"].ToString());
                if (action is null)
                {
                    base.SendNotification("ActionID", "Action not found.");
                    return ResponseDefault();
                }
            }
            else
            {
                if (data.ContainsKey("actionName") && !string.IsNullOrEmpty(data["actionName"].ToString()))
                {
                    action = _actionsRepository.GetByField("Name", data["actionName"].ToString());
                    if (action is null)
                    {
                        base.SendNotification("ActionID", "Action not found.");
                        return ResponseDefault();
                    }
                }
                else
                {
                    SendNotification("Run", "Search value not found: actionId or actionName");
                    return ResponseDefault();
                }
            }

            object entity = null;
            if (data.ContainsKey("entityId") && data["entityId"] != null)
            {
                entity = await _entityService.GetById(action.CallConfig.SchemaName, data["entityId"].ToString());
                if (entity == null)
                {
                    SendNotification("Run", "Entity not found by the provided entityId");
                    return ResponseDefault();
                }
            }

            var userHelper = _schemaRepository.GetUserHelper();
            var user = _userService.GetByUserName(userHelper.GetTenanty(), userHelper.GetUserName());

            var responseJson = await Run(action, data, authorization, user, entity);

            //   var responseJson= await RunSchedule(action, data, user, entity);
            return ResponseDefault(responseJson);
        }


        internal async Task<object> RunDynamic(ApiEntities.Action action, Dictionary<string, object> data, [FromHeader] string authorization, object user, object entity)
        {
            //var httpClientHandler = new HttpClientHandler();
            //httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
            //{
            //    return true;
            //};

            HttpClient client = new()
            {
                Timeout = TimeSpan.FromMilliseconds(action.Timeout >= 5000 ? action.Timeout : 5000)
                //Timeout = TimeSpan.FromMilliseconds(90000)

            };

            var values = new
            {
                code = action.JavascriptCode,
                @params = data ?? new Dictionary<string, object>{ },
                context = new Dictionary<string, object>()
                {
                    { "user", user },
                    { action.CallConfig.SchemaName ?? "schema", entity },
                    { "action", action },
                    { "token", authorization.Replace("Bearer", "").Trim() }
                }
            };

            var valuesJson = JsonConvert.SerializeObject(values, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize
                });

            var content = new StringContent(valuesJson, System.Text.Encoding.UTF8, "application/json");
            content.Headers.Add("Token", authorization.Replace("Bearer", "").Trim());

            //try { var r = await client.PostAsync(EnvironmentSettings.Get("ACTION_BACKEND_URL") + "/actions/run", content); } catch (Exception ex) { throw ex; }

               var response = await client.PostAsync(EnvironmentSettings.Get("ACTION_BACKEND_URL") + "/actions/run", content);
        //    var response = await client.PostAsync("https://f66e-191-189-164-60.ngrok-free.app/actions/run", content);

            string responseRawText = await response.Content.ReadAsStringAsync();
            var responseJson = Newtonsoft.Json.Linq.JObject.Parse(responseRawText == "" ? "{}" : responseRawText);

            return responseJson;
        }
        /// <summary>
        /// Execute Action
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPost("run/{actionId}")]
        [RequiresTwoStepAuthentication(true)]
        public async Task<IActionResult> RunDynamic(string actionId, [FromBody] Dictionary<string, object> data, [FromHeader] string authorization)
        {
            ApiEntities.Action action;
            if (string.IsNullOrEmpty(actionId))
            {
                base.SendNotification("ActionID", "Action not found.");
                return ResponseDefault();
            }

            action = _actionsRepository.GetById(actionId);
            if (action is null)
            {
                base.SendNotification("ActionID", "Action not found.");
                return ResponseDefault();
            }

            object entity = null;
            if (data.ContainsKey("entityId") && data["entityId"] != null)
            {
                entity = await _entityService.GetById(action.CallConfig.SchemaName, data["entityId"].ToString());
                if (entity == null)
                {
                    SendNotification("Run", "Entity not found by the provided entityId");
                    return ResponseDefault();
                }
            }

            var userHelper = _schemaRepository.GetUserHelper();
            var user = _userService.GetByUserName(userHelper.GetTenanty(), userHelper.GetUserName());

            var responseJson = await RunDynamic(action, data, authorization, user, entity);


            return ResponseDefault(responseJson);
        }

        /// <summary>
        /// Get Action by ID
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpGet("{id}")]
        public IActionResult Get([FromRoute] string id)
        {
            var action = _actionsRepository.GetById(id);
            return ResponseDefault(action);
        }

        /// <summary>
        /// Get Action by ID
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpGet("GetByName")]
        public IActionResult GetByName([Required] string actionName)
        {
            var action = _actionsRepository.GetByField("name", actionName);
            return ResponseDefault(action);
        }

        /// <summary>
        /// Get All Actions
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpGet]
        public IActionResult GetAll(int? page, int? limit, string sortField = null, bool sortDesc = false)
        {
            return ResponseDefault(_actionsRepository.GetAll(page, limit, sortField, sortDesc));
        }

        /// <summary>
        /// Get Actions Filtered by Fields
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpGet("search-fields")]
        public IActionResult SearchFields(string value, int? page, int? limit, string sortField = null, string groupBy = null, bool sortDesc = false, string searchFields = null, string ids = null)
        {
            if (value == null) value = "";

            if (searchFields == null) searchFields = "";
            //ApiEntities.Action teste = new();
            //var propertiesToSearch = teste.GetType().GetProperties();
            //List<string> properties = new();

            //if (searchFields == null)
            //{
            //    foreach (var obj in propertiesToSearch)
            //    {
            //        properties.Add(obj.Name.ToString());
            //    }

            //    searchFields = string.Join(',', properties);
            //}

            return ResponseDefault(_actionsRepository.SearchRegexByFields(searchFields, value, "", null, page, limit, sortField, groupBy, sortDesc, ids));
        }



        /// <summary>
        /// Update Action
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPut("{id}")]
        public IActionResult Update([FromRoute] string id, [FromBody] Dictionary<string, dynamic> data)
        {

            var action = _actionsRepository.GetById(id);
            var callType = action.CallType.ToString();
            List<ApiEntities.ActionParameter> parameters = new();
            foreach (var i in data["parameters"])
            {
                parameters.Add(ApiEntities.ActionParameter.ParseFromDynamic(i));
            }

            action.CallConfig = new ApiEntities.CallConfig()
            {
                SchemaName = data["callConfig"]["schemaName"],
                TriggerProperty = data["callConfig"]["triggerProperty"]
            };
            action.CallType = Enum.Parse<ApiEntities.ActionCallType>(data["callType"]);
            action.CreateAt = DateTime.Now;
            action.Description = data["description"];
            action.Icon = data["icon"];
            action.Module = data["module"];
            action.Name = data["name"];
            action.Timeout = Int32.Parse(data["timeout"].ToString());
            action.JavascriptCode = data["javascriptCode"];
            action.ResultType = Enum.Parse<ApiEntities.ActionResultType>(data["resultType"]);
            action.Parameters = parameters.ToArray();

            _actionsRepository.Update(action);

            if (callType != data["callType"].ToString())
            {
                if (data["callType"].ToString() == "Schedule")
                {
                    ApiEntities.ActionSchedule actSchedule = new()
                    {
                        CallConfig = new ApiEntities.CallConfig()
                        {
                            SchemaName = data["callConfig"]["schemaName"],
                            TriggerProperty = data["callConfig"]["triggerProperty"]
                        },
                        CallType = Enum.Parse<ApiEntities.ActionCallType>(data["callType"]),
                        CreateAt = DateTime.Now,
                        Description = data["description"],
                        Icon = data["icon"],
                        Module = data["module"],
                        Name = data["name"],
                        Timeout = Int32.Parse(data["timeout"].ToString()),
                        NextActionRun = data.ContainsKey("nextActionRun") ? DateTime.Parse(data["nextActionRun"]) : DateTime.Now,
                        IsExecuted = false,
                        Timer = data.ContainsKey("timer") ? Int32.Parse(data["timer"]) : 60,
                        JavascriptCode = data["javascriptCode"],
                        ResultType = Enum.Parse<ApiEntities.ActionResultType>(data["resultType"]),
                        Parameters = parameters.ToArray(),
                        ActionId = action.Id
                    };

                    _actionScheduleRepository.Insert(actSchedule);
                }
            }


            return ResponseDefault(action);
        }

        /// <summary>
        /// Delete Action
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] string id)
        {
            _actionsRepository.Delete(id);
            return ResponseDefault();
        }

        /// <summary>
        /// Insert new Action
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [HttpPost()]
        public IActionResult Insert([FromBody] Dictionary<string, dynamic> data)
        {
            List<ApiEntities.ActionParameter> parameters = new();

            foreach (var i in data["parameters"])
            {
                parameters.Add(ApiEntities.ActionParameter.ParseFromDynamic(i));
            }
            ApiEntities.Action act = new()
            {
                CallConfig = new ApiEntities.CallConfig()
                {
                    SchemaName = data["callConfig"]["schemaName"],
                    TriggerProperty = data["callConfig"]["triggerProperty"]
                },
                CallType = Enum.Parse<ApiEntities.ActionCallType>(data["callType"]),
                CreateAt = DateTime.Now,
                Description = data["description"],
                Icon = data["icon"],
                Module = data["module"],
                Name = data["name"],
                Timeout = Int32.Parse(data["timeout"].ToString()),
                JavascriptCode = data["javascriptCode"],
                ResultType = Enum.Parse<ApiEntities.ActionResultType>(data["resultType"]),
                Parameters = parameters.ToArray()
            };

            _actionsRepository.Insert(act);

            if (data["callType"].ToString() == "Schedule")
            {
                ApiEntities.ActionSchedule actSchedule = new()
                {
                    CallConfig = new ApiEntities.CallConfig()
                    {
                        SchemaName = data["callConfig"]["schemaName"],
                        TriggerProperty = data["callConfig"]["triggerProperty"]
                    },
                    CallType = Enum.Parse<ApiEntities.ActionCallType>(data["callType"]),
                    CreateAt = DateTime.Now,
                    Description = data["description"],
                    Icon = data["icon"],
                    Module = data["module"],
                    Name = data["name"],
                    Timeout = Int32.Parse(data["timeout"].ToString()),
                    NextActionRun = data.ContainsKey("nextActionRun") ? DateTime.Parse(data["nextActionRun"]) : DateTime.Now,
                    IsExecuted = false,
                    Timer = data.ContainsKey("timer") ? Int32.Parse(data["timer"]) : 60,
                    JavascriptCode = data["javascriptCode"],
                    ResultType = Enum.Parse<ApiEntities.ActionResultType>(data["resultType"]),
                    Parameters = parameters.ToArray(),
                    ActionId = act.Id
                };

                _actionScheduleRepository.Insert(actSchedule);
            }

            return ResponseDefault(act);
        }

        /// <summary>
        /// Publish Action in any tenanty database
        /// </summary>
        /// <response code="200">Return result from mql</response>
        [Authorize]
        [HttpPost("publish")]
        public async Task<IActionResult> Publish([Required] string url, [Required] string tenanty, [Required] string email, [Required] string password, [FromBody] Dictionary<string, object> actionData, string actionName = null)
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

                if (!string.IsNullOrEmpty(actionName))
                {
                    res = await client.GetAsync(url + $"/Action/GetByName?actionName={actionName}");
                }
                else
                {
                    res = new HttpResponseMessage();
                }


                content = await res.Content.ReadAsStringAsync();


                var action = JsonConvert.DeserializeObject<GetActionResult>(content);
                bool success = false;
                if (action != null && action.data != null)
                {
                    //updateAction
                    dynamic body = actionData;
                    json = JsonConvert.SerializeObject(body);
                    data = new StringContent(json, Encoding.UTF8, "application/json");
                    try { res = await client.PutAsync(url + $"/Action/{action.data.id}", data); success = res.IsSuccessStatusCode; } catch (Exception) { success = false; }
                    content = await res.Content.ReadAsStringAsync();
                }

                else
                {
                    //createAction
                    dynamic body = actionData;
                    json = JsonConvert.SerializeObject(body);
                    data = new StringContent(json, Encoding.UTF8, "application/json");
                    try { res = await client.PostAsync(url + $"/Action", data); success = res.IsSuccessStatusCode; } catch (Exception) { success = false; }
                    content = await res.Content.ReadAsStringAsync();
                }
                if (!success) return BadRequest();
            }
            return ResponseDefault();
        }
    }
}