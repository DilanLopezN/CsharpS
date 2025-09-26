using Simjob.Framework.Infra.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Models
{
    public class PermissionResponse
    {
        //public PermissionResponse(string tenanty, string userId, string username, string user
        //   , DateTime created, DateTime expiration, string accessToken, int a2f, string groupId, string telefone)
        //{
        //    Tenanty = tenanty;
        //    UserId = userId;
        //    Username = username;
        //    User = user;
        //}

        public string UserId { get; set; }
        public string User { get; set; }
        public string Email { get; set; }
        public List<SchemasGroup> Schemas { get; set; }
        public List<ActionsGroup> Actions { get; set; }
    }
}
