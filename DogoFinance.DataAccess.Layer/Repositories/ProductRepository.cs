using DogoFinance.DataAccess.Layer.Interfaces.Repositories;
using DogoFinance.DataAccess.Layer.Models.Entities;
using DogoFinance.DataAccess.Layer.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DogoFinance.DataAccess.Layer.Repositories
{
    public class ProductRepository : DataRepository, IProductRepository
    {
        // Products
        public async Task<IEnumerable<TblProduct>> GetAllProducts()
            => await BaseRepository().FindList<TblProduct>(n => n.IsActive);

        public async Task<TblProduct?> GetProductById(int productId)
            => await BaseRepository().FindEntity<TblProduct>(productId);

        public async Task<TblProduct?> GetProductByCode(string code)
            => await BaseRepository().FindEntity<TblProduct>(n => n.Code == code);

        public async Task SaveProduct(TblProduct product)
        {
            if (product.ProductId == 0)
            {
                product.CreatedAt = DateTime.UtcNow;
                await BaseRepository().Insert(product);
            }
            else
            {
                await BaseRepository().Update(product);
            }
        }

        public async Task DeleteProduct(int productId)
        {
            var product = await GetProductById(productId);
            if (product != null) await BaseRepository().Delete(product);
        }

        // Product Types
        public async Task<IEnumerable<TblProductType>> GetAllProductTypes()
            => await BaseRepository().FindList<TblProductType>();

        public async Task<TblProductType?> GetProductTypeById(int productTypeId)
            => await BaseRepository().FindEntity<TblProductType>(productTypeId);

        public async Task SaveProductType(TblProductType productType)
        {
            if (productType.ProductTypeId == 0)
            {
                productType.CreatedAt = DateTime.UtcNow;
                await BaseRepository().Insert(productType);
            }
            else
            {
                await BaseRepository().Update(productType);
            }
        }

        public async Task DeleteProductType(int productTypeId)
        {
            var type = await GetProductTypeById(productTypeId);
            if (type != null) await BaseRepository().Delete(type);
        }

        // Asset Types
        public async Task<IEnumerable<TblAssetType>> GetAllAssetTypes()
            => await BaseRepository().FindList<TblAssetType>();

        public async Task<TblAssetType?> GetAssetTypeById(int assetTypeId)
            => await BaseRepository().FindEntity<TblAssetType>(assetTypeId);

        public async Task SaveAssetType(TblAssetType assetType)
        {
            if (assetType.AssetTypeId == 0)
            {
                assetType.CreatedAt = DateTime.UtcNow;
                await BaseRepository().Insert(assetType);
            }
            else
            {
                await BaseRepository().Update(assetType);
            }
        }

        public async Task DeleteAssetType(int assetTypeId)
        {
            var assetType = await GetAssetTypeById(assetTypeId);
            if (assetType != null) await BaseRepository().Delete(assetType);
        }

        // Asset Allocations
        public async Task<IEnumerable<TblAssetAllocation>> GetAllocationsByProductId(int productId)
            => await BaseRepository().FindList<TblAssetAllocation>(n => n.ProductId == productId);

        public async Task SaveAssetAllocation(TblAssetAllocation allocation)
        {
            if (allocation.Id == 0)
            {
                await BaseRepository().Insert(allocation);
            }
            else
            {
                await BaseRepository().Update(allocation);
            }
        }

        public async Task DeleteAssetAllocation(int id)
        {
            var allocation = await BaseRepository().FindEntity<TblAssetAllocation>(id);
            if (allocation != null) await BaseRepository().Delete(allocation);
        }
    }
}
