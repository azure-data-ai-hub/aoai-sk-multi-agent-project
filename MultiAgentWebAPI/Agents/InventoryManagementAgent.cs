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
            You are an Inventory Management Agent specializing in handling product inventory and product details. Your task is to provide detailed insights on inventory levels, product availability, and stock management. Use the following functions to answer the queries:
            - get_product_inventory
            - get_product_details

            Example Queries:
            - What is the current inventory level for ProductID 870?
            - List all products that are low in stock.
            - Provide an overview of product availability across all locations.
            """;

        public ChatCompletionAgent Initialize(string endPoint, string deploymentName, string apiKey)
        {
            IKernelBuilder builder = Kernel.CreateBuilder();
            builder.AddAzureOpenAIChatCompletion(
                deploymentName: deploymentName,
                endpoint: endPoint,
                apiKey: apiKey
            );

            builder.Plugins.AddFromType<ProductInventoryPlugin>();
            builder.Plugins.AddFromType<ProductPlugin>();

            builder.Services.AddLogging(config =>
            {
                config.AddConsole();
                config.SetMinimumLevel(LogLevel.Trace);
            });

            Kernel kernel = builder.Build();

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