using System;

namespace Simjob.Framework.Domain.Models
{
    public class StockFilterSaidaModel
    {
        //Picking
        public string PickingCodigo { get; set; } = "";

        public string PickingStatus { get; set; } = "";
        public string PickingNotaFiscal { get; set; } = "";

        public DateTime? PickingDtInicio { get; set; }
        public DateTime? PickingDtFinal { get; set; }

        //PickingItem
        //public string PickingItemLote { get; set; } = "";

        //StockOperation
        public string StockOperationCode { get; set; } = "";
        //public string StockOperationStatus { get; set; } = "";


        public string LocalDescription { get; set; } = "";
        public string StockVolCode { get; set; } = "";
        public string StockLotCode { get; set; } = "";

        //Item
        public string ItemCodigo { get; set; } = "";
        public string ItemDescription { get; set; } = "";


        //PREDOCEMI
        public string Status { get; set; } = "";
        //ConfirmacaoEntrega
        public string StatusEntrega { get; set; } = "";

        //NF (FISDOCREC)
        //public string NfNumero { get; set; } = "";
        //public DateTime? NfDtInicio { get; set; }
        //public DateTime? NfDtFinal { get; set; }
        //public string NfStatus { get; set; } = "";

        public int? Page { get; set; }
        public int? Limit { get; set; }
        //public string? SortField { get; set; }
        //public bool SortDesc { get; set; } = false;
        //public string Ids { get; set; } = null;
    }
}
