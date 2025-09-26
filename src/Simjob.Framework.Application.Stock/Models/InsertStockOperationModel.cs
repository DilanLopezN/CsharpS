using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Simjob.Framework.Application.Stock.Models
{
    [ExcludeFromCodeCoverage]
    public class InsertStockOperationModel
    {
        public InsertStockOperationModel()
        {
            StockMovs = new List<Dictionary<string, object>>();
        }
        public Dictionary<string, object> StockOperation { get; set; }
        public  IEnumerable<Dictionary<string, object>> StockMovs { get; set; }
        public string LotCode { get; set; }
        public bool IsReal { get; set; }
    }
}
