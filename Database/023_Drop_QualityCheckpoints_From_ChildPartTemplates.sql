-- Drop QualityCheckpoints column from Masters_ChildPartTemplates
-- QualityCheckpoints is part of production module, not master data
USE MultiHitechERP;
GO

PRINT 'Dropping QualityCheckpoints column from Masters_ChildPartTemplates...';
GO

-- Drop the column if it exists
IF EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID('Masters_ChildPartTemplates')
    AND name = 'QualityCheckpoints'
)
BEGIN
    ALTER TABLE Masters_ChildPartTemplates DROP COLUMN QualityCheckpoints;
    PRINT 'QualityCheckpoints column dropped successfully';
END
ELSE
BEGIN
    PRINT 'QualityCheckpoints column does not exist - no action needed';
END
GO

PRINT '';
PRINT '=== Verification: Current table structure ===';
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Masters_ChildPartTemplates'
ORDER BY ORDINAL_POSITION;
GO

PRINT '';
PRINT 'Migration completed successfully!';
GO
