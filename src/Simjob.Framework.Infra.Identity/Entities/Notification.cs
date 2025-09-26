using Simjob.Framework.Domain.Core.Entities;
using System;

namespace Simjob.Framework.Infra.Identity.Entities
{
    public class Notification : Entity
    {
        public Notification(string schemaRecordId, string schemaName, string field, string value, string valueOld, string msg, string obs, string userId, bool view, int aprovMin, AprovEmailProperties[] aprovEmail, bool? aprov)
        {
            SchemaRecordId = schemaRecordId;
            SchemaName = schemaName;
            Field = field;
            Value = value;
            ValueOld = valueOld;

            Msg = msg;
            Obs = obs;
            UserId = userId;

            AprovMin = aprovMin;
            AprovEmail = aprovEmail;
            Aprov = aprov;
            View = view;
        }

        public Notification(string msg, string obs, string userId, bool view)
        {
            Msg = msg;
            Obs = obs;
            UserId = userId;
            View = view;
        }

        public string SchemaRecordId { get; set; }
        public string SchemaName { get; set; }
        public string Field { get; set; }
        public string Value { get; set; }

        public string ValueOld { get; set; }
        public string Msg { get; set; }
        public string Obs { get; set; }
        public string UserId { get; set; }
        public int AprovMin { get; set; }
        public AprovEmailProperties[]? AprovEmail { get; set; }
        public bool? Aprov { get; set; }
        public bool View { get; set; } = false;


        public class AprovEmailProperties
        {
            public string email { get; set; }
            public bool? aprov { get; set; }
            public bool view { get; set; }
            public DateTime? data  { get; set; }
        }
    }
}
