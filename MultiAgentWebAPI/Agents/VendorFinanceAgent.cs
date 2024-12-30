using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using MultiAgentWebAPI.Plugins;

public class VendorFinanceAgent
{
    private const string VendorFinanceAgentName = "VendorFinanceAgent";
    private const string VendorFinanceAgentInstructions =
            """
            You are a Vendor Financials Assistant dedicated to providing information on vendor finanace data for projects.

            Your responsibilities include:

            - Retrieving vednor financial details using the tools assigned to you.

            Ensure accuracy in all vendor finanacial reports.
            """;

    public ChatCompletionAgent Initialize(string endPoint, string deploymentName, string apiKey)
    {
        IKernelBuilder builder = Kernel.CreateBuilder();
        builder.AddAzureOpenAIChatCompletion(
            deploymentName: deploymentName,
            endpoint: endPoint,
            apiKey: apiKey
        );

        builder.Plugins.AddFromType<VendorFinancePlugin>();

        builder.Services.AddLogging(config => { config.AddConsole(); config.SetMinimumLevel(LogLevel.Trace); });

        Kernel kernel = builder.Build();

        ChatCompletionAgent vendorFinanceAgent = new()
        {
            Name = VendorFinanceAgentName,
            Instructions = VendorFinanceAgentInstructions,
            Kernel = kernel,
            Arguments = new KernelArguments(new AzureOpenAIPromptExecutionSettings()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                Temperature = 0.1
            })
                {
                    { "repository", "microsoft/semantic-kernel" }
                }
        };

        return vendorFinanceAgent;
    }
}

