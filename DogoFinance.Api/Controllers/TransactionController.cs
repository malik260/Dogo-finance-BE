using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.TransactionManagement.Interfaces;
using DogoFinance.DataAccess.Layer.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;

namespace DogoFinance.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ICustomerPortfolioService _portfolioService;
        private readonly ICustomerHoldingService _holdingService;
        private readonly ICustomerInvestmentService _investmentService;

        public TransactionController(
            ITransactionService transactionService,
            ICustomerPortfolioService portfolioService,
            ICustomerHoldingService holdingService,
            ICustomerInvestmentService investmentService)
        {
            _transactionService = transactionService;
            _portfolioService = portfolioService;
            _holdingService = holdingService;
            _investmentService = investmentService;
        }

        [HttpPost("invest")]
        public async Task<ActionResult<ApiResponse>> Invest([FromBody] InvestRequestDto model)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ApiResponse { Message = "Not logged in", Status = 401 });

            return Ok(await _investmentService.InvestAsync(model, long.Parse(userIdStr)));
        }

        [HttpPost("sell")]
        public async Task<ActionResult<ApiResponse>> Sell([FromBody] SellRequestDto model)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ApiResponse { Message = "Not logged in", Status = 401 });

            return Ok(await _investmentService.SellAsync(model, long.Parse(userIdStr)));
        }

        [HttpGet("portfolioSummary")]
        public async Task<ActionResult<ApiResponse>> GetSummary()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ApiResponse { Message = "Not logged in", Status = 401 });

            return Ok(await _investmentService.GetPortfolioSummary(long.Parse(userIdStr)));
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

        [HttpPost("deposit/charge")]
        public async Task<ActionResult<ApiResponse>> ChargeCard([FromBody] MonnifyChargeRequest request)
        {
            var response = await _transactionService.ChargeCard(request);
            if (response.Boolean) return Ok(response);
            return StatusCode(response.Status, response);
        }

        [HttpPost("deposit/authorize")]
        public async Task<ActionResult<ApiResponse>> AuthorizeDeposit([FromBody] MonnifyAuthorizeRequest request)
        {
            var response = await _transactionService.AuthorizeDeposit(request);
            if (response.Boolean) return Ok(response);
            return StatusCode(response.Status, response);
        }

        [HttpPost("deposit/confirm/{reference}")]
        public async Task<IActionResult> ConfirmDeposit(string reference)
        {
            var response = await _transactionService.ConfirmDeposit(reference);
            return StatusCode(response.Status, response);
        }

        [AllowAnonymous]
        [HttpPost("webhook/monnify")]
        public async Task<IActionResult> MonnifyWebhook()
        {
            var signature = Request.Headers["X-Monnify-Signature"].ToString();
            using var reader = new StreamReader(Request.Body);
            var payload = await reader.ReadToEndAsync();

            var response = await _transactionService.HandleMonnifyWebhook(payload, signature);
            return StatusCode(response.Status, response);
        }

        [HttpGet("deposit/virtual-account")]
        public async Task<IActionResult> GetVirtualAccount()
        {
            Console.WriteLine("--- GetVirtualAccount Endpoint Hit ---");
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ApiResponse { Message = "Not logged in", Status = 401 });
            
            var response = await _transactionService.CreateVirtualAccount(long.Parse(userIdStr));
            return StatusCode(response.Status, response);
        }

        [HttpPost("withdraw")]
        public async Task<ActionResult<ApiResponse>> InitiateWithdrawal([FromBody] WithdrawalRequest request)
        {
            var response = await _transactionService.InitiateWithdrawal(request);
            if (response.Boolean) return Ok(response);
            return StatusCode(response.Status, response);
        }

        [HttpPost("send-withdrawal-otp")]
        public async Task<ActionResult<ApiResponse>> SendWithdrawalOtp([FromBody] InitiateWithdrawalOtpRequest request)
        {
            return Ok(await _transactionService.SendWithdrawalOtp(request.CustomerId, request.Amount));
        }

        public class InitiateWithdrawalOtpRequest
        {
            public long CustomerId { get; set; }
            public decimal Amount { get; set; }
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
