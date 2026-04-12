using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.DataAccess.Layer.DTO;
using DogoFinance.ProductManagement.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DogoFinance.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PortfolioController : ControllerBase
    {
        private readonly IAssetClassService _assetClassService;
        private readonly IPortfolioTypeService _portfolioTypeService;
        private readonly IPortfolioService _portfolioService;
        private readonly IInstrumentService _instrumentService;
        private readonly IPortfolioInstrumentService _portfolioInstrumentService;
        private readonly IPortfolioAllocationRuleService _allocationRuleService;
        private readonly IInstrumentPriceService _priceService;

        public PortfolioController(
            IAssetClassService assetClassService,
            IPortfolioTypeService portfolioTypeService,
            IPortfolioService portfolioService,
            IInstrumentService instrumentService,
            IPortfolioInstrumentService portfolioInstrumentService,
            IPortfolioAllocationRuleService allocationRuleService,
            IInstrumentPriceService priceService)
        {
            _assetClassService = assetClassService;
            _portfolioTypeService = portfolioTypeService;
            _portfolioService = portfolioService;
            _instrumentService = instrumentService;
            _portfolioInstrumentService = portfolioInstrumentService;
            _allocationRuleService = allocationRuleService;
            _priceService = priceService;
        }

        // --- Asset Class ---
        [HttpGet("asset-classes")]
        public async Task<ActionResult<ApiResponse>> GetAssetClasses() => Ok(await _assetClassService.GetAssetClasses());

        [HttpPost("asset-classes")]
        public async Task<ActionResult<ApiResponse>> SaveAssetClass(AssetClassDto model) => Ok(await _assetClassService.SaveAssetClass(model));

        [HttpDelete("asset-classes/{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteAssetClass(int id) => Ok(await _assetClassService.DeleteAssetClass(id));

        // --- Portfolio Type ---
        [HttpGet("types")]
        public async Task<ActionResult<ApiResponse>> GetPortfolioTypes() => Ok(await _portfolioTypeService.GetList());

        [HttpPost("types")]
        public async Task<ActionResult<ApiResponse>> SavePortfolioType(PortfolioTypeDto model) => Ok(await _portfolioTypeService.Save(model));

        [HttpDelete("types/{id}")]
        public async Task<ActionResult<ApiResponse>> DeletePortfolioType(int id) => Ok(await _portfolioTypeService.Delete(id));

        // --- Portfolio ---
        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetPortfolios() => Ok(await _portfolioService.GetList());

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> SavePortfolio(PortfolioDto model) => Ok(await _portfolioService.Save(model));

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse>> DeletePortfolio(int id) => Ok(await _portfolioService.Delete(id));

        // --- Instruments ---
        [HttpGet("instruments")]
        public async Task<ActionResult<ApiResponse>> GetInstruments() => Ok(await _instrumentService.GetList());

        [HttpPost("instruments")]
        public async Task<ActionResult<ApiResponse>> SaveInstrument(InstrumentDto model) => Ok(await _instrumentService.Save(model));

        [HttpDelete("instruments/{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteInstrument(int id) => Ok(await _instrumentService.Delete(id));
    }
}
