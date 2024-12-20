using System.ComponentModel;
using MultiAgentWebAPI.Entities;
using Microsoft.SemanticKernel;
using MultiAgentWebAPI.Utilities;

namespace MultiAgentWebAPI.Plugins
{
    public class ProjectTaskPlugin()
    {
        [KernelFunction("get_project_task_details")]
        [Description("Gets Details for the Project Daily tasks by passing ProjectID as input.")]
        public IEnumerable<ProjectTask> GetProject(string ProjectID)
        {
            try
            {
                Console.WriteLine($"Get project details from CSV for: {ProjectID}.");

                var csvFilePath = "data/Project Management Daily Tasks.csv";

                var projectTasks = CSVHelper.ReadRecords<ProjectTask>(csvFilePath, s => s.ProjectID.Contains(ProjectID, StringComparison.OrdinalIgnoreCase));
                
                if (projectTasks != null)
                {
                    return projectTasks;
                }
                else
                {
                    Console.WriteLine("No schedule found in CSV.");
                    return Enumerable.Empty<ProjectTask>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An exception occurred while getting porject details: {ex}");
            }
        }
    }
}
