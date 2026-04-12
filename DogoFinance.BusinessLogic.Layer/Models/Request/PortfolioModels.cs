using System;
using DogoFinance.DataAccess.Layer.DTO;

namespace DogoFinance.BusinessLogic.Layer.Models.Request
{
    // These DTOs have been moved to DataAccess.Layer.DTO to avoid circular dependencies.
    // We import them here so existing business logic services can still find them in the expected namespace
    // or we can update the services to import the new namespace.
    
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
