using DogoFinance.BusinessLogic.Layer.Helpers;
using DogoFinance.BusinessLogic.Layer.Models.Request;
using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.DataAccess.Layer.Interfaces;
using DogoFinance.DataAccess.Layer.Models.Constants;
using DogoFinance.DataAccess.Layer.Models.Entities;
using DogoFinance.DataAccess.Layer.Repositories.Base;
using DogoFinance.Integration.Interfaces;
using DogoFinance.Integration.Models.Monnify;
using DogoFinance.TransactionManagement.DTOs;
using DogoFinance.TransactionManagement.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using DogoFinance.AccountingManagement.Interfaces;
using DogoFinance.DataAccess.Layer.DTO;

namespace DogoFinance.TransactionManagement.Services
{
    public class TransactionService : DataRepository, ITransactionService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMonnifyService _monnifyService;
        private readonly IEmailService _emailService;
        private readonly ILogger<TransactionService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IAccountingService _accountingService;
        private readonly IFxRateService _fxRateService;

        public TransactionService(IUnitOfWork uow, IMonnifyService monnifyService, IEmailService emailService, ILogger<TransactionService> logger, IConfiguration configuration, IAccountingService accountingService, IFxRateService fxRateService)
        {
            _uow = uow;
            _monnifyService = monnifyService;
            _emailService = emailService;
            _logger = logger;
            _configuration = configuration;
            _accountingService = accountingService;
            _fxRateService = fxRateService;
            
            // Link the base repository to the Unit of Work's shared context to avoid deadlocks
            SetSharedRepository(_uow.GenericRepository);
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
            try
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
                    PaymentReference = reference,
                    Status = "PENDING",
                    CreatedAt = DateTime.UtcNow
                };
                await _uow.Payments.SavePayment(payment);

                var baseUrl = (_configuration["SystemConfig:FrontendBaseUrl"] ?? "https://app.dogofinance.com").Trim().TrimEnd('/');
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
            catch (Exception ex)
            {

                throw;
            }        }

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

                // Call the consolidated processing logic
                return await ProcessDepositCredit(
                    reference: reference,
                    amount: monnifyVerify.ResponseBody.AmountPaid,
                    method: monnifyVerify.ResponseBody.PaymentMethod,
                    provider: "Monnify"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ConfirmDeposit Error for {Ref}", reference);
                response.SetError(ex.Message, 500);
                return response;
            }
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
                if (data == null)
                {
                    response.Status = 200;
                    return response;
                }

                if (data.eventType == "SUCCESSFUL_TRANSACTION")
                {
                    return await HandleDepositWebhook(data.eventData);
                }
                else if (data.eventType == "SUCCESSFUL_DISBURSEMENT")
                {
                    return await HandleDisbursementWebhook(data.eventData);
                }

                response.Status = 200;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Webhook processing error");
                response.SetError("Processing error", 200); // Always return 200 to Monnify to stop retries if it's a code error
                return response;
            }
        }
        private async Task<ApiResponse> HandleDepositWebhook(WebhookEventData eventData)
        {
            try
            {
                // 1. Only process successful payments
                if (eventData.paymentStatus != "PAID")
                    return new ApiResponse { Status = 200, Message = "Ignored: Status not PAID" };

                // 2. Call the consolidated logic
                // We use paymentReference (Monnify Trans Ref) as the primary key for credit
                return await ProcessDepositCredit(
                    reference: eventData.paymentReference,
                    amount: eventData.amountPaid,
                    method: eventData.paymentMethod,
                    provider: "Monnify (Webhook)",
                    customerEmail: eventData.customer?.email,
                    accountReference: eventData.accountReference
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Webhook Deposit Error");
                return new ApiResponse { Status = 200, Message = "Error logged" }; 
            }
        }

        /// <summary>
        /// Consolidated logic for crediting a customer's wallet after a successful deposit.
        /// Handles idempotency (no double-credit) and atomic updates across all financial tables.
        /// </summary>
        private async Task<ApiResponse> ProcessDepositCredit(string reference, decimal amount, string method, string provider, string? customerEmail = null, string? accountReference = null)
        {
            var response = new ApiResponse();
            
            // We use a global lock or rely on the DB transaction for race conditions.
            // Since this is likely a single-node web app, we can use a simple DB transaction + status check.
            await _uow.BeginTransactionAsync();

            try
            {
                // 1. Idempotency Check: Check if this payment is already processed
                // We check by ProviderReference (Monnify's Ref)
                var payment = await _uow.Payments.GetByPaymentRef(reference);
                
                // Fallback: If not found by PaymentRef, check by ProviderReference
                if (payment == null) payment = await _uow.Payments.GetByReference(reference);

                if (payment != null && (payment.Status == "Completed" || payment.Status == "SUCCESS"))
                {
                    await _uow.RollbackAsync(); // Release transaction
                    return new ApiResponse { Status = 200, Message = "Already processed" };
                }

                // 2. Find the Customer
                TblCustomer? customer = null;
                if (payment != null)
                {
                    customer = await _uow.Customers.GetByUserId(payment.UserId);
                }
                else
                {
                    // If no payment record exists (e.g., Transfer to Reserved Account), find customer by other means
                    if (!string.IsNullOrEmpty(accountReference))
                    {
                        var reservedAcc = await _uow.ReservedAccounts.GetByAccountReference(accountReference);
                        if (reservedAcc != null) customer = await _uow.Customers.GetByUserId(reservedAcc.UserId);
                    }
                    
                    if (customer == null && !string.IsNullOrEmpty(customerEmail))
                    {
                        var user = await _uow.Users.GetByEmail(customerEmail);
                        if (user != null) customer = await _uow.Customers.GetByUserId(user.UserId);
                    }
                }

                if (customer == null)
                {
                    await _uow.RollbackAsync();
                    _logger.LogWarning("Deposit Credit Failed: Customer not found for Reference {Ref}", reference);
                    return new ApiResponse { Status = 200, Message = "Customer not found" };
                }

                // 3. Find/Create Wallet
                var wallet = await _uow.Wallets.GetByCustomerId(customer.CustomerId);
                if (wallet == null)
                {
                    await _uow.RollbackAsync();
                    return new ApiResponse { Status = 404, Message = "Wallet not found" };
                }

                // 4. Update/Create Payment Record
                if (payment == null)
                {
                    payment = new TblPayment
                    {
                        UserId = customer.UserId,
                        Amount = amount,
                        PaymentProvider = 1, // Monnify
                        ProviderReference = reference,
                        PaymentReference = reference,
                        Status = "Completed",
                        CreatedAt = DateTime.UtcNow
                    };
                    await _uow.Payments.SavePayment(payment);
                }
                else
                {
                    payment.Status = "Completed";
                    payment.ProviderReference = reference;
                    await _uow.Payments.SavePayment(payment);
                }

                // 5. Update Wallet Balance
                wallet.Balance += amount;
                await _uow.Wallets.UpdateWallet(wallet);

                // 6. Create Transaction Log
                var transaction = new TblTransaction
                {
                    Reference = reference,
                    TransactionType = TransactionType.DEPOSIT,
                    Amount = amount,
                    Status = 1, // SUCCESS
                    Narration = $"Deposit via {method} ({provider})",
                    PaymentId = payment.Id,
                    InitiatedByUserId = customer.UserId,
                    CreatedAt = DateTime.UtcNow
                };
                await _uow.Transactions.CreateTransaction(transaction);

                // 7. Log Ledger (Sub-ledger for Wallet History)
                await LogLedger(
                    transaction.TransactionId,
                    wallet.WalletId,
                    EntryType.CREDIT,
                    amount,
                    wallet.Balance,
                    $"Deposit ({method})"
                );

                // 8. Post to General Ledger (Double-Entry Bookkeeping)
                await _accountingService.PostJournalAsync(new JournalEntryDto
                {
                    Reference = reference,
                    Narration = $"Wallet Deposit - Customer ID: {customer.CustomerId}",
                    TransactionDate = DateTime.UtcNow,
                    Lines = new List<JournalLineDto>
                    {
                        new JournalLineDto { AccountCode = "1110", Debit = amount, Credit = 0, Narration = "Bank Inflow" }, // Dr Bank
                        new JournalLineDto { AccountCode = "2110", Debit = 0, Credit = amount, Narration = "Customer Wallet Liability" } // Cr Wallet
                    }
                });

                await _uow.CommitAsync();
                return new ApiResponse { Success = true, Message = "Wallet credited successfully", Status = 200 };
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                _logger.LogError(ex, "ProcessDepositCredit CRITICAL ERROR for {Ref}", reference);
                throw; // Rethrow to let caller handle
            }
        }
        private async Task<ApiResponse> HandleDisbursementWebhook(WebhookEventData eventData)
        {
            var response = new ApiResponse();
            var transaction = await BaseRepository().FindEntity<TblTransaction>(t => t.Reference == eventData.reference);
            
            if (transaction == null)
            {
                _logger.LogWarning("Disbursement webhook received for unknown reference: {Ref}", eventData.reference);
                return new ApiResponse { Status = 200 }; // Still return 200
            }

            if (transaction.Status == 1) // Already successful
            {
                return new ApiResponse { Status = 200 };
            }

            var user = await BaseRepository().FindEntity<TblUser>(u => u.UserId == transaction.InitiatedByUserId);
            if (user == null) return new ApiResponse { Status = 200 };

            var customer = await _uow.Customers.GetByUserId(user.UserId);
            if (customer == null) return new ApiResponse { Status = 200 };

            var wallet = await _uow.Wallets.GetByCustomerId(customer.CustomerId);
            if (wallet == null) return new ApiResponse { Status = 200 };

            await _uow.BeginTransactionAsync();

            try
            {
                // 1. Debit Wallet
                wallet.Balance -= eventData.amount;
                await _uow.Wallets.UpdateWallet(wallet);

                // 2. Update Transaction
                transaction.Status = 1; // SUCCESS
                transaction.Narration += " (Confirmed)";
                await BaseRepository().Update(transaction);

                // 3. Ledger Logging
                await LogLedger(transaction.TransactionId, wallet.WalletId, EntryType.DEBIT, -eventData.amount, wallet.Balance, "Withdrawal (Automated Success)");

                // 4. Post to General Ledger (Double-Entry)
                await _accountingService.PostJournalAsync(new JournalEntryDto
                {
                    Reference = eventData.reference,
                    Narration = $"Withdrawal - Customer ID: {customer.CustomerId}",
                    TransactionDate = DateTime.UtcNow,
                    Lines = new List<JournalLineDto>
                    {
                        new JournalLineDto { AccountCode = "2110", Debit = eventData.amount, Credit = 0, Narration = "Wallet Liability Reduced" }, // Dr Wallet
                        new JournalLineDto { AccountCode = "1110", Debit = 0, Credit = eventData.amount, Narration = "Bank Outflow" } // Cr Bank
                    }
                });

                await _uow.CommitAsync();
                response.SetMessage("Disbursement processed", 200);
            }
            catch (Exception)
            {
                await _uow.RollbackAsync();
                throw;
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
            await _uow.BeginTransactionAsync();

            try
            {
                var customer = await _uow.GenericRepository.FindEntity<TblCustomer>(request.CustomerId);
                if (customer == null) {
                    await _uow.RollbackAsync();
                    return new ApiResponse { Message = "Customer not found", Status = 404 };
                }

                var user = await _uow.GenericRepository.FindEntity<TblUser>(customer.UserId);
                if (user == null) {
                    await _uow.RollbackAsync();
                    return new ApiResponse { Message = "User account not found", Status = 404 };
                }

                if (!user.IsPinSet)
                {
                    await _uow.RollbackAsync();
                    return new ApiResponse { Message = "Transaction PIN not setup. Please set it first.", Status = 400 };
                }

                if (!HashHelper.VerifyHash(request.Pin, user.TransactionPinHash!, user.TransactionPinSalt!))
                {
                    await _uow.RollbackAsync();
                    return new ApiResponse { Message = "Incorrect transaction PIN.", Status = 401 };
                }

                // 2FA Verification
                if (user.Is2faEnabled == true)
                {
                    if (string.IsNullOrEmpty(request.Otp))
                    {
                        await _uow.RollbackAsync();
                        return new ApiResponse { Message = "2FA is enabled. Please provide the OTP sent to your email.", Boolean = false, Status = 403 };
                    }

                    if (user.VerificationCode != request.Otp || user.VerificationExpiry < DateTime.UtcNow)
                    {
                        await _uow.RollbackAsync();
                        return new ApiResponse { Message = "Invalid or expired OTP code.", Boolean = false, Status = 403 };
                    }
                    
                    // Clear OTP after successful use
                    user.VerificationCode = null;
                    user.VerificationExpiry = null;
                    await _uow.GenericRepository.Update(user);
                }

                var wallet = await _uow.Wallets.GetByCustomerId(request.CustomerId);
                if (wallet == null || wallet.Balance < request.Amount)
                {
                    await _uow.RollbackAsync();
                    return new ApiResponse { Message = "Insufficient balance", Status = 400 };
                }

                // Check Threshold
                var settings = await _uow.GenericRepository.AsQueryable<TblSystemSetting>(s => true).FirstOrDefaultAsync();
                decimal threshold = settings?.WithdrawalAutoThreshold ?? 50000;
                bool needsApproval = request.Amount > threshold;

                bool isCorporate = customer.CustomerTypeId == 2;

                var reference = $"WD_{DateTime.UtcNow.Ticks}";

                if (isCorporate && !string.IsNullOrEmpty(customer.SignatoryMandate))
                {
                    // Fetch active signatories linked to user accounts
                    var signatories = await _uow.GenericRepository.AsQueryable<TblCorporateSignatory>(s => s.CustomerId == customer.CustomerId && !s.IsDeleted && s.UserId != null).ToListAsync();
                    if (!signatories.Any())
                    {
                        await _uow.RollbackAsync();
                        return new ApiResponse { Message = "No registered signatories found to approve this transaction.", Status = 400 };
                    }

                    // Create Admin Review Record for the Maker
                    var withdrawalReq = new TblWithdrawalRequest
                    {
                        CustomerId = customer.CustomerId,
                        Amount = request.Amount,
                        Status = "Pending Signatories",
                        Reference = reference,
                        Narration = (request.Narration ?? "Corporate Withdrawal") + " (Pending Signatures)",
                        BankCode = request.BankCode,
                        AccountNumber = request.AccountNumber,
                        InitiatedAt = DateTime.UtcNow
                    };
                    await _uow.GenericRepository.Insert(withdrawalReq);

                    // Create Transaction record for user visibility
                    var pendingTransaction = new TblTransaction
                    {
                        Reference = reference,
                        TransactionType = TransactionType.WITHDRAWAL,
                        Amount = request.Amount,
                        Status = 0, // PENDING
                        Narration = withdrawalReq.Narration,
                        InitiatedByUserId = customer.UserId,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _uow.Transactions.CreateTransaction(pendingTransaction);

                    // Create Approval Records
                    // Logic based on mandate (Sole, Either to sign, Both to sign, Any 2 to sign)
                    foreach (var sig in signatories)
                    {
                        var approval = new TblTransactionApproval
                        {
                            TransactionId = pendingTransaction.TransactionId,
                            ApproverUserId = sig.UserId.Value,
                            Status = "Pending",
                            CreatedAt = DateTime.UtcNow
                        };
                        await _uow.GenericRepository.Insert(approval);

                        // System Notification
                        var notification = new TblNotification
                        {
                            CustomerId = customer.CustomerId, // or maybe sig.CustomerId if they are linked
                            Title = "Action Required: Pending Withdrawal",
                            Message = $"A withdrawal of {request.Amount:N2} has been initiated for {customer.BusinessName ?? customer.FirstName} and requires your approval.",
                            IsRead = false,
                            CreatedAt = DateTime.UtcNow
                        };
                        await _uow.GenericRepository.Insert(notification);

                        // Email Notification
                        string subject = "Action Required: Pending Corporate Withdrawal";
                        string body = $@"
                        <p>Dear {sig.FirstName} {sig.Surname},</p>
                        <p>A withdrawal of <strong>{request.Amount:N2}</strong> has been initiated on the account of <strong>{customer.BusinessName ?? customer.FirstName}</strong>.</p>
                        <p>As an authorized signatory, your approval is required to process this transaction.</p>
                        <p>Please log in to the Corporate Portal to review and approve or reject this request.</p>
                        <br/>
                        <p>Regards,<br/>Dogo Finance Team</p>";
                        
                        // Fire and forget email or await it
                        _ = _emailService.SendEmail(sig.BusinessEmail, subject, body);
                    }

                    await _uow.CommitAsync();
                    return new ApiResponse { Success = true, Message = "Withdrawal initiated. Awaiting signatory approvals.", Status = 200 };
                }
                else if (needsApproval)
                {
                    // Create Admin Review Record
                    var withdrawalReq = new TblWithdrawalRequest
                    {
                        CustomerId = customer.CustomerId,
                        Amount = request.Amount,
                        Status = "Pending",
                        Reference = reference,
                        Narration = (request.Narration ?? "Fund Withdrawal") + " (Pending Approval)",
                        BankCode = request.BankCode,
                        AccountNumber = request.AccountNumber,
                        InitiatedAt = DateTime.UtcNow
                    };
                    await _uow.GenericRepository.Insert(withdrawalReq);

                    // Create Transaction record for user visibility (but no debit yet)
                    var pendingTransaction = new TblTransaction
                    {
                        Reference = reference,
                        TransactionType = TransactionType.WITHDRAWAL,
                        Amount = request.Amount,
                        Status = 0, // PENDING
                        Narration = (request.Narration ?? "Fund Withdrawal") + " (Pending Approval)",
                        InitiatedByUserId = customer.UserId,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _uow.Transactions.CreateTransaction(pendingTransaction);

                    await _uow.CommitAsync();
                    return new ApiResponse { Success = true, Message = "Withdrawal request submitted for administrative review. Funds will be deducted upon approval.", Status = 200 };
                }

                // --- ONLY FOR AUTOMATED WITHDRAWALS (BELOW THRESHOLD) ---

                // 1. Create Transaction (NO DEBIT during initiation per user request)
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

                // 2. Call Monnify for Disbursement (Automated)
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
                    // We keep transaction as PENDING. 
                    // Wallet impact and transaction success will be handled via webhook or status check.
                    if (monnifyResult.ResponseBody.Status == "FAILED")
                    {
                        throw new Exception("Monnify transfer returned FAILED status.");
                    }
                }
                else
                {
                    throw new Exception("Monnify API call failed.");
                }

                await _uow.CommitAsync();
                response.SetMessage("Withdrawal initiated successfully", true);
                return response;
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                _logger.LogError(ex, "Withdrawal Initiation Error");
                response.SetError(ex.Message, 500);
                return response;
            }
        }

        public async Task<ApiResponse> SendWithdrawalOtp(long customerId, decimal amount)
        {
            var response = new ApiResponse();
            var customer = await BaseRepository().FindEntity<TblCustomer>(customerId);
            if (customer == null) return new ApiResponse { Message = "Customer not found", Status = 404 };

            var user = await BaseRepository().FindEntity<TblUser>(customer.UserId);
            if (user == null) return new ApiResponse { Message = "User not found", Status = 404 };

            // 2. 2FA Check
            if (user.Is2faEnabled != true)
            {
                return new ApiResponse { Message = "2FA is not enabled for this user", Status = 400 };
            }

            var otp = new Random().Next(100000, 999999).ToString();
            user.VerificationCode = otp;
            user.VerificationExpiry = DateTime.UtcNow.AddMinutes(10);
            await BaseRepository().Update(user);

            var placeholders = new Dictionary<string, string>
            {
                { "FirstName", customer.FirstName ?? "Dogo User" },
                { "Amount", amount.ToString("N2") },
                { "Code", otp },
                { "Expiry", "10 minutes" }
            };

            await _emailService.SendTemplateEmail(user.Email, "Authorize Withdrawal - DogoFinance", "WithdrawalOtp", placeholders);

            response.SetMessage("OTP sent successfully to your email.", true);
            return response;
        }

        public async Task<ApiResponse> ValidateWithdrawalOtp(long customerId, string otp)
        {
            var response = new ApiResponse();
            var customer = await BaseRepository().FindEntity<TblCustomer>(customerId);
            if (customer == null) return new ApiResponse { Message = "Customer not found", Status = 404 };

            var user = await BaseRepository().FindEntity<TblUser>(customer.UserId);
            if (user == null) return new ApiResponse { Message = "User not found", Status = 404 };

            if (user.VerificationCode != otp)
            {
                return new ApiResponse { Message = "The OTP code you entered is incorrect.", Success = false, Status = 400 };
            }

            if (user.VerificationExpiry < DateTime.UtcNow)
            {
                return new ApiResponse { Message = "This OTP code has expired. Please request a new one.", Success = false, Status = 400 };
            }

            return new ApiResponse { Message = "OTP verified successfuly", Success = true, Status = 200 };
        }

        public async Task<ApiResponse> GetPendingApprovals(long userId)
        {
            var approvals = await _uow.GenericRepository.AsQueryable<TblTransactionApproval>(a => a.ApproverUserId == userId && a.Status == "Pending")
                .Include(a => a.Transaction)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            var data = approvals.Select(a => new
            {
                a.Id,
                a.TransactionId,
                a.Status,
                a.CreatedAt,
                Transaction = new
                {
                    a.Transaction.Amount,
                    a.Transaction.Reference,
                    a.Transaction.Narration,
                    a.Transaction.CreatedAt
                }
            });

            return new ApiResponse { Success = true, Data = data, Status = 200 };
        }

        public async Task<ApiResponse> ProcessTransactionApproval(long userId, long transactionId, bool isApproved, string pin)
        {
            await _uow.BeginTransactionAsync();
            try
            {
                var user = await _uow.GenericRepository.FindEntity<TblUser>(userId);
                if (user == null) return new ApiResponse { Message = "User not found", Status = 404 };

                if (!HashHelper.VerifyHash(pin, user.TransactionPinHash!, user.TransactionPinSalt!))
                {
                    await _uow.RollbackAsync();
                    return new ApiResponse { Message = "Incorrect transaction PIN.", Status = 401 };
                }

                var approval = await _uow.GenericRepository.FindEntity<TblTransactionApproval>(a => a.ApproverUserId == userId && a.TransactionId == transactionId && a.Status == "Pending");
                if (approval == null)
                {
                    await _uow.RollbackAsync();
                    return new ApiResponse { Message = "Pending approval request not found or already processed.", Status = 404 };
                }

                var transaction = await _uow.GenericRepository.FindEntity<TblTransaction>(transactionId);
                if (transaction == null || transaction.Status != 0) // 0 is Pending
                {
                    await _uow.RollbackAsync();
                    return new ApiResponse { Message = "Transaction is not in a pending state.", Status = 400 };
                }

                // Get the initiator user to figure out the customer mandate
                var initiator = await _uow.GenericRepository.FindEntity<TblUser>(transaction.InitiatedByUserId);
                var customer = await _uow.GenericRepository.FindEntity<TblCustomer>(c => c.UserId == initiator.UserId);

                if (customer == null)
                {
                    await _uow.RollbackAsync();
                    return new ApiResponse { Message = "Originating customer not found.", Status = 404 };
                }

                if (!isApproved)
                {
                    // Rejection logic: abort the entire transaction
                    approval.Status = "Rejected";
                    approval.ActedAt = DateTime.UtcNow;
                    await _uow.GenericRepository.Update(approval);

                    transaction.Status = 2; // FAILED/REJECTED
                    await _uow.GenericRepository.Update(transaction);

                    // Update withdrawal request to rejected
                    var withdrawalReq = await _uow.GenericRepository.FindEntity<TblWithdrawalRequest>(w => w.Reference == transaction.Reference);
                    if (withdrawalReq != null)
                    {
                        withdrawalReq.Status = "Rejected";
                        await _uow.GenericRepository.Update(withdrawalReq);
                    }

                    // Cancel other pending approvals
                    var otherApprovals = await _uow.GenericRepository.AsQueryable<TblTransactionApproval>(a => a.TransactionId == transactionId && a.Status == "Pending").ToListAsync();
                    foreach (var other in otherApprovals)
                    {
                        other.Status = "Cancelled";
                        await _uow.GenericRepository.Update(other);
                    }

                    await _uow.CommitAsync();

                    return new ApiResponse { Success = true, Message = "Transaction rejected successfully.", Status = 200 };
                }

                // Approval logic
                approval.Status = "Approved";
                approval.ActedAt = DateTime.UtcNow;
                await _uow.GenericRepository.Update(approval);

                // Check if mandate threshold is met
                var allApprovals = await _uow.GenericRepository.AsQueryable<TblTransactionApproval>(a => a.TransactionId == transactionId).ToListAsync();
                int approvedCount = allApprovals.Count(a => a.Status == "Approved");

                int requiredSignatures = 1; // Default
                string mandate = customer.SignatoryMandate ?? "Sole";
                if (mandate == "Both to sign" || mandate == "Any 2 to sign")
                {
                    requiredSignatures = 2;
                }

                if (approvedCount >= requiredSignatures)
                {
                    // Threshold met. Proceed to execute transaction.
                    var wallet = await _uow.Wallets.GetByCustomerId(customer.CustomerId);
                    if (wallet == null || wallet.Balance < transaction.Amount)
                    {
                        await _uow.RollbackAsync();
                        return new ApiResponse { Message = "Insufficient balance in customer account.", Status = 400 };
                    }

                    wallet.Balance -= transaction.Amount;
                    await _uow.Wallets.UpdateWallet(wallet);

                    transaction.Status = 1; // SUCCESS
                    await _uow.GenericRepository.Update(transaction);

                    var withdrawalReq = await _uow.GenericRepository.FindEntity<TblWithdrawalRequest>(w => w.Reference == transaction.Reference);
                    if (withdrawalReq != null)
                    {
                        withdrawalReq.Status = "Approved";
                        await _uow.GenericRepository.Update(withdrawalReq);
                    }

                    await LogLedger(transaction.TransactionId, wallet.WalletId, 2, transaction.Amount, wallet.Balance, transaction.Narration);

                    // Update any remaining pending approvals to "Cancelled"
                    foreach (var other in allApprovals.Where(a => a.Status == "Pending"))
                    {
                        other.Status = "Cancelled";
                        await _uow.GenericRepository.Update(other);
                    }

                    await _uow.CommitAsync();

                    // Notify signatories of completion
                    var signatories = await _uow.GenericRepository.AsQueryable<TblCorporateSignatory>(s => s.CustomerId == customer.CustomerId && !s.IsDeleted).ToListAsync();
                    foreach (var sig in signatories)
                    {
                        string subject = "Transaction Completed: Corporate Withdrawal";
                        string body = $@"
                        <p>Dear {sig.FirstName} {sig.Surname},</p>
                        <p>The withdrawal of <strong>{transaction.Amount:N2}</strong> on the account of <strong>{customer.BusinessName ?? customer.FirstName}</strong> has been fully approved and successfully processed.</p>
                        <br/>
                        <p>Regards,<br/>Dogo Finance Team</p>";
                        _ = _emailService.SendEmail(sig.BusinessEmail, subject, body);
                    }

                    return new ApiResponse { Success = true, Message = "Transaction fully approved and processed successfully.", Status = 200 };
                }
                else
                {
                    await _uow.CommitAsync();
                    return new ApiResponse { Success = true, Message = "Approval recorded. Awaiting further signatures.", Status = 200 };
                }
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                _logger.LogError(ex, "Error processing transaction approval");
                return new ApiResponse { Message = "An error occurred while processing approval", Status = 500 };
            }
        }

        public async Task<ApiResponse> GetTransactionHistory(long userId)
        {
            var customer = await _uow.Customers.GetByUserId(userId);
            
            if (customer == null)
            {
                var signatory = await _uow.GenericRepository.FindEntity<TblCorporateSignatory>(s => s.UserId == userId && !s.IsDeleted);
                if (signatory != null)
                {
                    customer = await _uow.GenericRepository.FindEntity<TblCustomer>(signatory.CustomerId);
                    if (customer != null)
                    {
                        userId = customer.UserId;
                    }
                }
            }

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
            var wallets = await _uow.GenericRepository.AsQueryable<TblWallet>(w => w.CustomerId == customerId).ToListAsync();
            
            var ngnWallet = wallets.FirstOrDefault(w => w.Currency == 1);
            var usdWallet = wallets.FirstOrDefault(w => w.Currency == 2);

            var walletData = new
            {
                Balance = ngnWallet?.Balance ?? 0, // NGN balance
                NgnBalance = ngnWallet?.Balance ?? 0,
                UsdBalance = usdWallet?.Balance ?? 0, // USD balance
                DollarBalance = usdWallet?.Balance ?? 0,
                NgnWalletNumber = ngnWallet?.WalletNumber,
                UsdWalletNumber = usdWallet?.WalletNumber,
                Wallets = wallets
            };

            response.SetMessage("Wallet fetched", true, walletData);
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
        public async Task<ApiResponse> SubmitManualFundingRequest(long userId, ManualFundingRequestDto request)
        {
            var response = new ApiResponse();
            try
            {
                var customer = await _uow.Customers.GetByUserId(userId);
                if (customer == null) return new ApiResponse { Message = "Customer not found", Status = 404 };

                var manualRequest = new TblManualFundingRequest
                {
                    CustomerId = customer.CustomerId,
                    Amount = request.Amount,
                    Reference = request.Reference,
                    ReceiptPath = request.ReceiptPath,
                    Status = "Pending",
                    InitiatedAt = DateTime.UtcNow
                };

                await BaseRepository().Insert(manualRequest);
                await _uow.SaveChangesAsync();

                response.SetMessage("Manual funding request submitted successfully", true);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting manual funding request");
                return new ApiResponse { Message = "An error occurred while submitting your request", Status = 500 };
            }
        }

        // ─── DOLLAR WALLET ────────────────────────────────────────────────────────

        /// <summary>
        /// Returns a live FX rate quote for converting a given NGN amount to USD.
        /// </summary>
        public async Task<ApiResponse> GetFxRateQuoteAsync(decimal ngnAmount)
        {
            try
            {
                var rateResult = await _fxRateService.GetNgnToUsdRateAsync();
                var usdAmount = Math.Round(ngnAmount / rateResult.EffectiveRateWithMargin, 2);

                var quote = new FxRateQuoteResponse
                {
                    NgnAmount = ngnAmount,
                    BaseNgnPerUsdRate = rateResult.NgnPerUsdRate,
                    EffectiveRateWithMargin = rateResult.EffectiveRateWithMargin,
                    EstimatedUsdAmount = usdAmount,
                    Provider = rateResult.Provider,
                    IsFallbackRate = rateResult.IsFallback,
                    Timestamp = rateResult.FetchedAt
                };

                return new ApiResponse { Success = true, Message = "FX rate quote retrieved", Data = quote, Status = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching FX rate quote");
                return new ApiResponse { Message = "Unable to retrieve exchange rate at this time.", Status = 503 };
            }
        }

        /// <summary>
        /// Converts NGN from the customer's NGN wallet into their USD wallet at the live FX rate.
        /// Double-entry: Dr NGN Wallet Liability + Dr USD Dom. Bank | Cr NGN Dom. Bank + Cr USD Wallet Liability.
        /// </summary>
        public async Task<ApiResponse> FundDollarWalletFromNairaAsync(long userID, FundDollarWalletFromNairaRequest request)
        {
            await _uow.BeginTransactionAsync();
            try
            {

                var user = await _uow.GenericRepository.FindEntity<TblUser>(userID);
                if (user == null) { await _uow.RollbackAsync(); return new ApiResponse { Message = "User not found", Status = 404 }; }


                // 1. Load customer & user
                var customer = await _uow.Customers.GetByUserId(user.UserId);
                if (customer == null) { await _uow.RollbackAsync(); return new ApiResponse { Message = "Customer not found", Status = 404 }; }

                
                // 3. Load NGN wallet & check balance
                var ngnWallet = await _uow.Wallets.GetByCustomerId(customer.CustomerId); // Currency = 1 (NGN)
                if (ngnWallet == null || ngnWallet.Currency != 1)
                {
                    await _uow.RollbackAsync();
                    return new ApiResponse { Message = "NGN wallet not found.", Status = 404 };
                }

                if (ngnWallet.Balance < request.NairaAmount)
                {
                    await _uow.RollbackAsync();
                    return new ApiResponse { Message = "Insufficient NGN balance.", Status = 400 };
                }

                // 4. Load USD wallet (auto-create if missing for existing customers)
                var usdWallet = await _uow.GenericRepository.FindEntity<TblWallet>(w => w.CustomerId == customer.CustomerId && w.Currency == 2);
                if (usdWallet == null)
                {
                    usdWallet = new TblWallet
                    {
                        CustomerId = customer.CustomerId,
                        WalletNumber = new Random().NextInt64(1000000000, 9999999999).ToString(),
                        Currency = 2, // USD
                        Balance = 0,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _uow.Wallets.CreateWallet(usdWallet);
                }

                // 5. Get live FX rate
                var rateResult = await _fxRateService.GetNgnToUsdRateAsync();
                var usdCredited = Math.Round(request.NairaAmount / rateResult.EffectiveRateWithMargin, 2);

                var reference = $"FXCONV_{DateTime.UtcNow.Ticks}";

                // 6. Debit NGN wallet
                ngnWallet.Balance -= request.NairaAmount;
                await _uow.Wallets.UpdateWallet(ngnWallet);

                // 7. Credit USD wallet
                usdWallet.Balance += usdCredited;
                await _uow.GenericRepository.Update(usdWallet);

                // 8. Create transaction log
                var transaction = new TblTransaction
                {
                    Reference = reference,
                    TransactionType = TransactionType.DEPOSIT, // FX conversion
                    Amount = request.NairaAmount,
                    Status = 1,
                    Narration = $"FX Conversion: NGN {request.NairaAmount:N2} → USD {usdCredited:N2} @ {rateResult.EffectiveRateWithMargin:N4}",
                    InitiatedByUserId = customer.UserId,
                    CreatedAt = DateTime.UtcNow
                };
                await _uow.Transactions.CreateTransaction(transaction);

                // 9. Sub-ledger entries
                await LogLedger(transaction.TransactionId, ngnWallet.WalletId, EntryType.DEBIT, request.NairaAmount, ngnWallet.Balance,
                    $"FX Conversion Debit (NGN)");
                await LogLedger(transaction.TransactionId, usdWallet.WalletId, EntryType.CREDIT, usdCredited, usdWallet.Balance,
                    $"FX Conversion Credit (USD)");

                // 10. Double-entry GL postings
                //  Dr 2110 (Customer NGN Wallet Liability reduces) — reduce our liability in NGN
                //  Cr 1110 (NGN Dom. Bank reduces)                  — NGN leaves our NGN bank
                //  Dr 1120 (USD Dom. Bank increases)                — USD enters our USD bank
                //  Cr 2120 (Customer USD Wallet Liability increases) — we owe customer in USD
                await _accountingService.PostJournalAsync(new JournalEntryDto
                {
                    Reference = reference,
                    Narration = $"FX Conversion NGN→USD - Customer {userID}",
                    TransactionDate = DateTime.UtcNow,
                    Lines = new List<JournalLineDto>
                    {
                        new JournalLineDto { AccountCode = "2110", Debit = request.NairaAmount, Credit = 0,             Narration = "NGN Wallet Liability Reduced" },
                        new JournalLineDto { AccountCode = "1110", Debit = 0,                   Credit = request.NairaAmount, Narration = "NGN Bank Outflow" },
                        new JournalLineDto { AccountCode = "1120", Debit = usdCredited,          Credit = 0,             Narration = "USD Dom. Bank Inflow" },
                        new JournalLineDto { AccountCode = "2120", Debit = 0,                   Credit = usdCredited,   Narration = "USD Wallet Liability Created" }
                    }
                });

                await _uow.CommitAsync();

                return new ApiResponse
                {
                    Success = true,
                    Message = $"USD wallet funded successfully. NGN {request.NairaAmount:N2} converted to USD {usdCredited:N2} at rate {rateResult.EffectiveRateWithMargin:N4}.",
                    Data = new { UsdCredited = usdCredited, NgnDebited = request.NairaAmount, Rate = rateResult.EffectiveRateWithMargin, Reference = reference },
                    Status = 200
                };
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                _logger.LogError(ex, "FundDollarWalletFromNaira Error for Customer {CustomerId}", userID);
                return new ApiResponse { Message = "An error occurred while processing the FX conversion.", Status = 500 };
            }
        }

        /// <summary>
        /// Records an admin-reviewed wire transfer funding request for a customer's USD wallet.
        /// Admin will manually verify bank inflow and approve to credit the USD wallet.
        /// </summary>
        public async Task<ApiResponse> InitiateDollarWireFundingAsync(long customerId, FundDollarWalletViaWireRequest request)
        {
            try
            {
                var customer = await _uow.GenericRepository.FindEntity<TblCustomer>(customerId);
                if (customer == null) return new ApiResponse { Message = "Customer not found", Status = 404 };

                var usdWallet = await _uow.GenericRepository.FindEntity<TblWallet>(w => w.CustomerId == customerId && w.Currency == 2);
                if (usdWallet == null)
                {
                    usdWallet = new TblWallet
                    {
                        CustomerId = customerId,
                        WalletNumber = new Random().NextInt64(1000000000, 9999999999).ToString(),
                        Currency = 2, // USD
                        Balance = 0,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _uow.Wallets.CreateWallet(usdWallet);
                }

                var reference = $"WIRE_{DateTime.UtcNow.Ticks}";

                // Log a pending manual funding record for admin review
                var manualRequest = new TblManualFundingRequest
                {
                    CustomerId = customerId,
                    Amount = request.UsdAmount,
                    Reference = request.BankReference,
                    ReceiptPath = request.ProofDocumentUrl,
                    Status = "Pending",
                    InitiatedAt = DateTime.UtcNow
                };
                await _uow.GenericRepository.Insert(manualRequest);

                // Also log a pending transaction for customer visibility
                var transaction = new TblTransaction
                {
                    Reference = reference,
                    TransactionType = TransactionType.DEPOSIT,
                    Amount = request.UsdAmount,
                    Status = 0, // PENDING — admin approval required
                    Narration = $"USD Wire Funding: {request.Remarks ?? "Wire Transfer"}. Bank Ref: {request.BankReference}",
                    InitiatedByUserId = customer.UserId,
                    CreatedAt = DateTime.UtcNow
                };
                await _uow.Transactions.CreateTransaction(transaction);
                await _uow.SaveChangesAsync();

                return new ApiResponse
                {
                    Success = true,
                    Message = "Wire funding request submitted. Your USD wallet will be credited once the transfer is confirmed by our team (typically within 1 business day).",
                    Data = new { Reference = reference, UsdAmount = request.UsdAmount },
                    Status = 200
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "InitiateDollarWireFunding Error for Customer {CustomerId}", customerId);
                return new ApiResponse { Message = "An error occurred while submitting the wire funding request.", Status = 500 };
            }
        }
    }
}
