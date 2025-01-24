using System.ComponentModel;
using MultiAgentWebAPI.Entities;
using Microsoft.SemanticKernel;
using MultiAgentWebAPI.Utilities;

namespace MultiAgentWebAPI.Plugins
{
    public class OrderDetailPlugin
    {
        [KernelFunction("get_order_details")]
        [Description("Gets Order Details for the given SalesOrderID.")]
        public IEnumerable<OrderDetail> GetOrderDetails(int SalesOrderID)
        {
            try
            {
                Console.WriteLine($"Get order details from CSV for: {SalesOrderID}.");

                var csvFilePath = "data/OrderDetail.csv";

                var orderDetails = CSVHelper.ReadRecords<OrderDetail>(csvFilePath, od => od.SalesOrderID == SalesOrderID);

                if (orderDetails != null)
                {
                    return orderDetails;
                }
                else
                {
                    Console.WriteLine("No order detail data found in CSV.");
                    return Enumerable.Empty<OrderDetail>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An exception occurred while getting order details: {ex.Message}");
            }
        }
    }
}