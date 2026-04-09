using System;

namespace DogoFinance.BusinessLogic.Layer.Models.Request
{
    public class AssetClassDto
    {
        public int AssetClassId { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public bool IsShariahCompliant { get; set; }
    }

    public class PortfolioTypeDto
    {
        public int PortfolioTypeId { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public bool SupportsAllocation { get; set; }
    }

    public class PortfolioDto
    {
        public int PortfolioId { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public int PortfolioTypeId { get; set; }
        public string? RiskLevel { get; set; }
        public string? Description { get; set; }
        public decimal? ExpectedAnnualReturn { get; set; }
        public bool IsActive { get; set; }
    }

    public class InstrumentDto
    {
        public int InstrumentId { get; set; }
        public string Name { get; set; } = null!;
        public string? Code { get; set; }
        public int AssetClassId { get; set; }
        public bool IsShariahCompliant { get; set; }
        public bool IsActive { get; set; }
    }

    public class PortfolioInstrumentDto
    {
        public int Id { get; set; }
        public int PortfolioId { get; set; }
        public int AssetClassId { get; set; }
        public decimal TargetWeight { get; set; }
    }

    public class PortfolioAllocationRuleDto
    {
        public int Id { get; set; }
        public int PortfolioId { get; set; }
        public int AssetClassId { get; set; }
        public decimal TargetPercentage { get; set; }
        public decimal MinPercentage { get; set; }
        public decimal MaxPercentage { get; set; }
        public decimal ExpectedReturn { get; set; }
    }

    public class InstrumentPriceDto
    {
        public int Id { get; set; }
        public int InstrumentId { get; set; }
        public DateTime PriceDate { get; set; }
        public decimal NAV { get; set; }
        public string? PriceSource { get; set; }
    }

    public class CustomerPortfolioDto
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public int PortfolioId { get; set; }
        public decimal TotalInvested { get; set; }
    }

    public class CustomerHoldingDto
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public int InstrumentId { get; set; }
        public decimal Units { get; set; }
        public decimal InvestedAmount { get; set; }
    }
}
