-- Migration: Create Product Templates and BOM Tables
-- Purpose: Store product template definitions and their bill of materials
-- Date: 2026-02-02

USE MultiHitechERP;
GO

PRINT 'Creating Product Templates tables...';

-- =============================================
-- Product Templates Table
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Masters_ProductTemplates')
BEGIN
    CREATE TABLE Masters_ProductTemplates (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ProductTemplateName NVARCHAR(200) NOT NULL,
        ProductTemplateCode NVARCHAR(50) NOT NULL UNIQUE,
        RollerType NVARCHAR(50) NOT NULL, -- 'MAGNETIC' or 'PRINTING'
        ProcessTemplateId INT NULL, -- Reference to Assembly Process Template

        -- Specifications
        Description NVARCHAR(1000) NULL,
        DrawingNumber NVARCHAR(100) NULL,
        DrawingRevision NVARCHAR(20) NULL,

        -- Final Product Dimensions
        Length DECIMAL(10, 2) NULL,
        Diameter DECIMAL(10, 2) NULL,
        CoreDiameter DECIMAL(10, 2) NULL,
        ShaftDiameter DECIMAL(10, 2) NULL,
        Weight DECIMAL(10, 2) NULL, -- in kg
        DimensionUnit NVARCHAR(10) NULL DEFAULT 'mm',

        -- Technical Information
        TechnicalNotes NVARCHAR(MAX) NULL,
        QualityCheckpoints NVARCHAR(MAX) NULL, -- JSON array of checkpoint descriptions

        -- Total Standard Time (sum of assembly process + sum of all child parts manufacturing)
        TotalStandardTimeHours DECIMAL(10, 2) NULL DEFAULT 0,

        -- Status and metadata
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        CreatedBy NVARCHAR(100) NULL,
        UpdatedBy NVARCHAR(100) NULL,

        CONSTRAINT FK_ProductTemplate_ProcessTemplate
            FOREIGN KEY (ProcessTemplateId)
            REFERENCES Masters_ProcessTemplates(Id),

        CONSTRAINT CK_ProductTemplate_RollerType
            CHECK (RollerType IN ('MAGNETIC', 'PRINTING'))
    );

    CREATE INDEX IX_ProductTemplate_RollerType ON Masters_ProductTemplates(RollerType);
    CREATE INDEX IX_ProductTemplate_ProcessTemplate ON Masters_ProductTemplates(ProcessTemplateId);
    CREATE INDEX IX_ProductTemplate_Active ON Masters_ProductTemplates(IsActive);
    CREATE INDEX IX_ProductTemplate_Code ON Masters_ProductTemplates(ProductTemplateCode);

    PRINT '✓ Created Masters_ProductTemplates table';
END
ELSE
BEGIN
    PRINT '✓ Masters_ProductTemplates table already exists';
END
GO

-- =============================================
-- Product Template BOM (Bill of Materials) Table
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Masters_ProductTemplateBOM')
BEGIN
    CREATE TABLE Masters_ProductTemplateBOM (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ProductTemplateId INT NOT NULL,
        ChildPartTemplateId INT NOT NULL,
        Quantity INT NOT NULL DEFAULT 1,
        Notes NVARCHAR(500) NULL, -- Assembly notes for this specific part

        -- Ordering for assembly sequence
        SequenceNumber INT NULL,

        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),

        CONSTRAINT FK_ProductBOM_ProductTemplate
            FOREIGN KEY (ProductTemplateId)
            REFERENCES Masters_ProductTemplates(Id)
            ON DELETE CASCADE,

        CONSTRAINT FK_ProductBOM_ChildPartTemplate
            FOREIGN KEY (ChildPartTemplateId)
            REFERENCES Masters_ChildPartTemplates(Id),

        CONSTRAINT CK_ProductBOM_Quantity
            CHECK (Quantity > 0),

        -- Ensure no duplicate child parts in same product
        CONSTRAINT UQ_ProductBOM_ProductChild
            UNIQUE (ProductTemplateId, ChildPartTemplateId)
    );

    CREATE INDEX IX_ProductBOM_ProductTemplate ON Masters_ProductTemplateBOM(ProductTemplateId);
    CREATE INDEX IX_ProductBOM_ChildPartTemplate ON Masters_ProductTemplateBOM(ChildPartTemplateId);
    CREATE INDEX IX_ProductBOM_Sequence ON Masters_ProductTemplateBOM(ProductTemplateId, SequenceNumber);

    PRINT '✓ Created Masters_ProductTemplateBOM table';
END
ELSE
BEGIN
    PRINT '✓ Masters_ProductTemplateBOM table already exists';
END
GO

PRINT 'Migration completed successfully!';
PRINT 'Product Templates and BOM tables created';
GO
