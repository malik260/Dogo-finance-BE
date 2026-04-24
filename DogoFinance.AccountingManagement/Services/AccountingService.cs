using DogoFinance.AccountingManagement.Interfaces;
using DogoFinance.BusinessLogic.Layer.Response;
using DogoFinance.DataAccess.Layer.DTO;
using DogoFinance.DataAccess.Layer.Interfaces;
using DogoFinance.DataAccess.Layer.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogoFinance.AccountingManagement.Services
{
    public class AccountingService : IAccountingService
    {
        private readonly IUnitOfWork _uow;

        public AccountingService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ApiResponse> GetChartOfAccountsAsync()
        {
            var accounts = await _uow.GenericRepository.AsQueryable<TblChartOfAccount>(a => a.IsActive).ToListAsync();
            var dtos = accounts.Select(a => new ChartOfAccountDto
            {
                Id = a.Id,
                AccountCode = a.AccountCode,
                AccountName = a.AccountName,
                AccountType = a.AccountType,
                IsLeaf = a.IsLeaf
            }).ToList();

            return new ApiResponse { Success = true, Data = dtos, Status = 200 };
        }

        public async Task<ApiResponse> PostJournalAsync(JournalEntryDto entryDto)
        {
            if (entryDto.Lines.Count < 2)
                return new ApiResponse { Success = false, Message = "A journal entry must have at least two lines.", Status = 400 };

            if (entryDto.Lines.Sum(l => l.Debit) != entryDto.Lines.Sum(l => l.Credit))
                return new ApiResponse { Success = false, Message = "Journal entry is not balanced. Debits must equal Credits.", Status = 400 };

            try
            {
                var entry = new TblJournalEntry
                {
                    Reference = entryDto.Reference,
                    Narration = entryDto.Narration,
                    TransactionDate = entryDto.TransactionDate,
                    CreatedAt = DateTime.UtcNow
                };

                foreach (var lineDto in entryDto.Lines)
                {
                    var account = await _uow.GenericRepository.FindEntity<TblChartOfAccount>(a => a.AccountCode == lineDto.AccountCode);
                    if (account == null)
                        return new ApiResponse { Success = false, Message = $"Account with code {lineDto.AccountCode} not found.", Status = 404 };

                    if (!account.IsLeaf)
                        return new ApiResponse { Success = false, Message = $"Account {lineDto.AccountCode} is a header account and cannot be posted to directly.", Status = 400 };

                    entry.JournalLines.Add(new TblJournalLine
                    {
                        AccountId = account.Id,
                        Debit = lineDto.Debit,
                        Credit = lineDto.Credit,
                        Narration = lineDto.Narration ?? entryDto.Narration,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                await _uow.GenericRepository.Insert(entry);
                await _uow.SaveChangesAsync();

                return new ApiResponse { Success = true, Message = "Journal entry posted successfully.", Status = 200 };
            }
            catch (Exception ex)
            {
                return new ApiResponse { Success = false, Message = $"Error posting journal: {ex.Message}", Status = 500 };
            }
        }

        public async Task<ApiResponse> GetTrialBalanceAsync()
        {
            try
            {
                // We'll perform an aggregation over JournalLines grouped by Account
                var trialBalance = await _uow.GenericRepository.AsQueryable<TblJournalLine>(l => true)
                    .GroupBy(l => l.AccountId)
                    .Select(g => new
                    {
                        AccountId = g.Key,
                        TotalDebit = g.Sum(x => x.Debit),
                        TotalCredit = g.Sum(x => x.Credit)
                    })
                    .ToListAsync();

                var accounts = await _uow.GenericRepository.AsQueryable<TblChartOfAccount>(a => a.IsActive).ToListAsync();

                var result = accounts.Select(a => {
                    var lineSum = trialBalance.FirstOrDefault(t => t.AccountId == a.Id);
                    decimal totalDr = lineSum?.TotalDebit ?? 0;
                    decimal totalCr = lineSum?.TotalCredit ?? 0;
                    
                    // Standard balance calculation: 
                    // Assets/Expenses: Dr - Cr
                    // Liabilities/Equity/Revenue: Cr - Dr
                    decimal balance = 0;
                    if (a.AccountType == "Asset" || a.AccountType == "Expense")
                        balance = totalDr - totalCr;
                    else
                        balance = totalCr - totalDr;

                    return new TrialBalanceDto
                    {
                        AccountCode = a.AccountCode,
                        AccountName = a.AccountName,
                        AccountType = a.AccountType,
                        TotalDebit = totalDr,
                        TotalCredit = totalCr,
                        Balance = balance
                    };
                }).OrderBy(a => a.AccountCode).ToList();

                return new ApiResponse { Success = true, Data = result, Status = 200 };
            }
            catch (Exception ex)
            {
                return new ApiResponse { Success = false, Message = $"Error generating trial balance: {ex.Message}", Status = 500 };
            }
        }

        public async Task<ApiResponse> SeedChartOfAccountsAsync()
        {
            var existing = await _uow.GenericRepository.AsQueryable<TblChartOfAccount>(a => true).AnyAsync();
            if (existing) return new ApiResponse { Message = "Chart of Accounts already seeded.", Status = 200 };

            var defaultAccounts = new List<TblChartOfAccount>
            {
                // Assets (1000s)
                new TblChartOfAccount { AccountCode = "1000", AccountName = "ASSETS", AccountType = "Asset", IsLeaf = false },
                new TblChartOfAccount { AccountCode = "1110", AccountName = "Cash at Bank (Monnify)", AccountType = "Asset", IsLeaf = true },
                new TblChartOfAccount { AccountCode = "1210", AccountName = "Investment Portfolios", AccountType = "Asset", IsLeaf = true },
                
                // Liabilities (2000s)
                new TblChartOfAccount { AccountCode = "2000", AccountName = "LIABILITIES", AccountType = "Liability", IsLeaf = false },
                new TblChartOfAccount { AccountCode = "2110", AccountName = "Customer Wallets", AccountType = "Liability", IsLeaf = true },
                new TblChartOfAccount { AccountCode = "2310", AccountName = "Tax Payable", AccountType = "Liability", IsLeaf = true },

                // Equity (3000s)
                new TblChartOfAccount { AccountCode = "3000", AccountName = "EQUITY", AccountType = "Equity", IsLeaf = false },
                new TblChartOfAccount { AccountCode = "3100", AccountName = "Share Capital", AccountType = "Equity", IsLeaf = true },
                new TblChartOfAccount { AccountCode = "3210", AccountName = "Retained Earnings", AccountType = "Equity", IsLeaf = true },

                // Revenue (4000s)
                new TblChartOfAccount { AccountCode = "4000", AccountName = "REVENUE", AccountType = "Revenue", IsLeaf = false },
                new TblChartOfAccount { AccountCode = "4210", AccountName = "Fee Income", AccountType = "Revenue", IsLeaf = true },
                new TblChartOfAccount { AccountCode = "4110", AccountName = "Investment Interest", AccountType = "Revenue", IsLeaf = true },

                // Expenses (5000s)
                new TblChartOfAccount { AccountCode = "5000", AccountName = "EXPENSES", AccountType = "Expense", IsLeaf = false },
                new TblChartOfAccount { AccountCode = "5110", AccountName = "Transaction Charges", AccountType = "Expense", IsLeaf = true },
                new TblChartOfAccount { AccountCode = "5210", AccountName = "Administrative Expenses", AccountType = "Expense", IsLeaf = true },
                new TblChartOfAccount { AccountCode = "5310", AccountName = "Tax Expense", AccountType = "Expense", IsLeaf = true }
            };

            try
            {
                foreach (var acc in defaultAccounts)
                {
                    await _uow.GenericRepository.Insert(acc);
                }
                await _uow.SaveChangesAsync();
                return new ApiResponse { Success = true, Message = "Standard Chart of Accounts seeded successfully.", Status = 200 };
            }
            catch (Exception ex)
            {
                return new ApiResponse { Success = false, Message = $"Seeding failed: {ex.Message}", Status = 500 };
            }
        }
    }
}
