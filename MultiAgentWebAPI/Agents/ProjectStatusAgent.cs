using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using MultiAgentWebAPI.Plugins;

namespace MultiAgentWebAPI.Agents
{
    public class ProjectStatusAgent
    {
        private const string ProjectStatusAgentName = "ProjectStatusAgent";
        private const string ProjectStatusAgentInstructions =
            """
            You are a Project Status Assistant dedicated to providing comprehensive project status reports. 
            Your responsibilities include retrieving project details, monitoring progress, identifying potential delays, 
            and offering insights to stakeholders. Ensure accuracy in all reports and ask for additional information 
            if necessary to deliver detailed and actionable project updates.
            """;

        public ChatCompletionAgent Initialize(string endPoint, string deploymentName, string apiKey)
        {
            IKernelBuilder builder = Kernel.CreateBuilder();
            builder.AddAzureOpenAIChatCompletion(
                deploymentName: deploymentName,
                endpoint: endPoint,
                apiKey: apiKey
            );

            builder.Plugins.AddFromType<ProjectPlugin>();

            builder.Services.AddLogging(config => { config.AddConsole(); config.SetMinimumLevel(LogLevel.Trace); });

            Kernel kernel = builder.Build();

            ChatCompletionAgent projectStatusAgent = new()
            {
                Name = ProjectStatusAgentName,
                Instructions = ProjectStatusAgentInstructions,
                Kernel = kernel,
                Arguments = new KernelArguments(new AzureOpenAIPromptExecutionSettings()
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
                })
                {
                    { "repository", "microsoft/semantic-kernel" }
                }
            };

            return projectStatusAgent;
        }
    }
}