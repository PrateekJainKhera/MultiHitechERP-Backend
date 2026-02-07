-- =============================================
-- Migration: Update Process DefaultMachine to Foreign Key
-- Purpose: Convert DefaultMachine from VARCHAR to INT (FK to Masters_Machines)
--          and automatically create ProcessMachineCapability records
-- Date: 2026-02-07
-- =============================================

-- Step 1: Add new columns
ALTER TABLE Masters_Processes
ADD DefaultMachineId INT NULL,
    DefaultSetupTimeHours DECIMAL(10,2) NULL,
    DefaultCycleTimePerPieceHours DECIMAL(10,4) NULL;
GO

-- Step 2: Set default values
UPDATE Masters_Processes
SET DefaultSetupTimeHours = 0.5,
    DefaultCycleTimePerPieceHours = 0.1
WHERE DefaultSetupTimeHours IS NULL;
GO

-- Step 3: Migrate existing data (match text to machine codes)
UPDATE p
SET p.DefaultMachineId = m.Id
FROM Masters_Processes p
INNER JOIN Masters_Machines m ON p.DefaultMachine = m.MachineCode
WHERE p.DefaultMachine IS NOT NULL;
GO

-- Step 4: Add foreign key constraint
ALTER TABLE Masters_Processes
ADD CONSTRAINT FK_Processes_DefaultMachine
FOREIGN KEY (DefaultMachineId) REFERENCES Masters_Machines(Id);
GO

-- Step 5: Create ProcessMachineCapability records for existing processes with default machines
INSERT INTO Masters_ProcessMachineCapability (
    ProcessId,
    MachineId,
    SetupTimeHours,
    CycleTimePerPieceHours,
    PreferenceLevel,
    EfficiencyRating,
    IsPreferredMachine,
    IsActive,
    CreatedAt,
    CreatedBy
)
SELECT
    p.Id,
    p.DefaultMachineId,
    ISNULL(p.DefaultSetupTimeHours, 0.5),
    ISNULL(p.DefaultCycleTimePerPieceHours, 0.1),
    1, -- Best choice (preference level 1)
    95.00, -- Default efficiency
    1, -- Is preferred machine
    1, -- Active
    GETDATE(),
    'System_Migration'
FROM Masters_Processes p
WHERE p.DefaultMachineId IS NOT NULL
AND NOT EXISTS (
    -- Don't create if capability already exists
    SELECT 1
    FROM Masters_ProcessMachineCapability pmc
    WHERE pmc.ProcessId = p.Id
    AND pmc.MachineId = p.DefaultMachineId
);
GO

-- Step 6: Show results
SELECT 'Migration completed successfully' AS Result;
SELECT
    COUNT(*) AS ProcessesWithDefaultMachine,
    (SELECT COUNT(*) FROM Masters_ProcessMachineCapability WHERE IsPreferredMachine = 1) AS PreferredCapabilitiesCreated
FROM Masters_Processes
WHERE DefaultMachineId IS NOT NULL;
