using Microsoft.EntityFrameworkCore;
using DogoFinance.DataAccess.Layer.Models.Entities;
using DogoFinance.DataAccess.Layer.Interfaces.Repositories;
using DogoFinance.DataAccess.Layer.Repositories.Base;

namespace DogoFinance.DataAccess.Layer.Repositories
{
    public class ReservedAccountRepository : DataRepository, IReservedAccountRepository
    {
        public async Task<TblReservedAccount?> GetByAccountReference(string reference)
        {
            return await BaseRepository().FindEntity<TblReservedAccount>(x => x.AccountReference == reference);
        }

        public async Task<TblReservedAccount?> GetByUserId(long userId)
        {
            return await BaseRepository().FindEntity<TblReservedAccount>(x => x.UserId == userId);
        }

        public async Task SaveReservedAccount(TblReservedAccount account)
        {
            if (account.Id == 0)
                await BaseRepository().Insert(account);
            else
                await BaseRepository().Update(account);
        }
    }
}
