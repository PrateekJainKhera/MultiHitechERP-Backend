-- Add missing columns to Masters_ProcessTemplates table
USE MultiHitechERP;
GO

PRINT 'Adding missing columns to Masters_ProcessTemplates table...';
GO

-- Add ProductId
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplates') AND name = 'ProductId')
BEGIN
    ALTER TABLE Masters_ProcessTemplates ADD ProductId INT NULL;
    PRINT 'Added ProductId column';
END

-- Add ProductCode
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplates') AND name = 'ProductCode')
BEGIN
    ALTER TABLE Masters_ProcessTemplates ADD ProductCode NVARCHAR(50) NULL;
    PRINT 'Added ProductCode column';
END

-- Add ProductName
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplates') AND name = 'ProductName')
BEGIN
    ALTER TABLE Masters_ProcessTemplates ADD ProductName NVARCHAR(200) NULL;
    PRINT 'Added ProductName column';
END

-- Add ChildPartId
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplates') AND name = 'ChildPartId')
BEGIN
    ALTER TABLE Masters_ProcessTemplates ADD ChildPartId INT NULL;
    PRINT 'Added ChildPartId column';
END

-- Add ChildPartName
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplates') AND name = 'ChildPartName')
BEGIN
    ALTER TABLE Masters_ProcessTemplates ADD ChildPartName NVARCHAR(200) NULL;
    PRINT 'Added ChildPartName column';
END

-- Add TemplateType (rename from ProductType if needed)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplates') AND name = 'TemplateType')
BEGIN
    ALTER TABLE Masters_ProcessTemplates ADD TemplateType NVARCHAR(50) NULL DEFAULT 'Standard';
    -- Copy from ProductType if exists
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplates') AND name = 'ProductType')
    BEGIN
        EXEC sp_executesql N'UPDATE Masters_ProcessTemplates SET TemplateType = ProductType';
    END
    PRINT 'Added TemplateType column';
END

-- Add Status
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplates') AND name = 'Status')
BEGIN
    ALTER TABLE Masters_ProcessTemplates ADD Status NVARCHAR(50) NULL DEFAULT 'Active';
    PRINT 'Added Status column';
END

-- Add IsDefault
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplates') AND name = 'IsDefault')
BEGIN
    ALTER TABLE Masters_ProcessTemplates ADD IsDefault BIT NULL DEFAULT 0;
    PRINT 'Added IsDefault column';
END

-- Add ApprovedBy
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplates') AND name = 'ApprovedBy')
BEGIN
    ALTER TABLE Masters_ProcessTemplates ADD ApprovedBy NVARCHAR(100) NULL;
    PRINT 'Added ApprovedBy column';
END

-- Add ApprovalDate
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplates') AND name = 'ApprovalDate')
BEGIN
    ALTER TABLE Masters_ProcessTemplates ADD ApprovalDate DATETIME NULL;
    PRINT 'Added ApprovalDate column';
END

-- Add Remarks
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplates') AND name = 'Remarks')
BEGIN
    ALTER TABLE Masters_ProcessTemplates ADD Remarks NVARCHAR(500) NULL;
    PRINT 'Added Remarks column';
END

-- Add UpdatedAt
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplates') AND name = 'UpdatedAt')
BEGIN
    ALTER TABLE Masters_ProcessTemplates ADD UpdatedAt DATETIME NULL;
    PRINT 'Added UpdatedAt column';
END

-- Add UpdatedBy
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplates') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE Masters_ProcessTemplates ADD UpdatedBy NVARCHAR(100) NULL;
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
WHERE TABLE_NAME = 'Masters_ProcessTemplates'
ORDER BY ORDINAL_POSITION;
GO

PRINT '';
PRINT 'All columns added successfully to Masters_ProcessTemplates table!';
GO
