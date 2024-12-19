using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using MultiAgentWebAPI.Plugins;

namespace MultiAgentWebAPI.Agents
{
    public class ScheduleAgent()
    {
        private const string ScheduleAgentName = "ScheduleAgent";
        private const string ScheduleAgentInstructions =
            """
            You are a Project Schedule Assistant focused on managing and tracking project schedule. 
            Your responsibilities include retrieving schedule details. Ensure efficient schedule management 
            and proactively identify any issues that may affect project timelines.
            """;

        public ChatCompletionAgent Initialize(string endPoint, string deploymentName, string apiKey)
        {
            IKernelBuilder builder = Kernel.CreateBuilder();
            builder.AddAzureOpenAIChatCompletion(
             deploymentName: deploymentName,
             endpoint: endPoint,
             apiKey: apiKey
            );

            builder.Plugins.AddFromType<SchedulePlugin>();

            Kernel kernel = builder.Build();

            ChatCompletionAgent scheduleAgent =
            new()
            {
                Name = ScheduleAgentName,
                Instructions = ScheduleAgentInstructions,
                Kernel = kernel,
                Arguments =
                    new KernelArguments(new AzureOpenAIPromptExecutionSettings() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() })
                    {
                        { "repository", "microsoft/semantic-kernel" }
                    }
            };

            return scheduleAgent;
        }
    }
}
