-- Migration 068: Update Process Master - Add Process Category Link
-- Date: 2026-02-16
-- Description:
--   1. Adds ProcessCategoryId to Masters_Processes table
--   2. Removes machine reference if it exists (processes no longer directly linked to machines)

-- Step 1: Add ProcessCategoryId to Processes table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'ProcessCategoryId')
BEGIN
    ALTER TABLE Masters_Processes
    ADD ProcessCategoryId INT NULL; -- NULL initially for existing processes

    PRINT 'Added ProcessCategoryId column to Masters_Processes';
END
ELSE
BEGIN
    PRINT 'ProcessCategoryId column already exists in Masters_Processes';
END
GO

-- Step 2: Add foreign key constraint
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Process_ProcessCategory')
BEGIN
    ALTER TABLE Masters_Processes
    ADD CONSTRAINT FK_Process_ProcessCategory
        FOREIGN KEY (ProcessCategoryId) REFERENCES Masters_ProcessCategories(Id);

    PRINT 'Added foreign key constraint FK_Process_ProcessCategory';
END
ELSE
BEGIN
    PRINT 'Foreign key constraint FK_Process_ProcessCategory already exists';
END
GO

-- Step 3: Create index for performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Process_ProcessCategoryId' AND object_id = OBJECT_ID('Masters_Processes'))
BEGIN
    CREATE INDEX IX_Process_ProcessCategoryId ON Masters_Processes(ProcessCategoryId);
    PRINT 'Created index IX_Process_ProcessCategoryId';
END
ELSE
BEGIN
    PRINT 'Index IX_Process_ProcessCategoryId already exists';
END
GO

-- Step 4: Remove old machine reference columns if they exist
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Processes') AND name = 'MachineId')
BEGIN
    -- First drop foreign key if exists
    DECLARE @constraintName NVARCHAR(200);
    SELECT @constraintName = name
    FROM sys.foreign_keys
    WHERE parent_object_id = OBJECT_ID('Masters_Processes')
    AND referenced_object_id = OBJECT_ID('Masters_Machines');

    IF @constraintName IS NOT NULL
    BEGIN
        EXEC('ALTER TABLE Masters_Processes DROP CONSTRAINT ' + @constraintName);
        PRINT 'Dropped foreign key constraint: ' + @constraintName;
    END

    -- Drop the column
    ALTER TABLE Masters_Processes DROP COLUMN MachineId;
    PRINT 'Removed MachineId column from Masters_Processes';
END
ELSE
BEGIN
    PRINT 'MachineId column does not exist in Masters_Processes (already clean)';
END
GO

-- Step 5: Auto-assign existing processes to categories based on process name (best effort)
-- This is a smart migration that tries to match process names to category names

UPDATE p
SET p.ProcessCategoryId = pc.Id
FROM Masters_Processes p
INNER JOIN Masters_ProcessCategories pc
    ON p.ProcessName LIKE '%' + REPLACE(pc.CategoryName, ' 1', '%') + '%'
    OR p.ProcessName LIKE '%' + REPLACE(pc.CategoryName, ' 2', '%') + '%'
WHERE p.ProcessCategoryId IS NULL;

DECLARE @updatedCount INT = @@ROWCOUNT;

IF @updatedCount > 0
BEGIN
    PRINT 'Auto-assigned ' + CAST(@updatedCount AS NVARCHAR(10)) + ' processes to categories based on name matching';
END

-- Set any remaining unassigned processes to 'Assembly' category as default
UPDATE p
SET p.ProcessCategoryId = (SELECT TOP 1 Id FROM Masters_ProcessCategories WHERE CategoryCode = 'ASSY')
FROM Masters_Processes p
WHERE p.ProcessCategoryId IS NULL;

PRINT 'Migration 068 completed successfully';
PRINT 'NOTE: Review process-category assignments in Process Master UI';
GO
