-- Add missing UpdatedBy column to ProcessTemplates
USE MultiHitechERP;
GO

PRINT 'Adding missing UpdatedBy column to ProcessTemplates...';
GO

-- Add UpdatedBy column if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplates') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE Masters_ProcessTemplates ADD UpdatedBy NVARCHAR(100) NULL;
    PRINT 'Added UpdatedBy column to Masters_ProcessTemplates';
END
ELSE
BEGIN
    PRINT 'UpdatedBy column already exists';
END
GO

PRINT 'ProcessTemplates table updated successfully!';
GO
