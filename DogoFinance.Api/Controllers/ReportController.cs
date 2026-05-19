using DogoFinance.AccountingManagement.Interfaces;
using DogoFinance.BusinessLogic.Layer.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DogoFinance.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IAccountingService _accountingService;

        public ReportController(IAccountingService accountingService)
        {
            _accountingService = accountingService;
        }

        private bool IsStaff()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            return !string.IsNullOrEmpty(role) && 
                   !role.Equals("Customer", StringComparison.OrdinalIgnoreCase) && 
                   !role.Equals("User", StringComparison.OrdinalIgnoreCase);
        }

        [HttpGet("trial-balance")]
        public async Task<ActionResult<ApiResponse>> GetTrialBalance()
        {
            if (!IsStaff()) return StatusCode(403, new ApiResponse { Message = "Access denied: Administrative privileges required.", Status = 403 });
            return Ok(await _accountingService.GetTrialBalanceAsync());
        }

        [HttpGet("chart-of-accounts")]
        public async Task<ActionResult<ApiResponse>> GetChartOfAccounts()
        {
            if (!IsStaff()) return StatusCode(403, new ApiResponse { Message = "Access denied: Administrative privileges required.", Status = 403 });
            return Ok(await _accountingService.GetChartOfAccountsAsync());
        }

        [HttpPost("seed-accounts")]
        public async Task<ActionResult<ApiResponse>> SeedAccounts()
        {
            if (!IsStaff()) return StatusCode(403, new ApiResponse { Message = "Access denied: Administrative privileges required.", Status = 403 });
            return Ok(await _accountingService.SeedChartOfAccountsAsync());
        }
    }
}
