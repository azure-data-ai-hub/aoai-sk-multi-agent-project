using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using MultiAgentWebAPI.Plugins;
using Microsoft.Extensions.Logging;

namespace MultiAgentWebAPI.Agents
{
    public class FinanceAgent: IAgent
    {
        private const string FinanceAgentName = "FinanceAgent";
        private const string FinanceAgentInstructions =
            """
            You are a Finance Agent specializing in analyzing purchase orders and product cost histories. Your task is to provide detailed financial insights, cost analyses, and purchase order evaluations. Use the following functions to answer the queries:
            - get_purchase_order_header
            - get_purchase_order_detail
            - get_product_cost_history

            Example Queries:
            - What are the total expenditures on purchases for the current fiscal year?
            - Analyze the cost trends of ProductID 870 over the past six months.
            - Provide a detailed report on purchase order histories for VendorID 12345.
            """;

        public ChatCompletionAgent Initialize(string endPoint, string deploymentName, string apiKey)
        {
            IKernelBuilder builder = Kernel.CreateBuilder();
            builder.AddAzureOpenAIChatCompletion(
                deploymentName: deploymentName,
                endpoint: endPoint,
                apiKey: apiKey
            );

            builder.Plugins.AddFromType<PurchaseOrderHeaderPlugin>();
            builder.Plugins.AddFromType<PurchaseOrderDetailPlugin>();
            builder.Plugins.AddFromType<ProductCostHistoryPlugin>();

            builder.Services.AddLogging(config =>
            {
                config.AddConsole();
                config.SetMinimumLevel(LogLevel.Trace);
            });

            Kernel kernel = builder.Build();

            ChatCompletionAgent financeAgent = new()
            {
                Name = FinanceAgentName,
                Instructions = FinanceAgentInstructions,
                Kernel = kernel,
                Arguments = new KernelArguments(new AzureOpenAIPromptExecutionSettings()
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                })
            };

            return financeAgent;
        }
    }
}