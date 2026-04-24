using DogoFinance.AdminManagement.Interfaces;
using DogoFinance.BusinessLogic.Layer.Helpers;
using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.DataAccess.Layer.Interfaces;
using DogoFinance.DataAccess.Layer.Models.Entities;
using DogoFinance.DataAccess.Layer.Repositories.Base;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using DogoFinance.Integration.Interfaces;
using DogoFinance.Integration.Models.Monnify;
using DogoFinance.DataAccess.Layer.Models.Constants;

namespace DogoFinance.AdminManagement.Services
{
    public class AdminService : DataRepository, IAdminService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<AdminService> _logger;
        private readonly IMonnifyService _monnifyService;

        public AdminService(IUnitOfWork uow, ILogger<AdminService> logger, IMonnifyService monnifyService)
        {
            _uow = uow;
            _logger = logger;
            _monnifyService = monnifyService;
            
            // Ensure child repositories share the same DB context for transaction safety
            SetSharedRepository(_uow.GenericRepository);
        }

        private async Task LogLedger(long transactionId, long walletId, int entryType, decimal amount, decimal balanceAfter, string narration)
        {
            var entry = new TblLedger
            {
                TransactionId = transactionId,
                WalletId = walletId,
                EntryType = entryType,
                Amount = amount,
                BalanceAfter = balanceAfter,
                Narration = narration,
                CreatedAt = DateTime.UtcNow
            };
            await _uow.Ledgers.CreateEntry(entry);
        }

        public async Task<ApiResponse> CreateAdmin(SignUpRequest request, int roleId)
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

                var defaultPassword = "StaffPass1234!"; // As requested by USER
                var (hash, salt) = HashHelper.CreateHash(string.IsNullOrWhiteSpace(request.Password) ? defaultPassword : request.Password);

                var user = new TblUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PasswordHash = hash,
                    Salt = salt,
                    IsActive = true, // Admins created by super admin are active by default
                    IsLocked = false,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false,
                    IsSystemUser = true
                };

                await _uow.Users.SaveUser(user);

                // Assign Role
                var userRole = new TblUserRole
                {
                    UserId = user.UserId,
                    RoleId = roleId
                };
                await BaseRepository().Insert(userRole);

                await BaseRepository().CommitTrans();
                response.SetMessage("Admin user created successfully", true);
                return response;
            }
            catch (Exception ex)
            {
                await BaseRepository().RollbackTrans();
                _logger.LogError(ex, "CreateAdmin Failed");
                response.SetError("Failed to create admin.", 500);
                return response;
            }
        }

        public async Task<ApiResponse> UpdateAdmin(long userId, SignUpRequest request, int roleId)
        {
            var response = new ApiResponse();
            try
            {
                var user = await BaseRepository().FindEntity<TblUser>(u => u.UserId == userId);
                if (user == null)
                {
                    response.SetError("User not found.", 404);
                    return response;
                }

                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.Email = request.Email;
                user.PhoneNumber = request.PhoneNumber;
                user.ModifiedAt = DateTime.UtcNow;

                await BaseRepository().Update(user);

                // Update Role
                var userRole = await BaseRepository().FindEntity<TblUserRole>(ur => ur.UserId == userId);
                if (userRole != null)
                {
                    userRole.RoleId = roleId;
                    await BaseRepository().Update(userRole);
                }
                else
                {
                    userRole = new TblUserRole { UserId = userId, RoleId = roleId };
                    await BaseRepository().Insert(userRole);
                }

                response.SetMessage("User updated successfully.", 200);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAdmin Failed");
                response.SetError("Failed to update admin.", 500);
                return response;
            }
        }

        public async Task<ApiResponse> GetAdmins()
        {
            var admins = await BaseRepository().AsQueryable<TblUser>(u => u.IsSystemUser)
                .Include(u => u.TblUserRoles)
                .ThenInclude(ur => ur.Role)
                .ToListAsync();

            var result = admins.Select(u => new
            {
                UserId = u.UserId,
                UserName = u.UserName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Role = u.TblUserRoles.FirstOrDefault()?.Role?.Name ?? "Admin",
                IsActive = u.IsActive,
                IsLocked = u.IsLocked,
                CreatedAt = u.CreatedAt
            }).ToList();

            return new ApiResponse { Success = true, Data = result, Message = "Admins retrieved" };
        }

        public async Task<ApiResponse> ListClients()
        {
            var customers = await BaseRepository().AsQueryable<TblCustomer>(c => true)
                .Include(c => c.User)
                .Include(c => c.TblWallets)
                .ToListAsync();
            var result = customers.Select(c => new
            {
                Id = "C" + c.CustomerId.ToString("D3"),
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.User?.Email ?? "N/A", // This depends on user being loaded
                Phone = c.PhoneNumber,
                Status = c.User?.IsActive == false ? "Pending KYC" : (c.Ninverified == true && c.Ninverified == true ? "Active" : "Pending KYC"),
                KycLevel = c.Ninverified == true && c.Bvnverified == true ? "Level 3 - Verified" : (c.Bvnverified == true ? "Level 2 - Partial" : "Level 1 - Basic"),
                AccountBalance = c.TblWallets.FirstOrDefault()?.Balance ?? 0, // Get balance from TblWallet
                DateJoined = c.CreatedAt.ToString("MM/dd/yyyy")
            }).ToList();

            return new ApiResponse { Success = true, Data = result, Message = "Clients retrieved" };
        }

        // --- ROLES ---
        public async Task<ApiResponse> GetRoles()
        {
            var roles = await BaseRepository().FindList<TblRole>(r => r.Id != 3);
            return new ApiResponse { Success = true, Data = roles, Message = "Roles retrieved" };
        }

        public async Task<ApiResponse> SaveRole(TblRole role)
        {
            if (role.Id > 0)
            {
                await BaseRepository().Update(role);
            }
            else
            {
                await BaseRepository().Insert(role);
            }
            return new ApiResponse { Success = true, Message = "Role saved successfully" };
        }

        public async Task<ApiResponse> DeleteRole(int id)
        {
            var role = await BaseRepository().FindEntity<TblRole>(r => r.Id == id);
            if (role == null) return new ApiResponse { Message = "Role not found", Status = 404 };
            await BaseRepository().Delete(role);
            return new ApiResponse { Success = true, Message = "Role deleted successfully" };
        }

        // --- ACCESS RIGHTS ---
        public async Task<ApiResponse> GetAccessRightsHierarchy(int roleId)
        {
            var modules = await BaseRepository().FindList<TblModule>(m => true);
            var allAccess = await BaseRepository().FindList<TblAccessRight>(ar => true);
            var roleAccess = (await BaseRepository().FindList<TblRoleAccessRight>(ra => ra.RoleId == roleId)).Select(ra => ra.AccessRightId).ToList();

            var hierarchy = modules.Select(m => new
            {
                m.Id,
                m.Name,
                m.Icon,
                m.Description,
                Permissions = allAccess.Where(ar => ar.ModuleId == m.Id).Select(ar => new
                {
                    ar.Id,
                    ar.Name,
                    ar.Label,
                    IsSelected = roleAccess.Contains(ar.Id)
                }).ToList()
            }).ToList();

            return new ApiResponse { Success = true, Data = hierarchy, Message = "Access mapping retrieved" };
        }

        public async Task<ApiResponse> UpdateRoleAccessRights(int roleId, List<int> accessRightIds)
        {
            var db = await BaseRepository().BeginTrans();
            try
            {
                // Clear existing
                var existing = await BaseRepository().FindList<TblRoleAccessRight>(ra => ra.RoleId == roleId);
                foreach (var ra in existing)
                {
                    await BaseRepository().Delete(ra);
                }

                // Add new
                foreach (var arId in accessRightIds)
                {
                    await BaseRepository().Insert(new TblRoleAccessRight { RoleId = roleId, AccessRightId = arId });
                }

                await BaseRepository().CommitTrans();
                return new ApiResponse { Success = true, Message = "Access rights updated" };
            }
            catch (Exception ex)
            {
                await BaseRepository().RollbackTrans();
                _logger.LogError(ex, "UpdateRoleAccessRights failed");
                return new ApiResponse { Message = "Failed to update access rights", Status = 500 };
            }
        }

        public async Task<ApiResponse> ListAddressVerifications(string? status)
        {
            var query = BaseRepository().AsQueryable<TblCustomerAddressVerification>(v => true)
                .Include(v => v.Customer)
                .Include(v => v.DocType)
                .OrderByDescending(v => v.CreatedAt);

            if (!string.IsNullOrEmpty(status))
            {
                query = (IOrderedQueryable<TblCustomerAddressVerification>)query.Where(v => v.Status == status);
            }

            var verifications = await query.ToListAsync();

            var result = verifications.Select(v => new
            {
                v.Id,
                CustomerName = v.Customer.FirstName + " " + v.Customer.LastName,
                CustomerCode = "CUST-" + v.Customer.CustomerId.ToString("D4"),
                DocumentType = v.DocType.Name,
                DateSubmitted = v.CreatedAt?.ToString("yyyy-MM-dd HH:mm"),
                Status = v.Status,
                DocumentUrl = v.DocumentUrl,
                ExtractedAddress = v.ExtractedAddress,
                ExtractedCity = v.ExtractedCity,
                ExtractedState = v.ExtractedState,
                ExtractedFullText = v.ExtractedFullText,
                ConfidenceScore = v.ConfidenceScore
            }).ToList();

            return new ApiResponse { Success = true, Data = result, Message = "Verifications retrieved" };
        }

        public async Task<ApiResponse> ReviewAddressVerification(AdminAddressReviewRequest request, long adminUserId)
        {
            var db = await BaseRepository().BeginTrans();
            try
            {
                var verif = await BaseRepository().FindEntity<TblCustomerAddressVerification>(v => v.Id == request.VerificationId);
                if (verif == null) return new ApiResponse { Message = "Verification request not found", Status = 404 };

                verif.Status = request.Approved ? "Approved" : "Rejected";
                verif.AdminNotes = request.AdminNotes;
                verif.ReviewedAt = DateTime.UtcNow;
                verif.ReviewedBy = adminUserId;

                await BaseRepository().Update(verif);

                if (request.Approved)
                {
                    // Update core customer profile with verified address
                    var customer = await _uow.Customers.GetCustomerDetailed(verif.CustomerId);
                    if (customer != null)
                    {
                        customer.Address = request.CorrectedAddress ?? verif.ExtractedAddress;
                        customer.City = request.CorrectedCity ?? verif.ExtractedCity;
                        customer.State = request.CorrectedState ?? verif.ExtractedState;
                        customer.ModifiedAt = DateTime.UtcNow;
                        customer.ModifiedBy = adminUserId;

                        // Optionally update overall KYC Status
                        if (customer.Bvnverified && customer.Ninverified)
                        {
                            customer.Kycstatus = 3; // Full Verified
                            customer.KycverifiedAt = DateTime.UtcNow;
                        }

                        await _uow.Customers.SaveCustomer(customer);
                    }
                }

                await BaseRepository().CommitTrans();
                return new ApiResponse { Success = true, Message = $"Verification {verif.Status} successfully" };
            }
            catch (Exception ex)
            {
                await BaseRepository().RollbackTrans();
                _logger.LogError(ex, "ReviewAddressVerification failed");
                return new ApiResponse { Message = "Failed to process review", Status = 500 };
            }
        }
        public async Task<ApiResponse> GetActivePortfolios()
        {
            var response = new ApiResponse();
            try
            {
                var activePortfolios = await BaseRepository().AsQueryable<TblCustomerPortfolio>(p => p.Units > 0)
                    .Include(p => p.Customer)
                    .ThenInclude(c => c.User)
                    .Include(p => p.Portfolio)
                    .ToListAsync();

                var results = new List<object>();

                foreach (var p in activePortfolios)
                {
                    // Get Latest NAV
                    var latestPrice = await BaseRepository().AsQueryable<TblPortfolioPrice>(pt => pt.PortfolioId == p.PortfolioId)
                        .OrderByDescending(pt => pt.PriceDate)
                        .FirstOrDefaultAsync();

                    decimal currentNav = latestPrice?.NAV ?? 1.0m;
                    decimal currentValue = Math.Round(p.Units * currentNav, 2);
                    decimal growth = p.InvestedAmount > 0 ? Math.Round(((currentValue - p.InvestedAmount) / p.InvestedAmount) * 100, 2) : 0;

                    // Placeholder for holdings as PortfolioInstrument is not currently in use
                    var holdings = new List<object>();

                    results.Add(new
                    {
                        Id = p.Id,
                        PortfolioId = p.PortfolioId,
                        PortfolioName = p.Portfolio.Name,
                        TotalInvested = Math.Round(p.InvestedAmount, 2),
                        CurrentValue = currentValue,
                        GrowthPercentage = growth,
                        Status = "active",
                        InvestedAt = p.CreatedAt.ToString("yyyy-MM-dd"),
                        ClientName = $"{p.Customer.FirstName} {p.Customer.LastName}",
                        ClientEmail = p.Customer.User?.Email ?? "N/A",
                        ClientInitials = (p.Customer.FirstName?[0].ToString() ?? "") + (p.Customer.LastName?[0].ToString() ?? ""),
                        Holdings = holdings
                    });
                }

                response.SetMessage("Active portfolios retrieved", true, results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetActivePortfolios Failed");
                response.SetError("Failed to retrieve portfolios", 500);
            }
            return response;
        }

        public async Task<ApiResponse> GetSystemSettings()
        {
            var settings = await BaseRepository().AsQueryable<TblSystemSetting>(s => true).FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new TblSystemSetting { SessionTimeoutInMinutes = 15, WithdrawalAutoThreshold = 50000, CreatedAt = DateTime.UtcNow };
                await BaseRepository().Insert(settings);
            }
            return new ApiResponse { Success = true, Data = settings, Message = "Settings retrieved" };
        }

        public async Task<ApiResponse> UpdateSystemSettings(TblSystemSetting settings)
        {
            try
            {
                var existing = await BaseRepository().AsQueryable<TblSystemSetting>(s => true).FirstOrDefaultAsync();
                if (existing != null)
                {
                    existing.SessionTimeoutInMinutes = settings.SessionTimeoutInMinutes;
                    existing.WithdrawalAutoThreshold = settings.WithdrawalAutoThreshold;
                    existing.UpdatedAt = DateTime.UtcNow;
                    await BaseRepository().Update(existing);
                }
                else
                {
                    settings.CreatedAt = DateTime.UtcNow;
                    await BaseRepository().Insert(settings);
                }
                return new ApiResponse { Success = true, Message = "Settings updated successfully" };
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<ApiResponse> ListWithdrawalRequests(string? status)
        {
            var query = BaseRepository().AsQueryable<TblWithdrawalRequest>(r => true)
                .Include(r => r.Customer)
                .ThenInclude(c => c.User)
                .OrderByDescending(r => r.InitiatedAt);

            if (!string.IsNullOrEmpty(status))
            {
                query = (IOrderedQueryable<TblWithdrawalRequest>)query.Where(r => r.Status == status);
            }

            var requests = await query.ToListAsync();
            var banks = await BaseRepository().FindList<TblBank>(b => true);

            var result = requests.Select(r => new
            {
                r.Id,
                CustomerName = r.Customer.FirstName + " " + r.Customer.LastName,
                Email = r.Customer.User?.Email ?? "N/A",
                r.Amount,
                r.Status,
                r.Reference,
                BankName = banks.FirstOrDefault(b => b.BankCode == r.BankCode)?.BankName ?? r.BankCode,
                r.BankCode,
                r.AccountNumber,
                r.Narration,
                InitiatedAt = r.InitiatedAt.ToString("yyyy-MM-dd HH:mm"),
                r.AdminNotes
            }).ToList();

            return new ApiResponse { Success = true, Data = result, Message = "Withdrawal requests retrieved" };
        }

        public async Task<ApiResponse> ReviewWithdrawalRequest(AdminWithdrawalReviewRequest request, long adminUserId)
        {
            var db = await BaseRepository().BeginTrans();
            try
            {
                var withdrawalReq = await BaseRepository().FindEntity<TblWithdrawalRequest>(r => r.Id == request.RequestId);
                if (withdrawalReq == null) return new ApiResponse { Message = "Request not found", Status = 404 };
                if (withdrawalReq.Status != "Pending") return new ApiResponse { Message = "Request has already been processed", Status = 400 };

                withdrawalReq.Status = request.Approved ? "Approved" : "Rejected";
                withdrawalReq.AdminNotes = request.AdminNotes;
                withdrawalReq.ReviewedAt = DateTime.UtcNow;
                withdrawalReq.ReviewedByAdminId = adminUserId;

                await BaseRepository().Update(withdrawalReq);

                var transaction = await BaseRepository().FindEntity<TblTransaction>(t => t.Reference == withdrawalReq.Reference);

                if (request.Approved)
                {
                    // Payment will happen outside the system. Impact needed tables and customer's wallet.
                    
                    var wallet = await _uow.Wallets.GetByCustomerId(withdrawalReq.CustomerId);
                    if (wallet == null) throw new Exception("Wallet not found for customer.");
                    if (wallet.Balance < withdrawalReq.Amount) throw new Exception("Insufficient balance in customer's wallet.");

                    // 1. Debit Wallet
                    wallet.Balance -= withdrawalReq.Amount;
                    await _uow.Wallets.UpdateWallet(wallet);

                    // 2. Update Transaction
                    if (transaction != null)
                    {
                        transaction.Status = 1; // SUCCESS
                        transaction.Narration = transaction.Narration?.Replace(" (Pending Approval)", "") ?? "Fund Withdrawal";
                        await BaseRepository().Update(transaction);
                        
                        // 3. Ledger Logging
                        await LogLedger(transaction.TransactionId, wallet.WalletId, EntryType.DEBIT, -withdrawalReq.Amount, wallet.Balance, "Withdrawal (Approved & Processed)");
                    }

                    withdrawalReq.Status = "Completed";
                    await BaseRepository().Update(withdrawalReq);
                }
                else
                {
                    // REJECTED
                    // Since funds were NOT debited at initiation, no refund is needed here.
                    withdrawalReq.Status = "Rejected";
                    await BaseRepository().Update(withdrawalReq);

                    if (transaction != null)
                    {
                        transaction.Status = 2; // FAILED/REJECTED
                        transaction.Narration = transaction.Narration?.Replace(" (Pending Approval)", " (Rejected by Admin)") ?? "Withdrawal Rejected";
                        await BaseRepository().Update(transaction);
                    }
                }

                await BaseRepository().CommitTrans();
                return new ApiResponse { Success = true, Message = $"Withdrawal {withdrawalReq.Status} successfully" };
            }
            catch (Exception ex)
            {
                await BaseRepository().RollbackTrans();
                _logger.LogError(ex, "ReviewWithdrawalRequest failed");
                return new ApiResponse { Message = ex.Message, Status = 500 };
            }
        }

        public async Task<ApiResponse> ListLiquidationRequests(int? status)
        {
            var query = BaseRepository().AsQueryable<TblLiquidationRequest>(r => true)
                .Include(r => r.Customer)
                .ThenInclude(c => c.User)
                .Include(r => r.Portfolio)
                .OrderByDescending(r => r.CreatedAt);

            if (status.HasValue)
            {
                query = (IOrderedQueryable<TblLiquidationRequest>)query.Where(r => r.Status == status.Value);
            }

            var requests = await query.ToListAsync();

            var result = requests.Select(r => new
            {
                r.Id,
                CustomerName = r.Customer.FirstName + " " + r.Customer.LastName,
                Email = r.Customer.User?.Email ?? "N/A",
                PortfolioName = r.Portfolio.Name,
                r.UnitsRequested,
                r.GrossAmount,
                r.ExitFeeApplied,
                r.NetPayableAmount,
                Status = r.Status,
                StatusName = Enum.GetName(typeof(LiquidationStatus), r.Status) ?? r.Status.ToString(),
                CreatedAt = r.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                ExpectedReleaseDate = r.ExpectedReleaseDate?.ToString("yyyy-MM-dd"),
                r.AdminNotes
            }).ToList();

            return new ApiResponse { Success = true, Data = result, Message = "Liquidation requests retrieved" };
        }

        public async Task<ApiResponse> ReviewLiquidationRequest(AdminLiquidationReviewRequest request, long adminUserId)
        {
            var db = await BaseRepository().BeginTrans();
            try
            {
                var liqReq = await BaseRepository().FindEntity<TblLiquidationRequest>(r => r.Id == request.RequestId);
                if (liqReq == null) return new ApiResponse { Message = "Request not found", Status = 404 };
                if (liqReq.Status != LiquidationStatus.PENDING_APPROVAL && liqReq.Status != LiquidationStatus.PENDING_NOTICE) 
                    return new ApiResponse { Message = "Request has already been processed", Status = 400 };

                liqReq.Status = request.Approved ? LiquidationStatus.COMPLETED : LiquidationStatus.REJECTED;
                liqReq.AdminNotes = request.AdminNotes;
                liqReq.ReviewedAt = DateTime.UtcNow;
                liqReq.ReviewedByAdminId = adminUserId;

                await BaseRepository().Update(liqReq);

                if (request.Approved)
                {
                    // Credit Wallet
                    var wallet = await _uow.Wallets.GetByCustomerId(liqReq.CustomerId);
                    if (wallet == null) throw new Exception("Wallet not found for customer.");

                    wallet.Balance += liqReq.NetPayableAmount;
                    await _uow.Wallets.UpdateWallet(wallet);

                    // Create Core Transaction Record
                    var mainTransaction = new TblTransaction
                    {
                        Reference = $"LIQ_APP_{DateTime.UtcNow.Ticks}",
                        TransactionType = TransactionType.LIQUIDATION,
                        Amount = liqReq.NetPayableAmount,
                        Status = 1, // SUCCESS
                        Narration = $"Liquidation from {liqReq.Portfolio?.Name ?? "Portfolio"} (Approved)",
                        CreatedAt = DateTime.UtcNow
                    };
                    await _uow.Transactions.CreateTransaction(mainTransaction);

                    // Log to Ledger
                    await LogLedger(mainTransaction.TransactionId, wallet.WalletId, EntryType.CREDIT, liqReq.NetPayableAmount, wallet.Balance, $"Portfolio Liquidation Approved: {liqReq.Portfolio?.Name}");
                }
                else
                {
                    // REJECTED: Return units to customer portfolio
                    var userPortfolio = await _uow.Portfolios.GetCustomerPortfolio(liqReq.CustomerId, liqReq.PortfolioId);
                    if (userPortfolio != null)
                    {
                        userPortfolio.Units += liqReq.UnitsRequested;
                        // Approximate the invested amount back? 
                        // Since we deducted proportional cost basis, we should ideally track it.
                        // For now, let's just add units back.
                        await _uow.Portfolios.SaveCustomerPortfolio(userPortfolio);
                    }
                }

                await BaseRepository().CommitTrans();
                return new ApiResponse { Success = true, Message = $"Liquidation { (request.Approved ? "Approved" : "Rejected") } successfully" };
            }
            catch (Exception ex)
            {
                await BaseRepository().RollbackTrans();
                _logger.LogError(ex, "ReviewLiquidationRequest failed");
                return new ApiResponse { Message = ex.Message, Status = 500 };
            }
        }
    }
}
