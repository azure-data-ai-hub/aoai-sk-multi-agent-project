namespace MultiAgentWebAPI.Entities;
public class VendorFinancials
{
    public required string OBJECTID { get; set; }
    public required string ContractNumber { get; set; }
    public required string ContractDescription { get; set; }
    public required string ContractStatus { get; set; }
    public required string ContractType { get; set; }
    public required string ContractSubtype { get; set; }
    public required string LastModified { get; set; }
    public required string OriginalAmount { get; set; }
    public required string RevisedAmount { get; set; }
    public required string VendorID { get; set; }
    public required string TempID { get; set; }
    public required string VendorName { get; set; }
    public required string ProjectID { get; set; }
}
