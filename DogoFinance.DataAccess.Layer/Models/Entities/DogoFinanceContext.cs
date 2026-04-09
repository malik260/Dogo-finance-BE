using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-FJOOMDN;Database=DogoFinanceDBStaging;Trusted_Connection=True;TrustServerCertificate=True;");
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

                entity.Property(e => e.Country).HasDefaultValueSql("('Nigeria')");

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

            modelBuilder.Entity<TblCustomerBank>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

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

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
