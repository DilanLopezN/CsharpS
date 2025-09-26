namespace Simjob.Framework.Domain.Models.PublishModels
{
    public class GetSchemaResult
    {


        public bool? success { get; set; }
        public Data? data { get; set; }


        public class Data
        {
            public string? name { get; set; }
            public bool? strongEntity { get; set; }
            public string? jsonValue { get; set; }
            public string? id { get; set; }
        }

    }
}
