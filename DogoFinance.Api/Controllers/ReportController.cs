using DogoFinance.AccountingManagement.Interfaces;
using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.ReportManagement.Interfaces;
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
        private readonly ICustomerReportService _customerReportService;

        public ReportController(IAccountingService accountingService, ICustomerReportService customerReportService)
        {
            _accountingService = accountingService;
            _customerReportService = customerReportService;
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

        [HttpGet("client-onboarding")]
        public async Task<ActionResult<ApiResponse>> GetClientOnboardingReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (!IsStaff()) return StatusCode(403, new ApiResponse { Message = "Access denied: Administrative privileges required.", Status = 403 });
            var report = await _customerReportService.GetClientOnboardingReportAsync(startDate, endDate, pageNumber, pageSize);
            return Ok(new ApiResponse { Status = 200, Message = "Success", Data = report });
        }

        [HttpGet("client-activity")]
        public async Task<ActionResult<ApiResponse>> GetClientActivityReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (!IsStaff()) return StatusCode(403, new ApiResponse { Message = "Access denied: Administrative privileges required.", Status = 403 });
            var report = await _customerReportService.GetClientActivityReportAsync(startDate, endDate, pageNumber, pageSize);
            return Ok(new ApiResponse { Status = 200, Message = "Success", Data = report });
        }

        [HttpGet("client-portfolio")]
        public async Task<ActionResult<ApiResponse>> GetClientPortfolioReport([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (!IsStaff()) return StatusCode(403, new ApiResponse { Message = "Access denied: Administrative privileges required.", Status = 403 });
            var report = await _customerReportService.GetClientPortfolioReportAsync(pageNumber, pageSize);
            return Ok(new ApiResponse { Status = 200, Message = "Success", Data = report });
        }
    }
}
