using DogoFinance.Integration.Models.YouVerify;

namespace DogoFinance.Integration.Interfaces
{
    public interface IYouVerifyService
    {
        Task<YouVerifyResponse<IdentityVerificationData>?> VerifyBvn(BvnVerificationRequest request);
        Task<YouVerifyResponse<IdentityVerificationData>?> VerifyNin(NinVerificationRequest request);
    }
}
