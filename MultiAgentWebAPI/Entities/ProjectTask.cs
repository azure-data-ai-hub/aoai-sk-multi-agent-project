namespace MultiAgentWebAPI.Entities;
public class ProjectTask
{
    public required string Ref { get; set; }
    public required string Status { get; set; }
    public required string Location { get; set; }
    public required string Name { get; set; }
    public required string Created { get; set; }
    public required string Type { get; set; }
    public required string StatusChanged { get; set; }
    public required string OpenActions { get; set; }
    public required string TotalActions { get; set; }
    public required string Association { get; set; }
    public required string OverDue { get; set; }
    public required string Images { get; set; }
    public required string Comments { get; set; }
    public required string Documents { get; set; }
    public required string ProjectID { get; set; }
    public required string ReportFormsStatus { get; set; }
    public required string ReportFormsGroup { get; set; }
}
