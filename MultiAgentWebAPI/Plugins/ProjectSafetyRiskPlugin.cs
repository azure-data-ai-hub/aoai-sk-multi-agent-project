using System.ComponentModel; 
using MultiAgentWebAPI.Entities;
using Microsoft.SemanticKernel;
using MultiAgentWebAPI.Utilities;

namespace MultiAgentWebAPI.Plugins
{
    public class ProjectSafetyRiskPlugin()
    {
        [KernelFunction("get_project_safety_risk_details")]
        [Description("Gets Project Safety Risks for the Project given ProjectID as input.")]
        public IEnumerable<ProjectSafetyRisk?> GetSafetyRisks(string ProjectID)
        {
            try
            {
                Console.WriteLine($"Get safety risk details from CSV for: {ProjectID}.");

                var csvFilePath = "data/Project Management Safety Risk Reports.csv";

                var safetyRisks = CSVHelper.ReadRecords<ProjectSafetyRisk>(csvFilePath, r => r.ProjectID.Equals(ProjectID, StringComparison.OrdinalIgnoreCase));

                if (safetyRisks != null)
                {
                    return safetyRisks;
                }
                else
                {
                    Console.WriteLine("No safety risk data found in CSV.");
                    return Enumerable.Empty<ProjectSafetyRisk>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An exception occurred while getting safety risk details: {ex.Message}");
            }
        }
    }
}
