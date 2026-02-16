-- =============================================
-- Migration 067: Remove Legacy Process and Machine Fields
-- Purpose: Clean up legacy fields now that we use Process Categories
--          for capacity-based scheduling
-- Date: 2026-02-16
-- =============================================

-- Remove legacy DefaultMachine fields from Masters_Processes
-- These are no longer needed as we now use Process Categories for capacity-based scheduling
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'DefaultMachine')
BEGIN
    ALTER TABLE Masters_Processes DROP COLUMN DefaultMachine;
    PRINT 'Dropped DefaultMachine column from Masters_Processes';
END
GO

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'DefaultMachineId')
BEGIN
    -- Drop foreign key constraint first if it exists
    DECLARE @ConstraintName NVARCHAR(200);
    SELECT @ConstraintName = fk.name
    FROM sys.foreign_keys fk
    INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
    WHERE fk.parent_object_id = OBJECT_ID('Masters_Processes')
      AND fk.referenced_object_id = OBJECT_ID('Masters_Machines')
      AND COL_NAME(fk.parent_object_id, fkc.parent_column_id) = 'DefaultMachineId';

    IF @ConstraintName IS NOT NULL
    BEGIN
        EXEC('ALTER TABLE Masters_Processes DROP CONSTRAINT ' + @ConstraintName);
        PRINT 'Dropped foreign key constraint: ' + @ConstraintName;
    END

    ALTER TABLE Masters_Processes DROP COLUMN DefaultMachineId;
    PRINT 'Dropped DefaultMachineId column from Masters_Processes';
END
GO

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'DefaultSetupTimeHours')
BEGIN
    ALTER TABLE Masters_Processes DROP COLUMN DefaultSetupTimeHours;
    PRINT 'Dropped DefaultSetupTimeHours column from Masters_Processes';
END
GO

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'DefaultCycleTimePerPieceHours')
BEGIN
    ALTER TABLE Masters_Processes DROP COLUMN DefaultCycleTimePerPieceHours;
    PRINT 'Dropped DefaultCycleTimePerPieceHours column from Masters_Processes';
END
GO

-- Optional: Drop the entire ProcessMachineCapability table if no longer needed
-- Commenting this out in case historical data is needed
/*
IF OBJECT_ID('Masters_ProcessMachineCapability', 'U') IS NOT NULL
BEGIN
    DROP TABLE Masters_ProcessMachineCapability;
    PRINT 'Dropped Masters_ProcessMachineCapability table';
END
GO
*/

PRINT '✅ Migration 067 completed successfully';
PRINT 'Removed legacy Process DefaultMachine fields';
PRINT 'Process Categories are now used for capacity-based scheduling';
GO
