using DogoFinance.DataAccess.Layer.Models.Entities;

namespace DogoFinance.DataAccess.Layer.Interfaces.Repositories
{
    public interface IReservedAccountRepository
    {
        Task<TblReservedAccount?> GetByAccountReference(string reference);
        Task<TblReservedAccount?> GetByUserId(long userId);
        Task SaveReservedAccount(TblReservedAccount account);
    }
}
