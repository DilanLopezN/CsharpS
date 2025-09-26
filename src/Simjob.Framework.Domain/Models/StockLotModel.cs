using System;

namespace Simjob.Framework.Domain.Models
{
    public class StockLotModel
    {
        public string? Id { get; set; }
        public string? Code { get; set; }
        public DateTime? ExpirateDate { get; set; }
        public string? ItemId { get; set; }
    }
}
