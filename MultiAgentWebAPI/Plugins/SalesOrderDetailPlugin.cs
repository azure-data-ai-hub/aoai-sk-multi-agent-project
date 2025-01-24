using System.ComponentModel;
using MultiAgentWebAPI.Entities;
using Microsoft.SemanticKernel;
using MultiAgentWebAPI.Utilities;

namespace MultiAgentWebAPI.Plugins
{
    public class SalesOrderDetailPlugin
    {
        [KernelFunction("get_sales_order_detail")]
        [Description("Gets Sales Order Detail Details for the given SalesOrderID.")]
        public IEnumerable<SalesOrderDetail> GetSalesOrderDetails(int SalesOrderID)
        {
            try
            {
                Console.WriteLine($"Get sales order details from CSV for: {SalesOrderID}.");

                var csvFilePath = "data/SalesOrderDetail.csv";

                var salesOrderDetails = CSVHelper.ReadRecords<SalesOrderDetail>(csvFilePath, sod => sod.SalesOrderID == SalesOrderID);

                if (salesOrderDetails != null && salesOrderDetails.Any())
                {
                    return salesOrderDetails;
                }
                else
                {
                    Console.WriteLine("No sales order detail data found in CSV.");
                    return Enumerable.Empty<SalesOrderDetail>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An exception occurred while getting sales order details: {ex.Message}");
            }
        }
    }
}