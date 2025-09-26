using System;

namespace Simjob.Framework.Domain.Models
{
    public class LoteFilterModel
    {
        public string IdItem { get; set; } = "";
        public string CodigoItem { get; set; } = "";
        public string IdLote { get; set; } = "";
        public string CodLote { get; set; } = "";
        public string IdLocal { get; set; } = "";
        public string CodLocal { get; set; } = "";
        public DateTime? DataInicioSaldo { get; set; }
        public DateTime? DataFimSaldo { get; set; }
        public DateTime? DataInicioValidade { get; set; }
        public DateTime? DataFimValidade { get; set; }

        public int? Page { get; set; }
        public int? Limit { get; set; }
    }
}
