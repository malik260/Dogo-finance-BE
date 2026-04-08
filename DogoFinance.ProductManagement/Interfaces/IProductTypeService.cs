using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.BusinessLogic.Layer.Response;

namespace DogoFinance.ProductManagement.Interfaces
{
    public interface IProductTypeService
    {
        Task<ApiResponse> GetProductTypes();
        Task<ApiResponse> SaveProductType(ProductTypeDto request);
        Task<ApiResponse> DeleteProductType(int id);
    }
}
