-- =====================================================================
-- Create Process-Machine Capability Linking Table
-- =====================================================================
-- This table establishes which machines can perform which processes
-- and stores machine-specific time estimates for scheduling
-- =====================================================================

USE MultiHitechERP;
GO

-- Check if table already exists
IF OBJECT_ID('Masters_ProcessMachineCapability', 'U') IS NOT NULL
BEGIN
    PRINT 'Masters_ProcessMachineCapability table already exists. Skipping creation.';
    RETURN;
END

PRINT 'Creating Masters_ProcessMachineCapability table...';
GO

CREATE TABLE Masters_ProcessMachineCapability (
    Id INT IDENTITY(1,1) PRIMARY KEY,

    -- Foreign Keys
    ProcessId INT NOT NULL,
    MachineId INT NOT NULL,

    -- Machine-Specific Time Estimates
    SetupTimeHours DECIMAL(10,2) NULL DEFAULT 0,
    CycleTimePerPieceHours DECIMAL(10,2) NULL DEFAULT 0,

    -- Machine Preference & Capability
    PreferenceLevel INT NULL DEFAULT 3, -- 1=Highest preference, 5=Lowest
    EfficiencyRating DECIMAL(5,2) NULL DEFAULT 100.00, -- 0-100%
    IsPreferredMachine BIT NOT NULL DEFAULT 0,

    -- Capacity Constraints (optional)
    MaxWorkpieceLength DECIMAL(10,3) NULL,
    MaxWorkpieceDiameter DECIMAL(10,3) NULL,
    MaxBatchSize INT NULL,

    -- Cost & Productivity
    HourlyRate DECIMAL(10,2) NULL,
    EstimatedCostPerPiece DECIMAL(10,2) NULL,

    -- Status & Availability
    IsActive BIT NOT NULL DEFAULT 1,
    AvailableFrom DATETIME NULL,
    AvailableTo DATETIME NULL,

    -- Metadata
    Remarks NVARCHAR(500) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedBy NVARCHAR(100) NULL,
    UpdatedAt DATETIME NULL,
    UpdatedBy NVARCHAR(100) NULL,

    -- Foreign Key Constraints
    CONSTRAINT FK_ProcessMachineCapability_Process
        FOREIGN KEY (ProcessId) REFERENCES Masters_Processes(Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_ProcessMachineCapability_Machine
        FOREIGN KEY (MachineId) REFERENCES Masters_Machines(Id)
        ON DELETE CASCADE,

    -- Ensure unique process-machine combination
    CONSTRAINT UQ_ProcessMachine UNIQUE (ProcessId, MachineId)
);

GO

-- Create indexes for better query performance
CREATE INDEX IX_ProcessMachineCapability_ProcessId
    ON Masters_ProcessMachineCapability(ProcessId);

CREATE INDEX IX_ProcessMachineCapability_MachineId
    ON Masters_ProcessMachineCapability(MachineId);

CREATE INDEX IX_ProcessMachineCapability_IsActive
    ON Masters_ProcessMachineCapability(IsActive);

CREATE INDEX IX_ProcessMachineCapability_IsPreferred
    ON Masters_ProcessMachineCapability(IsPreferredMachine);

CREATE INDEX IX_ProcessMachineCapability_PreferenceLevel
    ON Masters_ProcessMachineCapability(PreferenceLevel);

GO

PRINT '';
PRINT '=== Table Structure ===';
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    NUMERIC_PRECISION,
    NUMERIC_SCALE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Masters_ProcessMachineCapability'
ORDER BY ORDINAL_POSITION;

GO

PRINT '';
PRINT 'Masters_ProcessMachineCapability table created successfully!';
PRINT '';
PRINT '=== Usage Example ===';
PRINT 'This table links processes to capable machines with time estimates:';
PRINT '  Process: Turning → Machines: CNC-01, CNC-02, CNC-03';
PRINT '  - CNC-01: Setup 0.5h, Cycle 0.2h, Preference: 1 (best)';
PRINT '  - CNC-02: Setup 0.7h, Cycle 0.25h, Preference: 2';
PRINT '  - CNC-03: Setup 0.6h, Cycle 0.22h, Preference: 3';
PRINT '';
PRINT 'For Scheduling: Query this table to find capable machines,';
PRINT 'then apply load balancing to select the best available machine.';
GO
