using System.ComponentModel;
using MultiAgentWebAPI.Entities;
using Microsoft.SemanticKernel;
using MultiAgentWebAPI.Utilities;

namespace MultiAgentWebAPI.Plugins
{
    public class PurchaseOrderHeaderPlugin
    {
        [KernelFunction("get_purchase_order_header")]
        [Description("Gets Purchase Order Header Details for the given PurchaseOrderID.")]
        public IEnumerable<PurchaseOrderHeader> GetPurchaseOrderHeaders(int PurchaseOrderID)
        {
            try
            {
                Console.WriteLine($"Get purchase order headers from CSV for: {PurchaseOrderID}.");

                var csvFilePath = "data/PurchaseOrderHeader.csv";

                var purchaseOrderHeaders = CSVHelper.ReadRecords<PurchaseOrderHeader>(csvFilePath, poh => poh.PurchaseOrderID == PurchaseOrderID);

                if (purchaseOrderHeaders != null && purchaseOrderHeaders.Any())
                {
                    return purchaseOrderHeaders;
                }
                else
                {
                    Console.WriteLine("No purchase order header data found in CSV.");
                    return Enumerable.Empty<PurchaseOrderHeader>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An exception occurred while getting purchase order headers: {ex.Message}");
            }
        }
    }
}