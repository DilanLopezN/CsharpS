using Simjob.Framework.Domain.Core.Entities;
using Simjob.Framework.Infra.Identity.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Entities
{
    public class UserAccess : Entity
    {
        public UserAccess(string userId, string tenanty, string userName, string latitude, string longitude, string ip, string userAgent, string schemaName, string description, string schemaRecordId, Dictionary<string, string> valorOriginal, Dictionary<string, string> valorAlterado)
        {
            UserId = userId;
            Tenanty = tenanty;
            UserName = userName;
            Latitude = latitude;
            Longitude = longitude;
            Ip = ip;
            UserAgent = userAgent;
            SchemaName = schemaName;
            SchemaRecordId = schemaRecordId;
            Description = description;
            ValorOriginal = valorOriginal;
            ValorAlterado = valorAlterado;
        }

        public string UserId { get; set; }
        public string Tenanty { get; set; }
        public string UserName { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Ip { get; set; }
        public string UserAgent { get; set; }
        public string SchemaName { get; set; }
        public string Description { get; set; }
        public string SchemaRecordId { get; set; }
        public Dictionary<string, string> ValorOriginal { get; set; }
        public Dictionary<string, string> ValorAlterado { get; set; }
    }
}
