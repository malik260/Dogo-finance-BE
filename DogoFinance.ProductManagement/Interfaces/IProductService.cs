using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.BusinessLogic.Layer.Response;

namespace DogoFinance.ProductManagement.Interfaces
{
    public interface IProductService
    {
        Task<ApiResponse> GetProducts();
        Task<ApiResponse> GetProductByCode(string code);
        Task<ApiResponse> SaveProduct(ProductDto request);
        Task<ApiResponse> DeleteProduct(int id);
    }
}
