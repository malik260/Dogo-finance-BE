using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.BusinessLogic.Layer.Models.Request;
using System.Threading.Tasks;

namespace DogoFinance.TransactionManagement.Interfaces
{
    public interface ICustomerPortfolioService
    {
        Task<ApiResponse> GetByCustomer(long customerId);
        Task<ApiResponse> Save(CustomerPortfolioDto model);
        Task<ApiResponse> Delete(long id);
    }
}
