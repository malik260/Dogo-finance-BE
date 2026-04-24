using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.DataAccess.Layer.DTO;
using DogoFinance.DataAccess.Layer.Interfaces;
using DogoFinance.DataAccess.Layer.Models.Entities;
using DogoFinance.DataAccess.Layer.Repositories.Base;
using DogoFinance.TransactionManagement.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DogoFinance.TransactionManagement.Services
{
    public class CustomerInvestmentService : DataRepository, ICustomerInvestmentService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<CustomerInvestmentService> _logger;
        private readonly IMemoryCache _cache;

        public CustomerInvestmentService(IUnitOfWork uow, ILogger<CustomerInvestmentService> logger, IMemoryCache cache)
        {
            _uow = uow;
            _logger = logger;
            _cache = cache;
        }

        public async Task<ApiResponse> InvestAsync(InvestRequestDto request, long customerId)
        {
            var response = new ApiResponse();
            
            try
            {
                // 1. Validate Portfolio
                var portfolio = await _uow.Portfolios.GetPortfolioById(request.PortfolioId);

                if (portfolio == null || !portfolio.IsActive)
                {
                    response.SetError("Invalid portfolio or portfolio is inactive", 404);
                    return response;
                }

                // 2. Get Asset Allocation Rules
                var allocationRules = (await _uow.Portfolios.GetAllocationRules(request.PortfolioId)).ToList();

                if (!allocationRules.Any())
                {
                    response.SetError("No allocation rules defined for this portfolio", 400);
                    return response;
                }

                // VALIDATE: Asset allocation must sum to 100%
                var totalAssetWeight = allocationRules.Sum(x => x.TargetPercentage);
                if (totalAssetWeight != 100)
                {
                    response.SetError("Asset allocation target percentages must equal 100%", 400);
                    return response;
                }

                // 3. Get Portfolio Instruments
                var portfolioInstruments = (await _uow.Portfolios.GetInstrumentsDetailed(request.PortfolioId)).ToList();

                if (!portfolioInstruments.Any())
                {
                    response.SetError("No instruments configured for this portfolio", 400);
                    return response;
                }

                // VALIDATE: Instrument weights per asset class
                var grouped = portfolioInstruments.GroupBy(x => x.AssetClassId);
                foreach (var group in grouped)
                {
                    var sum = group.Sum(x => x.TargetWeight);
                    if (sum != 100)
                    {
                        response.SetError($"Instrument weights for Asset class {group.Key} must sum to 100%", 400);
                        return response;
                    }
                }

                // START TRANSACTION
                await _uow.BeginTransactionAsync();

                decimal totalAmount = request.Amount;

                // 4. Ensure Customer Portfolio exists
                var userPortfolio = await _uow.Portfolios.GetCustomerPortfolio(customerId, request.PortfolioId);

                if (userPortfolio == null)
                {
                    userPortfolio = new TblCustomerPortfolio
                    {
                        CustomerId = customerId,
                        PortfolioId = request.PortfolioId,
                        TotalInvested = 0,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _uow.Portfolios.SaveCustomerPortfolio(userPortfolio);
                }

                decimal totalInvested = 0;

                // 🔥 PRELOAD NAVs (Performance Fix)
                var instrumentIds = portfolioInstruments.Select(x => x.InstrumentId).Distinct().ToList();

                var navs = await BaseRepository().AsQueryable<TblInstrumentPrice>(p => instrumentIds.Contains(p.InstrumentId))
                    .GroupBy(p => p.InstrumentId)
                    .Select(g => new
                    {
                        InstrumentId = g.Key,
                        NAV = g.OrderByDescending(x => x.PriceDate)
                               .Select(x => x.NAV)
                               .FirstOrDefault()
                    })
                    .ToDictionaryAsync(x => x.InstrumentId, x => x.NAV);

                // 5. LOOP: Asset Class Allocation
                foreach (var asset in allocationRules)
                {
                    var assetAmount = (asset.TargetPercentage / 100m) * totalAmount;

                    // Get instruments for this asset class
                    var instruments = portfolioInstruments
                        .Where(pi => pi.AssetClassId == asset.AssetClassId)
                        .ToList();

                    if (!instruments.Any()) continue;

                    // 6. LOOP: Instrument Allocation
                    foreach (var instrumentMap in instruments)
                    {
                        var weight = instrumentMap.TargetWeight / 100m;
                        var splitAmount = assetAmount * weight;
                        var instrumentId = instrumentMap.InstrumentId;

                        // 7. Get NAV from preloaded dictionary
                        if (!navs.TryGetValue(instrumentId, out var nav) || nav <= 0)
                            throw new Exception($"NAV (Price) not found or invalid for instrument: {instrumentId}");

                        // 8. Calculate Units
                        var units = Math.Round(splitAmount / nav, 6);

                        // 9. Update Holdings
                        var holding = await _uow.Portfolios.GetCustomerHolding(customerId, instrumentId);

                        if (holding == null)
                        {
                            holding = new TblCustomerHolding
                            {
                                CustomerId = customerId,
                                InstrumentId = instrumentId,
                                Units = 0,
                                InvestedAmount = 0,
                                CreatedAt = DateTime.UtcNow
                            };
                        }

                        holding.Units += units;
                        holding.InvestedAmount += splitAmount;
                        await _uow.Portfolios.SaveCustomerHolding(holding);

                        // 10. Record Transaction
                        await _uow.Portfolios.SaveInvestmentTransaction(new TblInvestmentTransaction
                        {
                            CustomerId = customerId,
                            PortfolioId = request.PortfolioId,
                            InstrumentId = instrumentId,
                            Amount = splitAmount,
                            Units = units,
                            NAV = nav,
                            TransactionType = "BUY",
                            CreatedAt = DateTime.UtcNow
                        });

                        totalInvested += splitAmount;
                    }
                }

                // 11. Update Portfolio
                userPortfolio.TotalInvested += totalInvested;
                await _uow.Portfolios.SaveCustomerPortfolio(userPortfolio);

                // COMMIT TRANSACTION
                await _uow.CommitAsync();

                _cache.Remove($"portfolio_summary_{customerId}");

                response.SetMessage("Investment processed successfully", true, new { amountInvested = totalInvested });
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                _logger.LogError(ex, "Error processing investment for customer {CustomerId}", customerId);
                response.SetError(ex.Message, 500);
            }
            return response;
        }

        public async Task<ApiResponse> SellAsync(SellRequestDto request, long customerId)
        {
            var response = new ApiResponse();

            try
            {
                // 1. Get customer holdings for this portfolio
                // We need to filter holdings specifically for the portfolio's instruments
                var portfolioInstruments = await _uow.Portfolios.GetPortfolioInstruments(request.PortfolioId);
                var instrumentIds = portfolioInstruments.Select(pi => pi.InstrumentId).ToList();

                var allHoldings = await _uow.Portfolios.GetCustomerHoldings(customerId);
                var holdings = allHoldings
                    .Where(h => instrumentIds.Contains(h.InstrumentId) && h.Units > 0)
                    .ToList();

                if (!holdings.Any())
                {
                    response.SetError("No active holdings found for this portfolio", 400);
                    return response;
                }

                // 2. Get Customer Portfolio record
                var userPortfolio = await _uow.Portfolios.GetCustomerPortfolio(customerId, request.PortfolioId);

                if (userPortfolio == null)
                {
                    response.SetError("Customer portfolio not found", 404);
                    return response;
                }

                // 3. Calculate current value across all holdings in this portfolio
                decimal totalCurrentValue = 0;
                var holdingMetrics = new List<(TblCustomerHolding holding, decimal value, decimal nav)>();

                foreach (var holding in holdings)
                {
                    var nav = await _uow.Portfolios.GetLatestNAV(holding.InstrumentId);

                    if (nav <= 0) continue;

                    var currentValue = holding.Units * nav;
                    totalCurrentValue += currentValue;

                    holdingMetrics.Add((holding, currentValue, nav));
                }

                if (totalCurrentValue == 0)
                {
                    response.SetError("The current value of your portfolio is zero", 400);
                    return response;
                }

                if (request.Amount > totalCurrentValue)
                {
                    response.SetError($"Insufficient portfolio value. Current value is {totalCurrentValue}", 400);
                    return response;
                }

                // START TRANSACTION
                await _uow.BeginTransactionAsync();

                decimal totalSoldAmount = 0;

                // 4. Proportional Sell Logic
                foreach (var item in holdingMetrics)
                {
                    // Weight of this instrument in the portfolio
                    var weight = item.value / totalCurrentValue;
                    var targetSellAmount = request.Amount * weight;
                    
                    var unitsToSell = Math.Round(targetSellAmount / item.nav, 6);

                    // Caps units to what is available
                    if (unitsToSell > item.holding.Units)
                        unitsToSell = item.holding.Units;

                    var actualSellAmount = unitsToSell * item.nav;

                    // 5. Reduce holdings
                    // Calculate proportional invested amount reduction (FIFO/Weighted Average concept)
                    if (item.holding.Units > 0)
                    {
                        var reductionRatio = unitsToSell / item.holding.Units;
                        var investedReduction = item.holding.InvestedAmount * reductionRatio;
                        
                        item.holding.Units -= unitsToSell;
                        item.holding.InvestedAmount -= investedReduction;
                    }
                    else
                    {
                        item.holding.Units = 0;
                        item.holding.InvestedAmount = 0;
                    }

                    await _uow.Portfolios.SaveCustomerHolding(item.holding);

                    // 6. Log transaction
                    await _uow.Portfolios.SaveInvestmentTransaction(new TblInvestmentTransaction
                    {
                        CustomerId = customerId,
                        PortfolioId = request.PortfolioId,
                        InstrumentId = item.holding.InstrumentId,
                        Amount = actualSellAmount,
                        Units = unitsToSell,
                        NAV = item.nav,
                        TransactionType = "SELL",
                        CreatedAt = DateTime.UtcNow
                    });

                    totalSoldAmount += actualSellAmount;
                }

                // 7. Update portfolio total invested
                userPortfolio.TotalInvested -= totalSoldAmount;
                if (userPortfolio.TotalInvested < 0) userPortfolio.TotalInvested = 0;
                await _uow.Portfolios.SaveCustomerPortfolio(userPortfolio);

                // 8. Credit Wallet
                await CreditWallet(customerId, totalSoldAmount);

                // COMMIT
                await _uow.CommitAsync();

                _cache.Remove($"portfolio_summary_{customerId}");

                response.SetMessage("Portfolio liquidation processed successfully", true, new { amountSold = totalSoldAmount });
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                _logger.LogError(ex, "Error processing sell for customer {CustomerId}", customerId);
                response.SetError(ex.Message, 500);
            }
            return response;
        }

        public async Task<ApiResponse> GetPortfolioSummary(long customerId)
        {
            var response = new ApiResponse();
            try
            {
                var customerInfo = await _uow.Customers.GetByUserId(customerId);
                if (customerInfo == null) return new ApiResponse { Message = "Customer not found", Status = 404 };

                string cacheKey = $"portfolio_summary_{customerInfo.CustomerId}";
                
                var summary = await _cache.GetOrCreateAsync(cacheKey, async entry => {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                    return await _uow.Portfolios.GetPortfolioSummaryMetrics(customerInfo.CustomerId);
                });

                response.SetMessage("Portfolio summary retrieved successfully", true, summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting portfolio summary for customer {CustomerId}", customerId);
                response.SetError(ex.Message, 500);
            }
            return response;
        }

        private async Task CreditWallet(long customerId, decimal amount)
        {
            var wallet = await _uow.Wallets.GetByCustomerId(customerId);
            if (wallet == null)
            {
                throw new Exception("Customer wallet not found");
            }

            wallet.Balance += amount;
            await _uow.Wallets.UpdateWallet(wallet);
        }
    }
}
