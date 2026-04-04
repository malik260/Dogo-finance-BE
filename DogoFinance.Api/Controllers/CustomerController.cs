using DogoFinance.CustomerManagement.Interfaces;
using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.BusinessLogic.Layer.Response;
using Microsoft.AspNetCore.Mvc;

namespace DogoFinance.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly INextOfKinService _nokService;

        public CustomerController(ICustomerService customerService, INextOfKinService nokService)
        {
            _customerService = customerService;
            _nokService = nokService;
        }

        [HttpPost("signup")]
        public async Task<ActionResult<ApiResponse>> SignUp([FromBody] SignUpRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new ApiResponse { Message = "Validation failed: " + string.Join(", ", errors), Status = 400 });
            }

            var response = await _customerService.SignUp(request);
            if (response.Boolean) return Ok(response);
            return StatusCode(response.Status, response);
        }

        [HttpPost("verify-email")]
        public async Task<ActionResult<ApiResponse>> VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            var response = await _customerService.VerifyEmail(request);
            if (response.Boolean) return Ok(response);
            return StatusCode(response.Status, response);
        }

        [HttpPost("resend-code")]
        public async Task<ActionResult<ApiResponse>> ResendCode([FromQuery] string email)
        {
            var response = await _customerService.ResendVerificationCode(email);
            if (response.Boolean) return Ok(response);
            return StatusCode(response.Status, response);
        }

        [HttpPost("{customerId}/next-of-kin")]
        public async Task<ActionResult<ApiResponse>> AddNextOfKin(long customerId, [FromBody] AddNextOfKinRequest request)
        {
            var response = await _nokService.AddNextOfKin(customerId, request);
            if (response.Boolean) return Ok(response);
            return StatusCode(response.Status, response);
        }

        [HttpGet("{customerId}/next-of-kin")]
        public async Task<ActionResult<ApiResponse>> GetNextOfKins(long customerId)
        {
            var response = await _nokService.GetNextOfKins(customerId);
            return Ok(response);
        }

        [HttpGet("{customerId}/todo")]
        public async Task<ActionResult<ApiResponse>> GetTodoList(long customerId)
        {
            var response = await _customerService.GetTodoList(customerId);
            if (response.Boolean) return Ok(response);
            return StatusCode(response.Status, response);
        }

        [HttpGet("relationship-types")]
        public async Task<ActionResult<ApiResponse>> GetRelationshipTypes()
        {
            var response = await _nokService.GetRelationshipTypes();
            return Ok(response);
        }
    }
}
