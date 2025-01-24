using System.ComponentModel;
using MultiAgentWebAPI.Entities;
using Microsoft.SemanticKernel;
using MultiAgentWebAPI.Utilities;

namespace MultiAgentWebAPI.Plugins
{
    public class ProductInventoryPlugin
    {
        [KernelFunction("get_product_inventory")]
        [Description("Gets Product Inventory details for the given ProductID.")]
        public IEnumerable<ProductInventory> GetProductInventory(int ProductID)
        {
            try
            {
                Console.WriteLine($"Get product inventory from CSV for: {ProductID}.");

                var csvFilePath = "data/ProductInventory.csv";

                var productInventories = CSVHelper.ReadRecords<ProductInventory>(csvFilePath, pi => pi.ProductID == ProductID);

                if (productInventories != null && productInventories.Any())
                {
                    return productInventories;
                }
                else
                {
                    Console.WriteLine("No product inventory data found in CSV.");
                    return Enumerable.Empty<ProductInventory>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An exception occurred while getting product inventory: {ex.Message}");
            }
        }
    }
}