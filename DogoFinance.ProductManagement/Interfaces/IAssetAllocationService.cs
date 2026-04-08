using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.BusinessLogic.Layer.Response;

namespace DogoFinance.ProductManagement.Interfaces
{
    public interface IAssetAllocationService
    {
        Task<ApiResponse> GetAllocationsByProductId(int productId);
        Task<ApiResponse> SaveAssetAllocation(AssetAllocationDto request);
    }
}
