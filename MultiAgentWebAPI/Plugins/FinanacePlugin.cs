using System.ComponentModel; 
using MultiAgentWebAPI.Entities;
using Microsoft.Azure.Cosmos;
using Microsoft.SemanticKernel;

namespace MultiAgentWebAPI.Plugins
{
    public class FinanacePlugin()
    {
        //private readonly CosmosClient _cosmosClient = cosmosClient;

        [KernelFunction("get_finanace_details")]
        [Description("Gets Finanace Details for the Project.")]
        public async Task<Finanace> GetFinanaceDetails(Kernel kernel, string ProjectID)
        {
            try
            {
                Console.WriteLine($"Get Finanace detials from cosmos for the {ProjectID}.");

                /*var db = _cosmosClient.GetDatabase("MultiAgentCosmosDB");
                var container = db.GetContainer("Finanaces");

                var query = $"SELECT c.ProjectID , c.Cost, c.Details FROM c where c.ProjectID=={ProjectID}";

                var schdule = await container.ReadItemAsync<Finanace>(id: ProjectID.ToString(), partitionKey: new PartitionKey("Location"));*/

                return new() { Cost="100 Million Dollars", Details="Multi mortage finanace from KeyBank, FirstTech and Chase", ProjectID="P12345"};
            }
            catch (Exception ex)
            {
                throw new Exception($"An exception occurred while getting finanace details from cosmos: {ex}");
            }
        }
    }
}
