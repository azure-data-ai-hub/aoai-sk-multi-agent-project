 using Microsoft.SemanticKernel;
 using Microsoft.SemanticKernel.ChatCompletion;
 using Microsoft.SemanticKernel.Connectors.OpenAI;


namespace MultiAgentWebAPI.Agents
{
    public class MaintenanceCopilot(Kernel kernel)
    {
        public readonly Kernel _kernel = kernel;
        private ChatHistory _history = new( """
            You are a friendly assistant who likes to follow the rules. You will complete required steps
            and request approval before taking any consequential actions, such as saving the request to the database.
            If the user doesn't provide enough information for you to complete a task, you will keep asking questions
            until you have enough information to complete the task. Once the request has been saved to the database,
            inform the user that hotel maintenance has been notified and will address the issue as soon as possible.
            """ );

        public async Task<string> Chat(string userPrompt)
        {
            var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

            var openAIPromptExecutionSettings = new OpenAIPromptExecutionSettings()
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
            };

            _history.AddUserMessage(userPrompt);

            var result = await chatCompletionService.GetChatMessageContentAsync(
                _history,
                executionSettings: openAIPromptExecutionSettings,
                _kernel
            );

            _history.AddAssistantMessage(result.Content!);

            return result.Content!;
        }
    }
}
