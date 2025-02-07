using System.ComponentModel;
using MultiAgentWebAPI.Entities;
using Microsoft.SemanticKernel;
using MultiAgentWebAPI.Utilities;

namespace MultiAgentWebAPI.Plugins
{
    public class ProductInventoryPlugin
    {
        [KernelFunction("get_product_inventory")]
        [Description("Gets Product Inventory details")]
        public IEnumerable<ProductInventory> GetProductInventory()
        {
            try
            {
                Console.WriteLine($"Get product inventory from CSV");

                var csvFilePath = "data/ProductInventory.csv";

                var productInventories = CSVHelper.ReadRecords<ProductInventory>(csvFilePath, pi => pi.ProductID > 0);

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

        [KernelFunction("get_specific_product_inventory")]
        [Description("Gets Specific Product Inventory details given ProductID")]
        public IEnumerable<ProductInventory> GetSpecificProductInventory(int ProductID)
        {
            try
            {
                Console.WriteLine($"Get product inventory from CSV ");

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