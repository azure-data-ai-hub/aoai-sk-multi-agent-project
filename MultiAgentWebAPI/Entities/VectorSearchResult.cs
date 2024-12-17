namespace MultiAgentWebAPI.Entities
{
    public class VectorSearchResult
    {
        public required int ProjectId { get; set; }
        public required string ProjectName { get; set; }
        public required string ProjectDetails { get; set; }
        public required string Location { get; set; }
        public required float SimilarityScore { get; set; }
    }
}
