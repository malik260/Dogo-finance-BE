using DogoFinance.DataAccess.Layer.DTO;
using DogoFinance.DataAccess.Layer.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DogoFinance.DataAccess.Layer.Interfaces.Repositories
{
    public interface IPortfolioRepository
    {
        // Asset Class
        Task<IEnumerable<TblAssetClass>> GetAssetClasses();
        Task<TblAssetClass?> GetAssetClassById(int id);
        Task SaveAssetClass(TblAssetClass assetClass);
        Task DeleteAssetClass(int id);

        // Portfolio Type
        Task<IEnumerable<TblPortfolioType>> GetPortfolioTypes();
        Task<TblPortfolioType?> GetPortfolioTypeById(int id);
        Task SavePortfolioType(TblPortfolioType type);
        Task DeletePortfolioType(int id);

        // Portfolio
        Task<IEnumerable<TblPortfolio>> GetPortfolios();
        Task<IEnumerable<TblPortfolio>> GetPortfoliosDetailed();
        Task<TblPortfolio?> GetPortfolioById(int id);
        Task SavePortfolio(TblPortfolio portfolio);
        Task DeletePortfolio(int id);

        // Instrument
        Task<IEnumerable<TblInstrument>> GetInstruments();
        Task<IEnumerable<InstrumentDto>> GetInstrumentsDetailed();
        Task<TblInstrument?> GetInstrumentById(int id);
        Task SaveInstrument(TblInstrument instrument);
        Task DeleteInstrument(int id);

        // Portfolio Instrument
        Task<IEnumerable<TblPortfolioInstrument>> GetPortfolioInstruments(int portfolioId);
        Task<TblPortfolioInstrument?> GetPortfolioInstrumentById(int id);
        Task SavePortfolioInstrument(TblPortfolioInstrument pi);
        Task DeletePortfolioInstrument(int id);

        // Allocation Rule
        Task<IEnumerable<TblPortfolioAllocationRule>> GetAllocationRules(int portfolioId);
        Task<TblPortfolioAllocationRule?> GetAllocationRuleById(int id);
        Task SaveAllocationRule(TblPortfolioAllocationRule rule);
        Task DeleteAllocationRule(int id);

        // Instrument Price
        Task<IEnumerable<TblInstrumentPrice>> GetInstrumentPrices(int instrumentId);
        Task<TblInstrumentPrice?> GetInstrumentPriceById(int id);
        Task SaveInstrumentPrice(TblInstrumentPrice price);
        Task DeleteInstrumentPrice(int id);

        // Customer Portfolio
        Task<IEnumerable<TblCustomerPortfolio>> GetCustomerPortfolios(long customerId);
        Task<TblCustomerPortfolio?> GetCustomerPortfolioById(long id);
        Task<TblCustomerPortfolio?> GetCustomerPortfolio(long customerId, int portfolioId);
        Task SaveCustomerPortfolio(TblCustomerPortfolio cp);
        Task DeleteCustomerPortfolio(long id);

        // Customer Holding
        Task<IEnumerable<TblCustomerHolding>> GetCustomerHoldings(long customerId);
        Task<TblCustomerHolding?> GetCustomerHoldingById(long id);
        Task<TblCustomerHolding?> GetCustomerHolding(long customerId, int instrumentId);
        Task SaveCustomerHolding(TblCustomerHolding ch);
        Task DeleteCustomerHolding(long id);

        // Investment Helpers
        Task<IEnumerable<TblPortfolioInstrument>> GetInstrumentsDetailed(int portfolioId);
        Task<decimal> GetLatestNAV(int instrumentId);
        Task SaveInvestmentTransaction(TblInvestmentTransaction tx);
        Task<IEnumerable<TblInvestmentTransaction>> GetInvestmentTransactionsByCustomer(long customerId);

        Task<PortfolioSummaryDto> GetPortfolioSummaryMetrics(long customerId);
        Task<TblInstrumentPrice> GetInstrumentPriceInfo(int instrumentId);
    }
}
