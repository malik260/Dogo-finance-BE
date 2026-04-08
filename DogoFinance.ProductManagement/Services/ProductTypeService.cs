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
    public class ProductTypeService : DataRepository, IProductTypeService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<ProductTypeService> _logger;

        public ProductTypeService(IUnitOfWork uow, ILogger<ProductTypeService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<ApiResponse> GetProductTypes()
        {
            var response = new ApiResponse();
            try
            {
                var types = await _uow.Products.GetAllProductTypes();
                var typeDtos = types.Select(t => new ProductTypeDto
                {
                    ProductTypeId = t.ProductTypeId,
                    Name = t.Name,
                    Code = t.Code,
                    SupportsAllocation = t.SupportsAllocation,
                    SupportsProfitSharing = t.SupportsProfitSharing
                });
                response.SetMessage("Product types retrieved", true, typeDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product types");
                response.SetError("Failed to retrieve product types", 500);
            }
            return response;
        }

        public async Task<ApiResponse> SaveProductType(ProductTypeDto request)
        {
            var response = new ApiResponse();
            try
            {
                if (string.IsNullOrEmpty(request.Code))
                {
                    var baseName = request.Name ?? "ProductType";
                    var initials = string.Concat(baseName.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                       .Where(x => x.Length > 0)
                                       .Select(x => x[0]))
                                       .ToUpper();
                    request.Code = initials;
                }

                var type = new TblProductType
                {
                    ProductTypeId = request.ProductTypeId,
                    Name = request.Name ?? "Unnamed Type",
                    Code = request.Code ?? "CODE",
                    SupportsAllocation = request.SupportsAllocation,
                    SupportsProfitSharing = request.SupportsProfitSharing
                };
                await _uow.Products.SaveProductType(type);
                response.SetMessage("Product type saved successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving product type");
                response.SetError("Failed to save product type", 500);
            }
            return response;
        }

        public async Task<ApiResponse> DeleteProductType(int id)
        {
            var response = new ApiResponse();
            try
            {
                // Check if any products exist for this type
                var allProducts = await _uow.Products.GetAllProducts();
                if (allProducts.Any(p => p.ProductTypeId == id))
                {
                    response.SetError("Cannot delete type because it has associated products", 400);
                    return response;
                }

                await _uow.Products.DeleteProductType(id);
                response.SetMessage("Product type deleted successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product type {Id}", id);
                response.SetError("Failed to delete product type", 500);
            }
            return response;
        }
    }
}
