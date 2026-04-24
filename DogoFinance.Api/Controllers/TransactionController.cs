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
        private readonly ITemporaryInvestmentService _tempInvestmentService;

        public TransactionController(
            ITransactionService transactionService,
            ICustomerPortfolioService portfolioService,
            ICustomerHoldingService holdingService,
            ICustomerInvestmentService investmentService,
            ITemporaryInvestmentService tempInvestmentService)
        {
            _transactionService = transactionService;
            _portfolioService = portfolioService;
            _holdingService = holdingService;
            _investmentService = investmentService;
            _tempInvestmentService = tempInvestmentService;
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

            return Ok(await _tempInvestmentService.ProcessSell(long.Parse(userIdStr), model.PortfolioId, model.Amount, model.Pin, model.Otp));
        }

        [HttpGet("portfolioSummary")]
        public async Task<ActionResult<ApiResponse>> GetSummary()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ApiResponse { Message = "Not logged in", Status = 401 });

            return Ok(await _investmentService.GetPortfolioSummary(long.Parse(userIdStr)));
        }

        // --- TEMPORARY NAV-BASED INVESTMENT LOGIC ---
        [HttpPost("temp-invest")]
        public async Task<ActionResult<ApiResponse>> TempInvest([FromBody] TempInvestRequest model)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            return Ok(await _tempInvestmentService.ProcessTempInvestment(long.Parse(userIdStr), model.PortfolioId, model.Amount, model.Pin, model.Otp));
        }

        [HttpGet("temp-stats/{portfolioId}")]
        public async Task<ActionResult<ApiResponse>> GetTempStats(int portfolioId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            return Ok(await _tempInvestmentService.GetTempPortfolioStats(long.Parse(userIdStr), portfolioId));
        }

        [HttpPost("temp-simulate-growth")]
        public async Task<ActionResult<ApiResponse>> SimulateGrowth([FromBody] TempSimulateRequest model)
        {
            return Ok(await _tempInvestmentService.SimulateNAVGrowth(model.PortfolioId, model.Days));
        }

        public class TempInvestRequest { public int PortfolioId { get; set; } public decimal Amount { get; set; } public string? Pin { get; set; } public string? Otp { get; set; } }
        public class TempSimulateRequest { public int PortfolioId { get; set; } public int Days { get; set; } }

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

        [HttpGet("portfolio-summary")]
        public async Task<ActionResult<ApiResponse>> GetPortfolioSummary()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ApiResponse { Message = "Not logged in", Status = 401 });
            
            return Ok(await _investmentService.GetPortfolioSummary(long.Parse(userIdStr)));
        }

        [HttpGet("finance-summary")]
        public async Task<ActionResult<ApiResponse>> GetFinanceSummary()
        {
            return Ok(await _transactionService.GetFinanceSummary());
        }

        [HttpGet("active-investments/{customerId}")]
        public async Task<ActionResult<ApiResponse>> GetActiveInvestments(long customerId)
        {
            return Ok(await _tempInvestmentService.GetActiveInvestments(customerId));
        }

        [HttpPost("simulate-nav")]
        public async Task<ActionResult<ApiResponse>> SimulateNav([FromBody] SimulateNavRequest request)
        {
            return Ok(await _tempInvestmentService.SimulateNAVGrowth(request.PortfolioId, request.Days));
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

        [HttpPost("validate-withdrawal-otp")]
        public async Task<ActionResult<ApiResponse>> ValidateWithdrawalOtp([FromBody] ValidateWithdrawalOtpRequest request)
        {
            return Ok(await _transactionService.ValidateWithdrawalOtp(request.CustomerId, request.Otp));
        }

        public class ValidateWithdrawalOtpRequest
        {
            public long CustomerId { get; set; }
            public string Otp { get; set; } = null!;
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
