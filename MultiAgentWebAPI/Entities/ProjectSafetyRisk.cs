namespace MultiAgentWebAPI.Entities;
public class ProjectSafetyRisk
{
    public required string Ref { get; set; }
    public required string Status { get; set; }
    public required string Location { get; set; }
    public required string Description { get; set; }
    public required string Created { get; set; }
    public required string Target { get; set; }
    public required string Type { get; set; }
    public required string ToPackage { get; set; }
    public required string StatusChanged { get; set; }
    public required string Association { get; set; }
    public required string OverDue { get; set; }
    public required string Images { get; set; }
    public required string Comments { get; set; }
    public required string Documents { get; set; }
    public required string Priority { get; set; }
    public required string Cause { get; set; }
    public required string ProjectID { get; set; }
    public required string ReportStatus { get; set; }
    public required string TaskGroup { get; set; }
}
