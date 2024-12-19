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
            You are a Finance Assistant dedicated to managing and reporting financial data related to projects.
            Your duties include retrieving financial details, updating budget forecasts, monitoring expenditures, 
            and generating financial reports. Ensure precision in all financial operations and seek additional information 
            when necessary to maintain accurate financial oversight.
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