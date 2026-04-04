using DogoFinance.BusinessLogic.Layer.Helpers;
using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.CustomerManagement.Interfaces;
using DogoFinance.DataAccess.Layer.Interfaces;
using DogoFinance.DataAccess.Layer.Models.Entities;
using DogoFinance.DataAccess.Layer.Repositories.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DogoFinance.CustomerManagement.Services
{
    public class BankService : DataRepository, IBankService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<BankService> _logger;

        public BankService(IUnitOfWork uow, ILogger<BankService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<ApiResponse> GetBanks()
        {
            try
            {
                var banks = await BaseRepository().FindList<TblBank>(b => b.IsActive);
                return new ApiResponse
                {
                    Success = true,
                    Message = "Banks retrieved",
                    Data = banks.Select(b => new { b.BankId, b.BankName, b.BankCode, b.LogoUrl })
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetBanks Failed");
                return new ApiResponse { Message = "Internal error retrieving banks", Status = 500 };
            }
        }

        public async Task<ApiResponse> GetCustomerBanks(long userId)
        {
            try
            {
                var customer = await _uow.Customers.GetByUserId(userId);
                if (customer == null) return new ApiResponse { Message = "Customer not found", Status = 404 };

                var customerBanks = await BaseRepository().FindList<TblCustomerBank>(cb => cb.CustomerId == customer.CustomerId && !cb.IsDeleted);
                
                // Join manually or use Include if configured
                var banks = await BaseRepository().FindList<TblBank>(b => customerBanks.Select(cb => cb.BankId).Contains(b.BankId));

                var result = customerBanks.Select(cb =>
                {
                    var bank = banks.FirstOrDefault(b => b.BankId == cb.BankId);
                    return new
                    {
                        cb.CustomerBankId,
                        cb.BankId,
                        BankName = bank?.BankName ?? "Unknown Bank",
                        BankLogo = bank?.LogoUrl,
                        cb.AccountNumber,
                        cb.AccountName,
                        cb.IsDefault
                    };
                });

                return new ApiResponse
                {
                    Success = true,
                    Message = "Withdrawal accounts retrieved",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCustomerBanks Failed for userId {UserId}", userId);
                return new ApiResponse { Message = "Internal error retrieving bank accounts", Status = 500 };
            }
        }

        public async Task<ApiResponse> AddCustomerBank(long userId, AddCustomerBankRequest request)
        {
            try
            {
                var customer = await _uow.Customers.GetByUserId(userId);
                if (customer == null) return new ApiResponse { Message = "Customer not found", Status = 404 };

                var user = await _uow.Users.GetById(userId);
                if (user == null) return new ApiResponse { Message = "User not found", Status = 404 };

                // Check for duplicate account number/bank combo
                var existing = await BaseRepository().FindEntity<TblCustomerBank>(cb => 
                    cb.CustomerId == customer.CustomerId && 
                    cb.BankId == request.BankId && 
                    cb.AccountNumber == request.AccountNumber &&
                    !cb.IsDeleted);

                if (existing != null)
                {
                    return new ApiResponse { Message = "This bank account is already added.", Status = 400 };
                }

                // If first bank account OR request says default, handle defaults
                var hasBanks = (await BaseRepository().FindList<TblCustomerBank>(cb => cb.CustomerId == customer.CustomerId && !cb.IsDeleted)).Any();
                bool isDefault = request.IsDefault || !hasBanks;

                if (isDefault)
                {
                    // Unset others
                    var others = await BaseRepository().FindList<TblCustomerBank>(cb => cb.CustomerId == customer.CustomerId && cb.IsDefault);
                    foreach (var o in others)
                    {
                        o.IsDefault = false;
                        await BaseRepository().Update(o);
                    }
                }

                var customerBank = new TblCustomerBank
                {
                    CustomerId = customer.CustomerId,
                    BankId = request.BankId,
                    AccountNumber = request.AccountNumber,
                    AccountName = request.AccountName,
                    IsDefault = isDefault,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                await BaseRepository().Insert(customerBank);
                return new ApiResponse { Success = true, Message = "Bank account added successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddCustomerBank Failed for userId {UserId}", userId);
                return new ApiResponse { Message = "Internal error adding bank account.", Status = 500 };
            }
        }

        public async Task<ApiResponse> DeleteCustomerBank(long userId, long customerBankId)
        {
            try
            {
                var customer = await _uow.Customers.GetByUserId(userId);
                if (customer == null) return new ApiResponse { Message = "Customer not found", Status = 404 };

                var cb = await BaseRepository().FindEntity<TblCustomerBank>(customerBankId);
                if (cb == null || cb.CustomerId != customer.CustomerId)
                {
                    return new ApiResponse { Message = "Bank account not found or access denied.", Status = 404 };
                }

                cb.IsDeleted = true;
                await BaseRepository().Update(cb);

                // If we deleted the default, set another one as default if possible
                if (cb.IsDefault)
                {
                    var another = (await BaseRepository().FindList<TblCustomerBank>(b => b.CustomerId == customer.CustomerId && !b.IsDeleted)).FirstOrDefault();
                    if (another != null)
                    {
                        another.IsDefault = true;
                        await BaseRepository().Update(another);
                    }
                }

                return new ApiResponse { Success = true, Message = "Bank account removed successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteCustomerBank Failed for customerBankId {Id}", customerBankId);
                return new ApiResponse { Message = "Internal error removing bank account.", Status = 500 };
            }
        }

        public async Task<ApiResponse> SetDefaultBank(long userId, long customerBankId)
        {
            try
            {
                var customer = await _uow.Customers.GetByUserId(userId);
                if (customer == null) return new ApiResponse { Message = "Customer not found", Status = 404 };

                var target = await BaseRepository().FindEntity<TblCustomerBank>(customerBankId);
                if (target == null || target.CustomerId != customer.CustomerId)
                {
                    return new ApiResponse { Message = "Bank account not found or access denied.", Status = 404 };
                }

                var others = await BaseRepository().FindList<TblCustomerBank>(cb => cb.CustomerId == customer.CustomerId && cb.IsDefault);
                foreach (var o in others)
                {
                    o.IsDefault = false;
                    await BaseRepository().Update(o);
                }

                target.IsDefault = true;
                await BaseRepository().Update(target);

                return new ApiResponse { Success = true, Message = "Default bank account updated." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SetDefaultBank Failed for customerBankId {Id}", customerBankId);
                return new ApiResponse { Message = "Internal error.", Status = 500 };
            }
        }
    }
}
