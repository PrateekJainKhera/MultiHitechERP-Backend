-- Add missing columns to Masters_Processes table
USE MultiHitechERP;
GO

-- Verify table exists
IF OBJECT_ID('Masters_Processes', 'U') IS NULL
BEGIN
    PRINT 'ERROR: Masters_Processes table does not exist!';
    RETURN;
END

PRINT 'Adding missing columns to Masters_Processes table...';
GO

-- Add ProcessType
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'ProcessType')
BEGIN
    ALTER TABLE Masters_Processes ADD ProcessType NVARCHAR(100) NULL;
    PRINT 'Added ProcessType column';
END

-- Add Department
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'Department')
BEGIN
    ALTER TABLE Masters_Processes ADD Department NVARCHAR(100) NULL;
    PRINT 'Added Department column';
END

-- Add ProcessDetails
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'ProcessDetails')
BEGIN
    ALTER TABLE Masters_Processes ADD ProcessDetails NVARCHAR(MAX) NULL;
    PRINT 'Added ProcessDetails column';
END

-- Add DefaultMachineName
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'DefaultMachineName')
BEGIN
    ALTER TABLE Masters_Processes ADD DefaultMachineName NVARCHAR(200) NULL;
    PRINT 'Added DefaultMachineName column';
END

-- Add StandardSetupTimeMin
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'StandardSetupTimeMin')
BEGIN
    ALTER TABLE Masters_Processes ADD StandardSetupTimeMin DECIMAL(18,2) NULL;
    -- Copy from SetupTimeMin if exists
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'SetupTimeMin')
    BEGIN
        EXEC sp_executesql N'UPDATE Masters_Processes SET StandardSetupTimeMin = SetupTimeMin';
    END
    PRINT 'Added StandardSetupTimeMin column';
END

-- Add StandardCycleTimeMin
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'StandardCycleTimeMin')
BEGIN
    ALTER TABLE Masters_Processes ADD StandardCycleTimeMin DECIMAL(18,2) NULL;
    -- Copy from CycleTimeMin if exists
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'CycleTimeMin')
    BEGIN
        EXEC sp_executesql N'UPDATE Masters_Processes SET StandardCycleTimeMin = CycleTimeMin';
    END
    PRINT 'Added StandardCycleTimeMin column';
END

-- Add StandardCycleTimePerPiece
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'StandardCycleTimePerPiece')
BEGIN
    ALTER TABLE Masters_Processes ADD StandardCycleTimePerPiece DECIMAL(18,2) NULL;
    PRINT 'Added StandardCycleTimePerPiece column';
END

-- Add SkillLevel
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'SkillLevel')
BEGIN
    ALTER TABLE Masters_Processes ADD SkillLevel NVARCHAR(50) NULL;
    PRINT 'Added SkillLevel column';
END

-- Add OperatorsRequired
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'OperatorsRequired')
BEGIN
    ALTER TABLE Masters_Processes ADD OperatorsRequired INT NULL DEFAULT 1;
    PRINT 'Added OperatorsRequired column';
END

-- Add HourlyRate
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'HourlyRate')
BEGIN
    ALTER TABLE Masters_Processes ADD HourlyRate DECIMAL(18,2) NULL;
    PRINT 'Added HourlyRate column';
END

-- Add StandardCostPerPiece
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'StandardCostPerPiece')
BEGIN
    ALTER TABLE Masters_Processes ADD StandardCostPerPiece DECIMAL(18,2) NULL;
    -- Copy from OSPCostPerPiece if exists and IsOutsourced = 1
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'OSPCostPerPiece')
    BEGIN
        EXEC sp_executesql N'UPDATE Masters_Processes SET StandardCostPerPiece = OSPCostPerPiece WHERE IsOutsourced = 1';
    END
    PRINT 'Added StandardCostPerPiece column';
END

-- Add RequiresQC
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'RequiresQC')
BEGIN
    ALTER TABLE Masters_Processes ADD RequiresQC BIT NOT NULL DEFAULT 0;
    -- Copy from RequiresInspection if exists
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'RequiresInspection')
    BEGIN
        EXEC sp_executesql N'UPDATE Masters_Processes SET RequiresQC = RequiresInspection';
    END
    PRINT 'Added RequiresQC column';
END

-- Add QCCheckpoints
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'QCCheckpoints')
BEGIN
    ALTER TABLE Masters_Processes ADD QCCheckpoints NVARCHAR(MAX) NULL;
    -- Copy from InspectionType if exists
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'InspectionType')
    BEGIN
        EXEC sp_executesql N'UPDATE Masters_Processes SET QCCheckpoints = InspectionType';
    END
    PRINT 'Added QCCheckpoints column';
END

-- Add PreferredVendor
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'PreferredVendor')
BEGIN
    ALTER TABLE Masters_Processes ADD PreferredVendor NVARCHAR(200) NULL;
    PRINT 'Added PreferredVendor column';
END

-- Add Status
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'Status')
BEGIN
    ALTER TABLE Masters_Processes ADD Status NVARCHAR(50) NULL DEFAULT 'Active';
    PRINT 'Added Status column';
END

-- Add Remarks
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'Remarks')
BEGIN
    ALTER TABLE Masters_Processes ADD Remarks NVARCHAR(500) NULL;
    PRINT 'Added Remarks column';
END

-- Add UpdatedAt
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'UpdatedAt')
BEGIN
    ALTER TABLE Masters_Processes ADD UpdatedAt DATETIME NULL;
    PRINT 'Added UpdatedAt column';
END

-- Add UpdatedBy
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE Masters_Processes ADD UpdatedBy NVARCHAR(100) NULL;
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
WHERE TABLE_NAME = 'Masters_Processes'
ORDER BY ORDINAL_POSITION;
GO

PRINT '';
PRINT 'All columns added successfully to Masters_Processes table!';
GO
