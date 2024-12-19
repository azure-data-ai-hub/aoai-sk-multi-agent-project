using System.ComponentModel; 
using MultiAgentWebAPI.Entities;
using Microsoft.SemanticKernel;
using MultiAgentWebAPI.Utilities;

namespace MultiAgentWebAPI.Plugins
{
    public class SchedulePlugin()
    {
        [KernelFunction("get_schedule_details")]
        [Description("Gets Schedule Details for the Project.")]
        public IEnumerable<Schedule> GetSchedule(Kernel kernel, string ProjectID)
        {
             try
            {
                Console.WriteLine($"Get schedule details from CSV for: {ProjectID}.");

                var csvFilePath = "data/Project Financials.csv";

                var schedule = CSVHelper.ReadRecords<Schedule>(csvFilePath, s => s.ProjectID.Equals(ProjectID, StringComparison.OrdinalIgnoreCase));

                if (schedule != null)
                {
                    return schedule;
                }
                else
                {
                    Console.WriteLine("No schedule found in CSV.");
                    return Enumerable.Empty<Schedule>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An exception occurred while getting schedule details: {ex.Message}");
            }
        }
    }
}
