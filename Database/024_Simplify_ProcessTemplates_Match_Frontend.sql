-- Simplify Masters_ProcessTemplates and Masters_ProcessTemplateSteps to match frontend
-- Remove unused fields and add ApplicableTypes, IsMandatory, CanBeParallel
USE MultiHitechERP;
GO

PRINT 'Simplifying Masters_ProcessTemplates to match frontend...';
GO

-- ===== PROCESS TEMPLATES TABLE =====

-- Add ApplicableTypes column (stores JSON array: ["PRINTING", "MAGNETIC"])
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplates') AND name = 'ApplicableTypes')
BEGIN
    ALTER TABLE Masters_ProcessTemplates ADD ApplicableTypes NVARCHAR(MAX) NULL;
    PRINT 'Added ApplicableTypes column';
END
GO

-- Drop unused columns from Masters_ProcessTemplates
DECLARE @ConstraintName nvarchar(200)
DECLARE @DropSQL nvarchar(500)

-- Drop constraints before dropping columns
-- ProductId
SELECT @ConstraintName = name FROM sys.default_constraints
WHERE parent_object_id = OBJECT_ID('Masters_ProcessTemplates')
AND parent_column_id = (SELECT column_id FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplates') AND name = 'ProductId')
IF @ConstraintName IS NOT NULL
    EXEC('ALTER TABLE Masters_ProcessTemplates DROP CONSTRAINT ' + @ConstraintName)

-- Status
SELECT @ConstraintName = name FROM sys.default_constraints
WHERE parent_object_id = OBJECT_ID('Masters_ProcessTemplates')
AND parent_column_id = (SELECT column_id FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplates') AND name = 'Status')
IF @ConstraintName IS NOT NULL
    EXEC('ALTER TABLE Masters_ProcessTemplates DROP CONSTRAINT ' + @ConstraintName)

-- TemplateType
SELECT @ConstraintName = name FROM sys.default_constraints
WHERE parent_object_id = OBJECT_ID('Masters_ProcessTemplates')
AND parent_column_id = (SELECT column_id FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplates') AND name = 'TemplateType')
IF @ConstraintName IS NOT NULL
    EXEC('ALTER TABLE Masters_ProcessTemplates DROP CONSTRAINT ' + @ConstraintName)

-- IsDefault
SELECT @ConstraintName = name FROM sys.default_constraints
WHERE parent_object_id = OBJECT_ID('Masters_ProcessTemplates')
AND parent_column_id = (SELECT column_id FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplates') AND name = 'IsDefault')
IF @ConstraintName IS NOT NULL
    EXEC('ALTER TABLE Masters_ProcessTemplates DROP CONSTRAINT ' + @ConstraintName)
GO

-- Drop unused columns
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplates') AND name = 'ProductId')
    ALTER TABLE Masters_ProcessTemplates DROP COLUMN ProductId;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplates') AND name = 'ProductCode')
    ALTER TABLE Masters_ProcessTemplates DROP COLUMN ProductCode;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplates') AND name = 'ProductName')
    ALTER TABLE Masters_ProcessTemplates DROP COLUMN ProductName;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplates') AND name = 'ChildPartId')
    ALTER TABLE Masters_ProcessTemplates DROP COLUMN ChildPartId;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplates') AND name = 'ChildPartName')
    ALTER TABLE Masters_ProcessTemplates DROP COLUMN ChildPartName;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplates') AND name = 'TemplateType')
    ALTER TABLE Masters_ProcessTemplates DROP COLUMN TemplateType;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplates') AND name = 'Status')
    ALTER TABLE Masters_ProcessTemplates DROP COLUMN Status;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplates') AND name = 'IsDefault')
    ALTER TABLE Masters_ProcessTemplates DROP COLUMN IsDefault;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplates') AND name = 'ApprovedBy')
    ALTER TABLE Masters_ProcessTemplates DROP COLUMN ApprovedBy;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplates') AND name = 'ApprovalDate')
    ALTER TABLE Masters_ProcessTemplates DROP COLUMN ApprovalDate;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplates') AND name = 'Remarks')
    ALTER TABLE Masters_ProcessTemplates DROP COLUMN Remarks;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplates') AND name = 'UpdatedBy')
    ALTER TABLE Masters_ProcessTemplates DROP COLUMN UpdatedBy;

PRINT 'Dropped unused columns from Masters_ProcessTemplates';
GO

-- ===== PROCESS TEMPLATE STEPS TABLE =====

-- Add IsMandatory column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplateSteps') AND name = 'IsMandatory')
BEGIN
    ALTER TABLE Masters_ProcessTemplateSteps ADD IsMandatory BIT NOT NULL DEFAULT 1;
    PRINT 'Added IsMandatory column';
END
GO

-- Add CanBeParallel column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplateSteps') AND name = 'CanBeParallel')
BEGIN
    ALTER TABLE Masters_ProcessTemplateSteps ADD CanBeParallel BIT NOT NULL DEFAULT 0;
    PRINT 'Added CanBeParallel column';
END
GO

-- Copy IsParallel to CanBeParallel if it exists
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplateSteps') AND name = 'IsParallel')
BEGIN
    UPDATE Masters_ProcessTemplateSteps SET CanBeParallel = IsParallel WHERE IsParallel = 1;
    PRINT 'Copied IsParallel values to CanBeParallel';
END
GO

-- Drop default constraints for step columns
DECLARE @StepConstraintName nvarchar(200)

-- IsParallel
SELECT @StepConstraintName = name FROM sys.default_constraints
WHERE parent_object_id = OBJECT_ID('Masters_ProcessTemplateSteps')
AND parent_column_id = (SELECT column_id FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplateSteps') AND name = 'IsParallel')
IF @StepConstraintName IS NOT NULL
    EXEC('ALTER TABLE Masters_ProcessTemplateSteps DROP CONSTRAINT ' + @StepConstraintName)

-- RequiresQC
SELECT @StepConstraintName = name FROM sys.default_constraints
WHERE parent_object_id = OBJECT_ID('Masters_ProcessTemplateSteps')
AND parent_column_id = (SELECT column_id FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplateSteps') AND name = 'RequiresQC')
IF @StepConstraintName IS NOT NULL
    EXEC('ALTER TABLE Masters_ProcessTemplateSteps DROP CONSTRAINT ' + @StepConstraintName)
GO

-- Drop unused columns from Masters_ProcessTemplateSteps
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplateSteps') AND name = 'ProcessCode')
    ALTER TABLE Masters_ProcessTemplateSteps DROP COLUMN ProcessCode;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplateSteps') AND name = 'DefaultMachineId')
    ALTER TABLE Masters_ProcessTemplateSteps DROP COLUMN DefaultMachineId;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplateSteps') AND name = 'DefaultMachineName')
    ALTER TABLE Masters_ProcessTemplateSteps DROP COLUMN DefaultMachineName;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplateSteps') AND name = 'MachineType')
    ALTER TABLE Masters_ProcessTemplateSteps DROP COLUMN MachineType;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplateSteps') AND name = 'SetupTimeMin')
    ALTER TABLE Masters_ProcessTemplateSteps DROP COLUMN SetupTimeMin;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplateSteps') AND name = 'CycleTimeMin')
    ALTER TABLE Masters_ProcessTemplateSteps DROP COLUMN CycleTimeMin;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplateSteps') AND name = 'CycleTimePerPiece')
    ALTER TABLE Masters_ProcessTemplateSteps DROP COLUMN CycleTimePerPiece;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplateSteps') AND name = 'IsParallel')
    ALTER TABLE Masters_ProcessTemplateSteps DROP COLUMN IsParallel;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplateSteps') AND name = 'ParallelGroupNo')
    ALTER TABLE Masters_ProcessTemplateSteps DROP COLUMN ParallelGroupNo;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplateSteps') AND name = 'DependsOnSteps')
    ALTER TABLE Masters_ProcessTemplateSteps DROP COLUMN DependsOnSteps;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplateSteps') AND name = 'RequiresQC')
    ALTER TABLE Masters_ProcessTemplateSteps DROP COLUMN RequiresQC;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplateSteps') AND name = 'QCCheckpoints')
    ALTER TABLE Masters_ProcessTemplateSteps DROP COLUMN QCCheckpoints;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplateSteps') AND name = 'WorkInstructions')
    ALTER TABLE Masters_ProcessTemplateSteps DROP COLUMN WorkInstructions;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplateSteps') AND name = 'SafetyInstructions')
    ALTER TABLE Masters_ProcessTemplateSteps DROP COLUMN SafetyInstructions;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplateSteps') AND name = 'ToolingRequired')
    ALTER TABLE Masters_ProcessTemplateSteps DROP COLUMN ToolingRequired;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplateSteps') AND name = 'Remarks')
    ALTER TABLE Masters_ProcessTemplateSteps DROP COLUMN Remarks;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_ProcessTemplateSteps') AND name = 'CreatedAt')
    ALTER TABLE Masters_ProcessTemplateSteps DROP COLUMN CreatedAt;

PRINT 'Dropped unused columns from Masters_ProcessTemplateSteps';
GO

-- ===== VERIFICATION =====

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
PRINT 'Process Templates simplified to match frontend!';
GO
