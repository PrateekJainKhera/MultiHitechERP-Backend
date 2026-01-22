-- Add missing columns to Masters_Materials safely without dropping table
USE MultiHitechERP;
GO

-- Check if we're working with RawMaterials or Materials table
DECLARE @TableName NVARCHAR(128);

IF OBJECT_ID('Masters_Materials', 'U') IS NOT NULL
    SET @TableName = 'Masters_Materials';
ELSE IF OBJECT_ID('Masters_RawMaterials', 'U') IS NOT NULL
    SET @TableName = 'Masters_RawMaterials';
ELSE
BEGIN
    PRINT 'ERROR: Neither Masters_Materials nor Masters_RawMaterials table exists!';
    RETURN;
END

PRINT 'Working with table: ' + @TableName;
GO

-- Add Category
IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID('Masters_RawMaterials')
    AND name = 'Category'
)
BEGIN
    ALTER TABLE Masters_RawMaterials ADD Category NVARCHAR(100) NULL;
    PRINT 'Added Category column';
END
ELSE
    PRINT 'Category column already exists';

-- Add SubCategory
IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID('Masters_RawMaterials')
    AND name = 'SubCategory'
)
BEGIN
    ALTER TABLE Masters_RawMaterials ADD SubCategory NVARCHAR(100) NULL;
    PRINT 'Added SubCategory column';
END
ELSE
    PRINT 'SubCategory column already exists';

-- Add Description
IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID('Masters_RawMaterials')
    AND name = 'Description'
)
BEGIN
    ALTER TABLE Masters_RawMaterials ADD Description NVARCHAR(500) NULL;
    PRINT 'Added Description column';
END
ELSE
    PRINT 'Description column already exists';

-- Add HSNCode
IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID('Masters_RawMaterials')
    AND name = 'HSNCode'
)
BEGIN
    ALTER TABLE Masters_RawMaterials ADD HSNCode NVARCHAR(20) NULL;
    PRINT 'Added HSNCode column';
END
ELSE
    PRINT 'HSNCode column already exists';

-- Add Dimensions
IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID('Masters_RawMaterials')
    AND name = 'StandardLength'
)
BEGIN
    ALTER TABLE Masters_RawMaterials ADD StandardLength DECIMAL(18,3) NULL;
    PRINT 'Added StandardLength column';
END
ELSE
    PRINT 'StandardLength column already exists';

IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID('Masters_RawMaterials')
    AND name = 'Diameter'
)
BEGIN
    ALTER TABLE Masters_RawMaterials ADD Diameter DECIMAL(18,3) NULL;
    PRINT 'Added Diameter column';
END
ELSE
    PRINT 'Diameter column already exists';

IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID('Masters_RawMaterials')
    AND name = 'Thickness'
)
BEGIN
    ALTER TABLE Masters_RawMaterials ADD Thickness DECIMAL(18,3) NULL;
    PRINT 'Added Thickness column';
END
ELSE
    PRINT 'Thickness column already exists';

IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID('Masters_RawMaterials')
    AND name = 'Width'
)
BEGIN
    ALTER TABLE Masters_RawMaterials ADD Width DECIMAL(18,3) NULL;
    PRINT 'Added Width column';
END
ELSE
    PRINT 'Width column already exists';

-- Add Unit of Measure
IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID('Masters_RawMaterials')
    AND name = 'PrimaryUOM'
)
BEGIN
    ALTER TABLE Masters_RawMaterials ADD PrimaryUOM NVARCHAR(20) NULL;
    PRINT 'Added PrimaryUOM column';
END
ELSE
    PRINT 'PrimaryUOM column already exists';

IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID('Masters_RawMaterials')
    AND name = 'SecondaryUOM'
)
BEGIN
    ALTER TABLE Masters_RawMaterials ADD SecondaryUOM NVARCHAR(20) NULL;
    PRINT 'Added SecondaryUOM column';
END
ELSE
    PRINT 'SecondaryUOM column already exists';

IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID('Masters_RawMaterials')
    AND name = 'ConversionFactor'
)
BEGIN
    ALTER TABLE Masters_RawMaterials ADD ConversionFactor DECIMAL(18,6) NULL;
    PRINT 'Added ConversionFactor column';
END
ELSE
    PRINT 'ConversionFactor column already exists';

-- Add Weight columns
IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID('Masters_RawMaterials')
    AND name = 'WeightPerMeter'
)
BEGIN
    ALTER TABLE Masters_RawMaterials ADD WeightPerMeter DECIMAL(18,4) NULL;
    PRINT 'Added WeightPerMeter column';
END
ELSE
    PRINT 'WeightPerMeter column already exists';

IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID('Masters_RawMaterials')
    AND name = 'WeightPerPiece'
)
BEGIN
    ALTER TABLE Masters_RawMaterials ADD WeightPerPiece DECIMAL(18,4) NULL;
    PRINT 'Added WeightPerPiece column';
END
ELSE
    PRINT 'WeightPerPiece column already exists';

IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID('Masters_RawMaterials')
    AND name = 'Density'
)
BEGIN
    ALTER TABLE Masters_RawMaterials ADD Density DECIMAL(18,4) NULL;
    PRINT 'Added Density column';
END
ELSE
    PRINT 'Density column already exists';

-- Add Pricing columns
IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID('Masters_RawMaterials')
    AND name = 'LastPurchasePrice'
)
BEGIN
    ALTER TABLE Masters_RawMaterials ADD LastPurchasePrice DECIMAL(18,2) NULL;
    PRINT 'Added LastPurchasePrice column';
END
ELSE
    PRINT 'LastPurchasePrice column already exists';

IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID('Masters_RawMaterials')
    AND name = 'LastPurchaseDate'
)
BEGIN
    ALTER TABLE Masters_RawMaterials ADD LastPurchaseDate DATETIME NULL;
    PRINT 'Added LastPurchaseDate column';
END
ELSE
    PRINT 'LastPurchaseDate column already exists';

-- Add Inventory Control
IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID('Masters_RawMaterials')
    AND name = 'ReorderQuantity'
)
BEGIN
    ALTER TABLE Masters_RawMaterials ADD ReorderQuantity DECIMAL(18,2) NULL;
    PRINT 'Added ReorderQuantity column';
END
ELSE
    PRINT 'ReorderQuantity column already exists';

-- Add Supplier columns
IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID('Masters_RawMaterials')
    AND name = 'PreferredSupplierId'
)
BEGIN
    ALTER TABLE Masters_RawMaterials ADD PreferredSupplierId INT NULL;
    PRINT 'Added PreferredSupplierId column';
END
ELSE
    PRINT 'PreferredSupplierId column already exists';

IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID('Masters_RawMaterials')
    AND name = 'PreferredSupplierName'
)
BEGIN
    ALTER TABLE Masters_RawMaterials ADD PreferredSupplierName NVARCHAR(200) NULL;
    PRINT 'Added PreferredSupplierName column';
END
ELSE
    PRINT 'PreferredSupplierName column already exists';

-- Add Storage columns
IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID('Masters_RawMaterials')
    AND name = 'StorageLocation'
)
BEGIN
    ALTER TABLE Masters_RawMaterials ADD StorageLocation NVARCHAR(100) NULL;
    PRINT 'Added StorageLocation column';
END
ELSE
    PRINT 'StorageLocation column already exists';

IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID('Masters_RawMaterials')
    AND name = 'StorageConditions'
)
BEGIN
    ALTER TABLE Masters_RawMaterials ADD StorageConditions NVARCHAR(200) NULL;
    PRINT 'Added StorageConditions column';
END
ELSE
    PRINT 'StorageConditions column already exists';

-- Add Status
IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID('Masters_RawMaterials')
    AND name = 'Status'
)
BEGIN
    ALTER TABLE Masters_RawMaterials ADD Status NVARCHAR(50) NULL;
    PRINT 'Added Status column';
END
ELSE
    PRINT 'Status column already exists';

-- Add Remarks if missing
IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID('Masters_RawMaterials')
    AND name = 'Remarks'
)
BEGIN
    ALTER TABLE Masters_RawMaterials ADD Remarks NVARCHAR(500) NULL;
    PRINT 'Added Remarks column';
END
ELSE
    PRINT 'Remarks column already exists';

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
WHERE TABLE_NAME = 'Masters_RawMaterials'
ORDER BY ORDINAL_POSITION;
GO
