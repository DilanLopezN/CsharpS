using System;
using System.Text.Json.Serialization;

namespace Simjob.Framework.Domain.Models
{
    public class OperMovFilterModel
    {
        public DateTime? DataInicioReal { get; set; }
        public DateTime? DataFimReal { get; set; }
        public string IdOperacao { get; set; } = "";
        public string CodigoOperacao { get; set; } = "";
        public string IdVolume { get; set; } = "";
        public string CodigoVolume { get; set; } = "";
        public string IdItem { get; set; } = "";
        public string CodigoItem { get; set; } = "";
        public string IdLote { get; set; } = "";
        public string CodigoLote { get; set; } = "";
        public string IdLocal { get; set; } = "";
        public string CodigoLocal { get; set; } = "";
        public string DescricaoLocal { get; set; } = "";
        public int? Page { get; set; }
        public int? Limit { get; set; }
        public GroupBy? GroupBy { get; set; }
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GroupBy
    {
        Item,
        Lote,
        Volume
    }
}
