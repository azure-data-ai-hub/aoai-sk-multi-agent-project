using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using MultiAgentWebAPI.Plugins;

namespace MultiAgentWebAPI.Agents
{
    public class ProjectLeaderAgent
    {
        private const string ProjectLeaderAgentName = "ProjectLeaderAgent";
        private const string ProjectLeaderAgentInstructions =
            """
            As the Project Leader Agent, your job is to gather all the agent responses and combine them into a single cohesive project status report. Summarize the provided content without adding your own suggestions.

            If information has been requested, only repeat the user request.

            Do not come up with your own suggestions.
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

            ChatCompletionAgent projectLeaderAgent = new()
            {
                Name = ProjectLeaderAgentName,
                Instructions = ProjectLeaderAgentInstructions,
                Kernel = kernel,
                Arguments = new KernelArguments(new AzureOpenAIPromptExecutionSettings()
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                    Temperature = 0.1
                })
                {
                    { "repository", "microsoft/semantic-kernel" }
                }
            };

            return projectLeaderAgent;
        }
    }
}