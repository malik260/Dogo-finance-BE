using DogoFinance.DataAccess.Layer.Interfaces.Repositories;
using DogoFinance.DataAccess.Layer.Models.Entities;
using DogoFinance.DataAccess.Layer.Repositories.Base;
using DogoFinance.DataAccess.Layer.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogoFinance.DataAccess.Layer.Repositories
{
    public class PortfolioRepository : DataRepository, IPortfolioRepository
    {
        // Asset Class
        public async Task<IEnumerable<TblAssetClass>> GetAssetClasses()
            => await BaseRepository().FindList<TblAssetClass>();

        public async Task<TblAssetClass?> GetAssetClassById(int id)
            => await BaseRepository().FindEntity<TblAssetClass>(id);

        public async Task SaveAssetClass(TblAssetClass assetClass)
        {
            if (assetClass.AssetClassId == 0) await BaseRepository().Insert(assetClass);
            else await BaseRepository().Update(assetClass);
        }

        public async Task DeleteAssetClass(int id)
        {
            var entity = await GetAssetClassById(id);
            if (entity != null) await BaseRepository().Delete(entity);
        }

        // Portfolio Type
        public async Task<IEnumerable<TblPortfolioType>> GetPortfolioTypes()
            => await BaseRepository().FindList<TblPortfolioType>();

        public async Task<TblPortfolioType?> GetPortfolioTypeById(int id)
            => await BaseRepository().FindEntity<TblPortfolioType>(id);

        public async Task SavePortfolioType(TblPortfolioType type)
        {
            if (type.PortfolioTypeId == 0) await BaseRepository().Insert(type);
            else await BaseRepository().Update(type);
        }

        public async Task DeletePortfolioType(int id)
        {
            var entity = await GetPortfolioTypeById(id);
            if (entity != null) await BaseRepository().Delete(entity);
        }

        // Portfolio
        public async Task<IEnumerable<TblPortfolio>> GetPortfolios()
            => await BaseRepository().AsQueryable<TblPortfolio>(p => true)
                .Include(p => p.PortfolioType)
                .ToListAsync();

        public async Task<IEnumerable<TblPortfolio>> GetPortfoliosDetailed()
            => await BaseRepository().AsQueryable<TblPortfolio>(p => true)
                .Include(p => p.PortfolioType)
                .ToListAsync();

        public async Task<TblPortfolio?> GetPortfolioById(int id)
            => await BaseRepository().FindEntity<TblPortfolio>(id);

        public async Task SavePortfolio(TblPortfolio portfolio)
        {
            if (portfolio.PortfolioId == 0) await BaseRepository().Insert(portfolio);
            else await BaseRepository().Update(portfolio);
        }

        public async Task DeletePortfolio(int id)
        {
            var entity = await GetPortfolioById(id);
            if (entity != null) await BaseRepository().Delete(entity);
        }

        // Instrument
        public async Task<IEnumerable<TblInstrument>> GetInstruments()
            => await BaseRepository().FindList<TblInstrument>();

        public async Task<TblInstrument?> GetInstrumentById(int id)
            => await BaseRepository().FindEntity<TblInstrument>(id);

        public async Task<IEnumerable<InstrumentDto>> GetInstrumentsDetailed()
        {
            var instruments = await BaseRepository().AsQueryable<TblInstrument>(x => true)
                .ToListAsync();

            var data = new List<InstrumentDto>();
            foreach (var inst in instruments)
            {
                var priceInfo = await GetInstrumentPriceInfo(inst.InstrumentId);
                data.Add(new InstrumentDto
                {
                    InstrumentId = inst.InstrumentId,
                    Name = inst.Name,
                    Code = inst.Code,
                    //AssetClassId = inst.AssetClassId,
                    //AssetTypeName = inst.AssetClass?.Name,
                    UnitPrice = priceInfo?.NAV ?? 0,
                    IsShariahCompliant = inst.IsShariahCompliant,
                    IsActive = inst.IsActive,
                    PriceDate = priceInfo?.PriceDate,
                    PriceSource = priceInfo?.PriceSource,
                });
            }
            return data;
        }

        public async Task SaveInstrument(TblInstrument instrument)
        {
            if (instrument.InstrumentId == 0) await BaseRepository().Insert(instrument);
            else await BaseRepository().Update(instrument);
        }

        public async Task DeleteInstrument(int id)
        {
            var entity = await GetInstrumentById(id);
            if (entity != null) await BaseRepository().Delete(entity);
        }

        // Portfolio Instrument
        public async Task<IEnumerable<TblPortfolioInstrument>> GetPortfolioInstruments(int portfolioId)
            => await BaseRepository().FindList<TblPortfolioInstrument>(n => n.PortfolioId == portfolioId);

        public async Task<TblPortfolioInstrument?> GetPortfolioInstrumentById(int id)
            => await BaseRepository().FindEntity<TblPortfolioInstrument>(id);

        public async Task SavePortfolioInstrument(TblPortfolioInstrument pi)
        {
            if (pi.Id == 0) await BaseRepository().Insert(pi);
            else await BaseRepository().Update(pi);
        }

        public async Task DeletePortfolioInstrument(int id)
        {
            var entity = await GetPortfolioInstrumentById(id);
            if (entity != null) await BaseRepository().Delete(entity);
        }

        // Allocation Rule
        public async Task<IEnumerable<TblPortfolioAllocationRule>> GetAllocationRules(int portfolioId)
            => await BaseRepository().FindList<TblPortfolioAllocationRule>(n => n.PortfolioId == portfolioId);

        public async Task<TblPortfolioAllocationRule?> GetAllocationRuleById(int id)
            => await BaseRepository().FindEntity<TblPortfolioAllocationRule>(id);

        public async Task SaveAllocationRule(TblPortfolioAllocationRule rule)
        {
            if (rule.Id == 0) await BaseRepository().Insert(rule);
            else await BaseRepository().Update(rule);
        }

        public async Task DeleteAllocationRule(int id)
        {
            var entity = await GetAllocationRuleById(id);
            if (entity != null) await BaseRepository().Delete(entity);
        }

        // Instrument Price
        public async Task<IEnumerable<TblInstrumentPrice>> GetInstrumentPrices(int instrumentId)
            => await BaseRepository().FindList<TblInstrumentPrice>(n => n.InstrumentId == instrumentId);

        public async Task<TblInstrumentPrice?> GetInstrumentPriceById(int id)
            => await BaseRepository().FindEntity<TblInstrumentPrice>(id);

        public async Task SaveInstrumentPrice(TblInstrumentPrice price)
        {
            if (price.Id == 0) await BaseRepository().Insert(price);
            else await BaseRepository().Update(price);
        }

        public async Task DeleteInstrumentPrice(int id)
        {
            var entity = await GetInstrumentPriceById(id);
            if (entity != null) await BaseRepository().Delete(entity);
        }

        // Customer Portfolio
        public async Task<IEnumerable<TblCustomerPortfolio>> GetCustomerPortfolios(long customerId)
            => await BaseRepository().FindList<TblCustomerPortfolio>(n => n.CustomerId == customerId);

        public async Task<TblCustomerPortfolio?> GetCustomerPortfolioById(long id)
            => await BaseRepository().FindEntity<TblCustomerPortfolio>(id);

        public async Task<TblCustomerPortfolio?> GetCustomerPortfolio(long customerId, int portfolioId)
            => await BaseRepository().FindEntity<TblCustomerPortfolio>(n => n.CustomerId == customerId && n.PortfolioId == portfolioId);

        public async Task SaveCustomerPortfolio(TblCustomerPortfolio cp)
        {
            if (cp.Id == 0) await BaseRepository().Insert(cp);
            else await BaseRepository().Update(cp);
        }

        public async Task DeleteCustomerPortfolio(long id)
        {
            var entity = await GetCustomerPortfolioById(id);
            if (entity != null) await BaseRepository().Delete(entity);
        }

        // Customer Holding
        public async Task<IEnumerable<TblCustomerHolding>> GetCustomerHoldings(long customerId)
            => await BaseRepository().FindList<TblCustomerHolding>(n => n.CustomerId == customerId);

        public async Task<TblCustomerHolding?> GetCustomerHoldingById(long id)
            => await BaseRepository().FindEntity<TblCustomerHolding>(id);

        public async Task<TblCustomerHolding?> GetCustomerHolding(long customerId, int instrumentId)
            => await BaseRepository().FindEntity<TblCustomerHolding>(n => n.CustomerId == customerId && n.InstrumentId == instrumentId);

        public async Task SaveCustomerHolding(TblCustomerHolding ch)
        {
            if (ch.Id == 0) await BaseRepository().Insert(ch);
            else await BaseRepository().Update(ch);
        }

        public async Task DeleteCustomerHolding(long id)
        {
            var entity = await GetCustomerHoldingById(id);
            if (entity != null) await BaseRepository().Delete(entity);
        }

        public async Task<IEnumerable<TblPortfolioInstrument>> GetInstrumentsDetailed(int portfolioId)
            => await BaseRepository().AsQueryable<TblPortfolioInstrument>(n => n.PortfolioId == portfolioId)
                .Include(n => n.Instrument)
                .ToListAsync();

        public async Task<decimal> GetLatestNAV(int instrumentId)
            => await BaseRepository().AsQueryable<TblInstrumentPrice>(n => n.InstrumentId == instrumentId)
                .OrderByDescending(n => n.PriceDate)
                .Select(n => n.NAV)
                .FirstOrDefaultAsync();

        public async Task<TblInstrumentPrice> GetInstrumentPriceInfo(int instrumentId)
          => await BaseRepository().AsQueryable<TblInstrumentPrice>(n => n.InstrumentId == instrumentId)
              .OrderByDescending(n => n.PriceDate)
              .FirstOrDefaultAsync();

        public async Task SaveInvestmentTransaction(TblInvestmentTransaction tx)
            => await BaseRepository().Insert(tx);

        public async Task<PortfolioSummaryDto> GetPortfolioSummaryMetrics(long customerId)
        {
            // 1. Total invested
            var totalInvested = await BaseRepository().AsQueryable<TblCustomerPortfolio>(x => x.CustomerId == customerId)
                .SumAsync(x => x.TotalInvested);

            // 2. Get holdings + latest NAV
            // Since we use the BaseRepository which is a wrapper, we'll use the context directly via the shared repo if possible,
            // or just use AsQueryable to join.
            
            var query = from h in BaseRepository().AsQueryable<TblCustomerHolding>(h => h.CustomerId == customerId)
                        join p in BaseRepository().AsQueryable<TblInstrumentPrice>(p => true)
                            on h.InstrumentId equals p.InstrumentId
                        select new { h, p };

            var details = await query.ToListAsync();
            
            var grouped = details.GroupBy(x => x.h.InstrumentId);
            
            decimal totalValue = 0;
            
            foreach (var group in grouped)
            {
                var latestPrice = group.OrderByDescending(x => x.p.PriceDate).FirstOrDefault();
                if (latestPrice != null)
                {
                    totalValue += latestPrice.h.Units * latestPrice.p.NAV;
                }
            }

            var profit = totalValue - totalInvested;
            var returnPercentage = totalInvested == 0 ? 0 : (profit / totalInvested) * 100;

            return new PortfolioSummaryDto
            {
                Invested = Math.Round(totalInvested, 2),
                CurrentValue = Math.Round(totalValue, 2),
                Profit = Math.Round(profit, 2),
                ReturnPercentage = Math.Round(returnPercentage, 2)
            };
        }
    }
}
