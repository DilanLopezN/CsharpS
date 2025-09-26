using System.Collections.Generic;
using System;
using Newtonsoft.Json;

namespace Simjob.Framework.Services.Api.ViewModels.User
{
    public class UserModel
    {
        [JsonProperty("UserName")]
        public string UserName { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Tenanty")]
        public string Tenanty { get; set; }

        [JsonProperty("ApiKey")]
        public string ApiKey { get; set; }

        [JsonProperty("Telefone")]
        public string Telefone { get; set; }

        [JsonProperty("CompanySiteIdDefault")]
        public string CompanySiteIdDefault { get; set; }

        [JsonProperty("CompanySiteIds")]
        public List<string> CompanySiteIds { get; set; }

        //[JsonProperty("Claims")]
        //public List<string> Claims { get; set; }

        [JsonProperty("Hash")]
        public string Hash { get; set; }

        [JsonProperty("SuperiorUser")]
        public string SuperiorUser { get; set; }

        [JsonProperty("Function")]
        public string Function { get; set; }

        [JsonProperty("Picture")]
        public string Picture { get; set; }

        [JsonProperty("A2f")]
        public int A2f { get; set; }

        [JsonProperty("GroupId")]
        public string GroupId { get; set; }

        [JsonProperty("Root")]
        public bool Root { get; set; }

        [JsonProperty("FirstLogin")]
        public bool FirstLogin { get; set; }

        [JsonProperty("LogonAzure")]
        public bool LogonAzure { get; set; }

        [JsonProperty("HashAzure")]
        public string HashAzure { get; set; }

        [JsonProperty("RevendaId")]
        public string RevendaId { get; set; }

        [JsonProperty("NivelId")]
        public string NivelId { get; set; }

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("CreateAt")]
        public DateTime CreateAt { get; set; }

        [JsonProperty("CreateBy")]
        public string CreateBy { get; set; }

        [JsonProperty("UpdateAt")]
        public DateTime? UpdateAt { get; set; }

        [JsonProperty("UpdateBy")]
        public string UpdateBy { get; set; }

        [JsonProperty("DeleteAt")]
        public DateTime? DeleteAt { get; set; }

        [JsonProperty("DeleteBy")]
        public string DeleteBy { get; set; }

        [JsonProperty("IsDeleted")]
        public bool IsDeleted { get; set; }
    }
}