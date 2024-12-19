using System.ComponentModel;
using MultiAgentWebAPI.Entities;
using Microsoft.SemanticKernel;
using MultiAgentWebAPI.Utilities;

namespace MultiAgentWebAPI.Plugins
{
    public class ProjectPlugin()
    {
        [KernelFunction("get_project_details")]
        [Description("Gets Details for the Project by passing ProjectNam as input.")]
        public Project? GetProject(Kernel kernel, string ProjectName)
        {
            try
            {
                Console.WriteLine($"Get project details from CSV for: {ProjectName}.");

                var csvFilePath = "data/Projectinfo.csv";

                var project = CSVHelper.ReadRecords<Project>(csvFilePath, s => s.ProjectName.Contains(ProjectName, StringComparison.OrdinalIgnoreCase));
                
                if (project == null || !project.Any())
                {
                    Console.WriteLine("No project found in CSV.");
                    return null;
                }

                return project.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception($"An exception occurred while getting porject details: {ex}");
            }
        }
    }
}
