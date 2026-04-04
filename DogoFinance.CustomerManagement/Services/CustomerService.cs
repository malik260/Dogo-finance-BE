using DogoFinance.BusinessLogic.Layer.Enums;
using DogoFinance.BusinessLogic.Layer.Helpers;
using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.CustomerManagement.Interfaces;
using DogoFinance.DataAccess.Layer.Interfaces;
using DogoFinance.DataAccess.Layer.Models.Entities;
using DogoFinance.DataAccess.Layer.Repositories.Base;
using DogoFinance.Integration.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace DogoFinance.CustomerManagement.Services
{
    public class CustomerService : DataRepository, ICustomerService
    {
        private readonly IUnitOfWork _uow;
        private readonly IEmailService _emailService;
        private readonly ILogger<CustomerService> _logger;
        private readonly IConfiguration _configuration;

        public CustomerService(IUnitOfWork uow, IEmailService emailService, ILogger<CustomerService> logger, IConfiguration configuration)
        {
            _uow = uow;
            _emailService = emailService;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<ApiResponse> SignUp(SignUpRequest request)
        {
            var response = new ApiResponse();
            var db = await BaseRepository().BeginTrans();

            try
            {
                var existingUser = await _uow.Users.GetByEmail(request.Email);
                if (existingUser != null)
                {
                    response.SetError("Email already in use.", 400);
                    return response;
                }

                existingUser = await _uow.Users.GetByPhoneNumber(request.PhoneNumber);
                if (existingUser != null)
                {
                    response.SetError("Phone number already in use.", 400);
                    return response;
                }

                var (hash, salt) = HashHelper.CreateHash(request.Password);

                var user = new TblUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    PasswordHash = hash,
                    Salt = salt,
                    IsActive = false,
                    IsLocked = false,
                    FailedLoginAttempts = 0,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                var verificationCode = new Random().Next(100000, 999999).ToString();
                user.VerificationCode = verificationCode;
                user.VerificationExpiry = DateTime.UtcNow.AddMinutes(15);

                await _uow.Users.SaveUser(user);

                var customer = new TblCustomer
                {
                    UserId = user.UserId,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    DateOfBirth = request.DateOfBirth,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                await _uow.Customers.SaveCustomer(customer);

                // Assign Role (3 = Customer)
                var userRole = new TblUserRole
                {
                    UserId = user.UserId,
                    RoleId = (int)UserRole.Customer
                };
                await BaseRepository().Insert(userRole);

                var baseUrl = _configuration["SystemConfig:FrontendBaseUrl"] ?? "http://localhost:4200";
                var verificationLink = $"{baseUrl}/verify-email?email={Uri.EscapeDataString(user.Email)}&code={verificationCode}";

                var emailPlaceholders = new Dictionary<string, string>
                {
                    { "FirstName", request.FirstName },
                    { "VerificationLink", verificationLink }
                };

                var emailSent = await _emailService.SendTemplateEmail(
                    request.Email, 
                    "Verify Your Account - DogoFinance", 
                    "RegistrationVerification", 
                    emailPlaceholders
                );
                
                if (!emailSent) _logger.LogWarning("Verification email failed for {Email}", request.Email);

                await BaseRepository().CommitTrans();

                response.SetMessage("Sign up successful! Please check your email for a verification link.", true);
                return response;
            }
            catch (Exception ex)
            {
                await BaseRepository().RollbackTrans();
                _logger.LogError(ex, "Signup failed for {Email}", request.Email);
                response.SetError("Registration failed. Please try again later.", 500);
                return response;
            }
        }

        public async Task<ApiResponse> VerifyEmail(VerifyEmailRequest request)
        {
            var user = await _uow.Users.GetByEmail(request.Email);
            if (user == null) return new ApiResponse { Message = "User not found", Status = 404 };

            if (user.IsActive == true) return new ApiResponse { Success = true, Message = "Email already verified", Boolean = true };

            if (user.VerificationCode != request.Code) return new ApiResponse { Message = "Invalid verification code", Status = 400 };

            if (user.VerificationExpiry < DateTime.UtcNow) return new ApiResponse { Message = "Verification code expired", Status = 400 };

            user.IsActive = true;
            user.VerificationCode = null;
            user.VerificationExpiry = null;
            user.ModifiedAt = DateTime.UtcNow;

            await _uow.Users.SaveUser(user);

            return new ApiResponse { Success = true, Boolean = true, Message = "Email verified successfully" };
        }

        public async Task<ApiResponse> ResendVerificationCode(string email)
        {
            var user = await _uow.Users.GetByEmail(email);
            if (user == null) return new ApiResponse { Message = "User not found", Status = 404 };

            if (user.IsActive == true) return new ApiResponse { Message = "Email already verified", Status = 400 };

            var verificationCode = new Random().Next(100000, 999999).ToString();
            user.VerificationCode = verificationCode;
            user.VerificationExpiry = DateTime.UtcNow.AddMinutes(15);

            await _uow.Users.SaveUser(user);

            var customer = await _uow.Customers.GetByUserId(user.UserId);
            var firstName = customer?.FirstName ?? "there";

            var baseUrl = _configuration["SystemConfig:FrontendBaseUrl"] ?? "http://localhost:4200";
            var verificationLink = $"{baseUrl}/verify-email?email={Uri.EscapeDataString(email)}&code={verificationCode}";

            var emailPlaceholders = new Dictionary<string, string>
            {
                { "FirstName", firstName },
                { "VerificationLink", verificationLink }
            };

            var emailSent = await _emailService.SendTemplateEmail(
                email, 
                "Verify Your Account - DogoFinance", 
                "RegistrationVerification", 
                emailPlaceholders
            );

            if (!emailSent) return new ApiResponse { Message = "Failed to send email", Status = 500 };

            return new ApiResponse { Success = true, Boolean = true, Message = "Verification code resent successfully" };
        }

        public async Task<ApiResponse> GetTodoList(long customerId)
        {
            var todoList = new List<TodoItem>();

            var customer = await _uow.Customers.GetCustomerDetailed(customerId);
            if (customer == null) return new ApiResponse { Message = "Customer not found", Status = 404 };

            // 1. Verify BVN
            if (!customer.Bvnverified)
            {
                todoList.Add(new TodoItem
                {
                    Title = "Verify BVN",
                    Subtitle = "Secure your financial records with BVN",
                    ActionText = "VERIFY NOW",
                    ActionType = "BVN_VERIFY",
                    Icon = "fingerprint"
                });
            }

            // 2. Verify NIN
            if (!customer.Ninverified)
            {
                todoList.Add(new TodoItem
                {
                    Title = "Verify Your NIN",
                    Subtitle = "Secure your account identity",
                    ActionText = "VERIFY NOW",
                    ActionType = "NIN_VERIFY",
                    Icon = "security"
                });
            }

            // 3. Create Transaction PIN
            if (customer.User != null && !customer.User.IsPinSet)
            {
                todoList.Add(new TodoItem
                {
                    Title = "Create Transaction PIN",
                    Subtitle = "Secure your wallet from unauthorized access",
                    ActionText = "SETUP",
                    ActionType = "PIN_SETUP",
                    Icon = "lock"
                });
            }

            // 4. Add Next of Kin
            if (customer.TblNextOfKins == null || !customer.TblNextOfKins.Any())
            {
                todoList.Add(new TodoItem
                {
                    Title = "Add Next of Kin",
                    Subtitle = "Manage your wealth legacy",
                    ActionText = "UPDATE",
                    ActionType = "KIN_ADD",
                    Icon = "people"
                });
            }

            return new ApiResponse
            {
                Success = true,
                Boolean = true,
                Data = todoList,
                Message = "Todo list retrieved successfully"
            };
        }
    }
}
