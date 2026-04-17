using DogoFinance.Authentication.Interfaces;
using DogoFinance.BusinessLogic.Layer.Helpers;
using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.DataAccess.Layer.Interfaces;
using DogoFinance.DataAccess.Layer.Models.Entities;
using DogoFinance.DataAccess.Layer.Repositories.Base;
using DogoFinance.Integration.Interfaces;
using Microsoft.Extensions.Logging;

namespace DogoFinance.Authentication.Services
{
    public class PinService : DataRepository, IPinService
    {
        private readonly IUnitOfWork _uow;
        private readonly IEmailService _emailService;
        private readonly ILogger<PinService> _logger;

        public PinService(IUnitOfWork uow, IEmailService emailService, ILogger<PinService> logger)
        {
            _uow = uow;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<ApiResponse> SetTransactionPin(long userId, SetPinRequest request)
        {
            var response = new ApiResponse();
            try
            {
                var user = await _uow.Users.GetById(userId);
                if (user == null) return new ApiResponse { Message = "User not found", Status = 404 };

                if (user.IsPinSet)
                {
                    response.SetError("Transaction PIN is already setup.", 400);
                    return response;
                }

                var (hash, salt) = HashHelper.CreateHash(request.Pin);
                user.TransactionPinHash = hash;
                user.TransactionPinSalt = salt;
                user.IsPinSet = true;
                user.Is2faEnabled = true;
                user.LastPinChangeDate = DateTime.UtcNow;

                await _uow.Users.SaveUser(user);
                response.SetMessage("Transaction PIN setup successfully.", true, new { IsPinSet = true });
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SetTransactionPin failed for userId {UserId}", userId);
                response.SetError("Internal error during PIN setup.", 500);
                return response;
            }
        }

        public async Task<ApiResponse> ChangeTransactionPin(long userId, ChangePinRequest request)
        {
            var response = new ApiResponse();
            try
            {
                var user = await _uow.Users.GetById(userId);
                if (user == null) return new ApiResponse { Message = "User not found", Status = 404 };

                if (!user.IsPinSet) return new ApiResponse { Message = "PIN not setup", Status = 400 };

                if (!HashHelper.VerifyHash(request.OldPin, user.TransactionPinHash!, user.TransactionPinSalt!))
                {
                    response.SetError("Incorrect current PIN.", 400);
                    return response;
                }

                var (hash, salt) = HashHelper.CreateHash(request.NewPin);
                user.TransactionPinHash = hash;
                user.TransactionPinSalt = salt;
                user.LastPinChangeDate = DateTime.UtcNow;

                await _uow.Users.SaveUser(user);
                response.SetMessage("PIN changed successfully", true);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChangeTransactionPin failed for userId {UserId}", userId);
                response.SetError("Internal error changing PIN.", 500);
                return response;
            }
        }

        public async Task<ApiResponse> ForgotPin(ForgotPinRequest request)
        {
            var response = new ApiResponse();
            try
            {
                var user = await _uow.Users.GetByEmail(request.Email);
                if (user == null)
                {
                    response.SetMessage("If an account exists, a reset link has been sent.", true);
                    return response;
                }

                var token = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();
                var reset = new TblPinReset
                {
                    UserId = user.UserId,
                    ResetCode = token,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(30),
                    IsUsed = false,
                    CreatedAt = DateTime.UtcNow
                };

                await BaseRepository().Insert(reset);

                var customer = await BaseRepository().FindEntity<TblCustomer>(c => c.UserId == user.UserId);
                var placeholders = new Dictionary<string, string>
                {
                    { "FirstName", customer?.FirstName ?? "DogoFinance User" },
                    { "ResetCode", token }
                };

                await _emailService.SendTemplateEmail(request.Email, "Transaction PIN Reset - DogoFinance", "PinReset", placeholders);

                response.SetMessage("Reset code sent to your email.", true);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ForgotPin failed for email {Email}", request.Email);
                response.SetError("Internal error processing PIN reset.", 500);
                return response;
            }
        }

        public async Task<ApiResponse> ResetPin(ResetPinRequest request)
        {
            var response = new ApiResponse();
            var db = await BaseRepository().BeginTrans();

            try
            {
                var reset = await BaseRepository().FindEntity<TblPinReset>(r => r.ResetCode == request.Token && !r.IsUsed);
                if (reset == null || reset.ExpiresAt < DateTime.UtcNow)
                {
                    response.SetError("Invalid or expired reset code.", 400);
                    return response;
                }

                var user = await BaseRepository().FindEntity<TblUser>(reset.UserId);
                if (user == null) throw new Exception("User not found");

                var (hash, salt) = HashHelper.CreateHash(request.NewPin);
                user.TransactionPinHash = hash;
                user.TransactionPinSalt = salt;
                user.IsPinSet = true;
                user.Is2faEnabled = true;
                user.LastPinChangeDate = DateTime.UtcNow;

                await _uow.Users.SaveUser(user);

                reset.IsUsed = true;
                await BaseRepository().Update(reset);

                await BaseRepository().CommitTrans();
                response.SetMessage("PIN reset successfully", true);
                return response;
            }
            catch (Exception ex)
            {
                await BaseRepository().RollbackTrans();
                _logger.LogError(ex, "ResetPin Failed");
                response.SetError("Internal error.", 500);
                return response;
            }
        }

        public async Task<ApiResponse> Toggle2fa(long userId, bool status)
        {
            try
            {
                var user = await _uow.Users.GetById(userId);
                if (user == null) return new ApiResponse { Message = "User not found", Status = 404 };

                if (status && !user.IsPinSet)
                {
                    return new ApiResponse { Message = "You must set a transaction PIN before enabling 2FA.", Status = 400 };
                }

                user.Is2faEnabled = status;
                await _uow.Users.SaveUser(user);

                string msg = status ? "Two-Factor Authentication (2FA) enabled successfully." : "Two-Factor Authentication (2FA) disabled successfully.";
                return new ApiResponse { Success = true, Boolean = true, Status = 200, Message = msg };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Toggle2fa failed for userId {UserId}", userId);
                return new ApiResponse { Message = "Internal error toggling 2FA.", Status = 500 };
            }
        }
    }
}
