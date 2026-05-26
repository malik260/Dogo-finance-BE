using System.Threading.Tasks;
using DogoFinance.ReportManagement.Models.Dtos;

namespace DogoFinance.ReportManagement.Interfaces
{
    public interface ICustomerReportService
    {
        Task<ClientOnboardingReportDto> GetClientOnboardingReportAsync(DateTime? startDate, DateTime? endDate, int pageNumber = 1, int pageSize = 10);
        Task<ClientActivityReportDto> GetClientActivityReportAsync(DateTime? startDate, DateTime? endDate, int pageNumber = 1, int pageSize = 10);
        Task<ClientPortfolioReportDto> GetClientPortfolioReportAsync(int pageNumber = 1, int pageSize = 10);
    }
}
