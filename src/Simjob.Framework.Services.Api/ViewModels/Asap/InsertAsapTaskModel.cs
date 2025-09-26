using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Simjob.Framework.Services.Api.ViewModels.Asap
{
    public class InsertAsapTaskModel
    {
        public string AsapTypeCode { get; set; }
        public string Code { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string UserId { get; set; }
        public double Priority { get; set; }
        public DateTime SentDate { get; set; }

        public Dictionary<string, object> ToDictionary()
        {
            var dict = new Dictionary<string, object>();
            foreach (PropertyInfo prop in typeof(InsertAsapTaskModel).GetProperties())
            {
                dict[prop.Name] = prop.GetValue(this);
            }
            return dict;
        }
    }
}