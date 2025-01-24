using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using MultiAgentWebAPI.Plugins;
using Microsoft.Extensions.Logging;

namespace MultiAgentWebAPI.Agents
{
    public class DataAnalysisAgent: IAgent
    {
        private const string DataAnalysisAgentName = "DataAnalysisAgent";
        private const string DataAnalysisAgentInstructions =
            """
            You are a Data Analysis Agent specializing in analyzing personnel and sales territory data. Your task is to provide detailed insights on employee performance, sales distribution, and regional trends. Use the following functions to answer the queries:
            - get_person_details
            - get_sales_territory

            Example Queries:
            - What is the distribution of sales across different territories?
            - Analyze the performance of employees over the last year.
            - Identify trends in sales within the European region.
            """;

        public ChatCompletionAgent Initialize(string endPoint, string deploymentName, string apiKey)
        {
            IKernelBuilder builder = Kernel.CreateBuilder();
            builder.AddAzureOpenAIChatCompletion(
                deploymentName: deploymentName,
                endpoint: endPoint,
                apiKey: apiKey
            );

            builder.Plugins.AddFromType<PersonPlugin>();
            builder.Plugins.AddFromType<SalesTerritoryPlugin>();

            builder.Services.AddLogging(config =>
            {
                config.AddConsole();
                config.SetMinimumLevel(LogLevel.Trace);
            });

            Kernel kernel = builder.Build();

            ChatCompletionAgent dataAnalysisAgent = new()
            {
                Name = DataAnalysisAgentName,
                Instructions = DataAnalysisAgentInstructions,
                Kernel = kernel,
                Arguments = new KernelArguments(new AzureOpenAIPromptExecutionSettings()
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                })
            };

            return dataAnalysisAgent;
        }
    }
}