IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [TBL_ADDRESS_DOC_TYPE] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(250) NULL,
    [IsActive] bit NOT NULL DEFAULT (((1))),
    [CreatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    CONSTRAINT [PK_TBL_ADDRESS_DOC_TYPE] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [TBL_ASSET_CLASS] (
    [AssetClassId] int NOT NULL IDENTITY,
    [Name] nvarchar(300) NOT NULL,
    [Code] nvarchar(100) NOT NULL,
    [IsShariahCompliant] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    CONSTRAINT [PK_TBL_ASSET_CLASS] PRIMARY KEY ([AssetClassId])
);
GO

CREATE TABLE [TBL_BANK] (
    [BankId] int NOT NULL IDENTITY,
    [BankName] nvarchar(100) NOT NULL,
    [BankCode] nvarchar(10) NOT NULL,
    [LogoUrl] nvarchar(200) NULL,
    [IsActive] bit NOT NULL DEFAULT (((1))),
    [CreatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    CONSTRAINT [PK_TBL_BANK] PRIMARY KEY ([BankId])
);
GO

CREATE TABLE [TBL_CHART_OF_ACCOUNT] (
    [Id] int NOT NULL IDENTITY,
    [AccountCode] nvarchar(20) NOT NULL,
    [AccountName] nvarchar(100) NOT NULL,
    [AccountType] nvarchar(50) NOT NULL,
    [IsLeaf] bit NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    CONSTRAINT [PK_TBL_CHART_OF_ACCOUNT] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [TBL_CURRENCY] (
    [Id] int NOT NULL IDENTITY,
    [Code] nvarchar(10) NOT NULL,
    [Name] nvarchar(50) NOT NULL,
    [Symbol] nvarchar(10) NOT NULL,
    [IsActive] bit NOT NULL DEFAULT (((1))),
    CONSTRAINT [PK_TBL_CURRENCY] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [TBL_GENDER] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(50) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [IsActive] bit NOT NULL DEFAULT (((1))),
    CONSTRAINT [PK_TBL_GENDER] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [TBL_INSTRUMENT] (
    [InstrumentId] int NOT NULL IDENTITY,
    [Name] nvarchar(300) NOT NULL,
    [Code] nvarchar(100) NULL,
    [IsShariahCompliant] bit NOT NULL,
    [IsActive] bit NOT NULL DEFAULT (((1))),
    [CreatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    CONSTRAINT [PK_TBL_INSTRUMENT] PRIMARY KEY ([InstrumentId])
);
GO

CREATE TABLE [TBL_JOURNAL_ENTRY] (
    [Id] bigint NOT NULL IDENTITY,
    [Reference] nvarchar(100) NOT NULL,
    [Narration] nvarchar(500) NULL,
    [TransactionDate] datetime2 NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    [CreatedByUserId] bigint NULL,
    CONSTRAINT [PK_TBL_JOURNAL_ENTRY] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [TBL_LEDGER] (
    [Id] bigint NOT NULL IDENTITY,
    [TransactionId] bigint NOT NULL,
    [WalletId] bigint NOT NULL,
    [EntryType] int NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [BalanceAfter] decimal(18,2) NOT NULL,
    [Narration] nvarchar(255) NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    CONSTRAINT [PK_TBL_LEDGER] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [TBL_MODULE] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Icon] nvarchar(50) NOT NULL,
    [Description] nvarchar(250) NULL,
    CONSTRAINT [PK_TBL_MODULE] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [TBL_PORTFOLIO_TYPE] (
    [PortfolioTypeId] int NOT NULL IDENTITY,
    [Name] nvarchar(200) NOT NULL,
    [Code] nvarchar(100) NOT NULL,
    [SupportsAllocation] bit NOT NULL DEFAULT (((0))),
    [CreatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    CONSTRAINT [PK_TBL_PORTFOLIO_TYPE] PRIMARY KEY ([PortfolioTypeId])
);
GO

CREATE TABLE [TBL_RELATIONSHIP_TYPE] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(50) NOT NULL,
    [IsActive] bit NOT NULL DEFAULT (((1))),
    CONSTRAINT [PK_TBL_RELATIONSHIP_TYPE] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [TBL_ROLE] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_TBL_ROLE] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [TBL_SYSTEM_SETTING] (
    [Id] int NOT NULL IDENTITY,
    [SessionTimeoutInMinutes] int NOT NULL,
    [WithdrawalAutoThreshold] decimal(18,2) NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_TBL_SYSTEM_SETTING] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [TBL_TRANSACTION_TYPE] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_TBL_TRANSACTION_TYPE] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [TBL_USER] (
    [UserId] bigint NOT NULL IDENTITY,
    [UserName] nvarchar(100) NOT NULL,
    [Email] nvarchar(150) NOT NULL,
    [PhoneNumber] nvarchar(20) NOT NULL,
    [FirstName] nvarchar(100) NULL,
    [LastName] nvarchar(100) NULL,
    [PasswordHash] nvarchar(500) NOT NULL,
    [Salt] nvarchar(200) NOT NULL,
    [IsActive] bit NOT NULL DEFAULT (((1))),
    [IsLocked] bit NOT NULL,
    [FailedLoginAttempts] int NOT NULL,
    [LastLoginDate] datetime2 NULL,
    [LastLogoutDate] datetime2 NULL,
    [LastPasswordChangeDate] datetime2 NULL,
    [IsSystemUser] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    [CreatedBy] bigint NULL,
    [ModifiedAt] datetime2 NULL,
    [ModifiedBy] bigint NULL,
    [IsDeleted] bit NOT NULL,
    [RowVersion] rowversion NOT NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TransactionPinHash] nvarchar(500) NULL,
    [TransactionPinSalt] nvarchar(200) NULL,
    [IsPinSet] bit NOT NULL,
    [PinFailedAttempts] int NOT NULL,
    [IsPinLocked] bit NOT NULL,
    [LastPinChangeDate] datetime2 NULL,
    [Is2faEnabled] bit NULL,
    [VerificationCode] nvarchar(10) NULL,
    [VerificationExpiry] datetime2 NULL,
    [RefreshToken] nvarchar(500) NULL,
    [RefreshTokenExpiry] datetime2 NULL,
    CONSTRAINT [PK__TBL_USER__1788CC4C596A566F] PRIMARY KEY ([UserId])
);
GO

CREATE TABLE [TBL_INSTRUMENT_PRICE] (
    [Id] int NOT NULL IDENTITY,
    [InstrumentId] int NOT NULL,
    [PriceDate] datetime2 NOT NULL,
    [NAV] decimal(18,6) NOT NULL,
    [PriceSource] nvarchar(50) NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    CONSTRAINT [PK_TBL_INSTRUMENT_PRICE] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_INSTRUMENT_PRICE_TBL_INSTRUMENT_InstrumentId] FOREIGN KEY ([InstrumentId]) REFERENCES [TBL_INSTRUMENT] ([InstrumentId]) ON DELETE CASCADE
);
GO

CREATE TABLE [TBL_JOURNAL_LINE] (
    [Id] bigint NOT NULL IDENTITY,
    [JournalEntryId] bigint NOT NULL,
    [AccountId] int NOT NULL,
    [Debit] decimal(18,2) NOT NULL,
    [Credit] decimal(18,2) NOT NULL,
    [Narration] nvarchar(500) NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    CONSTRAINT [PK_TBL_JOURNAL_LINE] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_JOURNAL_LINE_TBL_CHART_OF_ACCOUNT_AccountId] FOREIGN KEY ([AccountId]) REFERENCES [TBL_CHART_OF_ACCOUNT] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_TBL_JOURNAL_LINE_TBL_JOURNAL_ENTRY_JournalEntryId] FOREIGN KEY ([JournalEntryId]) REFERENCES [TBL_JOURNAL_ENTRY] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [TBL_ACCESS_RIGHT] (
    [Id] int NOT NULL IDENTITY,
    [ModuleId] int NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [Label] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_TBL_ACCESS_RIGHT] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_ACCESS_RIGHT_TBL_MODULE_ModuleId] FOREIGN KEY ([ModuleId]) REFERENCES [TBL_MODULE] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [TBL_PORTFOLIO] (
    [PortfolioId] int NOT NULL IDENTITY,
    [Name] nvarchar(300) NOT NULL,
    [Code] nvarchar(100) NOT NULL,
    [PortfolioTypeId] int NOT NULL,
    [RiskLevel] nvarchar(100) NULL,
    [Description] nvarchar(max) NULL,
    [ExpectedAnnualReturn] decimal(18,4) NULL,
    [IsActive] bit NOT NULL DEFAULT (((1))),
    [LockInPeriodDays] int NOT NULL,
    [MinHoldingPeriodDays] int NOT NULL,
    [ExitFeePercentage] decimal(18,4) NOT NULL,
    [NoticePeriodDays] int NOT NULL,
    [ApprovalThresholdAmount] decimal(18,2) NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    CONSTRAINT [PK_TBL_PORTFOLIO] PRIMARY KEY ([PortfolioId]),
    CONSTRAINT [FK_TBL_PORTFOLIO_TBL_PORTFOLIO_TYPE_PortfolioTypeId] FOREIGN KEY ([PortfolioTypeId]) REFERENCES [TBL_PORTFOLIO_TYPE] ([PortfolioTypeId]) ON DELETE CASCADE
);
GO

CREATE TABLE [TBL_CUSTOMER] (
    [CustomerId] bigint NOT NULL IDENTITY,
    [UserId] bigint NOT NULL,
    [FirstName] nvarchar(100) NOT NULL,
    [LastName] nvarchar(100) NOT NULL,
    [OtherNames] nvarchar(150) NULL,
    [DateOfBirth] date NOT NULL,
    [Gender] int NULL,
    [BVN] nvarchar(11) NULL,
    [BVNVerified] bit NOT NULL,
    [BVNVerifiedAt] datetime2 NULL,
    [NIN] nvarchar(11) NULL,
    [NINVerified] bit NOT NULL,
    [NINVerifiedAt] datetime2 NULL,
    [Address] nvarchar(250) NULL,
    [City] nvarchar(100) NULL,
    [State] nvarchar(100) NULL,
    [Country] int NULL DEFAULT (('Nigeria')),
    [KYCStatus] int NOT NULL,
    [KYCVerifiedAt] datetime2 NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    [CreatedBy] bigint NULL,
    [ModifiedAt] datetime2 NULL,
    [ModifiedBy] bigint NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL,
    [IsPolitcallyExposed] bit NULL,
    [RowVersion] rowversion NOT NULL,
    CONSTRAINT [PK__TBL_CUST__A4AE64D8B9B6FDD4] PRIMARY KEY ([CustomerId]),
    CONSTRAINT [FK_TBL_CUSTOMER_USER] FOREIGN KEY ([UserId]) REFERENCES [TBL_USER] ([UserId])
);
GO

CREATE TABLE [TBL_PASSWORD_RESET] (
    [Id] bigint NOT NULL IDENTITY,
    [UserId] bigint NOT NULL,
    [ResetCode] nvarchar(100) NOT NULL,
    [ExpiresAt] datetime2 NOT NULL,
    [IsUsed] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    CONSTRAINT [PK_TBL_PASSWORD_RESET] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PASSWORD_RESET_USER] FOREIGN KEY ([UserId]) REFERENCES [TBL_USER] ([UserId])
);
GO

CREATE TABLE [TBL_PAYMENT] (
    [Id] bigint NOT NULL IDENTITY,
    [UserId] bigint NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [PaymentProvider] int NOT NULL,
    [ProviderReference] nvarchar(100) NULL,
    [Status] nvarchar(20) NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    CONSTRAINT [PK_TBL_PAYMENT] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PAYMENT_USER] FOREIGN KEY ([UserId]) REFERENCES [TBL_USER] ([UserId])
);
GO

CREATE TABLE [TBL_PIN_RESET] (
    [Id] bigint NOT NULL IDENTITY,
    [UserId] bigint NOT NULL,
    [ResetCode] nvarchar(100) NOT NULL,
    [ExpiresAt] datetime2 NOT NULL,
    [IsUsed] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_TBL_PIN_RESET] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_PIN_RESET_TBL_USER_UserId] FOREIGN KEY ([UserId]) REFERENCES [TBL_USER] ([UserId]) ON DELETE CASCADE
);
GO

CREATE TABLE [TBL_RESERVED_ACCOUNT] (
    [Id] int NOT NULL IDENTITY,
    [UserId] bigint NOT NULL,
    [AccountReference] nvarchar(max) NOT NULL,
    [AccountNumber] nvarchar(max) NOT NULL,
    [BankName] nvarchar(max) NOT NULL,
    [BankCode] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_TBL_RESERVED_ACCOUNT] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_RESERVED_ACCOUNT_TBL_USER_UserId] FOREIGN KEY ([UserId]) REFERENCES [TBL_USER] ([UserId]) ON DELETE CASCADE
);
GO

CREATE TABLE [TBL_USER_ROLE] (
    [Id] bigint NOT NULL IDENTITY,
    [UserId] bigint NOT NULL,
    [RoleId] int NOT NULL,
    CONSTRAINT [PK_TBL_USER_ROLE] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_USER_ROLE_ROLE] FOREIGN KEY ([RoleId]) REFERENCES [TBL_ROLE] ([Id]),
    CONSTRAINT [FK_USER_ROLE_USER] FOREIGN KEY ([UserId]) REFERENCES [TBL_USER] ([UserId])
);
GO

CREATE TABLE [TBL_USER_SESSION] (
    [SessionId] bigint NOT NULL IDENTITY,
    [UserId] bigint NOT NULL,
    [RefreshToken] nvarchar(500) NOT NULL,
    [RefreshTokenExpiry] datetime2 NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUsedAt] datetime2 NOT NULL,
    [DeviceName] nvarchar(200) NULL,
    [UserAgent] nvarchar(500) NULL,
    [IpAddress] nvarchar(50) NULL,
    [IsRevoked] bit NOT NULL,
    CONSTRAINT [PK_TBL_USER_SESSION] PRIMARY KEY ([SessionId]),
    CONSTRAINT [FK_TBL_USER_SESSION_TBL_USER_UserId] FOREIGN KEY ([UserId]) REFERENCES [TBL_USER] ([UserId]) ON DELETE CASCADE
);
GO

CREATE TABLE [TBL_ROLE_ACCESS_RIGHT] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] int NOT NULL,
    [AccessRightId] int NOT NULL,
    CONSTRAINT [PK_TBL_ROLE_ACCESS_RIGHT] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_ROLE_ACCESS_RIGHT_TBL_ACCESS_RIGHT_AccessRightId] FOREIGN KEY ([AccessRightId]) REFERENCES [TBL_ACCESS_RIGHT] ([Id]),
    CONSTRAINT [FK_TBL_ROLE_ACCESS_RIGHT_TBL_ROLE_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [TBL_ROLE] ([Id])
);
GO

CREATE TABLE [TBL_PORTFOLIO_ALLOCATION_RULE] (
    [Id] int NOT NULL IDENTITY,
    [PortfolioId] int NOT NULL,
    [AssetClassId] int NOT NULL,
    [TargetPercentage] decimal(18,4) NOT NULL,
    [MinPercentage] decimal(18,4) NOT NULL,
    [MaxPercentage] decimal(18,4) NOT NULL,
    [ExpectedReturn] decimal(18,4) NOT NULL,
    CONSTRAINT [PK_TBL_PORTFOLIO_ALLOCATION_RULE] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_PORTFOLIO_ALLOCATION_RULE_TBL_ASSET_CLASS_AssetClassId] FOREIGN KEY ([AssetClassId]) REFERENCES [TBL_ASSET_CLASS] ([AssetClassId]) ON DELETE CASCADE,
    CONSTRAINT [FK_TBL_PORTFOLIO_ALLOCATION_RULE_TBL_PORTFOLIO_PortfolioId] FOREIGN KEY ([PortfolioId]) REFERENCES [TBL_PORTFOLIO] ([PortfolioId]) ON DELETE CASCADE
);
GO

CREATE TABLE [TBL_PORTFOLIO_INSTRUMENT] (
    [Id] int NOT NULL IDENTITY,
    [PortfolioId] int NOT NULL,
    [InstrumentId] int NOT NULL,
    [AssetClassId] int NOT NULL,
    [TargetWeight] decimal(5,2) NOT NULL,
    CONSTRAINT [PK_TBL_PORTFOLIO_INSTRUMENT] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_PORTFOLIO_INSTRUMENT_TBL_ASSET_CLASS_AssetClassId] FOREIGN KEY ([AssetClassId]) REFERENCES [TBL_ASSET_CLASS] ([AssetClassId]) ON DELETE CASCADE,
    CONSTRAINT [FK_TBL_PORTFOLIO_INSTRUMENT_TBL_INSTRUMENT_InstrumentId] FOREIGN KEY ([InstrumentId]) REFERENCES [TBL_INSTRUMENT] ([InstrumentId]) ON DELETE CASCADE,
    CONSTRAINT [FK_TBL_PORTFOLIO_INSTRUMENT_TBL_PORTFOLIO_PortfolioId] FOREIGN KEY ([PortfolioId]) REFERENCES [TBL_PORTFOLIO] ([PortfolioId]) ON DELETE CASCADE
);
GO

CREATE TABLE [TBL_PORTFOLIO_PRICE] (
    [Id] bigint NOT NULL IDENTITY,
    [PortfolioId] int NOT NULL,
    [PriceDate] datetime2 NOT NULL,
    [NAV] decimal(18,6) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_TBL_PORTFOLIO_PRICE] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_PORTFOLIO_PRICE_TBL_PORTFOLIO_PortfolioId] FOREIGN KEY ([PortfolioId]) REFERENCES [TBL_PORTFOLIO] ([PortfolioId]) ON DELETE CASCADE
);
GO

CREATE TABLE [TBL_CUSTOMER_ADDRESS_VERIFICATION] (
    [Id] bigint NOT NULL IDENTITY,
    [CustomerId] bigint NOT NULL,
    [DocTypeId] int NOT NULL,
    [DocumentUrl] nvarchar(500) NULL,
    [CloudinaryPublicId] nvarchar(200) NULL,
    [ExtractedAddress] nvarchar(250) NULL,
    [ExtractedCity] nvarchar(100) NULL,
    [ExtractedState] nvarchar(100) NULL,
    [ExtractedFullText] nvarchar(MAX) NULL,
    [ConfidenceScore] decimal(18,2) NULL,
    [Status] nvarchar(20) NULL DEFAULT (('Pending')),
    [AdminNotes] nvarchar(MAX) NULL,
    [CreatedAt] datetime2 NULL DEFAULT ((getdate())),
    [ReviewedAt] datetime2 NULL,
    [ReviewedBy] bigint NULL,
    CONSTRAINT [PK_TBL_CUSTOMER_ADDRESS_VERIFICATION] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ADDR_VERIF_CUSTOMER] FOREIGN KEY ([CustomerId]) REFERENCES [TBL_CUSTOMER] ([CustomerId]),
    CONSTRAINT [FK_ADDR_VERIF_DOCTYPE] FOREIGN KEY ([DocTypeId]) REFERENCES [TBL_ADDRESS_DOC_TYPE] ([Id])
);
GO

CREATE TABLE [TBL_CUSTOMER_BANK] (
    [CustomerBankId] bigint NOT NULL IDENTITY,
    [CustomerId] bigint NOT NULL,
    [BankId] int NOT NULL,
    [AccountNumber] nvarchar(20) NOT NULL,
    [AccountName] nvarchar(100) NOT NULL,
    [IsDefault] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_TBL_CUSTOMER_BANK] PRIMARY KEY ([CustomerBankId]),
    CONSTRAINT [FK_CUSTOMER_BANK_BANK] FOREIGN KEY ([BankId]) REFERENCES [TBL_BANK] ([BankId]),
    CONSTRAINT [FK_CUSTOMER_BANK_CUSTOMER] FOREIGN KEY ([CustomerId]) REFERENCES [TBL_CUSTOMER] ([CustomerId])
);
GO

CREATE TABLE [TBL_CUSTOMER_HOLDING] (
    [Id] bigint NOT NULL IDENTITY,
    [CustomerId] bigint NOT NULL,
    [InstrumentId] int NOT NULL,
    [Units] decimal(18,4) NOT NULL,
    [InvestedAmount] decimal(18,4) NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    CONSTRAINT [PK_TBL_CUSTOMER_HOLDING] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_CUSTOMER_HOLDING_TBL_CUSTOMER_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [TBL_CUSTOMER] ([CustomerId]) ON DELETE CASCADE,
    CONSTRAINT [FK_TBL_CUSTOMER_HOLDING_TBL_INSTRUMENT_InstrumentId] FOREIGN KEY ([InstrumentId]) REFERENCES [TBL_INSTRUMENT] ([InstrumentId]) ON DELETE CASCADE
);
GO

CREATE TABLE [TBL_CUSTOMER_PORTFOLIO] (
    [Id] bigint NOT NULL IDENTITY,
    [CustomerId] bigint NOT NULL,
    [PortfolioId] int NOT NULL,
    [TotalInvested] decimal(18,4) NOT NULL,
    [Units] decimal(18,6) NOT NULL,
    [InvestedAmount] decimal(18,4) NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    CONSTRAINT [PK_TBL_CUSTOMER_PORTFOLIO] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_CUSTOMER_PORTFOLIO_TBL_CUSTOMER_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [TBL_CUSTOMER] ([CustomerId]) ON DELETE CASCADE,
    CONSTRAINT [FK_TBL_CUSTOMER_PORTFOLIO_TBL_PORTFOLIO_PortfolioId] FOREIGN KEY ([PortfolioId]) REFERENCES [TBL_PORTFOLIO] ([PortfolioId]) ON DELETE CASCADE
);
GO

CREATE TABLE [TBL_INVESTMENT_TRANSACTION] (
    [Id] bigint NOT NULL IDENTITY,
    [CustomerId] bigint NOT NULL,
    [PortfolioId] int NOT NULL,
    [InstrumentId] int NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Units] decimal(18,6) NOT NULL,
    [NAV] decimal(18,6) NOT NULL,
    [TransactionType] nvarchar(20) NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    CONSTRAINT [PK_TBL_INVESTMENT_TRANSACTION] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_INVESTMENT_TRANSACTION_TBL_CUSTOMER_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [TBL_CUSTOMER] ([CustomerId]) ON DELETE CASCADE,
    CONSTRAINT [FK_TBL_INVESTMENT_TRANSACTION_TBL_INSTRUMENT_InstrumentId] FOREIGN KEY ([InstrumentId]) REFERENCES [TBL_INSTRUMENT] ([InstrumentId]) ON DELETE CASCADE,
    CONSTRAINT [FK_TBL_INVESTMENT_TRANSACTION_TBL_PORTFOLIO_PortfolioId] FOREIGN KEY ([PortfolioId]) REFERENCES [TBL_PORTFOLIO] ([PortfolioId]) ON DELETE CASCADE
);
GO

CREATE TABLE [TBL_KYC_LOG] (
    [Id] bigint NOT NULL IDENTITY,
    [CustomerId] bigint NOT NULL,
    [Type] nvarchar(10) NULL,
    [Status] nvarchar(20) NULL,
    [Response] nvarchar(max) NULL,
    [CreatedAt] datetime2 NULL DEFAULT ((getdate())),
    CONSTRAINT [PK_TBL_KYC_LOG] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_KYC_CUSTOMER] FOREIGN KEY ([CustomerId]) REFERENCES [TBL_CUSTOMER] ([CustomerId])
);
GO

CREATE TABLE [TBL_LIQUIDATION_REQUEST] (
    [Id] bigint NOT NULL IDENTITY,
    [CustomerId] bigint NOT NULL,
    [PortfolioId] int NOT NULL,
    [UnitsRequested] decimal(18,6) NOT NULL,
    [GrossAmount] decimal(18,2) NOT NULL,
    [ExitFeeApplied] decimal(18,2) NOT NULL,
    [NetPayableAmount] decimal(18,2) NOT NULL,
    [Status] int NOT NULL DEFAULT (((1))),
    [ExpectedReleaseDate] datetime2 NULL,
    [AdminNotes] nvarchar(max) NULL,
    [ReviewedByAdminId] bigint NULL,
    [ReviewedAt] datetime2 NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_TBL_LIQUIDATION_REQUEST] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_LIQUIDATION_REQUEST_TBL_CUSTOMER_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [TBL_CUSTOMER] ([CustomerId]) ON DELETE CASCADE,
    CONSTRAINT [FK_TBL_LIQUIDATION_REQUEST_TBL_PORTFOLIO_PortfolioId] FOREIGN KEY ([PortfolioId]) REFERENCES [TBL_PORTFOLIO] ([PortfolioId]) ON DELETE CASCADE,
    CONSTRAINT [FK_TBL_LIQUIDATION_REQUEST_TBL_USER_ReviewedByAdminId] FOREIGN KEY ([ReviewedByAdminId]) REFERENCES [TBL_USER] ([UserId])
);
GO

CREATE TABLE [TBL_NEXT_OF_KIN] (
    [Id] bigint NOT NULL IDENTITY,
    [CustomerId] bigint NOT NULL,
    [FullName] nvarchar(150) NOT NULL,
    [RelationshipTypeId] int NOT NULL,
    [PhoneNumber] nvarchar(20) NOT NULL,
    [Email] nvarchar(150) NULL,
    [Address] nvarchar(250) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [ModifiedAt] datetime2 NULL,
    CONSTRAINT [PK_TBL_NEXT_OF_KIN] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_NEXT_OF_KIN_CUSTOMER] FOREIGN KEY ([CustomerId]) REFERENCES [TBL_CUSTOMER] ([CustomerId]),
    CONSTRAINT [FK_NEXT_OF_KIN_RELATIONSHIP] FOREIGN KEY ([RelationshipTypeId]) REFERENCES [TBL_RELATIONSHIP_TYPE] ([Id])
);
GO

CREATE TABLE [TBL_WALLET] (
    [WalletId] bigint NOT NULL IDENTITY,
    [CustomerId] bigint NOT NULL,
    [WalletNumber] nvarchar(10) NOT NULL,
    [Currency] int NOT NULL,
    [Balance] decimal(18,2) NOT NULL,
    [IsActive] bit NOT NULL DEFAULT (((1))),
    [CreatedAt] datetime2 NULL DEFAULT ((getdate())),
    [RowVersion] rowversion NOT NULL,
    CONSTRAINT [PK__TBL_WALL__84D4F90E05132222] PRIMARY KEY ([WalletId]),
    CONSTRAINT [FK_WALLET_CUSTOMER] FOREIGN KEY ([CustomerId]) REFERENCES [TBL_CUSTOMER] ([CustomerId])
);
GO

CREATE TABLE [TBL_WITHDRAWAL_REQUEST] (
    [Id] bigint NOT NULL IDENTITY,
    [CustomerId] bigint NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [Narration] nvarchar(250) NULL,
    [Reference] nvarchar(100) NOT NULL,
    [BankCode] nvarchar(20) NULL,
    [AccountNumber] nvarchar(50) NULL,
    [InitiatedAt] datetime2 NOT NULL,
    [ReviewedAt] datetime2 NULL,
    [ReviewedByAdminId] bigint NULL,
    [AdminNotes] nvarchar(500) NULL,
    CONSTRAINT [PK_TBL_WITHDRAWAL_REQUEST] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_WITHDRAWAL_REQUEST_TBL_CUSTOMER_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [TBL_CUSTOMER] ([CustomerId]) ON DELETE CASCADE,
    CONSTRAINT [FK_TBL_WITHDRAWAL_REQUEST_TBL_USER_ReviewedByAdminId] FOREIGN KEY ([ReviewedByAdminId]) REFERENCES [TBL_USER] ([UserId])
);
GO

CREATE TABLE [TBL_TRANSACTION] (
    [TransactionId] bigint NOT NULL IDENTITY,
    [Reference] nvarchar(100) NOT NULL,
    [TransactionType] int NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Status] int NULL DEFAULT (((0))),
    [Narration] nvarchar(255) NULL,
    [PaymentId] bigint NULL,
    [IsReversed] bit NOT NULL,
    [ReversedTransactionId] bigint NULL,
    [InitiatedByUserId] bigint NOT NULL,
    [ApprovedByUserId] bigint NULL,
    [ApprovedAt] datetime2 NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK__TBL_TRAN__55433A6BBD1D0E19] PRIMARY KEY ([TransactionId]),
    CONSTRAINT [FK_TXN_PAYMENT] FOREIGN KEY ([PaymentId]) REFERENCES [TBL_PAYMENT] ([Id]),
    CONSTRAINT [FK_TXN_REVERSAL] FOREIGN KEY ([ReversedTransactionId]) REFERENCES [TBL_TRANSACTION] ([TransactionId]),
    CONSTRAINT [FK_TXN_USER_APPROVE] FOREIGN KEY ([ApprovedByUserId]) REFERENCES [TBL_USER] ([UserId]),
    CONSTRAINT [FK_TXN_USER_INIT] FOREIGN KEY ([InitiatedByUserId]) REFERENCES [TBL_USER] ([UserId])
);
GO

CREATE INDEX [IX_TBL_ACCESS_RIGHT_ModuleId] ON [TBL_ACCESS_RIGHT] ([ModuleId]);
GO

CREATE UNIQUE INDEX [IX_TBL_CHART_OF_ACCOUNT_AccountCode] ON [TBL_CHART_OF_ACCOUNT] ([AccountCode]);
GO

CREATE UNIQUE INDEX [IX_TBL_CUSTOMER_BVN] ON [TBL_CUSTOMER] ([BVN]) WHERE ([BVN] IS NOT NULL);
GO

CREATE UNIQUE INDEX [IX_TBL_CUSTOMER_NIN] ON [TBL_CUSTOMER] ([NIN]) WHERE ([NIN] IS NOT NULL);
GO

CREATE UNIQUE INDEX [IX_TBL_CUSTOMER_UserId] ON [TBL_CUSTOMER] ([UserId]);
GO

CREATE INDEX [IX_TBL_CUSTOMER_ADDRESS_VERIFICATION_CustomerId] ON [TBL_CUSTOMER_ADDRESS_VERIFICATION] ([CustomerId]);
GO

CREATE INDEX [IX_TBL_CUSTOMER_ADDRESS_VERIFICATION_DocTypeId] ON [TBL_CUSTOMER_ADDRESS_VERIFICATION] ([DocTypeId]);
GO

CREATE INDEX [IX_TBL_CUSTOMER_BANK_BankId] ON [TBL_CUSTOMER_BANK] ([BankId]);
GO

CREATE INDEX [IX_TBL_CUSTOMER_BANK_CustomerId] ON [TBL_CUSTOMER_BANK] ([CustomerId]);
GO

CREATE INDEX [IX_TBL_CUSTOMER_HOLDING_CustomerId] ON [TBL_CUSTOMER_HOLDING] ([CustomerId]);
GO

CREATE INDEX [IX_TBL_CUSTOMER_HOLDING_InstrumentId] ON [TBL_CUSTOMER_HOLDING] ([InstrumentId]);
GO

CREATE INDEX [IX_TBL_CUSTOMER_PORTFOLIO_CustomerId] ON [TBL_CUSTOMER_PORTFOLIO] ([CustomerId]);
GO

CREATE INDEX [IX_TBL_CUSTOMER_PORTFOLIO_PortfolioId] ON [TBL_CUSTOMER_PORTFOLIO] ([PortfolioId]);
GO

CREATE INDEX [IX_TBL_INSTRUMENT_PRICE_InstrumentId] ON [TBL_INSTRUMENT_PRICE] ([InstrumentId]);
GO

CREATE INDEX [IX_TBL_INVESTMENT_TRANSACTION_CustomerId] ON [TBL_INVESTMENT_TRANSACTION] ([CustomerId]);
GO

CREATE INDEX [IX_TBL_INVESTMENT_TRANSACTION_InstrumentId] ON [TBL_INVESTMENT_TRANSACTION] ([InstrumentId]);
GO

CREATE INDEX [IX_TBL_INVESTMENT_TRANSACTION_PortfolioId] ON [TBL_INVESTMENT_TRANSACTION] ([PortfolioId]);
GO

CREATE INDEX [IX_TBL_JOURNAL_LINE_AccountId] ON [TBL_JOURNAL_LINE] ([AccountId]);
GO

CREATE INDEX [IX_TBL_JOURNAL_LINE_JournalEntryId] ON [TBL_JOURNAL_LINE] ([JournalEntryId]);
GO

CREATE INDEX [IX_TBL_KYC_LOG_CustomerId] ON [TBL_KYC_LOG] ([CustomerId]);
GO

CREATE INDEX [IX_TBL_LIQUIDATION_REQUEST_CustomerId] ON [TBL_LIQUIDATION_REQUEST] ([CustomerId]);
GO

CREATE INDEX [IX_TBL_LIQUIDATION_REQUEST_PortfolioId] ON [TBL_LIQUIDATION_REQUEST] ([PortfolioId]);
GO

CREATE INDEX [IX_TBL_LIQUIDATION_REQUEST_ReviewedByAdminId] ON [TBL_LIQUIDATION_REQUEST] ([ReviewedByAdminId]);
GO

CREATE INDEX [IX_TBL_NEXT_OF_KIN_CustomerId] ON [TBL_NEXT_OF_KIN] ([CustomerId]);
GO

CREATE INDEX [IX_TBL_NEXT_OF_KIN_RelationshipTypeId] ON [TBL_NEXT_OF_KIN] ([RelationshipTypeId]);
GO

CREATE INDEX [IX_TBL_PASSWORD_RESET_UserId] ON [TBL_PASSWORD_RESET] ([UserId]);
GO

CREATE INDEX [IX_TBL_PAYMENT_UserId] ON [TBL_PAYMENT] ([UserId]);
GO

CREATE INDEX [IX_TBL_PIN_RESET_UserId] ON [TBL_PIN_RESET] ([UserId]);
GO

CREATE INDEX [IX_TBL_PORTFOLIO_PortfolioTypeId] ON [TBL_PORTFOLIO] ([PortfolioTypeId]);
GO

CREATE INDEX [IX_TBL_PORTFOLIO_ALLOCATION_RULE_AssetClassId] ON [TBL_PORTFOLIO_ALLOCATION_RULE] ([AssetClassId]);
GO

CREATE INDEX [IX_TBL_PORTFOLIO_ALLOCATION_RULE_PortfolioId] ON [TBL_PORTFOLIO_ALLOCATION_RULE] ([PortfolioId]);
GO

CREATE INDEX [IX_TBL_PORTFOLIO_INSTRUMENT_AssetClassId] ON [TBL_PORTFOLIO_INSTRUMENT] ([AssetClassId]);
GO

CREATE INDEX [IX_TBL_PORTFOLIO_INSTRUMENT_InstrumentId] ON [TBL_PORTFOLIO_INSTRUMENT] ([InstrumentId]);
GO

CREATE INDEX [IX_TBL_PORTFOLIO_INSTRUMENT_PortfolioId] ON [TBL_PORTFOLIO_INSTRUMENT] ([PortfolioId]);
GO

CREATE INDEX [IX_TBL_PORTFOLIO_PRICE_PortfolioId] ON [TBL_PORTFOLIO_PRICE] ([PortfolioId]);
GO

CREATE INDEX [IX_TBL_RESERVED_ACCOUNT_UserId] ON [TBL_RESERVED_ACCOUNT] ([UserId]);
GO

CREATE INDEX [IX_TBL_ROLE_ACCESS_RIGHT_AccessRightId] ON [TBL_ROLE_ACCESS_RIGHT] ([AccessRightId]);
GO

CREATE INDEX [IX_TBL_ROLE_ACCESS_RIGHT_RoleId] ON [TBL_ROLE_ACCESS_RIGHT] ([RoleId]);
GO

CREATE INDEX [IX_TBL_TRANSACTION_ApprovedByUserId] ON [TBL_TRANSACTION] ([ApprovedByUserId]);
GO

CREATE INDEX [IX_TBL_TRANSACTION_InitiatedByUserId] ON [TBL_TRANSACTION] ([InitiatedByUserId]);
GO

CREATE INDEX [IX_TBL_TRANSACTION_PaymentId] ON [TBL_TRANSACTION] ([PaymentId]);
GO

CREATE INDEX [IX_TBL_TRANSACTION_ReversedTransactionId] ON [TBL_TRANSACTION] ([ReversedTransactionId]);
GO

CREATE UNIQUE INDEX [IX_TBL_USER_Email] ON [TBL_USER] ([Email]);
GO

CREATE UNIQUE INDEX [IX_TBL_USER_PhoneNumber] ON [TBL_USER] ([PhoneNumber]);
GO

CREATE UNIQUE INDEX [IX_TBL_USER_UserName] ON [TBL_USER] ([UserName]);
GO

CREATE INDEX [IX_TBL_USER_ROLE_RoleId] ON [TBL_USER_ROLE] ([RoleId]);
GO

CREATE INDEX [IX_TBL_USER_ROLE_UserId] ON [TBL_USER_ROLE] ([UserId]);
GO

CREATE INDEX [IX_TBL_USER_SESSION_UserId] ON [TBL_USER_SESSION] ([UserId]);
GO

CREATE INDEX [IX_TBL_WALLET_CustomerId] ON [TBL_WALLET] ([CustomerId]);
GO

CREATE INDEX [IX_TBL_WITHDRAWAL_REQUEST_CustomerId] ON [TBL_WITHDRAWAL_REQUEST] ([CustomerId]);
GO

CREATE INDEX [IX_TBL_WITHDRAWAL_REQUEST_ReviewedByAdminId] ON [TBL_WITHDRAWAL_REQUEST] ([ReviewedByAdminId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260425074230_InitialCreate', N'6.0.15');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [TBL_COMPANY_PROFILE] (
    [Id] int NOT NULL IDENTITY,
    [CompanyName] nvarchar(250) NOT NULL,
    [Address] nvarchar(500) NOT NULL,
    [PhoneNumber] nvarchar(50) NOT NULL,
    [Email] nvarchar(150) NOT NULL,
    [RcNumber] nvarchar(100) NOT NULL,
    [DateOfIncorporation] datetime2 NULL,
    [BankName] nvarchar(150) NULL,
    [AccountNumber] nvarchar(50) NULL,
    [XLink] nvarchar(250) NULL,
    [FacebookLink] nvarchar(250) NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_TBL_COMPANY_PROFILE] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260525164416_AddCompanyProfileTable', N'6.0.15');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260525203339_AddManualFundingRequest', N'6.0.15');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[TBL_CUSTOMER]') AND [c].[name] = N'DateOfBirth');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [TBL_CUSTOMER] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [TBL_CUSTOMER] ALTER COLUMN [DateOfBirth] date NULL;
GO

ALTER TABLE [TBL_CUSTOMER] ADD [BusinessName] nvarchar(250) NULL;
GO

ALTER TABLE [TBL_CUSTOMER] ADD [CustomerTypeId] int NULL DEFAULT (((1)));
GO

ALTER TABLE [TBL_CUSTOMER] ADD [DateOfIncorporation] date NULL;
GO

ALTER TABLE [TBL_CUSTOMER] ADD [RegistrationNumber] nvarchar(50) NULL;
GO

ALTER TABLE [TBL_CUSTOMER] ADD [TaxIdentificationNumber] nvarchar(50) NULL;
GO

CREATE TABLE [TBL_CUSTOMER_TYPE] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(50) NOT NULL,
    [Description] nvarchar(200) NULL,
    [IsActive] bit NOT NULL DEFAULT (((1))),
    CONSTRAINT [PK_TBL_CUSTOMER_TYPE] PRIMARY KEY ([Id])
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[TBL_CUSTOMER_TYPE]'))
    SET IDENTITY_INSERT [TBL_CUSTOMER_TYPE] ON;
INSERT INTO [TBL_CUSTOMER_TYPE] ([Id], [Description], [IsActive], [Name])
VALUES (1, N'Individual Customer Account', CAST(1 AS bit), N'Individual');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[TBL_CUSTOMER_TYPE]'))
    SET IDENTITY_INSERT [TBL_CUSTOMER_TYPE] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[TBL_CUSTOMER_TYPE]'))
    SET IDENTITY_INSERT [TBL_CUSTOMER_TYPE] ON;
INSERT INTO [TBL_CUSTOMER_TYPE] ([Id], [Description], [IsActive], [Name])
VALUES (2, N'Corporate/Business Account', CAST(1 AS bit), N'Corporate');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[TBL_CUSTOMER_TYPE]'))
    SET IDENTITY_INSERT [TBL_CUSTOMER_TYPE] OFF;
GO

CREATE INDEX [IX_TBL_CUSTOMER_CustomerTypeId] ON [TBL_CUSTOMER] ([CustomerTypeId]);
GO

ALTER TABLE [TBL_CUSTOMER] ADD CONSTRAINT [FK_TBL_CUSTOMER_CUSTOMER_TYPE] FOREIGN KEY ([CustomerTypeId]) REFERENCES [TBL_CUSTOMER_TYPE] ([Id]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260529080257_AddCorporateCustomerFields', N'6.0.15');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260530122107_AddCorporateProfileAdditionalFields', N'6.0.15');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [TBL_CORPORATE_CONTACT] (
    [Id] bigint NOT NULL IDENTITY,
    [CustomerId] bigint NOT NULL,
    [FullName] nvarchar(150) NULL,
    [Email] nvarchar(150) NULL,
    [PhoneNumber] nvarchar(50) NULL,
    [IsPrimary] bit NOT NULL,
    CONSTRAINT [PK_TBL_CORPORATE_CONTACT] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CORPORATE_CONTACT_CUSTOMER] FOREIGN KEY ([CustomerId]) REFERENCES [TBL_CUSTOMER] ([CustomerId])
);
GO

CREATE INDEX [IX_TBL_CORPORATE_CONTACT_CustomerId] ON [TBL_CORPORATE_CONTACT] ([CustomerId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260530133547_AddCorporateContactTable', N'6.0.15');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [TBL_CORPORATE_DOCUMENT] (
    [DocumentId] bigint NOT NULL IDENTITY,
    [CustomerId] bigint NOT NULL,
    [DocumentType] nvarchar(100) NOT NULL,
    [FilePath] nvarchar(500) NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [UploadedAt] datetime2 NOT NULL,
    [ReviewedAt] datetime2 NULL,
    [ReviewedByAdminId] bigint NULL,
    [AdminNotes] nvarchar(1000) NULL,
    CONSTRAINT [PK_TBL_CORPORATE_DOCUMENT] PRIMARY KEY ([DocumentId]),
    CONSTRAINT [FK_TBL_CORPORATE_DOCUMENT_TBL_CUSTOMER_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [TBL_CUSTOMER] ([CustomerId]) ON DELETE CASCADE,
    CONSTRAINT [FK_TBL_CORPORATE_DOCUMENT_TBL_USER_ReviewedByAdminId] FOREIGN KEY ([ReviewedByAdminId]) REFERENCES [TBL_USER] ([UserId])
);
GO

CREATE INDEX [IX_TBL_CORPORATE_DOCUMENT_CustomerId] ON [TBL_CORPORATE_DOCUMENT] ([CustomerId]);
GO

CREATE INDEX [IX_TBL_CORPORATE_DOCUMENT_ReviewedByAdminId] ON [TBL_CORPORATE_DOCUMENT] ([ReviewedByAdminId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260530140746_AddCorporateDocumentTable', N'6.0.15');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [TBL_VERIFICATION_ITEM] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(250) NOT NULL,
    [Type] nvarchar(100) NOT NULL,
    [IsSystemVerified] bit NOT NULL,
    [SystemRule] nvarchar(100) NULL,
    [TargetEntityTypes] nvarchar(250) NULL,
    [Icon] nvarchar(100) NULL,
    [DisplayOrder] int NOT NULL,
    [IsActive] bit NOT NULL DEFAULT (((1))),
    CONSTRAINT [PK_TBL_VERIFICATION_ITEM] PRIMARY KEY ([Id])
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'DisplayOrder', N'Icon', N'IsActive', N'IsSystemVerified', N'Name', N'SystemRule', N'TargetEntityTypes', N'Type') AND [object_id] = OBJECT_ID(N'[TBL_VERIFICATION_ITEM]'))
    SET IDENTITY_INSERT [TBL_VERIFICATION_ITEM] ON;
INSERT INTO [TBL_VERIFICATION_ITEM] ([Id], [DisplayOrder], [Icon], [IsActive], [IsSystemVerified], [Name], [SystemRule], [TargetEntityTypes], [Type])
VALUES (1, 1, N'ri-file-list-3-line', CAST(1 AS bit), CAST(1 AS bit), N'1. Completed Application Form', N'CheckAppForm', N'Corporate', N'appForm'),
(2, 2, N'ri-verified-badge-line', CAST(1 AS bit), CAST(0 AS bit), N'2. Certificate of Incorporation', NULL, N'Corporate', N'incorporation'),
(3, 3, N'ri-user-line', CAST(1 AS bit), CAST(0 AS bit), N'3. Passport Photography of each Authorized Signatory', NULL, N'Corporate', N'passport'),
(4, 4, N'ri-book-read-line', CAST(1 AS bit), CAST(0 AS bit), N'4. Memorandum & Articles of Association', NULL, N'Corporate', N'memart'),
(5, 5, N'ri-pie-chart-line', CAST(1 AS bit), CAST(0 AS bit), N'5. Form CAC 2 (Return of Allotment of Shares)', NULL, N'Corporate', N'cac2'),
(6, 6, N'ri-folder-user-line', CAST(1 AS bit), CAST(0 AS bit), N'6. Form CAC 7 (Particulars of Directors)', NULL, N'Corporate', N'cac7'),
(7, 7, N'ri-map-pin-user-line', CAST(1 AS bit), CAST(0 AS bit), N'7. Form CAC 3 (Notice of Situation/Change of Registered Address)', NULL, N'Corporate', N'cac3'),
(8, 8, N'ri-shield-user-line', CAST(1 AS bit), CAST(0 AS bit), N'8. Copy of Identification of Authorized Signatories and Directors', NULL, N'Corporate', N'signatoryId'),
(9, 9, N'ri-team-line', CAST(1 AS bit), CAST(0 AS bit), N'9. Board Resolution/minutes of meeting confirming Authorized Signatories', NULL, N'Corporate', N'boardResolution'),
(10, 10, N'ri-bank-line', CAST(1 AS bit), CAST(1 AS bit), N'10. Link Settlement Bank Account', N'CheckBankLinked', N'Corporate', N'settlementLink');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'DisplayOrder', N'Icon', N'IsActive', N'IsSystemVerified', N'Name', N'SystemRule', N'TargetEntityTypes', N'Type') AND [object_id] = OBJECT_ID(N'[TBL_VERIFICATION_ITEM]'))
    SET IDENTITY_INSERT [TBL_VERIFICATION_ITEM] OFF;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260530141851_AddVerificationItemTable', N'6.0.15');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [TBL_CUSTOMER_BANK] ADD [BeneficiaryAccountName] nvarchar(250) NULL;
GO

ALTER TABLE [TBL_CUSTOMER_BANK] ADD [BeneficiaryAccountNumber] nvarchar(50) NULL;
GO

ALTER TABLE [TBL_CUSTOMER_BANK] ADD [BeneficiaryAddress] nvarchar(500) NULL;
GO

ALTER TABLE [TBL_CUSTOMER_BANK] ADD [CorrespondentBank] nvarchar(250) NULL;
GO

ALTER TABLE [TBL_CUSTOMER_BANK] ADD [CurrencyCode] nvarchar(3) NOT NULL DEFAULT (('NGN'));
GO

ALTER TABLE [TBL_CUSTOMER_BANK] ADD [FfcDetails] nvarchar(500) NULL;
GO

ALTER TABLE [TBL_CUSTOMER_BANK] ADD [Iban] nvarchar(50) NULL;
GO

ALTER TABLE [TBL_CUSTOMER_BANK] ADD [SortCode] nvarchar(50) NULL;
GO

ALTER TABLE [TBL_CUSTOMER_BANK] ADD [SwiftCode] nvarchar(50) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260530143537_AddDomiciliaryFieldsToCustomerBank', N'6.0.15');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [TBL_CORPORATE_SIGNATORY] (
    [SignatoryId] int NOT NULL IDENTITY,
    [CustomerId] bigint NOT NULL,
    [Title] nvarchar(50) NOT NULL,
    [Surname] nvarchar(100) NOT NULL,
    [FirstName] nvarchar(100) NOT NULL,
    [OtherNames] nvarchar(100) NULL,
    [Designation] nvarchar(100) NOT NULL,
    [DateOfBirth] date NOT NULL,
    [ResidentialAddress] nvarchar(500) NOT NULL,
    [BusinessEmail] nvarchar(200) NOT NULL,
    [PhoneNumber] nvarchar(20) NOT NULL,
    [Bvn] nvarchar(20) NOT NULL,
    [Nationality] nvarchar(100) NOT NULL,
    [Gender] nvarchar(20) NOT NULL,
    [SigningClass] nvarchar(50) NOT NULL,
    [IdentityType] nvarchar(50) NOT NULL,
    [IdNumber] nvarchar(100) NOT NULL,
    [IsPep] bit NOT NULL,
    [PassportPhotoUrl] nvarchar(500) NOT NULL,
    [SignatureCardUrl] nvarchar(500) NOT NULL,
    [IdentityDocumentUrl] nvarchar(500) NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    [UpdatedAt] datetime2 NULL,
    [DeletedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_TBL_CORPORATE_SIGNATORY] PRIMARY KEY ([SignatoryId]),
    CONSTRAINT [FK_CORPORATE_SIGNATORY_CUSTOMER] FOREIGN KEY ([CustomerId]) REFERENCES [TBL_CUSTOMER] ([CustomerId])
);
GO

CREATE INDEX [IX_TBL_CORPORATE_SIGNATORY_CustomerId] ON [TBL_CORPORATE_SIGNATORY] ([CustomerId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260530151016_AddCorporateSignatories', N'6.0.15');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [TBL_CORPORATE_DIRECTOR] (
    [DirectorId] int NOT NULL IDENTITY,
    [CustomerId] bigint NOT NULL,
    [Title] nvarchar(50) NOT NULL,
    [Surname] nvarchar(100) NOT NULL,
    [FirstName] nvarchar(100) NOT NULL,
    [OtherNames] nvarchar(100) NULL,
    [Designation] nvarchar(100) NOT NULL,
    [DateOfBirth] date NOT NULL,
    [ResidentialAddress] nvarchar(500) NOT NULL,
    [BusinessEmail] nvarchar(200) NOT NULL,
    [PhoneNumber] nvarchar(20) NOT NULL,
    [Bvn] nvarchar(20) NOT NULL,
    [Nationality] nvarchar(100) NOT NULL,
    [Gender] nvarchar(20) NOT NULL,
    [SigningClass] nvarchar(50) NOT NULL,
    [IdentityType] nvarchar(50) NOT NULL,
    [IdNumber] nvarchar(100) NOT NULL,
    [IsPep] bit NOT NULL,
    [PassportPhotoUrl] nvarchar(500) NOT NULL,
    [SignatureCardUrl] nvarchar(500) NOT NULL,
    [IdentityDocumentUrl] nvarchar(500) NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [DeletedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_TBL_CORPORATE_DIRECTOR] PRIMARY KEY ([DirectorId]),
    CONSTRAINT [FK_TBL_CORPORATE_DIRECTOR_TBL_CUSTOMER] FOREIGN KEY ([CustomerId]) REFERENCES [TBL_CUSTOMER] ([CustomerId])
);
GO

CREATE INDEX [IX_TBL_CORPORATE_DIRECTOR_CustomerId] ON [TBL_CORPORATE_DIRECTOR] ([CustomerId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260530152507_AddCorporateDirector', N'6.0.15');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

UPDATE [TBL_VERIFICATION_ITEM] SET [IsSystemVerified] = CAST(1 AS bit), [SystemRule] = N'CheckSignatoryPhotos'
WHERE [Id] = 3;
SELECT @@ROWCOUNT;

GO

UPDATE [TBL_VERIFICATION_ITEM] SET [IsSystemVerified] = CAST(1 AS bit), [SystemRule] = N'CheckDirectorsAdded'
WHERE [Id] = 6;
SELECT @@ROWCOUNT;

GO

UPDATE [TBL_VERIFICATION_ITEM] SET [IsSystemVerified] = CAST(1 AS bit), [SystemRule] = N'CheckSignatoryDirectorsId'
WHERE [Id] = 8;
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260531001031_UpdateCorporateVerificationSeeding3', N'6.0.15');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [TBL_NOTIFICATION] (
    [NotificationId] bigint NOT NULL IDENTITY,
    [CustomerId] bigint NOT NULL,
    [Title] nvarchar(100) NOT NULL,
    [Message] nvarchar(1000) NOT NULL,
    [IsRead] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    CONSTRAINT [PK_TBL_NOTIFICATION] PRIMARY KEY ([NotificationId]),
    CONSTRAINT [FK_TBL_NOTIFICATION_CUSTOMER] FOREIGN KEY ([CustomerId]) REFERENCES [TBL_CUSTOMER] ([CustomerId]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_TBL_NOTIFICATION_CustomerId] ON [TBL_NOTIFICATION] ([CustomerId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260531004138_AddNotificationSystem', N'6.0.15');
GO

COMMIT;
GO

