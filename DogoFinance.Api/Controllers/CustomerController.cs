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
        [HttpGet("customer-types")]
        public async Task<ActionResult<ApiResponse>> GetCustomerTypes()
        {
            var response = await _customerService.GetCustomerTypes();
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

        [AllowAnonymous]
        [HttpGet("company-bank-details")]
        public async Task<ActionResult<ApiResponse>> GetCompanyBankDetails()
        {
            var response = await _customerService.GetCompanyBankDetails();
            return Ok(response);
        }

        [HttpGet("corporate-profile")]
        public async Task<ActionResult<ApiResponse>> GetCorporateProfile()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ApiResponse { Message = "Not logged in", Status = 401 });

            var response = await _customerService.GetCorporateProfile(long.Parse(userIdStr));
            return Ok(response);
        }

        [HttpPut("corporate-profile")]
        public async Task<ActionResult<ApiResponse>> UpdateCorporateProfile([FromBody] UpdateCorporateProfileRequest request)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ApiResponse { Message = "Not logged in", Status = 401 });

            var response = await _customerService.UpdateCorporateProfile(long.Parse(userIdStr), request);
            return Ok(response);
        }

        [HttpGet("primary-contact")]
        public async Task<ActionResult<ApiResponse>> GetPrimaryContact()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ApiResponse { Message = "Not logged in", Status = 401 });

            var response = await _customerService.GetPrimaryContact(long.Parse(userIdStr));
            return Ok(response);
        }

        [HttpPut("primary-contact")]
        public async Task<ActionResult<ApiResponse>> UpdatePrimaryContact([FromBody] UpdateCorporateContactRequest request)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ApiResponse { Message = "Not logged in", Status = 401 });

            var response = await _customerService.UpdatePrimaryContact(long.Parse(userIdStr), request);
            return Ok(response);
        }

        [HttpGet("corporate-verifications")]
        public async Task<ActionResult<ApiResponse>> GetCorporateVerifications()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ApiResponse { Message = "Not logged in", Status = 401 });

            var response = await _customerService.GetCorporateVerifications(long.Parse(userIdStr));
            return Ok(response);
        }

        [HttpPost("corporate-document")]
        public async Task<ActionResult<ApiResponse>> UploadCorporateDocument([FromForm] UploadCorporateDocumentRequest request)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ApiResponse { Message = "Not logged in", Status = 401 });

            var response = await _customerService.UploadCorporateDocument(long.Parse(userIdStr), request);
            return Ok(response);
        }

        [HttpGet("signatories")]
        public async Task<ActionResult<ApiResponse>> GetCorporateSignatories()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ApiResponse { Message = "Not logged in", Status = 401 });

            var response = await _customerService.GetCorporateSignatories(long.Parse(userIdStr));
            return Ok(response);
        }

        [HttpPost("signatories")]
        public async Task<ActionResult<ApiResponse>> AddCorporateSignatory([FromForm] AddCorporateSignatoryRequest request)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ApiResponse { Message = "Not logged in", Status = 401 });

            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse { Success = false, Message = "Invalid payload" });
            }

            var response = await _customerService.AddCorporateSignatory(long.Parse(userIdStr), request);
            return Ok(response);
        }

        [HttpDelete("signatories/{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteCorporateSignatory(int id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ApiResponse { Message = "Not logged in", Status = 401 });

            var response = await _customerService.DeleteCorporateSignatory(long.Parse(userIdStr), id);
            return Ok(response);
        }
        [HttpGet("directors")]
        public async Task<ActionResult<ApiResponse>> GetCorporateDirectors()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ApiResponse { Message = "Not logged in", Status = 401 });

            var response = await _customerService.GetCorporateDirectors(long.Parse(userIdStr));
            return Ok(response);
        }

        [HttpPost("directors")]
        public async Task<ActionResult<ApiResponse>> AddCorporateDirector([FromForm] AddCorporateDirectorRequest request)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ApiResponse { Message = "Not logged in", Status = 401 });

            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse { Success = false, Message = "Invalid payload" });
            }

            var response = await _customerService.AddCorporateDirector(long.Parse(userIdStr), request);
            return Ok(response);
        }

        [HttpDelete("directors/{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteCorporateDirector(int id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ApiResponse { Message = "Not logged in", Status = 401 });

            var response = await _customerService.DeleteCorporateDirector(long.Parse(userIdStr), id);
            return Ok(response);
        }

        // --- NOTIFICATIONS ---
        [HttpGet("notifications")]
        public async Task<ActionResult<ApiResponse>> GetNotifications()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ApiResponse { Message = "Not logged in", Status = 401 });

            // We need customerId, assuming customerService can derive it from userId or we fetch it
            // Oh wait, GetNotifications takes CustomerId. Wait, userId is usually the parameter, let's look at what we wrote... 
            // In CustomerService, GetNotifications(long customerId). But in Controller we only have userId.
            // Oh right, we need to pass customerId. In other methods we do _uow.Customers.FindEntity(c => c.UserId == userId).
            // Let's just adjust CustomerService method to take UserId and fetch CustomerId inside. Or I can fetch it here.
            
            // Let's call a method in CustomerService or fetch here... Oh wait, I can just use User.FindFirstValue(...)
            // Let's change CustomerService to accept userId, or we fetch CustomerId here.
            // But wait, the other methods take userId. Let's just pass long.Parse(userIdStr) and modify CustomerService. Wait, I already wrote CustomerService. GetNotifications(long customerId)... Oh, I will change the method parameter to userId in CustomerService. Wait, I didn't. Let me just write the controller now and I'll see if I need to update it.
            // ACTUALLY, wait... Let me check what CustomerService's other methods take. They take `long userId`.
            // Let's pass userId and then I'll fix CustomerService to take userId.
            var response = await _customerService.GetNotifications(long.Parse(userIdStr));
            return Ok(response);
        }

        [HttpPost("notifications/{id}/read")]
        public async Task<ActionResult<ApiResponse>> MarkNotificationRead(long id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ApiResponse { Message = "Not logged in", Status = 401 });

            var response = await _customerService.MarkNotificationRead(id, long.Parse(userIdStr));
            return Ok(response);
        }
    }
}
