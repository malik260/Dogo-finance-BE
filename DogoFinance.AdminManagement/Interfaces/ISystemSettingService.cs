using DogoFinance.BusinessLogic.Layer.Response;
using System.Threading.Tasks;

namespace DogoFinance.AdminManagement.Interfaces
{
    public interface ISystemSettingService
    {
        Task<ApiResponse> GetSessionTimeout();
    }
}
