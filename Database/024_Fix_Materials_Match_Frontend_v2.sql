-- Fix Masters_Materials to match frontend exactly
USE MultiHitechERP;
GO

PRINT 'Fixing Masters_Materials to match frontend...';
GO

-- Step 1: Add missing Shape column (MaterialShape enum: Rod, Pipe, Forged)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'Shape')
BEGIN
    ALTER TABLE Masters_Materials ADD Shape NVARCHAR(50) NULL;
    PRINT 'Added Shape column';
END
GO

-- Step 2: Rename StandardLength to LengthInMM
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'StandardLength')
AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'LengthInMM')
BEGIN
    EXEC sp_rename 'Masters_Materials.StandardLength', 'LengthInMM', 'COLUMN';
    PRINT 'Renamed StandardLength to LengthInMM';
END
GO

-- Step 3: Rename WeightPerPiece to WeightKG
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'WeightPerPiece')
AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'WeightKG')
BEGIN
    EXEC sp_rename 'Masters_Materials.WeightPerPiece', 'WeightKG', 'COLUMN';
    PRINT 'Renamed WeightPerPiece to WeightKG';
END
GO

-- Step 4: Add StockQty column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'StockQty')
BEGIN
    ALTER TABLE Masters_Materials ADD StockQty DECIMAL(18,2) DEFAULT 0 NULL;
    PRINT 'Added StockQty column';
END
GO

-- Step 5: Drop unique constraint on MaterialCode
IF EXISTS (SELECT * FROM sys.key_constraints WHERE parent_object_id = OBJECT_ID('Masters_Materials') AND name = 'UQ__Masters___170C54BAAFC37CFB')
BEGIN
    ALTER TABLE Masters_Materials DROP CONSTRAINT UQ__Masters___170C54BAAFC37CFB;
    PRINT 'Dropped unique constraint on MaterialCode';
END
GO

-- Step 6: Drop unnecessary columns
PRINT 'Removing unnecessary columns...';
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'MaterialCode')
    ALTER TABLE Masters_Materials DROP COLUMN MaterialCode;
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'MaterialType')
    ALTER TABLE Masters_Materials DROP COLUMN MaterialType;
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'Specification')
    ALTER TABLE Masters_Materials DROP COLUMN Specification;
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'UOM')
    ALTER TABLE Masters_Materials DROP COLUMN UOM;
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'StandardCost')
    ALTER TABLE Masters_Materials DROP COLUMN StandardCost;
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'MaxStockLevel')
    ALTER TABLE Masters_Materials DROP COLUMN MaxStockLevel;
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'ReorderLevel')
    ALTER TABLE Masters_Materials DROP COLUMN ReorderLevel;
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'LeadTimeDays')
    ALTER TABLE Masters_Materials DROP COLUMN LeadTimeDays;
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'IsActive')
    ALTER TABLE Masters_Materials DROP COLUMN IsActive;
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'CreatedBy')
    ALTER TABLE Masters_Materials DROP COLUMN CreatedBy;
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'UpdatedBy')
    ALTER TABLE Masters_Materials DROP COLUMN UpdatedBy;
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'Category')
    ALTER TABLE Masters_Materials DROP COLUMN Category;
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'SubCategory')
    ALTER TABLE Masters_Materials DROP COLUMN SubCategory;
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'Description')
    ALTER TABLE Masters_Materials DROP COLUMN Description;
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'HSNCode')
    ALTER TABLE Masters_Materials DROP COLUMN HSNCode;
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'Thickness')
    ALTER TABLE Masters_Materials DROP COLUMN Thickness;
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'Width')
    ALTER TABLE Masters_Materials DROP COLUMN Width;
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'PrimaryUOM')
    ALTER TABLE Masters_Materials DROP COLUMN PrimaryUOM;
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'SecondaryUOM')
    ALTER TABLE Masters_Materials DROP COLUMN SecondaryUOM;
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'ConversionFactor')
    ALTER TABLE Masters_Materials DROP COLUMN ConversionFactor;
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'WeightPerMeter')
    ALTER TABLE Masters_Materials DROP COLUMN WeightPerMeter;
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'LastPurchasePrice')
    ALTER TABLE Masters_Materials DROP COLUMN LastPurchasePrice;
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'LastPurchaseDate')
    ALTER TABLE Masters_Materials DROP COLUMN LastPurchaseDate;
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'ReorderQuantity')
    ALTER TABLE Masters_Materials DROP COLUMN ReorderQuantity;
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'PreferredSupplierId')
    ALTER TABLE Masters_Materials DROP COLUMN PreferredSupplierId;
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'PreferredSupplierName')
    ALTER TABLE Masters_Materials DROP COLUMN PreferredSupplierName;
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'StorageLocation')
    ALTER TABLE Masters_Materials DROP COLUMN StorageLocation;
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'StorageConditions')
    ALTER TABLE Masters_Materials DROP COLUMN StorageConditions;
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'Status')
    ALTER TABLE Masters_Materials DROP COLUMN Status;
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'Remarks')
    ALTER TABLE Masters_Materials DROP COLUMN Remarks;
GO

PRINT 'Dropped 30+ unnecessary columns';
GO

-- Step 7: Set defaults before making NOT NULL
PRINT 'Setting default values...';
GO

UPDATE Masters_Materials SET MaterialName = 'Unknown' WHERE MaterialName IS NULL OR MaterialName = '';
UPDATE Masters_Materials SET Grade = 'EN8' WHERE Grade IS NULL OR Grade = '';
UPDATE Masters_Materials SET Shape = 'Rod' WHERE Shape IS NULL;
UPDATE Masters_Materials SET Diameter = 0 WHERE Diameter IS NULL;
UPDATE Masters_Materials SET LengthInMM = 0 WHERE LengthInMM IS NULL;
UPDATE Masters_Materials SET Density = 7.85 WHERE Density IS NULL; -- Default steel density
UPDATE Masters_Materials SET WeightKG = 0 WHERE WeightKG IS NULL;
UPDATE Masters_Materials SET StockQty = 0 WHERE StockQty IS NULL;
UPDATE Masters_Materials SET MinStockLevel = 0 WHERE MinStockLevel IS NULL;
UPDATE Masters_Materials SET CreatedAt = GETDATE() WHERE CreatedAt IS NULL;
UPDATE Masters_Materials SET UpdatedAt = GETDATE() WHERE UpdatedAt IS NULL;
GO

-- Step 8: Make required columns NOT NULL
PRINT 'Setting NOT NULL constraints...';
GO

ALTER TABLE Masters_Materials ALTER COLUMN MaterialName NVARCHAR(200) NOT NULL;
ALTER TABLE Masters_Materials ALTER COLUMN Grade NVARCHAR(50) NOT NULL;
ALTER TABLE Masters_Materials ALTER COLUMN Shape NVARCHAR(50) NOT NULL;
ALTER TABLE Masters_Materials ALTER COLUMN Diameter DECIMAL(18,2) NOT NULL;
ALTER TABLE Masters_Materials ALTER COLUMN LengthInMM DECIMAL(18,2) NOT NULL;
ALTER TABLE Masters_Materials ALTER COLUMN Density DECIMAL(18,4) NOT NULL;
ALTER TABLE Masters_Materials ALTER COLUMN WeightKG DECIMAL(18,3) NOT NULL;
ALTER TABLE Masters_Materials ALTER COLUMN StockQty DECIMAL(18,2) NOT NULL;
ALTER TABLE Masters_Materials ALTER COLUMN MinStockLevel DECIMAL(18,2) NOT NULL;
ALTER TABLE Masters_Materials ALTER COLUMN CreatedAt DATETIME NOT NULL;
ALTER TABLE Masters_Materials ALTER COLUMN UpdatedAt DATETIME NOT NULL;
GO

PRINT '';
PRINT '=== Final Materials structure ===';
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION, NUMERIC_SCALE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Masters_Materials'
ORDER BY ORDINAL_POSITION;
GO

PRINT '';
PRINT '✓ Materials table fixed to match frontend!';
PRINT '  Final fields: Id, MaterialName, Grade, Shape, Diameter, LengthInMM, Density, WeightKG, StockQty, MinStockLevel, CreatedAt, UpdatedAt';
GO
