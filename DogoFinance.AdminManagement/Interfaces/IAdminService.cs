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
    }
}
