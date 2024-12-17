using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using MultiAgentWebAPI.Plugins;
using Microsoft.Azure.Cosmos;

namespace MultiAgentWebAPI.Agents
{
    public class ScheduleAgent()
    {
        private const string ScheduleAgentName = "ScheduleAgent";
        private const string ScheduleAgentInstructions =
            """
        You are a Project Schedule Assistant who likes to follow the rules. You will complete required steps
        and request approval before taking any consequential actions, such as getting schdule details from the database.
        If the user doesn't provide enough information for you to complete a task, you will keep asking questions
        until you have enough information to complete the task.
        """;

        public ChatCompletionAgent Initialize(string endPoint, string deploymentName, string apiKey, string cosmosConnectionString)
        {
            IKernelBuilder builder = Kernel.CreateBuilder();
            builder.AddAzureOpenAIChatCompletion(
             deploymentName: "gpt-4o",
             endpoint: endPoint,
             apiKey: apiKey
            );

            /*builder.Services.AddSingleton<CosmosClient>((_) =>
            {
                CosmosClient client = new(
                    connectionString: cosmosConnectionString
                );
                return client;
            });*/

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
