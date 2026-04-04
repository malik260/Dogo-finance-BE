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

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

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
