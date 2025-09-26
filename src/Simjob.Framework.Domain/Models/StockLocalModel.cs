namespace Simjob.Framework.Domain.Models
{
    public class StockLocalModel
    {
        public string? Id { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }

        public bool? IsAvailable { get; set; }
        public bool? IsPickingLocation { get; set; }
    }
}
