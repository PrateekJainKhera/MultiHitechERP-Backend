-- Standardize Masters_Components table to match other masters
USE MultiHitechERP;
GO

PRINT 'Standardizing Masters_Components table...';
GO

-- Add IsActive column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Components') AND name = 'IsActive')
BEGIN
    ALTER TABLE Masters_Components ADD IsActive BIT NOT NULL DEFAULT 1;
    PRINT 'Added IsActive column';
END
ELSE
BEGIN
    PRINT 'IsActive column already exists';
END
GO

-- Add CreatedBy column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Components') AND name = 'CreatedBy')
BEGIN
    ALTER TABLE Masters_Components ADD CreatedBy NVARCHAR(100) NULL;
    PRINT 'Added CreatedBy column';
END
ELSE
BEGIN
    PRINT 'CreatedBy column already exists';
END
GO

-- Add UpdatedBy column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Components') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE Masters_Components ADD UpdatedBy NVARCHAR(100) NULL;
    PRINT 'Added UpdatedBy column';
END
ELSE
BEGIN
    PRINT 'UpdatedBy column already exists';
END
GO

-- Drop UnitCost column (not used in frontend)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Components') AND name = 'UnitCost')
BEGIN
    ALTER TABLE Masters_Components DROP COLUMN UnitCost;
    PRINT 'Dropped UnitCost column';
END
ELSE
BEGIN
    PRINT 'UnitCost column already dropped';
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
WHERE TABLE_NAME = 'Masters_Components'
ORDER BY ORDINAL_POSITION;
GO

PRINT '';
PRINT 'Masters_Components table standardized successfully!';
PRINT 'Added: IsActive, CreatedBy, UpdatedBy';
PRINT 'Dropped: UnitCost';
GO
