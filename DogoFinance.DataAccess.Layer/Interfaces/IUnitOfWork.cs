using DogoFinance.DataAccess.Layer.Interfaces.Repositories;

namespace DogoFinance.DataAccess.Layer.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        ICustomerRepository Customers { get; }
        IWalletRepository Wallets { get; }
        IPaymentRepository Payments { get; }
        ITransactionRepository Transactions { get; }
        INextOfKinRepository NextOfKins { get; }
        ILedgerRepository Ledgers { get; }
        IProductRepository Products { get; }
        ISystemSettingRepository SystemSettings { get; }
    }
}
