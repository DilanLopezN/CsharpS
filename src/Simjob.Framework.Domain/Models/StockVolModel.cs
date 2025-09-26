using System;

namespace Simjob.Framework.Domain.Models
{
    public class StockVolModel
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public string UnitId { get; set; }
        public DateTime? DateVol { get; set; }
        public string StockLocalId { get; set; }
        public string? StockLocalCode { get; set; }
        public string StockVolId { get; set; }
        public bool Closed { get; set; }
        public DateTime? BlockedDate { get; set; }
        public string BlockedBy { get; set; }
        public string[] StockVolCont { get; set; }
        public string[] StockVolIds { get; set; }
        public string VirginTag { get; set; }
        public string? OrigemId { get; set; }
        public string? OrigemCode { get; set; }

    }
}
