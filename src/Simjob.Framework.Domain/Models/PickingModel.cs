using System;
using System.Collections.Generic;

namespace Simjob.Framework.Domain.Models
{
    public class PickingModel
    {
        public string? Id { get; set; }
        public string? Codigo { get; set; }
        public string? Chave { get; set; }
        public string? Confirmado { get; set; }
        public string? TipoRemessa { get; set; }
        public string? Familia { get; set; }
        public DateTime? DatePicking { get; set; }
        public DateTime? DateInitPicking { get; set; }
        public DateTime? DataPrevistaDeEntrega { get; set; }
        public string? UserInitPicking { get; set; }
        public List<string> ItemsPicking { get; set; }
        public string? LocalDest { get; set; }
        public string? LocalDestDescription { get; set; }
        public string? LocalDestDelivery { get; set; }
        public string? LocalDestRoute{ get; set; }
        public string? NotaFiscal { get; set; }
        public string? OperationStockId { get; set; }
        public string? Remessa { get; set; }
        public string? Status { get; set; }

        public string? MaterialIsBatchManaged { get; set; }
        public string? Type { get; set; }
    }
}
