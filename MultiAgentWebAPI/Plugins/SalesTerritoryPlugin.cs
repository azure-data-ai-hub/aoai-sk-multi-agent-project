using System.ComponentModel;
using MultiAgentWebAPI.Entities;
using Microsoft.SemanticKernel;
using MultiAgentWebAPI.Utilities;

namespace MultiAgentWebAPI.Plugins
{
    public class SalesTerritoryPlugin
    {
        [KernelFunction("get_sales_territory")]
        [Description("Gets Sales Territory details for the given TerritoryID.")]
        public IEnumerable<SalesTerritoryEntity> GetSalesTerritories(int TerritoryID)
        {
            try
            {
                Console.WriteLine($"Get sales territory from CSV for: {TerritoryID}.");

                var csvFilePath = "data/SalesTerritory.csv";

                var salesTerritories = CSVHelper.ReadRecords<SalesTerritoryEntity>(csvFilePath, st => st.TerritoryID == TerritoryID);

                if (salesTerritories != null && salesTerritories.Any())
                {
                    return salesTerritories;
                }
                else
                {
                    Console.WriteLine("No sales territory data found in CSV.");
                    return Enumerable.Empty<SalesTerritoryEntity>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An exception occurred while getting sales territory details: {ex.Message}");
            }
        }
    }
}