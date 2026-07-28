using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DogoFinance.AccountingManagement.Interfaces;
using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.DataAccess.Layer.DTO;
using DogoFinance.DataAccess.Layer.Interfaces;
using DogoFinance.DataAccess.Layer.Models.Entities;
using DogoFinance.ProductManagement.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DogoFinance.ProductManagement.Services
{
    public class FeeComputationService : IFeeComputationService
    {
        private readonly IUnitOfWork _uow;
        private readonly IAccountingService _accountingService;
        private readonly ILogger<FeeComputationService> _logger;

        public FeeComputationService(IUnitOfWork uow, IAccountingService accountingService, ILogger<FeeComputationService> logger)
        {
            _uow = uow;
            _accountingService = accountingService;
            _logger = logger;
        }

        public async Task<ApiResponse> GetProductFeeConfigsAsync(int portfolioId)
        {
            var configs = await _uow.GenericRepository
                .AsQueryable<TblPortfolioFeeConfig>(f => f.PortfolioId == portfolioId)
                .ToListAsync();

            var dtos = configs.Select(MapToDto).ToList();
            return new ApiResponse { Success = true, Data = dtos, Status = 200 };
        }

        public async Task<ApiResponse> SaveProductFeeConfigAsync(PortfolioFeeConfigDto dto)
        {
            try
            {
                TblPortfolioFeeConfig? entity = null;
                if (dto.Id > 0)
                {
                    entity = await _uow.GenericRepository.FindEntity<TblPortfolioFeeConfig>(f => f.Id == dto.Id);
                }

                if (entity == null)
                {
                    entity = new TblPortfolioFeeConfig
                    {
                        PortfolioId = dto.PortfolioId,
                        FeeType = dto.FeeType.ToUpper(),
                        PercentagePerAnnum = dto.PercentagePerAnnum,
                        CalculationBasis = dto.CalculationBasis ?? "AVERAGE_MONTH_END_NAV",
                        BillingFrequency = dto.BillingFrequency ?? "QUARTERLY",
                        ChargeDayOfMonth = dto.ChargeDayOfMonth <= 0 ? 10 : dto.ChargeDayOfMonth,
                        TargetAccountCode = dto.TargetAccountCode,
                        IsLiability = dto.IsLiability,
                        IsWaived = dto.IsWaived,
                        IsActive = dto.IsActive,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _uow.GenericRepository.Insert(entity);
                }
                else
                {
                    entity.FeeType = dto.FeeType.ToUpper();
                    entity.PercentagePerAnnum = dto.PercentagePerAnnum;
                    entity.CalculationBasis = dto.CalculationBasis;
                    entity.BillingFrequency = dto.BillingFrequency;
                    entity.ChargeDayOfMonth = dto.ChargeDayOfMonth;
                    entity.TargetAccountCode = dto.TargetAccountCode;
                    entity.IsLiability = dto.IsLiability;
                    entity.IsWaived = dto.IsWaived;
                    entity.IsActive = dto.IsActive;
                    await _uow.GenericRepository.Update(entity);
                }

                await _uow.SaveChangesAsync();
                return new ApiResponse { Success = true, Message = "Product fee configuration saved successfully.", Data = MapToDto(entity), Status = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving product fee configuration");
                return new ApiResponse { Success = false, Message = $"Error: {ex.Message}", Status = 500 };
            }
        }

        public async Task<ApiResponse> ToggleWaiveFeeConfigAsync(int configId, bool isWaived)
        {
            var config = await _uow.GenericRepository.FindEntity<TblPortfolioFeeConfig>(f => f.Id == configId);
            if (config == null) return new ApiResponse { Message = "Fee configuration rule not found.", Status = 404 };

            config.IsWaived = isWaived;
            await _uow.GenericRepository.Update(config);
            await _uow.SaveChangesAsync();

            return new ApiResponse { Success = true, Message = $"Fee rule {(isWaived ? "waived" : "activated")} successfully.", Status = 200 };
        }

        public async Task<ApiResponse> SeedDefaultFeeConfigsAsync()
        {
            var portfolios = await _uow.GenericRepository.AsQueryable<TblPortfolio>(p => p.IsActive).ToListAsync();
            int seededCount = 0;

            foreach (var portfolio in portfolios)
            {
                var existing = await _uow.GenericRepository
                    .AsQueryable<TblPortfolioFeeConfig>(f => f.PortfolioId == portfolio.PortfolioId)
                    .AnyAsync();

                if (!existing)
                {
                    // 1. Management Fee (1.5% p.a.) -> Revenue 4220
                    await _uow.GenericRepository.Insert(new TblPortfolioFeeConfig
                    {
                        PortfolioId = portfolio.PortfolioId,
                        FeeType = "MANAGEMENT",
                        PercentagePerAnnum = 1.50m,
                        CalculationBasis = "AVERAGE_MONTH_END_NAV",
                        BillingFrequency = "QUARTERLY",
                        ChargeDayOfMonth = 10,
                        TargetAccountCode = "4220",
                        IsLiability = false,
                        IsWaived = false,
                        IsActive = true
                    });

                    // 2. Custody Fee (0.25% p.a.) -> Revenue 4230
                    await _uow.GenericRepository.Insert(new TblPortfolioFeeConfig
                    {
                        PortfolioId = portfolio.PortfolioId,
                        FeeType = "CUSTODY",
                        PercentagePerAnnum = 0.25m,
                        CalculationBasis = "AVERAGE_MONTH_END_NAV",
                        BillingFrequency = "QUARTERLY",
                        ChargeDayOfMonth = 10,
                        TargetAccountCode = "4230",
                        IsLiability = false,
                        IsWaived = false,
                        IsActive = true
                    });

                    // 3. SEC Regulatory Fee (0.25% p.a.) -> Liability 2210
                    await _uow.GenericRepository.Insert(new TblPortfolioFeeConfig
                    {
                        PortfolioId = portfolio.PortfolioId,
                        FeeType = "SEC_REGULATORY",
                        PercentagePerAnnum = 0.25m,
                        CalculationBasis = "AVERAGE_MONTH_END_NAV",
                        BillingFrequency = "QUARTERLY",
                        ChargeDayOfMonth = 10,
                        TargetAccountCode = "2210",
                        IsLiability = true,
                        IsWaived = false,
                        IsActive = true
                    });

                    seededCount += 3;
                }
            }

            await _uow.SaveChangesAsync();
            return new ApiResponse { Success = true, Message = $"Seeded {seededCount} default fee configurations across active portfolios.", Status = 200 };
        }

        public async Task<ApiResponse> PreviewQuarterlyFeesAsync(int year, int quarter, int? portfolioId = null)
        {
            var previews = await GenerateFeeCalculationItemsAsync(year, quarter, portfolioId);
            return new ApiResponse { Success = true, Data = previews, Status = 200 };
        }

        public async Task<ApiResponse> ExecuteQuarterlyFeeDeductionsAsync(int year, int quarter, int? portfolioId = null)
        {
            await _uow.BeginTransactionAsync();
            try
            {
                var calculationItems = await GenerateFeeCalculationItemsAsync(year, quarter, portfolioId);
                int processedCount = 0;
                int skippedCount = 0;

                foreach (var item in calculationItems)
                {
                    if (item.IsAlreadyExecuted || item.CalculatedQuarterlyFee <= 0)
                    {
                        skippedCount++;
                        continue;
                    }

                    // 1. Check idempotency guard in DB
                    var existingLog = await _uow.GenericRepository.AsQueryable<TblQuarterlyFeeLog>(l =>
                        l.CustomerId == item.CustomerId &&
                        l.PortfolioId == item.PortfolioId &&
                        l.Year == year &&
                        l.Quarter == quarter &&
                        l.FeeType == item.FeeType &&
                        l.Status == "DEDUCTED").FirstOrDefaultAsync();

                    if (existingLog != null)
                    {
                        skippedCount++;
                        continue;
                    }

                    // 2. Reduce portfolio investment balance / record fee transaction
                    // Calculate units to deduct so the dashboard value is net of fees.
                    // Use AverageNav as the NAV at which the fee is charged;
                    // fall back to 1.0 to avoid a divide-by-zero if no price exists.
                    decimal navForFee = item.AverageNav > 0 ? item.AverageNav : 1.0m;
                    decimal unitsToDeduct = Math.Round(item.CalculatedQuarterlyFee / navForFee, 6);

                    var investmentTx = new TblPortfolioInvestmentTx
                    {
                        CustomerId = item.CustomerId,
                        PortfolioId = item.PortfolioId,
                        Amount = -item.CalculatedQuarterlyFee,
                        Units = -unitsToDeduct,
                        NAV = navForFee,
                        TransactionType = "FEE_DEDUCTION",
                        CreatedAt = DateTime.UtcNow
                    };
                    await _uow.GenericRepository.Insert(investmentTx);

                    // 2b. Reduce the customer's live unit balance so the dashboard
                    //     profit figure is net of fees (value = Units × NAV).
                    var customerPortfolio = await _uow.GenericRepository
                        .FindEntity<TblCustomerPortfolio>(cp =>
                            cp.CustomerId == item.CustomerId &&
                            cp.PortfolioId == item.PortfolioId);

                    if (customerPortfolio != null && customerPortfolio.Units > 0)
                    {
                        // Cap deduction at available units to prevent going negative
                        decimal actualUnitsDeducted = Math.Min(unitsToDeduct, customerPortfolio.Units);
                        customerPortfolio.Units -= actualUnitsDeducted;

                        // Also reduce InvestedAmount by the fee so cost basis stays consistent
                        customerPortfolio.InvestedAmount = Math.Max(0, customerPortfolio.InvestedAmount - item.CalculatedQuarterlyFee);
                        customerPortfolio.TotalInvested  = Math.Max(0, customerPortfolio.TotalInvested  - item.CalculatedQuarterlyFee);

                        await _uow.GenericRepository.Update(customerPortfolio);
                    }

                    var refNo = $"FEE_{year}Q{quarter}_{item.FeeType}_{item.CustomerId}_{item.PortfolioId}_{DateTime.UtcNow.Ticks}";

                    // 3. Post Double-Entry Journal
                    // Dr. 2110 (Customer Wallet / Liability reduces by fee)
                    // Cr. Revenue (4220/4230) OR Liability (2210 SEC Payable)
                    var journalDto = new JournalEntryDto
                    {
                        Reference = refNo,
                        Narration = $"Quarterly {item.FeeType} Fee Q{quarter}-{year} (Product #{item.PortfolioId}, Customer #{item.CustomerId})",
                        TransactionDate = DateTime.UtcNow,
                        Lines = new List<JournalLineDto>
                        {
                            new JournalLineDto { AccountCode = "2110", Debit = item.CalculatedQuarterlyFee, Credit = 0, Narration = $"Customer Fee Debit ({item.FeeType})" },
                            new JournalLineDto { AccountCode = item.TargetAccountCode, Debit = 0, Credit = item.CalculatedQuarterlyFee, Narration = $"Fee Credit ({item.FeeType})" }
                        }
                    };

                    await _accountingService.PostJournalAsync(journalDto);

                    // 4. Log execution in TBL_QUARTERLY_FEE_LOG
                    var feeLog = new TblQuarterlyFeeLog
                    {
                        CustomerId = item.CustomerId,
                        PortfolioId = item.PortfolioId,
                        FeeConfigId = 0, // General
                        Year = year,
                        Quarter = quarter,
                        Month1EndNav = item.Month1Nav,
                        Month2EndNav = item.Month2Nav,
                        Month3EndNav = item.Month3Nav,
                        AverageNav = item.AverageNav,
                        FeeType = item.FeeType,
                        FeeRateApplied = item.FeeRatePerAnnum,
                        CalculatedFeeAmount = item.CalculatedQuarterlyFee,
                        Status = "DEDUCTED",
                        JournalReference = refNo,
                        ProcessedAt = DateTime.UtcNow
                    };
                    await _uow.GenericRepository.Insert(feeLog);

                    processedCount++;
                }

                await _uow.CommitAsync();
                return new ApiResponse
                {
                    Success = true,
                    Message = $"Quarterly fee execution complete. {processedCount} fee items deducted & posted. {skippedCount} items skipped (already processed or zero fee).",
                    Data = new { ProcessedCount = processedCount, SkippedCount = skippedCount },
                    Status = 200
                };
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                _logger.LogError(ex, "ExecuteQuarterlyFeeDeductions Error for Year {Year} Q{Quarter}", year, quarter);
                return new ApiResponse { Success = false, Message = $"Error executing quarterly fees: {ex.Message}", Status = 500 };
            }
        }

        public async Task<ApiResponse> GetSecRemittanceReportAsync(int year, int quarter)
        {
            var logs = await _uow.GenericRepository
                .AsQueryable<TblQuarterlyFeeLog>(l => l.Year == year && l.Quarter == quarter && l.FeeType == "SEC_REGULATORY" && l.Status == "DEDUCTED")
                .Include(l => l.Customer)
                .Include(l => l.Portfolio)
                .ToListAsync();

            var totalSecRemittance = logs.Sum(l => l.CalculatedFeeAmount);

            var report = new
            {
                Year = year,
                Quarter = quarter,
                TotalCollectedForSec = totalSecRemittance,
                RecordCount = logs.Count,
                Details = logs.Select(l => new
                {
                    l.CustomerId,
                    CustomerName = l.Customer != null ? $"{l.Customer.FirstName} {l.Customer.LastName}" : "Customer",
                    l.PortfolioId,
                    PortfolioName = l.Portfolio?.Name ?? "Portfolio",
                    l.AverageNav,
                    l.FeeRateApplied,
                    SecFeeDeducted = l.CalculatedFeeAmount,
                    l.JournalReference,
                    l.ProcessedAt
                })
            };

            return new ApiResponse { Success = true, Data = report, Status = 200 };
        }

        // ─── PRIVATE HELPER COMPUTATION METHODS ──────────────────────────────────────

        private async Task<List<FeeCalculationPreviewDto>> GenerateFeeCalculationItemsAsync(int year, int quarter, int? portfolioId)
        {
            var result = new List<FeeCalculationPreviewDto>();

            // 1. Determine quarter months
            int startMonth = ((quarter - 1) * 3) + 1;
            int m1 = startMonth, m2 = startMonth + 1, m3 = startMonth + 2;

            // 2. Fetch active portfolio fee configurations
            var feeConfigsQuery = _uow.GenericRepository.AsQueryable<TblPortfolioFeeConfig>(f => f.IsActive && !f.IsWaived);
            if (portfolioId.HasValue && portfolioId.Value > 0)
            {
                feeConfigsQuery = feeConfigsQuery.Where(f => f.PortfolioId == portfolioId.Value);
            }
            var feeConfigs = await feeConfigsQuery.ToListAsync();

            // 3. Fetch active customer investments grouped by Customer & Portfolio
            var customerInvestments = await _uow.GenericRepository
                .AsQueryable<TblPortfolioInvestmentTx>(t => true)
                .GroupBy(t => new { t.CustomerId, t.PortfolioId })
                .Select(g => new
                {
                    g.Key.CustomerId,
                    g.Key.PortfolioId,
                    TotalInvestedAmount = g.Sum(x => x.Amount)
                })
                .Where(x => x.TotalInvestedAmount > 0)
                .ToListAsync();

            var customers = await _uow.GenericRepository.AsQueryable<TblCustomer>(c => true).ToDictionaryAsync(c => c.CustomerId);
            var portfolios = await _uow.GenericRepository.AsQueryable<TblPortfolio>(p => true).ToDictionaryAsync(p => p.PortfolioId);

            // Fetch already executed fee logs for idempotency status in preview
            var executedLogs = await _uow.GenericRepository
                .AsQueryable<TblQuarterlyFeeLog>(l => l.Year == year && l.Quarter == quarter && l.Status == "DEDUCTED")
                .ToListAsync();

            foreach (var inv in customerInvestments)
            {
                var matchingConfigs = feeConfigs.Where(f => f.PortfolioId == inv.PortfolioId).ToList();
                if (!matchingConfigs.Any()) continue;

                // Fetch NAV prices for month-end dates
                var prices = await _uow.GenericRepository
                    .AsQueryable<TblPortfolioPrice>(p => p.PortfolioId == inv.PortfolioId && p.PriceDate.Year == year)
                    .ToListAsync();

                decimal nav1 = prices.FirstOrDefault(p => p.PriceDate.Month == m1)?.NAV ?? 1.0m;
                decimal nav2 = prices.FirstOrDefault(p => p.PriceDate.Month == m2)?.NAV ?? 1.0m;
                decimal nav3 = prices.FirstOrDefault(p => p.PriceDate.Month == m3)?.NAV ?? 1.0m;

                decimal averageNavMultiplier = (nav1 + nav2 + nav3) / 3m;
                decimal averagePortfolioValue = Math.Round(inv.TotalInvestedAmount * averageNavMultiplier, 2);

                customers.TryGetValue(inv.CustomerId, out var cust);
                portfolios.TryGetValue(inv.PortfolioId, out var port);

                string custName = cust != null ? $"{cust.FirstName} {cust.LastName}" : $"Customer #{inv.CustomerId}";
                string portName = port?.Name ?? $"Portfolio #{inv.PortfolioId}";

                foreach (var config in matchingConfigs)
                {
                    // Quarterly fee rate = (p.a. percentage / 100) / 4
                    decimal quarterlyRate = (config.PercentagePerAnnum / 100m) / 4m;
                    decimal calculatedFee = Math.Round(averagePortfolioValue * quarterlyRate, 2);

                    bool isExecuted = executedLogs.Any(l => l.CustomerId == inv.CustomerId && l.PortfolioId == inv.PortfolioId && l.FeeType == config.FeeType);

                    result.Add(new FeeCalculationPreviewDto
                    {
                        CustomerId = inv.CustomerId,
                        CustomerName = custName,
                        PortfolioId = inv.PortfolioId,
                        PortfolioName = portName,
                        Year = year,
                        Quarter = quarter,
                        Month1Nav = inv.TotalInvestedAmount * nav1,
                        Month2Nav = inv.TotalInvestedAmount * nav2,
                        Month3Nav = inv.TotalInvestedAmount * nav3,
                        AverageNav = averagePortfolioValue,
                        FeeType = config.FeeType,
                        FeeRatePerAnnum = config.PercentagePerAnnum,
                        CalculatedQuarterlyFee = calculatedFee,
                        TargetAccountCode = config.TargetAccountCode,
                        IsAlreadyExecuted = isExecuted
                    });
                }
            }

            return result;
        }

        private static PortfolioFeeConfigDto MapToDto(TblPortfolioFeeConfig f) => new PortfolioFeeConfigDto
        {
            Id = f.Id,
            PortfolioId = f.PortfolioId,
            FeeType = f.FeeType,
            PercentagePerAnnum = f.PercentagePerAnnum,
            CalculationBasis = f.CalculationBasis,
            BillingFrequency = f.BillingFrequency,
            ChargeDayOfMonth = f.ChargeDayOfMonth,
            TargetAccountCode = f.TargetAccountCode,
            IsLiability = f.IsLiability,
            IsWaived = f.IsWaived,
            IsActive = f.IsActive
        };
    }
}
