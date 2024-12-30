using MultiAgentWebAPI.Agents;
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
});

var app = builder.Build();

app.UseCors("AllowLocalhost3000");

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

app.MapPost("/ProjectManagerAgentChat", string (HttpRequest request) =>
{
    return "Welcome to Project Manager Agent Chat";
})
    .WithName("ProjectManagerAgentChat");

app.MapPost("/ScheduleAgentChat", string (HttpRequest request) =>
{
    return"Welcome to Scheudle Agent Chat";
})
    .WithName("ScheduleAgentChat");

app.MapPost("/FinanaceAgentChat", string (HttpRequest request) =>
{
    return "Welcome to Finanace Agent Chat";
})
    .WithName("FinanaceAgentChat");


app.MapPost("/MultiAgentChat", async ([FromBody] string message) =>
{

    // TODO: Factory pattern to create these agents

    ChatCompletionAgent projectLeaderAgent = new ProjectLeaderAgent().Initialize(
        deploymentName: builder.Configuration["AzureOpenAI:DeploymentName"]!,
        endPoint: builder.Configuration["AzureOpenAI:EndPoint"]!,
        apiKey: builder.Configuration["AzureOpenAI:ApiKey"]!
        );

    ChatCompletionAgent projectManagerAgent = new ProjectManagerAgent().Initialize(
        deploymentName: builder.Configuration["AzureOpenAI:DeploymentName"]!,
        endPoint: builder.Configuration["AzureOpenAI:EndPoint"]!,
        apiKey: builder.Configuration["AzureOpenAI:ApiKey"]!
        );

    ChatCompletionAgent projectTaskAgent = new ProjectTasksAgent().Initialize(
        deploymentName: builder.Configuration["AzureOpenAI:DeploymentName"]!,
        endPoint: builder.Configuration["AzureOpenAI:EndPoint"]!,
        apiKey: builder.Configuration["AzureOpenAI:ApiKey"]!
        );

    ChatCompletionAgent safetyRiskAgent = new SafetyRiskAgent().Initialize(
        deploymentName: builder.Configuration["AzureOpenAI:DeploymentName"]!,
        endPoint: builder.Configuration["AzureOpenAI:EndPoint"]!,
        apiKey: builder.Configuration["AzureOpenAI:ApiKey"]!
        );

    ChatCompletionAgent scheduleAgent = new ScheduleAgent().Initialize(
        deploymentName: builder.Configuration["AzureOpenAI:DeploymentName"]!,
        endPoint: builder.Configuration["AzureOpenAI:EndPoint"]!,
        apiKey: builder.Configuration["AzureOpenAI:ApiKey"]!
        );

    ChatCompletionAgent finanaceAgent = new FinanceAgent().Initialize(
        deploymentName: builder.Configuration["AzureOpenAI:DeploymentName"]!,
        endPoint: builder.Configuration["AzureOpenAI:EndPoint"]!,
        apiKey: builder.Configuration["AzureOpenAI:ApiKey"]!
        );

    ChatCompletionAgent vendorFinanceAgent = new VendorFinanceAgent().Initialize(
        deploymentName: builder.Configuration["AzureOpenAI:DeploymentName"]!,
        endPoint: builder.Configuration["AzureOpenAI:EndPoint"]!,
        apiKey: builder.Configuration["AzureOpenAI:ApiKey"]!
        );

    IKernelBuilder kbuilder = Kernel.CreateBuilder();
    kbuilder.AddAzureOpenAIChatCompletion(
        deploymentName: "gpt-4o",
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
        - {{{projectLeaderAgent.Name}}}
        - {{{projectTaskAgent.Name}}}
        - {{{safetyRiskAgent.Name}}}
        - {{{scheduleAgent.Name}}}
        - {{{finanaceAgent.Name}}}
        - {{{vendorFinanceAgent.Name}}}

        Based on the history, delegate requests to the appropriate agents:
        - If any financial details are required, ask {{{finanaceAgent.Name}}} to get the required data.
        - If any Project daily tasks details are required, ask {{{projectTaskAgent.Name}}}.
        - If any Safety, risks, and compliance details are required, ask {{{safetyRiskAgent.Name}}}.
        - If any Schedule details are required, ask {{{scheduleAgent.Name}}}.
        - If any vendor data, ask {{{vendorFinanceAgent.Name}}}.
        - If any of the agents requests additional information, then ask {{{projectLeaderAgent.Name}}}

        History:
        {{$history}}
        """,
        safeParameterNames: "history");

    // Define the selection strategy
    KernelFunctionSelectionStrategy selectionStrategy =
      new(selectionFunction, kernel)
      {
          // Always start with the writer agent.
          InitialAgent = projectManagerAgent,
          // Parse the function response.
          ResultParser = (result) => result.GetValue<string>() ?? projectManagerAgent.Name!,
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
        new(projectLeaderAgent, projectManagerAgent, projectTaskAgent, safetyRiskAgent, scheduleAgent, finanaceAgent, vendorFinanceAgent)
        {
            ExecutionSettings = new() { SelectionStrategy = selectionStrategy, TerminationStrategy = terminationStrategy }
        };

    AggregatorAgent projectAgent =
            new(CreateChat)
            {
                Name = "ProjectStatus",
                Mode = AggregatorMode.Nested,
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
