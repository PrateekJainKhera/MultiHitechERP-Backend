-- Add missing columns to Masters_ProductTemplates table
USE MultiHitechERP;
GO

PRINT 'Adding missing columns to Masters_ProductTemplates table...';
GO

-- Add ProductType (rename from RollerType if needed)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProductTemplates') AND name = 'ProductType')
BEGIN
    ALTER TABLE Masters_ProductTemplates ADD ProductType NVARCHAR(100) NULL;
    -- Copy from RollerType if exists
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProductTemplates') AND name = 'RollerType')
    BEGIN
        EXEC sp_executesql N'UPDATE Masters_ProductTemplates SET ProductType = RollerType WHERE RollerType IS NOT NULL';
    END
    PRINT 'Added ProductType column';
END

-- Add EstimatedLeadTimeDays
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProductTemplates') AND name = 'EstimatedLeadTimeDays')
BEGIN
    ALTER TABLE Masters_ProductTemplates ADD EstimatedLeadTimeDays INT NULL;
    PRINT 'Added EstimatedLeadTimeDays column';
END

-- Add StandardCost
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProductTemplates') AND name = 'StandardCost')
BEGIN
    ALTER TABLE Masters_ProductTemplates ADD StandardCost DECIMAL(18,2) NULL;
    PRINT 'Added StandardCost column';
END

-- Add ProcessTemplateName for denormalization
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProductTemplates') AND name = 'ProcessTemplateName')
BEGIN
    ALTER TABLE Masters_ProductTemplates ADD ProcessTemplateName NVARCHAR(200) NULL;
    PRINT 'Added ProcessTemplateName column';
END

-- Add Category
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProductTemplates') AND name = 'Category')
BEGIN
    ALTER TABLE Masters_ProductTemplates ADD Category NVARCHAR(100) NULL;
    PRINT 'Added Category column';
END

-- Add Status
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProductTemplates') AND name = 'Status')
BEGIN
    ALTER TABLE Masters_ProductTemplates ADD Status NVARCHAR(50) NULL DEFAULT 'Active';
    PRINT 'Added Status column';
END

-- Add IsDefault
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProductTemplates') AND name = 'IsDefault')
BEGIN
    ALTER TABLE Masters_ProductTemplates ADD IsDefault BIT NULL DEFAULT 0;
    PRINT 'Added IsDefault column';
END

-- Add ApprovedBy
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProductTemplates') AND name = 'ApprovedBy')
BEGIN
    ALTER TABLE Masters_ProductTemplates ADD ApprovedBy NVARCHAR(100) NULL;
    PRINT 'Added ApprovedBy column';
END

-- Add ApprovalDate
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProductTemplates') AND name = 'ApprovalDate')
BEGIN
    ALTER TABLE Masters_ProductTemplates ADD ApprovalDate DATETIME NULL;
    PRINT 'Added ApprovalDate column';
END

-- Add Remarks
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProductTemplates') AND name = 'Remarks')
BEGIN
    ALTER TABLE Masters_ProductTemplates ADD Remarks NVARCHAR(500) NULL;
    PRINT 'Added Remarks column';
END

-- Add UpdatedAt
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProductTemplates') AND name = 'UpdatedAt')
BEGIN
    ALTER TABLE Masters_ProductTemplates ADD UpdatedAt DATETIME NULL;
    PRINT 'Added UpdatedAt column';
END

-- Add UpdatedBy
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProductTemplates') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE Masters_ProductTemplates ADD UpdatedBy NVARCHAR(100) NULL;
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
WHERE TABLE_NAME = 'Masters_ProductTemplates'
ORDER BY ORDINAL_POSITION;
GO

PRINT '';
PRINT 'All columns added successfully to Masters_ProductTemplates table!';
GO
