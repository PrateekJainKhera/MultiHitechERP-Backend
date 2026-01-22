-- Add missing columns to Masters_Machines table
USE MultiHitechERP;
GO

-- Verify table exists
IF OBJECT_ID('Masters_Machines', 'U') IS NULL
BEGIN
    PRINT 'ERROR: Masters_Machines table does not exist!';
    RETURN;
END

PRINT 'Adding missing columns to Masters_Machines table...';
GO

-- Add MachineType (was "Type")
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Machines') AND name = 'MachineType')
BEGIN
    ALTER TABLE Masters_Machines ADD MachineType NVARCHAR(50) NULL;
    -- Copy data from old Type column if it exists
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Machines') AND name = 'Type')
    BEGIN
        EXEC sp_executesql N'UPDATE Masters_Machines SET MachineType = Type';
    END
    PRINT 'Added MachineType column';
END

-- Add Category
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Machines') AND name = 'Category')
BEGIN
    ALTER TABLE Masters_Machines ADD Category NVARCHAR(100) NULL;
    PRINT 'Added Category column';
END

-- Add SerialNumber
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Machines') AND name = 'SerialNumber')
BEGIN
    ALTER TABLE Masters_Machines ADD SerialNumber NVARCHAR(50) NULL;
    PRINT 'Added SerialNumber column';
END

-- Add CapacityUnit
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Machines') AND name = 'CapacityUnit')
BEGIN
    ALTER TABLE Masters_Machines ADD CapacityUnit NVARCHAR(20) NULL;
    PRINT 'Added CapacityUnit column';
END

-- Modify Capacity to DECIMAL if it's NVARCHAR
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Machines') AND name = 'Capacity' AND system_type_id = 231)
BEGIN
    -- Capacity is NVARCHAR, we'll leave it as is since changing data type is risky
    PRINT 'Capacity column exists as NVARCHAR - keeping as is';
END

-- Add Specifications
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Machines') AND name = 'Specifications')
BEGIN
    ALTER TABLE Masters_Machines ADD Specifications NVARCHAR(500) NULL;
    PRINT 'Added Specifications column';
END

-- Add MaxWorkpieceLength
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Machines') AND name = 'MaxWorkpieceLength')
BEGIN
    ALTER TABLE Masters_Machines ADD MaxWorkpieceLength DECIMAL(18,3) NULL;
    -- Copy from MaxLength if exists
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Machines') AND name = 'MaxLength')
    BEGIN
        EXEC sp_executesql N'UPDATE Masters_Machines SET MaxWorkpieceLength = MaxLength';
    END
    PRINT 'Added MaxWorkpieceLength column';
END

-- Add MaxWorkpieceDiameter
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Machines') AND name = 'MaxWorkpieceDiameter')
BEGIN
    ALTER TABLE Masters_Machines ADD MaxWorkpieceDiameter DECIMAL(18,3) NULL;
    -- Copy from MaxDiameter if exists
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Machines') AND name = 'MaxDiameter')
    BEGIN
        EXEC sp_executesql N'UPDATE Masters_Machines SET MaxWorkpieceDiameter = MaxDiameter';
    END
    PRINT 'Added MaxWorkpieceDiameter column';
END

-- Add ChuckSize
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Machines') AND name = 'ChuckSize')
BEGIN
    ALTER TABLE Masters_Machines ADD ChuckSize DECIMAL(18,3) NULL;
    PRINT 'Added ChuckSize column';
END

-- Add ShopFloor
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Machines') AND name = 'ShopFloor')
BEGIN
    ALTER TABLE Masters_Machines ADD ShopFloor NVARCHAR(100) NULL;
    -- Copy from ShopFloorLocation if exists
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Machines') AND name = 'ShopFloorLocation')
    BEGIN
        EXEC sp_executesql N'UPDATE Masters_Machines SET ShopFloor = ShopFloorLocation';
    END
    PRINT 'Added ShopFloor column';
END

-- Add Location
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Machines') AND name = 'Location')
BEGIN
    ALTER TABLE Masters_Machines ADD Location NVARCHAR(200) NULL;
    PRINT 'Added Location column';
END

-- Add HourlyRate
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Machines') AND name = 'HourlyRate')
BEGIN
    ALTER TABLE Masters_Machines ADD HourlyRate DECIMAL(18,2) NULL;
    PRINT 'Added HourlyRate column';
END

-- Add PowerConsumption
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Machines') AND name = 'PowerConsumption')
BEGIN
    ALTER TABLE Masters_Machines ADD PowerConsumption DECIMAL(18,2) NULL;
    PRINT 'Added PowerConsumption column';
END

-- Add OperatorsRequired
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Machines') AND name = 'OperatorsRequired')
BEGIN
    ALTER TABLE Masters_Machines ADD OperatorsRequired INT NULL;
    PRINT 'Added OperatorsRequired column';
END

-- Add PurchaseDate
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Machines') AND name = 'PurchaseDate')
BEGIN
    ALTER TABLE Masters_Machines ADD PurchaseDate DATETIME NULL;
    PRINT 'Added PurchaseDate column';
END

-- Add NextMaintenanceDate (may already exist)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Machines') AND name = 'NextMaintenanceDate')
BEGIN
    ALTER TABLE Masters_Machines ADD NextMaintenanceDate DATETIME NULL;
    PRINT 'Added NextMaintenanceDate column';
END

-- Add MaintenanceSchedule
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Machines') AND name = 'MaintenanceSchedule')
BEGIN
    ALTER TABLE Masters_Machines ADD MaintenanceSchedule NVARCHAR(200) NULL;
    PRINT 'Added MaintenanceSchedule column';
END

-- Add CurrentJobCardNo
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Machines') AND name = 'CurrentJobCardNo')
BEGIN
    ALTER TABLE Masters_Machines ADD CurrentJobCardNo NVARCHAR(50) NULL;
    PRINT 'Added CurrentJobCardNo column';
END

-- Add IsAvailable
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Machines') AND name = 'IsAvailable')
BEGIN
    ALTER TABLE Masters_Machines ADD IsAvailable BIT NOT NULL DEFAULT 1;
    PRINT 'Added IsAvailable column';
END

-- Add AvailableFrom
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Machines') AND name = 'AvailableFrom')
BEGIN
    ALTER TABLE Masters_Machines ADD AvailableFrom DATETIME NULL;
    PRINT 'Added AvailableFrom column';
END

-- Add Remarks if missing
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Machines') AND name = 'Remarks')
BEGIN
    ALTER TABLE Masters_Machines ADD Remarks NVARCHAR(500) NULL;
    PRINT 'Added Remarks column';
END

-- Add UpdatedAt if missing
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Machines') AND name = 'UpdatedAt')
BEGIN
    ALTER TABLE Masters_Machines ADD UpdatedAt DATETIME NULL;
    PRINT 'Added UpdatedAt column';
END

-- Add UpdatedBy if missing
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Machines') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE Masters_Machines ADD UpdatedBy NVARCHAR(100) NULL;
    PRINT 'Added UpdatedBy column';
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
WHERE TABLE_NAME = 'Masters_Machines'
ORDER BY ORDINAL_POSITION;
GO

PRINT '';
PRINT 'All columns added successfully to Masters_Machines table!';
GO
