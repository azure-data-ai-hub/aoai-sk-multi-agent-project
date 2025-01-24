using Microsoft.SemanticKernel.Agents;

namespace MultiAgentWebAPI.Agents
{
    public static class AgentFactory
    {
        public static ChatCompletionAgent CreateAgent<T>(IConfiguration config)
            where T : IAgent, new()
        {
            string deploymentName = config["AzureOpenAI:DeploymentName"]!;
            string endPoint = config["AzureOpenAI:EndPoint"]!;
            string apiKey   = config["AzureOpenAI:ApiKey"]!;

            var agent = new T();
            return agent.Initialize(endPoint, deploymentName, apiKey);
        } 
    }
   
}