using DotLiquid;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Simjob.Framework.Application.Controllers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Domain.Models.PublishModels;
using Simjob.Framework.Domain.Util;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Infra.Schemas.Commands.Views;
using Simjob.Framework.Infra.Schemas.Entities;
using Simjob.Framework.Infra.Schemas.Interfaces;
using Simjob.Framework.Services.Api.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Services.Api.Controllers
{
    public class ViewController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IViewService _viewService;
        private readonly IUserHelper _userHelper;
        private readonly IExternalTokensService _externalTokensService;
        private readonly IPermissionService _permissionService;
        //private readonly ViewContext _context;
        private readonly MongoDbContext _context;
        protected readonly IMongoCollection<Source> _sourceCollection;
        protected readonly SourceContext SourceContext;
        public ViewController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, IUserService userService, IPermissionService permissionService, IExternalTokensService externalTokensService, MongoDbContext context, SourceContext sourceContext, IViewService viewService, IUserHelper userHelper) : base(bus, notifications)
        {
            _externalTokensService = externalTokensService;
            _context = context;
            _userService = userService;
            _userHelper = userHelper;
            _viewService = viewService;
            _permissionService = permissionService;
            _sourceCollection = sourceContext.GetUserCollection();
            SourceContext = sourceContext;
        }

        /// <summary>
        /// Execute a query command from view and return a result
        /// </summary>
        internal object Run(Views view, Dictionary<string, object> data, string tenanty)
        {
            string quote = "\"";

            if (data["page"] == null)
                data["page"] = 1;

            if (data["limit"] == null)
                data["limit"] = 30;

        
            var page = Convert.ToInt32(data["page"]);
            var limit = Convert.ToInt32(data["limit"]);
            var parametros =  data["params"].ToString();

            if (parametros != "[]")
            {
                var objs = new List<object>();
                foreach (KeyValuePair<string, object> entry in data)
                {
                    objs.Add(entry.Value);
                }
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(objs.FirstOrDefault(x => x is JArray));
                var dictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);

                var listaParametros = new List<ViewParameter>();


                foreach (var lista in dictionary)
                {
                    var viewParameter = new ViewParameter();

                    if (lista.ContainsKey("name"))
                    {
                        viewParameter.Name = lista.GetValueOrDefault("name");
                    }

                    if (lista.ContainsKey("dataType"))
                    {
                        viewParameter.DataType = lista.GetValueOrDefault("dataType");
                    }

                    if (lista.ContainsKey("value"))
                    {
                        viewParameter.Value = lista.GetValueOrDefault("value");
                    }
                    listaParametros.Add(viewParameter);
                }



                foreach (var param in listaParametros)
                {
                    var parametro = "@" + param.Name;

                    if (view.Query.Contains(param.Name))
                    {
                        if (param.DataType.ToLower() == "date")
                            view.Query = view.Query.Replace(parametro, quote + param.Value + quote);
                        else if (param.DataType.ToLower() == "array")
                        {
                            string[] values = param.Value.Split(',');
                            string valueReplace = "";
                            for (int i = 0; i < values.Length; i++)
                            {
                                if (!string.IsNullOrEmpty(valueReplace)) valueReplace += "," + quote + values[i] + quote;
                                else valueReplace = "[" + quote + values[i] + quote;
                            }
                            valueReplace += "]";
                            string replaceField = $"[\"{parametro}\"]";
                            view.Query = view.Query.Replace(replaceField, valueReplace);
                        }
                        else
                        {
                            view.Query = view.Query.Replace(parametro, param.Value);
                        }
                    }
                }

            }

            else
            {
                foreach (var param in view.Parameters)
                {

                    var parametro = "@" + param.Name;

                    if (view.Query.Contains(param.Name))
                    {
                        if (param.DataType.ToLower() == "date")
                            view.Query = view.Query.Replace(parametro, quote + param.Value + quote);
                        else if (param.DataType.ToLower() == "array")
                        {
                            string[] values = param.Value.Split(',');
                            string valueReplace = "";
                            for (int i = 0; i < values.Length; i++)
                            {
                                if (!string.IsNullOrEmpty(valueReplace)) valueReplace += "," + quote + values[i] + quote;
                                else valueReplace = "[" + quote + values[i] + quote;
                            }
                            valueReplace += "]";
                            string replaceField = $"[\"{parametro}\"]";
                            view.Query = view.Query.Replace(replaceField, valueReplace);
                        }
                        else
                        {
                            view.Query = view.Query.Replace(parametro, param.Value);
                        }

                    }
                }
            }
            if (!data.ContainsKey("paginationType") || data["paginationType"] == null || Convert.ToInt32(data["paginationType"]) == 0)
            {
                int defaultPage = 1;
                int defaultLimit = int.MaxValue;
                var defaultSkip = (1 - 1) * int.MaxValue;

                view.Query = view.Query.Replace("@page", defaultPage.ToString());
                view.Query = view.Query.Replace("@skip", defaultSkip.ToString());
                view.Query = view.Query.Replace("@limit", defaultLimit.ToString());

                var command = view.Query.ToString();

               var list =  _context.RunCommand(command, tenanty);
                List<object> result = new List<object>();

                result = list;
                var res = result.Skip((page - 1) * limit).Take(limit);
                long count = Convert.ToInt64(result.Count());
                return new PaginationData<object>(res.ToList(), page, limit, count);
            }
            else if(Convert.ToInt32(data["paginationType"]) == 1)
            {
                var skip = (page - 1) * limit;

                view.Query = view.Query.Replace("@page", page.ToString());
                view.Query = view.Query.Replace("@skip", skip.ToString());
                view.Query = view.Query.Replace("@limit", limit.ToString());

                var command = view.Query.ToString();

                var list =  _context.RunCommand(command, tenanty);
                List<object> result = new List<object>();

                result = list;

                long count = Convert.ToInt64(result.Count());
                return new PaginationData<object>(result.ToList(), page, limit, count);
            }
            return ResponseDefault();
        }


        private async Task<dynamic> RunQuerySql(string query)
        {
            var filterActive = Builders<Source>.Filter.Eq(u => u.Active, true);
            var filterIsDeleted = Builders<Source>.Filter.Eq(u => u.IsDeleted, false);

            var source = _sourceCollection.Find(filterIsDeleted & filterActive).FirstOrDefault();
            if (source == null) return null;
            var selectSql = query;
            //var countSql = $"SELECT COUNT(*) FROM [{schemaName}] {whereClause};";

            var connectionString = $"Server={source.Host},{source.Port};Database={source.DbName};User Id={source.User};Password={source.Password};TrustServerCertificate=True";


           // var connectionString = $"Server=8.221.125.159,1433;Database=sgferp;User Id=SA;Password=Accist@123;TrustServerCertificate=True";
            var results = new List<Dictionary<string, object>>();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    int total;
                    //using (var countCmd = new SqlCommand(countSql, connection))
                    //{
                    //    total = (int)await countCmd.ExecuteScalarAsync();
                    //}
                    using (var selectCmd = new SqlCommand(selectSql, connection))
                    using (var reader = await selectCmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var fieldName = reader.GetName(i);
                                var fieldValue = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
                                row[fieldName] = fieldValue!;
                            }
                            results.Add(row);
                        }
                    }

                    //return (true, results, total, null);
                }
            }
            catch (Exception ex)
            {
                //return (false, null, 0, ex.Message);
            }
            return results;
        }
        /// <summary>
        /// Execute a query command from view and return a result
        /// </summary>
        internal async Task<dynamic> RunSql(Views view, Dictionary<string, object> data, string tenanty)
        {
            string quote = "\"";

            if (data["page"] == null)
                data["page"] = 1;

            if (data["limit"] == null)
                data["limit"] = 30;


            var page = Convert.ToInt32(data["page"]);
            var limit = Convert.ToInt32(data["limit"]);
            var parametros = data["params"].ToString();

            if (parametros != "[]")
            {
                var objs = new List<object>();
                foreach (KeyValuePair<string, object> entry in data)
                {
                    objs.Add(entry.Value);
                }
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(objs.FirstOrDefault(x=> x is JArray));
                var dictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);

                var listaParametros = new List<ViewParameter>();


                foreach (var lista in dictionary)
                {
                    var viewParameter = new ViewParameter();

                    if (lista.ContainsKey("name"))
                    {
                        viewParameter.Name = lista.GetValueOrDefault("name");
                    }

                    if (lista.ContainsKey("dataType"))
                    {
                        viewParameter.DataType = lista.GetValueOrDefault("dataType");
                    }

                    if (lista.ContainsKey("value"))
                    {
                        viewParameter.Value = lista.GetValueOrDefault("value");
                    }
                    listaParametros.Add(viewParameter);
                }



                foreach (var param in listaParametros)
                {
                    var parametro = "@" + param.Name;

                    if (view.Query.Contains(param.Name))
                    {
                        if (param.DataType.ToLower() == "date")
                            view.Query = view.Query.Replace(parametro, quote + param.Value + quote);
                        else if (param.DataType.ToLower() == "array")
                        {
                            string[] values = param.Value.Split(',');
                            string valueReplace = "";
                            for (int i = 0; i < values.Length; i++)
                            {
                                if (!string.IsNullOrEmpty(valueReplace)) valueReplace += "," + quote + values[i] + quote;
                                else valueReplace = "[" + quote + values[i] + quote;
                            }
                            valueReplace += "]";
                            string replaceField = $"[\"{parametro}\"]";
                            view.Query = view.Query.Replace(replaceField, valueReplace);
                        }
                        else
                        {
                            view.Query = view.Query.Replace(parametro, param.Value);
                        }
                    }
                }

            }

            else
            {
                foreach (var param in view.Parameters)
                {

                    var parametro = "@" + param.Name;

                    if (view.Query.Contains(param.Name))
                    {
                        if (param.DataType.ToLower() == "date")
                            view.Query = view.Query.Replace(parametro, quote + param.Value + quote);
                        else if (param.DataType.ToLower() == "array")
                        {
                            string[] values = param.Value.Split(',');
                            string valueReplace = "";
                            for (int i = 0; i < values.Length; i++)
                            {
                                if (!string.IsNullOrEmpty(valueReplace)) valueReplace += "," + quote + values[i] + quote;
                                else valueReplace = "[" + quote + values[i] + quote;
                            }
                            valueReplace += "]";
                            string replaceField = $"[\"{parametro}\"]";
                            view.Query = view.Query.Replace(replaceField, valueReplace);
                        }
                        else
                        {
                            view.Query = view.Query.Replace(parametro, param.Value);
                        }

                    }
                }
            }
            if (!data.ContainsKey("paginationType") || data["paginationType"] == null || Convert.ToInt32(data["paginationType"]) == 0)
            {
                int defaultPage = 1;
                int defaultLimit = int.MaxValue;
                var defaultSkip = (1 - 1) * int.MaxValue;

                view.Query = view.Query.Replace("@page", defaultPage.ToString());
                view.Query = view.Query.Replace("@skip", defaultSkip.ToString());
                view.Query = view.Query.Replace("@limit", defaultLimit.ToString());

                var command = view.Query.ToString();

                var list = await RunQuerySql(command);
                List<Dictionary<string,object>> result = new List<Dictionary<string, object>>();

                result = list;
                var res = result.Skip((page - 1) * limit).Take(limit);
                long count = Convert.ToInt64(result.Count());
                return new PaginationData<dynamic>(res.ToList(), page, limit, count);
            }
            else if (Convert.ToInt32(data["paginationType"]) == 1)
            {
                var skip = (page - 1) * limit;

                view.Query = view.Query.Replace("@page", page.ToString());
                view.Query = view.Query.Replace("@skip", skip.ToString());
                view.Query = view.Query.Replace("@limit", limit.ToString());

                var command = view.Query.ToString();

                var list = await RunQuerySql(command);
                List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

                result = list;

                long count = Convert.ToInt64(result.Count());
                return new PaginationData<dynamic>(result.ToList(), page, limit, count);
            }
            return ResponseDefault();
        }

        /// <summary>
        /// Execute view query from view Id
        /// </summary>
        /// <returns>Result from view </returns>
        /// <response code="200">Return a result from view query</response>
        // [Authorize]
        [HttpPost("run")]
        [RequiresTwoStepAuthentication(true)]
        [Authorize]

        public async Task<IActionResult> Run([FromBody] Dictionary<string, object> data)
        {
            Views view;
            if (data.ContainsKey("viewName") && !string.IsNullOrEmpty(data["viewName"].ToString()))
            {

                view = _viewService.GetViewByName(data["viewName"].ToString());
                if (view is null)
                {

                    base.SendNotification("viewName", "View not found.");
                    return ResponseDefault();
                }
                if(view.Type == "SQLSERVER")
                {
                    return ResponseDefault(await RunSql(view, data, null));
                }
                var responseJson = Run(view, data, null);

                var verifyStatus = responseJson.GetType().GetProperties().FirstOrDefault(o => o.Name == "StatusCode");
                if (verifyStatus != null)
                {
                    var statusCode = responseJson.GetType().GetProperties().First(o => o.Name == "StatusCode").GetValue(responseJson, null);
                    if ((int)statusCode == 400) return BadRequest("valores de parametros não podem ser vazios");
                }

                return ResponseDefault(responseJson);
            }
            if (data.ContainsKey("viewId") && !string.IsNullOrEmpty(data["viewId"].ToString()))
            {
                view = _viewService.GetById(data["viewId"].ToString());

                if (view is null)
                {

                    base.SendNotification("viewId", "View not found.");
                    return ResponseDefault();
                }
                if (view.Type == "SQLSERVER")
                {
                    return ResponseDefault(await RunSql(view, data, null));
                }
                var responseJson =  Run(view, data, null);

                var verifyStatus = responseJson.GetType().GetProperties().FirstOrDefault(o => o.Name == "StatusCode");
                if (verifyStatus != null)
                {
                    var statusCode = responseJson.GetType().GetProperties().First(o => o.Name == "StatusCode").GetValue(responseJson, null);
                    if ((int)statusCode == 400) return BadRequest("valores de parametros não podem ser vazios");
                }

                return ResponseDefault(responseJson);

            }

            return ResponseDefault();
        }


        /// <summary>
        /// Execute view query from view Id
        /// </summary>
        /// <returns>Result from view </returns>
        /// <response code="200">Return a result from view query</response>
        [HttpPost("Externalrun")]
        // [RequiresTwoStepAuthentication(false)]
        // [AllowAnonymous]
        public IActionResult ExternalRun(string token, [FromBody] Dictionary<string, object> data)
        {
            var tokens = _externalTokensService.GetByUserToken(token);
            if (tokens == null) return BadRequest("invalid token");

            var jwtToken = new JwtSecurityToken(tokens.UserToken);

            if ((jwtToken == null) || (jwtToken.ValidFrom > DateTime.UtcNow) || (jwtToken.ValidTo < DateTime.UtcNow)) return BadRequest("invalid token");
            var getUser = _userService.GetUserById(tokens.UserId);

            Views view;
            if (data.ContainsKey("viewName") && !string.IsNullOrEmpty(data["viewName"].ToString()))
            {
                view = _viewService.GetViewByNameExternal(data["viewName"].ToString(), getUser.Tenanty);

                if (view is null)
                {
                    base.SendNotification("viewName", "View not found.");
                    return ResponseDefault();
                }

                var responseJson = Run(view, data, getUser.Tenanty);

                return ResponseDefault(responseJson);
            }
            if (data.ContainsKey("viewId") && !string.IsNullOrEmpty(data["viewId"].ToString()))
            {
                view = _viewService.GetByIdExternal(data["viewId"].ToString(), getUser.Tenanty);

                if (view is null)
                {
                    base.SendNotification("viewId", "View not found.");
                    return ResponseDefault();
                }

                var responseJson = Run(view, data, getUser.Tenanty);

                return ResponseDefault(responseJson);
            }


            return ResponseDefault();
        }



        /// <summary>
        /// Execute a query command from view and return a result
        /// </summary>
        internal object RunBasic(Views view, string tenanty,int page,int limit)
        {
            string quote = "\"";

         
            //var parametros = data.ContainsKey("params") ? data["params"].ToString() : "[]";

            //if (parametros != "[]")
            //{
            //    var objs = new List<object>();
            //    foreach (KeyValuePair<string, object> entry in data)
            //    {
            //        objs.Add(entry.Value);
            //    }
            //    var json = Newtonsoft.Json.JsonConvert.SerializeObject(objs[0]);
            //    var dictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);

            //    var listaParametros = new List<ViewParameter>();


            //    foreach (var lista in dictionary)
            //    {
            //        var viewParameter = new ViewParameter();

            //        if (lista.ContainsKey("name"))
            //        {
            //            viewParameter.Name = lista.GetValueOrDefault("name");
            //        }

            //        if (lista.ContainsKey("dataType"))
            //        {
            //            viewParameter.DataType = lista.GetValueOrDefault("dataType");
            //        }

            //        if (lista.ContainsKey("value"))
            //        {
            //            viewParameter.Value = lista.GetValueOrDefault("value");
            //        }
            //        listaParametros.Add(viewParameter);
            //    }



            //    foreach (var param in listaParametros)
            //    {
            //        var parametro = "@" + param.Name;

            //        if (view.Query.Contains(param.Name))
            //        {
            //            if (param.DataType.ToLower() == "date")
            //                view.Query = view.Query.Replace(parametro, quote + param.Value + quote);
            //            else if (param.DataType.ToLower() == "array")
            //            {
            //                string[] values = param.Value.Split(',');
            //                string valueReplace = "";
            //                for (int i = 0; i < values.Length; i++)
            //                {
            //                    if (!string.IsNullOrEmpty(valueReplace)) valueReplace += "," + quote + values[i] + quote;
            //                    else valueReplace = "[" + quote + values[i] + quote;
            //                }
            //                valueReplace += "]";
            //                string replaceField = $"[\"{parametro}\"]";
            //                view.Query = view.Query.Replace(replaceField, valueReplace);
            //            }
            //            else
            //            {
            //                view.Query = view.Query.Replace(parametro, param.Value);
            //            }
            //        }
            //    }

            //}

            //else
            //{
                foreach (var param in view.Parameters)
                {

                    var parametro = "@" + param.Name;

                    if (view.Query.Contains(param.Name))
                    {
                        if (param.DataType.ToLower() == "date")
                            view.Query = view.Query.Replace(parametro, quote + param.Value + quote);
                        else if (param.DataType.ToLower() == "array")
                        {
                            string[] values = param.Value.Split(',');
                            string valueReplace = "";
                            for (int i = 0; i < values.Length; i++)
                            {
                                if (!string.IsNullOrEmpty(valueReplace)) valueReplace += "," + quote + values[i] + quote;
                                else valueReplace = "[" + quote + values[i] + quote;
                            }
                            valueReplace += "]";
                            string replaceField = $"[\"{parametro}\"]";
                            view.Query = view.Query.Replace(replaceField, valueReplace);
                        }
                        else
                        {
                            view.Query = view.Query.Replace(parametro, param.Value);
                        }
                    }
                }
            //}
            var command = view.Query.ToString();

            var list =  _context.RunCommand(command, tenanty);
            List<object> result = new List<object>();
            result = list;

            var res = result.Skip((page - 1) * limit).Take(limit);
            long count = Convert.ToInt64(result.Count());
            return new PaginationData<object>(res.ToList(), page, limit, count);

        }
        /// <summary>
        /// Execute view query from view Id
        /// </summary>
        /// <returns>Result from view </returns>
        /// <response code="200">Return a result from view query</response>
        [HttpGet("runBasic/{tenanty}")]
        public IActionResult ExternalRunBasic(string tenanty,string? viewName, string? viewId,int? page,int? limit)
        {

            // Apply Basic Authentication manually using the service\
            var basicAuth = new BasicAuthAttribute(_userService);
            basicAuth.OnAuthorization(new AuthorizationFilterContext(
                new ActionContext(HttpContext, RouteData, new ActionDescriptor()),
                new List<IFilterMetadata>()
            ));

            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }



            if (page == null) page = 1;
            if (limit == null) limit = 30;
            Views view;
            if (!string.IsNullOrEmpty(viewName))
            {
                view = _viewService.GetViewByNameExternal(viewName, tenanty);

                if (view is null)
                {
                    base.SendNotification("viewName", "View not found.");
                    return ResponseDefault();
                }

                var responseJson = RunBasic(view, tenanty, (int)page, (int)limit);

                return ResponseDefault(responseJson);
            }
            if (!string.IsNullOrEmpty(viewId))
            {
                view = _viewService.GetByIdExternal(viewId, tenanty);

                if (view is null)
                {
                    base.SendNotification("viewId", "View not found.");
                    return ResponseDefault();
                }

                var responseJson = RunBasic(view, tenanty,(int)page,(int)limit);

                return ResponseDefault(responseJson);
            }


            return ResponseDefault();
        }
        /// <summary>
        /// Get all view pagined from database
        /// </summary>
        /// <returns>Return a list from views</returns>
        /// <response code="200">Return a list from views</response>
        [HttpGet]
        public async Task<IActionResult> Get(int? page = null, int? limit = null, string sortField = null, bool sortDesc = false)
        {
            return ResponseDefault(await _viewService.GetAll(page, limit, sortField, sortDesc));
        }




        /// <summary>
        /// Search view based on especifc field
        /// </summary>
        /// <returns>Return a list of results from search</returns>
        /// <response code="200">Return a list of results from search</response>
        [HttpGet("search-fields")]
        public async Task<IActionResult> GetSearch(string fields, string search, int? page = null, int? limit = null, string sortField = null, bool sortDesc = false)
        {
            return ResponseDefault(await _viewService.SerachFields(fields, search, page, limit, sortField, sortDesc));
        }

        /// <summary>
        /// Post a new view
        /// </summary>
        /// <returns>Return a view created or notifications</returns>
        /// <response code="200">Return a view created or notifications</response>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] InsertViewCommand command)
        {
            await SendCommand(command);
            if (HasNotifications()) return ResponseDefault();
            return ResponseDefault(_viewService.GetViewByName(command.Name));
        }

        /// <summary>
        /// Update views
        /// </summary>
        /// <response code="200">Return a view updated</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] string id, [FromBody] UpdateViewCommand command)
        {
            var view = _viewService.GetById(id);
            if (view != null)
            {
                command.Id = view.Id;
                await SendCommand(command);
                if (HasNotifications()) return ResponseDefault();
                return ResponseDefault(_viewService.GetById(id));
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Delete view
        /// </summary>
        /// <response code="200">Return a view updated</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            await SendCommand(new DeleteViewCommand() { Id = id });
            return ResponseDefault();
        }
        /// <summary>
        /// Get View by Name
        /// </summary>
        /// <returns>Return success</returns>
        /// <response code="200">Return success</response>
        [Authorize]
        [HttpGet("GetByName")]
        public IActionResult Get([Required] string viewName)
        {
            var view = _viewService.GetViewByName(viewName);
            return ResponseDefault(view);
        }


        /// <summary>
        /// Publish View in any tenanty database
        /// </summary>
        /// <response code="200">Return result</response>
        [Authorize]
        [HttpPost("publish")]
        public async Task<IActionResult> Publish([Required] string url, [Required] string tenanty, [Required] string email, [Required] string password, [FromBody] InsertViewCommand viewData, string viewName = null)
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

                if (!string.IsNullOrEmpty(viewName))
                {
                    res = await client.GetAsync(url + $"/View/GetByName?viewName={viewName}");
                }
                else
                {
                    res = new HttpResponseMessage();
                }

                content = await res.Content.ReadAsStringAsync();


                var view = JsonConvert.DeserializeObject<GetViewResult>(content);
                bool success = false;
                if (view != null && view.data != null)
                {
                    //updateView
                    UpdateViewCommand viewDataUpdate = new UpdateViewCommand();
                    viewDataUpdate.Name = viewData.Name;
                    viewDataUpdate.Icon = viewData.Icon;
                    viewDataUpdate.Description = viewData.Description;
                    viewDataUpdate.Parameters = viewData.Parameters;
                    viewDataUpdate.Query = viewData.Query;
                    viewDataUpdate.SchemaName = viewData.SchemaName;
                    viewDataUpdate.Id = view.data.id;
                    dynamic body = viewDataUpdate;
                    json = JsonConvert.SerializeObject(body);
                    data = new StringContent(json, Encoding.UTF8, "application/json");
                    try { res = await client.PutAsync(url + $"/View/{view.data.id}", data); success = res.IsSuccessStatusCode; } catch (Exception) { success = false; }
                    content = await res.Content.ReadAsStringAsync();
                }

                else
                {
                    //createView

                    dynamic body = viewData;
                    json = JsonConvert.SerializeObject(body);
                    data = new StringContent(json, Encoding.UTF8, "application/json");
                    try { res = await client.PostAsync(url + $"/View", data); success = res.IsSuccessStatusCode; } catch (Exception) { success = false; }
                    content = await res.Content.ReadAsStringAsync();
                }
                if (!success) return BadRequest();
            }
            return ResponseDefault();
        }

    }
}
