namespace MultiAgentWebAPI.Entities;
public class Schedule
{
    public required string ProjectID { get; set; }
    public required string LatestScheduleChanges { get; set; }
    public required string TotalScheduleChanges { get; set; }
}
