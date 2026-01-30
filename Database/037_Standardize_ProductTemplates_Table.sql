-- Standardize Masters_ProductTemplates table
USE MultiHitechERP;
GO

PRINT 'Standardizing Masters_ProductTemplates table...';
GO

-- Add UpdatedBy column (missing)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProductTemplates') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE Masters_ProductTemplates ADD UpdatedBy NVARCHAR(100) NULL;
    PRINT 'Added UpdatedBy column';
END
ELSE
BEGIN
    PRINT 'UpdatedBy column already exists';
END
GO

PRINT '';
PRINT '=== Verification: Final table structure ===';
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Masters_ProductTemplates'
ORDER BY ORDINAL_POSITION;
GO

PRINT '';
PRINT 'Masters_ProductTemplates table standardized successfully!';
PRINT 'Added: UpdatedBy';
GO
