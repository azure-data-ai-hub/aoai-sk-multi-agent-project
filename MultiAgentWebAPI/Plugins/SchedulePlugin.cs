using System.ComponentModel; 
using MultiAgentWebAPI.Entities;
using Microsoft.Azure.Cosmos;
using Microsoft.SemanticKernel;

namespace MultiAgentWebAPI.Plugins
{
    public class SchedulePlugin()
    {
        //private readonly CosmosClient _cosmosClient = cosmosClient;

        [KernelFunction("get_schedule_details")]
        [Description("Gets Schedule Details for the Project.")]
        public async Task<Schedule> GetSchedule(Kernel kernel, string ProjectID)
        {
            try
            {
                Console.WriteLine($"Get schedule details from cosmos for the {ProjectID}.");

                /*var db = _cosmosClient.GetDatabase("MultiAgentCosmosDB");
                var container = db.GetContainer("Schedules");

                var query = $"SELECT c.ProjectID , c.CurrentMileStone, c.Details FROM c where c.ProjectID=={ProjectID}";

                var schdule = await container.ReadItemAsync<Schedule>(id: ProjectID.ToString(), partitionKey: new PartitionKey("Location"));*/

                return new() { CurrentMileStone="December 31st", Details="Interior wroks to be completed by 12/24", ProjectID="P12345"};
            }
            catch (Exception ex)
            {
                throw new Exception($"An exception occurred while getting schedule details: {ex}");
            }
        }
    }
}
