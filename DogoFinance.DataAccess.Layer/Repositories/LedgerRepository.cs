using DogoFinance.DataAccess.Layer.Interfaces.Repositories;
using DogoFinance.DataAccess.Layer.Models.Constants;
using DogoFinance.DataAccess.Layer.Models.Entities;
using DogoFinance.DataAccess.Layer.Repositories.Base;

namespace DogoFinance.DataAccess.Layer.Repositories
{
    public class LedgerRepository : DataRepository, ILedgerRepository
    {
        public async Task CreateEntry(TblLedger entry)
        {
            await BaseRepository().Insert(entry);
        }

        public async Task<decimal> GetTotalInflows()
        {
            var entries = await BaseRepository().FindList<TblLedger>(e => e.EntryType == EntryType.CREDIT);
            return entries.Sum(e => e.Amount);
        }

        public async Task<decimal> GetTotalOutflows()
        {
            var entries = await BaseRepository().FindList<TblLedger>(e => e.EntryType == EntryType.DEBIT);
            return entries.Sum(e => Math.Abs(e.Amount)); // Always positive amount for reporting
        }

        public async Task<IEnumerable<TblLedger>> GetAccountStatement(long walletId)
        {
            return await BaseRepository().FindList<TblLedger>(e => e.WalletId == walletId);
        }
    }
}
