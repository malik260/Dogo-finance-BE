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
using System.Text.Json;

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
                amount = amount,
                customerName = $"{customer.FirstName} {customer.LastName}",
                customerEmail = user.Email,
                paymentReference = reference,
                paymentDescription = "Wallet Deposit",
                redirectUrl = $"{baseUrl}/deposit-success"
            };

            var monnifyResult = await _monnifyService.InitializeTransaction(monnifyRequest);

            if (monnifyResult != null && monnifyResult.RequestSuccessful)
            {
                string transref = monnifyResult.ResponseBody.TransactionReference;
                response.SetMessage("Payment initialized", true, new { monnifyResult.ResponseBody.CheckoutUrl, reference, transref });

                payment.ProviderReference = transref;
                await _uow.Payments.SavePayment(payment);
            }
            else
            {
                payment.Status = "FAILED";
                await _uow.Payments.SavePayment(payment);
                response.SetError("Payment initiation failed at Monnify", 400);
            }

            return response;
        }

        public async Task<ApiResponse> ChargeCard(MonnifyChargeRequest request)
        {
            try
            {
                var response = new ApiResponse();
                var monnifyRequest = new CardChargeRequest
                {
                    transactionReference = request.Reference,
                    card = new CardDetails
                    {
                        number = request.CardNumber,
                        expiryMonth = request.ExpiryMonth,
                        expiryYear = request.ExpiryYear,
                        cvv = request.CVV,
                        pin = request.Pin
                    }
                };

                var result = await _monnifyService.ChargeCard(monnifyRequest);
                if (result != null && result.RequestSuccessful)
                {
                    response.SetMessage("Charge initiated", true, result);
                }
                else
                {
                    response.SetError(result?.ResponseMessage ?? "Failed to initiate card charge", 400);
                }
                return response;

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<ApiResponse> AuthorizeDeposit(MonnifyAuthorizeRequest request)
        {
            var response = new ApiResponse();
            var authorizeReq = new AuthorizeOtpRequest
            {
                transactionReference = request.Reference,
                tokenId = request.Id,
                token = request.Otp
            };

            var success = await _monnifyService.AuthorizeOtp(authorizeReq);
            if (success)
            {
                // Note: We don't credit yet. We wait for confirmation or verify after some time.
                // Or we can verify right now if the capture is immediate.
                response.SetMessage("Authorization successful", true);
            }
            else
            {
                response.SetError("Authorization failed or OTP invalid", 400);
            }
            return response;
        }

        public async Task<ApiResponse> ConfirmDeposit(string reference)
        {
            var response = new ApiResponse();
            try
            {
                var monnifyVerify = await _monnifyService.VerifyTransaction(reference);
                if (monnifyVerify == null || monnifyVerify.ResponseBody.PaymentStatus != "PAID")
                {
                    response.SetError("Payment not verified with provider", 400);
                    return response;
                }

                await _uow.BeginTransactionAsync();
                var payment = await _uow.Payments.GetByReference(reference);
                if (payment == null) { response.SetError("Payment records not found", 404); return response; }

                if (payment.Status == "Completed" || payment.Status == "SUCCESS")
                {
                    response.SetMessage("Success", 200);
                    return response;
                }

                var customer = await _uow.Customers.GetByUserId(payment.UserId);
                if (customer == null) throw new Exception("Customer not linked to user");

                var wallet = await _uow.Wallets.GetByCustomerId(customer.CustomerId);
                if (wallet == null) { response.SetError("Wallet not found", 404); return response; }

                // Credit Wallet
                wallet.Balance += payment.Amount;
                await _uow.Wallets.UpdateWallet(wallet);

                // Update Payment
                payment.Status = "Completed";
                //payment.ModifiedAt = DateTime.UtcNow;
                await _uow.Payments.SavePayment(payment);

                // Create Transaction
                var transaction = new TblTransaction
                {
                    Reference = reference,
                    TransactionType = TransactionType.DEPOSIT,
                    Amount = payment.Amount,
                    Status = 1, // SUCCESS
                    Narration = "Wallet Deposit (Confirmed)",
                    PaymentId = payment.Id,
                    InitiatedByUserId = payment.UserId,
                    CreatedAt = DateTime.UtcNow
                };
                await _uow.Transactions.CreateTransaction(transaction);

                // Log Ledger
                await LogLedger(transaction.TransactionId, wallet.WalletId, EntryType.CREDIT, payment.Amount, wallet.Balance, "Deposit via Card");

                await _uow.CommitAsync();
                response.SetMessage("Wallet credited successfully", 200);
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                response.SetError(ex.Message, 500);
            }
            return response;
        }

        public async Task<ApiResponse> CreateVirtualAccount(long userId)
        {
            var response = new ApiResponse();
            try
            {
                var user = await _uow.Users.GetById(userId);
                if (user == null) { response.SetError("User not found", 404); return response; }
                var customer = await _uow.Customers.GetByUserId(userId);
                if (customer == null) { response.SetError("Customer profile not found. Please complete your profile.", 404); return response; }

                var existingAccounts = await _uow.ReservedAccounts.GetAccountsByUserId(userId);
                if (existingAccounts != null && existingAccounts.Any())
                {
                    _logger.LogInformation("Existing virtual accounts found for user {UserId}", userId);
                    var mappedAccounts = existingAccounts.Select(a => new
                    {
                        a.BankName,
                        a.AccountNumber,
                        a.AccountReference
                    }).ToList();
                    response.SetMessage("Existing accounts found", true, mappedAccounts);
                    return response;
                }

                _logger.LogInformation("No existing accounts. Calling Monnify for user {UserId}", userId);

                var request = new CreateReservedAccountRequest
                {
                    accountReference = $"DOGO-{userId}-{Guid.NewGuid().ToString().Substring(0, 8)}",
                    accountName = $"{customer.FirstName} {customer.LastName}",
                    customerEmail = user.Email,
                    customerName = $"{customer.FirstName} {customer.LastName}",
                    getAllAvailableBanks = true,
                    customerBvn = customer.Bvn // Monnify now requires this
                };

                if (string.IsNullOrEmpty(customer.Bvn) || !customer.Bvnverified)
                {
                    response.SetError("BVN verification is required to create a virtual account.", 400);
                    return response;
                }

                var monnifyResult = await _monnifyService.CreateReservedAccount(request);
                if (monnifyResult == null || !monnifyResult.requestSuccessful)
                {
                    response.SetError("Failed to create reserved account", 400);
                    return response;
                }

                var body = monnifyResult.responseBody;
                if (body.accounts == null || body.accounts.Count == 0)
                {
                    response.SetError("No bank accounts returned from provider", 400);
                    return response;
                }

                var savedAccounts = new List<TblReservedAccount>();
                foreach (var acc in body.accounts)
                {
                    var accountEntity = new TblReservedAccount
                    {
                        UserId = userId,
                        AccountReference = body.accountReference,
                        AccountNumber = acc.accountNumber,
                        BankName = acc.bankName,
                        BankCode = acc.bankCode,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _uow.ReservedAccounts.SaveReservedAccount(accountEntity);
                    savedAccounts.Add(accountEntity);
                }

                await _uow.SaveChangesAsync();

                var finalMapped = savedAccounts.Select(a => new
                {
                    a.BankName,
                    a.AccountNumber,
                    a.AccountReference
                }).ToList();

                response.SetMessage("Virtual accounts created successfully", true, finalMapped);
            }
            catch (Exception ex)
            {
                response.SetError(ex.Message, 500);
            }
            return response;
        }

        public async Task<ApiResponse> HandleMonnifyWebhook(string payload, string signature)
        {
            var response = new ApiResponse();
            try
            {
                if (!IsValidSignature(payload, signature))
                {
                    response.SetError("Invalid signature", 401);
                    return response;
                }

                var data = JsonSerializer.Deserialize<MonnifyWebhookPayload>(payload);
                if (data == null || data.eventType != "SUCCESSFUL_TRANSACTION")
                {
                    response.Status = 200;
                    return response;
                }

                var eventData = data.eventData;

                // Check duplicate
                var existingPayment = await _uow.Payments.GetByReference(eventData.paymentReference);
                if (existingPayment != null && (existingPayment.Status == "Completed" || existingPayment.Status == "SUCCESS"))
                {
                    response.Status = 200;
                    return response;
                }

                var reservedAcc = await _uow.ReservedAccounts.GetByAccountReference(eventData.accountReference);
                if (reservedAcc == null)
                {
                    response.SetError("Reserved account not found", 404);
                    return response;
                }

                var customer = await _uow.Customers.GetByUserId(reservedAcc.UserId);
                if (customer == null) { response.SetError("Customer records missing", 404); return response; }

                var wallet = await _uow.Wallets.GetByCustomerId(customer.CustomerId);
                if (wallet == null) { response.SetError("Wallet not found", 404); return response; }

                await _uow.BeginTransactionAsync();

                // Credit Wallet
                wallet.Balance += eventData.amountPaid;
                await _uow.Wallets.UpdateWallet(wallet);

                // Create or update payment log
                if (existingPayment == null)
                {
                    existingPayment = new TblPayment
                    {
                        UserId = customer.UserId,
                        Amount = eventData.amountPaid,
                        ProviderReference = eventData.paymentReference,
                        Status = "Completed",
                        CreatedAt = DateTime.UtcNow
                    };
                    await _uow.Payments.SavePayment(existingPayment);
                }
                else
                {
                    existingPayment.Status = "Completed";
                    await _uow.Payments.SavePayment(existingPayment);
                }

                // Create Transaction
                var transaction = new TblTransaction
                {
                    Reference = eventData.paymentReference,
                    TransactionType = TransactionType.DEPOSIT,
                    Amount = eventData.amountPaid,
                    Status = 1, // SUCCESS
                    Narration = "Transfer Deposit Received",
                    PaymentId = existingPayment.Id,
                    InitiatedByUserId = customer.UserId,
                    CreatedAt = DateTime.UtcNow
                };
                await _uow.Transactions.CreateTransaction(transaction);

                // Ledger Entry
                await LogLedger(transaction.TransactionId, wallet.WalletId, EntryType.CREDIT, eventData.amountPaid, wallet.Balance, "Bank Transfer Deposit");

                await _uow.CommitAsync();
                response.SetMessage("Webhook processed successfully", 200);
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                response.SetError(ex.Message, 500);
            }
            return response;
        }

        private bool IsValidSignature(string payload, string signature)
        {
            var secretKey = _configuration["Monnify:SecretKey"];
            if (string.IsNullOrEmpty(secretKey)) return false;

            using (var hmac = new System.Security.Cryptography.HMACSHA512(System.Text.Encoding.UTF8.GetBytes(secretKey)))
            {
                var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(payload));
                var computed = BitConverter.ToString(hash).Replace("-", "").ToLower();
                return computed == signature.ToLower();
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
            var customer = await _uow.Customers.GetByUserId(userId);
            var walletHistory = await _uow.Transactions.GetByUserId(userId);
            
            var historyList = walletHistory.Select(t => new {
                Id = t.TransactionId.ToString(),
                Reference = t.Reference,
                Type = t.TransactionType == TransactionType.DEPOSIT ? "deposit" : 
                       (t.TransactionType == TransactionType.WITHDRAWAL ? "withdrawal" : "other"),
                Amount = t.Amount,
                Status = t.Status == 1 ? "completed" : (t.Status == 0 ? "pending" : "failed"),
                Description = t.Narration,
                Date = t.CreatedAt
            }).ToList<object>();

            if (customer != null)
            {
                var investmentHistory = await _uow.Portfolios.GetInvestmentTransactionsByCustomer(customer.CustomerId);
                var mappedInvestments = investmentHistory.Select(i => new {
                    Id = $"INV_{i.Id}",
                    Reference = $"REF_{i.Id}",
                    Type = i.TransactionType == "BUY" ? "investment" : "liquidation",
                    Amount = i.Amount,
                    Status = "completed",
                    Description = i.TransactionType == "BUY" ? $"Investment in {i.Portfolio?.Name ?? "Portfolio"}" : $"Liquidation of {i.Portfolio?.Name ?? "Portfolio"}",
                    Date = i.CreatedAt
                });
                historyList.AddRange(mappedInvestments);
            }

            var sortedHistory = historyList
                .OrderByDescending(h => (DateTime)((dynamic)h).Date)
                .ToList();

            var response = new ApiResponse();
            response.SetMessage("History retrieved", true, sortedHistory);
            return response;
        }

        public async Task<ApiResponse> GetWallet(long customerId)
        {
            var response = new ApiResponse();
            var wallet = await _uow.Wallets.GetByCustomerId(customerId);
            if (wallet == null) {
                response.SetError("Wallet not found", 404);
                return response;
            }
            response.SetMessage("Wallet fetched", true, wallet);
            return response;
        }

        public async Task<ApiResponse> GetFinanceSummary()
        {
            var totalInflows = await _uow.Ledgers.GetTotalInflows();
            var totalOutflows = await _uow.Ledgers.GetTotalOutflows();

            var response = new ApiResponse();
            response.SetMessage("Finance summary retrieved", true, new { 
                TotalInflows = totalInflows, 
                TotalOutflows = totalOutflows, 
                NetLiability = totalInflows - totalOutflows 
            });
            return response;
        }
    }
}
