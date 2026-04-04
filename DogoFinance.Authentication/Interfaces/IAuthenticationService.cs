using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.BusinessLogic.Layer.Response;

namespace DogoFinance.Authentication.Interfaces
{
    public interface IAuthenticationService
    {
        Task<ApiResponse> Login(LoginRequest request);
        Task<ApiResponse> ChangePassword(long userId, ChangePasswordRequest request);
        Task<ApiResponse> ForgotPassword(ForgotPasswordRequest request);
        Task<ApiResponse> ResetPassword(ResetPasswordRequest request);
        Task<ApiResponse> Logout(long userId);
        Task<ApiResponse> RefreshToken(string accessToken, string refreshToken);
        Task<ApiResponse> GetActiveSessions(long userId);
        Task<ApiResponse> RevokeSession(long sessionId, long userId);
    }
}
