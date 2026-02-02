-- Migration: Add IsPurchased column to Masters_ChildPartTemplates
-- Purpose: Support purchased parts that don't need manufacturing process templates
-- Date: 2026-01-31

USE MultiHitechERP;
GO

PRINT 'Adding IsPurchased column to Masters_ChildPartTemplates...';

-- Check if column doesn't exist before adding
IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID('Masters_ChildPartTemplates')
    AND name = 'IsPurchased'
)
BEGIN
    ALTER TABLE Masters_ChildPartTemplates
    ADD IsPurchased BIT NOT NULL DEFAULT 0;

    PRINT '✓ Added IsPurchased column';
END
ELSE
BEGIN
    PRINT '✓ IsPurchased column already exists';
END

GO

PRINT 'Migration completed successfully!';
GO
