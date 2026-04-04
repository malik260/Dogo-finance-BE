using DogoFinance.DataAccess.Layer.Models.Entities;

namespace DogoFinance.DataAccess.Layer.Interfaces.Repositories
{
    public interface ILedgerRepository
    {
        Task CreateEntry(TblLedger entry);
        Task<decimal> GetTotalInflows();
        Task<decimal> GetTotalOutflows();
        Task<IEnumerable<TblLedger>> GetAccountStatement(long walletId);
    }
}
