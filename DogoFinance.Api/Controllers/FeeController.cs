using System.Threading.Tasks;
using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.ProductManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DogoFinance.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class FeeController : ControllerBase
    {
        private readonly IFeeComputationService _feeService;

        public FeeController(IFeeComputationService feeService)
        {
            _feeService = feeService;
        }

        /// <summary>
        /// GET api/fee/product/{portfolioId}
        /// Returns all configured fee rules for a specific product/portfolio.
        /// </summary>
        [HttpGet("product/{portfolioId}")]
        public async Task<IActionResult> GetProductFeeConfigs(int portfolioId)
        {
            var response = await _feeService.GetProductFeeConfigsAsync(portfolioId);
            return StatusCode(response.Status, response);
        }

        /// <summary>
        /// POST api/fee/product
        /// Adds or updates a fee rule for a product.
        /// </summary>
        [HttpPost("product")]
        public async Task<IActionResult> SaveProductFeeConfig([FromBody] PortfolioFeeConfigDto dto)
        {
            var response = await _feeService.SaveProductFeeConfigAsync(dto);
            return StatusCode(response.Status, response);
        }

        /// <summary>
        /// PUT api/fee/config/{id}/toggle-waive?isWaived=true
        /// Toggles fee waiver for a specific product fee rule.
        /// </summary>
        [HttpPut("config/{id}/toggle-waive")]
        public async Task<IActionResult> ToggleWaiveFeeConfig(int id, [FromQuery] bool isWaived)
        {
            var response = await _feeService.ToggleWaiveFeeConfigAsync(id, isWaived);
            return StatusCode(response.Status, response);
        }

        /// <summary>
        /// POST api/fee/seed-defaults
        /// Seeds standard 1.5% Mgmt, 0.25% Custody, 0.25% SEC fee rules for active portfolios.
        /// </summary>
        [HttpPost("seed-defaults")]
        public async Task<IActionResult> SeedDefaultFeeConfigs()
        {
            var response = await _feeService.SeedDefaultFeeConfigsAsync();
            return StatusCode(response.Status, response);
        }

        /// <summary>
        /// GET api/fee/preview-quarterly?year=2026&quarter=1&portfolioId=2
        /// Previews computed fees for a quarter before execution.
        /// </summary>
        [HttpGet("preview-quarterly")]
        public async Task<IActionResult> PreviewQuarterlyFees([FromQuery] int year, [FromQuery] int quarter, [FromQuery] int? portfolioId = null)
        {
            var response = await _feeService.PreviewQuarterlyFeesAsync(year, quarter, portfolioId);
            return StatusCode(response.Status, response);
        }

        /// <summary>
        /// POST api/fee/execute-quarterly?year=2026&quarter=1&portfolioId=2
        /// Triggers fee deductions & double-entry GL journal postings. Idempotent.
        /// </summary>
        [HttpPost("execute-quarterly")]
        public async Task<IActionResult> ExecuteQuarterlyFees([FromQuery] int year, [FromQuery] int quarter, [FromQuery] int? portfolioId = null)
        {
            var response = await _feeService.ExecuteQuarterlyFeeDeductionsAsync(year, quarter, portfolioId);
            return StatusCode(response.Status, response);
        }

        /// <summary>
        /// GET api/fee/sec-remittance-report?year=2026&quarter=1
        /// Generates SEC regulatory fee collection & remittance report.
        /// </summary>
        [HttpGet("sec-remittance-report")]
        public async Task<IActionResult> GetSecRemittanceReport([FromQuery] int year, [FromQuery] int quarter)
        {
            var response = await _feeService.GetSecRemittanceReportAsync(year, quarter);
            return StatusCode(response.Status, response);
        }
    }
}
