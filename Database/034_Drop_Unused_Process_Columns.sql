-- Drop unused columns from Masters_Processes table
USE MultiHitechERP;
GO

PRINT 'Dropping unused columns from Masters_Processes table...';
GO

-- Drop ProcessType (not used - we have IsOutsourced)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'ProcessType')
BEGIN
    ALTER TABLE Masters_Processes DROP COLUMN ProcessType;
    PRINT 'Dropped ProcessType column';
END

-- Drop StandardTime (old field - we use StandardSetupTimeMin)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'StandardTime')
BEGIN
    ALTER TABLE Masters_Processes DROP COLUMN StandardTime;
    PRINT 'Dropped StandardTime column';
END

-- Drop Department (not in simplified form)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'Department')
BEGIN
    ALTER TABLE Masters_Processes DROP COLUMN Department;
    PRINT 'Dropped Department column';
END

-- Drop ProcessDetails (not in simplified form)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'ProcessDetails')
BEGIN
    ALTER TABLE Masters_Processes DROP COLUMN ProcessDetails;
    PRINT 'Dropped ProcessDetails column';
END

-- Drop DefaultMachineName (replaced by DefaultMachine)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'DefaultMachineName')
BEGIN
    ALTER TABLE Masters_Processes DROP COLUMN DefaultMachineName;
    PRINT 'Dropped DefaultMachineName column';
END

-- Drop StandardCycleTimeMin (not in simplified form)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'StandardCycleTimeMin')
BEGIN
    ALTER TABLE Masters_Processes DROP COLUMN StandardCycleTimeMin;
    PRINT 'Dropped StandardCycleTimeMin column';
END

-- Drop StandardCycleTimePerPiece (not in simplified form)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'StandardCycleTimePerPiece')
BEGIN
    ALTER TABLE Masters_Processes DROP COLUMN StandardCycleTimePerPiece;
    PRINT 'Dropped StandardCycleTimePerPiece column';
END

-- Drop SkillLevel (not in simplified form)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'SkillLevel')
BEGIN
    ALTER TABLE Masters_Processes DROP COLUMN SkillLevel;
    PRINT 'Dropped SkillLevel column';
END

-- Drop OperatorsRequired (not in simplified form)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'OperatorsRequired')
BEGIN
    -- Drop default constraint first if exists
    DECLARE @OperatorsRequiredConstraint NVARCHAR(200);
    SELECT @OperatorsRequiredConstraint = name
    FROM sys.default_constraints
    WHERE parent_object_id = OBJECT_ID('Masters_Processes')
    AND parent_column_id = (SELECT column_id FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'OperatorsRequired');

    IF @OperatorsRequiredConstraint IS NOT NULL
    BEGIN
        EXEC('ALTER TABLE Masters_Processes DROP CONSTRAINT ' + @OperatorsRequiredConstraint);
    END

    ALTER TABLE Masters_Processes DROP COLUMN OperatorsRequired;
    PRINT 'Dropped OperatorsRequired column';
END

-- Drop HourlyRate (not in simplified form)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'HourlyRate')
BEGIN
    ALTER TABLE Masters_Processes DROP COLUMN HourlyRate;
    PRINT 'Dropped HourlyRate column';
END

-- Drop StandardCostPerPiece (not in simplified form)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'StandardCostPerPiece')
BEGIN
    ALTER TABLE Masters_Processes DROP COLUMN StandardCostPerPiece;
    PRINT 'Dropped StandardCostPerPiece column';
END

-- Drop RequiresQC (not in simplified form)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'RequiresQC')
BEGIN
    -- Drop default constraint first if exists
    DECLARE @RequiresQCConstraint NVARCHAR(200);
    SELECT @RequiresQCConstraint = name
    FROM sys.default_constraints
    WHERE parent_object_id = OBJECT_ID('Masters_Processes')
    AND parent_column_id = (SELECT column_id FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'RequiresQC');

    IF @RequiresQCConstraint IS NOT NULL
    BEGIN
        EXEC('ALTER TABLE Masters_Processes DROP CONSTRAINT ' + @RequiresQCConstraint);
    END

    ALTER TABLE Masters_Processes DROP COLUMN RequiresQC;
    PRINT 'Dropped RequiresQC column';
END

-- Drop QCCheckpoints (not in simplified form)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'QCCheckpoints')
BEGIN
    ALTER TABLE Masters_Processes DROP COLUMN QCCheckpoints;
    PRINT 'Dropped QCCheckpoints column';
END

-- Drop PreferredVendor (not in simplified form)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'PreferredVendor')
BEGIN
    ALTER TABLE Masters_Processes DROP COLUMN PreferredVendor;
    PRINT 'Dropped PreferredVendor column';
END

-- Drop Remarks (not in simplified form - we have Description)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'Remarks')
BEGIN
    ALTER TABLE Masters_Processes DROP COLUMN Remarks;
    PRINT 'Dropped Remarks column';
END

-- Drop MachineType (not in simplified form)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'MachineType')
BEGIN
    ALTER TABLE Masters_Processes DROP COLUMN MachineType;
    PRINT 'Dropped MachineType column';
END

-- Drop DefaultMachineId (not in simplified form - we use DefaultMachine string)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'DefaultMachineId')
BEGIN
    ALTER TABLE Masters_Processes DROP COLUMN DefaultMachineId;
    PRINT 'Dropped DefaultMachineId column';
END

GO

PRINT '';
PRINT '=== Verification: Final table structure ===';
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    NUMERIC_PRECISION,
    NUMERIC_SCALE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Masters_Processes'
ORDER BY ORDINAL_POSITION;
GO

PRINT '';
PRINT 'Unused columns dropped successfully from Masters_Processes table!';
PRINT 'Final columns: Id, ProcessCode, ProcessName, Category, DefaultMachine, StandardSetupTimeMin, RestTimeHours, Description, IsOutsourced, IsActive, Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy';
GO
