using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DogoFinance.DataAccess.Layer.Models.Entities;
using DogoFinance.DataAccess.Layer.Interfaces;
using DogoFinance.DataAccess.Layer.Repositories.Base;
using DogoFinance.ReportManagement.Interfaces;
using DogoFinance.ReportManagement.Models.Dtos;

namespace DogoFinance.ReportManagement.Services
{
    public class CustomerReportService : DataRepository, ICustomerReportService
    {
        private readonly IUnitOfWork _uow;

        public CustomerReportService(IUnitOfWork uow)
        {
            _uow = uow;
            SetSharedRepository(_uow.GenericRepository);
        }

        public async Task<ClientOnboardingReportDto> GetClientOnboardingReportAsync(DateTime? startDate, DateTime? endDate, int pageNumber = 1, int pageSize = 10)
        {
            var companyProfile = await GetCompanyProfileAsync();
            var queryStartDate = startDate ?? DateTime.UtcNow.AddDays(-30);
            var queryEndDate = endDate ?? DateTime.UtcNow;
            
            var query = BaseRepository().AsQueryable<TblCustomer>()
                .Include(c => c.User)
                .Where(c => !c.IsDeleted && c.User != null && !c.User.IsDeleted);

            var allCustomers = await query.ToListAsync();

            var newUsers = allCustomers.Where(c => c.CreatedAt >= queryStartDate && c.CreatedAt <= queryEndDate).ToList();

            var totalRecords = newUsers.Count;
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            var report = new ClientOnboardingReportDto
            {
                TotalRecords = totalRecords,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                CompanyProfile = companyProfile,
                TotalNewUsers = newUsers.Count,
                KycVerified = allCustomers.Count(c => c.Kycstatus == 3),
                KycPending = allCustomers.Count(c => c.Kycstatus == 2),
                KycUnverified = allCustomers.Count(c => c.Kycstatus == 1 || c.Kycstatus == 0),
                RecentSignups = newUsers
                    .OrderByDescending(c => c.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(c => new NewUserSignupDto
                    {
                        Name = $"{c.FirstName} {c.LastName}".Trim(),
                        Email = c.User.Email,
                        DateJoined = c.CreatedAt,
                        KycStatus = GetKycStatusString(c.Kycstatus)
                    }).ToList()
            };

            return report;
        }

        public async Task<ClientActivityReportDto> GetClientActivityReportAsync(DateTime? startDate, DateTime? endDate, int pageNumber = 1, int pageSize = 10)
        {
            var companyProfile = await GetCompanyProfileAsync();
            var queryStartDate = startDate ?? DateTime.UtcNow.AddDays(-30);
            var queryEndDate = endDate ?? DateTime.UtcNow;
            
            var users = await BaseRepository().AsQueryable<TblUser>()
                .Include(u => u.TblCustomer)
                .Where(u => !u.IsDeleted && !u.IsSystemUser)
                .ToListAsync();

            var activeUsers = users.Count(u => u.LastLoginDate >= queryStartDate && u.LastLoginDate <= queryEndDate);
            var inactiveUsers = users.Count - activeUsers;

            var totalRecords = users.Count;
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            var activityLogs = users
                .OrderByDescending(u => u.LastLoginDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new ClientActivityItemDto
                {
                    Name = $"{u.TblCustomer?.FirstName} {u.TblCustomer?.LastName}".Trim(),
                    Email = u.Email,
                    LastLoginDate = u.LastLoginDate,
                    Status = (u.LastLoginDate >= queryStartDate && u.LastLoginDate <= queryEndDate) ? "Active" : "Inactive"
                }).ToList();

            return new ClientActivityReportDto
            {
                TotalRecords = totalRecords,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                CompanyProfile = companyProfile,
                TotalActiveUsers = activeUsers,
                TotalInactiveUsers = inactiveUsers,
                ActivityLogs = activityLogs
            };
        }

        public async Task<ClientPortfolioReportDto> GetClientPortfolioReportAsync(int pageNumber = 1, int pageSize = 10)
        {
            var companyProfile = await GetCompanyProfileAsync();
            var portfolios = await BaseRepository().AsQueryable<TblCustomerPortfolio>()
                .Include(cp => cp.Customer)
                    .ThenInclude(c => c.User)
                .Include(cp => cp.Portfolio)
                .ToListAsync();

            var totalInvestment = portfolios.Sum(p => p.InvestedAmount);
            var totalPortfolioValue = portfolios.Sum(p => p.TotalInvested);

            var productSpread = portfolios
                .GroupBy(p => p.Portfolio.Name)
                .Select(g => new ProductSpreadDto
                {
                    ProductName = g.Key,
                    AmountInvested = g.Sum(p => p.InvestedAmount)
                })
                .ToList();

            var allClientPortfolios = portfolios
                .GroupBy(p => new { p.CustomerId, p.Customer.FirstName, p.Customer.LastName, p.Customer.User.Email })
                .Select(g => new ClientPortfolioItemDto
                {
                    ClientName = $"{g.Key.FirstName} {g.Key.LastName}".Trim(),
                    Email = g.Key.Email,
                    TotalInvested = g.Sum(p => p.InvestedAmount),
                    PortfolioValue = g.Sum(p => p.TotalInvested)
                })
                .OrderByDescending(c => c.PortfolioValue)
                .ToList();

            var totalRecords = allClientPortfolios.Count;
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            var paginatedClientPortfolios = allClientPortfolios
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new ClientPortfolioReportDto
            {
                TotalRecords = totalRecords,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                CompanyProfile = companyProfile,
                TotalInvestment = totalInvestment,
                TotalPortfolioValue = totalPortfolioValue,
                ProductSpread = productSpread,
                ClientPortfolios = paginatedClientPortfolios
            };
        }

        private string GetKycStatusString(int status)
        {
            return status switch
            {
                3 => "Verified",
                2 => "Pending",
                1 => "Unverified",
                0 => "Unverified",
                _ => "Unknown"
            };
        }

        private async Task<CompanyProfileDto?> GetCompanyProfileAsync()
        {
            var company = await BaseRepository().AsQueryable<TblCompanyProfile>().FirstOrDefaultAsync();
            if (company == null) return null;

            return new CompanyProfileDto
            {
                CompanyName = company.CompanyName,
                Address = company.Address
            };
        }
    }
}
