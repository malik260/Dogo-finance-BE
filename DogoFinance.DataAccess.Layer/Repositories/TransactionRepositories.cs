using DogoFinance.DataAccess.Layer.Interfaces.Repositories;
using DogoFinance.DataAccess.Layer.Models.Entities;
using DogoFinance.DataAccess.Layer.Repositories.Base;

namespace DogoFinance.DataAccess.Layer.Repositories
{
    public class WalletRepository : DataRepository, IWalletRepository
    {
        public async Task<TblWallet?> GetByCustomerId(long customerId)
            => await BaseRepository().FindEntity<TblWallet>(w => w.CustomerId == customerId);

        public async Task CreateWallet(TblWallet wallet)
        {
            try
            {
                await BaseRepository().Insert(wallet);

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task UpdateWallet(TblWallet wallet)
        {
            await BaseRepository().Update(wallet);
        }
    }

    public class PaymentRepository : DataRepository, IPaymentRepository
    {
        public async Task<TblPayment?> GetByReference(string reference)
            => await BaseRepository().FindEntity<TblPayment>(p => p.ProviderReference == reference);

        public async Task SavePayment(TblPayment payment)
        {
            if (payment.Id == 0)
                await BaseRepository().Insert(payment);
            else
                await BaseRepository().Update(payment);
        }
    }

    public class TransactionRepository : DataRepository, ITransactionRepository
    {
        public async Task CreateTransaction(TblTransaction transaction)
        {
            try
            {
                await BaseRepository().Insert(transaction);

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<IEnumerable<TblTransaction>> GetByUserId(long userId)
            => await BaseRepository().FindList<TblTransaction>(t => t.InitiatedByUserId == userId);
    }
}
