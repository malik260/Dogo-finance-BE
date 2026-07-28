-- ============================================================
-- Idempotent: Adds AccruedFeesApplied column to 
-- TBL_LIQUIDATION_REQUEST to store pro-rated platform fees
-- (management, custody, SEC) deducted at liquidation time.
-- ============================================================

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME   = 'TBL_LIQUIDATION_REQUEST'
      AND COLUMN_NAME  = 'AccruedFeesApplied'
)
BEGIN
    ALTER TABLE [TBL_LIQUIDATION_REQUEST]
    ADD [AccruedFeesApplied] DECIMAL(18, 2) NOT NULL DEFAULT 0;
    PRINT 'Column AccruedFeesApplied added to TBL_LIQUIDATION_REQUEST.';
END
ELSE
BEGIN
    PRINT 'Column AccruedFeesApplied already exists — skipped.';
END
GO
