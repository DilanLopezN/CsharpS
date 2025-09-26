namespace Simjob.Framework.Application.Stock.Models
{

}

public class StockMovModel
{
    public string Id { get; set; }
    public string StockLotId { get; set; }
    public string StockOperationId { get; set; }
    public float Qty { get; set; }
    public float UnitValue { get; set; }
    public float TotalValue { get; set; }
    public string DateMov { get; set; }
    public string DateRes { get; set; }
    public string ItemId { get; set; }
    public string ItemDescription { get; set; }
    public string StockLotCode { get; set; }
    public string Code { get; set; }
}
