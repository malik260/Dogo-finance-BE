using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.DataAccess.Layer.DTO;

namespace DogoFinance.AccountingManagement.Interfaces
{
    public interface IAccountingService
    {
        Task<ApiResponse> SeedChartOfAccountsAsync();
        Task<ApiResponse> GetTrialBalanceAsync();
        Task<ApiResponse> PostJournalAsync(JournalEntryDto entry);
        Task<ApiResponse> GetChartOfAccountsAsync();
    }
}
