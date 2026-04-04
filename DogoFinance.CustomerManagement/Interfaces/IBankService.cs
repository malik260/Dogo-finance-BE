using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.BusinessLogic.Layer.Response;
using System.Threading.Tasks;

namespace DogoFinance.CustomerManagement.Interfaces
{
    public interface IBankService
    {
        Task<ApiResponse> GetBanks();
        Task<ApiResponse> GetCustomerBanks(long userId);
        Task<ApiResponse> AddCustomerBank(long userId, AddCustomerBankRequest request);
        Task<ApiResponse> DeleteCustomerBank(long userId, long customerBankId);
        Task<ApiResponse> SetDefaultBank(long userId, long customerBankId);
    }
}
