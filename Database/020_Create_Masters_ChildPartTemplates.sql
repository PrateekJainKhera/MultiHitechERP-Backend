-- Create Masters_ChildPartTemplates table with INT IDs
USE MultiHitechERP;
GO

PRINT 'Creating Masters_ChildPartTemplates table...';
GO

-- Drop table if exists (for clean recreation)
IF OBJECT_ID('Masters_ChildPartTemplates', 'U') IS NOT NULL
BEGIN
    DROP TABLE Masters_ChildPartTemplates;
    PRINT 'Dropped existing Masters_ChildPartTemplates table';
END
GO

-- Create the table
CREATE TABLE Masters_ChildPartTemplates (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TemplateName NVARCHAR(200) NOT NULL,
    ChildPartType NVARCHAR(50) NOT NULL,

    -- Description
    Description NVARCHAR(500) NULL,
    Category NVARCHAR(100) NULL,

    -- Dimensions
    Length DECIMAL(10,2) NULL,
    Diameter DECIMAL(10,2) NULL,
    InnerDiameter DECIMAL(10,2) NULL,
    OuterDiameter DECIMAL(10,2) NULL,
    Thickness DECIMAL(10,2) NULL,
    Width DECIMAL(10,2) NULL,
    Height DECIMAL(10,2) NULL,

    -- Material & Process
    MaterialType NVARCHAR(100) NULL,
    MaterialGrade NVARCHAR(50) NULL,
    ProcessTemplateId INT NULL,
    ProcessTemplateName NVARCHAR(200) NULL,

    -- Estimates
    EstimatedCost DECIMAL(18,2) NULL,
    EstimatedLeadTimeDays INT NULL,
    EstimatedWeight DECIMAL(10,3) NULL,

    -- Status
    IsActive BIT NOT NULL DEFAULT 1,
    Status NVARCHAR(50) NULL DEFAULT 'Active',
    IsDefault BIT NOT NULL DEFAULT 0,

    -- Approval
    ApprovedBy NVARCHAR(100) NULL,
    ApprovalDate DATETIME NULL,

    Remarks NVARCHAR(500) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedBy NVARCHAR(100) NULL,
    UpdatedAt DATETIME NULL,
    UpdatedBy NVARCHAR(100) NULL,

    -- Foreign key to Masters_ProcessTemplates
    CONSTRAINT FK_ChildPartTemplate_ProcessTemplate
        FOREIGN KEY (ProcessTemplateId)
        REFERENCES Masters_ProcessTemplates(Id)
        ON DELETE SET NULL
);
GO

-- Create index on ProcessTemplateId for performance
CREATE INDEX IX_ChildPartTemplates_ProcessTemplateId
ON Masters_ChildPartTemplates(ProcessTemplateId);
GO

-- Create index on ChildPartType for lookups
CREATE INDEX IX_ChildPartTemplates_ChildPartType
ON Masters_ChildPartTemplates(ChildPartType);
GO

PRINT '';
PRINT '=== Verification: Table structure ===';
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    NUMERIC_PRECISION,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Masters_ChildPartTemplates'
ORDER BY ORDINAL_POSITION;
GO

PRINT '';
PRINT 'Masters_ChildPartTemplates table created successfully!';
GO
