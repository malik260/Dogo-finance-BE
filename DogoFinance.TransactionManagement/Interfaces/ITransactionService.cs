using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.BusinessLogic.Layer.Response;

namespace DogoFinance.TransactionManagement.Interfaces
{
    public interface ITransactionService
    {
        Task<ApiResponse> InitiateDeposit(long customerId, decimal amount);
        Task<ApiResponse> ChargeCard(MonnifyChargeRequest request);
        Task<ApiResponse> AuthorizeDeposit(MonnifyAuthorizeRequest request);
        Task<ApiResponse> ConfirmDeposit(string reference);
        Task<ApiResponse> CreateVirtualAccount(long userId);
        Task<ApiResponse> HandleMonnifyWebhook(string payload, string signature);
        Task<ApiResponse> InitiateWithdrawal(WithdrawalRequest request);
        Task<ApiResponse> GetTransactionHistory(long userId);
        Task<ApiResponse> GetWallet(long customerId);
        Task<ApiResponse> GetFinanceSummary();
    }
}
