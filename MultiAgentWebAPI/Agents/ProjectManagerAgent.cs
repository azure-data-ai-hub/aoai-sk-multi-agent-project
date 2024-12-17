using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using MultiAgentWebAPI.Plugins;
using Microsoft.Azure.Cosmos;


namespace MultiAgentWebAPI.Agents
{
    public class ProjectManagerAgent()
    {
        private const string ProectManagerAgentName = "ProjectManagerAgent";
        private const string ProectManagerAgentInstructions =
            """
        You are a project manager which will get the details of the project and further call schdule and finanace agents, 
        to get all the project information like schdule, finanace and agregate the results and respond back to the user with 
        conslidated project report. You are the guardian of quality, ensuring the final response meets all the details requested 
        by the user. Once all details like schdule, finanaces are available you can approve the task by just responding "approve"
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

            builder.Plugins.AddFromType<ProjectPlugin>();

            Kernel kernel = builder.Build();

            ChatCompletionAgent projectMangaerAgent =
            new()
            {
                Name = ProectManagerAgentName,
                Instructions = ProectManagerAgentInstructions,
                Kernel = kernel,
                Arguments =
                    new KernelArguments(new AzureOpenAIPromptExecutionSettings() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() })
                    {
                        { "repository", "microsoft/semantic-kernel" }
                    }
            };

            return projectMangaerAgent;
        }
    }
}
