using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DogoFinance.DataAccess.Layer.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBL_ADDRESS_DOC_TYPE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_ADDRESS_DOC_TYPE", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBL_ASSET_CLASS",
                columns: table => new
                {
                    AssetClassId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsShariahCompliant = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_ASSET_CLASS", x => x.AssetClassId);
                });

            migrationBuilder.CreateTable(
                name: "TBL_BANK",
                columns: table => new
                {
                    BankId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BankCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    LogoUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_BANK", x => x.BankId);
                });

            migrationBuilder.CreateTable(
                name: "TBL_CHART_OF_ACCOUNT",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccountType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsLeaf = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_CHART_OF_ACCOUNT", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBL_CURRENCY",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_CURRENCY", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBL_GENDER",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_GENDER", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBL_INSTRUMENT",
                columns: table => new
                {
                    InstrumentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsShariahCompliant = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_INSTRUMENT", x => x.InstrumentId);
                });

            migrationBuilder.CreateTable(
                name: "TBL_JOURNAL_ENTRY",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Narration = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_JOURNAL_ENTRY", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBL_LEDGER",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionId = table.Column<long>(type: "bigint", nullable: false),
                    WalletId = table.Column<long>(type: "bigint", nullable: false),
                    EntryType = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BalanceAfter = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Narration = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_LEDGER", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBL_MODULE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_MODULE", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBL_PORTFOLIO_TYPE",
                columns: table => new
                {
                    PortfolioTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SupportsAllocation = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((0))"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_PORTFOLIO_TYPE", x => x.PortfolioTypeId);
                });

            migrationBuilder.CreateTable(
                name: "TBL_RELATIONSHIP_TYPE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_RELATIONSHIP_TYPE", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBL_ROLE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_ROLE", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBL_SYSTEM_SETTING",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionTimeoutInMinutes = table.Column<int>(type: "int", nullable: false),
                    WithdrawalAutoThreshold = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_SYSTEM_SETTING", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBL_TRANSACTION_TYPE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_TRANSACTION_TYPE", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBL_USER",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Salt = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    FailedLoginAttempts = table.Column<int>(type: "int", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastLogoutDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastPasswordChangeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsSystemUser = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TransactionPinHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TransactionPinSalt = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsPinSet = table.Column<bool>(type: "bit", nullable: false),
                    PinFailedAttempts = table.Column<int>(type: "int", nullable: false),
                    IsPinLocked = table.Column<bool>(type: "bit", nullable: false),
                    LastPinChangeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Is2faEnabled = table.Column<bool>(type: "bit", nullable: true),
                    VerificationCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    VerificationExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RefreshTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TBL_USER__1788CC4C596A566F", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "TBL_INSTRUMENT_PRICE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstrumentId = table.Column<int>(type: "int", nullable: false),
                    PriceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NAV = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    PriceSource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_INSTRUMENT_PRICE", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_INSTRUMENT_PRICE_TBL_INSTRUMENT_InstrumentId",
                        column: x => x.InstrumentId,
                        principalTable: "TBL_INSTRUMENT",
                        principalColumn: "InstrumentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TBL_JOURNAL_LINE",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JournalEntryId = table.Column<long>(type: "bigint", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    Debit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Credit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Narration = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_JOURNAL_LINE", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_JOURNAL_LINE_TBL_CHART_OF_ACCOUNT_AccountId",
                        column: x => x.AccountId,
                        principalTable: "TBL_CHART_OF_ACCOUNT",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TBL_JOURNAL_LINE_TBL_JOURNAL_ENTRY_JournalEntryId",
                        column: x => x.JournalEntryId,
                        principalTable: "TBL_JOURNAL_ENTRY",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TBL_ACCESS_RIGHT",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModuleId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_ACCESS_RIGHT", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_ACCESS_RIGHT_TBL_MODULE_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "TBL_MODULE",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TBL_PORTFOLIO",
                columns: table => new
                {
                    PortfolioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PortfolioTypeId = table.Column<int>(type: "int", nullable: false),
                    RiskLevel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpectedAnnualReturn = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    LockInPeriodDays = table.Column<int>(type: "int", nullable: false),
                    MinHoldingPeriodDays = table.Column<int>(type: "int", nullable: false),
                    ExitFeePercentage = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    NoticePeriodDays = table.Column<int>(type: "int", nullable: false),
                    ApprovalThresholdAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_PORTFOLIO", x => x.PortfolioId);
                    table.ForeignKey(
                        name: "FK_TBL_PORTFOLIO_TBL_PORTFOLIO_TYPE_PortfolioTypeId",
                        column: x => x.PortfolioTypeId,
                        principalTable: "TBL_PORTFOLIO_TYPE",
                        principalColumn: "PortfolioTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TBL_CUSTOMER",
                columns: table => new
                {
                    CustomerId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OtherNames = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "date", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: true),
                    BVN = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    BVNVerified = table.Column<bool>(type: "bit", nullable: false),
                    BVNVerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NIN = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    NINVerified = table.Column<bool>(type: "bit", nullable: false),
                    NINVerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Country = table.Column<int>(type: "int", nullable: true, defaultValueSql: "('Nigeria')"),
                    KYCStatus = table.Column<int>(type: "int", nullable: false),
                    KYCVerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsPolitcallyExposed = table.Column<bool>(type: "bit", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TBL_CUST__A4AE64D8B9B6FDD4", x => x.CustomerId);
                    table.ForeignKey(
                        name: "FK_TBL_CUSTOMER_USER",
                        column: x => x.UserId,
                        principalTable: "TBL_USER",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "TBL_PASSWORD_RESET",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ResetCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_PASSWORD_RESET", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PASSWORD_RESET_USER",
                        column: x => x.UserId,
                        principalTable: "TBL_USER",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "TBL_PAYMENT",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentProvider = table.Column<int>(type: "int", nullable: false),
                    ProviderReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_PAYMENT", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PAYMENT_USER",
                        column: x => x.UserId,
                        principalTable: "TBL_USER",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "TBL_PIN_RESET",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ResetCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_PIN_RESET", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_PIN_RESET_TBL_USER_UserId",
                        column: x => x.UserId,
                        principalTable: "TBL_USER",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TBL_RESERVED_ACCOUNT",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    AccountReference = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_RESERVED_ACCOUNT", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_RESERVED_ACCOUNT_TBL_USER_UserId",
                        column: x => x.UserId,
                        principalTable: "TBL_USER",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TBL_USER_ROLE",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_USER_ROLE", x => x.Id);
                    table.ForeignKey(
                        name: "FK_USER_ROLE_ROLE",
                        column: x => x.RoleId,
                        principalTable: "TBL_ROLE",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_USER_ROLE_USER",
                        column: x => x.UserId,
                        principalTable: "TBL_USER",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "TBL_USER_SESSION",
                columns: table => new
                {
                    SessionId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RefreshTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUsedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeviceName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_USER_SESSION", x => x.SessionId);
                    table.ForeignKey(
                        name: "FK_TBL_USER_SESSION_TBL_USER_UserId",
                        column: x => x.UserId,
                        principalTable: "TBL_USER",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TBL_ROLE_ACCESS_RIGHT",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    AccessRightId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_ROLE_ACCESS_RIGHT", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_ROLE_ACCESS_RIGHT_TBL_ACCESS_RIGHT_AccessRightId",
                        column: x => x.AccessRightId,
                        principalTable: "TBL_ACCESS_RIGHT",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_ROLE_ACCESS_RIGHT_TBL_ROLE_RoleId",
                        column: x => x.RoleId,
                        principalTable: "TBL_ROLE",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TBL_PORTFOLIO_ALLOCATION_RULE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PortfolioId = table.Column<int>(type: "int", nullable: false),
                    AssetClassId = table.Column<int>(type: "int", nullable: false),
                    TargetPercentage = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    MinPercentage = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    MaxPercentage = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    ExpectedReturn = table.Column<decimal>(type: "decimal(18,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_PORTFOLIO_ALLOCATION_RULE", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_PORTFOLIO_ALLOCATION_RULE_TBL_ASSET_CLASS_AssetClassId",
                        column: x => x.AssetClassId,
                        principalTable: "TBL_ASSET_CLASS",
                        principalColumn: "AssetClassId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TBL_PORTFOLIO_ALLOCATION_RULE_TBL_PORTFOLIO_PortfolioId",
                        column: x => x.PortfolioId,
                        principalTable: "TBL_PORTFOLIO",
                        principalColumn: "PortfolioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TBL_PORTFOLIO_INSTRUMENT",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PortfolioId = table.Column<int>(type: "int", nullable: false),
                    InstrumentId = table.Column<int>(type: "int", nullable: false),
                    AssetClassId = table.Column<int>(type: "int", nullable: false),
                    TargetWeight = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_PORTFOLIO_INSTRUMENT", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_PORTFOLIO_INSTRUMENT_TBL_ASSET_CLASS_AssetClassId",
                        column: x => x.AssetClassId,
                        principalTable: "TBL_ASSET_CLASS",
                        principalColumn: "AssetClassId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TBL_PORTFOLIO_INSTRUMENT_TBL_INSTRUMENT_InstrumentId",
                        column: x => x.InstrumentId,
                        principalTable: "TBL_INSTRUMENT",
                        principalColumn: "InstrumentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TBL_PORTFOLIO_INSTRUMENT_TBL_PORTFOLIO_PortfolioId",
                        column: x => x.PortfolioId,
                        principalTable: "TBL_PORTFOLIO",
                        principalColumn: "PortfolioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TBL_PORTFOLIO_PRICE",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PortfolioId = table.Column<int>(type: "int", nullable: false),
                    PriceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NAV = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_PORTFOLIO_PRICE", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_PORTFOLIO_PRICE_TBL_PORTFOLIO_PortfolioId",
                        column: x => x.PortfolioId,
                        principalTable: "TBL_PORTFOLIO",
                        principalColumn: "PortfolioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TBL_CUSTOMER_ADDRESS_VERIFICATION",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    DocTypeId = table.Column<int>(type: "int", nullable: false),
                    DocumentUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CloudinaryPublicId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ExtractedAddress = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ExtractedCity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ExtractedState = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ExtractedFullText = table.Column<string>(type: "nvarchar(MAX)", nullable: true),
                    ConfidenceScore = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValueSql: "('Pending')"),
                    AdminNotes = table.Column<string>(type: "nvarchar(MAX)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_CUSTOMER_ADDRESS_VERIFICATION", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ADDR_VERIF_CUSTOMER",
                        column: x => x.CustomerId,
                        principalTable: "TBL_CUSTOMER",
                        principalColumn: "CustomerId");
                    table.ForeignKey(
                        name: "FK_ADDR_VERIF_DOCTYPE",
                        column: x => x.DocTypeId,
                        principalTable: "TBL_ADDRESS_DOC_TYPE",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TBL_CUSTOMER_BANK",
                columns: table => new
                {
                    CustomerBankId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    BankId = table.Column<int>(type: "int", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_CUSTOMER_BANK", x => x.CustomerBankId);
                    table.ForeignKey(
                        name: "FK_CUSTOMER_BANK_BANK",
                        column: x => x.BankId,
                        principalTable: "TBL_BANK",
                        principalColumn: "BankId");
                    table.ForeignKey(
                        name: "FK_CUSTOMER_BANK_CUSTOMER",
                        column: x => x.CustomerId,
                        principalTable: "TBL_CUSTOMER",
                        principalColumn: "CustomerId");
                });

            migrationBuilder.CreateTable(
                name: "TBL_CUSTOMER_HOLDING",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    InstrumentId = table.Column<int>(type: "int", nullable: false),
                    Units = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    InvestedAmount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_CUSTOMER_HOLDING", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_CUSTOMER_HOLDING_TBL_CUSTOMER_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "TBL_CUSTOMER",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TBL_CUSTOMER_HOLDING_TBL_INSTRUMENT_InstrumentId",
                        column: x => x.InstrumentId,
                        principalTable: "TBL_INSTRUMENT",
                        principalColumn: "InstrumentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TBL_CUSTOMER_PORTFOLIO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    PortfolioId = table.Column<int>(type: "int", nullable: false),
                    TotalInvested = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Units = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    InvestedAmount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_CUSTOMER_PORTFOLIO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_CUSTOMER_PORTFOLIO_TBL_CUSTOMER_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "TBL_CUSTOMER",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TBL_CUSTOMER_PORTFOLIO_TBL_PORTFOLIO_PortfolioId",
                        column: x => x.PortfolioId,
                        principalTable: "TBL_PORTFOLIO",
                        principalColumn: "PortfolioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TBL_INVESTMENT_TRANSACTION",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    PortfolioId = table.Column<int>(type: "int", nullable: false),
                    InstrumentId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Units = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    NAV = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_INVESTMENT_TRANSACTION", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_INVESTMENT_TRANSACTION_TBL_CUSTOMER_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "TBL_CUSTOMER",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TBL_INVESTMENT_TRANSACTION_TBL_INSTRUMENT_InstrumentId",
                        column: x => x.InstrumentId,
                        principalTable: "TBL_INSTRUMENT",
                        principalColumn: "InstrumentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TBL_INVESTMENT_TRANSACTION_TBL_PORTFOLIO_PortfolioId",
                        column: x => x.PortfolioId,
                        principalTable: "TBL_PORTFOLIO",
                        principalColumn: "PortfolioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TBL_KYC_LOG",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Response = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_KYC_LOG", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KYC_CUSTOMER",
                        column: x => x.CustomerId,
                        principalTable: "TBL_CUSTOMER",
                        principalColumn: "CustomerId");
                });

            migrationBuilder.CreateTable(
                name: "TBL_LIQUIDATION_REQUEST",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    PortfolioId = table.Column<int>(type: "int", nullable: false),
                    UnitsRequested = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    GrossAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExitFeeApplied = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetPayableAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValueSql: "((1))"),
                    ExpectedReleaseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AdminNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewedByAdminId = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_LIQUIDATION_REQUEST", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_LIQUIDATION_REQUEST_TBL_CUSTOMER_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "TBL_CUSTOMER",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TBL_LIQUIDATION_REQUEST_TBL_PORTFOLIO_PortfolioId",
                        column: x => x.PortfolioId,
                        principalTable: "TBL_PORTFOLIO",
                        principalColumn: "PortfolioId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TBL_LIQUIDATION_REQUEST_TBL_USER_ReviewedByAdminId",
                        column: x => x.ReviewedByAdminId,
                        principalTable: "TBL_USER",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "TBL_NEXT_OF_KIN",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    RelationshipTypeId = table.Column<int>(type: "int", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_NEXT_OF_KIN", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NEXT_OF_KIN_CUSTOMER",
                        column: x => x.CustomerId,
                        principalTable: "TBL_CUSTOMER",
                        principalColumn: "CustomerId");
                    table.ForeignKey(
                        name: "FK_NEXT_OF_KIN_RELATIONSHIP",
                        column: x => x.RelationshipTypeId,
                        principalTable: "TBL_RELATIONSHIP_TYPE",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TBL_WALLET",
                columns: table => new
                {
                    WalletId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    WalletNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Currency = table.Column<int>(type: "int", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TBL_WALL__84D4F90E05132222", x => x.WalletId);
                    table.ForeignKey(
                        name: "FK_WALLET_CUSTOMER",
                        column: x => x.CustomerId,
                        principalTable: "TBL_CUSTOMER",
                        principalColumn: "CustomerId");
                });

            migrationBuilder.CreateTable(
                name: "TBL_WITHDRAWAL_REQUEST",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Narration = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Reference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BankCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AccountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InitiatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedByAdminId = table.Column<long>(type: "bigint", nullable: true),
                    AdminNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_WITHDRAWAL_REQUEST", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_WITHDRAWAL_REQUEST_TBL_CUSTOMER_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "TBL_CUSTOMER",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TBL_WITHDRAWAL_REQUEST_TBL_USER_ReviewedByAdminId",
                        column: x => x.ReviewedByAdminId,
                        principalTable: "TBL_USER",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "TBL_TRANSACTION",
                columns: table => new
                {
                    TransactionId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: true, defaultValueSql: "((0))"),
                    Narration = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PaymentId = table.Column<long>(type: "bigint", nullable: true),
                    IsReversed = table.Column<bool>(type: "bit", nullable: false),
                    ReversedTransactionId = table.Column<long>(type: "bigint", nullable: true),
                    InitiatedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    ApprovedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TBL_TRAN__55433A6BBD1D0E19", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_TXN_PAYMENT",
                        column: x => x.PaymentId,
                        principalTable: "TBL_PAYMENT",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TXN_REVERSAL",
                        column: x => x.ReversedTransactionId,
                        principalTable: "TBL_TRANSACTION",
                        principalColumn: "TransactionId");
                    table.ForeignKey(
                        name: "FK_TXN_USER_APPROVE",
                        column: x => x.ApprovedByUserId,
                        principalTable: "TBL_USER",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_TXN_USER_INIT",
                        column: x => x.InitiatedByUserId,
                        principalTable: "TBL_USER",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBL_ACCESS_RIGHT_ModuleId",
                table: "TBL_ACCESS_RIGHT",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CHART_OF_ACCOUNT_AccountCode",
                table: "TBL_CHART_OF_ACCOUNT",
                column: "AccountCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CUSTOMER_BVN",
                table: "TBL_CUSTOMER",
                column: "BVN",
                unique: true,
                filter: "([BVN] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CUSTOMER_NIN",
                table: "TBL_CUSTOMER",
                column: "NIN",
                unique: true,
                filter: "([NIN] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CUSTOMER_UserId",
                table: "TBL_CUSTOMER",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CUSTOMER_ADDRESS_VERIFICATION_CustomerId",
                table: "TBL_CUSTOMER_ADDRESS_VERIFICATION",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CUSTOMER_ADDRESS_VERIFICATION_DocTypeId",
                table: "TBL_CUSTOMER_ADDRESS_VERIFICATION",
                column: "DocTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CUSTOMER_BANK_BankId",
                table: "TBL_CUSTOMER_BANK",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CUSTOMER_BANK_CustomerId",
                table: "TBL_CUSTOMER_BANK",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CUSTOMER_HOLDING_CustomerId",
                table: "TBL_CUSTOMER_HOLDING",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CUSTOMER_HOLDING_InstrumentId",
                table: "TBL_CUSTOMER_HOLDING",
                column: "InstrumentId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CUSTOMER_PORTFOLIO_CustomerId",
                table: "TBL_CUSTOMER_PORTFOLIO",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CUSTOMER_PORTFOLIO_PortfolioId",
                table: "TBL_CUSTOMER_PORTFOLIO",
                column: "PortfolioId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_INSTRUMENT_PRICE_InstrumentId",
                table: "TBL_INSTRUMENT_PRICE",
                column: "InstrumentId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_INVESTMENT_TRANSACTION_CustomerId",
                table: "TBL_INVESTMENT_TRANSACTION",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_INVESTMENT_TRANSACTION_InstrumentId",
                table: "TBL_INVESTMENT_TRANSACTION",
                column: "InstrumentId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_INVESTMENT_TRANSACTION_PortfolioId",
                table: "TBL_INVESTMENT_TRANSACTION",
                column: "PortfolioId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_JOURNAL_LINE_AccountId",
                table: "TBL_JOURNAL_LINE",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_JOURNAL_LINE_JournalEntryId",
                table: "TBL_JOURNAL_LINE",
                column: "JournalEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_KYC_LOG_CustomerId",
                table: "TBL_KYC_LOG",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_LIQUIDATION_REQUEST_CustomerId",
                table: "TBL_LIQUIDATION_REQUEST",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_LIQUIDATION_REQUEST_PortfolioId",
                table: "TBL_LIQUIDATION_REQUEST",
                column: "PortfolioId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_LIQUIDATION_REQUEST_ReviewedByAdminId",
                table: "TBL_LIQUIDATION_REQUEST",
                column: "ReviewedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_NEXT_OF_KIN_CustomerId",
                table: "TBL_NEXT_OF_KIN",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_NEXT_OF_KIN_RelationshipTypeId",
                table: "TBL_NEXT_OF_KIN",
                column: "RelationshipTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PASSWORD_RESET_UserId",
                table: "TBL_PASSWORD_RESET",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PAYMENT_UserId",
                table: "TBL_PAYMENT",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PIN_RESET_UserId",
                table: "TBL_PIN_RESET",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PORTFOLIO_PortfolioTypeId",
                table: "TBL_PORTFOLIO",
                column: "PortfolioTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PORTFOLIO_ALLOCATION_RULE_AssetClassId",
                table: "TBL_PORTFOLIO_ALLOCATION_RULE",
                column: "AssetClassId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PORTFOLIO_ALLOCATION_RULE_PortfolioId",
                table: "TBL_PORTFOLIO_ALLOCATION_RULE",
                column: "PortfolioId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PORTFOLIO_INSTRUMENT_AssetClassId",
                table: "TBL_PORTFOLIO_INSTRUMENT",
                column: "AssetClassId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PORTFOLIO_INSTRUMENT_InstrumentId",
                table: "TBL_PORTFOLIO_INSTRUMENT",
                column: "InstrumentId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PORTFOLIO_INSTRUMENT_PortfolioId",
                table: "TBL_PORTFOLIO_INSTRUMENT",
                column: "PortfolioId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PORTFOLIO_PRICE_PortfolioId",
                table: "TBL_PORTFOLIO_PRICE",
                column: "PortfolioId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RESERVED_ACCOUNT_UserId",
                table: "TBL_RESERVED_ACCOUNT",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_ROLE_ACCESS_RIGHT_AccessRightId",
                table: "TBL_ROLE_ACCESS_RIGHT",
                column: "AccessRightId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_ROLE_ACCESS_RIGHT_RoleId",
                table: "TBL_ROLE_ACCESS_RIGHT",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_TRANSACTION_ApprovedByUserId",
                table: "TBL_TRANSACTION",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_TRANSACTION_InitiatedByUserId",
                table: "TBL_TRANSACTION",
                column: "InitiatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_TRANSACTION_PaymentId",
                table: "TBL_TRANSACTION",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_TRANSACTION_ReversedTransactionId",
                table: "TBL_TRANSACTION",
                column: "ReversedTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_USER_Email",
                table: "TBL_USER",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_USER_PhoneNumber",
                table: "TBL_USER",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_USER_UserName",
                table: "TBL_USER",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_USER_ROLE_RoleId",
                table: "TBL_USER_ROLE",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_USER_ROLE_UserId",
                table: "TBL_USER_ROLE",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_USER_SESSION_UserId",
                table: "TBL_USER_SESSION",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_WALLET_CustomerId",
                table: "TBL_WALLET",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_WITHDRAWAL_REQUEST_CustomerId",
                table: "TBL_WITHDRAWAL_REQUEST",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_WITHDRAWAL_REQUEST_ReviewedByAdminId",
                table: "TBL_WITHDRAWAL_REQUEST",
                column: "ReviewedByAdminId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBL_CURRENCY");

            migrationBuilder.DropTable(
                name: "TBL_CUSTOMER_ADDRESS_VERIFICATION");

            migrationBuilder.DropTable(
                name: "TBL_CUSTOMER_BANK");

            migrationBuilder.DropTable(
                name: "TBL_CUSTOMER_HOLDING");

            migrationBuilder.DropTable(
                name: "TBL_CUSTOMER_PORTFOLIO");

            migrationBuilder.DropTable(
                name: "TBL_GENDER");

            migrationBuilder.DropTable(
                name: "TBL_INSTRUMENT_PRICE");

            migrationBuilder.DropTable(
                name: "TBL_INVESTMENT_TRANSACTION");

            migrationBuilder.DropTable(
                name: "TBL_JOURNAL_LINE");

            migrationBuilder.DropTable(
                name: "TBL_KYC_LOG");

            migrationBuilder.DropTable(
                name: "TBL_LEDGER");

            migrationBuilder.DropTable(
                name: "TBL_LIQUIDATION_REQUEST");

            migrationBuilder.DropTable(
                name: "TBL_NEXT_OF_KIN");

            migrationBuilder.DropTable(
                name: "TBL_PASSWORD_RESET");

            migrationBuilder.DropTable(
                name: "TBL_PIN_RESET");

            migrationBuilder.DropTable(
                name: "TBL_PORTFOLIO_ALLOCATION_RULE");

            migrationBuilder.DropTable(
                name: "TBL_PORTFOLIO_INSTRUMENT");

            migrationBuilder.DropTable(
                name: "TBL_PORTFOLIO_PRICE");

            migrationBuilder.DropTable(
                name: "TBL_RESERVED_ACCOUNT");

            migrationBuilder.DropTable(
                name: "TBL_ROLE_ACCESS_RIGHT");

            migrationBuilder.DropTable(
                name: "TBL_SYSTEM_SETTING");

            migrationBuilder.DropTable(
                name: "TBL_TRANSACTION");

            migrationBuilder.DropTable(
                name: "TBL_TRANSACTION_TYPE");

            migrationBuilder.DropTable(
                name: "TBL_USER_ROLE");

            migrationBuilder.DropTable(
                name: "TBL_USER_SESSION");

            migrationBuilder.DropTable(
                name: "TBL_WALLET");

            migrationBuilder.DropTable(
                name: "TBL_WITHDRAWAL_REQUEST");

            migrationBuilder.DropTable(
                name: "TBL_ADDRESS_DOC_TYPE");

            migrationBuilder.DropTable(
                name: "TBL_BANK");

            migrationBuilder.DropTable(
                name: "TBL_CHART_OF_ACCOUNT");

            migrationBuilder.DropTable(
                name: "TBL_JOURNAL_ENTRY");

            migrationBuilder.DropTable(
                name: "TBL_RELATIONSHIP_TYPE");

            migrationBuilder.DropTable(
                name: "TBL_ASSET_CLASS");

            migrationBuilder.DropTable(
                name: "TBL_INSTRUMENT");

            migrationBuilder.DropTable(
                name: "TBL_PORTFOLIO");

            migrationBuilder.DropTable(
                name: "TBL_ACCESS_RIGHT");

            migrationBuilder.DropTable(
                name: "TBL_PAYMENT");

            migrationBuilder.DropTable(
                name: "TBL_ROLE");

            migrationBuilder.DropTable(
                name: "TBL_CUSTOMER");

            migrationBuilder.DropTable(
                name: "TBL_PORTFOLIO_TYPE");

            migrationBuilder.DropTable(
                name: "TBL_MODULE");

            migrationBuilder.DropTable(
                name: "TBL_USER");
        }
    }
}
