using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using MultiAgentWebAPI.Plugins;

namespace MultiAgentWebAPI.Agents
{
    public class SalesAgent: IAgent
    {
        private const string SalesAgentName = "SalesAgent";
        private const string SalesAgentInstructions =
            """
            You are a Sales Agent specializing in providing sales related data. For user sales related queries, create valid SQL queries from the user's prompt and intent and execute the query using the tool:
            - execute_sql_query

            SQL Table Schema Overview:
            - SalesLT.SalesOrderHeader -> (SalesOrderID, RevisionNumber, OrderDate, DueDate, ShipDate, Status, OnlineOrderFlag, SalesOrderNumber, PurchaseOrderNumber, AccountNumber, CustomerID, ShipToAddressID, BillToAddressID, ShipMethod, CreditCardApprovalCode, SubTotal, TaxAmt, Freight, TotalDue, Comment, rowguid, ModifiedDate)
            - SalesLT.SalesOrderDetail -> (SalesOrderID, SalesOrderDetailID, OrderQty, ProductID, UnitPrice, UnitPriceDiscount, LineTotal, rowguid, ModifiedDate)
            - SalesLT.Customer -> (CustomerID, NameStyle, Title, FirstName, MiddleName, LastName, Suffix, CompanyName, SalesPerson, EmailAddress, Phone, rowguid, ModifiedDate)

            Example Queries:
            - What were the total sales for the last quarter?
            - Which product had the highest sales revenue last month?
            - Provide a sales trend analysis for the past year.
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

            ChatCompletionAgent salesAgent = new()
            {
                Name = SalesAgentName,
                Instructions = SalesAgentInstructions,
                Kernel = kernel,
                Arguments = new KernelArguments(new AzureOpenAIPromptExecutionSettings()
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                })
            };

            return salesAgent;
        }
    }
}