using System.ComponentModel;
using MultiAgentWebAPI.Entities;
using Microsoft.SemanticKernel;
using MultiAgentWebAPI.Utilities;

namespace MultiAgentWebAPI.Plugins
{
    public class PurchaseOrderDetailPlugin
    {
        [KernelFunction("get_purchase_order_detail")]
        [Description("Gets Purchase Order Details for the given PurchaseOrderID.")]
        public IEnumerable<PurchaseOrderDetail> GetPurchaseOrderDetails(int PurchaseOrderID)
        {
            try
            {
                Console.WriteLine($"Get purchase order details from CSV for: {PurchaseOrderID}.");

                var csvFilePath = "data/PurchaseOrderDetail.csv";

                var purchaseOrderDetails = CSVHelper.ReadRecords<PurchaseOrderDetail>(csvFilePath, pod => pod.PurchaseOrderID == PurchaseOrderID);

                if (purchaseOrderDetails != null && purchaseOrderDetails.Any())
                {
                    return purchaseOrderDetails;
                }
                else
                {
                    Console.WriteLine("No purchase order detail data found in CSV.");
                    return Enumerable.Empty<PurchaseOrderDetail>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An exception occurred while getting purchase order details: {ex.Message}");
            }
        }
    }
}