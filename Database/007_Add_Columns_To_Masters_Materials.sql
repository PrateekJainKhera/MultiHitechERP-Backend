-- Add missing columns to Masters_Materials table
USE MultiHitechERP;
GO

-- Verify table exists
IF OBJECT_ID('Masters_Materials', 'U') IS NULL
BEGIN
    PRINT 'ERROR: Masters_Materials table does not exist!';
    RETURN;
END

PRINT 'Adding missing columns to Masters_Materials table...';
GO

-- Add Category
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'Category')
BEGIN
    ALTER TABLE Masters_Materials ADD Category NVARCHAR(100) NULL;
    PRINT 'Added Category column';
END

-- Add SubCategory
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'SubCategory')
BEGIN
    ALTER TABLE Masters_Materials ADD SubCategory NVARCHAR(100) NULL;
    PRINT 'Added SubCategory column';
END

-- Add Description
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'Description')
BEGIN
    ALTER TABLE Masters_Materials ADD Description NVARCHAR(500) NULL;
    PRINT 'Added Description column';
END

-- Add HSNCode
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'HSNCode')
BEGIN
    ALTER TABLE Masters_Materials ADD HSNCode NVARCHAR(20) NULL;
    PRINT 'Added HSNCode column';
END

-- Add StandardLength
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'StandardLength')
BEGIN
    ALTER TABLE Masters_Materials ADD StandardLength DECIMAL(18,3) NULL;
    PRINT 'Added StandardLength column';
END

-- Add Diameter
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'Diameter')
BEGIN
    ALTER TABLE Masters_Materials ADD Diameter DECIMAL(18,3) NULL;
    PRINT 'Added Diameter column';
END

-- Add Thickness
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'Thickness')
BEGIN
    ALTER TABLE Masters_Materials ADD Thickness DECIMAL(18,3) NULL;
    PRINT 'Added Thickness column';
END

-- Add Width
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'Width')
BEGIN
    ALTER TABLE Masters_Materials ADD Width DECIMAL(18,3) NULL;
    PRINT 'Added Width column';
END

-- Add PrimaryUOM
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'PrimaryUOM')
BEGIN
    ALTER TABLE Masters_Materials ADD PrimaryUOM NVARCHAR(20) NULL;
    PRINT 'Added PrimaryUOM column';
END

-- Add SecondaryUOM
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'SecondaryUOM')
BEGIN
    ALTER TABLE Masters_Materials ADD SecondaryUOM NVARCHAR(20) NULL;
    PRINT 'Added SecondaryUOM column';
END

-- Add ConversionFactor
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'ConversionFactor')
BEGIN
    ALTER TABLE Masters_Materials ADD ConversionFactor DECIMAL(18,6) NULL;
    PRINT 'Added ConversionFactor column';
END

-- Add WeightPerMeter
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'WeightPerMeter')
BEGIN
    ALTER TABLE Masters_Materials ADD WeightPerMeter DECIMAL(18,4) NULL;
    PRINT 'Added WeightPerMeter column';
END

-- Add WeightPerPiece
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'WeightPerPiece')
BEGIN
    ALTER TABLE Masters_Materials ADD WeightPerPiece DECIMAL(18,4) NULL;
    PRINT 'Added WeightPerPiece column';
END

-- Add Density
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'Density')
BEGIN
    ALTER TABLE Masters_Materials ADD Density DECIMAL(18,4) NULL;
    PRINT 'Added Density column';
END

-- Add LastPurchasePrice
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'LastPurchasePrice')
BEGIN
    ALTER TABLE Masters_Materials ADD LastPurchasePrice DECIMAL(18,2) NULL;
    PRINT 'Added LastPurchasePrice column';
END

-- Add LastPurchaseDate
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'LastPurchaseDate')
BEGIN
    ALTER TABLE Masters_Materials ADD LastPurchaseDate DATETIME NULL;
    PRINT 'Added LastPurchaseDate column';
END

-- Add ReorderQuantity
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'ReorderQuantity')
BEGIN
    ALTER TABLE Masters_Materials ADD ReorderQuantity DECIMAL(18,2) NULL;
    PRINT 'Added ReorderQuantity column';
END

-- Add PreferredSupplierId
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'PreferredSupplierId')
BEGIN
    ALTER TABLE Masters_Materials ADD PreferredSupplierId INT NULL;
    PRINT 'Added PreferredSupplierId column';
END

-- Add PreferredSupplierName
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'PreferredSupplierName')
BEGIN
    ALTER TABLE Masters_Materials ADD PreferredSupplierName NVARCHAR(200) NULL;
    PRINT 'Added PreferredSupplierName column';
END

-- Add StorageLocation
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'StorageLocation')
BEGIN
    ALTER TABLE Masters_Materials ADD StorageLocation NVARCHAR(100) NULL;
    PRINT 'Added StorageLocation column';
END

-- Add StorageConditions
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'StorageConditions')
BEGIN
    ALTER TABLE Masters_Materials ADD StorageConditions NVARCHAR(200) NULL;
    PRINT 'Added StorageConditions column';
END

-- Add Status
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'Status')
BEGIN
    ALTER TABLE Masters_Materials ADD Status NVARCHAR(50) NULL;
    PRINT 'Added Status column';
END

-- Add Remarks if missing
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'Remarks')
BEGIN
    ALTER TABLE Masters_Materials ADD Remarks NVARCHAR(500) NULL;
    PRINT 'Added Remarks column';
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
WHERE TABLE_NAME = 'Masters_Materials'
ORDER BY ORDINAL_POSITION;
GO

PRINT '';
PRINT 'All columns added successfully to Masters_Materials table!';
GO
