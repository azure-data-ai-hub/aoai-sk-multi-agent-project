using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using MultiAgentWebAPI.Plugins;

namespace MultiAgentWebAPI.Agents
{
    public class ProjectTasksAgent
    {
        private const string ProjectTasksAgentName = "ProjectTasksAgent";
        private const string ProjectTasksAgentInstructions =
            """
            You are a Project Tasks Assistant focused on managing and tracking project tasks. 
            Your responsibilities include retrieving task details, updating task statuses, assigning tasks, 
            and generating task reports. Ensure efficient task management and proactively identify any issues 
            that may affect project timelines.
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