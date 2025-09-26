using System;

namespace Simjob.Framework.Domain.Models
{
    public class ItemFilterModel
    {
        public string IdItem { get; set; } = "";
        public string CodigoItem { get; set; } = "";
        public string Descricao { get; set; } = "";
        public string Segmento { get; set; } = "";
        public string IdLocal { get; set; } = "";
        public string CodLocal { get; set; } = "";
        public string CodLote { get; set; } = "";
        public string CodVolume { get; set; } = "";
        public DateTime? DataInicioSaldo { get; set; }
        public DateTime? DataFimSaldo { get; set; }

        public int? Page { get; set; }
        public int? Limit { get; set; }
    }
}
