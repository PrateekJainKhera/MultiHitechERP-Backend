-- Add more missing columns to Masters_Processes table
USE MultiHitechERP;
GO

PRINT 'Adding additional missing columns to Masters_Processes table...';
GO

-- Add Category
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'Category')
BEGIN
    ALTER TABLE Masters_Processes ADD Category NVARCHAR(100) NULL;
    PRINT 'Added Category column';
END

-- Add MachineType (may already exist from original schema)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'MachineType')
BEGIN
    ALTER TABLE Masters_Processes ADD MachineType NVARCHAR(50) NULL;
    PRINT 'Added MachineType column';
END
ELSE
BEGIN
    PRINT 'MachineType column already exists';
END

-- Add DefaultMachineId (may already exist from original schema)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'DefaultMachineId')
BEGIN
    ALTER TABLE Masters_Processes ADD DefaultMachineId INT NULL;
    PRINT 'Added DefaultMachineId column';
END
ELSE
BEGIN
    PRINT 'DefaultMachineId column already exists';
END

-- Add IsOutsourced (may already exist from original schema)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'IsOutsourced')
BEGIN
    ALTER TABLE Masters_Processes ADD IsOutsourced BIT NOT NULL DEFAULT 0;
    PRINT 'Added IsOutsourced column';
END
ELSE
BEGIN
    PRINT 'IsOutsourced column already exists';
END

GO

PRINT '';
PRINT '=== Verification: Current table structure ===';
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    NUMERIC_PRECISION,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Masters_Processes'
ORDER BY ORDINAL_POSITION;
GO

PRINT '';
PRINT 'All additional columns added successfully to Masters_Processes table!';
GO
