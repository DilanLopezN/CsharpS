namespace Simjob.Framework.Domain.Models.PublishModels
{
    public class Token
    {
        public bool success { get; set; }
        public Data data { get; set; }
        public class Data
        {
            public string accessToken { get; set; }
        }


    }

}
