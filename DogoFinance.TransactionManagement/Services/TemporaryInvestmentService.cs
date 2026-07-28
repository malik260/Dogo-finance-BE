using DogoFinance.AccountingManagement.Interfaces;
using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.BusinessLogic.Layer.Helpers;
using DogoFinance.DataAccess.Layer.DTO;
using DogoFinance.DataAccess.Layer.Interfaces;
using DogoFinance.DataAccess.Layer.Models.Constants;
using DogoFinance.DataAccess.Layer.Models.Entities;
using DogoFinance.DataAccess.Layer.Repositories.Base;
using DogoFinance.Integration.Interfaces;
using DogoFinance.TransactionManagement.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace DogoFinance.TransactionManagement.Services
{
    // DTO used internally to pass accrued fee items through the liquidation flow
    internal sealed class AccruedFeeItem
    {
        public string FeeType       { get; set; } = string.Empty;
        public decimal RatePerAnnum { get; set; }
        public int     DaysHeld     { get; set; }
        public decimal Amount        { get; set; }
        public string  AccountCode  { get; set; } = string.Empty;
        public bool    IsLiability  { get; set; }
    }

    public class TemporaryInvestmentService : DataRepository, ITemporaryInvestmentService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<TemporaryInvestmentService> _logger;
        private readonly IEmailService _emailService;
        private readonly IMemoryCache _cache;
        private readonly IAccountingService _accountingService;

        public TemporaryInvestmentService(
            IUnitOfWork uow,
            ILogger<TemporaryInvestmentService> logger,
            IEmailService emailService,
            IMemoryCache cache,
            IAccountingService accountingService)
        {
            _uow = uow;
            _logger = logger;
            _emailService = emailService;
            _cache = cache;
            _accountingService = accountingService;
        }

        public async Task<ApiResponse> ProcessTempInvestment(long customerId, int portfolioId, decimal amount, string? pin = null, string? otp = null)
        {
            var response = new ApiResponse();
            try
            {
                var customer = await _uow.Customers.GetByUserId(customerId);
                if (customer == null)
                {
                    response.SetError("Customer not found", 404);
                    return response;
                }

                // 1. Security Check: BVN or Corporate Verification
                if (customer.CustomerTypeId == 2) // Corporate
                {
                    bool isFullyVerified = await IsCorporateVerificationComplete(customer.CustomerId);
                    if (!isFullyVerified)
                    {
                        response.SetError("CORPORATE_VERIFICATION_REQUIRED", 403);
                        return response;
                    }
                }
                else
                {
                    if (!customer.Bvnverified)
                    {
                        response.SetError("BVN verification is required before you can invest. Please verify your BVN in settings.", 403);
                        return response;
                    }
                }

                var user = await _uow.Users.GetById(customer.UserId);
                if (user == null) { response.SetError("User account not found", 404); return response; }


                var portfolio = await _uow.Portfolios.GetPortfolioById(portfolioId);
                if (portfolio == null)
                {
                    response.SetError("Portfolio not found", 404);
                    return response;
                }

                // 4. Financial Check: Wallet Balance
                await _uow.BeginTransactionAsync();

                try
                {
                    // a. Debit Wallet
                    var wallet = await _uow.Wallets.GetByCustomerId(customer.CustomerId);
                    if (wallet == null || wallet.Balance < amount)
                    {
                        response.SetError("Insufficient wallet balance for this investment", 400);
                        return response;
                    }


                    wallet.Balance -= amount;
                    await _uow.Wallets.UpdateWallet(wallet);

                    // b. Create Core Transaction (for Dashboard History)
                    var mainTransaction = new TblTransaction
                    {
                        Reference = $"INV_{DateTime.UtcNow.Ticks}",
                        TransactionType = TransactionType.INVESTMENT,
                        Amount = amount,
                        Status = 1, // SUCCESS
                        Narration = $"Investment in {portfolio.Name}",
                        InitiatedByUserId = user.UserId,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _uow.Transactions.CreateTransaction(mainTransaction);

                    // c. Log to Ledger
                    await LogLedger(mainTransaction.TransactionId, wallet.WalletId, EntryType.DEBIT, -amount, wallet.Balance, $"Investment: {portfolio.Name}");

                    // d. Get current projected NAV and calculate units
                    decimal currentNav = await GetProjectedNAV(portfolio);
                    var latestPrice = await GetLatestPrice(portfolioId);

                    // Ensure we have a price record for TODAY so that future projections 
                    // from this moment start from zero profit.
                    if (latestPrice == null || latestPrice.PriceDate.Date < DateTime.UtcNow.Date)
                    {
                        await _uow.GenericRepository.Insert(new TblPortfolioPrice
                        {
                            PortfolioId = portfolioId,
                            PriceDate = DateTime.UtcNow.Date,
                            NAV = currentNav,
                            CreatedAt = DateTime.UtcNow
                        });
                    }

                    decimal units = Math.Round(amount / currentNav, 6);

                    // e. Update Customer Portfolio
                    var userPortfolio = await _uow.Portfolios.GetCustomerPortfolio(customerId, portfolioId);
                    if (userPortfolio == null)
                    {
                        userPortfolio = new TblCustomerPortfolio
                        {
                            CustomerId = customer.CustomerId,
                            PortfolioId = portfolioId,
                            Units = 0,
                            InvestedAmount = 0,
                            TotalInvested = 0,
                            CreatedAt = DateTime.UtcNow
                        };
                        await _uow.Portfolios.SaveCustomerPortfolio(userPortfolio);
                    }

                    userPortfolio.Units += units;
                    userPortfolio.InvestedAmount += amount;
                    userPortfolio.TotalInvested += amount;
                    //userPortfolio. = DateTime.UtcNow;
                    await _uow.Portfolios.SaveCustomerPortfolio(userPortfolio);

                    // f. Create Portfolio Investment Transaction Record (Temporary Mode)
                    var portfolioTx = new TblPortfolioInvestmentTx
                    {
                        CustomerId = customer.CustomerId,
                        PortfolioId = portfolioId,
                        Amount = amount,
                        Units = units,
                        NAV = currentNav,
                        TransactionType = "BUY",
                        CreatedAt = DateTime.UtcNow
                    };
                    await _uow.GenericRepository.Insert(portfolioTx);

                    await _uow.CommitAsync();

                    _cache.Remove($"portfolio_summary_{customer.CustomerId}");

                    response.SetMessage("Investment processed successfully", true, new
                    {
                        UnitsAdded = units,
                        NAVAtPurchase = currentNav,
                        TotalUnits = userPortfolio.Units
                    });
                }
                catch (Exception ex)
                {
                    await _uow.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProcessTempInvestment failed");
                response.SetError(ex.Message, 500);
            }
            return response;
        }

        public async Task<ApiResponse> ProcessSell(long customerId, int portfolioId, decimal amount, string? pin = null, string? otp = null)
        {
            var response = new ApiResponse();
            try
            {
                var customer = await _uow.Customers.GetByUserId(customerId);
                if (customer == null) { response.SetError("Customer not found", 404); return response; }

                var user = await _uow.Users.GetById(customer.UserId);
                if (user == null) { response.SetError("User account not found", 404); return response; }

                // 1. Security Check: Transaction PIN
                if (string.IsNullOrEmpty(pin))
                {
                    response.SetMessage("PIN_REQUIRED", 200);
                    return response;
                }

                if (!HashHelper.VerifyHash(pin, user.TransactionPinHash!, user.TransactionPinSalt!))
                {
                    response.SetError("Incorrect transaction PIN.", 401);
                    return response;
                }

                // 2. Security Check: 2FA OTP (if enabled)
                if (user.Is2faEnabled == true)
                {
                    if (string.IsNullOrEmpty(otp))
                    {
                        await SendSecurityOtp(user, customer.FirstName ?? "Investor");
                        response.SetMessage("OTP_REQUIRED", 200);
                        return response;
                    }

                    if (user.VerificationCode != otp || user.VerificationExpiry < DateTime.UtcNow)
                    {
                        response.SetError("Invalid or expired OTP code.", 401);
                        return response;
                    }

                    user.VerificationCode = null;
                    user.VerificationExpiry = null;
                    await _uow.Users.SaveUser(user);
                }

                // 3. Get Portfolio and Customer Portfolio
                var portfolio = await _uow.Portfolios.GetPortfolioById(portfolioId);
                var userPortfolio = await _uow.Portfolios.GetCustomerPortfolio(customer.CustomerId, portfolioId);

                if (userPortfolio == null || userPortfolio.Units <= 0)
                {
                    response.SetError("No active investment found in this portfolio.", 400);
                    return response;
                }

                // 4. Hardened Workflow Business Rules
                
                // a. Lock-in Period Check
                var oldestInvestment = await BaseRepository().AsQueryable<TblPortfolioInvestmentTx>(t => 
                    t.CustomerId == customer.CustomerId && t.PortfolioId == portfolioId && t.TransactionType == "BUY")
                    .OrderBy(t => t.CreatedAt)
                    .FirstOrDefaultAsync();

                if (oldestInvestment != null)
                {
                    var daysHeld = (DateTime.UtcNow - oldestInvestment.CreatedAt).TotalDays;
                    if (daysHeld < portfolio.LockInPeriodDays)
                    {
                        var remainingDays = Math.Ceiling(portfolio.LockInPeriodDays - daysHeld);
                        response.SetError($"Portfolio is under lock-in. You can liquidate in {remainingDays} day(s).", 403);
                        return response;
                    }
                }

                // 5. Valuation
                var latestPrice = await GetLatestPrice(portfolioId);
                decimal currentNav = latestPrice?.NAV ?? 1.0m;
                decimal currentValue = Math.Round(userPortfolio.Units * currentNav, 2);

                if (amount > currentValue)
                {
                    response.SetError($"Insufficient portfolio value. Available: ₦{currentValue}", 400);
                    return response;
                }

                // 6a. Exit Fee (charged when selling before MinHoldingPeriodDays)
                decimal exitFee = 0;
                if (oldestInvestment != null)
                {
                    var daysHeldTotal = (DateTime.UtcNow - oldestInvestment.CreatedAt).TotalDays;
                    if (daysHeldTotal < portfolio.MinHoldingPeriodDays)
                    {
                        if (currentValue > 0)
                        {
                            decimal ratio = amount / currentValue;
                            decimal capitalPortion = userPortfolio.InvestedAmount * ratio;
                            decimal profitPortion = amount - capitalPortion;

                            if (profitPortion > 0)
                            {
                                exitFee = Math.Round(profitPortion * (portfolio.ExitFeePercentage / 100m), 2);
                            }
                        }
                    }
                }

                // 6b. Accrued Platform Fees (management, custody, SEC) — pro-rated for days held
                //     Applied proportionally to the portion of portfolio being redeemed.
                decimal redeemProportion = currentValue > 0 ? amount / currentValue : 1m;
                var accruedFeeItems = await CalculateAccruedFeesAtLiquidation(
                    customer.CustomerId, portfolioId, amount, redeemProportion, currentNav);
                decimal accruedFeesTotal = accruedFeeItems.Sum(f => f.Amount);

                // Cap: total fees cannot exceed gross amount (net >= 0)
                decimal totalFees = exitFee + accruedFeesTotal;
                if (totalFees > amount)
                {
                    decimal scale = amount / totalFees;
                    exitFee = Math.Round(exitFee * scale, 2);
                    accruedFeesTotal = Math.Round(accruedFeesTotal * scale, 2);
                    foreach (var fi in accruedFeeItems) fi.Amount = Math.Round(fi.Amount * scale, 2);
                    totalFees = exitFee + accruedFeesTotal;
                }

                decimal netAmount = amount - exitFee - accruedFeesTotal;

                // 7. Execute Sale Transaction (Unit Deduction First)
                var db = await BaseRepository().BeginTrans();
                try
                {
                    decimal unitsToSell = Math.Round(amount / currentNav, 6);
                    if (unitsToSell > userPortfolio.Units) unitsToSell = userPortfolio.Units;

                    // Proportional Cost Basis Reduction
                    decimal reductionRatio = unitsToSell / userPortfolio.Units;
                    decimal investedAmountToReduce = userPortfolio.InvestedAmount * reductionRatio;

                    // Update Customer Portfolio (Subtract units immediately to freeze them)
                    userPortfolio.Units -= unitsToSell;
                    userPortfolio.InvestedAmount -= investedAmountToReduce;
                    userPortfolio.TotalInvested -= investedAmountToReduce;

                    if (userPortfolio.Units < 0.000001m)
                    {
                        userPortfolio.Units = 0;
                        userPortfolio.InvestedAmount = 0;
                        userPortfolio.TotalInvested = 0;
                    }
                    await _uow.Portfolios.SaveCustomerPortfolio(userPortfolio);

                    // 8. Workflow Branching
                    bool isImmediate = true;
                    int requestStatus = LiquidationStatus.COMPLETED;
                    string workflowMessage = "Portfolio liquidation processed successfully";
                    DateTime? expectedRelease = null;

                    if (netAmount > portfolio.ApprovalThresholdAmount)
                    {
                        isImmediate = false;
                        requestStatus = LiquidationStatus.PENDING_APPROVAL;
                        workflowMessage = "Liquidation request initiated. Due to the high amount, this requires manual Admin Approval.";
                    }
                    else if (portfolio.NoticePeriodDays > 0)
                    {
                        isImmediate = false;
                        requestStatus = LiquidationStatus.PENDING_NOTICE;
                        expectedRelease = DateTime.UtcNow.AddDays(portfolio.NoticePeriodDays);
                        workflowMessage = $"Liquidation request initiated. Funds will be available on {expectedRelease:dd MMM yyyy} after the {portfolio.NoticePeriodDays}-day notice period.";
                    }

                    // 9. Create Liquidation Request Record
                    var liqRequest = new TblLiquidationRequest
                    {
                        CustomerId = customer.CustomerId,
                        PortfolioId = portfolioId,
                        UnitsRequested = unitsToSell,
                        GrossAmount = amount,
                        ExitFeeApplied = exitFee,
                        AccruedFeesApplied = accruedFeesTotal,
                        NetPayableAmount = netAmount,
                        Status = requestStatus,
                        ExpectedReleaseDate = expectedRelease,
                        CreatedAt = DateTime.UtcNow
                    };
                    await BaseRepository().Insert(liqRequest);

                    // 9b. Post accrued platform fees — journal + fee log + unit deduction
                    foreach (var fi in accruedFeeItems.Where(f => f.Amount > 0))
                    {
                        decimal feeNav = currentNav > 0 ? currentNav : 1.0m;
                        decimal feeUnits = Math.Round(fi.Amount / feeNav, 6);

                        // Reduce customer units for this fee
                        if (feeUnits > 0 && userPortfolio.Units >= feeUnits)
                        {
                            userPortfolio.Units -= feeUnits;
                            userPortfolio.InvestedAmount = Math.Max(0, userPortfolio.InvestedAmount - fi.Amount);
                            userPortfolio.TotalInvested  = Math.Max(0, userPortfolio.TotalInvested  - fi.Amount);
                        }

                        // Audit record in portfolio investment transactions
                        await BaseRepository().Insert(new TblPortfolioInvestmentTx
                        {
                            CustomerId    = customer.CustomerId,
                            PortfolioId   = portfolioId,
                            Amount        = -fi.Amount,
                            Units         = -feeUnits,
                            NAV           = feeNav,
                            TransactionType = "FEE_DEDUCTION",
                            CreatedAt     = DateTime.UtcNow
                        });

                        // Double-entry journal: Dr 2110 (customer liability) / Cr revenue or sec payable
                        var feeRef = $"LIQ_FEE_{liqRequest.Id}_{fi.FeeType}_{DateTime.UtcNow.Ticks}";
                        await _accountingService.PostJournalAsync(new JournalEntryDto
                        {
                            Reference       = feeRef,
                            Narration       = $"Liquidation {fi.FeeType} Fee — LiqRequest #{liqRequest.Id} (Customer #{customer.CustomerId})",
                            TransactionDate = DateTime.UtcNow,
                            Lines = new List<JournalLineDto>
                            {
                                new JournalLineDto { AccountCode = "2110",         Debit = fi.Amount, Credit = 0,         Narration = $"Customer Fee Debit ({fi.FeeType})" },
                                new JournalLineDto { AccountCode = fi.AccountCode, Debit = 0,         Credit = fi.Amount, Narration = $"Fee Credit ({fi.FeeType})" }
                            }
                        });

                        // Fee log for audit / idempotency guard on future quarterly runs
                        await BaseRepository().Insert(new TblQuarterlyFeeLog
                        {
                            CustomerId           = customer.CustomerId,
                            PortfolioId          = portfolioId,
                            FeeConfigId          = 0,
                            Year                 = DateTime.UtcNow.Year,
                            Quarter              = (DateTime.UtcNow.Month - 1) / 3 + 1,
                            AverageNav           = currentNav,
                            FeeType              = fi.FeeType,
                            FeeRateApplied       = fi.RatePerAnnum,
                            CalculatedFeeAmount  = fi.Amount,
                            Status               = "DEDUCTED",
                            JournalReference     = feeRef,
                            ProcessedAt          = DateTime.UtcNow
                        });
                    }

                    // Save updated unit balance after fee deductions
                    await _uow.Portfolios.SaveCustomerPortfolio(userPortfolio);

                    // 10. Immediate Fulfillment (if applicable)
                    if (isImmediate)
                    {
                        // Credit Wallet
                        var wallet = await _uow.Wallets.GetByCustomerId(customer.CustomerId);
                        wallet.Balance += netAmount;
                        await _uow.Wallets.UpdateWallet(wallet);

                        // Create Core Transaction Record
                        var mainTransaction = new TblTransaction
                        {
                            Reference = $"LIQ_{DateTime.UtcNow.Ticks}",
                            TransactionType = TransactionType.LIQUIDATION,
                            Amount = netAmount,
                            Status = 1, // SUCCESS
                            Narration = $"Liquidation from {portfolio.Name} (Net of fees)",
                            InitiatedByUserId = user.UserId,
                            CreatedAt = DateTime.UtcNow
                        };
                        await _uow.Transactions.CreateTransaction(mainTransaction);

                        // Log to Ledger
                        await LogLedger(mainTransaction.TransactionId, wallet.WalletId, EntryType.CREDIT, netAmount, wallet.Balance, $"Portfolio Liquidation: {portfolio.Name}");
                    }

                    // Log Portfolio Specific Transaction (Audit of unit movement)
                    var portfolioTx = new TblPortfolioInvestmentTx
                    {
                        CustomerId = customer.CustomerId,
                        PortfolioId = portfolioId,
                        Amount = amount,
                        Units = unitsToSell,
                        NAV = currentNav,
                        TransactionType = "SELL",
                        CreatedAt = DateTime.UtcNow
                    };
                    await BaseRepository().Insert(portfolioTx);

                    await db.CommitTrans();

                    // Invalidate Cache
                    _cache.Remove($"portfolio_summary_{customer.CustomerId}");

                    response.SetMessage(workflowMessage, true, new
                    {
                        RequestId        = liqRequest.Id,
                        GrossAmount      = amount,
                        ExitFeeApplied   = exitFee,
                        AccruedFees      = accruedFeeItems.Select(f => new
                        {
                            f.FeeType,
                            RatePerAnnum = f.RatePerAnnum,
                            DaysHeld     = f.DaysHeld,
                            Amount       = f.Amount
                        }),
                        TotalAccruedFees = accruedFeesTotal,
                        NetPayable       = netAmount,
                        Status           = Enum.GetName(typeof(LiquidationStatus), requestStatus) ?? requestStatus.ToString()
                    });
                }
                catch (Exception)
                {
                    await db.RollbackTrans();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProcessSell (Hardened) failed");
                response.SetError(ex.Message, 500);
            }
            return response;
        }


        private async Task<bool> IsCorporateVerificationComplete(long customerId)
        {
            var customer = await _uow.Customers.GetCustomerDetailed(customerId);
            if (customer == null) return false;

            var checklistItems = await _uow.GenericRepository.FindList<TblVerificationItem>(v => v.IsActive && (v.TargetEntityTypes == null || v.TargetEntityTypes.Contains("Corporate")));
            var documents = await _uow.GenericRepository.FindList<TblCorporateDocument>(d => d.CustomerId == customerId);
            var docsDict = documents.ToDictionary(d => d.DocumentType, d => d.Status);
            
            var contact = await _uow.GenericRepository.FindEntity<TblCorporateContact>(c => c.CustomerId == customerId && c.IsPrimary);
            var hasBank = (await _uow.GenericRepository.FindList<TblCustomerBank>(p => p.CustomerId == customerId)).Any();
            bool appFormVerified = !string.IsNullOrEmpty(customer.BusinessName) && !string.IsNullOrEmpty(customer.RegistrationNumber) && contact != null;

            foreach (var item in checklistItems)
            {
                string status = "unverified";
                if (item.IsSystemVerified)
                {
                    switch (item.SystemRule)
                    {
                        case "CheckAppForm": status = appFormVerified ? "verified" : "unverified"; break;
                        case "CheckBankLinked": status = hasBank ? "verified" : "unverified"; break;
                        case "CheckSignatoryPhotos":
                            var hasSignatories = await _uow.GenericRepository.FindList<TblCorporateSignatory>(s => s.CustomerId == customerId && !s.IsDeleted);
                            if (!hasSignatories.Any())
                                status = "unverified";
                            else if (hasSignatories.All(x => x.Status.ToLower() == "approved"))
                                status = "verified";
                            else
                                status = "pending";
                            break;
                        case "CheckDirectorsAdded":
                            var hasDirectors = await _uow.GenericRepository.FindList<TblCorporateDirector>(d => d.CustomerId == customerId && !d.IsDeleted);
                            if (!hasDirectors.Any())
                                status = "unverified";
                            else if (hasDirectors.All(x => x.Status.ToLower() == "approved"))
                                status = "verified";
                            else
                                status = "pending";
                            break;
                        case "CheckSignatoryDirectorsId":
                            var sigs = await _uow.GenericRepository.FindList<TblCorporateSignatory>(s => s.CustomerId == customerId && !s.IsDeleted);
                            var dirs = await _uow.GenericRepository.FindList<TblCorporateDirector>(d => d.CustomerId == customerId && !d.IsDeleted);
                            if (!sigs.Any() && !dirs.Any())
                                status = "unverified";
                            else if (sigs.Any() && dirs.Any() && sigs.All(x => x.Status.ToLower() == "approved") && dirs.All(x => x.Status.ToLower() == "approved"))
                                status = "verified";
                            else
                                status = "pending";
                            break;
                    }
                }
                else
                {
                    status = docsDict.GetValueOrDefault(item.Type, "unverified").ToLower();
                }

                if (status != "verified" && status != "approved") return false;
            }
            return true;
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

        public async Task<ApiResponse> GetTempPortfolioStats(long userId, int portfolioId)
        {
            var response = new ApiResponse();
            try
            {
                var customer = await _uow.Customers.GetByUserId(userId);
                if (customer == null) { response.SetError("Customer not found", 404); return response; }

                var userPortfolio = await _uow.Portfolios.GetCustomerPortfolio(customer.CustomerId, portfolioId);
                if (userPortfolio == null || userPortfolio.Units == 0)
                {
                    return new ApiResponse { Success = true, Data = new { units = 0, nav = 0, value = 0, profit = 0 } };
                }

                var portfolio = await _uow.Portfolios.GetPortfolioById(portfolioId);
                                // Calculate "Current" NAV based on Expected Annual Return and Time since last price update
                decimal currentNav = await GetProjectedNAV(portfolio);


                decimal currentValue = Math.Round(userPortfolio.Units * currentNav, 2);
                decimal profit = currentValue - userPortfolio.InvestedAmount;

                response.SetMessage("Portfolio stats retrieved", true, new
                {
                    units = userPortfolio.Units,
                    nav = Math.Round(currentNav, 4),
                    value = currentValue,
                    profit = Math.Round(profit, 2),
                    invested = userPortfolio.InvestedAmount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetTempPortfolioStats failed");
                response.SetError("Failed to retrieve stats", 500);
            }
            return response;
        }

        public async Task<ApiResponse> SimulateNAVGrowth(int portfolioId, int days)
        {
            var response = new ApiResponse();
            try
            {
                var portfolio = await _uow.Portfolios.GetPortfolioById(portfolioId);
                if (portfolio == null || !portfolio.ExpectedAnnualReturn.HasValue)
                {
                    response.SetError("Portfolio or Expected Return not configured", 400);
                    return response;
                }

                var latestPrice = await GetLatestPrice(portfolioId);
                decimal startNav = latestPrice?.NAV ?? 1.0m;

                decimal annualRate = portfolio.ExpectedAnnualReturn.Value / 100m;
                decimal dailyRate = annualRate / 365m;
                decimal newNav = startNav * (decimal)Math.Pow(1 + (double)dailyRate, days);

                var newPrice = new TblPortfolioPrice
                {
                    PortfolioId = portfolioId,
                    PriceDate = DateTime.UtcNow.AddDays(days).Date,
                    NAV = Math.Round(newNav, 6),
                    CreatedAt = DateTime.UtcNow
                };

                await BaseRepository().Insert(newPrice);
                response.SetMessage($"Simulated {days} days of growth. New NAV: {newPrice.NAV}", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SimulateNAVGrowth failed");
                response.SetError("Failed to simulate growth", 500);
            }
            return response;
        }

        private async Task SendSecurityOtp(TblUser user, string firstName)
        {
            var otp = new Random().Next(100000, 999999).ToString();
            user.VerificationCode = otp;
            user.VerificationExpiry = DateTime.UtcNow.AddMinutes(15);
            await _uow.Users.SaveUser(user);

            var placeholders = new Dictionary<string, string>
            {
                { "FirstName", firstName },
                { "OtpCode", otp },
                { "Action", "confirm your investment" }
            };

            await _emailService.SendTemplateEmail(user.Email, "Secure Investment OTP - DogoFinance", "SecurityOtp", placeholders);
        }

        /// <summary>
        /// Calculates pro-rated platform fees (management, custody, SEC, etc.) owed at the
        /// moment of liquidation, covering only the days since the last quarterly deduction.
        /// Fees are scaled by <paramref name="redeemProportion"/> so partial redeems only
        /// pay fees on the portion being sold.
        /// </summary>
        private async Task<List<AccruedFeeItem>> CalculateAccruedFeesAtLiquidation(
            long customerId, int portfolioId, decimal grossAmount,
            decimal redeemProportion, decimal currentNav)
        {
            var result = new List<AccruedFeeItem>();

            // 1. Load active, non-waived fee configs for this portfolio
            var feeConfigs = await BaseRepository()
                .AsQueryable<TblPortfolioFeeConfig>(f =>
                    f.PortfolioId == portfolioId && f.IsActive && !f.IsWaived)
                .ToListAsync();

            if (!feeConfigs.Any()) return result;

            // 2. Find the date up to which fees have already been deducted
            //    (latest TBL_QUARTERLY_FEE_LOG entry for this customer+portfolio)
            var lastFeeDate = await BaseRepository()
                .AsQueryable<TblQuarterlyFeeLog>(l =>
                    l.CustomerId == customerId &&
                    l.PortfolioId == portfolioId &&
                    l.Status == "DEDUCTED")
                .OrderByDescending(l => l.ProcessedAt)
                .Select(l => (DateTime?)l.ProcessedAt)
                .FirstOrDefaultAsync();

            // 3. If no fee was ever deducted, use the customer's oldest BUY date
            if (lastFeeDate == null)
            {
                lastFeeDate = await BaseRepository()
                    .AsQueryable<TblPortfolioInvestmentTx>(t =>
                        t.CustomerId == customerId &&
                        t.PortfolioId == portfolioId &&
                        t.TransactionType == "BUY")
                    .OrderBy(t => t.CreatedAt)
                    .Select(t => (DateTime?)t.CreatedAt)
                    .FirstOrDefaultAsync();
            }

            if (lastFeeDate == null) return result; // No investment history — nothing to charge

            int daysHeld = (DateTime.UtcNow.Date - lastFeeDate.Value.Date).Days;
            if (daysHeld <= 0) return result; // Fees already current — nothing to charge

            // 4. Compute pro-rated daily fee per config
            //    formula: grossAmount × (rate / 100 / 365) × daysHeld × redeemProportion
            foreach (var config in feeConfigs)
            {
                decimal dailyRate = config.PercentagePerAnnum / 100m / 365m;
                decimal feeAmount = Math.Round(grossAmount * dailyRate * daysHeld * redeemProportion, 2);

                if (feeAmount <= 0) continue;

                result.Add(new AccruedFeeItem
                {
                    FeeType      = config.FeeType,
                    RatePerAnnum = config.PercentagePerAnnum,
                    DaysHeld     = daysHeld,
                    Amount       = feeAmount,
                    AccountCode  = config.TargetAccountCode,
                    IsLiability  = config.IsLiability
                });
            }

            return result;
        }

        private async Task<TblPortfolioPrice?> GetLatestPrice(int portfolioId)
        {
            return await BaseRepository().AsQueryable<TblPortfolioPrice>(p => p.PortfolioId == portfolioId)
                .OrderByDescending(p => p.PriceDate)
                .FirstOrDefaultAsync();
        }
        private async Task<decimal> GetProjectedNAV(TblPortfolio portfolio)
        {
            var latestPrice = await GetLatestPrice(portfolio.PortfolioId);
            decimal currentNav = latestPrice?.NAV ?? 1.0m;

            if (portfolio.ExpectedAnnualReturn.HasValue && latestPrice != null)
            {
                var daysPassed = (DateTime.UtcNow.Date - latestPrice.PriceDate.Date).Days;
                if (daysPassed > 0)
                {
                    decimal dRate = portfolio.ExpectedAnnualReturn.Value / 100m / 365m;
                    currentNav = latestPrice.NAV * (decimal)Math.Pow(1 + (double)dRate, daysPassed);
                }
            }
            return currentNav;
        }

        public async Task<ApiResponse> GetActiveInvestments(long customerId)
        {
            var response = new ApiResponse();
            try
            {
                var portfolios = await _uow.Portfolios.GetCustomerPortfolios(customerId);
                var activeHoldings = portfolios.Where(p => p.Units > 0).ToList();

                var results = new List<object>();
                foreach (var up in activeHoldings)
                {
                    var portfolio = await _uow.Portfolios.GetPortfolioById(up.PortfolioId);
                    if (portfolio == null) continue;

                    // Project current NAV
                    decimal currentNav = await GetProjectedNAV(portfolio);

                    decimal currentValue = up.Units * currentNav;
                    decimal profit = currentValue - up.InvestedAmount;
                    decimal growth = up.InvestedAmount == 0 ? 0 : (profit / up.InvestedAmount) * 100;

                    // Find oldest investment date for tenure (Holding Period)
                    var oldestTx = await _uow.GenericRepository.AsQueryable<TblPortfolioInvestmentTx>(t => 
                        t.CustomerId == customerId && t.PortfolioId == up.PortfolioId && t.TransactionType == "BUY")
                        .OrderBy(t => t.CreatedAt)
                        .Select(t => (DateTime?)t.CreatedAt)
                        .FirstOrDefaultAsync();

                    // Find latest investment date for activity display
                    var latestTx = await _uow.GenericRepository.AsQueryable<TblPortfolioInvestmentTx>(t => 
                        t.CustomerId == customerId && t.PortfolioId == up.PortfolioId && t.TransactionType == "BUY")
                        .OrderByDescending(t => t.CreatedAt)
                        .Select(t => (DateTime?)t.CreatedAt)
                        .FirstOrDefaultAsync();
                    
                    var displayInvestedAt = oldestTx ?? up.CreatedAt;
                    var lastToppedUp = latestTx ?? up.CreatedAt;

                    results.Add(new
                    {
                        portfolioId = up.PortfolioId,
                        portfolioName = portfolio.Name,
                        portfolioCode = portfolio.Code,
                        units = up.Units,
                        investedAmount = Math.Round(up.InvestedAmount, 2),
                        currentValue = Math.Round(currentValue, 2),
                        profit = Math.Round(profit, 2),
                        growth = Math.Round(growth, 2),
                        riskLevel = portfolio.RiskLevel,
                        
                        // Liquidation Policy
                        lockInPeriodDays = portfolio.LockInPeriodDays,
                        minHoldingPeriodDays = portfolio.MinHoldingPeriodDays,
                        exitFeePercentage = portfolio.ExitFeePercentage,
                        noticePeriodDays = portfolio.NoticePeriodDays,
                        approvalThresholdAmount = portfolio.ApprovalThresholdAmount,
                        investedAt = displayInvestedAt,
                        lastToppedUp = lastToppedUp,

                        // Detailed Batches (Tranches)
                        batches = await _uow.GenericRepository.AsQueryable<TblPortfolioInvestmentTx>(t => 
                            t.CustomerId == customerId && t.PortfolioId == up.PortfolioId && t.TransactionType == "BUY")
                            .OrderBy(t => t.CreatedAt)
                            .Select(t => new {
                                id = t.Id,
                                date = t.CreatedAt,
                                invested = t.Amount,
                                units = t.Units,
                                // Calculate profit for this specific batch
                                currentValue = Math.Round(t.Units * currentNav, 2),
                                profit = Math.Round((t.Units * currentNav) - t.Amount, 2),
                                yield = t.Amount == 0 ? 0 : Math.Round((((t.Units * currentNav) - t.Amount) / t.Amount) * 100, 2)
                            })
                            .ToListAsync()
                    });
                }
                response.SetMessage("Retrieved active investments", true, results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetActiveInvestments failed for {CustomerId}", customerId);
                response.SetError(ex.Message, 500);
            }
            return response;
        }
    }
}
