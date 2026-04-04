using DogoFinance.DataAccess.Layer.Models.Entities;

namespace DogoFinance.DataAccess.Layer.Interfaces.Repositories
{
    public interface ITransactionRepository
    {
        Task CreateTransaction(TblTransaction transaction);
        Task<IEnumerable<TblTransaction>> GetByUserId(long userId);
    }
}
