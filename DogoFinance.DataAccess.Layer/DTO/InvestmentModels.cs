namespace DogoFinance.DataAccess.Layer.DTO
{
    public class InvestRequestDto
    {
        public int PortfolioId { get; set; }
        public decimal Amount { get; set; }
    }

    public class SellRequestDto
    {
        public int PortfolioId { get; set; }
        public decimal Amount { get; set; }
    }

    public class PortfolioSummaryDto
    {
        public decimal Invested { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal Profit { get; set; }
        public decimal ReturnPercentage { get; set; }
    }
}
