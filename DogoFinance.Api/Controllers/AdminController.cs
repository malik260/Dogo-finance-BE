using DogoFinance.AdminManagement.Interfaces;
using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.BusinessLogic.Layer.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DogoFinance.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin")]
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

        [HttpGet("list")]
        public async Task<ActionResult<ApiResponse>> ListAdmins()
        {
            var response = await _adminService.GetAdmins();
            return Ok(response);
        }
    }

    public class CreateAdminRequest
    {
        public SignUpRequest UserData { get; set; } = null!;
        public int RoleId { get; set; } // 1 for SuperAdmin, 2 for Admin
    }
}
