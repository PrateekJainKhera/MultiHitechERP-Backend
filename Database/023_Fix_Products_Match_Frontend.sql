-- Fix Masters_Products to match frontend exactly
USE MultiHitechERP;
GO

PRINT 'Fixing Masters_Products to match frontend...';
GO

-- Add missing columns
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Products') AND name = 'RevisionDate')
BEGIN
    ALTER TABLE Masters_Products ADD RevisionDate NVARCHAR(50) NULL;
    PRINT 'Added RevisionDate column';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Products') AND name = 'NumberOfTeeth')
BEGIN
    ALTER TABLE Masters_Products ADD NumberOfTeeth INT NULL;
    PRINT 'Added NumberOfTeeth column';
END
GO

-- Drop foreign key constraints before dropping columns
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Products_Customer')
    ALTER TABLE Masters_Products DROP CONSTRAINT FK_Products_Customer;

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Products_Customers')
    ALTER TABLE Masters_Products DROP CONSTRAINT FK_Products_Customers;

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Products_ProcessTemplate')
    ALTER TABLE Masters_Products DROP CONSTRAINT FK_Products_ProcessTemplate;

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Products_ProductTemplate')
    ALTER TABLE Masters_Products DROP CONSTRAINT FK_Products_ProductTemplate;

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Products_Drawing')
    ALTER TABLE Masters_Products DROP CONSTRAINT FK_Products_Drawing;
GO

-- Drop indexes before dropping columns
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Products_CustomerId')
    DROP INDEX IX_Products_CustomerId ON Masters_Products;

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Products_ProcessTemplateId')
    DROP INDEX IX_Products_ProcessTemplateId ON Masters_Products;

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Products_ProductTemplateId')
    DROP INDEX IX_Products_ProductTemplateId ON Masters_Products;
GO

-- Drop check constraints before dropping columns
IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CHK_Products_Weight')
    ALTER TABLE Masters_Products DROP CONSTRAINT CHK_Products_Weight;
GO

-- Drop default constraints before dropping columns
DECLARE @ConstraintName nvarchar(200)

-- UOM
SELECT @ConstraintName = name FROM sys.default_constraints
WHERE parent_object_id = OBJECT_ID('Masters_Products')
AND parent_column_id = (SELECT column_id FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Products') AND name = 'UOM')
IF @ConstraintName IS NOT NULL
    EXEC('ALTER TABLE Masters_Products DROP CONSTRAINT ' + @ConstraintName)

-- IsActive
SELECT @ConstraintName = name FROM sys.default_constraints
WHERE parent_object_id = OBJECT_ID('Masters_Products')
AND parent_column_id = (SELECT column_id FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Products') AND name = 'IsActive')
IF @ConstraintName IS NOT NULL
    EXEC('ALTER TABLE Masters_Products DROP CONSTRAINT ' + @ConstraintName)
GO

-- Drop extra columns that don't exist in frontend
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Products') AND name = 'CustomerId')
    ALTER TABLE Masters_Products DROP COLUMN CustomerId;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Products') AND name = 'Weight')
    ALTER TABLE Masters_Products DROP COLUMN Weight;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Products') AND name = 'DrawingId')
    ALTER TABLE Masters_Products DROP COLUMN DrawingId;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Products') AND name = 'ProductTemplateId')
    ALTER TABLE Masters_Products DROP COLUMN ProductTemplateId;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Products') AND name = 'StandardCost')
    ALTER TABLE Masters_Products DROP COLUMN StandardCost;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Products') AND name = 'SellingPrice')
    ALTER TABLE Masters_Products DROP COLUMN SellingPrice;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Products') AND name = 'StandardLeadTimeDays')
    ALTER TABLE Masters_Products DROP COLUMN StandardLeadTimeDays;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Products') AND name = 'MinOrderQuantity')
    ALTER TABLE Masters_Products DROP COLUMN MinOrderQuantity;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Products') AND name = 'Category')
    ALTER TABLE Masters_Products DROP COLUMN Category;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Products') AND name = 'ProductType')
    ALTER TABLE Masters_Products DROP COLUMN ProductType;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Products') AND name = 'Description')
    ALTER TABLE Masters_Products DROP COLUMN Description;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Products') AND name = 'HSNCode')
    ALTER TABLE Masters_Products DROP COLUMN HSNCode;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Products') AND name = 'UOM')
    ALTER TABLE Masters_Products DROP COLUMN UOM;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Products') AND name = 'Remarks')
    ALTER TABLE Masters_Products DROP COLUMN Remarks;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Products') AND name = 'IsActive')
    ALTER TABLE Masters_Products DROP COLUMN IsActive;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Products') AND name = 'UpdatedBy')
    ALTER TABLE Masters_Products DROP COLUMN UpdatedBy;

PRINT 'Dropped extra columns';
GO

-- Set default values for NULL columns before making them NOT NULL
UPDATE Masters_Products SET Diameter = 0 WHERE Diameter IS NULL;
UPDATE Masters_Products SET Length = 0 WHERE Length IS NULL;
UPDATE Masters_Products SET CreatedAt = GETDATE() WHERE CreatedAt IS NULL;
UPDATE Masters_Products SET UpdatedAt = GETDATE() WHERE UpdatedAt IS NULL;
UPDATE Masters_Products SET CreatedBy = 'System' WHERE CreatedBy IS NULL;
UPDATE Masters_Products SET ProcessTemplateId = 1 WHERE ProcessTemplateId IS NULL;
GO

-- Make required columns NOT NULL if they aren't already
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Products') AND name = 'Diameter' AND is_nullable = 1)
    ALTER TABLE Masters_Products ALTER COLUMN Diameter DECIMAL(18,2) NOT NULL;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Products') AND name = 'Length' AND is_nullable = 1)
    ALTER TABLE Masters_Products ALTER COLUMN Length DECIMAL(18,2) NOT NULL;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Products') AND name = 'CreatedAt' AND is_nullable = 1)
    ALTER TABLE Masters_Products ALTER COLUMN CreatedAt DATETIME NOT NULL;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Products') AND name = 'UpdatedAt' AND is_nullable = 1)
    ALTER TABLE Masters_Products ALTER COLUMN UpdatedAt DATETIME NOT NULL;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Products') AND name = 'CreatedBy' AND is_nullable = 1)
    ALTER TABLE Masters_Products ALTER COLUMN CreatedBy NVARCHAR(100) NOT NULL;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Products') AND name = 'ProcessTemplateId' AND is_nullable = 1)
    ALTER TABLE Masters_Products ALTER COLUMN ProcessTemplateId INT NOT NULL;
GO

PRINT '';
PRINT '=== Final Products structure ===';
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Masters_Products'
ORDER BY ORDINAL_POSITION;
GO

PRINT '';
PRINT 'Products table fixed to match frontend!';
GO
