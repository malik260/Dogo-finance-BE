using DogoFinance.DataAccess.Layer.Models.Entities;

namespace DogoFinance.DataAccess.Layer.Interfaces.Repositories
{
    public interface IWalletRepository
    {
        Task<TblWallet?> GetByCustomerId(long customerId);
        Task UpdateWallet(TblWallet wallet);
        Task CreateWallet(TblWallet wallet);
    }
}
