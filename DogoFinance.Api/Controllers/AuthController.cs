using DogoFinance.Authentication.Interfaces;
using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.BusinessLogic.Layer.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DogoFinance.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authService;
        private readonly IPinService _pinService;

        public AuthController(IAuthenticationService authService, IPinService pinService)
        {
            _authService = authService;
            _pinService = pinService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse>> Login([FromBody] LoginRequest request)
        {
            var response = await _authService.Login(request);
            if (response.Boolean) return Ok(response);
            return StatusCode(response.Status, response);
        }

        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponse>> Logout()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ApiResponse { Message = "Not logged in", Status = 401 });

            var response = await _authService.Logout(long.Parse(userIdStr));
            return Ok(response);
        }

        [HttpPost("change-password")]
        public async Task<ActionResult<ApiResponse>> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            // In a real app, userId comes from User.FindFirstValue(ClaimTypes.NameIdentifier)
            // For now, I'll provide a placeholder or you can pass it if not authenticated yet.
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ApiResponse { Message = "Not logged in", Status = 401 });

            var response = await _authService.ChangePassword(long.Parse(userIdStr), request);
            if (response.Boolean) return Ok(response);
            return StatusCode(response.Status, response);
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<ActionResult<ApiResponse>> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var response = await _authService.ForgotPassword(request);
            if (response.Boolean) return Ok(response);
            return StatusCode(response.Status, response);
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<ActionResult<ApiResponse>> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var response = await _authService.ResetPassword(request);
            if (response.Boolean) return Ok(response);
            return StatusCode(response.Status, response);
        }

        [HttpPost("pin/setup")]
        public async Task<ActionResult<ApiResponse>> SetupPin([FromBody] SetPinRequest request)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ApiResponse { Message = "Not logged in", Status = 401 });

            var response = await _pinService.SetTransactionPin(long.Parse(userIdStr), request);
            if (response.Boolean) return Ok(response);
            return StatusCode(response.Status, response);
        }

        [HttpPost("pin/change")]
        public async Task<ActionResult<ApiResponse>> ChangePin([FromBody] ChangePinRequest request)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ApiResponse { Message = "Not logged in", Status = 401 });

            var response = await _pinService.ChangeTransactionPin(long.Parse(userIdStr), request);
            if (response.Boolean) return Ok(response);
            return StatusCode(response.Status, response);
        }

        [AllowAnonymous]
        [HttpPost("pin/forgot")]
        public async Task<ActionResult<ApiResponse>> ForgotPin([FromBody] ForgotPinRequest request)
        {
            var response = await _pinService.ForgotPin(request);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("pin/reset")]
        public async Task<ActionResult<ApiResponse>> ResetPin([FromBody] ResetPinRequest request)
        {
            var response = await _pinService.ResetPin(request);
            if (response.Boolean) return Ok(response);
            return StatusCode(response.Status, response);
        }

        [HttpPost("2fa/toggle")]
        public async Task<ActionResult<ApiResponse>> Toggle2fa([FromQuery] bool status)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ApiResponse { Message = "Not logged in", Status = 401 });

            var response = await _pinService.Toggle2fa(long.Parse(userIdStr), status);
            if (response.Boolean) return Ok(response);
            return StatusCode(response.Status, response);
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<ActionResult<ApiResponse>> Refresh([FromBody] TokenRequest request)
        {
            var response = await _authService.RefreshToken(request.Token, request.RefreshToken);
            if (response.Boolean) return Ok(response);
            return StatusCode(response.Status, response);
        }

        [HttpGet("sessions")]
        public async Task<ActionResult<ApiResponse>> GetSessions()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ApiResponse { Message = "Not logged in", Status = 401 });

            var response = await _authService.GetActiveSessions(long.Parse(userIdStr));
            return Ok(response);
        }

        [HttpDelete("sessions/{sessionId}")]
        public async Task<ActionResult<ApiResponse>> RevokeSession(long sessionId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ApiResponse { Message = "Not logged in", Status = 401 });

            var response = await _authService.RevokeSession(sessionId, long.Parse(userIdStr));
            return Ok(response);
        }
    }

    public class TokenRequest
    {
        public string Token { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}
