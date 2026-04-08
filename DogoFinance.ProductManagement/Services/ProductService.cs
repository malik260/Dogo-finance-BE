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
    public class ProductService : DataRepository, IProductService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IUnitOfWork uow, ILogger<ProductService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<ApiResponse> GetProducts()
        {
            var response = new ApiResponse();
            try
            {
                var products = await _uow.Products.GetAllProducts();
                var productTypes = await _uow.Products.GetAllProductTypes();
                var assetTypes = await _uow.Products.GetAllAssetTypes();

                var productDtos = new List<ProductDto>();
                foreach (var p in products)
                {
                    var allocations = await _uow.Products.GetAllocationsByProductId(p.ProductId);
                    
                    productDtos.Add(new ProductDto
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        Code = p.Code,
                        ProductTypeId = p.ProductTypeId,
                        RiskLevel = p.RiskLevel,
                        Description = p.Description,
                        IsActive = p.IsActive,
                        MinTenorInDays = p.MinTenorInDays,
                        MaxTenorInDays = p.MaxTenorInDays,
                        ProductTypeName = productTypes.FirstOrDefault(pt => pt.ProductTypeId == p.ProductTypeId)?.Name,
                        Allocations = allocations.Select(a => new AssetAllocationDto
                        {
                            Id = a.Id,
                            ProductId = a.ProductId,
                            AssetTypeId = a.AssetTypeId,
                            TargetPercentage = a.TargetPercentage,
                            MinPercentage = a.MinPercentage,
                            MaxPercentage = a.MaxPercentage,
                            AssetTypeName = assetTypes.FirstOrDefault(at => at.AssetTypeId == a.AssetTypeId)?.Name
                        }).ToList()
                    });
                }

                response.SetMessage("Products retrieved successfully", true, productDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products");
                response.SetError("Failed to retrieve products", 500);
            }
            return response;
        }

        public async Task<ApiResponse> GetProductByCode(string code)
        {
            var response = new ApiResponse();
            try
            {
                var product = await _uow.Products.GetProductByCode(code);
                if (product == null)
                {
                    response.SetError("Product not found", 404);
                    return response;
                }

                var allocations = await _uow.Products.GetAllocationsByProductId(product.ProductId);
                var assetTypes = await _uow.Products.GetAllAssetTypes();

                var productDto = new ProductDto
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Code = product.Code,
                    ProductTypeId = product.ProductTypeId,
                    RiskLevel = product.RiskLevel,
                    Description = product.Description,
                    IsActive = product.IsActive,
                    MinTenorInDays = product.MinTenorInDays,
                    MaxTenorInDays = product.MaxTenorInDays,
                    Allocations = allocations.Select(a => new AssetAllocationDto
                    {
                        Id = a.Id,
                        ProductId = a.ProductId,
                        AssetTypeId = a.AssetTypeId,
                        TargetPercentage = a.TargetPercentage,
                        MinPercentage = a.MinPercentage,
                        MaxPercentage = a.MaxPercentage,
                        AssetTypeName = assetTypes.FirstOrDefault(at => at.AssetTypeId == a.AssetTypeId)?.Name
                    }).ToList()
                };

                response.SetMessage("Product retrieved successfully", true, productDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product by code {Code}", code);
                response.SetError("Failed to retrieve product", 500);
            }
            return response;
        }

        public async Task<ApiResponse> SaveProduct(ProductDto request)
        {
            var response = new ApiResponse();
            try
            {
                // Automatic code generation for Product
                if (string.IsNullOrEmpty(request.Code))
                {
                    var baseName = request.Name ?? "Product";
                    request.Code = string.Concat(baseName.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                  .Where(x => x.Length > 0)
                                  .Select(x => x[0])).ToUpper() + "-" + DateTime.Now.ToString("mmss");
                }

                var product = new TblProduct
                {
                    ProductId = request.ProductId,
                    Name = request.Name ?? "Unnamed Product",
                    Code = request.Code ?? "CODE",
                    ProductTypeId = request.ProductTypeId,
                    RiskLevel = request.RiskLevel,
                    Description = request.Description,
                    IsActive = request.IsActive,
                    MinTenorInDays = request.MinTenorInDays,
                    MaxTenorInDays = request.MaxTenorInDays
                };

                await _uow.Products.SaveProduct(product);

                // Save or update allocations if provided
                if (request.Allocations != null && request.Allocations.Any())
                {
                    foreach (var alloc in request.Allocations)
                    {
                        var entity = new TblAssetAllocation
                        {
                            Id = alloc.Id,
                            ProductId = product.ProductId,
                            AssetTypeId = alloc.AssetTypeId,
                            TargetPercentage = alloc.TargetPercentage,
                            MinPercentage = alloc.MinPercentage,
                            MaxPercentage = alloc.MaxPercentage
                        };
                        await _uow.Products.SaveAssetAllocation(entity);
                    }
                }

                response.SetMessage("Product saved successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving product");
                response.SetError("Failed to save product", 500);
            }
            return response;
        }

        public async Task<ApiResponse> DeleteProduct(int id)
        {
            var response = new ApiResponse();
            try
            {
                // Delete associated allocations first
                var allocations = await _uow.Products.GetAllocationsByProductId(id);
                foreach (var a in allocations) await _uow.Products.DeleteAssetAllocation(a.Id);

                await _uow.Products.DeleteProduct(id);
                response.SetMessage("Product deleted successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {Id}", id);
                response.SetError("Failed to delete product", 500);
            }
            return response;
        }
    }
}
