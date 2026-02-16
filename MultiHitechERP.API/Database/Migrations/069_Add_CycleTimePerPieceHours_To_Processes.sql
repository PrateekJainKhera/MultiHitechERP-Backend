-- Migration 069: Add CycleTimePerPieceHours to Masters_Processes
-- Date: 2026-02-16
-- Description:
--   Adds CycleTimePerPieceHours to Masters_Processes for scheduling calculations.
--   Formula: Total Process Time = SetupTime + (Quantity × CycleTimePerPieceHours)
--   Also removes orphaned legacy columns if they still exist.

-- Step 1: Add CycleTimePerPieceHours column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'CycleTimePerPieceHours')
BEGIN
    ALTER TABLE Masters_Processes
    ADD CycleTimePerPieceHours DECIMAL(8,4) NULL;

    PRINT 'Added CycleTimePerPieceHours column to Masters_Processes';
END
ELSE
BEGIN
    PRINT 'CycleTimePerPieceHours column already exists in Masters_Processes';
END
GO

-- Step 2: Remove orphaned DefaultSetupTimeHours if it exists
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'DefaultSetupTimeHours')
BEGIN
    ALTER TABLE Masters_Processes DROP COLUMN DefaultSetupTimeHours;
    PRINT 'Removed orphaned DefaultSetupTimeHours column';
END
GO

-- Step 3: Remove orphaned DefaultCycleTimePerPieceHours if it exists (replaced by CycleTimePerPieceHours)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'DefaultCycleTimePerPieceHours')
BEGIN
    ALTER TABLE Masters_Processes DROP COLUMN DefaultCycleTimePerPieceHours;
    PRINT 'Removed orphaned DefaultCycleTimePerPieceHours column';
END
GO

-- Step 4: Remove orphaned DefaultMachineId if it exists
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'DefaultMachineId')
BEGIN
    -- Drop foreign key first if exists
    DECLARE @fkName NVARCHAR(200);
    SELECT @fkName = name FROM sys.foreign_keys
    WHERE parent_object_id = OBJECT_ID('Masters_Processes')
      AND referenced_object_id = OBJECT_ID('Masters_Machines');
    IF @fkName IS NOT NULL
        EXEC('ALTER TABLE Masters_Processes DROP CONSTRAINT ' + @fkName);

    ALTER TABLE Masters_Processes DROP COLUMN DefaultMachineId;
    PRINT 'Removed orphaned DefaultMachineId column';
END
GO

PRINT 'Migration 069 completed successfully';
GO
