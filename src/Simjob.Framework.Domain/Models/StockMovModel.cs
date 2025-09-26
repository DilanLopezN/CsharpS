using System;

namespace Simjob.Framework.Infra.Domain.Models
{
    public class StockMovModel
    {
        public string Id { get; set; }
        public string StockLotId { get; set; }
        public string StockOperationId { get; set; }
        public string StockVolId { get; set; }
        public string StockVolIdOrigem { get; set; }
        public string? StockLocalFromId { get; set; }
        public string StockVolCode { get; set; }
        public string? StockVolCodeOrigem { get; set; }
        public double Qty { get; set; }
        public double UnitValue { get; set; }
        public double TotalValue { get; set; }
        public DateTime DateMov { get; set; }
        public DateTime DateRes { get; set; }
        public string ItemId { get; set; }
        public string ItemDescription { get; set; }
        public string StockLotCode { get; set; }
        public string Code { get; set; }
        public string CodeNewVol { get; set; }
        public string? CodeExt { get; set; }
        public string? Unit { get; set; }
    }
}


