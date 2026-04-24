using DogoFinance.AdminManagement.Interfaces;
using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.BusinessLogic.Layer.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DogoFinance.DataAccess.Layer.Models.Entities;
using System.Security.Claims;

namespace DogoFinance.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost("create")]
        public async Task<ActionResult<ApiResponse>> CreateAdmin([FromBody] CreateAdminRequest request)
        {
            // For simplicity, SignUpRequest is reused for data, but nested in a context
            var response = await _adminService.CreateAdmin(request.UserData, request.RoleId);
            if (response.Boolean) return Ok(response);
            return StatusCode(response.Status, response);
        }

        [HttpPut("update/{userId}")]
        public async Task<ActionResult<ApiResponse>> UpdateAdmin(long userId, [FromBody] CreateAdminRequest request)
        {
            var response = await _adminService.UpdateAdmin(userId, request.UserData, request.RoleId);
            if (response.Boolean) return Ok(response);
            return StatusCode(response.Status, response);
        }

        [HttpGet("list")]
        public async Task<ActionResult<ApiResponse>> ListAdmins()
        {
            var response = await _adminService.GetAdmins();
            return Ok(response);
        }

        [HttpGet("clients")]
        public async Task<ActionResult<ApiResponse>> ListClients()
        {
            var response = await _adminService.ListClients();
            return Ok(response);
        }

        // --- ROLES MANAGEMENT ---
        [HttpGet("roles")]
        public async Task<ActionResult<ApiResponse>> ListRoles()
        {
            var response = await _adminService.GetRoles();
            return Ok(response);
        }

        [HttpPost("roles")]
        public async Task<ActionResult<ApiResponse>> SaveRole([FromBody] TblRole role)
        {
            var response = await _adminService.SaveRole(role);
            return Ok(response);
        }

        [HttpDelete("roles/{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteRole(int id)
        {
            var response = await _adminService.DeleteRole(id);
            return Ok(response);
        }

        // --- ACCESS RIGHTS ---
        [HttpGet("roles/{roleId}/access-rights")]
        public async Task<ActionResult<ApiResponse>> GetAccessRights(int roleId)
        {
            var response = await _adminService.GetAccessRightsHierarchy(roleId);
            return Ok(response);
        }

        [HttpPut("roles/{roleId}/access-rights")]
        public async Task<ActionResult<ApiResponse>> UpdateAccessRights(int roleId, [FromBody] List<int> accessRightIds)
        {
            var response = await _adminService.UpdateRoleAccessRights(roleId, accessRightIds);
            return Ok(response);
        }

        // --- ADDRESS VERIFICATIONS ---
        [HttpGet("address-verifications")]
        public async Task<ActionResult<ApiResponse>> ListAddressVerifications([FromQuery] string? status)
        {
            var response = await _adminService.ListAddressVerifications(status);
            return Ok(response);
        }

        [HttpPost("address-verifications/review")]
        public async Task<ActionResult<ApiResponse>> ReviewAddressVerification([FromBody] AdminAddressReviewRequest request)
        {
            var userIdStr = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            var response = await _adminService.ReviewAddressVerification(request, long.Parse(userIdStr));
            return Ok(response);
        }

        [HttpGet("portfolios/active")]
        public async Task<ActionResult<ApiResponse>> ListActivePortfolios()
        {
            var response = await _adminService.GetActivePortfolios();
            return Ok(response);
        }

        // --- SYSTEM SETTINGS ---
        [HttpGet("settings")]
        public async Task<ActionResult<ApiResponse>> GetSettings()
        {
            var response = await _adminService.GetSystemSettings();
            return Ok(response);
        }

        [HttpPut("settings")]
        public async Task<ActionResult<ApiResponse>> UpdateSettings([FromBody] TblSystemSetting settings)
        {
            var response = await _adminService.UpdateSystemSettings(settings);
            return Ok(response);
        }

        // --- WITHDRAWAL MANAGEMENT ---
        [HttpGet("withdrawals")]
        public async Task<ActionResult<ApiResponse>> ListWithdrawals([FromQuery] string? status)
        {
            var response = await _adminService.ListWithdrawalRequests(status);
            return Ok(response);
        }

        [HttpPost("withdrawals/review")]
        public async Task<ActionResult<ApiResponse>> ReviewWithdrawal([FromBody] AdminWithdrawalReviewRequest request)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            var response = await _adminService.ReviewWithdrawalRequest(request, long.Parse(userIdStr));
            return Ok(response);
        }

        // --- LIQUIDATION MANAGEMENT ---
        [HttpGet("liquidations")]
        public async Task<ActionResult<ApiResponse>> ListLiquidations([FromQuery] int? status)
        {
            var response = await _adminService.ListLiquidationRequests(status);
            return Ok(response);
        }

        [HttpPost("liquidations/review")]
        public async Task<ActionResult<ApiResponse>> ReviewLiquidation([FromBody] AdminLiquidationReviewRequest request)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            var response = await _adminService.ReviewLiquidationRequest(request, long.Parse(userIdStr));
            return Ok(response);
        }
    }

    public class CreateAdminRequest
    {
        public SignUpRequest UserData { get; set; } = null!;
        public int RoleId { get; set; } // 1 for SuperAdmin, 2 for Admin
    }
}
