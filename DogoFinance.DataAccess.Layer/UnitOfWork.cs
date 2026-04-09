using DogoFinance.DataAccess.Layer.Interfaces;
using DogoFinance.DataAccess.Layer.Interfaces.Repositories;
using DogoFinance.DataAccess.Layer.Repositories;

namespace DogoFinance.DataAccess.Layer
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork() { }

        private IUserRepository? _users;
        public IUserRepository Users => _users ??= new UserRepository();

        private ICustomerRepository? _customers;
        public ICustomerRepository Customers => _customers ??= new CustomerRepository();

        private IWalletRepository? _wallets;
        public IWalletRepository Wallets => _wallets ??= new WalletRepository();

        private IPaymentRepository? _payments;
        public IPaymentRepository Payments => _payments ??= new PaymentRepository();

        private ITransactionRepository? _transactions;
        public ITransactionRepository Transactions => _transactions ??= new TransactionRepository();

        private INextOfKinRepository? _nok;
        public INextOfKinRepository NextOfKins => _nok ??= new NextOfKinRepository();

        private ILedgerRepository? _ledger;
        public ILedgerRepository Ledgers => _ledger ??= new LedgerRepository();

        private IPortfolioRepository? _portfolios;
        public IPortfolioRepository Portfolios => _portfolios ??= new PortfolioRepository();


        private ISystemSettingRepository? _settings;
        public ISystemSettingRepository SystemSettings => _settings ??= new SystemSettingRepository();
    }
}
