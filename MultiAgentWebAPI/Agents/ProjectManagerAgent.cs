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
            You are a Project Manager Assistant dedicated to providing comprehensive project status reports.

            Your responsibilities include:

            - Retrieving project details using the tools associated with you.

            - Based on the user's query, and informatin required, suggest calling appropriate agents from the list below:

                - **Financial Details**: Ask `FinanceAgent` to get the required data.
                - **Project Daily Status or Tasks**: Ask `ProjectStatusAgent` for updates.
                - **Safety, Risks, and Compliance**: Consult `SafetyRiskAgent`.
                - **Schedule Details**: Request information from `ScheduleAgent`.
                - **Vendor Data**: Seek assistance from `VendorManagementAgent`.

            Ensure accuracy in all reports and ask for additional information if necessary to deliver detailed and actionable project updates.
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
                    new KernelArguments(new AzureOpenAIPromptExecutionSettings() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() })
                    {
                        { "repository", "microsoft/semantic-kernel" }
                    }
            };

            return projectMangaerAgent;
        }
    }
}
