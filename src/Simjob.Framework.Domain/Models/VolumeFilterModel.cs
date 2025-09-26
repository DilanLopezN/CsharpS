namespace Simjob.Framework.Domain.Models
{
    public class VolumeFilterModel
    {
        public string CodigoVolume { get; set; } = "";
        public string CodigoItem { get; set; } = "";
        public string CodigoLocal { get; set; } = "";
        public string CodigoLote { get; set; } = "";
        public int? Page { get; set; }
        public int? Limit { get; set; }

    }
}
