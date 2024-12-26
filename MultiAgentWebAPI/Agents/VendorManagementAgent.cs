using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using MultiAgentWebAPI.Plugins;

public class VendorManagementAgent
{
    private const string VendorManagementAgentName = "VendorFinanceAgent";
    private const string VendorManagementAgentInstructions =
            """
            You are a Vendor Management Assistant dedicated to providing information on vendor data for projects.

            Your responsibilities include:

            - Retrieving vendor details using the `GetVendorDetails` Kernel Function under `VendorManagementPlugin`.

            - Based on the user's query, provide updates on vendor performance, contracts, and any issues encountered.

            Ensure accuracy in all vendor reports.
            """;

    public ChatCompletionAgent Initialize(string endPoint, string deploymentName, string apiKey)
    {
        IKernelBuilder builder = Kernel.CreateBuilder();
        builder.AddAzureOpenAIChatCompletion(
            deploymentName: deploymentName,
            endpoint: endPoint,
            apiKey: apiKey
        );

        builder.Plugins.AddFromType<VendorManagementPlugin>();

        builder.Services.AddLogging(config => { config.AddConsole(); config.SetMinimumLevel(LogLevel.Trace); });

        Kernel kernel = builder.Build();

        ChatCompletionAgent vendorManagementAgent = new()
        {
            Name = VendorManagementAgentName,
            Instructions = VendorManagementAgentInstructions,
            Kernel = kernel,
            Arguments = new KernelArguments(new AzureOpenAIPromptExecutionSettings()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            })
                {
                    { "repository", "microsoft/semantic-kernel" }
                }
        };

        return vendorManagementAgent;
    }
}

