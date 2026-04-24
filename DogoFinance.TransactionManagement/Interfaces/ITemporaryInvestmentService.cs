using DogoFinance.BusinessLogic.Layer.Response;
using System.Threading.Tasks;

namespace DogoFinance.TransactionManagement.Interfaces
{
    public interface ITemporaryInvestmentService
    {
        /// <summary>
        /// Processes a portfolio investment using the NAV model.
        /// </summary>
        Task<ApiResponse> ProcessTempInvestment(long customerId, int portfolioId, decimal amount, string? pin = null, string? otp = null);

        /// <summary>
        /// Gets the current valuation and profit for a customer's portfolio.
        /// </summary>
        Task<ApiResponse> GetTempPortfolioStats(long customerId, int portfolioId);

        /// <summary>
        /// Simulates NAV growth for testing/demo purposes.
        /// </summary>
        Task<ApiResponse> SimulateNAVGrowth(int portfolioId, int days);
        Task<ApiResponse> GetActiveInvestments(long customerId);
        Task<ApiResponse> ProcessSell(long customerId, int portfolioId, decimal amount, string? pin = null, string? otp = null);
    }
}
