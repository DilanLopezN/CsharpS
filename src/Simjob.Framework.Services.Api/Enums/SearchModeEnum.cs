using System.Text.Json.Serialization;

namespace Simjob.Framework.Services.Api.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SearchModeEnum
    {
        Contains, Equals, Range
    }
}
