using System.Collections.Generic;
using System.Reflection;
using System;

namespace Simjob.Framework.Services.Api.ViewModels.Asap
{
    public class TaskLogModel
    {
        public string UserId { get; set; }
        public DateTime SentDate { get; set; }
        public string Id { get; set; }

        public Dictionary<string, object> ToDictionary()
        {
            var dict = new Dictionary<string, object>();
            foreach (PropertyInfo prop in typeof(TaskLogModel).GetProperties())
            {
                dict[prop.Name] = prop.GetValue(this);
            }
            return dict;
        }
    }
}