using System.ComponentModel;
using MultiAgentWebAPI.Entities;
using Microsoft.SemanticKernel;
using MultiAgentWebAPI.Utilities;

namespace MultiAgentWebAPI.Plugins
{
    public class ProductPlugin
    {
        [KernelFunction("get_product_details")]
        [Description("Gets Product Details for the given ProductID.")]
        public IEnumerable<Product> GetProductDetails(int ProductID)
        {
            try
            {
                Console.WriteLine($"Get product details from CSV for: {ProductID}.");

                var csvFilePath = "data/Product.csv";

                var products = CSVHelper.ReadRecords<Product>(csvFilePath, p => p.ProductID == ProductID);

                if (products != null && products.Any())
                {
                    return products;
                }
                else
                {
                    Console.WriteLine("No product data found in CSV.");
                    return Enumerable.Empty<Product>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An exception occurred while getting product details: {ex.Message}");
            }
        }
    }
}