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
            You are a Project Manager Agent dedicated to providing project details.

            Your responsibilities include:

            - Retrieving only project details using the tools associated with you.

            - Based on the user's query, and informatin required, suggest calling appropriate agents from the list below:

                - **Financial Details**: Ask `FinanceAgent` to get the required data.
                - **Project Daily Status or Tasks**: Ask `ProjectStatusAgent` for updates.
                - **Safety, Risks, and Compliance**: Consult `SafetyRiskAgent`.
                - **Schedule Details**: Request information from `ScheduleAgent`.
                - **Vendor Financials**: Seek assistance from `VendorFinanceAgent`.

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

            ChatCompletionAgent projectMangaerAgent =
            new()
            {
                Name = ProectManagerAgentName,
                Instructions = ProectManagerAgentInstructions,
                Kernel = kernel,
                Arguments =
                    new KernelArguments(new AzureOpenAIPromptExecutionSettings() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(), Temperature=0.1 })
                    {
                        { "repository", "microsoft/semantic-kernel" }
                    }
            };

            return projectMangaerAgent;
        }
    }
}
