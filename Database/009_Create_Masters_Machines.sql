-- Create Masters_Machines table
USE MultiHitechERP;
GO

-- Check if table already exists
IF OBJECT_ID('Masters_Machines', 'U') IS NOT NULL
BEGIN
    PRINT 'Masters_Machines table already exists. Skipping creation.';
    RETURN;
END

PRINT 'Creating Masters_Machines table...';
GO

CREATE TABLE Masters_Machines (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    MachineCode NVARCHAR(50) NOT NULL UNIQUE,
    MachineName NVARCHAR(200) NOT NULL,

    -- Classification
    MachineType NVARCHAR(50) NULL,
    Category NVARCHAR(100) NULL,

    -- Technical Details
    Manufacturer NVARCHAR(100) NULL,
    Model NVARCHAR(100) NULL,
    SerialNumber NVARCHAR(50) NULL,
    YearOfManufacture INT NULL,

    -- Capacity & Specifications
    Capacity DECIMAL(18,2) NULL,
    CapacityUnit NVARCHAR(20) NULL,
    Specifications NVARCHAR(500) NULL,

    -- Dimensions
    MaxWorkpieceLength DECIMAL(18,3) NULL,
    MaxWorkpieceDiameter DECIMAL(18,3) NULL,
    ChuckSize DECIMAL(18,3) NULL,

    -- Location
    Department NVARCHAR(100) NULL,
    ShopFloor NVARCHAR(100) NULL,
    Location NVARCHAR(200) NULL,

    -- Operational Details
    HourlyRate DECIMAL(18,2) NULL,
    PowerConsumption DECIMAL(18,2) NULL,
    OperatorsRequired INT NULL,

    -- Maintenance
    PurchaseDate DATETIME NULL,
    LastMaintenanceDate DATETIME NULL,
    NextMaintenanceDate DATETIME NULL,
    MaintenanceSchedule NVARCHAR(200) NULL,

    -- Status
    IsActive BIT NOT NULL DEFAULT 1,
    Status NVARCHAR(50) NULL DEFAULT 'Available',
    CurrentJobCardNo NVARCHAR(50) NULL,

    -- Availability
    IsAvailable BIT NOT NULL DEFAULT 1,
    AvailableFrom DATETIME NULL,

    Remarks NVARCHAR(500) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedBy NVARCHAR(100) NULL,
    UpdatedAt DATETIME NULL,
    UpdatedBy NVARCHAR(100) NULL
);

GO

-- Create indexes for better performance
CREATE INDEX IX_Masters_Machines_MachineCode ON Masters_Machines(MachineCode);
CREATE INDEX IX_Masters_Machines_MachineType ON Masters_Machines(MachineType);
CREATE INDEX IX_Masters_Machines_Category ON Masters_Machines(Category);
CREATE INDEX IX_Masters_Machines_IsActive ON Masters_Machines(IsActive);
CREATE INDEX IX_Masters_Machines_IsAvailable ON Masters_Machines(IsAvailable);
CREATE INDEX IX_Masters_Machines_Status ON Masters_Machines(Status);
CREATE INDEX IX_Masters_Machines_Department ON Masters_Machines(Department);

GO

PRINT 'Masters_Machines table created successfully!';
PRINT '';
PRINT '=== Table Structure ===';
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    NUMERIC_PRECISION,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Masters_Machines'
ORDER BY ORDINAL_POSITION;

GO
