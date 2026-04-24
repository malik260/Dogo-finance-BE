using DogoFinance.AccountingManagement.Interfaces;
using DogoFinance.BusinessLogic.Layer.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DogoFinance.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class ReportController : ControllerBase
    {
        private readonly IAccountingService _accountingService;

        public ReportController(IAccountingService accountingService)
        {
            _accountingService = accountingService;
        }

        [HttpGet("trial-balance")]
        public async Task<ActionResult<ApiResponse>> GetTrialBalance()
        {
            return Ok(await _accountingService.GetTrialBalanceAsync());
        }

        [HttpGet("chart-of-accounts")]
        public async Task<ActionResult<ApiResponse>> GetChartOfAccounts()
        {
            return Ok(await _accountingService.GetChartOfAccountsAsync());
        }

        [HttpPost("seed-accounts")]
        public async Task<ActionResult<ApiResponse>> SeedAccounts()
        {
            return Ok(await _accountingService.SeedChartOfAccountsAsync());
        }
    }
}
