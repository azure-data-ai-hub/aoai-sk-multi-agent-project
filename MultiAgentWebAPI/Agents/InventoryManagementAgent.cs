using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using MultiAgentWebAPI.Plugins;
using Microsoft.Extensions.Logging;

namespace MultiAgentWebAPI.Agents
{
    public class InventoryManagementAgent: IAgent
    {
        private const string InventoryManagementAgentName = "InventoryManagementAgent";
        private const string InventoryManagementAgentInstructions =
            """
            You are an Inventory Management Agent specializing in handling product inventory and product details, create valid SQL queries from the user's prompt and intent and execute the query using the tool:
            - execute_sql_query
            
            SQL Table Schema Overview:
            - ProductInventory -> (ProductID, LocationID, Shelf, Bin, Quantity, rowguid, ModifiedDate)
            - Product -> (ProductID, Name, ProductNumber, MakeFlag, FinishedGoodsFlag, Color, SafetyStockLevel, ReorderPoint, StandardCost, ListPrice, Size, SizeUnitMeasureCode, WeightUnitMeasureCode, Weight, DaysToManufacture, ProductLine, Class, Style, ProductSubcategoryID, ProductModelID, SellStartDate, SellEndDate, DiscontinuedDate, rowguid, ModifiedDate)
            - Production Location -> (LocationID, Name, CostRate, Availability, ModifiedDate)

            Example Queries:
            - What is the current inventory level for ProductID 870?
            - List all products that are low in stock.
            - Provide an overview of product availability across all locations.
            """;

        public ChatCompletionAgent Initialize(string endPoint, string deploymentName, string apiKey, string sqlConnectionString)
        {
            IKernelBuilder builder = Kernel.CreateBuilder();
            builder.AddAzureOpenAIChatCompletion(
                deploymentName: deploymentName,
                endpoint: endPoint,
                apiKey: apiKey
            );

            builder.Plugins.AddFromType<ExecuteSQLQueryPlugin>();

            builder.Services.AddLogging(config =>
            {
                config.AddConsole();
                config.SetMinimumLevel(LogLevel.Trace);
            });

            Kernel kernel = builder.Build();
            kernel.Data["sqlConnectionString"] = sqlConnectionString;

            ChatCompletionAgent inventoryManagementAgent = new()
            {
                Name = InventoryManagementAgentName,
                Instructions = InventoryManagementAgentInstructions,
                Kernel = kernel,
                Arguments = new KernelArguments(new AzureOpenAIPromptExecutionSettings()
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                })
            };

            return inventoryManagementAgent;
        }
    }
}