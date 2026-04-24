namespace DogoFinance.Integration.Interfaces
{
    public interface IDocumentProcessingService
    {
        Task<ExtractedAddressData> ExtractAddressAsync(string imageUrl);
    }

    public class ExtractedAddressData
    {
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? FullText { get; set; }
        public decimal ConfidenceScore { get; set; }
    }
}
