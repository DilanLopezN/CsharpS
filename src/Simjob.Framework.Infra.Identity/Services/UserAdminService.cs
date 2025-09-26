using MongoDB.Driver;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Infra.Identity.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Services
{
    public class UserAdminService : IUserAdminService
    {
        private readonly UserAdminContext Context;
        protected readonly IMongoCollection<UserAdmin> Collection;
        private readonly IRepository<UserAdminContext, UserAdmin> _userAdminRepository;
        private readonly IMediatorHandler _bus;
        public UserAdminService(IRepository<UserAdminContext, UserAdmin> userAdminRepository, IMediatorHandler bus, UserAdminContext context)
        {
            _userAdminRepository = userAdminRepository;
            Collection = context.GetUserCollection();
            _bus = bus;
            Context = context;
        }

        private static string HashPassword(string userId, string password)
        {
            if (password == null || password.Length <= 0)
            {
                throw new ArgumentNullException("password");
            }

            return geraSenhaHashSHA1(password);
        }

        public static string geraSenhaHashSHA1(string credentialsPassword)
        {
            //SHA1 sha1 = new SHA1CryptoServiceProvider();
            SHA1 sha1 = SHA1.Create();
            credentialsPassword = credentialsPassword + credentialsPassword + credentialsPassword;
            byte[] data = System.Text.Encoding.ASCII.GetBytes(credentialsPassword);
            byte[] hash = sha1.ComputeHash(data);
            StringBuilder strigBuilder = new StringBuilder();
            foreach (var item in hash)
            {
                strigBuilder.Append(item.ToString("X2"));
            }

            return strigBuilder.ToString().ToUpper();
        }

        public bool Register(UserAdmin user, string password)
        {
            bool isFirstUser = !_userAdminRepository.Exists(u => u.Tenanty == user.Tenanty);
            bool userExists = false;

            if (password == null || password.Length <= 0)
            {
                return false;
            }

            if (!isFirstUser)
                userExists = _userAdminRepository.Exists(u => u.UserName == user.UserName && u.Tenanty == user.Tenanty);

            if (userExists)
            {
                _bus.RaiseEvent(new DomainNotification("UserAdminService", "username already exists"));
                return false;
            }

            string hash = HashPassword(user.Id, password);
            user.Hash = hash;

            if (isFirstUser) user.Claims.Add(new System.Security.Claims.Claim("superuser", "superuser"));
            _userAdminRepository.Insert(user);
            return true;
        }

        public void Update(UserAdmin user, string groupId)
        {
            var userToUpdate = GetUserAdminById(user.Id);
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

                _userAdminRepository.Update(userToUpdate);
            }
            else
            {
                _bus.RaiseEvent(new DomainNotification("UserService", "User not exists"));
                return;
            }
        }


        public void DeleteUserAdmin(string id)
        {
            _userAdminRepository.Delete(id);
        }

        public UserAdmin GetByUserName(string tenanty, string username)
        {
            var filterName = Builders<UserAdmin>.Filter.Eq(u => u.UserName, username.ToLower());
            var filterTenanty = Builders<UserAdmin>.Filter.Eq(u => u.Tenanty, tenanty.ToLower());
            var filterisDeleted = Builders<UserAdmin>.Filter.Eq(u => u.IsDeleted, false);

            return Collection.Find(filterName & filterTenanty & filterisDeleted).FirstOrDefault();
        }

        public UserAdmin GetUserAdminById(string id)
        {
            return Collection.Find(Builders<UserAdmin>.Filter.Eq(u => u.Id, id)).FirstOrDefault();
        }

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

            string a = hashedPassword;
            string b = HashPassword(userId, password);

            return a.Equals(b, StringComparison.OrdinalIgnoreCase);
        }
        public bool LoginAdmin(string tenanty, string username, string password)
        {
            var user = GetByUserName(tenanty.ToLower(),username.ToLower());
            if (user is null)
            {
                _bus.RaiseEvent(new DomainNotification("UserAdminService", "User not found"));
                return false;
            }

            var hashOk = VerifyHashedPassword(user.Hash, user.Id, password);
            if (!hashOk)
            {
                _bus.RaiseEvent(new DomainNotification("UserAdminService", "Username or password incorrect"));
                return false;
            }
            return true;
        }

       

        public Task<object> SearchFields(string tenanty, string value, int? page, int? limit, string sortField = null, bool sortDesc = false, string ids = null)
        {
            if (value == null) value = "";
            var repository = _userAdminRepository;
            return Task.FromResult(repository
                    .GetType().GetMethod("SearchRegexByFields")
                    .Invoke(repository, new object[] { tenanty, value, "", null, page, limit, sortField, null, sortDesc, ids, new List<string>(), "" }));
        }

        
    }
}
