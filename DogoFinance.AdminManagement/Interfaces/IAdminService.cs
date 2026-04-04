using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.BusinessLogic.Layer.Response;

namespace DogoFinance.AdminManagement.Interfaces
{
    public interface IAdminService
    {
        Task<ApiResponse> CreateAdmin(SignUpRequest request, int roleId);
        Task<ApiResponse> GetAdmins();
    }
}
