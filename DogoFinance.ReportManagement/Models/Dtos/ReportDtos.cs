using System;
using System.Collections.Generic;

namespace DogoFinance.ReportManagement.Models.Dtos
{
    public class ClientOnboardingReportDto
    {
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public CompanyProfileDto? CompanyProfile { get; set; }
        
        public int TotalNewUsers { get; set; }
        public int KycVerified { get; set; }
        public int KycUnverified { get; set; }
        public int KycPending { get; set; }
        public List<NewUserSignupDto> RecentSignups { get; set; } = new();
    }

    public class NewUserSignupDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DateJoined { get; set; }
        public string KycStatus { get; set; } = string.Empty;
    }

    public class ClientActivityReportDto
    {
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public CompanyProfileDto? CompanyProfile { get; set; }
        
        public int TotalActiveUsers { get; set; }
        public int TotalInactiveUsers { get; set; }
        public List<ClientActivityItemDto> ActivityLogs { get; set; } = new();
    }

    public class ClientActivityItemDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime? LastLoginDate { get; set; }
        public string Status { get; set; } = string.Empty; // "Active" or "Inactive"
    }

    public class ClientPortfolioReportDto
    {
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public CompanyProfileDto? CompanyProfile { get; set; }
        
        public decimal TotalInvestment { get; set; }
        public decimal TotalPortfolioValue { get; set; }
        public List<ProductSpreadDto> ProductSpread { get; set; } = new();
        public List<ClientPortfolioItemDto> ClientPortfolios { get; set; } = new();
    }

    public class ProductSpreadDto
    {
        public string ProductName { get; set; } = string.Empty;
        public decimal AmountInvested { get; set; }
    }

    public class ClientPortfolioItemDto
    {
        public string ClientName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal TotalInvested { get; set; }
        public decimal PortfolioValue { get; set; }
    }

    public class CompanyProfileDto
    {
        public string CompanyName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
