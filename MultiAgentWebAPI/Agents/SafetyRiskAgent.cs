using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using MultiAgentWebAPI.Plugins;

namespace MultiAgentWebAPI.Agents
{
    public class SafetyRiskAgent
    {
        private const string SafetyRiskAgentName = "SafetyRiskAgent";
        private const string SafetyRiskAgentInstructions =
             """
            You are a Safety and Risk Assistant dedicated to providing information on safety, risks, and compliance for projects.

            Your responsibilities include:

            - Retrieving safety and risk details using the tools associated wtih you.

            - Based on the user's query, provide updates on safety measures, risk assessments, and compliance status.

            Ensure accuracy in all safety and risk reports and ask for additional information if necessary to deliver detailed and actionable updates.
            """;

        public ChatCompletionAgent Initialize(string endPoint, string deploymentName, string apiKey)
        {
            IKernelBuilder builder = Kernel.CreateBuilder();
            builder.AddAzureOpenAIChatCompletion(
                deploymentName: deploymentName,
                endpoint: endPoint,
                apiKey: apiKey
            );

            builder.Plugins.AddFromType<ProjectSafetyRiskPlugin>();

            builder.Services.AddLogging(config => { config.AddConsole(); config.SetMinimumLevel(LogLevel.Trace); });

            Kernel kernel = builder.Build();

            ChatCompletionAgent safetyRiskAgent = new()
            {
                Name = SafetyRiskAgentName,
                Instructions = SafetyRiskAgentInstructions,
                Kernel = kernel,
                Arguments = new KernelArguments(new AzureOpenAIPromptExecutionSettings()
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
                })
                {
                    { "repository", "microsoft/semantic-kernel" }
                }
            };

            return safetyRiskAgent;
        }
    }
}