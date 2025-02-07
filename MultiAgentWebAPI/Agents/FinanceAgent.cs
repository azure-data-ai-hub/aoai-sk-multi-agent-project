using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using MultiAgentWebAPI.Plugins;
using Microsoft.Extensions.Logging;

namespace MultiAgentWebAPI.Agents
{
    public class FinanceAgent: IAgent
    {
        private const string FinanceAgentName = "FinanceAgent";
        private const string FinanceAgentInstructions =
            """
            You are a Finance Agent specializing in providing finanace related data. For user purchase related queries, create valid SQL queries from the user's prompt and intent and execute the query using the tool:
            - execute_sql_query
         
            SQL Table Schema Overview:
            - PurchaseOrderDetail -> (PurchaseOrderID, PurchaseOrderDetailID, DueDate, OrderQty, ProductID, UnitPrice, LineTotal, ReceivedQty, RejectedQty, StockedQty, ModifiedDate)
            - PurchaseOrderHeader -> (PurchaseOrderID, RevisionNumber, Status, EmployeeID, VendorID, ShipMethodID, OrderDate, ShipDate, SubTotal, TaxAmt, Freight, TotalDue, ModifiedDate)
            - ProductCostHistory -> (ProductID,StartDate,EndDate,StandardCost,ModifiedDate)
            
            Example Queries:
            - What are the total expenditures on purchases for the current fiscal year?
            - Analyze the cost trends of ProductID 870 over the past six months.
            - Provide a detailed report on purchase order histories for VendorID 12345.
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

            ChatCompletionAgent financeAgent = new()
            {
                Name = FinanceAgentName,
                Instructions = FinanceAgentInstructions,
                Kernel = kernel,
                Arguments = new KernelArguments(new AzureOpenAIPromptExecutionSettings()
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                })
            };

            return financeAgent;
        }
    }
}