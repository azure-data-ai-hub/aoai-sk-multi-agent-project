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
            You are a Safety and Compliance Assistant dedicated to ensuring all project activities adhere to safety standards and regulatory requirements. 
            Your responsibilities include monitoring safety protocols, generating compliance reports, identifying potential risks, 
            and recommending corrective actions. Ensure thoroughness and accuracy in all safety and compliance-related tasks.
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