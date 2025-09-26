namespace Simjob.Framework.Domain.Models
{
    public class StockOperationTypeModel
    {

        public string Id { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool RefDOCFISEMI { get; set; }
        public bool RefDOCFISREC { get; set; }
        public bool RefDOCOP { get; set; }
    }
}
