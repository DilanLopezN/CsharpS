namespace Simjob.Framework.Domain.Models.PublishModels
{
    public class GetViewResult
    {


        public bool success { get; set; }
        public Data data { get; set; }


        public class Data
        {
            public string name { get; set; }
            public string description { get; set; }
            public object[] parameters { get; set; }
            public string query { get; set; }
            public object schemaName { get; set; }
            public string id { get; set; }
        }

    }
}
