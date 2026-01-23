-- Fix Masters_ProductTemplates to match frontend exactly
USE MultiHitechERP;
GO

PRINT 'Fixing Masters_ProductTemplates to match frontend...';
GO

-- Add missing TemplateCode column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProductTemplates') AND name = 'TemplateCode')
BEGIN
    ALTER TABLE Masters_ProductTemplates ADD TemplateCode NVARCHAR(50) NULL;
    PRINT 'Added TemplateCode column';
END
GO

-- Set default values for existing rows (separate batch)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProductTemplates') AND name = 'TemplateCode')
BEGIN
    UPDATE Masters_ProductTemplates
    SET TemplateCode = 'TPL-' + RIGHT('00000' + CAST(Id AS VARCHAR), 5)
    WHERE TemplateCode IS NULL OR TemplateCode = '';

    ALTER TABLE Masters_ProductTemplates ALTER COLUMN TemplateCode NVARCHAR(50) NOT NULL;
    PRINT 'Set TemplateCode values and made NOT NULL';
END
GO

-- Drop default constraint on Status before dropping column
DECLARE @ConstraintName nvarchar(200)
SELECT @ConstraintName = name FROM sys.default_constraints
WHERE parent_object_id = OBJECT_ID('Masters_ProductTemplates')
AND parent_column_id = (SELECT column_id FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProductTemplates') AND name = 'Status')

IF @ConstraintName IS NOT NULL
    EXEC('ALTER TABLE Masters_ProductTemplates DROP CONSTRAINT ' + @ConstraintName)
GO

-- Drop default constraint on IsDefault before dropping column
DECLARE @ConstraintName nvarchar(200)
SELECT @ConstraintName = name FROM sys.default_constraints
WHERE parent_object_id = OBJECT_ID('Masters_ProductTemplates')
AND parent_column_id = (SELECT column_id FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProductTemplates') AND name = 'IsDefault')

IF @ConstraintName IS NOT NULL
    EXEC('ALTER TABLE Masters_ProductTemplates DROP CONSTRAINT ' + @ConstraintName)
GO

-- Drop columns that don't exist in frontend
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProductTemplates') AND name = 'ProductType')
    ALTER TABLE Masters_ProductTemplates DROP COLUMN ProductType;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProductTemplates') AND name = 'Category')
    ALTER TABLE Masters_ProductTemplates DROP COLUMN Category;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProductTemplates') AND name = 'EstimatedLeadTimeDays')
    ALTER TABLE Masters_ProductTemplates DROP COLUMN EstimatedLeadTimeDays;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProductTemplates') AND name = 'StandardCost')
    ALTER TABLE Masters_ProductTemplates DROP COLUMN StandardCost;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProductTemplates') AND name = 'ProcessTemplateName')
    ALTER TABLE Masters_ProductTemplates DROP COLUMN ProcessTemplateName;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProductTemplates') AND name = 'Status')
    ALTER TABLE Masters_ProductTemplates DROP COLUMN Status;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProductTemplates') AND name = 'IsDefault')
    ALTER TABLE Masters_ProductTemplates DROP COLUMN IsDefault;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProductTemplates') AND name = 'ApprovedBy')
    ALTER TABLE Masters_ProductTemplates DROP COLUMN ApprovedBy;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProductTemplates') AND name = 'ApprovalDate')
    ALTER TABLE Masters_ProductTemplates DROP COLUMN ApprovalDate;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProductTemplates') AND name = 'Remarks')
    ALTER TABLE Masters_ProductTemplates DROP COLUMN Remarks;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProductTemplates') AND name = 'UpdatedBy')
    ALTER TABLE Masters_ProductTemplates DROP COLUMN UpdatedBy;
GO

-- Create ProductTemplateChildParts table
IF OBJECT_ID('Masters_ProductTemplateChildParts', 'U') IS NOT NULL
    DROP TABLE Masters_ProductTemplateChildParts;
GO

CREATE TABLE Masters_ProductTemplateChildParts (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProductTemplateId INT NOT NULL,
    ChildPartName NVARCHAR(200) NOT NULL,
    ChildPartCode NVARCHAR(50) NULL,
    Quantity DECIMAL(18,3) NOT NULL,
    Unit NVARCHAR(20) NOT NULL,
    Notes NVARCHAR(500) NULL,
    SequenceNo INT NOT NULL,
    ChildPartTemplateId INT NULL,

    CONSTRAINT FK_ProductTemplateChildParts_ProductTemplate
        FOREIGN KEY (ProductTemplateId)
        REFERENCES Masters_ProductTemplates(Id)
        ON DELETE CASCADE
);
GO

CREATE INDEX IX_ProductTemplateChildParts_ProductTemplateId
ON Masters_ProductTemplateChildParts(ProductTemplateId);
GO

PRINT '';
PRINT '=== Final ProductTemplates structure ===';
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Masters_ProductTemplates'
ORDER BY ORDINAL_POSITION;
GO

PRINT '';
PRINT 'ProductTemplates fixed to match frontend!';
GO
