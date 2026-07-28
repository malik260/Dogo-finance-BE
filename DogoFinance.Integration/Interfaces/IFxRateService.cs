namespace DogoFinance.Integration.Interfaces
{
    public class FxRateResult
    {
        public decimal NgnPerUsdRate { get; set; }
        public decimal EffectiveRateWithMargin { get; set; }
        public string Provider { get; set; } = string.Empty;
        public bool IsFallback { get; set; }
        public DateTime FetchedAt { get; set; } = DateTime.UtcNow;
    }

    public interface IFxRateService
    {
        Task<FxRateResult> GetNgnToUsdRateAsync();
    }
}
