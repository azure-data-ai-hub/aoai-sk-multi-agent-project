using MultiAgentWebAPI.Agents;
using Microsoft.AspNetCore.Mvc;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Agents;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<Kernel>((_) =>
{
    IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
    kernelBuilder.AddAzureOpenAIChatCompletion(
     deploymentName: builder.Configuration["AzureOpenAI:DeploymentName"]!,
     endpoint: builder.Configuration["AzureOpenAI:EndPoint"]!,
     apiKey: builder.Configuration["ApiManagement:ApiKey"]!
    );

  
    return kernelBuilder.Build();
});

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

    ChatCompletionAgent projectStatusAgent = new ProjectStatusAgent().Initialize(
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

    ChatCompletionAgent vendorManagementAgent = new VendorManagementAgent().Initialize(
        deploymentName: builder.Configuration["AzureOpenAI:DeploymentName"]!,
        endPoint: builder.Configuration["AzureOpenAI:EndPoint"]!,
        apiKey: builder.Configuration["AzureOpenAI:ApiKey"]!
        );

    // Create a chat for agent interaction.
    AgentGroupChat chat =
        new(projectManagerAgent, projectStatusAgent, projectTaskAgent, safetyRiskAgent, scheduleAgent, finanaceAgent, vendorManagementAgent)
        {
            ExecutionSettings =
                new()
                {
                    // Here a TerminationStrategy subclass is used that will terminate when
                    // an assistant message contains the term "approve".
                    TerminationStrategy =
                        new ApprovalTerminationStrategy()
                        {
                            Agents = [projectStatusAgent],
                            MaximumIterations = 10,
                        }
                }
        };


    // Invoke chat and display messages.
    ChatMessageContent input = new(AuthorRole.User, message);
    chat.AddChatMessage(input);

    var messages = new List<object>();

    await foreach (ChatMessageContent content in chat.InvokeAsync())
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
