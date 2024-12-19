using System.ComponentModel; 
using MultiAgentWebAPI.Entities;
using Microsoft.SemanticKernel;
using MultiAgentWebAPI.Utilities;

namespace MultiAgentWebAPI.Plugins
{
    public class FinancePlugin()
    {
        [KernelFunction("get_finanace_details")]
        [Description("Gets Finanace Details for the Project given ProjectID.")]
        public IEnumerable<Finance> GetFinanaceDetails(string ProjectID)
        {
            try
            {
                Console.WriteLine($"Get finance details from CSV for: {ProjectID}.");

                var csvFilePath = "data/Project Financials.csv";

                var finances = CSVHelper.ReadRecords<Finance>(csvFilePath, f => f.ProjectID.Equals(ProjectID, StringComparison.OrdinalIgnoreCase));

                if (finances != null)
                {
                    return finances;
                }
                else
                {
                    Console.WriteLine("No finance data found in CSV.");
                    return Enumerable.Empty<Finance>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An exception occurred while getting finance details: {ex.Message}");
            }
        }
    }
}
