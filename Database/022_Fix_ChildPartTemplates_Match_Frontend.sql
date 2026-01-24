-- Fix Masters_ChildPartTemplates to match frontend exactly
USE MultiHitechERP;
GO

PRINT 'Fixing Masters_ChildPartTemplates to match frontend...';
GO

-- Add missing columns
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ChildPartTemplates') AND name = 'TemplateCode')
BEGIN
    ALTER TABLE Masters_ChildPartTemplates ADD TemplateCode NVARCHAR(50) NULL;
    PRINT 'Added TemplateCode column';
END
GO

-- Set default values for TemplateCode
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ChildPartTemplates') AND name = 'TemplateCode')
BEGIN
    UPDATE Masters_ChildPartTemplates
    SET TemplateCode = 'CPT-' + RIGHT('00000' + CAST(Id AS VARCHAR), 5)
    WHERE TemplateCode IS NULL OR TemplateCode = '';

    ALTER TABLE Masters_ChildPartTemplates ALTER COLUMN TemplateCode NVARCHAR(50) NOT NULL;
    PRINT 'Set TemplateCode values and made NOT NULL';
END
GO

-- Add RollerType
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ChildPartTemplates') AND name = 'RollerType')
BEGIN
    ALTER TABLE Masters_ChildPartTemplates ADD RollerType NVARCHAR(50) NULL;
    PRINT 'Added RollerType column';
END
GO

-- Add DrawingNumber
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ChildPartTemplates') AND name = 'DrawingNumber')
BEGIN
    ALTER TABLE Masters_ChildPartTemplates ADD DrawingNumber NVARCHAR(100) NULL;
    PRINT 'Added DrawingNumber column';
END
GO

-- Add DrawingRevision
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ChildPartTemplates') AND name = 'DrawingRevision')
BEGIN
    ALTER TABLE Masters_ChildPartTemplates ADD DrawingRevision NVARCHAR(50) NULL;
    PRINT 'Added DrawingRevision column';
END
GO

-- Add DimensionUnit
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ChildPartTemplates') AND name = 'DimensionUnit')
BEGIN
    ALTER TABLE Masters_ChildPartTemplates ADD DimensionUnit NVARCHAR(20) NOT NULL DEFAULT 'mm';
    PRINT 'Added DimensionUnit column';
END
GO

-- Add TotalStandardTimeHours
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ChildPartTemplates') AND name = 'TotalStandardTimeHours')
BEGIN
    ALTER TABLE Masters_ChildPartTemplates ADD TotalStandardTimeHours DECIMAL(18,2) NOT NULL DEFAULT 0;
    PRINT 'Added TotalStandardTimeHours column';
END
GO

-- Add TechnicalNotes
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ChildPartTemplates') AND name = 'TechnicalNotes')
BEGIN
    ALTER TABLE Masters_ChildPartTemplates ADD TechnicalNotes NVARCHAR(MAX) NULL;
    PRINT 'Added TechnicalNotes column';
END
GO

-- Add QualityCheckpoints (stored as JSON string)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ChildPartTemplates') AND name = 'QualityCheckpoints')
BEGIN
    ALTER TABLE Masters_ChildPartTemplates ADD QualityCheckpoints NVARCHAR(MAX) NULL;
    PRINT 'Added QualityCheckpoints column';
END
GO

-- Drop default constraints before dropping columns
DECLARE @ConstraintName nvarchar(200)

-- Status
SELECT @ConstraintName = name FROM sys.default_constraints
WHERE parent_object_id = OBJECT_ID('Masters_ChildPartTemplates')
AND parent_column_id = (SELECT column_id FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ChildPartTemplates') AND name = 'Status')
IF @ConstraintName IS NOT NULL
    EXEC('ALTER TABLE Masters_ChildPartTemplates DROP CONSTRAINT ' + @ConstraintName)

-- IsDefault
SELECT @ConstraintName = name FROM sys.default_constraints
WHERE parent_object_id = OBJECT_ID('Masters_ChildPartTemplates')
AND parent_column_id = (SELECT column_id FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ChildPartTemplates') AND name = 'IsDefault')
IF @ConstraintName IS NOT NULL
    EXEC('ALTER TABLE Masters_ChildPartTemplates DROP CONSTRAINT ' + @ConstraintName)
GO

-- Drop foreign key constraint on ProcessTemplateId before dropping column
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ChildPartTemplate_ProcessTemplate')
    ALTER TABLE Masters_ChildPartTemplates DROP CONSTRAINT FK_ChildPartTemplate_ProcessTemplate;

-- Drop index on ProcessTemplateId before dropping column
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ChildPartTemplates_ProcessTemplateId')
    DROP INDEX IX_ChildPartTemplates_ProcessTemplateId ON Masters_ChildPartTemplates;
GO

-- Drop columns that don't exist in frontend
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ChildPartTemplates') AND name = 'Category')
    ALTER TABLE Masters_ChildPartTemplates DROP COLUMN Category;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ChildPartTemplates') AND name = 'Width')
    ALTER TABLE Masters_ChildPartTemplates DROP COLUMN Width;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ChildPartTemplates') AND name = 'Height')
    ALTER TABLE Masters_ChildPartTemplates DROP COLUMN Height;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ChildPartTemplates') AND name = 'MaterialType')
    ALTER TABLE Masters_ChildPartTemplates DROP COLUMN MaterialType;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ChildPartTemplates') AND name = 'MaterialGrade')
    ALTER TABLE Masters_ChildPartTemplates DROP COLUMN MaterialGrade;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ChildPartTemplates') AND name = 'ProcessTemplateId')
    ALTER TABLE Masters_ChildPartTemplates DROP COLUMN ProcessTemplateId;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ChildPartTemplates') AND name = 'ProcessTemplateName')
    ALTER TABLE Masters_ChildPartTemplates DROP COLUMN ProcessTemplateName;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ChildPartTemplates') AND name = 'EstimatedCost')
    ALTER TABLE Masters_ChildPartTemplates DROP COLUMN EstimatedCost;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ChildPartTemplates') AND name = 'EstimatedLeadTimeDays')
    ALTER TABLE Masters_ChildPartTemplates DROP COLUMN EstimatedLeadTimeDays;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ChildPartTemplates') AND name = 'EstimatedWeight')
    ALTER TABLE Masters_ChildPartTemplates DROP COLUMN EstimatedWeight;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ChildPartTemplates') AND name = 'Status')
    ALTER TABLE Masters_ChildPartTemplates DROP COLUMN Status;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ChildPartTemplates') AND name = 'IsDefault')
    ALTER TABLE Masters_ChildPartTemplates DROP COLUMN IsDefault;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ChildPartTemplates') AND name = 'ApprovedBy')
    ALTER TABLE Masters_ChildPartTemplates DROP COLUMN ApprovedBy;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ChildPartTemplates') AND name = 'ApprovalDate')
    ALTER TABLE Masters_ChildPartTemplates DROP COLUMN ApprovalDate;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ChildPartTemplates') AND name = 'Remarks')
    ALTER TABLE Masters_ChildPartTemplates DROP COLUMN Remarks;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ChildPartTemplates') AND name = 'UpdatedBy')
    ALTER TABLE Masters_ChildPartTemplates DROP COLUMN UpdatedBy;

PRINT 'Dropped extra columns';
GO

-- Create ChildPartTemplateMaterialRequirements table
IF OBJECT_ID('Masters_ChildPartTemplateMaterialRequirements', 'U') IS NOT NULL
    DROP TABLE Masters_ChildPartTemplateMaterialRequirements;
GO

CREATE TABLE Masters_ChildPartTemplateMaterialRequirements (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ChildPartTemplateId INT NOT NULL,
    RawMaterialId INT NULL,
    RawMaterialName NVARCHAR(200) NOT NULL,
    MaterialGrade NVARCHAR(100) NOT NULL,
    QuantityRequired DECIMAL(18,3) NOT NULL,
    Unit NVARCHAR(20) NOT NULL,
    WastagePercent DECIMAL(5,2) NOT NULL DEFAULT 0,

    CONSTRAINT FK_ChildPartTemplateMaterialReq_Template
        FOREIGN KEY (ChildPartTemplateId)
        REFERENCES Masters_ChildPartTemplates(Id)
        ON DELETE CASCADE
);
GO

CREATE INDEX IX_ChildPartTemplateMaterialReq_TemplateId
ON Masters_ChildPartTemplateMaterialRequirements(ChildPartTemplateId);
GO

-- Create ChildPartTemplateProcessSteps table
IF OBJECT_ID('Masters_ChildPartTemplateProcessSteps', 'U') IS NOT NULL
    DROP TABLE Masters_ChildPartTemplateProcessSteps;
GO

CREATE TABLE Masters_ChildPartTemplateProcessSteps (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ChildPartTemplateId INT NOT NULL,
    ProcessId INT NULL,
    ProcessName NVARCHAR(200) NOT NULL,
    StepNumber INT NOT NULL,
    MachineName NVARCHAR(200) NULL,
    StandardTimeHours DECIMAL(18,2) NOT NULL,
    RestTimeHours DECIMAL(18,2) NULL,
    Description NVARCHAR(MAX) NULL,

    CONSTRAINT FK_ChildPartTemplateProcessStep_Template
        FOREIGN KEY (ChildPartTemplateId)
        REFERENCES Masters_ChildPartTemplates(Id)
        ON DELETE CASCADE
);
GO

CREATE INDEX IX_ChildPartTemplateProcessStep_TemplateId
ON Masters_ChildPartTemplateProcessSteps(ChildPartTemplateId);
GO

PRINT '';
PRINT '=== Final ChildPartTemplates structure ===';
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Masters_ChildPartTemplates'
ORDER BY ORDINAL_POSITION;
GO

PRINT '';
PRINT 'ChildPartTemplates fixed to match frontend!';
GO
