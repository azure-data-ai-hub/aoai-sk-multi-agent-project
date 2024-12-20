using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using MultiAgentWebAPI.Plugins;

namespace MultiAgentWebAPI.Agents
{
    public class ProjectTasksAgent
    {
        private const string ProjectTasksAgentName = "ProjectDailyTasksAgent";
        private const string ProjectTasksAgentInstructions =
            """
            You are a Project Tasks Assistant dedicated to providing detailed information on daily project tasks.

            Your responsibilities include:

            - Retrieving project task details using the tools associated with you.

            - Based on the user's query, provide updates on daily tasks, their status, and any issues encountered.

            Ensure accuracy in all task reports and ask for additional information if necessary to deliver detailed and actionable task updates.
            """;

        public ChatCompletionAgent Initialize(string endPoint, string deploymentName, string apiKey)
        {
            IKernelBuilder builder = Kernel.CreateBuilder();
            builder.AddAzureOpenAIChatCompletion(
                deploymentName: deploymentName,
                endpoint: endPoint,
                apiKey: apiKey
            );

            builder.Plugins.AddFromType<ProjectTaskPlugin>();

            builder.Services.AddLogging(config => { config.AddConsole(); config.SetMinimumLevel(LogLevel.Trace); });

            Kernel kernel = builder.Build();

            ChatCompletionAgent projectTasksAgent = new()
            {
                Name = ProjectTasksAgentName,
                Instructions = ProjectTasksAgentInstructions,
                Kernel = kernel,
                Arguments = new KernelArguments(new AzureOpenAIPromptExecutionSettings()
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
                })
                {
                    { "repository", "microsoft/semantic-kernel" }
                }
            };

            return projectTasksAgent;
        }
    }
}