using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.BusinessLogic.Layer.Response;

namespace DogoFinance.TransactionManagement.Interfaces
{
    public interface ITransactionService
    {
        Task<ApiResponse> InitiateDeposit(long customerId, decimal amount);
        Task<ApiResponse> ConfirmDeposit(string reference);
        Task<ApiResponse> InitiateWithdrawal(WithdrawalRequest request);
        Task<ApiResponse> GetTransactionHistory(long userId);
        Task<ApiResponse> GetWallet(long customerId);
        Task<ApiResponse> GetFinanceSummary();
    }
}
