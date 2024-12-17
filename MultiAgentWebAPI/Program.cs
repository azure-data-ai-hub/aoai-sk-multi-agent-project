using Microsoft.Azure.Cosmos;
using MultiAgentWebAPI.Agents;
using MultiAgentWebAPI.Plugins;
using MultiAgentWebAPI.Services;
using Azure.AI.OpenAI;
using Azure;
using Microsoft.AspNetCore.Mvc;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.ChatCompletion;
using MultiAgentWebAPI;
using Microsoft.SemanticKernel.Agents;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IVectorizationService, VectorizationService>();
builder.Services.AddSingleton<MaintenanceCopilot, MaintenanceCopilot>();

builder.Services.AddSingleton<CosmosClient>((_) =>
{
    CosmosClient client = new(
        connectionString: builder.Configuration["CosmosDB:ConnectionString"]!
    );
    return client;
});

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

app.MapGet("/", async () =>
{
    return "Welcome to the Multi Agent Web API!";
})
    .WithName("Index");

app.MapPost("/ProjectManagerAgentChat", async Task<string> (HttpRequest request) =>
{
    return "Welcome to Project Manager Agent Chat";
})
    .WithName("ProjectManagerAgentChat");

app.MapPost("/ScheduleAgentChat", async Task<string> (HttpRequest request) =>
{
    return"Welcome to Scheudle Agent Chat";
})
    .WithName("ScheduleAgentChat");

app.MapPost("/FinanaceAgentChat", async Task<string> (HttpRequest request) =>
{
    return "Welcome to Finanace Agent Chat";
})
    .WithName("FinanaceAgentChat");


app.MapPost("/MultiAgentChat", async ([FromBody] string message) =>
{

    // TODO: Factory pattern to create tehse agents

    ChatCompletionAgent projectManagerAgent = new ProjectManagerAgent().Initialize(
        deploymentName: builder.Configuration["AzureOpenAI:DeploymentName"]!,
        endPoint: builder.Configuration["AzureOpenAI:EndPoint"]!,
        apiKey: builder.Configuration["AzureOpenAI:ApiKey"]!,
        cosmosConnectionString: ""
        );


    ChatCompletionAgent scheduleAgent = new ScheduleAgent().Initialize(
        deploymentName: builder.Configuration["AzureOpenAI:DeploymentName"]!,
        endPoint: builder.Configuration["AzureOpenAI:EndPoint"]!,
        apiKey: builder.Configuration["AzureOpenAI:ApiKey"]!,
        cosmosConnectionString: ""
        );

    ChatCompletionAgent finanaceAgent = new FinanaceAgent().Initialize(
        deploymentName: builder.Configuration["AzureOpenAI:DeploymentName"]!,
        endPoint: builder.Configuration["AzureOpenAI:EndPoint"]!,
        apiKey: builder.Configuration["AzureOpenAI:ApiKey"]!,
        cosmosConnectionString: ""
        );

    // Create a chat for agent interaction.
    AgentGroupChat chat =
        new(projectManagerAgent, scheduleAgent, finanaceAgent)
        {
            ExecutionSettings =
                new()
                {
                    // Here a TerminationStrategy subclass is used that will terminate when
                    // an assistant message contains the term "approve".
                    TerminationStrategy =
                        new ApprovalTerminationStrategy()
                        {
                            Agents = [projectManagerAgent],
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


app.MapGet("/Vectorize", async (string text, [FromServices] IVectorizationService vectorizationService) =>
{
    var embeddings = await vectorizationService.GetEmbeddings(text);
    return embeddings;
})
    .WithName("Vectorize");

app.MapPost("/VectorSearch", async ([FromBody] float[] queryVector, [FromServices] IVectorizationService vectorizationService, int max_results = 0, double minimum_similarity_score = 0.8) =>
{
    var results = await vectorizationService.ExecuteVectorSearch(queryVector, max_results, minimum_similarity_score);
    return results;

})
    .WithName("VectorSearch");

app.MapPost("/MaintenanceCopilotChat", async ([FromBody] string message, [FromServices] MaintenanceCopilot copilot) =>
{
    var response = await copilot.Chat(message);
    return response;

})
    .WithName("Copilot");

app.Run();
