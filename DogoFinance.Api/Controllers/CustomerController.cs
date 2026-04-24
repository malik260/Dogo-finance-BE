using DogoFinance.CustomerManagement.Interfaces;
using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.BusinessLogic.Layer.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace DogoFinance.Api.Controllers
{
    [ApiController]
    [Authorize]
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

        [AllowAnonymous]
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

        [AllowAnonymous]
        [HttpPost("verify-email")]
        public async Task<ActionResult<ApiResponse>> VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            var response = await _customerService.VerifyEmail(request);
            if (response.Boolean) return Ok(response);
            return StatusCode(response.Status, response);
        }

        [AllowAnonymous]
        [HttpPost("resend-code")]
        public async Task<ActionResult<ApiResponse>> ResendCode([FromBody] ResendCodeRequest request)
        {
            var response = await _customerService.ResendVerificationCode(request.Email);
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

        [HttpPost("{customerId}/verify-bvn")]
        public async Task<ActionResult<ApiResponse>> VerifyBvn(long customerId, [FromBody] BvnVerificationRequest request)
        {
            var response = await _customerService.VerifyBvn(customerId, request);
            if (response.Boolean) return Ok(response);
            return StatusCode(response.Status, response);
        }

        [HttpPost("{customerId}/verify-nin")]
        public async Task<ActionResult<ApiResponse>> VerifyNin(long customerId, [FromBody] NinVerificationRequest request)
        {
            var response = await _customerService.VerifyNin(customerId, request);
            if (response.Boolean) return Ok(response);
            return StatusCode(response.Status, response);
        }

        [HttpGet("relationship-types")]
        public async Task<ActionResult<ApiResponse>> GetRelationshipTypes()
        {
            var response = await _nokService.GetRelationshipTypes();
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("genders")]
        public async Task<ActionResult<ApiResponse>> GetGenders()
        {
            var response = await _customerService.GetGenders();
            return Ok(response);
        }

        [HttpGet("profile")]
        public async Task<ActionResult<ApiResponse>> GetProfile()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ApiResponse { Message = "Not logged in", Status = 401 });

            var response = await _customerService.GetProfile(long.Parse(userIdStr));
            return Ok(response);
        }

        [HttpPost("update-profile")]
        public async Task<ActionResult<ApiResponse>> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ApiResponse { Message = "Not logged in", Status = 401 });

            var response = await _customerService.UpdateProfile(long.Parse(userIdStr), request);
            return Ok(response);
        }

        [HttpGet("address-doc-types")]
        public async Task<ActionResult<ApiResponse>> GetAddressDocTypes()
        {
            var response = await _customerService.GetAddressDocTypes();
            return Ok(response);
        }

        [HttpPost("verify-address")]
        public async Task<ActionResult<ApiResponse>> VerifyAddress([FromForm] AddressVerificationRequest request)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ApiResponse { Message = "Not logged in", Status = 401 });

            // In a real app, customer ID might be linked to User ID 1:1
            var response = await _customerService.InitiateAddressVerification(long.Parse(userIdStr), request);
            return Ok(response);
        }

        [HttpGet("verifications")]
        public async Task<ActionResult<ApiResponse>> GetVerifications()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ApiResponse { Message = "Not logged in", Status = 401 });

            var response = await _customerService.GetVerificationStatuses(long.Parse(userIdStr));
            return Ok(response);
        }
    }
}
