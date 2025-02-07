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
            You are a Customer Service Agent specializing in providing customer informatin and personal details. For user customer related queries, create valid SQL queries from the user's prompt and intent and execute the query using the tool:
            - execute_sql_query

            SQL Table Schema Overview:
            - SalesLT.Customer -> (CustomerID, NameStyle, Title, FirstName, MiddleName, LastName, Suffix, CompanyName, SalesPerson, EmailAddress, Phone, rowguid, ModifiedDate)
            - Peron -> (BusinessEntityID, PersonType, NameStyle, Title, FirstName, MiddleName, LastName, Suffix, EmailPromotion, AdditionalContactInfo, Demographics, rowguid, ModifiedDate)

            Example Queries:
            - Retrieve detailed information for CustomerID 12345.
            - Analyze the demographics of our top 100 customers.
            - Provide a report on customer behavior trends over the past year.
            """;

        public ChatCompletionAgent Initialize(string endPoint, string deploymentName, string apiKey, string sqlConnectionString)
        {
            IKernelBuilder builder = Kernel.CreateBuilder();
            builder.AddAzureOpenAIChatCompletion(
                deploymentName: deploymentName,
                endpoint: endPoint,
                apiKey: apiKey
            );

            builder.Plugins.AddFromType<ExecuteSQLQueryPlugin>();

            builder.Services.AddLogging(config =>
            {
                config.AddConsole();
                config.SetMinimumLevel(LogLevel.Trace);
            });

            Kernel kernel = builder.Build();
            kernel.Data["sqlConnectionString"] = sqlConnectionString;

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