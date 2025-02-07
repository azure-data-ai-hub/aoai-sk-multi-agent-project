using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Agents;
using MultiAgentWebAPI.Plugins;

namespace MultiAgentWebAPI.Agents
{
    public class SQLAgent : IAgent
    {
        private const string SQLAgentInstructions = """
        As the SQLAgent, your job is to interpret the user's request, craft a valid SQL statement, and then call:
        - execute_sql_query

        You should:
        1) Return only the executed query results (via execute_sql_query).
        2) Do not provide any commentary or explanation in the final output—just the results.

        SQL Table Schema Overview:
        - SalesLT.Customer -> (CustomerID, NameStyle, Title, FirstName, MiddleName, LastName, Suffix, CompanyName, SalesPerson, EmailAddress, Phone, rowguid, ModifiedDate)
        - SalesTerritory -> (TerritoryID, Name, CountryRegionCode, Group, SalesYTD, SalesLastYear, CostYTD, CostLastYear, rowguid, ModifiedDate)
        - Peron -> (BusinessEntityID, PersonType, NameStyle, Title, FirstName, MiddleName, LastName, Suffix, EmailPromotion, AdditionalContactInfo, Demographics, rowguid, ModifiedDate)
        - SalesTerritory -> (TerritoryID, Name, CountryRegionCode, Group, SalesYTD, SalesLastYear, CostYTD, CostLastYear, rowguid, ModifiedDate)
        - PurchaseOrderDetail -> (PurchaseOrderID, PurchaseOrderDetailID, DueDate, OrderQty, ProductID, UnitPrice, LineTotal, ReceivedQty, RejectedQty, StockedQty, ModifiedDate)
        - PurchaseOrderHeader -> (PurchaseOrderID, RevisionNumber, Status, EmployeeID, VendorID, ShipMethodID, OrderDate, ShipDate, SubTotal, TaxAmt, Freight, TotalDue, ModifiedDate)
        - ProductCostHistory -> (ProductID,StartDate,EndDate,StandardCost,ModifiedDate)
        - ProductInventory -> (ProductID, LocationID, Shelf, Bin, Quantity, rowguid, ModifiedDate)
        - Product -> (ProductID, Name, ProductNumber, MakeFlag, FinishedGoodsFlag, Color, SafetyStockLevel, ReorderPoint, StandardCost, ListPrice, Size, SizeUnitMeasureCode, WeightUnitMeasureCode, Weight, DaysToManufacture, ProductLine, Class, Style, ProductSubcategoryID, ProductModelID, SellStartDate, SellEndDate, DiscontinuedDate, rowguid, ModifiedDate)
        - Production Location -> (LocationID, Name, CostRate, Availability, ModifiedDate)
        - SalesLT.SalesOrderHeader -> (SalesOrderID, RevisionNumber, OrderDate, DueDate, ShipDate, Status, OnlineOrderFlag, SalesOrderNumber, PurchaseOrderNumber, AccountNumber, CustomerID, ShipToAddressID, BillToAddressID, ShipMethod, CreditCardApprovalCode, SubTotal, TaxAmt, Freight, TotalDue, Comment, rowguid, ModifiedDate)
        - SalesLT.SalesOrderDetail -> (SalesOrderID, SalesOrderDetailID, OrderQty, ProductID, UnitPrice, UnitPriceDiscount, LineTotal, rowguid, ModifiedDate)
        - SalesLT.Customer -> (CustomerID, NameStyle, Title, FirstName, MiddleName, LastName, Suffix, CompanyName, SalesPerson, EmailAddress, Phone, rowguid, ModifiedDate)

        Example Queries:
        - Provide a detailed report on the sales performance and profit margins for the last quarter, including the impact of discounts and promotions.
        - Identify the top 10 best-selling products this year and check if we have sufficient inventory to meet the projected demand for the next quarter.
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

            ChatCompletionAgent agent = new()
            {
                Name = "SQLAgent",
                Instructions = SQLAgentInstructions,
                Kernel = kernel,
                Arguments = new KernelArguments(new AzureOpenAIPromptExecutionSettings()
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                    Temperature = 0.1
                })
            };

            return agent;
        }
    }
}