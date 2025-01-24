using System.ComponentModel; 
using MultiAgentWebAPI.Entities;
using Microsoft.SemanticKernel;
using MultiAgentWebAPI.Utilities;

namespace MultiAgentWebAPI.Plugins
{
    public class CustomerPlugin
    {
        [KernelFunction("get_customer_details")]
        [Description("Gets Customer Details for the given CustomerID.")]
        public IEnumerable<Customer> GetCustomerDetails(string CustomerID)
        {
            try
            {
                Console.WriteLine($"Get customer details from CSV for: {CustomerID}.");

                var csvFilePath = "data/Customers.csv";

                var customers = CSVHelper.ReadRecords<Customer>(csvFilePath, c => c.CustomerID.ToString().Equals(CustomerID, StringComparison.OrdinalIgnoreCase));

                if (customers != null)
                {
                    return customers;
                }
                else
                {
                    Console.WriteLine("No customer data found in CSV.");
                    return Enumerable.Empty<Customer>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An exception occurred while getting customer details: {ex.Message}");
            }
        }
    }
}