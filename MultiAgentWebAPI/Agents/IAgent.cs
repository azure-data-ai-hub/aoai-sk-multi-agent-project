using Microsoft.SemanticKernel.Agents;

namespace MultiAgentWebAPI.Agents
{
    public interface IAgent
    {
        ChatCompletionAgent Initialize(string endPoint, string deploymentName, string apiKey);
    }
}