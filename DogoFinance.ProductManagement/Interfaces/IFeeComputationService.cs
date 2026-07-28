using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DogoFinance.BusinessLogic.Layer.Response;

namespace DogoFinance.ProductManagement.Interfaces
{
    public class PortfolioFeeConfigDto
    {
        public int Id { get; set; }
        public int PortfolioId { get; set; }
        public string FeeType { get; set; } = null!;
        public decimal PercentagePerAnnum { get; set; }
        public string CalculationBasis { get; set; } = "AVERAGE_MONTH_END_NAV";
        public string BillingFrequency { get; set; } = "QUARTERLY";
        public int ChargeDayOfMonth { get; set; } = 10;
        public string TargetAccountCode { get; set; } = null!;
        public bool IsLiability { get; set; }
        public bool IsWaived { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class FeeCalculationPreviewDto
    {
        public long CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int PortfolioId { get; set; }
        public string PortfolioName { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Quarter { get; set; }
        public decimal Month1Nav { get; set; }
        public decimal Month2Nav { get; set; }
        public decimal Month3Nav { get; set; }
        public decimal AverageNav { get; set; }
        public string FeeType { get; set; } = string.Empty;
        public decimal FeeRatePerAnnum { get; set; }
        public decimal CalculatedQuarterlyFee { get; set; }
        public string TargetAccountCode { get; set; } = string.Empty;
        public bool IsAlreadyExecuted { get; set; }
    }

    public interface IFeeComputationService
    {
        // Fee Config CRUD
        Task<ApiResponse> GetProductFeeConfigsAsync(int portfolioId);
        Task<ApiResponse> SaveProductFeeConfigAsync(PortfolioFeeConfigDto dto);
        Task<ApiResponse> ToggleWaiveFeeConfigAsync(int configId, bool isWaived);
        Task<ApiResponse> SeedDefaultFeeConfigsAsync();

        // Computation & Deduction Engine
        Task<ApiResponse> PreviewQuarterlyFeesAsync(int year, int quarter, int? portfolioId = null);
        Task<ApiResponse> ExecuteQuarterlyFeeDeductionsAsync(int year, int quarter, int? portfolioId = null);
        Task<ApiResponse> GetSecRemittanceReportAsync(int year, int quarter);
    }
}
