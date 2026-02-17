-- Migration 076: Add MinimumStock to Masters_Components
-- Used to trigger low-stock alerts on Dashboard and Receive Components page.
-- A value of 0 means "no minimum set" — no alert for that component.

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'Masters_Components' AND COLUMN_NAME = 'MinimumStock'
)
BEGIN
    ALTER TABLE Masters_Components
    ADD MinimumStock DECIMAL(18,3) NOT NULL DEFAULT 0;

    PRINT 'Migration 076: MinimumStock column added to Masters_Components';
END
ELSE
BEGIN
    PRINT 'Migration 076: MinimumStock column already exists — skipped';
END
GO
