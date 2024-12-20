using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using MultiAgentWebAPI.Plugins;

namespace MultiAgentWebAPI.Agents
{
    public class FinanceAgent
    {
        private const string FinanceAgentName = "FinanceAgent";
        private const string FinanceAgentInstructions =
            """
            You are a Finance Assistant dedicated to providing comprehensive financial details for projects.

            Your responsibilities include:

            - Retrieving financial details using the tools assigned to you.

            Ensure accuracy in all financial reports and ask for additional information if necessary to deliver detailed and actionable financial updates.
            """;

        public ChatCompletionAgent Initialize(string endPoint, string deploymentName, string apiKey)
        {
            IKernelBuilder builder = Kernel.CreateBuilder();
            builder.AddAzureOpenAIChatCompletion(
                deploymentName: deploymentName,
                endpoint: endPoint,
                apiKey: apiKey
            );

            builder.Plugins.AddFromType<FinancePlugin>();

            builder.Services.AddLogging(config => { config.AddConsole(); config.SetMinimumLevel(LogLevel.Trace);});

            Kernel kernel = builder.Build();

            ChatCompletionAgent financeAgent = new()
            {
                Name = FinanceAgentName,
                Instructions = FinanceAgentInstructions,
                Kernel = kernel,
                Arguments = new KernelArguments(new AzureOpenAIPromptExecutionSettings()
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
                })
                {
                    { "repository", "microsoft/semantic-kernel" }
                }
            };

            return financeAgent;
        }
    }
}