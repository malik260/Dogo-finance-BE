using DogoFinance.BusinessLogic.Layer.Response;
using System.Threading.Tasks;

namespace DogoFinance.ProductManagement.Interfaces
{
    public interface ISystemSettingService
    {
        Task<ApiResponse> GetSessionTimeout();
    }
}
