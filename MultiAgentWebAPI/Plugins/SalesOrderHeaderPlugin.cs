using System.ComponentModel;
using MultiAgentWebAPI.Entities;
using Microsoft.SemanticKernel;
using MultiAgentWebAPI.Utilities;

namespace MultiAgentWebAPI.Plugins
{
    public class SalesOrderHeaderPlugin
    {
        [KernelFunction("get_sales_order_header")]
        [Description("Gets Sales Order Header Details for the given SalesOrderID.")]
        public IEnumerable<SalesOrderHeader> GetSalesOrderHeaders(int SalesOrderID)
        {
            try
            {
                Console.WriteLine($"Get sales order headers from CSV for: {SalesOrderID}.");

                var csvFilePath = "data/SalesOrderHeader.csv";

                var salesOrderHeaders = CSVHelper.ReadRecords<SalesOrderHeader>(csvFilePath, soh => soh.SalesOrderID == SalesOrderID);

                if (salesOrderHeaders != null && salesOrderHeaders.Any())
                {
                    return salesOrderHeaders;
                }
                else
                {
                    Console.WriteLine("No sales order header data found in CSV.");
                    return Enumerable.Empty<SalesOrderHeader>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An exception occurred while getting sales order headers: {ex.Message}");
            }
        }
    }
}