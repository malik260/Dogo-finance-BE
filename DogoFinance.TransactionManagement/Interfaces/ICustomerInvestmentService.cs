using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.DataAccess.Layer.DTO;
using System.Threading.Tasks;

namespace DogoFinance.TransactionManagement.Interfaces
{
    public interface ICustomerInvestmentService
    {
        Task<ApiResponse> InvestAsync(InvestRequestDto request, long customerId);
        Task<ApiResponse> SellAsync(SellRequestDto request, long customerId);
        Task<ApiResponse> GetPortfolioSummary(long customerId);
    }
}
