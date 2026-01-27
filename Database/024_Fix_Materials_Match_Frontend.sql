-- Fix Masters_Materials to match frontend exactly
USE MultiHitechERP;
GO

PRINT 'Fixing Masters_Materials to match frontend...';
GO

-- Add missing Shape column (MaterialShape enum: Rod, Pipe, Forged)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'Shape')
BEGIN
    ALTER TABLE Masters_Materials ADD Shape NVARCHAR(50) NULL;
    PRINT 'Added Shape column';
END
GO

-- Copy StandardLength to LengthInMM before dropping
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'StandardLength')
AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'LengthInMM')
BEGIN
    EXEC sp_rename 'Masters_Materials.StandardLength', 'LengthInMM', 'COLUMN';
    PRINT 'Renamed StandardLength to LengthInMM';
END
GO

-- Copy WeightPerPiece to WeightKG before dropping
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'WeightPerPiece')
AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'WeightKG')
BEGIN
    EXEC sp_rename 'Masters_Materials.WeightPerPiece', 'WeightKG', 'COLUMN';
    PRINT 'Renamed WeightPerPiece to WeightKG';
END
GO

-- Add StockQty column (current inventory quantity in master data)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'StockQty')
BEGIN
    ALTER TABLE Masters_Materials ADD StockQty DECIMAL(18,2) DEFAULT 0 NULL;
    PRINT 'Added StockQty column';
END
GO

-- Drop unnecessary columns to simplify the table
PRINT 'Removing unnecessary columns...';
GO

-- Drop MaterialCode (not needed in frontend)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'MaterialCode')
BEGIN
    -- Drop unique constraint first if exists
    DECLARE @ConstraintName NVARCHAR(200)

    -- Check for unique constraint
    SELECT @ConstraintName = name FROM sys.objects
    WHERE object_id IN (
        SELECT constraint_object_id FROM sys.key_constraints
        WHERE parent_object_id = OBJECT_ID('Masters_Materials')
        AND type = 'UQ'
    ) OR object_id IN (
        SELECT object_id FROM sys.indexes
        WHERE object_id = OBJECT_ID('Masters_Materials')
        AND is_unique_constraint = 1
    )
    AND @ConstraintName LIKE '%Material%'

    IF @ConstraintName IS NOT NULL
        EXEC('ALTER TABLE Masters_Materials DROP CONSTRAINT ' + @ConstraintName)

    ALTER TABLE Masters_Materials DROP COLUMN MaterialCode;
    PRINT 'Dropped MaterialCode column';
END
GO

-- Drop MaterialType
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'MaterialType')
    ALTER TABLE Masters_Materials DROP COLUMN MaterialType;
GO

-- Drop Specification
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'Specification')
    ALTER TABLE Masters_Materials DROP COLUMN Specification;
GO

-- Drop UOM (we'll use KG as standard)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'UOM')
BEGIN
    -- Drop default constraint first
    DECLARE @DefaultConstraint NVARCHAR(200)
    SELECT @DefaultConstraint = name FROM sys.default_constraints
    WHERE parent_object_id = OBJECT_ID('Masters_Materials')
    AND parent_column_id = (SELECT column_id FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'UOM')
    IF @DefaultConstraint IS NOT NULL
        EXEC('ALTER TABLE Masters_Materials DROP CONSTRAINT ' + @DefaultConstraint)

    ALTER TABLE Masters_Materials DROP COLUMN UOM;
END
GO

-- Drop StandardCost
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'StandardCost')
    ALTER TABLE Masters_Materials DROP COLUMN StandardCost;
GO

-- Drop MaxStockLevel
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'MaxStockLevel')
    ALTER TABLE Masters_Materials DROP COLUMN MaxStockLevel;
GO

-- Drop ReorderLevel
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'ReorderLevel')
    ALTER TABLE Masters_Materials DROP COLUMN ReorderLevel;
GO

-- Drop LeadTimeDays
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'LeadTimeDays')
    ALTER TABLE Masters_Materials DROP COLUMN LeadTimeDays;
GO

-- Drop IsActive
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'IsActive')
BEGIN
    -- Drop default constraint first
    DECLARE @IsActiveDefault NVARCHAR(200)
    SELECT @IsActiveDefault = name FROM sys.default_constraints
    WHERE parent_object_id = OBJECT_ID('Masters_Materials')
    AND parent_column_id = (SELECT column_id FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'IsActive')
    IF @IsActiveDefault IS NOT NULL
        EXEC('ALTER TABLE Masters_Materials DROP CONSTRAINT ' + @IsActiveDefault)

    ALTER TABLE Masters_Materials DROP COLUMN IsActive;
END
GO

-- Drop CreatedBy
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'CreatedBy')
    ALTER TABLE Masters_Materials DROP COLUMN CreatedBy;
GO

-- Drop UpdatedBy
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'UpdatedBy')
    ALTER TABLE Masters_Materials DROP COLUMN UpdatedBy;
GO

-- Drop Category
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'Category')
    ALTER TABLE Masters_Materials DROP COLUMN Category;
GO

-- Drop SubCategory
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'SubCategory')
    ALTER TABLE Masters_Materials DROP COLUMN SubCategory;
GO

-- Drop Description
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'Description')
    ALTER TABLE Masters_Materials DROP COLUMN Description;
GO

-- Drop HSNCode
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'HSNCode')
    ALTER TABLE Masters_Materials DROP COLUMN HSNCode;
GO

-- StandardLength already renamed to LengthInMM above
-- Skipping drop

-- Drop Thickness
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'Thickness')
    ALTER TABLE Masters_Materials DROP COLUMN Thickness;
GO

-- Drop Width
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'Width')
    ALTER TABLE Masters_Materials DROP COLUMN Width;
GO

-- Drop PrimaryUOM
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'PrimaryUOM')
    ALTER TABLE Masters_Materials DROP COLUMN PrimaryUOM;
GO

-- Drop SecondaryUOM
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'SecondaryUOM')
    ALTER TABLE Masters_Materials DROP COLUMN SecondaryUOM;
GO

-- Drop ConversionFactor
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'ConversionFactor')
    ALTER TABLE Masters_Materials DROP COLUMN ConversionFactor;
GO

-- Drop WeightPerMeter
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'WeightPerMeter')
    ALTER TABLE Masters_Materials DROP COLUMN WeightPerMeter;
GO

-- WeightPerPiece already renamed to WeightKG above
-- Skipping drop

-- Drop LastPurchasePrice
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'LastPurchasePrice')
    ALTER TABLE Masters_Materials DROP COLUMN LastPurchasePrice;
GO

-- Drop LastPurchaseDate
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'LastPurchaseDate')
    ALTER TABLE Masters_Materials DROP COLUMN LastPurchaseDate;
GO

-- Drop ReorderQuantity
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'ReorderQuantity')
    ALTER TABLE Masters_Materials DROP COLUMN ReorderQuantity;
GO

-- Drop PreferredSupplierId
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'PreferredSupplierId')
    ALTER TABLE Masters_Materials DROP COLUMN PreferredSupplierId;
GO

-- Drop PreferredSupplierName
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'PreferredSupplierName')
    ALTER TABLE Masters_Materials DROP COLUMN PreferredSupplierName;
GO

-- Drop StorageLocation
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'StorageLocation')
    ALTER TABLE Masters_Materials DROP COLUMN StorageLocation;
GO

-- Drop StorageConditions
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'StorageConditions')
    ALTER TABLE Masters_Materials DROP COLUMN StorageConditions;
GO

-- Drop Status
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'Status')
    ALTER TABLE Masters_Materials DROP COLUMN Status;
GO

-- Drop Remarks
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'Remarks')
    ALTER TABLE Masters_Materials DROP COLUMN Remarks;
GO

-- Make required columns NOT NULL
PRINT 'Setting required columns...';
GO

-- Set defaults before making NOT NULL
UPDATE Masters_Materials SET MaterialName = 'Unknown' WHERE MaterialName IS NULL OR MaterialName = '';
UPDATE Masters_Materials SET Grade = 'EN8' WHERE Grade IS NULL OR Grade = '';
UPDATE Masters_Materials SET Shape = 'Rod' WHERE Shape IS NULL OR Shape = '';
UPDATE Masters_Materials SET Diameter = 0 WHERE Diameter IS NULL;
UPDATE Masters_Materials SET LengthInMM = 0 WHERE LengthInMM IS NULL;
UPDATE Masters_Materials SET Density = 0 WHERE Density IS NULL;
UPDATE Masters_Materials SET WeightKG = 0 WHERE WeightKG IS NULL;
UPDATE Masters_Materials SET StockQty = 0 WHERE StockQty IS NULL;
UPDATE Masters_Materials SET MinStockLevel = 0 WHERE MinStockLevel IS NULL;
UPDATE Masters_Materials SET CreatedAt = GETDATE() WHERE CreatedAt IS NULL;
UPDATE Masters_Materials SET UpdatedAt = GETDATE() WHERE UpdatedAt IS NULL;
GO

-- Alter columns to NOT NULL
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'MaterialName' AND is_nullable = 1)
    ALTER TABLE Masters_Materials ALTER COLUMN MaterialName NVARCHAR(200) NOT NULL;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'Grade' AND is_nullable = 1)
    ALTER TABLE Masters_Materials ALTER COLUMN Grade NVARCHAR(50) NOT NULL;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'Shape' AND is_nullable = 1)
    ALTER TABLE Masters_Materials ALTER COLUMN Shape NVARCHAR(50) NOT NULL;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'Diameter' AND is_nullable = 1)
    ALTER TABLE Masters_Materials ALTER COLUMN Diameter DECIMAL(18,2) NOT NULL;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'LengthInMM' AND is_nullable = 1)
    ALTER TABLE Masters_Materials ALTER COLUMN LengthInMM DECIMAL(18,2) NOT NULL;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'Density' AND is_nullable = 1)
    ALTER TABLE Masters_Materials ALTER COLUMN Density DECIMAL(18,4) NOT NULL;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'WeightKG' AND is_nullable = 1)
    ALTER TABLE Masters_Materials ALTER COLUMN WeightKG DECIMAL(18,3) NOT NULL;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'StockQty' AND is_nullable = 1)
    ALTER TABLE Masters_Materials ALTER COLUMN StockQty DECIMAL(18,2) NOT NULL;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'MinStockLevel' AND is_nullable = 1)
    ALTER TABLE Masters_Materials ALTER COLUMN MinStockLevel DECIMAL(18,2) NOT NULL;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'CreatedAt' AND is_nullable = 1)
    ALTER TABLE Masters_Materials ALTER COLUMN CreatedAt DATETIME NOT NULL;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Materials') AND name = 'UpdatedAt' AND is_nullable = 1)
    ALTER TABLE Masters_Materials ALTER COLUMN UpdatedAt DATETIME NOT NULL;
GO

PRINT '';
PRINT '=== Final Materials structure ===';
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Masters_Materials'
ORDER BY ORDINAL_POSITION;
GO

PRINT '';
PRINT 'Materials table fixed to match frontend!';
PRINT 'Final fields: Id, MaterialName, Grade, Shape, Diameter, LengthInMM, Density, WeightKG, StockQty, MinStockLevel, CreatedAt, UpdatedAt';
GO
