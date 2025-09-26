using System;

namespace Simjob.Framework.Domain.Models
{
    public class StockFilterEntradaModel
    {

        public string? Nota { get; set; } = ""; //fisdocrec
        public DateTime? DtInicio { get; set; } //stockOperation
        public DateTime? DtFinal { get; set; } //stockOperation
        public string? Status { get; set; } = ""; //fisdocrec
        public string? Item { get; set; } = ""; //item - description
        public string? Volume { get; set; } = ""; //stockVol
        public string? Lote { get; set; } = ""; //stockLot

        public int? Page { get; set; }
        public int? Limit { get; set; }
        //public string? SortField { get; set; }
        //public bool SortDesc { get; set; } = false;
        //public string Ids { get; set; } = null;

    }
}
