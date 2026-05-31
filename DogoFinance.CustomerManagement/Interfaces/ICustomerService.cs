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
        Task<ApiResponse> GetProfile(long userId);
        Task<ApiResponse> UpdateProfile(long userId, UpdateProfileRequest request);
        Task<ApiResponse> GetGenders();
        Task<ApiResponse> GetCustomerTypes();
        Task<ApiResponse> GetAddressDocTypes();
        Task<ApiResponse> InitiateAddressVerification(long customerId, AddressVerificationRequest request);
        Task<ApiResponse> GetVerificationStatuses(long customerId);
        Task<ApiResponse> GetCompanyBankDetails();
        Task<ApiResponse> GetCorporateProfile(long userId);
        Task<ApiResponse> UpdateCorporateProfile(long userId, UpdateCorporateProfileRequest request);
        Task<ApiResponse> GetPrimaryContact(long userId);
        Task<ApiResponse> UpdatePrimaryContact(long userId, UpdateCorporateContactRequest request);
        Task<ApiResponse> GetCorporateVerifications(long userId);
        Task<ApiResponse> UploadCorporateDocument(long userId, UploadCorporateDocumentRequest request);
        Task<ApiResponse> AddCorporateSignatory(long userId, AddCorporateSignatoryRequest request);
        Task<ApiResponse> GetCorporateSignatories(long userId);
        Task<ApiResponse> DeleteCorporateSignatory(long userId, int signatoryId);
        Task<ApiResponse> AddCorporateDirector(long userId, AddCorporateDirectorRequest request);
        Task<ApiResponse> GetCorporateDirectors(long userId);
        Task<ApiResponse> DeleteCorporateDirector(long userId, int directorId);

        // Notifications
        Task<ApiResponse> GetNotifications(long userId);
        Task<ApiResponse> MarkNotificationRead(long notificationId, long userId);
    }
}
