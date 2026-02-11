-- Migration 051: Make MachineId nullable in Scheduling_MachineSchedules
-- Reason: OSP (Outside Service Process) steps have no machine assigned.
--         Inserting MachineId = 0 violated FK_Schedule_Machine constraint.
--         NULL bypasses the FK check in SQL Server, so NULL = no machine (OSP).

-- Step 1: Drop the existing FK constraint
IF EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = 'FK_Schedule_Machine'
      AND parent_object_id = OBJECT_ID('Scheduling_MachineSchedules')
)
BEGIN
    ALTER TABLE Scheduling_MachineSchedules
    DROP CONSTRAINT FK_Schedule_Machine;
    PRINT 'Dropped FK_Schedule_Machine constraint';
END
ELSE
    PRINT 'FK_Schedule_Machine not found — skipping drop';

-- Step 2: Alter MachineId column to allow NULLs
ALTER TABLE Scheduling_MachineSchedules
ALTER COLUMN MachineId INT NULL;
PRINT 'Altered MachineId to INT NULL';

-- Step 3: Re-add FK constraint (NULL values bypass FK in SQL Server automatically)
ALTER TABLE Scheduling_MachineSchedules
ADD CONSTRAINT FK_Schedule_Machine
    FOREIGN KEY (MachineId) REFERENCES Masters_Machines(Id);
PRINT 'Re-added FK_Schedule_Machine constraint';
