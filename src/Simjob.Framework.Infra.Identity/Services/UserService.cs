using MediatR;
using Microsoft.Extensions.Primitives;
using MongoDB.Bson;
using MongoDB.Driver;
using MoreLinq;
using Newtonsoft.Json;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Domain.Util;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Infra.Identity.Models;
using Simjob.Framework.Infra.Schemas.Entities;
using Simjob.Framework.Infra.Schemas.Interfaces;
using Simjob.Framework.Infra.Schemas.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Services
{
    public class UserService : IUserService
    {
        private readonly IdentityContext Context;
        private readonly DomainNotificationHandler _notifications;
        protected readonly IMongoCollection<User> Collection;
        private readonly IMediatorHandler _bus;
        private readonly IRepository<IdentityContext, User> _userRepository;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;
        private readonly ISchemaBuilder _schemaBuilder;
        private readonly IServiceProvider _serviceProvider;

        public UserService(IdentityContext context, INotificationHandler<DomainNotification> notifications, IMediatorHandler bus,
            IRepository<IdentityContext, User> userRepository, IRepository<MongoDbContext, Schema> schemaRepository, ISchemaBuilder schemaBuilder, IServiceProvider serviceProvider)
        {
            Context = context;
            _notifications = (DomainNotificationHandler)notifications;
            Collection = context.GetUserCollection();
            _bus = bus;
            _userRepository = userRepository;
            _schemaRepository = schemaRepository;
            _schemaBuilder = schemaBuilder;
            _serviceProvider = serviceProvider;
        }

        private static string HashPassword(string userId, string password)
        {
            if (password == null || password.Length <= 0)
            {
                throw new ArgumentNullException("password");
            }

            // Usa o hash Simjob (padrão do sistema) para novos usuários
            return GeraHashSimjob(userId, password);
        }

        public static string GeraHashSimjob(string userId, string password)
        {
            var environmentSalt = EnvironmentSettings.Get("TOP_LEVEL_SALT", "");
            byte[] part_01, part_02, part_03, final;

            using (var sha1 = System.Security.Cryptography.SHA384.Create())
            {
                byte[] part_01_raw_bytes = Encoding.UTF8.GetBytes(userId.ToLower());
                byte[] part_02_raw_bytes = Encoding.UTF8.GetBytes(environmentSalt);
                byte[] part_03_raw_bytes = Encoding.UTF8.GetBytes(password);

                part_01 = sha1.ComputeHash(part_01_raw_bytes);
                part_02 = sha1.ComputeHash(part_02_raw_bytes);
                part_03 = sha1.ComputeHash(part_03_raw_bytes);

                final = sha1.ComputeHash(part_01.Concat(part_02).Concat(part_03).ToArray());
            }

            return BitConverter.ToString(final).Replace("-", "").ToLower();
        }

        public static string GeraHashAntigo(string password)
        {
            //SHA1 sha1 = new SHA1CryptoServiceProvider();
            SHA1 sha1 = SHA1.Create();
            string credentialsPassword = password + password + password;
            byte[] data = System.Text.Encoding.ASCII.GetBytes(credentialsPassword);
            byte[] hash = sha1.ComputeHash(data);
            StringBuilder strigBuilder = new StringBuilder();
            foreach (var item in hash)
            {
                strigBuilder.Append(item.ToString("X2"));
            }

            return strigBuilder.ToString().ToUpper();
        }

        [ExcludeFromCodeCoverage]
        private bool VerifyHashedPassword(string hashedPassword, string userId, string password)
        {
            if (hashedPassword == null)
            {
                return false;
            }
            if (password == null || String.IsNullOrEmpty(password))
            {
                return false;
                //throw new ArgumentNullException("password");
            }

            // 1. Tenta primeiro com hash Simjob (padrão do sistema)
            string hashSimjob = GeraHashSimjob(userId, password);
            if (hashedPassword.Equals(hashSimjob, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // 2. Se não funcionou, tenta com hash antigo (usuários migrados)
            string hashAntigo = GeraHashAntigo(password);
            if (hashedPassword.Equals(hashAntigo, StringComparison.OrdinalIgnoreCase))
            {
                // Opcional: Migrar automaticamente para hash Simjob
                MigrarParaHashSimjob(userId, hashSimjob);
                return true;
            }

            return false;
        }

        private void MigrarParaHashSimjob(string userId, string novoHashSimjob)
        {
            try
            {
                var userIdFilter = Builders<User>.Filter.Eq(u => u.Id, userId);
                var hashUserUpdate = Builders<User>.Update.Set(u => u.Hash, novoHashSimjob);
                Collection.UpdateOne(userIdFilter, hashUserUpdate);
            }
            catch (Exception)
            {
                // Se falhar a migração, não impede o login
                // Log opcional aqui se necessário
            }
        }

        public bool Register(User user, string password)
        {
            bool isFirstUser = !_userRepository.Exists(u => u.Tenanty == user.Tenanty);
            bool userExists = false;

            if (password == null || password.Length <= 0)
            {
                return false;
            }

            if (!isFirstUser)
                userExists = _userRepository.Exists(u => u.UserName == user.UserName && u.Tenanty == user.Tenanty);

            if (userExists)
            {
                _bus.RaiseEvent(new DomainNotification("UserService", "username already exists"));
                return false;
            }

            string hash = HashPassword(user.Id, password);
            user.Hash = hash;

            if (isFirstUser) user.Claims.Add(new System.Security.Claims.Claim("superuser", "superuser"));
            _userRepository.Insert(user);
            return true;
        }

        public User GetUserById(string id)
        {
            return Collection.Find(Builders<User>.Filter.Eq(u => u.Id, id)).FirstOrDefault();
        }

        public User GetUserByUserName(string userName, string tenanty)
        {
            var userNameFilter = Builders<User>.Filter.Eq(u => u.UserName, userName);
            var tenantyFilter = Builders<User>.Filter.Eq(u => u.Tenanty, tenanty);
            var deletFilter = Builders<User>.Filter.Eq(u => u.IsDeleted, false);

            return Collection.Find(userNameFilter & tenantyFilter & deletFilter).FirstOrDefault();
        }

        public int EnableOrDisableA2F(User user, int a2f)
        {
            try
            {
                var userIdFilter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
                var userUpdateA2F = Builders<User>.Update.Set(u => u.A2f, a2f);
                var update = Collection.UpdateOne(userIdFilter, userUpdateA2F);
                if (a2f == 1)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                throw;
            }
        }

        public void UpdateRoot(User user, bool level)
        {
            if (user.Root == true)
            {
                try
                {
                    var userIdFilter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
                    var userUpdateLevel = Builders<User>.Update.Set(u => u.Root, level);
                    var update = Collection.UpdateOne(userIdFilter, userUpdateLevel);
                }
                catch
                {
                    throw;
                }
            }
        }

        public void UpdateUserName(User user, string newUserName)
        {
            var exists = GetUserByUserName(newUserName, user.Tenanty) != null;

            if (exists)
            {
                _bus.RaiseEvent(new DomainNotification("UserService", "Username already exists"));
                return;
            }

            var userIdFilter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
            var userUserNameUpdate = Builders<User>.Update.Set(u => u.UserName, newUserName);
            Collection.UpdateOne(userIdFilter, userUserNameUpdate);
        }

        public void UpdateHashAzure(User user, string newHashAzure)
        {
            var userIdFilter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
            var userHashAzureUpdate = Builders<User>.Update.Set(u => u.HashAzure, newHashAzure);
            Collection.UpdateOne(userIdFilter, userHashAzureUpdate);
        }

        public void UpdateLogonAzure(User user, bool logonAzure)
        {
            var userIdFilter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
            var userLogonAzureUpdate = Builders<User>.Update.Set(u => u.LogonAzure, logonAzure);
            Collection.UpdateOne(userIdFilter, userLogonAzureUpdate);
        }

        public void UpdatePassword(User user, string oldPassword, string newPassword)
        {
            bool valid = VerifyHashedPassword(user.Hash, user.Id, oldPassword);
            if (!valid)
            {
                _bus.RaiseEvent(new DomainNotification("UserService", "Password Incorrect"));
                return;
            }

            if (newPassword.Length < 6)
            {
                _bus.RaiseEvent(new DomainNotification("UserService", "Passwords must be at least 6 characters"));
                return;
            }

            if (!newPassword.Any(c => char.IsDigit(c)))
            {
                _bus.RaiseEvent(new DomainNotification("UserService", "Required a minimum of 1 numeric character [0-9]"));
                return;
            }

            if (!newPassword.Any(c => char.IsUpper(c)))
            {
                _bus.RaiseEvent(new DomainNotification("UserService", "Required a minimum of 1 upper case letter [A-Z] "));
                return;
            }

            if (!newPassword.Any(c => char.IsLower(c)))
            {
                _bus.RaiseEvent(new DomainNotification("UserService", "Required a minimum of 1 lower case letter [a-z]"));
                return;
            }

            Regex rgx = new("[^A-Za-z0-9]");
            bool hasSpecialChars = rgx.IsMatch(newPassword.ToString());
            if (!hasSpecialChars)
            {
                _bus.RaiseEvent(new DomainNotification("UserService", "Required a minimum of 1 special character"));
                return;
            }

            user.FirstLogin = false;

            ResetPassword(user, newPassword);
        }

        public void UpdatePasswordAdmin(User user, string newPassword)
        {
            user.FirstLogin = true;
            ResetPassword(user, newPassword);
        }
        public void UpdatePasswordLogin(User user, string newPassword)
        {
            user.FirstLogin = false;
            ResetPassword(user, newPassword);
        }
        public void UpdateGroup(User user, string groupId)
        {
            var userToUpdate = GetUserById(user.Id);
            if (userToUpdate != null)
            {
                userToUpdate.Name = user.Name ?? userToUpdate.Name;
                userToUpdate.UserName = user.UserName ?? userToUpdate.UserName;
                userToUpdate.Telefone = user.Telefone ?? userToUpdate.Telefone;
                userToUpdate.GroupId = user.GroupId ?? userToUpdate.GroupId;
                userToUpdate.Tenanty = userToUpdate.Tenanty;
                userToUpdate.Root = user.Root;
                userToUpdate.LogonAzure = user.LogonAzure;
                userToUpdate.NivelId = user.NivelId;
                userToUpdate.RevendaId = user.RevendaId;
                userToUpdate.CompanySiteIdDefault = user.CompanySiteIdDefault;
                userToUpdate.CompanySiteIds = user.CompanySiteIds;
                userToUpdate.UpdateBy = user.UpdateBy;

                _userRepository.Update(userToUpdate);
            }
            else
            {
                _bus.RaiseEvent(new DomainNotification("UserService", "User not exists"));
                return;
            }
        }

        public void ResetPassword(User user, string newPassword)
        {
            string hash = HashPassword(user.Id, newPassword);
            var userIdFilter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
            var hashUserUpdate = Builders<User>.Update.Set(u => u.Hash, hash).Set(u => u.FirstLogin, user.FirstLogin);
            Collection.UpdateOne(userIdFilter, hashUserUpdate);
        }

        public bool Login(string tenanty, string username, string password)
        {
            //var user = _userRepository.Search(u => u.UserName.ToLower() == username.ToLower() && u.Tenanty.ToLower() == tenanty.ToLower())?.Data?.FirstOrDefault();
            var user = GetUserByUserName(username.ToLower(), tenanty.ToLower());
            if (user is null)
            {
                _bus.RaiseEvent(new DomainNotification("UserService", "User not found"));
                return false;
            }

            var hashOk = VerifyHashedPassword(user.Hash, user.Id, password);
            if (!hashOk)
            {
                _bus.RaiseEvent(new DomainNotification("UserService", "Username or password incorrect"));
                return false;
            }
            return true;
        }

        public User GetByUserName(string tenanty, string username)
        {
            var filterName = Builders<User>.Filter.Eq(u => u.UserName, username.ToLower());
            var filterTenanty = Builders<User>.Filter.Eq(u => u.Tenanty, tenanty.ToLower());
            var filterisDeleted = Builders<User>.Filter.Eq(u => u.IsDeleted, false);

            return Collection.Find(filterName & filterTenanty & filterisDeleted).FirstOrDefault();
        }

        public List<User> GetUsersByUserName(string[] userName, string tenanty)
        {
            return Collection.Find(Builders<User>.Filter.In(u => u.UserName, userName) & Builders<User>.Filter.Eq(u => u.Tenanty, tenanty) & Builders<User>.Filter.Eq(u => u.IsDeleted, false)).ToList();
        }

        public List<User> GetUsersByGroupId(string groupId)
        {
            return Collection.Find(Builders<User>.Filter.Eq(u => u.GroupId, groupId) & Builders<User>.Filter.Eq(u => u.IsDeleted, false)).ToList();
        }
        public List<User> GetUsersByGroupIds(List<string> groupIds)
        {
            return Collection.Find(Builders<User>.Filter.In(u => u.GroupId, groupIds) & Builders<User>.Filter.Eq(u => u.IsDeleted, false)).ToList();
        }
        public List<User> GetUsersByUserIds(List<string> userIds, string tenanty)
        {
            return Collection.Find(Builders<User>.Filter.In(u => u.Id, userIds) & Builders<User>.Filter.Eq(u => u.Tenanty, tenanty) & Builders<User>.Filter.Eq(u => u.IsDeleted, false)).ToList();
        }

        public List<User> GetUsersByTenanty(string tenanty)
        {
            return Collection.Find(Builders<User>.Filter.Eq(u => u.Tenanty, tenanty) & Builders<User>.Filter.Eq(u => u.IsDeleted, false)).ToList();
        }

        public List<User> GetUsersByTenantyAndGroupID(string tenanty, string groupId)
        {
            return Collection.Find(Builders<User>.Filter.Eq(u => u.Tenanty, tenanty) & Builders<User>.Filter.Eq(u => u.GroupId, groupId) & Builders<User>.Filter.Eq(u => u.IsDeleted, false)).ToList();
        }

        public object GetRepository(Type schemaType)
        {
            var typeRepo = typeof(IRepository<,>).MakeGenericType(typeof(MongoDbContext), schemaType);
            return _serviceProvider.GetService(typeRepo);
        }

        public Task<object> SerachFields(string tenanty, string value, int? page, int? limit, string sortField = null, bool sortDesc = false, string ids = null)
        {
            if (value == null) value = "";
            var repository = _userRepository;
            return Task.FromResult(repository
                    .GetType().GetMethod("SearchRegexByFields")
                    .Invoke(repository, new object[] { tenanty, value, "", null, page, limit, sortField, null, sortDesc, ids, new List<string>(), "" }));
        }

        public PaginationData<User> SearchFieldsByTenanty(string groupId, string name, string value, int? page, int? limit, string sortField = null, bool sortDesc = false, string ids = null, string companySiteId = null, bool usuarioMatriz = false)
        {
            SortDefinition<User> sort = null;
            if (page == null) page = 1;
            if (limit == null) limit = 10;
            var filterIsDeleted = Builders<User>.Filter.Eq(u => u.IsDeleted, false);
            var filterRegex = string.IsNullOrEmpty(name) ? Builders<User>.Filter.Empty : Builders<User>.Filter.Regex(u => u.Name, new BsonRegularExpression($"/^.*{name}.*$/i"));
            var filterTenanty = Builders<User>.Filter.Eq(u => u.Tenanty, value);
            var filterGroup = string.IsNullOrEmpty(groupId) ? Builders<User>.Filter.Empty : Builders<User>.Filter.Eq(u => u.GroupId, groupId);
            var filterIds = string.IsNullOrEmpty(ids) ? Builders<User>.Filter.Empty : Builders<User>.Filter.In("Id", ids.Split(','));
            var filterCompanySiteId = string.IsNullOrEmpty(companySiteId) ? Builders<User>.Filter.Empty : Builders<User>.Filter.In("CompanySiteIds", new[] { companySiteId });
            var filterMatriz = !usuarioMatriz
                ? Builders<User>.Filter.Ne(u => u.UsuarioMatriz, true)
                : Builders<User>.Filter.Empty;
            if (sortField != null) sort = sortDesc ? Builders<User>.Sort.Descending(sortField) : Builders<User>.Sort.Ascending(sortField);
            var filters = filterIsDeleted & filterRegex & filterTenanty & filterGroup & filterIds & filterCompanySiteId & filterMatriz;
            var listBusca = string.IsNullOrEmpty(sortField) ? Collection.Find(filters).Sort(sort).ToList() : Collection.Find(filters).ToList();
            var res = listBusca.Skip((int)((page - 1) * limit)).Take((int)limit);

            long count = Convert.ToInt64(listBusca.Count());
            return new PaginationData<User>(res.ToList(), page, limit, count);
        }

        public async Task<IEnumerable<ModuleModel>> GetModules()
        {
            await _schemaBuilder.InsertInternSchemas();
            var schemas = _schemaRepository.GetAll()?.Data?.Select(s => s.JsonValue);
            var schemaModels = schemas.Select(s => JsonConvert.DeserializeObject<SchemaModel>(s)).ToList();
            var schemaModuleDistinct = schemaModels.Select(s => s.Module);
            var modules = schemaModels
                .Where(m => m.Module != null && !m.Intern && m.Form != null && m.Form.Keys.Any())
                .Select(m => new ModuleModel { Title = m.Module, Icon = m.Icon })
                .DistinctBy(s => s.Title)
                .ToList();

            foreach (var module in modules)
            {
                var schemasInModule = schemaModels
                    .Where(model => model.Module == module.Title)
                    .ToList();

                module.Groups = schemasInModule
                                    .Where(p => p.Principal && !p.Intern && p.Form != null && p.Form.Keys.Any())
                                    .Select(s => new ModuleModel { Title = s.Title, Name = s.Description, Icon = s.Icon })
                    .ToList();
            }

            return modules.SelectMany(m => m.Groups).DistinctBy(g => g.Title).ToList();
        }

        public async Task<IEnumerable<ModulePermissionActionModel>> GetModulesPermissions(Permission permission)
        {
            await _schemaBuilder.InsertInternSchemas();
            var schemas = _schemaRepository.GetAll()?.Data?.Select(s => s.JsonValue);
            var schemaModels = schemas.Select(s => JsonConvert.DeserializeObject<SchemaModel>(s)).ToList();
            var schemaModuleDistinct = schemaModels.Select(s => s.Module);

            List<SchemasGroup> listaSchemas = new();
            List<ActionsGroup> listaActions = new();
            if (permission != null)
            {
                if (permission.Schemas != null)
                {
                    foreach (var perm in permission.Schemas)
                    {
                        if (perm.Permissions.Count > 0)
                        {
                            SchemasGroup schemaToAdd = new(perm.SchemaName, perm.SchemaID, perm.Permissions, perm.Actions);
                            listaSchemas.Add(schemaToAdd);
                        }
                    }
                }

                List<ModulePermissionActionModel> modules = new();
                Permission newPermission = new(listaSchemas, permission.UserID, permission.Name, permission.Email);
                foreach (var teste in newPermission.Schemas)
                {
                    var item = schemaModels
                    .Where(m => m.Module != null && m.Principal && !m.Intern && m.Form != null && m.Form.Keys.Any())
                    .Where(m => m.Title == teste.SchemaName)
                    .Select(m => new ModulePermissionActionModel { Title = m.Title, Icon = m.Icon, Permissions = teste.Permissions, Name = m.Description, Actions = teste.Actions })
                    .DistinctBy(s => s.Title).FirstOrDefault();

                    if (item != null) modules.Add(item);
                }
                return modules;
            }
            return default;
        }

        public void DeleteUser(string id)
        {
            _userRepository.Delete(id);
            //var usuario = GetUserById(id);
            //var userIdFilter = Builders<User>.Filter.Eq(u => u.Id,usuario.Id);
            //var UserUpdate = Builders<User>.Update.Set(u => u.IsDeleted, true);
            //Collection.UpdateOne(userIdFilter, UserUpdate);
        }

        public List<User> GetByUserNameRegex(string id, string name)
        {
            return Collection.Find(
                Builders<User>.Filter.Eq(u => u.GroupId, id) &
                Builders<User>.Filter.Eq(u => u.IsDeleted, false) &
                Builders<User>.Filter.Regex(u => u.Name, new BsonRegularExpression($"/^.*{name}.*$/i"))).ToList();
        }

        public object ConvertOwnerIdSingle(object response, string userField)
        {
            if (userField != null)
            {
                var stringBuilder = new StringBuilder(userField);
                stringBuilder[0] = char.ToUpper(stringBuilder[0]);
                userField = stringBuilder.ToString();
                var userValue = response.GetType().GetProperty(userField).GetValue(response, null);
                if (userValue != null)
                {
                    dynamic user = GetUserById((string)userValue);
                    if (user != null)
                    {
                        response.GetType().GetProperty(userField).SetValue(response, new { Name = user.Name, UserId = user.Id }, null);
                    }
                    else
                    {
                        response.GetType().GetProperty(userField).SetValue(response, new { Name = "", UserId = "" }, null);
                    }
                }
            }
            if (response.GetType().GetProperty("Owners") != null)
            {
                var owner = response.GetType().GetProperty("Owners");
                dynamic getValue = response.GetType().GetProperty("Owners").GetValue(response, null);
                if (getValue != null)
                {
                    List<object> names = new List<object>();

                    foreach (var ownersId in getValue)
                    {
                        dynamic user = GetUserById(ownersId);
                        if (user == null)
                        {
                            names.Add(new { UserName = "", UserId = "" });
                        }
                        else
                        {
                            Type type = response.GetType();
                            System.Reflection.PropertyInfo propertyInfo = type.GetProperty("Owners");

                            names.Add(new { UserName = user.Name, UserId = user.Id });
                        }
                    }
                    response.GetType().GetProperty("Owners").SetValue(response, names, null);
                }
            }
            return response;
        }

        public object ConvertOwnerId(object response, string userField)
        {
            foreach (var propInfo in response.GetType().GetProperties())
            {
                var val = propInfo.GetValue(response, null);

                if (typeof(IEnumerable).IsAssignableFrom(propInfo.PropertyType))
                {
                    var collectionItems = (IEnumerable)val;
                    var lista = collectionItems.Cast<dynamic>().ToList();

                    foreach (dynamic v in lista)
                    {
                        if (userField != null)
                        {
                            var stringBuilder = new StringBuilder(userField);
                            stringBuilder[0] = char.ToUpper(stringBuilder[0]);
                            userField = stringBuilder.ToString();

                            var userValue = v.GetType().GetProperty(userField).GetValue(v, null);

                            if (userValue != null)
                            {
                                dynamic user = GetUserById(userValue);
                                if (user != null)
                                {
                                    v.GetType().GetProperty(userField).SetValue(v, new { Name = user.Name, UserId = user.Id }, null);
                                }
                                else
                                {
                                    v.GetType().GetProperty(userField).SetValue(v, new { Name = "", UserId = "" }, null);
                                }
                            }
                        }

                        if (v.GetType().GetProperty("Owners") != null)
                        {
                            var owner = v.GetType().GetProperty("Owners");
                            var getValue = v.GetType().GetProperty("Owners").GetValue(v, null);
                            if (getValue != null)
                            {
                                List<object> names = new List<object>();

                                foreach (var ownersId in getValue)
                                {
                                    dynamic user = GetUserById(ownersId);
                                    if (user == null)
                                    {
                                        names.Add(new { UserName = "", UserId = "" });
                                    }
                                    else
                                    {
                                        Type type = v.GetType();
                                        System.Reflection.PropertyInfo propertyInfo = type.GetProperty("Owners");

                                        names.Add(new { UserName = user.Name, UserId = user.Id });
                                    }
                                }
                                v.GetType().GetProperty("Owners").SetValue(v, names, null);
                            }
                        }
                    }
                }
            }
            return response;
        }

        public string ClaimUserId(StringValues accessToken)
        {
            var userId = "";
            if (accessToken != "")
            {
                var token = accessToken.ToString().Split(" ");
                var onlyToken = "";
                foreach (var itens in token) { if (itens.Length > 10) onlyToken = itens; }
                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(onlyToken);
                var claims = jwtSecurityToken.Claims.ToList();
                foreach (var claim in claims)
                {
                    if (claim.Type == "userid")
                    {
                        userId = claim.Value;
                        break;
                    }
                }
            }
            return userId;
        }

        public string ClaimCompanySiteIds(StringValues accessToken)
        {
            var companySiteIds = "";
            if (accessToken != "")
            {
                var token = accessToken.ToString().Split(" ");
                var onlyToken = "";
                foreach (var itens in token) { if (itens.Length > 10) onlyToken = itens; }
                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(onlyToken);
                var claims = jwtSecurityToken.Claims.ToList();
                foreach (var claim in claims)
                {
                    if (claim.Type == "companySiteIdDefault")
                    {
                        if (!string.IsNullOrEmpty(claim.Value)) companySiteIds = claim.Value;
                    }

                    if (claim.Type == "companySiteIds")
                    {
                        if (!string.IsNullOrEmpty(claim.Value)) companySiteIds += "," + claim.Value;
                        break;
                    }
                }
            }
            return companySiteIds;
        }

        public List<User> GetUsersByField()
        {
            return _userRepository.GetUsersByField();
        }

        public void UpdateApiKey(User user, string encryptedKey)
        {
            var userIdFilter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
            var userUserNameUpdate = Builders<User>.Update.Set(u => u.ApiKey, encryptedKey);
            Collection.UpdateOne(userIdFilter, userUserNameUpdate);
        }

        public User GetByApiKey(string apiKey)
        {
            return Collection.Find(Builders<User>.Filter.Eq(u => u.ApiKey, EncryptorDecryptor.EncryptApiKey(apiKey))).FirstOrDefault();
        }


    }
}