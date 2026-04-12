using DogoFinance.DataAccess.Layer.Interfaces;
using DogoFinance.DataAccess.Layer.Interfaces.Repositories;
using DogoFinance.DataAccess.Layer.Repositories;
using DogoFinance.DataAccess.Layer.Repositories.Base;
using System.Threading.Tasks;

namespace DogoFinance.DataAccess.Layer
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DogoFinance.DataAccess.Layer.Repositories.Base.DogoDbContext _context;
        private readonly DogoFinance.DataAccess.Layer.Interfaces.Base.IDbRepository _sharedRepo;

        public UnitOfWork()
        {
            _context = new DogoFinance.DataAccess.Layer.Repositories.Base.DogoDbContext();
            _sharedRepo = new DogoFinance.DataAccess.Layer.Repositories.Base.DbRepository(_context);
        }

        private T CreateRepository<T>() where T : DataRepository, new()
        {
            var repo = new T();
            repo.SetSharedRepository(_sharedRepo);
            return repo;
        }

        private IUserRepository? _users;
        public IUserRepository Users => _users ??= CreateRepository<UserRepository>();

        private ICustomerRepository? _customers;
        public ICustomerRepository Customers => _customers ??= CreateRepository<CustomerRepository>();

        private IWalletRepository? _wallets;
        public IWalletRepository Wallets => _wallets ??= CreateRepository<WalletRepository>();

        private IPaymentRepository? _payments;
        public IPaymentRepository Payments => _payments ??= CreateRepository<PaymentRepository>();

        private ITransactionRepository? _transactions;
        public ITransactionRepository Transactions => _transactions ??= CreateRepository<TransactionRepository>();

        private INextOfKinRepository? _nok;
        public INextOfKinRepository NextOfKins => _nok ??= CreateRepository<NextOfKinRepository>();

        private ILedgerRepository? _ledger;
        public ILedgerRepository Ledgers => _ledger ??= CreateRepository<LedgerRepository>();

        private IPortfolioRepository? _portfolios;
        public IPortfolioRepository Portfolios => _portfolios ??= CreateRepository<PortfolioRepository>();

        private ISystemSettingRepository? _settings;
        public ISystemSettingRepository SystemSettings => _settings ??= CreateRepository<SystemSettingRepository>();

        private IReservedAccountRepository? _reservedAccounts;
        public IReservedAccountRepository ReservedAccounts => _reservedAccounts ??= CreateRepository<ReservedAccountRepository>();

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
        public async Task BeginTransactionAsync() => await _sharedRepo.BeginTrans();
        public async Task CommitAsync() => await _sharedRepo.CommitTrans();
        public async Task RollbackAsync() => await _sharedRepo.RollbackTrans();
    }
}
