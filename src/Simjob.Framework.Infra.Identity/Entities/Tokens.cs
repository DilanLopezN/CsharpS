using Simjob.Framework.Domain.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Entities
{
    public class Tokens : Entity
    {
        public Tokens(string userId, string userToken)
        {
            UserId = userId;
            UserToken = userToken;
        }

        public Tokens(string id, string userId, string userToken)
        {
            UserId = userId;
            UserToken = userToken;
            Id = id;
        }


        public string UserId { get; set; }
        public string UserToken { get; set; }
    }
}
