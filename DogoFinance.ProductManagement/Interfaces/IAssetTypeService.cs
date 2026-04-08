using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.BusinessLogic.Layer.Response;

namespace DogoFinance.ProductManagement.Interfaces
{
    public interface IAssetTypeService
    {
        Task<ApiResponse> GetAssetTypes();
        Task<ApiResponse> SaveAssetType(AssetTypeDto request);
        Task<ApiResponse> DeleteAssetType(int id);
    }
}
