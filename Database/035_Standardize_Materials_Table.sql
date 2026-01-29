-- Standardize Masters_Materials table to match other masters
USE MultiHitechERP;
GO

PRINT 'Standardizing Masters_Materials table...';
GO

-- Add MaterialCode column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'MaterialCode')
BEGIN
    ALTER TABLE Masters_Materials ADD MaterialCode NVARCHAR(50) NULL;
    PRINT 'Added MaterialCode column';
END
ELSE
BEGIN
    PRINT 'MaterialCode column already exists';
END
GO

-- Add IsActive column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'IsActive')
BEGIN
    ALTER TABLE Masters_Materials ADD IsActive BIT NOT NULL DEFAULT 1;
    PRINT 'Added IsActive column';
END
ELSE
BEGIN
    PRINT 'IsActive column already exists';
END
GO

-- Add CreatedBy column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'CreatedBy')
BEGIN
    ALTER TABLE Masters_Materials ADD CreatedBy NVARCHAR(100) NULL;
    PRINT 'Added CreatedBy column';
END
ELSE
BEGIN
    PRINT 'CreatedBy column already exists';
END
GO

-- Add UpdatedBy column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE Masters_Materials ADD UpdatedBy NVARCHAR(100) NULL;
    PRINT 'Added UpdatedBy column';
END
ELSE
BEGIN
    PRINT 'UpdatedBy column already exists';
END
GO

-- Generate MaterialCode for existing records (if any)
-- Format: GRADE-SHAPE-DIAMETER (e.g., EN8-ROD-050, SS304-PIPE-100)
UPDATE Masters_Materials
SET MaterialCode =
    REPLACE(Grade, ' ', '') + '-' +
    UPPER(SUBSTRING(Shape, 1, 3)) + '-' +
    RIGHT('000' + CAST(CAST(Diameter AS INT) AS VARCHAR), 3)
WHERE MaterialCode IS NULL;
GO

-- Make MaterialCode NOT NULL after populating existing records
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'MaterialCode' AND is_nullable = 1)
BEGIN
    ALTER TABLE Masters_Materials ALTER COLUMN MaterialCode NVARCHAR(50) NOT NULL;
    PRINT 'MaterialCode set to NOT NULL';
END
GO

-- Create unique index on MaterialCode
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'UX_Materials_MaterialCode')
BEGIN
    CREATE UNIQUE INDEX UX_Materials_MaterialCode ON Masters_Materials(MaterialCode);
    PRINT 'Created unique index on MaterialCode';
END
GO

PRINT '';
PRINT '=== Verification: Final table structure ===';
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    NUMERIC_PRECISION,
    NUMERIC_SCALE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Masters_Materials'
ORDER BY ORDINAL_POSITION;
GO

PRINT '';
PRINT 'Masters_Materials table standardized successfully!';
PRINT 'New columns: MaterialCode, IsActive, CreatedBy, UpdatedBy';
GO
