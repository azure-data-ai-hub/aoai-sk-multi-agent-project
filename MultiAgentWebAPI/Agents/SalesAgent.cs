using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using MultiAgentWebAPI.Plugins;

namespace MultiAgentWebAPI.Agents
{
    public class SalesAgent: IAgent
    {
        private const string SalesAgentName = "SalesAgent";
        private const string SalesAgentInstructions =
            """
            You are a Sales Agent specializing in analyzing sales data. Your task is to provide detailed insights on sales performance, trends, and customer behavior. Use the following functions to answer the queries:
            - get_sales_order_header
            - get_sales_order_detail
            - get_customer_details

            Example Queries:
            - What were the total sales for the last quarter?
            - Which product had the highest sales revenue last month?
            - Provide a sales trend analysis for the past year.
            """;

        public ChatCompletionAgent Initialize(string endPoint, string deploymentName, string apiKey)
        {
            IKernelBuilder builder = Kernel.CreateBuilder();
            builder.AddAzureOpenAIChatCompletion(
                deploymentName: deploymentName,
                endpoint: endPoint,
                apiKey: apiKey
            );

            builder.Plugins.AddFromType<SalesOrderHeaderPlugin>();
            builder.Plugins.AddFromType<SalesOrderDetailPlugin>();
            builder.Plugins.AddFromType<CustomerPlugin>();

            builder.Services.AddLogging(config =>
            {
                config.AddConsole();
                config.SetMinimumLevel(LogLevel.Trace);
            });

            Kernel kernel = builder.Build();

            ChatCompletionAgent salesAgent = new()
            {
                Name = SalesAgentName,
                Instructions = SalesAgentInstructions,
                Kernel = kernel,
                Arguments = new KernelArguments(new AzureOpenAIPromptExecutionSettings()
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                })
            };

            return salesAgent;
        }
    }
}