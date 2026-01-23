-- Create Masters_ProcessTemplateSteps table
USE MultiHitechERP;
GO

PRINT 'Creating Masters_ProcessTemplateSteps table...';
GO

-- Drop table if exists (for clean recreation)
IF OBJECT_ID('Masters_ProcessTemplateSteps', 'U') IS NOT NULL
BEGIN
    DROP TABLE Masters_ProcessTemplateSteps;
    PRINT 'Dropped existing Masters_ProcessTemplateSteps table';
END
GO

-- Create the table
CREATE TABLE Masters_ProcessTemplateSteps (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TemplateId INT NOT NULL,
    StepNo INT NOT NULL,

    -- Process Reference
    ProcessId INT NOT NULL,
    ProcessCode NVARCHAR(50) NULL,
    ProcessName NVARCHAR(200) NULL,

    -- Machine Requirement
    DefaultMachineId INT NULL,
    DefaultMachineName NVARCHAR(200) NULL,
    MachineType NVARCHAR(100) NULL,

    -- Time Standards
    SetupTimeMin INT NULL,
    CycleTimeMin INT NULL,
    CycleTimePerPiece DECIMAL(18,3) NULL,

    -- Sequence
    IsParallel BIT NULL DEFAULT 0,
    ParallelGroupNo INT NULL,

    -- Dependencies (JSON array of step numbers)
    DependsOnSteps NVARCHAR(200) NULL,

    -- Quality
    RequiresQC BIT NULL DEFAULT 0,
    QCCheckpoints NVARCHAR(MAX) NULL,

    -- Instructions
    WorkInstructions NVARCHAR(MAX) NULL,
    SafetyInstructions NVARCHAR(MAX) NULL,

    -- Tooling
    ToolingRequired NVARCHAR(500) NULL,

    Remarks NVARCHAR(500) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

    -- Foreign key to Masters_ProcessTemplates
    CONSTRAINT FK_ProcessTemplateSteps_ProcessTemplates
        FOREIGN KEY (TemplateId)
        REFERENCES Masters_ProcessTemplates(Id)
        ON DELETE CASCADE
);
GO

-- Create index on TemplateId for performance
CREATE INDEX IX_ProcessTemplateSteps_TemplateId
ON Masters_ProcessTemplateSteps(TemplateId);
GO

-- Create index on ProcessId for lookups
CREATE INDEX IX_ProcessTemplateSteps_ProcessId
ON Masters_ProcessTemplateSteps(ProcessId);
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
WHERE TABLE_NAME = 'Masters_ProcessTemplateSteps'
ORDER BY ORDINAL_POSITION;
GO

PRINT '';
PRINT 'Masters_ProcessTemplateSteps table created successfully!';
GO
