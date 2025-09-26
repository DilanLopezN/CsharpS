using System;

namespace Simjob.Framework.Domain.Models
{
    public class NotaFiscalModel
    {
        public string? Id { get; set; }
        public string? Numero { get; set; }
        public DateTime? DataEmissao { get; set; }
        public string? Status { get; set; }
        public string? StockOperation { get; set; }
        public string[] fisDocRecItems { get; set; }
    }
}
