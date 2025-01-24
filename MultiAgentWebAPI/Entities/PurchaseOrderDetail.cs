namespace MultiAgentWebAPI.Entities;
public class PurchaseOrderDetail
{
    public int PurchaseOrderID { get; set; }
    public int PurchaseOrderDetailID { get; set; }
    public DateTime DueDate { get; set; }
    public int OrderQty { get; set; }
    public int ProductID { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
    public int ReceivedQty { get; set; }
    public int RejectedQty { get; set; }
    public int StockedQty { get; set; }
    public DateTime ModifiedDate { get; set; }
}