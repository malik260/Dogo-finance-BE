-- ============================================================
-- Fee Configuration Seed Script
-- Based on Fund Manager Agreement, Section 6 — Fees and Other Charges
--
-- (a) Management Fee:       1.50% p.a. — charged quarterly in arrears
--                           Basis: Average month-end NAV of portfolio
--                           Charge day: 10th business day after quarter end
--
-- (b) Custody Fee:          0.25% p.a. — chargeable quarterly in arrears
--                           Basis: Average month-end NAV
--
-- (c) Annual Regulatory Fee (SEC): 0.25% p.a. — charged quarterly
--                           Remitted to the Securities and Exchange Commission
--
-- Script is IDEMPOTENT — safe to re-run.
-- For each active portfolio, it inserts the 3 fee rows only if they
-- do not already exist for that portfolio + fee type combination.
-- ============================================================

DECLARE @PortfolioId INT;

DECLARE portfolio_cursor CURSOR FOR
    SELECT PortfolioId
    FROM   TBL_PORTFOLIO
    WHERE  IsActive = 1;

OPEN portfolio_cursor;
FETCH NEXT FROM portfolio_cursor INTO @PortfolioId;

WHILE @@FETCH_STATUS = 0
BEGIN

    -- (a) Management Fee: 1.5% p.a. → Revenue account 4220
    IF NOT EXISTS (
        SELECT 1 FROM TBL_PORTFOLIO_FEE_CONFIG
        WHERE  PortfolioId = @PortfolioId AND FeeType = 'MANAGEMENT'
    )
    BEGIN
        INSERT INTO TBL_PORTFOLIO_FEE_CONFIG
            (PortfolioId, FeeType, PercentagePerAnnum, CalculationBasis,
             BillingFrequency, ChargeDayOfMonth, TargetAccountCode,
             IsLiability, IsWaived, IsActive, CreatedAt)
        VALUES
            (@PortfolioId, 'MANAGEMENT', 1.50, 'AVERAGE_MONTH_END_NAV',
             'QUARTERLY', 10, '4220',
             0, 0, 1, GETUTCDATE());

        PRINT CONCAT('  [Portfolio ', @PortfolioId, '] MANAGEMENT fee inserted.');
    END
    ELSE
        PRINT CONCAT('  [Portfolio ', @PortfolioId, '] MANAGEMENT fee already exists — skipped.');

    -- (b) Custody Fee: 0.25% p.a. → Revenue account 4230
    IF NOT EXISTS (
        SELECT 1 FROM TBL_PORTFOLIO_FEE_CONFIG
        WHERE  PortfolioId = @PortfolioId AND FeeType = 'CUSTODY'
    )
    BEGIN
        INSERT INTO TBL_PORTFOLIO_FEE_CONFIG
            (PortfolioId, FeeType, PercentagePerAnnum, CalculationBasis,
             BillingFrequency, ChargeDayOfMonth, TargetAccountCode,
             IsLiability, IsWaived, IsActive, CreatedAt)
        VALUES
            (@PortfolioId, 'CUSTODY', 0.25, 'AVERAGE_MONTH_END_NAV',
             'QUARTERLY', 10, '4230',
             0, 0, 1, GETUTCDATE());

        PRINT CONCAT('  [Portfolio ', @PortfolioId, '] CUSTODY fee inserted.');
    END
    ELSE
        PRINT CONCAT('  [Portfolio ', @PortfolioId, '] CUSTODY fee already exists — skipped.');

    -- (c) Annual Regulatory Fee (SEC): 0.25% p.a. → Liability account 2210
    IF NOT EXISTS (
        SELECT 1 FROM TBL_PORTFOLIO_FEE_CONFIG
        WHERE  PortfolioId = @PortfolioId AND FeeType = 'SEC_REGULATORY'
    )
    BEGIN
        INSERT INTO TBL_PORTFOLIO_FEE_CONFIG
            (PortfolioId, FeeType, PercentagePerAnnum, CalculationBasis,
             BillingFrequency, ChargeDayOfMonth, TargetAccountCode,
             IsLiability, IsWaived, IsActive, CreatedAt)
        VALUES
            (@PortfolioId, 'SEC_REGULATORY', 0.25, 'AVERAGE_MONTH_END_NAV',
             'QUARTERLY', 10, '2210',
             1, 0, 1, GETUTCDATE());

        PRINT CONCAT('  [Portfolio ', @PortfolioId, '] SEC_REGULATORY fee inserted.');
    END
    ELSE
        PRINT CONCAT('  [Portfolio ', @PortfolioId, '] SEC_REGULATORY fee already exists — skipped.');

    FETCH NEXT FROM portfolio_cursor INTO @PortfolioId;
END

CLOSE portfolio_cursor;
DEALLOCATE portfolio_cursor;

PRINT '';
PRINT 'Fee configuration seed complete.';
PRINT '';
PRINT 'Summary of rates applied:';
PRINT '  Management Fee:       1.50% p.a.  (Quarterly, Avg Month-End NAV, Acct: 4220)';
PRINT '  Custody Fee:          0.25% p.a.  (Quarterly, Avg Month-End NAV, Acct: 4230)';
PRINT '  SEC Regulatory Fee:   0.25% p.a.  (Quarterly, Avg Month-End NAV, Acct: 2210 - Liability)';
PRINT '  Total Annual Charge:  2.00% p.a.';
PRINT '  Charge Day:           10th of month following quarter end';
GO

-- Verification query — view what was inserted
SELECT
    p.Name                  AS Portfolio,
    f.FeeType,
    f.PercentagePerAnnum    AS [Rate (% p.a.)],
    f.BillingFrequency,
    f.CalculationBasis,
    f.ChargeDayOfMonth      AS [Charge Day],
    f.TargetAccountCode     AS [GL Account],
    CASE f.IsLiability WHEN 1 THEN 'Liability (SEC)' ELSE 'Revenue' END AS AccountType,
    CASE f.IsWaived    WHEN 1 THEN 'WAIVED' ELSE 'Active' END AS Status
FROM   TBL_PORTFOLIO_FEE_CONFIG f
JOIN   TBL_PORTFOLIO p ON p.PortfolioId = f.PortfolioId
ORDER  BY p.Name, f.FeeType;
