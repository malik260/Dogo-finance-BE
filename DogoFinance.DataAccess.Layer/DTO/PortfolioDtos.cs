using System;
using System.Collections.Generic;

namespace DogoFinance.DataAccess.Layer.DTO
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
        public string? PortfolioTypeName { get; set; }
        public string? RiskLevel { get; set; }
        public string? Description { get; set; }
        public decimal? ExpectedAnnualReturn { get; set; }
        public bool IsActive { get; set; }
        
        public int LockInPeriodDays { get; set; }
        public int MinHoldingPeriodDays { get; set; }
        public decimal ExitFeePercentage { get; set; }
        public int NoticePeriodDays { get; set; }
        public decimal ApprovalThresholdAmount { get; set; }

        public List<PortfolioAllocationRuleDto>? Allocations { get; set; }
    }


    public class InstrumentDto
    {
        public int InstrumentId { get; set; }
        public string Name { get; set; } = null!;
        public string? Code { get; set; }       
        public bool IsShariahCompliant { get; set; }
        public bool IsActive { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime? PriceDate { get; set; }
        public string? PriceSource { get; set; }
    }

    public class PortfolioAllocationRuleDto
    {
        public int Id { get; set; }
        public int PortfolioId { get; set; }
        public int AssetClassId { get; set; }
        public string? AssetClassName { get; set; }
        public decimal TargetPercentage { get; set; }
        public decimal MinPercentage { get; set; }
        public decimal MaxPercentage { get; set; }
        public decimal ExpectedReturn { get; set; }
        public List<PortfolioInstrumentDto>? Instruments { get; set; }
    }

    public class PortfolioInstrumentDto
    {
        public int Id { get; set; }
        public int PortfolioId { get; set; }
        public int AssetClassId { get; set; }
        public int InstrumentId { get; set; }
        public string? InstrumentName { get; set; }
        public decimal Percentage { get; set; }
    }

    public class SimulateNavRequest
    {
        public int PortfolioId { get; set; }
        public int Days { get; set; }
    }
}
