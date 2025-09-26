namespace Simjob.Framework.Domain.Models.PublishModels
{
    public class GetActionResult
    {
        public bool? success { get; set; }
        public Data? data { get; set; }
        public class Data
        {
            public string name { get; set; }
            public string id { get; set; }

        }

        public class Callconfig
        {
            public string schemaName { get; set; }
            public object triggerProperty { get; set; }
        }

    }
}
