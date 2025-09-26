using System;

namespace Simjob.Framework.Domain.Models
{
    public class StockVolContModel
    {

        public string Id { get; set; }
        public string IdItem { get; set; }
        public string? DescriptionItem { get; set; }
        public string IdLote { get; set; }
        public string? LoteCode { get; set; }
        public DateTime? LoteExpirationDate { get; set; }
        public double Quantidade { get; set; }


    }
}
