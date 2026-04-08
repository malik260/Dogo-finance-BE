using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.DataAccess.Layer.Interfaces;
using DogoFinance.DataAccess.Layer.Models.Entities;
using DogoFinance.DataAccess.Layer.Repositories.Base;
using DogoFinance.ProductManagement.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogoFinance.ProductManagement.Services
{
    public class AssetAllocationService : DataRepository, IAssetAllocationService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<AssetAllocationService> _logger;

        public AssetAllocationService(IUnitOfWork uow, ILogger<AssetAllocationService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<ApiResponse> GetAllocationsByProductId(int productId)
        {
            var response = new ApiResponse();
            try
            {
                var allocations = await _uow.Products.GetAllocationsByProductId(productId);
                var assetTypes = await _uow.Products.GetAllAssetTypes();

                var allocationDtos = allocations.Select(a => new AssetAllocationDto
                {
                    Id = a.Id,
                    ProductId = a.ProductId,
                    AssetTypeId = a.AssetTypeId,
                    TargetPercentage = a.TargetPercentage,
                    MinPercentage = a.MinPercentage,
                    MaxPercentage = a.MaxPercentage,
                    AssetTypeName = assetTypes.FirstOrDefault(at => at.AssetTypeId == a.AssetTypeId)?.Name
                });
                response.SetMessage("Allocations retrieved", true, allocationDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving allocations for product {ProductId}", productId);
                response.SetError("Failed to retrieve allocations", 500);
            }
            return response;
        }

        public async Task<ApiResponse> SaveAssetAllocation(AssetAllocationDto request)
        {
            var response = new ApiResponse();
            try
            {
                var allocation = new TblAssetAllocation
                {
                    Id = request.Id,
                    ProductId = request.ProductId,
                    AssetTypeId = request.AssetTypeId,
                    TargetPercentage = request.TargetPercentage,
                    MinPercentage = request.MinPercentage,
                    MaxPercentage = request.MaxPercentage
                };
                await _uow.Products.SaveAssetAllocation(allocation);
                response.SetMessage("Asset allocation saved successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving asset allocation");
                response.SetError("Failed to save asset allocation", 500);
            }
            return response;
        }
    }
}
