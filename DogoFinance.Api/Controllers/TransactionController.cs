using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.TransactionManagement.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DogoFinance.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ICustomerPortfolioService _portfolioService;
        private readonly ICustomerHoldingService _holdingService;

        public TransactionController(
            ITransactionService transactionService,
            ICustomerPortfolioService portfolioService,
            ICustomerHoldingService holdingService)
        {
            _transactionService = transactionService;
            _portfolioService = portfolioService;
            _holdingService = holdingService;
        }

        // --- Customer Portfolios ---
        [HttpGet("portfolios/{customerId}")]
        public async Task<ActionResult<ApiResponse>> GetCustomerPortfolios(long customerId) => Ok(await _portfolioService.GetByCustomer(customerId));

        [HttpPost("portfolios")]
        public async Task<ActionResult<ApiResponse>> SaveCustomerPortfolio(CustomerPortfolioDto model) => Ok(await _portfolioService.Save(model));

        [HttpDelete("portfolios/{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteCustomerPortfolio(long id) => Ok(await _portfolioService.Delete(id));

        // --- Customer Holdings ---
        [HttpGet("holdings/{customerId}")]
        public async Task<ActionResult<ApiResponse>> GetCustomerHoldings(long customerId) => Ok(await _holdingService.GetByCustomer(customerId));

        [HttpPost("holdings")]
        public async Task<ActionResult<ApiResponse>> SaveCustomerHolding(CustomerHoldingDto model) => Ok(await _holdingService.Save(model));

        [HttpDelete("holdings/{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteCustomerHolding(long id) => Ok(await _holdingService.Delete(id));

        [HttpPost("deposit/initiate")]
        public async Task<ActionResult<ApiResponse>> InitiateDeposit([FromBody] InitiateDepositRequest request)
        {
            var response = await _transactionService.InitiateDeposit(request.CustomerId, request.Amount);
            if (response.Boolean) return Ok(response);
            return StatusCode(response.Status, response);
        }

        [HttpPost("deposit/confirm/{reference}")]
        public async Task<ActionResult<ApiResponse>> ConfirmDeposit(string reference)
        {
            var response = await _transactionService.ConfirmDeposit(reference);
            if (response.Boolean) return Ok(response);
            return StatusCode(response.Status, response);
        }

        [HttpPost("withdraw")]
        public async Task<ActionResult<ApiResponse>> InitiateWithdrawal([FromBody] WithdrawalRequest request)
        {
            var response = await _transactionService.InitiateWithdrawal(request);
            if (response.Boolean) return Ok(response);
            return StatusCode(response.Status, response);
        }

        [HttpGet("history")]
        public async Task<ActionResult<ApiResponse>> GetHistory()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ApiResponse { Message = "Not logged in", Status = 401 });

            var response = await _transactionService.GetTransactionHistory(long.Parse(userIdStr));
            return Ok(response);
        }

        [HttpGet("wallet/{customerId}")]
        public async Task<ActionResult<ApiResponse>> GetWallet(long customerId)
        {
            var response = await _transactionService.GetWallet(customerId);
            if (response.Boolean) return Ok(response);
            return StatusCode(response.Status, response);
        }
    }

    public class InitiateDepositRequest
    {
        public long CustomerId { get; set; }
        public decimal Amount { get; set; }
    }
}
