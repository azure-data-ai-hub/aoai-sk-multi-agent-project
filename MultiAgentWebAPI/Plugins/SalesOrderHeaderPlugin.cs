using System.ComponentModel;
using MultiAgentWebAPI.Entities;
using Microsoft.SemanticKernel;
using MultiAgentWebAPI.Utilities;

namespace MultiAgentWebAPI.Plugins
{
    public class SalesOrderHeaderPlugin
    {
        [KernelFunction("get_sales_order_header")]
        [Description("Gets all Sales Order Header Details.")]
        public IEnumerable<SalesOrderHeader> GetSalesOrderHeaders()
        {
            try
            {
                Console.WriteLine($"Get sales order headers from CSV");

                var csvFilePath = "data/SalesOrderHeader.csv";

                var salesOrderHeaders = CSVHelper.ReadRecords<SalesOrderHeader>(csvFilePath, soh => soh.SalesOrderID > 0);

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

        [KernelFunction("get_sales_order_header_with_dates")]
        [Description("Gets Sales Order Header Details for the given from date and to date")]
        public IEnumerable<SalesOrderHeader> GetSalesOrderHeaders(DateTime from, DateTime to)
        {
            try
            {
                Console.WriteLine($"Get sales order headers from CSV");

                var csvFilePath = "data/SalesOrderHeader.csv";

                var salesOrderHeaders = CSVHelper.ReadRecords<SalesOrderHeader>(csvFilePath, soh => soh.OrderDate >= from && soh.OrderDate <= to);

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