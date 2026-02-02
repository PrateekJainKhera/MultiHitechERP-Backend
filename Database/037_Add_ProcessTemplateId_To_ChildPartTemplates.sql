-- Migration: Add ProcessTemplateId column to Masters_ChildPartTemplates
-- Purpose: Link child part templates to their manufacturing process templates
-- Date: 2026-02-02

USE MultiHitechERP;
GO

PRINT 'Adding ProcessTemplateId column to Masters_ChildPartTemplates...';

-- Check if column doesn't exist before adding
IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID('Masters_ChildPartTemplates')
    AND name = 'ProcessTemplateId'
)
BEGIN
    ALTER TABLE Masters_ChildPartTemplates
    ADD ProcessTemplateId INT NULL;

    PRINT '✓ Added ProcessTemplateId column';

    -- Optionally add foreign key constraint to ProcessTemplates table
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Masters_ProcessTemplates')
    BEGIN
        ALTER TABLE Masters_ChildPartTemplates
        ADD CONSTRAINT FK_ChildPartTemplates_ProcessTemplates
        FOREIGN KEY (ProcessTemplateId) REFERENCES Masters_ProcessTemplates(Id);

        PRINT '✓ Added foreign key constraint to ProcessTemplates';
    END
END
ELSE
BEGIN
    PRINT '✓ ProcessTemplateId column already exists';
END

GO

PRINT 'Migration completed successfully!';
GO
