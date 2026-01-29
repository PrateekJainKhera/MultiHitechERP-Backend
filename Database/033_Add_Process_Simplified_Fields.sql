-- Add simplified Process fields (DefaultMachine and RestTimeHours)
USE MultiHitechERP;
GO

PRINT 'Adding simplified Process fields to Masters_Processes table...';
GO

-- Add DefaultMachine (simple string field for machine assignment)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'DefaultMachine')
BEGIN
    ALTER TABLE Masters_Processes ADD DefaultMachine NVARCHAR(200) NULL;
    PRINT 'Added DefaultMachine column';
END
ELSE
BEGIN
    PRINT 'DefaultMachine column already exists';
END
GO

-- Copy from DefaultMachineName if it exists (separate batch after column is created)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'DefaultMachine')
   AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'DefaultMachineName')
BEGIN
    UPDATE Masters_Processes
    SET DefaultMachine = DefaultMachineName
    WHERE DefaultMachineName IS NOT NULL;
    PRINT 'Copied DefaultMachineName values to DefaultMachine';
END
GO

-- Add RestTimeHours
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'RestTimeHours')
BEGIN
    ALTER TABLE Masters_Processes ADD RestTimeHours DECIMAL(18,2) NULL;
    PRINT 'Added RestTimeHours column';
END
ELSE
BEGIN
    PRINT 'RestTimeHours column already exists';
END
GO

-- Ensure StandardSetupTimeMin is INT (fix if it's DECIMAL)
IF EXISTS (
    SELECT * FROM sys.columns c
    INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
    WHERE c.object_id = OBJECT_ID('Masters_Processes')
    AND c.name = 'StandardSetupTimeMin'
    AND t.name = 'decimal'
)
BEGIN
    PRINT 'Converting StandardSetupTimeMin from DECIMAL to INT...';
    ALTER TABLE Masters_Processes ADD StandardSetupTimeMin_Temp INT NULL;
END
ELSE
BEGIN
    PRINT 'StandardSetupTimeMin is already INT or does not exist';
END
GO

-- Copy values in separate batch
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'StandardSetupTimeMin_Temp')
BEGIN
    UPDATE Masters_Processes
    SET StandardSetupTimeMin_Temp = CAST(ROUND(ISNULL(StandardSetupTimeMin, 0), 0) AS INT);
    PRINT 'Copied values to temp column';
END
GO

-- Drop old column and rename in separate batch
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'StandardSetupTimeMin_Temp')
BEGIN
    ALTER TABLE Masters_Processes DROP COLUMN StandardSetupTimeMin;
    EXEC sp_rename 'Masters_Processes.StandardSetupTimeMin_Temp', 'StandardSetupTimeMin', 'COLUMN';
    PRINT 'StandardSetupTimeMin converted to INT';
END
GO

PRINT '';
PRINT '=== Verification: Current table structure ===';
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
PRINT 'Process simplified fields added successfully!';
GO
