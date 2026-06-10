using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using DogoFinance.DataAccess.Layer.Global;


namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    public partial class DogoFinanceContext : DbContext
    {
        public DogoFinanceContext()
        {
        }

        public DogoFinanceContext(DbContextOptions<DogoFinanceContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TblCurrency> TblCurrencies { get; set; } = null!;
        public virtual DbSet<TblCustomer> TblCustomers { get; set; } = null!;
        public virtual DbSet<TblCustomerType> TblCustomerTypes { get; set; } = null!;
        public virtual DbSet<TblKycLog> TblKycLogs { get; set; } = null!;
        public virtual DbSet<TblLedger> TblLedgers { get; set; } = null!;
        public virtual DbSet<TblNextOfKin> TblNextOfKins { get; set; } = null!;
        public virtual DbSet<TblPasswordReset> TblPasswordResets { get; set; } = null!;
        public virtual DbSet<TblPayment> TblPayments { get; set; } = null!;
        public virtual DbSet<TblRelationshipType> TblRelationshipTypes { get; set; } = null!;
        public virtual DbSet<TblRole> TblRoles { get; set; } = null!;
        public virtual DbSet<TblTransaction> TblTransactions { get; set; } = null!;
        public virtual DbSet<TblTransactionType> TblTransactionTypes { get; set; } = null!;
        public virtual DbSet<TblUser> TblUsers { get; set; } = null!;
        public virtual DbSet<TblUserRole> TblUserRoles { get; set; } = null!;
        public virtual DbSet<TblWallet> TblWallets { get; set; } = null!;
        public virtual DbSet<TblPinReset> TblPinResets { get; set; } = null!;
        public virtual DbSet<TblUserSession> TblUserSessions { get; set; } = null!;
        public virtual DbSet<TblBank> TblBanks { get; set; } = null!;
        public virtual DbSet<TblCustomerBank> TblCustomerBanks { get; set; } = null!;
        public virtual DbSet<TblModule> TblModules { get; set; } = null!;
        public virtual DbSet<TblAccessRight> TblAccessRights { get; set; } = null!;
        public virtual DbSet<TblRoleAccessRight> TblRoleAccessRights { get; set; } = null!;
        public virtual DbSet<TblSystemSetting> TblSystemSettings { get; set; } = null!;
        public virtual DbSet<TblCompanyProfile> TblCompanyProfiles { get; set; } = null!;
        public virtual DbSet<TblGender> TblGenders { get; set; } = null!;
        public virtual DbSet<TblAddressDocType> TblAddressDocTypes { get; set; } = null!;
        public virtual DbSet<TblCustomerAddressVerification> TblCustomerAddressVerifications { get; set; } = null!;
        public virtual DbSet<TblWithdrawalRequest> TblWithdrawalRequests { get; set; } = null!;
        public virtual DbSet<TblLiquidationRequest> TblLiquidationRequests { get; set; } = null!;
        public virtual DbSet<TblManualFundingRequest> TblManualFundingRequests { get; set; } = null!;
        public virtual DbSet<TblCorporateContact> TblCorporateContacts { get; set; } = null!;
        public virtual DbSet<TblCorporateDocument> TblCorporateDocuments { get; set; } = null!;
        public virtual DbSet<TblVerificationItem> TblVerificationItems { get; set; } = null!;
        public virtual DbSet<TblCorporateSignatory> TblCorporateSignatories { get; set; } = null!;
        public virtual DbSet<TblCorporateDirector> TblCorporateDirectors { get; set; } = null!;
        public virtual DbSet<TblNotification> TblNotifications { get; set; } = null!;


        // Portfolio Management
        public virtual DbSet<TblAssetClass> TblAssetClasses { get; set; } = null!;
        public virtual DbSet<TblPortfolioType> TblPortfolioTypes { get; set; } = null!;
        public virtual DbSet<TblPortfolio> TblPortfolios { get; set; } = null!;
        public virtual DbSet<TblInstrument> TblInstruments { get; set; } = null!;
        public virtual DbSet<TblPortfolioInstrument> TblPortfolioInstruments { get; set; } = null!;
        public virtual DbSet<TblPortfolioAllocationRule> TblPortfolioAllocationRules { get; set; } = null!;
        public virtual DbSet<TblInstrumentPrice> TblInstrumentPrices { get; set; } = null!;
        public virtual DbSet<TblCustomerPortfolio> TblCustomerPortfolios { get; set; } = null!;
        public virtual DbSet<TblCustomerHolding> TblCustomerHoldings { get; set; } = null!;
        public virtual DbSet<TblInvestmentTransaction> TblInvestmentTransactions { get; set; } = null!;
        public virtual DbSet<TblReservedAccount> TblReservedAccounts { get; set; } = null!;
        public virtual DbSet<TblPortfolioPrice> TblPortfolioPrices { get; set; } = null!;

        // Accounting & Bookkeeping
        public virtual DbSet<TblChartOfAccount> TblChartOfAccounts { get; set; } = null!;
        public virtual DbSet<TblJournalEntry> TblJournalEntries { get; set; } = null!;
        public virtual DbSet<TblJournalLine> TblJournalLines { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(GlobalContext.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TblCurrency>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<TblCustomer>(entity =>
            {
                entity.HasKey(e => e.CustomerId)
                    .HasName("PK__TBL_CUST__A4AE64D8B9B6FDD4");

                entity.HasIndex(e => e.Bvn, "IX_TBL_CUSTOMER_BVN")
                    .IsUnique()
                    .HasFilter("([BVN] IS NOT NULL)");

                entity.HasIndex(e => e.Nin, "IX_TBL_CUSTOMER_NIN")
                    .IsUnique()
                    .HasFilter("([NIN] IS NOT NULL)");

                entity.Property(e => e.CustomerTypeId).HasDefaultValueSql("((1))");
                entity.Property(e => e.Country).HasDefaultValueSql("('Nigeria')");

                entity.HasOne(d => d.CustomerType)
                    .WithMany(p => p.TblCustomers)
                    .HasForeignKey(d => d.CustomerTypeId)
                    .HasConstraintName("FK_TBL_CUSTOMER_CUSTOMER_TYPE");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.User)
                    .WithOne(p => p.TblCustomer)
                    .HasForeignKey<TblCustomer>(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TBL_CUSTOMER_USER");
            });

            modelBuilder.Entity<TblCorporateContact>(entity =>
            {
                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.TblCorporateContacts)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CORPORATE_CONTACT_CUSTOMER");
            });

            modelBuilder.Entity<TblCorporateDirector>(entity =>
            {
                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.TblCorporateDirectors)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TBL_CORPORATE_DIRECTOR_TBL_CUSTOMER");
            });

            modelBuilder.Entity<TblNotification>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.TblNotifications)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_TBL_NOTIFICATION_CUSTOMER");
            });

            modelBuilder.Entity<TblCorporateSignatory>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.TblCorporateSignatories)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CORPORATE_SIGNATORY_CUSTOMER");
            });

            modelBuilder.Entity<TblKycLog>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.TblKycLogs)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_KYC_CUSTOMER");
            });

            modelBuilder.Entity<TblLedger>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<TblNextOfKin>(entity =>
            {
                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.TblNextOfKins)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NEXT_OF_KIN_CUSTOMER");

                entity.HasOne(d => d.RelationshipType)
                    .WithMany(p => p.TblNextOfKins)
                    .HasForeignKey(d => d.RelationshipTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NEXT_OF_KIN_RELATIONSHIP");
            });

            modelBuilder.Entity<TblPasswordReset>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TblPasswordResets)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PASSWORD_RESET_USER");
            });

            modelBuilder.Entity<TblPayment>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TblPayments)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PAYMENT_USER");
            });

            modelBuilder.Entity<TblRelationshipType>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<TblTransaction>(entity =>
            {
                entity.HasKey(e => e.TransactionId)
                    .HasName("PK__TBL_TRAN__55433A6BBD1D0E19");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Status).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.ApprovedByUser)
                    .WithMany(p => p.TblTransactionApprovedByUsers)
                    .HasForeignKey(d => d.ApprovedByUserId)
                    .HasConstraintName("FK_TXN_USER_APPROVE");

                entity.HasOne(d => d.InitiatedByUser)
                    .WithMany(p => p.TblTransactionInitiatedByUsers)
                    .HasForeignKey(d => d.InitiatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TXN_USER_INIT");

                entity.HasOne(d => d.Payment)
                    .WithMany(p => p.TblTransactions)
                    .HasForeignKey(d => d.PaymentId)
                    .HasConstraintName("FK_TXN_PAYMENT");

                entity.HasOne(d => d.ReversedTransaction)
                    .WithMany(p => p.InverseReversedTransaction)
                    .HasForeignKey(d => d.ReversedTransactionId)
                    .HasConstraintName("FK_TXN_REVERSAL");
            });

            modelBuilder.Entity<TblUser>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__TBL_USER__1788CC4C596A566F");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });

            modelBuilder.Entity<TblUserRole>(entity =>
            {
                entity.HasOne(d => d.Role)
                    .WithMany(p => p.TblUserRoles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_USER_ROLE_ROLE");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TblUserRoles)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_USER_ROLE_USER");
            });

            modelBuilder.Entity<TblWallet>(entity =>
            {
                entity.HasKey(e => e.WalletId)
                    .HasName("PK__TBL_WALL__84D4F90E05132222");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.TblWallets)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WALLET_CUSTOMER");
            });

            modelBuilder.Entity<TblBank>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<TblCustomerType>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.HasData(
                    new TblCustomerType { Id = 1, Name = "Individual", Description = "Individual Customer Account", IsActive = true },
                    new TblCustomerType { Id = 2, Name = "Corporate", Description = "Corporate/Business Account", IsActive = true }
                );
            });

            modelBuilder.Entity<TblCustomerBank>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.CurrencyCode).HasDefaultValueSql("('NGN')");

                entity.HasOne(d => d.Bank)
                    .WithMany(p => p.TblCustomerBanks)
                    .HasForeignKey(d => d.BankId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CUSTOMER_BANK_BANK");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.TblCustomerBanks)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CUSTOMER_BANK_CUSTOMER");
            });

            modelBuilder.Entity<TblRoleAccessRight>(entity =>
            {
                entity.ToTable("TBL_ROLE_ACCESS_RIGHT");
                entity.HasKey(e => e.Id);

                entity.HasOne(d => d.Role)
                    .WithMany()
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.AccessRight)
                    .WithMany()
                    .HasForeignKey(d => d.AccessRightId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });


            modelBuilder.Entity<TblSystemSetting>(entity =>
            {
                entity.ToTable("TBL_SYSTEM_SETTING");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<TblGender>(entity =>
            {
                entity.ToTable("TBL_GENDER");
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<TblCompanyProfile>(entity =>
            {
                entity.ToTable("TBL_COMPANY_PROFILE");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            });

            // Portfolio Management Configs
            modelBuilder.Entity<TblAssetClass>(entity => {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<TblPortfolioType>(entity => {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.SupportsAllocation).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TblPortfolio>(entity => {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<TblInstrument>(entity => {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<TblCustomerPortfolio>(entity => {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<TblCustomerHolding>(entity => {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<TblInstrumentPrice>(entity => {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<TblInvestmentTransaction>(entity => {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<TblLiquidationRequest>(entity => {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.Status).HasDefaultValueSql("((1))"); // PENDING_APPROVAL
            });

            modelBuilder.Entity<TblManualFundingRequest>(entity => {
                entity.Property(e => e.InitiatedAt).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.Status).HasDefaultValueSql("('Pending')");
            });


            modelBuilder.Entity<TblAddressDocType>(entity => {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<TblCustomerAddressVerification>(entity => {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.Status).HasDefaultValueSql("('Pending')");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.TblCustomerAddressVerifications)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ADDR_VERIF_CUSTOMER");

                entity.HasOne(d => d.DocType)
                    .WithMany(p => p.TblCustomerAddressVerifications)
                    .HasForeignKey(d => d.DocTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ADDR_VERIF_DOCTYPE");
            });

            modelBuilder.Entity<TblChartOfAccount>(entity => {
                entity.HasIndex(e => e.AccountCode).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<TblJournalEntry>(entity => {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<TblJournalLine>(entity => {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<TblVerificationItem>(entity => {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.HasData(
                    new TblVerificationItem { Id = 1, Name = "1. Completed Application Form", Type = "appForm", IsSystemVerified = true, SystemRule = "CheckAppForm", TargetEntityTypes = "Corporate", Icon = "ri-file-list-3-line", DisplayOrder = 1, IsActive = true, RequiresUpload = false },
                    new TblVerificationItem { Id = 2, Name = "2. Certificate of Incorporation", Type = "incorporation", IsSystemVerified = false, TargetEntityTypes = "Corporate", Icon = "ri-verified-badge-line", DisplayOrder = 2, IsActive = true, RequiresUpload = true },
                    new TblVerificationItem { Id = 3, Name = "3. Passport Photography of each Authorized Signatory", Type = "passport", IsSystemVerified = true, SystemRule = "CheckSignatoryPhotos", TargetEntityTypes = "Corporate", Icon = "ri-user-line", DisplayOrder = 3, IsActive = true, RequiresUpload = true },
                    new TblVerificationItem { Id = 4, Name = "4. Memorandum & Articles of Association", Type = "memart", IsSystemVerified = false, TargetEntityTypes = "Corporate", Icon = "ri-book-read-line", DisplayOrder = 4, IsActive = true, RequiresUpload = true },
                    new TblVerificationItem { Id = 5, Name = "5. Form CAC 2 (Return of Allotment of Shares)", Type = "cac2", IsSystemVerified = false, TargetEntityTypes = "Corporate", Icon = "ri-pie-chart-line", DisplayOrder = 5, IsActive = true, RequiresUpload = true },
                    new TblVerificationItem { Id = 6, Name = "6. Form CAC 7 (Particulars of Directors)", Type = "cac7", IsSystemVerified = true, SystemRule = "CheckDirectorsAdded", TargetEntityTypes = "Corporate", Icon = "ri-folder-user-line", DisplayOrder = 6, IsActive = true, RequiresUpload = false },
                    new TblVerificationItem { Id = 7, Name = "7. Form CAC 3 (Notice of Situation/Change of Registered Address)", Type = "cac3", IsSystemVerified = false, TargetEntityTypes = "Corporate", Icon = "ri-map-pin-user-line", DisplayOrder = 7, IsActive = true, RequiresUpload = true },
                    new TblVerificationItem { Id = 8, Name = "8. Copy of Identification of Authorized Signatories and Directors", Type = "signatoryId", IsSystemVerified = true, SystemRule = "CheckSignatoryDirectorsId", TargetEntityTypes = "Corporate", Icon = "ri-shield-user-line", DisplayOrder = 8, IsActive = true, RequiresUpload = true },
                    new TblVerificationItem { Id = 9, Name = "9. Board Resolution/minutes of meeting confirming Authorized Signatories", Type = "boardResolution", IsSystemVerified = false, TargetEntityTypes = "Corporate", Icon = "ri-team-line", DisplayOrder = 9, IsActive = true, RequiresUpload = true },
                    new TblVerificationItem { Id = 10, Name = "10. Link Settlement Bank Account", Type = "settlementLink", IsSystemVerified = true, SystemRule = "CheckBankLinked", TargetEntityTypes = "Corporate", Icon = "ri-bank-line", DisplayOrder = 10, IsActive = true, RequiresUpload = false }
                );
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
