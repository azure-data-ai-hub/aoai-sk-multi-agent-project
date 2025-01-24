using MultiAgentWebAPI.Agents;
using MultiAgentWebAPI.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.Agents.History;
using ChatResponseFormat = OpenAI.Chat.ChatResponseFormat;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000",
        builder => builder.WithOrigins("http://localhost:3000")
                          .AllowAnyMethod()
                          .AllowAnyHeader());

    options.AddPolicy("AllowMultiAgentWebAPP",
        builder => builder.WithOrigins("https://multiagentwebapp.azurewebsites.net")
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

var app = builder.Build();

app.UseCors("AllowLocalhost3000");
app.UseCors("AllowMultiAgentWebAPP");

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () =>
{
    return "Welcome to the Multi Agent Web API!";
})
    .WithName("Index");

app.MapPost("/MultiAgentChat", async ([FromBody] string message) =>
{
    var projectLeaderAgent = AgentFactory.CreateAgent<ProjectLeaderAgent>(builder.Configuration);
    var dataAnalysisAgent = AgentFactory.CreateAgent<DataAnalysisAgent>(builder.Configuration);
    var inventoryManagementAgent = AgentFactory.CreateAgent<InventoryManagementAgent>(builder.Configuration);
    var salesAgent = AgentFactory.CreateAgent<SalesAgent>(builder.Configuration);
    var finanaceAgent = AgentFactory.CreateAgent<FinanceAgent>(builder.Configuration);
    var customerServiceAgent = AgentFactory.CreateAgent<CustomerServiceAgent>(builder.Configuration);

    IKernelBuilder kbuilder = Kernel.CreateBuilder();
    kbuilder.AddAzureOpenAIChatCompletion(
        deploymentName: builder.Configuration["AzureOpenAI:ReasoningModelDeploymentName"]!,
        endpoint: builder.Configuration["AzureOpenAI:EndPoint"]!,
        apiKey: builder.Configuration["AzureOpenAI:ApiKey"]!
    );

    kbuilder.Services.AddLogging(config => { config.AddConsole(); config.SetMinimumLevel(LogLevel.Trace); });
    
    var kernel = kbuilder.Build();

    KernelFunction selectionFunction =
    AgentGroupChat.CreatePromptFunctionForStrategy(
        $$$"""
        Determine which participant takes the next turn in a conversation based on the most recent participant's response and the history of the conversation.
        State only the name of the participant with out explanation to take the next turn.

        Choose only from these participants:
        - {{{salesAgent.Name}}}
        - {{{dataAnalysisAgent.Name}}}
        - {{{inventoryManagementAgent.Name}}}
        - {{{finanaceAgent.Name}}}
        - {{{customerServiceAgent.Name}}}
        
        
        Based on the history, delegate requests to the appropriate agents:
        - If any financial details are required, ask {{{finanaceAgent.Name}}} to get the required data.
        - If any sales details details are required, ask {{{salesAgent.Name}}}.
        - If any inventory details are required, ask {{{inventoryManagementAgent.Name}}}.
        - If any customer details are required, ask {{{customerServiceAgent.Name}}}.
        - If any vendor data analysis required, ask {{{dataAnalysisAgent.Name}}}.
        - If any of the agents requests additional information, then ask {{{projectLeaderAgent.Name}}}

        History:
        {{$history}}
        """,
        safeParameterNames: "history");

    // Define the selection strategy
    KernelFunctionSelectionStrategy selectionStrategy =
      new(selectionFunction, kernel)
      {
          // Parse the function response.
          ResultParser = (result) => result.GetValue<string>() ?? projectLeaderAgent.Name!,
          // The prompt variable name for the history argument.
          HistoryVariableName = "history",
          // Save tokens by not including the entire history in the prompt
          HistoryReducer = new ChatHistoryTruncationReducer(5),
          Arguments = new KernelArguments(new AzureOpenAIPromptExecutionSettings() { Temperature = 0.1 } )
      };

    KernelFunction terminationFunction =
        AgentGroupChat.CreatePromptFunctionForStrategy(
            $$$"""
            Examine the RESPONSE and determine whether the content has been deemed satisfactory for user query "{{{message}}}".
            
            If the content has all information to satisfy the user query then respond with a single word: yes otherwise response with: no
            
            RESPONSE:
            {{$lastmessage}}
            """,
            safeParameterNames: "lastmessage");

    // Define the termination strategy
    KernelFunctionTerminationStrategy terminationStrategy =
      new(terminationFunction, kernel)
      {
          Agents = [projectLeaderAgent],

          // Parse the function response.
          ResultParser = (result) =>
        result.GetValue<string>()?.Contains("yes", StringComparison.OrdinalIgnoreCase) ?? false,
          // The prompt variable name for the history argument.
          HistoryVariableName = "lastmessage",
          // Save tokens by not including the entire history in the prompt
          HistoryReducer = new ChatHistoryTruncationReducer(1),
          // Limit total number of turns no matter what
          MaximumIterations = 5,
          Arguments = new KernelArguments(new AzureOpenAIPromptExecutionSettings() { Temperature = 0.1 })
      };

    const string OuterTerminationInstructions =
        $$$"""
        Determine if user request has been fully answered.
        
        respond with a single word: yes otherwise response with: no
        
        History:
        {{${{{KernelFunctionTerminationStrategy.DefaultHistoryVariableName}}}}}
        """;

    KernelFunction outerTerminationFunction = KernelFunctionFactory.CreateFromPrompt(OuterTerminationInstructions, new AzureOpenAIPromptExecutionSettings() {Temperature=0.1});

    // Create a chat for agent interaction.
    AgentGroupChat CreateChat() =>
        new(projectLeaderAgent, salesAgent, inventoryManagementAgent, customerServiceAgent, dataAnalysisAgent, finanaceAgent)
        {
            ExecutionSettings = new() { SelectionStrategy = selectionStrategy, TerminationStrategy = terminationStrategy }
        };

    AggregatorAgent projectAgent =
            new(CreateChat)
            {
                Name = "ProjectStatus",
                Mode = AggregatorMode.Flat,
            };

    AgentGroupChat chat =
        new(projectAgent)
        {
            ExecutionSettings =
                new()
                {
                    TerminationStrategy =
                        new KernelFunctionTerminationStrategy(outerTerminationFunction, kernel.Clone())
                        {
                            ResultParser = (result) => result.GetValue<string>()?.Contains("yes", StringComparison.OrdinalIgnoreCase) ?? false,
                            MaximumIterations = 5,
                        },
                }
        };

    // Invoke chat and display messages.
    ChatMessageContent input = new(AuthorRole.User, message);
    chat.AddChatMessage(input);

    var messages = new List<object>();

    await foreach (ChatMessageContent content in chat.InvokeAsync(projectAgent))
    {
        messages.Add(new
        {
            Role = content.Role,
            AuthorName = content.AuthorName ?? "*",
            Content = content.Content
        });
    }

    // Return JSON response to the caller
    return Results.Json(messages);
})
    .WithName("MultiAgentChat");


app.MapPost("/Chat", async Task<string> (HttpRequest request) =>
{
    var message = await Task.FromResult(request.Form["message"]);
    var kernel = app.Services.GetRequiredService<Kernel>();
    var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
    var executionSettings = new OpenAIPromptExecutionSettings
    {
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
    };
    var response = await chatCompletionService.GetChatMessageContentAsync(message.ToString(), executionSettings, kernel);
    return response?.Content!;
})
    .WithName("Chat");

app.Run();
