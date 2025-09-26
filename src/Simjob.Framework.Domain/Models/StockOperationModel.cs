using System;

namespace Simjob.Framework.Domain.Models
{
    public class StockOperationModel
    {
        public string? Id { get; set; }
        public string? Status { get; set; }
        public string? Code { get; set; }
        public string? StockOperationTypeId { get; set; }
        public DateTime? DateReal { get; set; }
        public string? StockLocalFromId { get; set; }
        public string? StockLocalToId { get; set; }
        public string[] StockMovIds { get; set; }

    }
}


