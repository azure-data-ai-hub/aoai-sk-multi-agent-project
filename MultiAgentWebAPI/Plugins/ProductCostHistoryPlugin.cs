using System.ComponentModel;
using MultiAgentWebAPI.Entities;
using Microsoft.SemanticKernel;
using MultiAgentWebAPI.Utilities;

namespace MultiAgentWebAPI.Plugins
{
    public class ProductCostHistoryPlugin
    {
        [KernelFunction("get_product_cost_history")]
        [Description("Gets Product Cost History for the given ProductID.")]
        public IEnumerable<ProductCostHistory> GetProductCostHistory(int ProductID)
        {
            try
            {
                Console.WriteLine($"Get product cost history from CSV for: {ProductID}.");

                var csvFilePath = "data/ProductCostHistory.csv";

                var productCostHistories = CSVHelper.ReadRecords<ProductCostHistory>(csvFilePath, pch => pch.ProductID == ProductID);

                if (productCostHistories != null && productCostHistories.Any())
                {
                    return productCostHistories;
                }
                else
                {
                    Console.WriteLine("No product cost history data found in CSV.");
                    return Enumerable.Empty<ProductCostHistory>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An exception occurred while getting product cost history: {ex.Message}");
            }
        }
    }
}