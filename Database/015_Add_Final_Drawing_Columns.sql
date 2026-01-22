-- Add final missing columns to Masters_Drawings table
USE MultiHitechERP;
GO

PRINT 'Adding final missing columns to Masters_Drawings table...';
GO

-- Add FileName (may already exist from original schema)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'FileName')
BEGIN
    ALTER TABLE Masters_Drawings ADD FileName NVARCHAR(255) NULL;
    PRINT 'Added FileName column';
END
ELSE
BEGIN
    PRINT 'FileName column already exists';
END

-- Add ApprovedBy (may already exist from original schema)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'ApprovedBy')
BEGIN
    ALTER TABLE Masters_Drawings ADD ApprovedBy NVARCHAR(100) NULL;
    PRINT 'Added ApprovedBy column';
END
ELSE
BEGIN
    PRINT 'ApprovedBy column already exists';
END

-- Add Status (may already exist from original schema)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'Status')
BEGIN
    ALTER TABLE Masters_Drawings ADD Status NVARCHAR(50) NULL DEFAULT 'Draft';
    PRINT 'Added Status column';
END
ELSE
BEGIN
    PRINT 'Status column already exists';
END

GO

PRINT '';
PRINT '=== Final verification: Current table structure ===';
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    NUMERIC_PRECISION,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Masters_Drawings'
ORDER BY ORDINAL_POSITION;
GO

PRINT '';
PRINT 'All final columns added successfully to Masters_Drawings table!';
GO
