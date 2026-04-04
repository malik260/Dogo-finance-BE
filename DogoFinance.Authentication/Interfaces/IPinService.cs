using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.BusinessLogic.Layer.Response;

namespace DogoFinance.Authentication.Interfaces
{
    public interface IPinService
    {
        Task<ApiResponse> SetTransactionPin(long userId, SetPinRequest request);
        Task<ApiResponse> ChangeTransactionPin(long userId, ChangePinRequest request);
        Task<ApiResponse> ForgotPin(ForgotPinRequest request);
        Task<ApiResponse> ResetPin(ResetPinRequest request);
        Task<ApiResponse> Toggle2fa(long userId, bool status);
    }
}
