using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.DataAccess.Layer.Models.Entities;

namespace DogoFinance.AdminManagement.Interfaces
{
    public interface IAdminService
    {
        Task<ApiResponse> CreateAdmin(SignUpRequest request, int roleId);
        Task<ApiResponse> UpdateAdmin(long userId, SignUpRequest request, int roleId);
        Task<ApiResponse> GetAdmins();

        // Roles Management
        Task<ApiResponse> GetRoles();
        Task<ApiResponse> SaveRole(TblRole role);
        Task<ApiResponse> DeleteRole(int id);

        Task<ApiResponse> ListClients();
        Task<ApiResponse> GetAccessRightsHierarchy(int roleId);
        Task<ApiResponse> UpdateRoleAccessRights(int roleId, List<int> accessRightIds);

        // Address Verifications
        Task<ApiResponse> ListAddressVerifications(string? status);
        Task<ApiResponse> ReviewAddressVerification(AdminAddressReviewRequest request, long adminUserId);

        Task<ApiResponse> GetActivePortfolios();

        // System Settings
        Task<ApiResponse> GetSystemSettings();
        Task<ApiResponse> UpdateSystemSettings(TblSystemSetting settings);

        // Withdrawal Management
        Task<ApiResponse> ListWithdrawalRequests(string? status);
        Task<ApiResponse> ReviewWithdrawalRequest(AdminWithdrawalReviewRequest request, long adminUserId);

        // Liquidation Management
        Task<ApiResponse> ListLiquidationRequests(int? status);
        Task<ApiResponse> ReviewLiquidationRequest(AdminLiquidationReviewRequest request, long adminUserId);
    }

    public class AdminWithdrawalReviewRequest
    {
        public long RequestId { get; set; }
        public bool Approved { get; set; }
        public string? AdminNotes { get; set; }
    }

    public class AdminLiquidationReviewRequest
    {
        public long RequestId { get; set; }
        public bool Approved { get; set; }
        public string? AdminNotes { get; set; }
    }
}
