using System.ComponentModel; 
using MultiAgentWebAPI.Entities;
using Microsoft.Azure.Cosmos;
using Microsoft.SemanticKernel;

namespace MultiAgentWebAPI.Plugins
{
    public class ProjectPlugin()
    {
        //private readonly CosmosClient _cosmosClient = cosmosClient;

        [KernelFunction("get_project_details")]
        [Description("Gets Details for the Project.")]
        public async Task<Project> GetProject(Kernel kernel, string ProjectName)
        {
            try
            {
                Console.WriteLine($"Get project details from cosmos for the {ProjectName}.");

                /* var db = _cosmosClient.GetDatabase("MultiAgentCosmosDB");
                var container = db.GetContainer("Schedules");

                var query = $"SELECT c.ProjectID , c.ProjectDetails FROM c where c.ProjectName=={ProjectName}";

                var schdule = await container.ReadItemAsync<Project>(id: ProjectName, partitionKey: new PartitionKey("Location")); */

                return new() { Details="Grand Hotel Project Located in the heart of Seattle, WA", ID="P12345", Location="Seattle, WA", Name="Hotel Grand" };
            }
            catch (Exception ex)
            {
                throw new Exception($"An exception occurred while getting porject details: {ex}");
            }
        }
    }
}
