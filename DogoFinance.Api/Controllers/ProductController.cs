using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.ProductManagement.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DogoFinance.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IProductTypeService _productTypeService;
        private readonly IAssetTypeService _assetTypeService;
        private readonly IAssetAllocationService _assetAllocationService;

        public ProductController(
            IProductService productService,
            IProductTypeService productTypeService,
            IAssetTypeService assetTypeService,
            IAssetAllocationService assetAllocationService)
        {
            _productService = productService;
            _productTypeService = productTypeService;
            _assetTypeService = assetTypeService;
            _assetAllocationService = assetAllocationService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetProducts()
        {
            var result = await _productService.GetProducts();
            return Ok(result);
        }

        [HttpGet("{code}")]
        public async Task<ActionResult<ApiResponse>> GetProductByCode(string code)
        {
            var result = await _productService.GetProductByCode(code);
            return Ok(result);
        }

        [HttpGet("types")]
        public async Task<ActionResult<ApiResponse>> GetProductTypes()
        {
            var result = await _productTypeService.GetProductTypes();
            return Ok(result);
        }

        [HttpGet("asset-types")]
        public async Task<ActionResult<ApiResponse>> GetAssetTypes()
        {
            var result = await _assetTypeService.GetAssetTypes();
            return Ok(result);
        }

        [HttpGet("{productId}/allocations")]
        public async Task<ActionResult<ApiResponse>> GetAllocations(int productId)
        {
            var result = await _assetAllocationService.GetAllocationsByProductId(productId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> SaveProduct([FromBody] ProductDto request)
        {
            var result = await _productService.SaveProduct(request);
            return Ok(result);
        }

        [HttpPost("types")]
        public async Task<ActionResult<ApiResponse>> SaveProductType([FromBody] ProductTypeDto request)
        {
            var result = await _productTypeService.SaveProductType(request);
            return Ok(result);
        }

        [HttpDelete("types/{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteProductType(int id)
        {
            var result = await _productTypeService.DeleteProductType(id);
            return Ok(result);
        }

        [HttpPost("asset-types")]
        public async Task<ActionResult<ApiResponse>> SaveAssetType([FromBody] AssetTypeDto request)
        {
            var result = await _assetTypeService.SaveAssetType(request);
            return Ok(result);
        }

        [HttpDelete("asset-types/{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteAssetType(int id)
        {
            var result = await _assetTypeService.DeleteAssetType(id);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProduct(id);
            return Ok(result);
        }

        [HttpPost("allocations")]
        public async Task<ActionResult<ApiResponse>> SaveAllocation([FromBody] AssetAllocationDto request)
        {
            var result = await _assetAllocationService.SaveAssetAllocation(request);
            return Ok(result);
        }
    }
}
