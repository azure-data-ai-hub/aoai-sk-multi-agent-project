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
            You are a Project Schedule Assistant, your responsibilities include retrieving schedule details.
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

            builder.Services.AddLogging(config => { config.AddConsole(); config.SetMinimumLevel(LogLevel.Trace); });

            Kernel kernel = builder.Build();

            ChatCompletionAgent scheduleAgent =
            new()
            {
                Name = ScheduleAgentName,
                Instructions = ScheduleAgentInstructions,
                Kernel = kernel,
                Arguments =
                    new KernelArguments(new AzureOpenAIPromptExecutionSettings() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(), Temperature = 0.1 })
                    {
                        { "repository", "microsoft/semantic-kernel" }
                    }
            };

            return scheduleAgent;
        }
    }
}
