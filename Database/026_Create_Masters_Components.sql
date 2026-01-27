-- Migration: Create Masters_Components table
-- Description: Master data for components (bearings, gears, seals, etc.)
-- Note: This is NOT inventory - just catalog of components that can be used

-- Drop table if exists (for clean migration during development)
IF OBJECT_ID('Masters_Components', 'U') IS NOT NULL
BEGIN
    DROP TABLE Masters_Components;
    PRINT 'Existing Masters_Components table dropped';
END
GO

-- Create Masters_Components table
CREATE TABLE Masters_Components (
    Id INT PRIMARY KEY IDENTITY(1,1),
    PartNumber NVARCHAR(100) NOT NULL UNIQUE,
    ComponentName NVARCHAR(200) NOT NULL,
    Category NVARCHAR(50) NOT NULL,
    Manufacturer NVARCHAR(200) NULL,
    SupplierName NVARCHAR(200) NULL,
    Specifications NVARCHAR(MAX) NULL,
    UnitCost DECIMAL(18,2) NOT NULL,
    LeadTimeDays INT NOT NULL,
    Unit NVARCHAR(20) NOT NULL,
    Notes NVARCHAR(MAX) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Create index on PartNumber for faster lookups
CREATE INDEX IX_Components_PartNumber ON Masters_Components(PartNumber);
GO

-- Create index on Category for filtering
CREATE INDEX IX_Components_Category ON Masters_Components(Category);
GO

PRINT 'Masters_Components table created successfully';
GO
