using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using MultiAgentWebAPI.Plugins;

namespace MultiAgentWebAPI.Agents
{
    public class ProjectManagerAgent()
    {
        private const string ProectManagerAgentName = "ProjectManagerAgent";
        private const string ProectManagerAgentInstructions =
            """
            You are a Project Manager agent dedicated to providing comprehensive project information. 
            Your responsibilities include retrieving project details given Project Name.  Other agents
            might call you to get the ProjectID.  If you receive the project name like 'West Branch Bypass Tunnel Construction'
            look up the name uing get_project_details and return the project details including ProjectID to the caller.
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
