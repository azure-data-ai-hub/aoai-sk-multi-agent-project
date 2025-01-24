using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using MultiAgentWebAPI.Plugins;

namespace MultiAgentWebAPI.Agents
{
    public class CustomerServiceAgent: IAgent
    {
        private const string CustomerServiceAgentName = "CustomerServiceAgent";
        private const string CustomerServiceAgentInstructions =
            """
            You are a Customer Service Agent specializing in managing customer information and personal details. Your task is to provide comprehensive insights on customer profiles, demographics, and behaviors. Use the following functions to answer the queries:
            - get_customer_details
            - get_person_details

            Example Queries:
            - Retrieve detailed information for CustomerID 12345.
            - Analyze the demographics of our top 100 customers.
            - Provide a report on customer behavior trends over the past year.
            """;

        public ChatCompletionAgent Initialize(string endPoint, string deploymentName, string apiKey)
        {
            IKernelBuilder builder = Kernel.CreateBuilder();
            builder.AddAzureOpenAIChatCompletion(
                deploymentName: deploymentName,
                endpoint: endPoint,
                apiKey: apiKey
            );

            builder.Plugins.AddFromType<CustomerPlugin>();
            builder.Plugins.AddFromType<PersonPlugin>();

            builder.Services.AddLogging(config =>
            {
                config.AddConsole();
                config.SetMinimumLevel(LogLevel.Trace);
            });

            Kernel kernel = builder.Build();

            ChatCompletionAgent customerServiceAgent = new()
            {
                Name = CustomerServiceAgentName,
                Instructions = CustomerServiceAgentInstructions,
                Kernel = kernel,
                Arguments = new KernelArguments(new AzureOpenAIPromptExecutionSettings()
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                })
            };

            return customerServiceAgent;
        }
    }
}