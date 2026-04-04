using DogoFinance.BusinessLogic.Layer.Helpers;
using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.DataAccess.Layer.Interfaces;
using DogoFinance.DataAccess.Layer.Models.Constants;
using DogoFinance.DataAccess.Layer.Models.Entities;
using DogoFinance.DataAccess.Layer.Repositories.Base;
using DogoFinance.Integration.Interfaces;
using DogoFinance.Integration.Models.Monnify;
using DogoFinance.TransactionManagement.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace DogoFinance.TransactionManagement.Services
{
    public class TransactionService : DataRepository, ITransactionService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMonnifyService _monnifyService;
        private readonly ILogger<TransactionService> _logger;
        private readonly IConfiguration _configuration;

        public TransactionService(IUnitOfWork uow, IMonnifyService monnifyService, ILogger<TransactionService> logger, IConfiguration configuration)
        {
            _uow = uow;
            _monnifyService = monnifyService;
            _logger = logger;
            _configuration = configuration;
        }

        private async Task LogLedger(long transactionId, long walletId, int entryType, decimal amount, decimal balanceAfter, string narration)
        {
            var entry = new TblLedger
            {
                TransactionId = transactionId,
                WalletId = walletId,
                EntryType = entryType,
                Amount = amount,
                BalanceAfter = balanceAfter,
                Narration = narration,
                CreatedAt = DateTime.UtcNow
            };
            await _uow.Ledgers.CreateEntry(entry);
        }

        public async Task<ApiResponse> InitiateDeposit(long customerId, decimal amount)
        {
            var response = new ApiResponse();
            var customer = await BaseRepository().FindEntity<TblCustomer>(customerId);
            if (customer == null) return new ApiResponse { Message = "Customer not found", Status = 404 };

            var user = await BaseRepository().FindEntity<TblUser>(customer.UserId);
            if (user == null) return new ApiResponse { Message = "User account not found", Status = 404 };

            var reference = $"DEP_{DateTime.UtcNow.Ticks}";

            var payment = new TblPayment
            {
                UserId = user.UserId,
                Amount = amount,
                PaymentProvider = 1, // Monnify
                ProviderReference = reference,
                Status = "PENDING",
                CreatedAt = DateTime.UtcNow
            };
            await _uow.Payments.SavePayment(payment);

            var baseUrl = _configuration["SystemConfig:FrontendBaseUrl"] ?? "https://app.dogofinance.com";
            var monnifyRequest = new InitializeTransactionRequest
            {
                Amount = amount,
                CustomerName = $"{customer.FirstName} {customer.LastName}",
                CustomerEmail = user.Email,
                PaymentReference = reference,
                PaymentDescription = "Wallet Deposit",
                RedirectUrl = $"{baseUrl}/deposit-success"
            };

            var monnifyResult = await _monnifyService.InitializeTransaction(monnifyRequest);

            if (monnifyResult != null && monnifyResult.RequestSuccessful)
            {
                response.SetMessage("Payment initialized", true, new { monnifyResult.ResponseBody.CheckoutUrl, reference });
            }
            else
            {
                payment.Status = "FAILED";
                await _uow.Payments.SavePayment(payment);
                response.SetError("Payment initiation failed at Monnify", 400);
            }

            return response;
        }

        public async Task<ApiResponse> ConfirmDeposit(string reference)
        {
            var response = new ApiResponse();
            var db = await BaseRepository().BeginTrans();

            try
            {
                var payment = await _uow.Payments.GetByReference(reference);
                if (payment == null) return new ApiResponse { Message = "Payment reference not found", Status = 404 };
                if (payment.Status == "SUCCESS") return new ApiResponse { Message = "Payment already processed", Success = true };

                payment.Status = "SUCCESS";
                await _uow.Payments.SavePayment(payment);

                var customer = await BaseRepository().FindEntity<TblCustomer>(c => c.UserId == payment.UserId);
                if (customer == null) throw new Exception("Customer not linked to user");

                var wallet = await _uow.Wallets.GetByCustomerId(customer.CustomerId);
                if (wallet == null)
                {
                    wallet = new TblWallet { CustomerId = customer.CustomerId, Balance = 0, WalletNumber = "W" + DateTime.UtcNow.Ticks.ToString().Substring(0, 9), Currency = 1, IsActive = true, CreatedAt = DateTime.UtcNow };
                    await _uow.Wallets.CreateWallet(wallet);
                }

                wallet.Balance += payment.Amount;
                await _uow.Wallets.UpdateWallet(wallet);

                var transaction = new TblTransaction
                {
                    Reference = reference,
                    TransactionType = TransactionType.DEPOSIT,
                    Amount = payment.Amount,
                    Status = 1, // SUCCESS
                    Narration = "Account Credited",
                    PaymentId = payment.Id,
                    InitiatedByUserId = payment.UserId,
                    CreatedAt = DateTime.UtcNow
                };
                await _uow.Transactions.CreateTransaction(transaction);

                // LEDGER LOGGING
                await LogLedger(transaction.TransactionId, wallet.WalletId, EntryType.CREDIT, payment.Amount, wallet.Balance, "Deposit via Monnify");

                await BaseRepository().CommitTrans();
                response.SetMessage("Deposit successful", true);
                return response;
            }
            catch (Exception ex)
            {
                await BaseRepository().RollbackTrans();
                _logger.LogError(ex, "Deposit Confirmation Error for {Ref}", reference);
                response.SetError("Internal error during confirmation", 500);
                return response;
            }
        }

        public async Task<ApiResponse> InitiateWithdrawal(WithdrawalRequest request)
        {
            var response = new ApiResponse();
            var db = await BaseRepository().BeginTrans();

            try
            {
                var customer = await BaseRepository().FindEntity<TblCustomer>(request.CustomerId);
                if (customer == null) return new ApiResponse { Message = "Customer not found", Status = 404 };

                var user = await BaseRepository().FindEntity<TblUser>(customer.UserId);
                if (user == null) return new ApiResponse { Message = "User account not found", Status = 404 };

                if (!user.IsPinSet)
                {
                    return new ApiResponse { Message = "Transaction PIN not setup. Please set it first.", Status = 400 };
                }

                if (!HashHelper.VerifyHash(request.Pin, user.TransactionPinHash!, user.TransactionPinSalt!))
                {
                    return new ApiResponse { Message = "Incorrect transaction PIN.", Status = 401 };
                }

                var wallet = await _uow.Wallets.GetByCustomerId(request.CustomerId);
                if (wallet == null || wallet.Balance < request.Amount)
                {
                    return new ApiResponse { Message = "Insufficient balance", Status = 400 };
                }

                var reference = $"WD_{DateTime.UtcNow.Ticks}";

                // 1. Debit Wallet immediately
                wallet.Balance -= request.Amount;
                await _uow.Wallets.UpdateWallet(wallet);

                // 2. Create Transaction
                var transaction = new TblTransaction
                {
                    Reference = reference,
                    TransactionType = TransactionType.WITHDRAWAL,
                    Amount = request.Amount,
                    Status = 0, // PENDING
                    Narration = request.Narration ?? "Fund Withdrawal",
                    InitiatedByUserId = customer.UserId,
                    CreatedAt = DateTime.UtcNow
                };
                await _uow.Transactions.CreateTransaction(transaction);

                // LEDGER LOGGING (Debit)
                await LogLedger(transaction.TransactionId, wallet.WalletId, EntryType.DEBIT, -request.Amount, wallet.Balance, "Withdrawal Initiation");

                // 3. Call Monnify for Disbursement
                var monnifyRequest = new SingleTransferRequest
                {
                    Amount = request.Amount,
                    Reference = reference,
                    Narration = transaction.Narration,
                    DestinationBankCode = request.BankCode,
                    DestinationAccountNumber = request.AccountNumber
                };

                var monnifyResult = await _monnifyService.SingleTransfer(monnifyRequest);

                if (monnifyResult != null && monnifyResult.RequestSuccessful)
                {
                    // If immediate success (some banks), or keep as pending
                    if (monnifyResult.ResponseBody.Status == "SUCCESS")
                    {
                        transaction.Status = 1;
                        await BaseRepository().Update(transaction);
                    }
                    else if (monnifyResult.ResponseBody.Status == "FAILED")
                    {
                        // Rollback wallet in real scenario or handle failure
                        throw new Exception("Monnify transfer returned FAILED status.");
                    }
                }
                else
                {
                    throw new Exception("Monnify API call failed.");
                }

                await BaseRepository().CommitTrans();
                response.SetMessage("Withdrawal initiated successfully", true);
                return response;
            }
            catch (Exception ex)
            {
                await BaseRepository().RollbackTrans();
                _logger.LogError(ex, "Withdrawal Initiation Error");
                response.SetError(ex.Message, 500);
                return response;
            }
        }

        public async Task<ApiResponse> GetTransactionHistory(long userId)
        {
            var history = await _uow.Transactions.GetByUserId(userId);
            return new ApiResponse { Success = true, Data = history, Message = "History retrieved" };
        }

        public async Task<ApiResponse> GetWallet(long customerId)
        {
            var wallet = await _uow.Wallets.GetByCustomerId(customerId);
            if (wallet == null) return new ApiResponse { Message = "Wallet not found", Status = 404 };
            return new ApiResponse { Success = true, Data = wallet, Message = "Wallet fetched" };
        }

        public async Task<ApiResponse> GetFinanceSummary()
        {
            var totalInflows = await _uow.Ledgers.GetTotalInflows();
            var totalOutflows = await _uow.Ledgers.GetTotalOutflows();
            
            return new ApiResponse 
            { 
                Success = true, 
                Data = new { TotalInflows = totalInflows, TotalOutflows = totalOutflows, NetLiability = totalInflows - totalOutflows },
                Message = "Finance summary retrieved" 
            };
        }
    }
}
