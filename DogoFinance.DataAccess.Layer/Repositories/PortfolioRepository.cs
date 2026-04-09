using DogoFinance.DataAccess.Layer.Interfaces.Repositories;
using DogoFinance.DataAccess.Layer.Models.Entities;
using DogoFinance.DataAccess.Layer.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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
            => await BaseRepository().FindList<TblPortfolio>();

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
    }
}
