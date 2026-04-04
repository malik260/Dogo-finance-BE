using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.CustomerManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DogoFinance.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class BankController : ControllerBase
    {
        private readonly IBankService _bankService;

        public BankController(IBankService bankService)
        {
            _bankService = bankService;
        }

        [AllowAnonymous]
        [HttpGet("all")]
        public async Task<ActionResult<ApiResponse>> GetBanks()
        {
            var response = await _bankService.GetBanks();
            return Ok(response);
        }

        [HttpGet("accounts")]
        public async Task<ActionResult<ApiResponse>> GetMyBanks()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            var response = await _bankService.GetCustomerBanks(long.Parse(userIdStr));
            return Ok(response);
        }

        [HttpPost("accounts")]
        public async Task<ActionResult<ApiResponse>> AddBank([FromBody] AddCustomerBankRequest request)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            var response = await _bankService.AddCustomerBank(long.Parse(userIdStr), request);
            if (response.Boolean) return Ok(response);
            return StatusCode(response.Status, response);
        }

        [HttpDelete("accounts/{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteBank(long id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            var response = await _bankService.DeleteCustomerBank(long.Parse(userIdStr), id);
            if (response.Boolean) return Ok(response);
            return StatusCode(response.Status, response);
        }

        [HttpPost("accounts/{id}/default")]
        public async Task<ActionResult<ApiResponse>> SetDefault(long id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            var response = await _bankService.SetDefaultBank(long.Parse(userIdStr), id);
            if (response.Boolean) return Ok(response);
            return StatusCode(response.Status, response);
        }
    }
}
