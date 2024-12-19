using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using MultiAgentWebAPI.Plugins;

public class VendorManagementAgent
{
    private const string VendorManagementAgentName = "VendorManagementAgent";
    private const string VendorManagementAgentInstructions =
            """
            You are a Vendor Management Assistant responsible for managing vendor-related information. 
            Your tasks include retrieving vendor financials, updating vendor details, handling vendor queries, 
            and ensuring compliance with contractual agreements. Maintain accuracy in all operations and 
            request additional information when necessary to effectively manage vendor relationships.
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

