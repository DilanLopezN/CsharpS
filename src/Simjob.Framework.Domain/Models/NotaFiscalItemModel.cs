namespace Simjob.Framework.Domain.Models
{
    public class NotaFiscalItemModel
    {
        public string? Id { get; set; }
        public string? Seqitem { get; set; }
        public string? ItemId { get; set; }
        public string? Description { get; set; }
        public string? Unit { get; set; }
        public string? CodPurchaseOrder { get; set; }
        public int? SeqItemPurchaseOrder { get; set; }
        public string? DescriptionItem { get; set; }
        public double? Qty { get; set; }
        public double? Qtd_recebido { get; set; }
        public double? Vlrunit { get; set; }
        public double? Vlrtotal { get; set; }
        public string? Obs { get; set; }
        public string? Fisoper { get; set; }
        public double? ICMSBaseCalculo { get; set; }
        public double? ICMSPercIcms { get; set; }
        public double? ICMSValor { get; set; }
        public double? IPIBaseCalculo { get; set; }
        public double? IPIPercIPI { get; set; }
        public double? IPIValor { get; set; }
        public double? PISBaseCalculo { get; set; }
        public double? PISPerc { get; set; }
        public double? PISValor { get; set; }
        public double? COFINSBaseCalculo { get; set; }
        public double? COFINSPerc { get; set; }
        public double? COFINSValor { get; set; }
        public double? CFOP { get; set; }
        public double? NCM { get; set; }
    }
}
