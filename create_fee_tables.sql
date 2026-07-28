-- ============================================================
-- Idempotent: Creates TBL_PORTFOLIO_FEE_CONFIG and
-- TBL_QUARTERLY_FEE_LOG if they do not already exist.
-- Run this once against your database.
-- ============================================================

-- 1. Portfolio Fee Configuration
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_NAME = 'TBL_PORTFOLIO_FEE_CONFIG'
)
BEGIN
    CREATE TABLE [TBL_PORTFOLIO_FEE_CONFIG] (
        [Id]                  INT              NOT NULL IDENTITY(1,1),
        [PortfolioId]         INT              NOT NULL,
        [FeeType]             NVARCHAR(50)     NOT NULL,   -- MANAGEMENT | CUSTODY | SEC_REGULATORY | PERFORMANCE | EXIT
        [PercentagePerAnnum]  DECIMAL(18, 4)   NOT NULL,   -- e.g. 1.50 for 1.5 %
        [CalculationBasis]    NVARCHAR(50)     NOT NULL DEFAULT 'AVERAGE_MONTH_END_NAV',
        [BillingFrequency]    NVARCHAR(30)     NOT NULL DEFAULT 'QUARTERLY',
        [ChargeDayOfMonth]    INT              NOT NULL DEFAULT 10,
        [TargetAccountCode]   NVARCHAR(20)     NOT NULL,   -- e.g. '4220', '4230', '2210'
        [IsLiability]         BIT              NOT NULL DEFAULT 0,
        [IsWaived]            BIT              NOT NULL DEFAULT 0,
        [IsActive]            BIT              NOT NULL DEFAULT 1,
        [CreatedAt]           DATETIME2        NOT NULL DEFAULT GETUTCDATE(),

        CONSTRAINT [PK_TBL_PORTFOLIO_FEE_CONFIG] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TBL_PORTFOLIO_FEE_CONFIG_Portfolio]
            FOREIGN KEY ([PortfolioId]) REFERENCES [TBL_PORTFOLIO] ([PortfolioId])
    );

    PRINT 'Created TBL_PORTFOLIO_FEE_CONFIG';
END
ELSE
BEGIN
    PRINT 'TBL_PORTFOLIO_FEE_CONFIG already exists — skipped.';
END
GO

-- 2. Quarterly Fee Execution Log
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_NAME = 'TBL_QUARTERLY_FEE_LOG'
)
BEGIN
    CREATE TABLE [TBL_QUARTERLY_FEE_LOG] (
        [Id]                   BIGINT           NOT NULL IDENTITY(1,1),
        [CustomerId]           BIGINT           NOT NULL,
        [PortfolioId]          INT              NOT NULL,
        [FeeConfigId]          INT              NOT NULL DEFAULT 0,
        [Year]                 INT              NOT NULL,
        [Quarter]              INT              NOT NULL,   -- 1, 2, 3, 4
        [Month1EndNav]         DECIMAL(18, 4)   NOT NULL DEFAULT 0,
        [Month2EndNav]         DECIMAL(18, 4)   NOT NULL DEFAULT 0,
        [Month3EndNav]         DECIMAL(18, 4)   NOT NULL DEFAULT 0,
        [AverageNav]           DECIMAL(18, 4)   NOT NULL DEFAULT 0,
        [FeeType]              NVARCHAR(50)     NOT NULL,
        [FeeRateApplied]       DECIMAL(18, 4)   NOT NULL DEFAULT 0,
        [CalculatedFeeAmount]  DECIMAL(18, 2)   NOT NULL DEFAULT 0,
        [Status]               NVARCHAR(30)     NOT NULL DEFAULT 'PENDING',  -- PENDING | DEDUCTED | FAILED | WAIVED
        [JournalReference]     NVARCHAR(100)    NULL,
        [ProcessedAt]          DATETIME2        NOT NULL DEFAULT GETUTCDATE(),

        CONSTRAINT [PK_TBL_QUARTERLY_FEE_LOG] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TBL_QUARTERLY_FEE_LOG_Customer]
            FOREIGN KEY ([CustomerId]) REFERENCES [TBL_CUSTOMER] ([CustomerId]),
        CONSTRAINT [FK_TBL_QUARTERLY_FEE_LOG_Portfolio]
            FOREIGN KEY ([PortfolioId]) REFERENCES [TBL_PORTFOLIO] ([PortfolioId]),
        CONSTRAINT [FK_TBL_QUARTERLY_FEE_LOG_FeeConfig]
            FOREIGN KEY ([FeeConfigId]) REFERENCES [TBL_PORTFOLIO_FEE_CONFIG] ([Id])
    );

    -- Indexes for common query patterns
    CREATE INDEX [IX_TBL_QUARTERLY_FEE_LOG_Customer]  ON [TBL_QUARTERLY_FEE_LOG] ([CustomerId]);
    CREATE INDEX [IX_TBL_QUARTERLY_FEE_LOG_Portfolio] ON [TBL_QUARTERLY_FEE_LOG] ([PortfolioId]);
    CREATE INDEX [IX_TBL_QUARTERLY_FEE_LOG_YearQtr]  ON [TBL_QUARTERLY_FEE_LOG] ([Year], [Quarter]);

    PRINT 'Created TBL_QUARTERLY_FEE_LOG';
END
ELSE
BEGIN
    PRINT 'TBL_QUARTERLY_FEE_LOG already exists — skipped.';
END
GO

PRINT 'Done.';
