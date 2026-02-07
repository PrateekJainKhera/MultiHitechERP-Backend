-- =============================================
-- Migration: Link Processes to Real Machines
-- Purpose: Update existing processes to link to actual machines from Masters_Machines
--          and create ProcessMachineCapability records
-- Date: 2026-02-07
-- =============================================

-- CNC Turning processes → CNC Lathe Machine (MCH-001)
UPDATE p
SET p.DefaultMachineId = m.Id,
    p.DefaultSetupTimeHours = 0.5,
    p.DefaultCycleTimePerPieceHours = 0.1
FROM Masters_Processes p
CROSS APPLY (SELECT Id FROM Masters_Machines WHERE MachineCode = 'MCH-001') m
WHERE p.ProcessName LIKE '%CNC Turning%'
   OR p.ProcessName LIKE '%Hard Turning%'
   OR p.ProcessName LIKE '%Roller Turning%'
   OR p.ProcessName LIKE '%Bearing Seat Turning%'
   OR p.ProcessName LIKE '%Pipe Turning%'
   OR p.ProcessName LIKE '%SS Finish Turning%';
GO

-- Center to Center Turning → Hydraulic Turning Lathe (MCH-003)
UPDATE p
SET p.DefaultMachineId = m.Id,
    p.DefaultSetupTimeHours = 0.75,
    p.DefaultCycleTimePerPieceHours = 0.15
FROM Masters_Processes p
CROSS APPLY (SELECT Id FROM Masters_Machines WHERE MachineCode = 'MCH-003') m
WHERE p.ProcessName LIKE '%Center to Center%';
GO

-- Grinding processes → Surface Grinder (GRD-001)
UPDATE p
SET p.DefaultMachineId = m.Id,
    p.DefaultSetupTimeHours = 0.5,
    p.DefaultCycleTimePerPieceHours = 0.2
FROM Masters_Processes p
CROSS APPLY (SELECT Id FROM Masters_Machines WHERE MachineCode = 'GRD-001') m
WHERE p.ProcessName LIKE '%Grinding%'
   OR p.ProcessName LIKE '%OD Grinding%'
   OR p.ProcessName LIKE '%Gear Grinding%'
   OR p.ProcessName LIKE '%Ends Grinding%'
   OR p.ProcessName LIKE '%Rough Grinding%'
   OR p.ProcessName LIKE '%Finish Grinding%';
GO

-- Drilling and Tapping → Radial Drill Press (MCH-005)
UPDATE p
SET p.DefaultMachineId = m.Id,
    p.DefaultSetupTimeHours = 0.3,
    p.DefaultCycleTimePerPieceHours = 0.05
FROM Masters_Processes p
CROSS APPLY (SELECT Id FROM Masters_Machines WHERE MachineCode = 'MCH-005') m
WHERE p.ProcessName LIKE '%Drilling%'
   OR p.ProcessName LIKE '%Tapping%';
GO

-- PCD Holes (VMC work) → VMC Milling Machine (VMC-001)
UPDATE p
SET p.DefaultMachineId = m.Id,
    p.DefaultSetupTimeHours = 1.0,
    p.DefaultCycleTimePerPieceHours = 0.25
FROM Masters_Processes p
CROSS APPLY (SELECT Id FROM Masters_Machines WHERE MachineCode = 'VMC-001') m
WHERE p.ProcessName LIKE '%PCD%';
GO

-- SS Boring → Horizontal Boring Mill (MCH-008)
UPDATE p
SET p.DefaultMachineId = m.Id,
    p.DefaultSetupTimeHours = 0.8,
    p.DefaultCycleTimePerPieceHours = 0.3
FROM Masters_Processes p
CROSS APPLY (SELECT Id FROM Masters_Machines WHERE MachineCode = 'MCH-008') m
WHERE p.ProcessName LIKE '%Boring%';
GO

-- Material Cutting → Test Lathe (MCH-002)
UPDATE p
SET p.DefaultMachineId = m.Id,
    p.DefaultSetupTimeHours = 0.25,
    p.DefaultCycleTimePerPieceHours = 0.05
FROM Masters_Processes p
CROSS APPLY (SELECT Id FROM Masters_Machines WHERE MachineCode = 'MCH-002') m
WHERE p.ProcessName LIKE '%Cutting%';
GO

-- Create ProcessMachineCapability records for all processes with DefaultMachineId
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
    'System_Migration_047'
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

-- Show results
SELECT 'Migration completed successfully' AS Result;
SELECT
    COUNT(*) AS TotalProcesses,
    SUM(CASE WHEN DefaultMachineId IS NOT NULL THEN 1 ELSE 0 END) AS ProcessesWithDefaultMachine,
    SUM(CASE WHEN DefaultMachineId IS NULL THEN 1 ELSE 0 END) AS ProcessesWithoutDefaultMachine
FROM Masters_Processes;
GO

SELECT
    m.MachineCode,
    m.MachineName,
    COUNT(p.Id) AS ProcessesLinked
FROM Masters_Machines m
LEFT JOIN Masters_Processes p ON p.DefaultMachineId = m.Id
WHERE m.IsActive = 1
GROUP BY m.MachineCode, m.MachineName
ORDER BY COUNT(p.Id) DESC;
GO

SELECT COUNT(*) AS CapabilitiesCreated
FROM Masters_ProcessMachineCapability
WHERE CreatedBy = 'System_Migration_047';
GO
