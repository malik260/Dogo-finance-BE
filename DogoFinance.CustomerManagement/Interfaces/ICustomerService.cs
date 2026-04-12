using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.BusinessLogic.Layer.Response;

namespace DogoFinance.CustomerManagement.Interfaces
{
    public interface ICustomerService
    {
        Task<ApiResponse> SignUp(SignUpRequest request);
        Task<ApiResponse> VerifyEmail(VerifyEmailRequest request);
        Task<ApiResponse> ResendVerificationCode(string email);
        Task<ApiResponse> GetTodoList(long customerId);
        Task<ApiResponse> VerifyBvn(long customerId, BvnVerificationRequest request);
        Task<ApiResponse> VerifyNin(long customerId, NinVerificationRequest request);
    }
}
