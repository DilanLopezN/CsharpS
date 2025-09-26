using System;

namespace Simjob.Framework.Domain.Models
{
    public class StockBalanceModel
    {
        public string Id { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemDescription { get; set; }
        public string? ItemUn { get; set; }
        public string StockLotCode { get; set; }
        public string StockLocCode { get; set; }
        public string? EstoqueAtual { get; set; }
        public string ItemId { get; set; }
        public string StockLotId { get; set; }
        public string StockLocId { get; set; }
        public DateTime? Date { get; set; }
        public double QtAvailable { get; set; }
        public double QtTotal { get; set; }
        public double QtReserved { get; set; }
        public double ValueAvailable { get; set; }
        public double ValueTotal { get; set; }
        public double ValueReserved { get; set; }
    }
}
