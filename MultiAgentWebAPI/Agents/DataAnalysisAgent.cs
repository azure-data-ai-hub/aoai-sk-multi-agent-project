using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using MultiAgentWebAPI.Plugins;
using Microsoft.Extensions.Logging;

namespace MultiAgentWebAPI.Agents
{
    public class DataAnalysisAgent: IAgent
    {
        private const string DataAnalysisAgentName = "DataAnalysisAgent";
        private const string DataAnalysisAgentInstructions =
            """
            You are a Data Analysis Agent specializing in analyzing personnel and sales territory data. For data analysis related queries, create valid SQL queries from the user's prompt and intent and execute the query using the tool:
            - execute_sql_query

            SQL Table Schema Overview:
            - SalesLT.Customer -> (CustomerID, NameStyle, Title, FirstName, MiddleName, LastName, Suffix, CompanyName, SalesPerson, EmailAddress, Phone, rowguid, ModifiedDate)
            - SalesTerritory -> (TerritoryID, Name, CountryRegionCode, Group, SalesYTD, SalesLastYear, CostYTD, CostLastYear, rowguid, ModifiedDate)

            Example Queries:
            - What is the distribution of sales across different territories?
            - Analyze the performance of employees over the last year.
            - Identify trends in sales within the European region.
            
            """;

        public ChatCompletionAgent Initialize(string endPoint, string deploymentName, string apiKey, string sqlConnectionString)
        {
            IKernelBuilder builder = Kernel.CreateBuilder();
            builder.AddAzureOpenAIChatCompletion(
                deploymentName: deploymentName,
                endpoint: endPoint,
                apiKey: apiKey
            );

            builder.Plugins.AddFromType<PersonPlugin>();
            builder.Plugins.AddFromType<SalesTerritoryPlugin>();

            builder.Services.AddLogging(config =>
            {
                config.AddConsole();
                config.SetMinimumLevel(LogLevel.Trace);
            });

            Kernel kernel = builder.Build();
            kernel.Data["sqlConnectionString"] = sqlConnectionString;

            ChatCompletionAgent dataAnalysisAgent = new()
            {
                Name = DataAnalysisAgentName,
                Instructions = DataAnalysisAgentInstructions,
                Kernel = kernel,
                Arguments = new KernelArguments(new AzureOpenAIPromptExecutionSettings()
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                })
            };

            return dataAnalysisAgent;
        }
    }
}