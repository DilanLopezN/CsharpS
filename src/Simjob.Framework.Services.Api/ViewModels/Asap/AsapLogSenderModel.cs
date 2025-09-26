using Newtonsoft.Json;
using System;

namespace Simjob.Framework.Services.Api.ViewModels.Asap
{
    public class AsapLogSenderModel
    {
        [JsonProperty("UserId")]
        public string UserId { get; set; }

        [JsonProperty("SentDate")]
        public DateTime SentDate { get; set; }

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("CreateAt")]
        public DateTime? CreateAt { get; set; }

        [JsonProperty("CreateBy")]
        public string? CreateBy { get; set; }

        [JsonProperty("UpdateAt")]
        public DateTime? UpdateAt { get; set; }

        [JsonProperty("UpdateBy")]
        public string? UpdateBy { get; set; }

        [JsonProperty("DeleteAt")]
        public DateTime? DeleteAt { get; set; }

        [JsonProperty("DeleteBy")]
        public string? DeleteBy { get; set; }

        [JsonProperty("IsDeleted")]
        public bool? IsDeleted { get; set; }
    }
}