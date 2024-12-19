namespace MultiAgentWebAPI.Entities;
public class Finance
{
    public required string DateReportedAsOf { get; set; }
    public required string ProjectID { get; set; }
    public required string ProjectName { get; set; }
    public required string Description { get; set; }
    public required string Category { get; set; }
    public required string Borough { get; set; }
    public required string ManagingAgency { get; set; }
    public required string ClientAgency { get; set; }
    public required string CurrentPhase { get; set; }
    public required string DesignStart { get; set; }
    public required string BudgetForecast { get; set; }
    public required string LatestBudgetChanges { get; set; }
    public required string TotalBudgetChanges { get; set; }
    public required string ForecastCompletion { get; set; }
}
