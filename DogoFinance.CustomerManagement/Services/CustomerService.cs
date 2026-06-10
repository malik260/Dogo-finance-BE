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
using DogoFinance.TransactionManagement.Interfaces;
using DogoFinance.Integration.Models.Monnify;
using Microsoft.EntityFrameworkCore;

namespace DogoFinance.CustomerManagement.Services
{
    public class CustomerService : DataRepository, ICustomerService
    {
        private readonly IUnitOfWork _uow;
        private readonly IEmailService _emailService;
        private readonly ILogger<CustomerService> _logger;
        private readonly IConfiguration _configuration;
        private readonly ITransactionService _transactionService;
        private readonly IMonnifyService _monnifyService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IDocumentProcessingService _docProcessor;
        private readonly IYouVerifyService _youVerifyService;

        public CustomerService(IUnitOfWork uow, IEmailService emailService, ILogger<CustomerService> logger, IConfiguration configuration, ITransactionService transactionService, IMonnifyService monnifyService, ICloudinaryService cloudinaryService, IDocumentProcessingService docProcessor, IYouVerifyService youVerifyService)
        {
            _uow = uow;
            _emailService = emailService;
            _logger = logger;
            _configuration = configuration;
            _transactionService = transactionService;
            _monnifyService = monnifyService;
            _cloudinaryService = cloudinaryService;
            _docProcessor = docProcessor;
            _youVerifyService = youVerifyService;
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

                bool isCorporate = request.CustomerTypeId == 2;

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
                    IsDeleted = false,
                    FirstName = isCorporate ? request.BusinessName : request.FirstName,
                    LastName = isCorporate ? request.BusinessName : request.LastName
                };

                var verificationCode = new Random().Next(100000, 999999).ToString();
                user.VerificationCode = verificationCode;
                user.VerificationExpiry = DateTime.UtcNow.AddMinutes(15);

                await _uow.Users.SaveUser(user);

                var customer = new TblCustomer
                {
                    UserId = user.UserId,
                    FirstName = isCorporate ? request.BusinessName : request.FirstName,
                    LastName = isCorporate ? request.BusinessName : request.LastName,
                    DateOfBirth = request.DateOfBirth,
                    Gender = request.GenderId,
                    CustomerTypeId = request.CustomerTypeId,
                    BusinessName = request.BusinessName,
                    RegistrationNumber = request.RegistrationNumber,
                    TaxIdentificationNumber = request.TaxIdentificationNumber,
                    DateOfIncorporation = request.DateOfIncorporation,
                    IsPolitcallyExposed = request.IsPoliticallyExposed,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false,
                    PhoneNumber = request.PhoneNumber
                };

                await _uow.Customers.SaveCustomer(customer);
                
                // Create Wallet for Customer
                var wallet = new TblWallet
                {
                    CustomerId = customer.CustomerId,
                    WalletNumber = new Random().NextInt64(1000000000, 9999999999).ToString(),
                    Currency = 1, // Assume 1 for NGN
                    Balance = 0,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                await _uow.Wallets.CreateWallet(wallet);

                // Assign Role (3 = Customer)
                var userRole = new TblUserRole
                {
                    UserId = user.UserId,
                    RoleId = (int)UserRole.Customer
                };
                await BaseRepository().Insert(userRole);


                var baseUrl = (_configuration["SystemConfig:FrontendBaseUrl"] ?? "http://localhost:4200").Trim().TrimEnd('/');
                //var verificationLink = $"{baseUrl}/verify-email?email={Uri.EscapeDataString(user.Email.Trim())}&code={verificationCode}";

                var verificationLink = $"{baseUrl}/verify-email?email={Uri.EscapeDataString(user.Email.Trim())}" +
                                        $"&code={Uri.EscapeDataString(verificationCode)}";

                var apiBaseUrl = (_configuration["SystemConfig:ApiBaseUrl"] ?? "https://localhost:7168").Trim().TrimEnd('/');
                var logoUrl = $"{apiBaseUrl}/Images/DOGO.jpg.webp";

                var emailPlaceholders = new Dictionary<string, string>
                {
                    { "FirstName", isCorporate ? request.BusinessName : request.FirstName },
                    { "VerificationLink", verificationLink },
                    { "LogoUrl", "cid:logo" }
                };

                var emailSubject = isCorporate ? "Verify Your Corporate Account - DogoFinance" : "Verify Your Account - DogoFinance";

                var emailSent = await _emailService.SendTemplateEmail(
                    request.Email, 
                    emailSubject, 
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
            var email = request.Email.Trim();
            var code = request.Code.Trim();

            var user = await _uow.Users.GetByEmail(email);
            if (user == null) return new ApiResponse { Message = "User not found", Status = 404 };

            if (user.IsActive == true) return new ApiResponse { Success = true, Message = "Email already verified", Boolean = true };

            if (user.VerificationCode != code) return new ApiResponse { Message = "Invalid verification code", Status = 400 };

            if (user.VerificationExpiry < DateTime.UtcNow) return new ApiResponse { Message = "Verification code expired", Status = 400 };

            user.IsActive = true;
            user.VerificationCode = null;
            user.VerificationExpiry = null;
            user.ModifiedAt = DateTime.UtcNow;

            await _uow.Users.SaveUser(user);
            // Explicitly save changes to ensure persistence outside of any implicit transactions
            await _uow.SaveChangesAsync();

            return new ApiResponse { Success = true, Boolean = true, Message = "Email verified successfully" };
        }

        public async Task<ApiResponse> ResendVerificationCode(string email)
        {
            if (string.IsNullOrEmpty(email)) return new ApiResponse { Message = "Email is required", Status = 400 };
            
            var user = await _uow.Users.GetByEmail(email.Trim());
            if (user == null) return new ApiResponse { Message = "User not found", Status = 404 };

            if (user.IsActive == true) return new ApiResponse { Message = "Email already verified", Status = 400 };

            var verificationCode = new Random().Next(100000, 999999).ToString();
            user.VerificationCode = verificationCode;
            user.VerificationExpiry = DateTime.UtcNow.AddMinutes(15);

            await _uow.Users.SaveUser(user);
            await _uow.SaveChangesAsync();

            var customer = await _uow.Customers.GetByUserId(user.UserId);
            var firstName = customer?.FirstName ?? "there";

            var baseUrl = (_configuration["SystemConfig:FrontendBaseUrl"] ?? "http://localhost:4200").Trim().TrimEnd('/');
            var verificationLink = $"{baseUrl}/verify-email?email={Uri.EscapeDataString(user.Email.Trim())}&code={verificationCode}";

            var apiBaseUrl = (_configuration["SystemConfig:ApiBaseUrl"] ?? "https://localhost:7168").Trim().TrimEnd('/');
            var logoUrl = $"{apiBaseUrl}/Images/DOGO.jpg.webp";

            var emailPlaceholders = new Dictionary<string, string>
            {
                { "FirstName", firstName },
                { "VerificationLink", verificationLink },
                { "LogoUrl", "cid:logo" }
            };

            var emailSent = await _emailService.SendTemplateEmail(
                user.Email, 
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

            // 5. Address Verification
            var addrVerif = await BaseRepository().FindEntity<TblCustomerAddressVerification>(v => v.CustomerId == customerId);
            if (addrVerif == null || (addrVerif.Status != "Approved" && addrVerif.Status != "Approved"))
            {
                // Only show if not already approved or at least not verified yet
                if (string.IsNullOrEmpty(customer.Address))
                {
                    todoList.Add(new TodoItem
                    {
                        Title = "Address Verification",
                        Subtitle = "Confirm your residential address with a utility bill",
                        ActionText = "VERIFY NOW",
                        ActionType = "ADDR_VERIFY",
                        Icon = "map"
                    });
                }
            }

            return new ApiResponse
            {
                Success = true,
                Boolean = true,
                Data = todoList,
                Message = "Todo list retrieved successfully"
            };
        }

        public async Task<ApiResponse> VerifyBvn(long customerId, BvnVerificationRequest request)
        {
            var customer = await _uow.Customers.GetCustomerDetailed(customerId);
            if (customer == null) return new ApiResponse { Message = "Customer not found", Status = 404 };

            if (customer.Bvnverified) return new ApiResponse { Success = true, Message = "BVN already verified", Boolean = true };

            var youVerifyRequest = new Integration.Models.YouVerify.BvnVerificationRequest
            {
                Id = request.Bvn,
                IsSubjectConsent = true,
                Validations = new Integration.Models.YouVerify.IdentityValidations
                {
                    Data = new Integration.Models.YouVerify.IdentityDataValidation
                    {
                        FirstName = customer.FirstName,
                        LastName = customer.LastName,
                        DateOfBirth = customer.DateOfBirth?.ToString("yyyy-MM-dd")
                    }
                },
                Metadata = new Dictionary<string, string> { { "customerId", customerId.ToString() } }
            };

            var youVerifyResponse = await _youVerifyService.VerifyBvn(youVerifyRequest);

            if (youVerifyResponse != null && youVerifyResponse.Success)
            {
                // Successful match logic
                if (youVerifyResponse.Data?.Status == "found")
                {
                    // Ensure the data validation also passed
                    if (!youVerifyResponse.Data.AllValidationPassed)
                    {
                        var messages = youVerifyResponse.Data.Validations?.ValidationMessages ?? "Data validation failed.";
                        return new ApiResponse { Message = $"BVN details found but validation failed: {messages}", Status = 400 };
                    }

                    customer.Bvn = request.Bvn;
                    customer.Bvnverified = true;
                    customer.BvnverifiedAt = DateTime.UtcNow;
                    customer.ModifiedAt = DateTime.UtcNow;

                    await _uow.Customers.SaveCustomer(customer);

                    return new ApiResponse { Success = true, Message = "BVN verified successfully", Boolean = true, Data = youVerifyResponse.Data };
                }
                
                return new ApiResponse { Message = "BVN not found or invalid.", Status = 400 };
            }

            return new ApiResponse { Message = youVerifyResponse?.Message ?? "BVN verification failed", Status = 400 };
        }

        public async Task<ApiResponse> VerifyNin(long customerId, NinVerificationRequest request)
        {
            var customer = await _uow.Customers.GetCustomerDetailed(customerId);
            if (customer == null) return new ApiResponse { Message = "Customer not found", Status = 404 };

            if (customer.Ninverified) return new ApiResponse { Success = true, Message = "NIN already verified", Boolean = true };

            var youVerifyRequest = new Integration.Models.YouVerify.NinVerificationRequest
            {
                Id = request.Nin,
                IsSubjectConsent = true,
                Validations = new Integration.Models.YouVerify.IdentityValidations
                {
                    Data = new Integration.Models.YouVerify.IdentityDataValidation
                    {
                        FirstName = customer.FirstName,
                        LastName = customer.LastName,
                        DateOfBirth = customer.DateOfBirth?.ToString("yyyy-MM-dd")
                    }
                },
                Metadata = new Dictionary<string, string> { { "customerId", customerId.ToString() } }
            };

            var youVerifyResponse = await _youVerifyService.VerifyNin(youVerifyRequest);

            if (youVerifyResponse != null && youVerifyResponse.Success)
            {
                if (youVerifyResponse.Data?.Status == "found")
                {
                    // Ensure the data validation also passed
                    if (!youVerifyResponse.Data.AllValidationPassed)
                    {
                        var messages = youVerifyResponse.Data.Validations?.ValidationMessages ?? "Data validation failed.";
                        return new ApiResponse { Message = $"NIN details found but validation failed: {messages}", Status = 400 };
                    }

                    customer.Nin = request.Nin;
                    customer.Ninverified = true;
                    customer.NinverifiedAt = DateTime.UtcNow;
                    customer.ModifiedAt = DateTime.UtcNow;

                    await _uow.Customers.SaveCustomer(customer);

                    return new ApiResponse { Success = true, Message = "NIN verified successfully", Boolean = true, Data = youVerifyResponse.Data };
                }

                return new ApiResponse { Message = "NIN not found or invalid.", Status = 400 };
            }

            return new ApiResponse { Message = youVerifyResponse?.Message ?? "NIN verification failed", Status = 400 };
        }

        public async Task<ApiResponse> GetProfile(long userId)
        {
            var customer = await _uow.Customers.GetByUserId(userId);
            if (customer == null) return new ApiResponse { Message = "Profile not found", Status = 404 };

            var user = await _uow.Users.GetById(userId);
            
            var initials = (customer.FirstName?.Length > 0 ? customer.FirstName[0].ToString() : "") + 
                           (customer.LastName?.Length > 0 ? customer.LastName[0].ToString() : "");

            var profile = new
            {
                customer.FirstName,
                customer.LastName,
                Email = user?.Email,
                Phone = user?.PhoneNumber ?? customer.PhoneNumber,
                Avatar = initials,
                Tier = customer.Bvnverified ? "Tier 2 Investor" : "Tier 1 Investor"
            };

            return new ApiResponse { Success = true, Data = profile, Message = "Profile retrieved", Boolean = true };
        }

        public async Task<ApiResponse> UpdateProfile(long userId, UpdateProfileRequest request)
        {
            var customer = await _uow.Customers.GetByUserId(userId);
            if (customer == null) return new ApiResponse { Message = "Profile not found", Status = 404 };

            var user = await _uow.Users.GetById(userId);
            if (user == null) return new ApiResponse { Message = "User not found", Status = 404 };

            if (!string.IsNullOrEmpty(request.FirstName)) customer.FirstName = request.FirstName;
            if (!string.IsNullOrEmpty(request.LastName)) customer.LastName = request.LastName;
            
            if (!string.IsNullOrEmpty(request.Phone) && request.Phone != user.PhoneNumber)
            {
                var existingUser = await _uow.Users.GetByPhoneNumber(request.Phone);
                if (existingUser != null) return new ApiResponse { Message = "Phone number already in use.", Status = 400 };

                user.PhoneNumber = request.Phone;
                customer.PhoneNumber = request.Phone;
            }

            customer.ModifiedAt = DateTime.UtcNow;
            user.ModifiedAt = DateTime.UtcNow;

            await _uow.Customers.SaveCustomer(customer);
            await _uow.Users.SaveUser(user);

            var response = await GetProfile(userId);
            response.Message = "Profile updated successfully";
            return response;
        }
        public async Task<ApiResponse> GetGenders()
        {
            var genders = await BaseRepository().FindList<TblGender>(g => g.IsActive);
            return new ApiResponse { Success = true, Data = genders, Message = "Genders retrieved successfully", Boolean = true };
        }

        public async Task<ApiResponse> GetCustomerTypes()
        {
            var types = await BaseRepository().FindList<TblCustomerType>(t => true);
            return new ApiResponse { Success = true, Data = types, Message = "Customer types retrieved successfully", Boolean = true };
        }

        public async Task<ApiResponse> GetAddressDocTypes()
        {
            var types = await BaseRepository().FindList<TblAddressDocType>(t => t.IsActive);
            return new ApiResponse { Success = true, Data = types, Message = "Document types retrieved", Boolean = true };
        }

        public async Task<ApiResponse> InitiateAddressVerification(long customerId, AddressVerificationRequest request)
        {
            var response = new ApiResponse();
            
            try 
            {
                var customer = await _uow.Customers.GetByUserId(customerId);
                if (customer == null) return new ApiResponse { Message = "Customer not found", Status = 404 };

                // 1. Upload to Cloudinary
                var (url, publicId) = await _cloudinaryService.UploadImageAsync(request.File, "address_verifications");
                
                if (string.IsNullOrEmpty(url)) 
                {
                    return new ApiResponse { Message = "File upload failed", Status = 500 };
                }

                // 2. Extract contents using AI
                var extraction = await _docProcessor.ExtractAddressAsync(url);

                // 3. Save verification request
                var verification = new TblCustomerAddressVerification
                {
                    CustomerId = customer.CustomerId,
                    DocTypeId = request.DocTypeId,
                    DocumentUrl = url,
                    CloudinaryPublicId = publicId,
                    ExtractedAddress = extraction.Address,
                    ExtractedCity = extraction.City,
                    ExtractedState = extraction.State,
                    ExtractedFullText = extraction.FullText,
                    ConfidenceScore = extraction.ConfidenceScore,
                    Status = "Review", // System has extracted, now pending review
                    CreatedAt = DateTime.UtcNow
                };

                await BaseRepository().Insert(verification);
                await _uow.SaveChangesAsync();

                // 4. Log the action
                var log = new TblKycLog
                {
                    CustomerId = customer.CustomerId,
                    Type = "ADDR_VERIF",
                    Status = "Initiated",
                    Response = $"Extracted: {extraction.Address}, {extraction.City}",
                    CreatedAt = DateTime.UtcNow
                };
                await BaseRepository().Insert(log);
                await _uow.SaveChangesAsync();

                response.SetMessage("Document uploaded successfully. Extraction complete and pending review.", true);
                response.Data = new 
                {
                    verification.Id,
                    verification.DocumentUrl,
                    ExtractedAddress = extraction.Address,
                    ExtractedCity = extraction.City,
                    ExtractedState = extraction.State
                };
                
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Address verification initiation failed for customer {CustomerId}", customerId);
                return new ApiResponse { Message = "An error occurred during verification", Status = 500 };
            }
        }

        public async Task<ApiResponse> GetVerificationStatuses(long customerId)
        {
            try
            {
                var customer = await _uow.Customers.GetByUserId(customerId);
                if (customer == null) return new ApiResponse { Message = "Customer not found", Status = 404 };

                var addrVerif = await BaseRepository().FindEntity<TblCustomerAddressVerification>(v => v.CustomerId == customer.CustomerId);
                
                var statuses = new List<object>
                {
                    new { type = "BVN", label = "BVN Verification", status = customer.Bvnverified ? "verified" : (string.IsNullOrEmpty(customer.Bvn) ? "not_started" : "pending"), icon = "ri-bank-card-line" },
                    new { type = "NIN", label = "NIN Verification", status = customer.Ninverified ? "verified" : (string.IsNullOrEmpty(customer.Nin) ? "not_started" : "pending"), icon = "ri-shield-user-line" },
                    new { type = "Address", label = "Address Verification", status = MapAddressStatus(addrVerif?.Status), icon = "ri-map-pin-user-line", reason = addrVerif?.AdminNotes }
                };

                return new ApiResponse { Success = true, Data = statuses, Message = "Verification statuses retrieved", Boolean = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving verification statuses for customer {CustomerId}", customerId);
                return new ApiResponse { Message = "Error retrieving verification statuses", Status = 500 };
            }
        }

        private string MapAddressStatus(string? status)
        {
            if (string.IsNullOrEmpty(status)) return "not_started";
            status = status.ToLower();
            if (status == "review") return "pending";
            if (status == "approved") return "verified";
            return status;
        }

        public async Task<ApiResponse> GetCompanyBankDetails()
        {
            try
            {
                var profile = await BaseRepository().FindEntity<TblCompanyProfile>(_ => true);
                if (profile == null) return new ApiResponse { Message = "Company profile not found", Status = 404 };

                string bankName = "Unknown Bank";
                if (profile.BankId.HasValue)
                {
                    var bank = await BaseRepository().FindEntity<TblBank>(b => b.BankId == profile.BankId.Value);
                    if (bank != null)
                    {
                        bankName = bank.BankName;
                    }
                }

                var data = new
                {
                    AccountName = profile.CompanyName,
                    AccountNumber = profile.AccountNumber,
                    BankName = bankName
                };

                return new ApiResponse { Success = true, Boolean = true, Data = data, Message = "Bank details retrieved successfully" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving company bank details");
                return new ApiResponse { Message = "Error retrieving company bank details", Status = 500 };
            }
        }

        public async Task<ApiResponse> GetCorporateProfile(long userId)
        {
            var customer = await _uow.Customers.GetByUserId(userId);
            if (customer == null) return new ApiResponse { Message = "Profile not found", Status = 404 };

            var user = await _uow.Users.GetById(userId);

            var profile = new
            {
                CompanyName = customer.BusinessName,
                customer.RegistrationNumber,
                customer.DateOfIncorporation,
                customer.NatureOfBusiness,
                customer.Address,
                customer.EntityType,
                customer.OtherEntityType,
                Phone = customer.PhoneNumber,
                Tin = customer.TaxIdentificationNumber,
                Email = user?.Email,
                customer.AnnualTurnover,
                customer.SourceOfFunds,
                customer.ClientSegmentation
            };

            return new ApiResponse { Success = true, Data = profile, Message = "Corporate profile retrieved", Boolean = true };
        }

        public async Task<ApiResponse> UpdateCorporateProfile(long userId, UpdateCorporateProfileRequest request)
        {
            var customer = await _uow.Customers.GetByUserId(userId);
            if (customer == null) return new ApiResponse { Message = "Profile not found", Status = 404 };

            var user = await _uow.Users.GetById(userId);
            if (user == null) return new ApiResponse { Message = "User not found", Status = 404 };

            if (!string.IsNullOrEmpty(request.CompanyName))
            {
                customer.BusinessName = request.CompanyName;
                customer.FirstName = request.CompanyName;
                customer.LastName = request.CompanyName;
                user.FirstName = request.CompanyName;
                user.LastName = request.CompanyName;
            }
            if (!string.IsNullOrEmpty(request.RegistrationNumber)) customer.RegistrationNumber = request.RegistrationNumber;
            if (request.DateOfIncorporation.HasValue) customer.DateOfIncorporation = request.DateOfIncorporation;
            if (!string.IsNullOrEmpty(request.NatureOfBusiness)) customer.NatureOfBusiness = request.NatureOfBusiness;
            if (!string.IsNullOrEmpty(request.Address)) customer.Address = request.Address;
            if (!string.IsNullOrEmpty(request.EntityType)) customer.EntityType = request.EntityType;
            if (!string.IsNullOrEmpty(request.OtherEntityType)) customer.OtherEntityType = request.OtherEntityType;
            if (!string.IsNullOrEmpty(request.Tin)) customer.TaxIdentificationNumber = request.Tin;
            if (!string.IsNullOrEmpty(request.AnnualTurnover)) customer.AnnualTurnover = request.AnnualTurnover;
            if (!string.IsNullOrEmpty(request.SourceOfFunds)) customer.SourceOfFunds = request.SourceOfFunds;
            if (!string.IsNullOrEmpty(request.ClientSegmentation)) customer.ClientSegmentation = request.ClientSegmentation;

            if (!string.IsNullOrEmpty(request.Phone) && request.Phone != user.PhoneNumber)
            {
                var existingUser = await _uow.Users.GetByPhoneNumber(request.Phone);
                if (existingUser != null && existingUser.UserId != userId) return new ApiResponse { Message = "Phone number already in use.", Status = 400 };

                user.PhoneNumber = request.Phone;
                customer.PhoneNumber = request.Phone;
            }

            customer.ModifiedAt = DateTime.UtcNow;
            user.ModifiedAt = DateTime.UtcNow;

            await _uow.Customers.SaveCustomer(customer);
            await _uow.Users.SaveUser(user);

            var response = await GetCorporateProfile(userId);
            response.Message = "Corporate profile updated successfully";
            return response;
        }

        public async Task<ApiResponse> GetPrimaryContact(long userId)
        {
            var customer = await _uow.Customers.GetByUserId(userId);
            if (customer == null) return new ApiResponse { Message = "Customer not found", Status = 404 };

            var contact = await _uow.GenericRepository.FindEntity<TblCorporateContact>(c => c.CustomerId == customer.CustomerId && c.IsPrimary);
            
            if (contact == null)
            {
                // Return empty so frontend handles it gracefully
                return new ApiResponse { Success = true, Data = null, Message = "No primary contact found" };
            }

            var contactData = new
            {
                contact.FullName,
                contact.Email,
                Phone = contact.PhoneNumber
            };

            return new ApiResponse { Success = true, Data = contactData, Message = "Primary contact retrieved" };
        }

        public async Task<ApiResponse> UpdatePrimaryContact(long userId, UpdateCorporateContactRequest request)
        {
            var customer = await _uow.Customers.GetByUserId(userId);
            if (customer == null) return new ApiResponse { Message = "Customer not found", Status = 404 };

            var contact = await _uow.GenericRepository.FindEntity<TblCorporateContact>(c => c.CustomerId == customer.CustomerId && c.IsPrimary);

            if (contact == null)
            {
                contact = new TblCorporateContact
                {
                    CustomerId = customer.CustomerId,
                    IsPrimary = true,
                    FullName = request.FullName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber
                };
                await _uow.GenericRepository.Insert(contact);
            }
            else
            {
                contact.FullName = request.FullName;
                contact.Email = request.Email;
                contact.PhoneNumber = request.PhoneNumber;
                await _uow.GenericRepository.Update(contact);
            }

            return new ApiResponse { Success = true, Message = "Primary contact updated successfully" };
        }

        public async Task<ApiResponse> GetCorporateVerifications(long userId)
        {
            var customer = await _uow.Customers.GetByUserId(userId);
            if (customer == null) return new ApiResponse { Message = "Customer not found", Status = 404 };

            // Fetch the dynamic checklist from the database
            var checklistItems = await _uow.GenericRepository.FindList<TblVerificationItem>(v => v.IsActive && (v.TargetEntityTypes == null || v.TargetEntityTypes.Contains("Corporate")));
            checklistItems = checklistItems.OrderBy(v => v.DisplayOrder);

            // Fetch uploaded documents
            var documents = await _uow.GenericRepository.FindList<TblCorporateDocument>(d => d.CustomerId == customer.CustomerId);
            var docsDict = documents.ToDictionary(d => d.DocumentType, d => d.Status);

            // Fetch other dependencies for system verification
            var contact = await _uow.GenericRepository.FindEntity<TblCorporateContact>(c => c.CustomerId == customer.CustomerId && c.IsPrimary);
            
            // Check banks
            var hasBank = (await _uow.GenericRepository.FindList<TblCustomerBank>(p => p.CustomerId == customer.CustomerId)).Any();

            // 1. App Form logic
            bool appFormVerified = !string.IsNullOrEmpty(customer.BusinessName) && 
                                   !string.IsNullOrEmpty(customer.RegistrationNumber) &&
                                   contact != null;

            var verifications = new List<CorporateVerificationDto>();

            foreach (var item in checklistItems)
            {
                string status = "unverified";

                if (item.IsSystemVerified)
                {
                    switch (item.SystemRule)
                    {
                        case "CheckAppForm":
                            status = appFormVerified ? "verified" : "unverified";
                            break;
                        case "CheckBankLinked":
                            status = hasBank ? "verified" : "unverified";
                            break;
                        case "CheckSignatoryPhotos":
                            // Item 3: Verified if there is at least one signatory (passport photo is required to add one)
                            var hasSignatories = await _uow.GenericRepository.FindList<TblCorporateSignatory>(s => s.CustomerId == customer.CustomerId);
                            status = hasSignatories.Any() ? "verified" : "unverified";
                            break;
                        case "CheckDirectorsAdded":
                            // Item 6: Verified if there is at least one director
                            var hasDirectors = await _uow.GenericRepository.FindList<TblCorporateDirector>(d => d.CustomerId == customer.CustomerId);
                            status = hasDirectors.Any() ? "verified" : "unverified";
                            break;
                        case "CheckSignatoryDirectorsId":
                            // Item 8: Verified if there is at least one signatory and one director (ID document is required for both)
                            var sigs = await _uow.GenericRepository.FindList<TblCorporateSignatory>(s => s.CustomerId == customer.CustomerId);
                            var dirs = await _uow.GenericRepository.FindList<TblCorporateDirector>(d => d.CustomerId == customer.CustomerId);
                            status = (sigs.Any() && dirs.Any()) ? "verified" : "unverified";
                            break;
                        default:
                            status = "unverified";
                            break;
                    }
                }
                else
                {
                    status = docsDict.GetValueOrDefault(item.Type, "unverified").ToLower();
                }

                verifications.Add(new CorporateVerificationDto
                {
                    Name = item.Name,
                    Type = item.Type,
                    Status = status,
                    Icon = item.Icon ?? "ri-file-list-3-line",
                    RequiresUpload = item.RequiresUpload
                });
            }

            return new ApiResponse { Success = true, Data = verifications, Message = "Corporate verifications retrieved" };
        }

        public async Task<ApiResponse> UploadCorporateDocument(long userId, UploadCorporateDocumentRequest request)
        {
            var customer = await _uow.Customers.GetByUserId(userId);
            if (customer == null) return new ApiResponse { Message = "Customer not found", Status = 404 };

            var (url, publicId) = await _cloudinaryService.UploadImageAsync(request.File, "corporate_documents");
            if (string.IsNullOrEmpty(url)) return new ApiResponse { Message = "File upload failed", Status = 500 };

            var existingDoc = await _uow.GenericRepository.FindEntity<TblCorporateDocument>(d => d.CustomerId == customer.CustomerId && d.DocumentType == request.DocumentType);

            if (existingDoc != null)
            {
                existingDoc.FilePath = url;
                existingDoc.Status = "Pending";
                existingDoc.UploadedAt = DateTime.UtcNow;
                existingDoc.ReviewedAt = null;
                existingDoc.ReviewedByAdminId = null;
                await _uow.GenericRepository.Update(existingDoc);
            }
            else
            {
                var doc = new TblCorporateDocument
                {
                    CustomerId = customer.CustomerId,
                    DocumentType = request.DocumentType,
                    FilePath = url,
                    Status = "Pending",
                    UploadedAt = DateTime.UtcNow
                };
                await _uow.GenericRepository.Insert(doc);
            }

            return new ApiResponse { Success = true, Message = "Document uploaded successfully", Data = new { Url = url } };
        }

        public async Task<ApiResponse> AddCorporateSignatory(long userId, AddCorporateSignatoryRequest request)
        {
            try
            {
                var customer = await _uow.GenericRepository.FindEntity<TblCustomer>(c => c.UserId == userId);
                if (customer == null)
                    return new ApiResponse { Success = false, Message = "Customer not found", Status = 404 };

                // Upload files to Cloudinary
                var (passportUrl, _) = await _cloudinaryService.UploadImageAsync(request.PassportPhoto, "signatories/passports");
                var (signatureUrl, _) = await _cloudinaryService.UploadImageAsync(request.SignatureCard, "signatories/signatures");
                var (idDocUrl, _) = await _cloudinaryService.UploadImageAsync(request.IdentityDocument, "signatories/id_docs");

                if (string.IsNullOrEmpty(passportUrl) || string.IsNullOrEmpty(signatureUrl) || string.IsNullOrEmpty(idDocUrl))
                {
                    return new ApiResponse { Success = false, Message = "File upload failed", Status = 500 };
                }

                var dob = DateTime.Parse(request.DateOfBirth);

                var signatory = new TblCorporateSignatory
                {
                    CustomerId = customer.CustomerId,
                    Title = request.Title,
                    Surname = request.Surname,
                    FirstName = request.FirstName,
                    OtherNames = request.OtherNames,
                    Designation = request.Designation,
                    DateOfBirth = dob,
                    ResidentialAddress = request.ResidentialAddress,
                    BusinessEmail = request.BusinessEmail,
                    PhoneNumber = request.PhoneNumber,
                    Bvn = request.Bvn,
                    Nationality = request.Nationality,
                    Gender = request.Gender,
                    SigningClass = request.SigningClass,
                    IdentityType = request.IdentityType,
                    IdNumber = request.IdNumber,
                    IsPep = request.IsPep,
                    PassportPhotoUrl = passportUrl,
                    SignatureCardUrl = signatureUrl,
                    IdentityDocumentUrl = idDocUrl,
                    IsActive = true
                };

                await _uow.GenericRepository.Insert(signatory);

                return new ApiResponse
                {
                    Success = true,
                    Message = "Signatory added successfully",
                    Data = signatory
                };
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<ApiResponse> GetCorporateSignatories(long userId)
        {
            var customer = await _uow.GenericRepository.FindEntity<TblCustomer>(c => c.UserId == userId);
            if (customer == null)
                return new ApiResponse { Success = false, Message = "Customer not found", Status = 404 };

            var signatories = await _uow.GenericRepository.FindList<TblCorporateSignatory>(s => s.CustomerId == customer.CustomerId && !s.IsDeleted);

            return new ApiResponse
            {
                Success = true,
                Message = "Signatories retrieved",
                Data = signatories
            };
        }

        public async Task<ApiResponse> DeleteCorporateSignatory(long userId, int signatoryId)
        {
            var customer = await _uow.GenericRepository.FindEntity<TblCustomer>(c => c.UserId == userId);
            if (customer == null)
                return new ApiResponse { Success = false, Message = "Customer not found", Status = 404 };

            var signatory = await _uow.GenericRepository.FindEntity<TblCorporateSignatory>(s => s.SignatoryId == signatoryId && s.CustomerId == customer.CustomerId && !s.IsDeleted);
            if (signatory == null)
                return new ApiResponse { Success = false, Message = "Signatory not found", Status = 404 };

            signatory.IsDeleted = true;
            signatory.DeletedAt = DateTime.UtcNow;
            await _uow.GenericRepository.Update(signatory);

            return new ApiResponse
            {
                Success = true,
                Message = "Signatory removed successfully"
            };
        }
        public async Task<ApiResponse> AddCorporateDirector(long userId, AddCorporateDirectorRequest request)
        {
            var customer = await _uow.GenericRepository.FindEntity<TblCustomer>(c => c.UserId == userId);
            if (customer == null)
                return new ApiResponse { Success = false, Message = "Customer not found", Status = 404 };

            // Upload files to Cloudinary
            var (passportUrl, _) = await _cloudinaryService.UploadImageAsync(request.PassportPhoto, "directors/passports");
            var (signatureUrl, _) = await _cloudinaryService.UploadImageAsync(request.SignatureCard, "directors/signatures");
            var (idDocUrl, _) = await _cloudinaryService.UploadImageAsync(request.IdentityDocument, "directors/id_docs");

            if (string.IsNullOrEmpty(passportUrl) || string.IsNullOrEmpty(signatureUrl) || string.IsNullOrEmpty(idDocUrl))
            {
                return new ApiResponse { Success = false, Message = "File upload failed", Status = 500 };
            }

            var dob = DateTime.Parse(request.DateOfBirth);

            var director = new TblCorporateDirector
            {
                CustomerId = customer.CustomerId,
                Title = request.Title,
                Surname = request.Surname,
                FirstName = request.FirstName,
                OtherNames = request.OtherNames,
                Designation = request.Designation,
                DateOfBirth = dob,
                ResidentialAddress = request.ResidentialAddress,
                BusinessEmail = request.BusinessEmail,
                PhoneNumber = request.PhoneNumber,
                Bvn = request.Bvn,
                Nationality = request.Nationality,
                Gender = request.Gender,
                SigningClass = request.SigningClass,
                IdentityType = request.IdentityType,
                IdNumber = request.IdNumber,
                IsPep = request.IsPep,
                PassportPhotoUrl = passportUrl,
                SignatureCardUrl = signatureUrl,
                IdentityDocumentUrl = idDocUrl,
                IsActive = true
            };

            await _uow.GenericRepository.Insert(director);

            return new ApiResponse
            {
                Success = true,
                Message = "Director added successfully",
                Data = director
            };
        }

        public async Task<ApiResponse> GetCorporateDirectors(long userId)
        {
            var customer = await _uow.GenericRepository.FindEntity<TblCustomer>(c => c.UserId == userId);
            if (customer == null)
                return new ApiResponse { Success = false, Message = "Customer not found", Status = 404 };

            var directors = await _uow.GenericRepository.FindList<TblCorporateDirector>(s => s.CustomerId == customer.CustomerId && !s.IsDeleted);

            return new ApiResponse
            {
                Success = true,
                Message = "Directors retrieved",
                Data = directors
            };
        }

        public async Task<ApiResponse> DeleteCorporateDirector(long userId, int directorId)
        {
            var customer = await _uow.GenericRepository.FindEntity<TblCustomer>(c => c.UserId == userId);
            if (customer == null)
                return new ApiResponse { Success = false, Message = "Customer not found", Status = 404 };

            var director = await _uow.GenericRepository.FindEntity<TblCorporateDirector>(s => s.DirectorId == directorId && s.CustomerId == customer.CustomerId && !s.IsDeleted);
            if (director == null)
                return new ApiResponse { Success = false, Message = "Director not found", Status = 404 };

            director.IsDeleted = true;
            director.DeletedAt = DateTime.UtcNow;
            await _uow.GenericRepository.Update(director);

            return new ApiResponse
            {
                Success = true,
                Message = "Director removed successfully"
            };
        }

        public async Task<ApiResponse> GetNotifications(long userId)
        {
            var customer = await _uow.GenericRepository.FindEntity<TblCustomer>(c => c.UserId == userId);
            if (customer == null)
                return new ApiResponse { Success = false, Message = "Customer not found", Status = 404 };

            var notifications = await _uow.GenericRepository.AsQueryable<TblNotification>(n => n.CustomerId == customer.CustomerId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return new ApiResponse
            {
                Success = true,
                Data = notifications.Select(n => new
                {
                    n.NotificationId,
                    n.Title,
                    n.Message,
                    n.IsRead,
                    CreatedAt = n.CreatedAt.ToString("MMM dd, yyyy h:mm tt")
                }).ToList()
            };
        }

        public async Task<ApiResponse> MarkNotificationRead(long notificationId, long userId)
        {
            var customer = await _uow.GenericRepository.FindEntity<TblCustomer>(c => c.UserId == userId);
            if (customer == null)
                return new ApiResponse { Success = false, Message = "Customer not found", Status = 404 };

            var notif = await _uow.GenericRepository.FindEntity<TblNotification>(n => n.NotificationId == notificationId && n.CustomerId == customer.CustomerId);
            if (notif == null)
                return new ApiResponse { Success = false, Message = "Notification not found", Status = 404 };

            notif.IsRead = true;
            await _uow.GenericRepository.Update(notif);

            return new ApiResponse { Success = true, Message = "Notification marked as read" };
        }
    }
}
