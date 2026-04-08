using DogoFinance.DataAccess.Layer.Models.Entities;

namespace DogoFinance.DataAccess.Layer.Interfaces.Repositories
{
    public interface IProductRepository
    {
        // Products
        Task<IEnumerable<TblProduct>> GetAllProducts();
        Task<TblProduct?> GetProductById(int productId);
        Task<TblProduct?> GetProductByCode(string code);
        Task SaveProduct(TblProduct product);
        Task DeleteProduct(int productId);

        // Product Types
        Task<IEnumerable<TblProductType>> GetAllProductTypes();
        Task<TblProductType?> GetProductTypeById(int productTypeId);
        Task SaveProductType(TblProductType productType);
        Task DeleteProductType(int productTypeId);

        // Asset Types
        Task<IEnumerable<TblAssetType>> GetAllAssetTypes();
        Task<TblAssetType?> GetAssetTypeById(int assetTypeId);
        Task SaveAssetType(TblAssetType assetType);
        Task DeleteAssetType(int assetTypeId);

        // Asset Allocations
        Task<IEnumerable<TblAssetAllocation>> GetAllocationsByProductId(int productId);
        Task SaveAssetAllocation(TblAssetAllocation allocation);
        Task DeleteAssetAllocation(int id);
    }
}
