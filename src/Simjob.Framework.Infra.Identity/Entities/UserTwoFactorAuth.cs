using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using NJsonSchema.Annotations;
using Simjob.Framework.Domain.Core.Entities;
using System;

namespace Simjob.Framework.Infra.Identity.Entities
{
    public class UserTwoFactorAuth : Entity
    {
        public UserTwoFactorAuth(string userId, string code, string email, string publicIP, string hash, bool authenticated)
        {
            UserId = userId;
            Code = code;
            Email = email;
            PublicIP = publicIP;
            Hash = hash;
            Authenticated = authenticated;
            CreateAt = DateTime.Now;
            ExpireAt = DateTime.Now.AddMinutes(2).ToString();
        }

        public string UserId { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
        public string PublicIP { get; set; }
        public string Hash { get; set; }
        public bool Authenticated { get; set; }
        [BsonElement("expiry")]
        public string ExpireAt { get; set; }
    }
}
