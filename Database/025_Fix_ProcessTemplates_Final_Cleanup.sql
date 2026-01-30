-- Final cleanup for Process Templates
USE MultiHitechERP;
GO

PRINT 'Final cleanup of Process Templates...';
GO

-- Drop default constraint on CreatedAt in ProcessTemplateSteps before dropping column
DECLARE @StepCreatedAtConstraint nvarchar(200)
SELECT @StepCreatedAtConstraint = name FROM sys.default_constraints
WHERE parent_object_id = OBJECT_ID('Masters_ProcessTemplateSteps')
AND parent_column_id = (SELECT column_id FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplateSteps') AND name = 'CreatedAt')
IF @StepCreatedAtConstraint IS NOT NULL
BEGIN
    EXEC('ALTER TABLE Masters_ProcessTemplateSteps DROP CONSTRAINT ' + @StepCreatedAtConstraint)
    PRINT 'Dropped CreatedAt default constraint from ProcessTemplateSteps';
END
GO

-- Now drop CreatedAt from ProcessTemplateSteps
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplateSteps') AND name = 'CreatedAt')
BEGIN
    ALTER TABLE Masters_ProcessTemplateSteps DROP COLUMN CreatedAt;
    PRINT 'Dropped CreatedAt column from ProcessTemplateSteps';
END
GO

-- Drop remaining unused columns from ProcessTemplates
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplates') AND name = 'ProductType')
BEGIN
    ALTER TABLE Masters_ProcessTemplates DROP COLUMN ProductType;
    PRINT 'Dropped ProductType column';
END
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplates') AND name = 'TotalEstimatedTime')
BEGIN
    ALTER TABLE Masters_ProcessTemplates DROP COLUMN TotalEstimatedTime;
    PRINT 'Dropped TotalEstimatedTime column';
END
GO

PRINT '';
PRINT '=== Final ProcessTemplates structure ===';
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Masters_ProcessTemplates'
ORDER BY ORDINAL_POSITION;
GO

PRINT '';
PRINT '=== Final ProcessTemplateSteps structure ===';
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Masters_ProcessTemplateSteps'
ORDER BY ORDINAL_POSITION;
GO

PRINT '';
PRINT 'Process Templates final cleanup complete!';
GO
