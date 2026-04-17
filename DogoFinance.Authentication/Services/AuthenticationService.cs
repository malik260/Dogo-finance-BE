using DogoFinance.Authentication.Interfaces;
using DogoFinance.BusinessLogic.Layer.Helpers;
using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.DataAccess.Layer.Interfaces;
using DogoFinance.DataAccess.Layer.Models.Entities;
using DogoFinance.DataAccess.Layer.Repositories.Base;
using DogoFinance.Integration.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace DogoFinance.Authentication.Services
{
    public class AuthenticationService : DataRepository, IAuthenticationService
    {
        private readonly IUnitOfWork _uow;
        private readonly IEmailService _emailService;
        private readonly JwtHelper _jwtHelper;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationService(IUnitOfWork uow, IEmailService emailService, JwtHelper jwtHelper, ILogger<AuthenticationService> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _uow = uow;
            _emailService = emailService;
            _jwtHelper = jwtHelper;
            _logger = logger;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse> Login(LoginRequest request)
        {
            try
            {
                var response = new ApiResponse();
                var user = await _uow.Users.GetByEmail(request.Email);

                if (user == null || !HashHelper.VerifyHash(request.Password, user.PasswordHash, user.Salt))
                {
                    response.SetError("Invalid email or password.", 401);
                    return response;
                }

                if (user.IsActive == false)
                {
                    // If verification code is still present, email is likely not verified.
                    if (!string.IsNullOrEmpty(user.VerificationCode))
                    {
                        response.SetError("Please verify your email address before logging in. Check your inbox for the verification link.", 403);
                        return response;
                    }

                    response.SetError("Your account is suspended or blocked. Please contact admin.", 403);
                    return response;
                }

                if (user.IsLocked)
                {
                    response.SetError("Your account is locked due to multiple failed login attempts. Please reset your password or contact support.", 403);
                    return response;
                }

                var userRole = await BaseRepository().FindEntity<TblUserRole>(ur => ur.UserId == user.UserId);
                var roleName = "User";
                var permissions = new List<string>();

                if (userRole != null)
                {
                    var role = await BaseRepository().FindEntity<TblRole>(r => r.Id == userRole.RoleId);
                    
                    // Standardize role names for application authorization
                    if (userRole.RoleId == 1) roleName = "SuperAdmin";
                    else if (userRole.RoleId == 2) roleName = "Admin";
                    else roleName = role?.Name ?? "User";
                    
                    // Fetch individual permissions for this role
                    // FAIL-SAFE: SuperAdmin automatically gets all permissions
                    if (userRole.RoleId == 1 || roleName == "SuperAdmin")
                    {
                        var allPermissions = await BaseRepository().FindList<TblAccessRight>(ar => true);
                        permissions.AddRange(allPermissions.Select(ar => ar.Name));
                    }
                    else
                    {
                        var accessRights = await BaseRepository().FindList<TblRoleAccessRight>(ra => ra.RoleId == userRole.RoleId);
                        foreach (var ra in accessRights)
                        {
                            var ar = await BaseRepository().FindEntity<TblAccessRight>(x => x.Id == ra.AccessRightId);
                            if (ar != null) permissions.Add(ar.Name);
                        }
                    }
                }

                var customer = await _uow.Customers.GetByUserId(user.UserId);

                var tokenResponse = _jwtHelper.GenerateTokenResponse(user.UserId, user.Email, roleName);

                // Create a new session for this device
                var context = _httpContextAccessor.HttpContext;
                var session = new TblUserSession
                {
                    UserId = user.UserId,
                    RefreshToken = tokenResponse.RefreshToken,
                    RefreshTokenExpiry = DateTime.UtcNow.AddDays(7),
                    CreatedAt = DateTime.UtcNow,
                    LastUsedAt = DateTime.UtcNow,
                    DeviceName = request.DeviceName ?? "Unknown Device",
                    UserAgent = context?.Request.Headers["User-Agent"].ToString(),
                    IpAddress = context?.Connection.RemoteIpAddress?.ToString(),
                    IsRevoked = false
                };

                await BaseRepository().Insert(session);

                response.SetMessage("Login successful", true, new
                {
                    Token = tokenResponse.AccessToken,
                    tokenResponse.RefreshToken,
                    tokenResponse.Expiry,
                    user.Email,
                    Role = roleName,
                    Permissions = permissions,
                    FirstName = customer?.FirstName ?? user.FirstName ?? "Dogo",
                    LastName = customer?.LastName ?? user.LastName ?? "User",
                    UserId = user.UserId,
                    CustomerId = customer?.CustomerId ?? 0,
                    SessionId = session.SessionId,
                    user.IsPinSet
                });
                return response;

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<ApiResponse> ChangePassword(long userId, ChangePasswordRequest request)
        {
            var response = new ApiResponse();
            var user = await _uow.Users.GetById(userId);
            if (user == null) return new ApiResponse { Message = "User not found", Status = 404 };

            if (!HashHelper.VerifyHash(request.OldPassword, user.PasswordHash, user.Salt))
            {
                response.SetError("Incorrect current password.", 400);
                return response;
            }

            var (hash, salt) = HashHelper.CreateHash(request.NewPassword);
            user.PasswordHash = hash;
            user.Salt = salt;
            user.ModifiedAt = DateTime.UtcNow;

            await _uow.Users.SaveUser(user);
            response.SetMessage("Password updated successfully.", true);
            return response;
        }

        public async Task<ApiResponse> ForgotPassword(ForgotPasswordRequest request)
        {
            var response = new ApiResponse();
            var user = await _uow.Users.GetByEmail(request.Email);
            if (user == null)
            {
                // Security: Don't reveal if email exists or not, but for this demo:
                response.SetMessage("If an account exists for this email, a reset link has been sent.", true);
                return response;
            }

            var token = Guid.NewGuid().ToString("N");
            var reset = new TblPasswordReset
            {
                UserId = user.UserId,
                ResetCode = token,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            await BaseRepository().Insert(reset);

            // Send reset email
            var customer = await BaseRepository().FindEntity<TblCustomer>(c => c.UserId == user.UserId);
            var baseUrl = _configuration["SystemConfig:FrontendBaseUrl"] ?? "https://app.dogofinance.com";
            var placeholders = new Dictionary<string, string>
            {
                { "FirstName", customer?.FirstName ?? "DogoFinance User" },
                { "ResetLink", $"{baseUrl}/reset-password?token={token}" }
            };

            await _emailService.SendTemplateEmail(request.Email, "Reset Your Password - DogoFinance", "PasswordReset", placeholders);

            response.SetMessage("Reset link sent.", true);
            return response;
        }

        public async Task<ApiResponse> ResetPassword(ResetPasswordRequest request)
        {
            var response = new ApiResponse();
            var db = await BaseRepository().BeginTrans();

            try
            {
                var reset = await BaseRepository().FindEntity<TblPasswordReset>(r => r.ResetCode == request.Token && !r.IsUsed);
                if (reset == null || reset.ExpiresAt < DateTime.UtcNow)
                {
                    response.SetError("Invalid or expired reset token.", 400);
                    return response;
                }

                var user = await _uow.Users.GetById(reset.UserId);
                if (user == null) throw new Exception("User not found for reset");

                var (hash, salt) = HashHelper.CreateHash(request.NewPassword);
                user.PasswordHash = hash;
                user.Salt = salt;
                user.ModifiedAt = DateTime.UtcNow;

                // Use the same repository/context for user update to maintain transaction integrity
                await BaseRepository().Update(user);

                reset.IsUsed = true;
                await BaseRepository().Update(reset);

                await BaseRepository().CommitTrans();
                response.SetMessage("Password reset successfully. You can now login.", true);
                return response;
            }
            catch (Exception ex)
            {
                await BaseRepository().RollbackTrans();
                _logger.LogError(ex, "ResetPassword Failed");
                response.SetError("Internal error during reset.", 500);
                return response;
            }
        }

        public async Task<ApiResponse> Logout(long userId)
        {
            try
            {
                var user = await _uow.Users.GetById(userId);
                if (user != null)
                {
                    user.LastLogoutDate = DateTime.UtcNow;
                    await _uow.Users.SaveUser(user);

                    // Revoke the current specific session if possible
                    var refreshToken = _httpContextAccessor.HttpContext?.Request.Headers["X-Refresh-Token"].ToString();
                    if (!string.IsNullOrEmpty(refreshToken))
                    {
                        var session = await BaseRepository().FindEntity<TblUserSession>(s => s.RefreshToken == refreshToken && s.UserId == userId);
                        if (session != null)
                        {
                            await BaseRepository().Delete(session);
                        }
                    }
                }

                _logger.LogInformation("User {UserId} logged out at {Time}", userId, DateTime.UtcNow);
                return new ApiResponse { Success = true, Message = "Logged out successfully", Boolean = true };
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<ApiResponse> RefreshToken(string accessToken, string refreshToken)
        {
            var response = new ApiResponse();

            // Find active session with this refresh token
            var session = await BaseRepository().FindEntity<TblUserSession>(s => s.RefreshToken == refreshToken && !s.IsRevoked);

            if (session == null || session.RefreshTokenExpiry < DateTime.UtcNow)
            {
                response.SetError("Invalid or expired session. Please login again.", 401);
                return response;
            }

            var user = await _uow.Users.GetById(session.UserId);
            if (user == null)
            {
                response.SetError("User not found.", 401);
                return response;
            }

            // Get role
            var userRole = await BaseRepository().FindEntity<TblUserRole>(ur => ur.UserId == user.UserId);
            var roleName = "User";
            if (userRole != null)
            {
                var role = await BaseRepository().FindEntity<TblRole>(r => r.Id == userRole.RoleId);
                roleName = role?.Name ?? "User";
            }

            // Generate new pair (Rotation)
            var tokenResponse = _jwtHelper.GenerateTokenResponse(user.UserId, user.Email, roleName);

            // Update session (Rotation)
            session.RefreshToken = tokenResponse.RefreshToken;
            session.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            session.LastUsedAt = DateTime.UtcNow;
            session.IpAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

            await BaseRepository().Update(session);

            response.SetMessage("Session renewed", true, new
            {
                Token = tokenResponse.AccessToken,
                tokenResponse.RefreshToken,
                tokenResponse.Expiry
            });

            return response;
        }
        public async Task<ApiResponse> GetActiveSessions(long userId)
        {
            var sessions = await BaseRepository().FindList<TblUserSession>(s => s.UserId == userId && !s.IsRevoked);
            return new ApiResponse
            {
                Success = true,
                Boolean = true,
                Status = 200,
                Message = "Sessions retrieved",
                Data = sessions.Select(s => new
                {
                    s.SessionId,
                    s.DeviceName,
                    s.IpAddress,
                    s.LastUsedAt,
                    s.CreatedAt,
                    IsCurrent = _httpContextAccessor.HttpContext?.Request.Headers["X-Refresh-Token"].ToString() == s.RefreshToken
                })
            };
        }

        public async Task<ApiResponse> RevokeSession(long sessionId, long userId)
        {
            var session = await BaseRepository().FindEntity<TblUserSession>(s => s.SessionId == sessionId && s.UserId == userId);
            if (session == null) return new ApiResponse { Message = "Session not found", Status = 404 };

            await BaseRepository().Delete(session);
            return new ApiResponse { Success = true, Boolean = true, Status = 200, Message = "Session revoked successfully" };
        }
    }
}
