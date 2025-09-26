using System.Collections.Generic;

namespace Simjob.Framework.Domain.Models
{
    public class PickingItemModel
    {
        public string Id { get; set; }
        public string? DescriptionItem { get; set; }
        public string? ItemId { get; set; }
        public string? OrigemId { get; set; }
        public string? StockLocCode { get; set; }
        public string? OrderItem { get; set; }
        public string? Lote { get; set; }
        public string? PedidoDeVenda { get; set; }
        public string? PedidoDeVendaItem { get; set; }
        public double? Qtd { get; set; }
        public double? QtdSeparate { get; set; }
        List<string> StockMovIds { get; set; }
    }
}
