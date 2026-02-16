-- Migration 067: Update Machines - Add Process Categories and Daily Capacity
-- Date: 2026-02-16
-- Description:
--   1. Creates Machine-ProcessCategory junction table (many-to-many relationship)
--   2. Adds DailyCapacityHours field to Machines
--   3. Removes old ProcessCapabilities field if it exists

-- Step 1: Add DailyCapacityHours to Machines table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Machines') AND name = 'DailyCapacityHours')
BEGIN
    ALTER TABLE Masters_Machines
    ADD DailyCapacityHours DECIMAL(5,2) NOT NULL DEFAULT 8.0;

    PRINT 'Added DailyCapacityHours column to Masters_Machines';
END
ELSE
BEGIN
    PRINT 'DailyCapacityHours column already exists in Masters_Machines';
END
GO

-- Step 2: Create Machine-ProcessCategory junction table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Machine_ProcessCategories')
BEGIN
    CREATE TABLE Machine_ProcessCategories (
        Id INT PRIMARY KEY IDENTITY(1,1),
        MachineId INT NOT NULL,
        ProcessCategoryId INT NOT NULL,

        -- Audit
        CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
        CreatedBy NVARCHAR(100) NULL,

        -- Foreign Keys
        CONSTRAINT FK_MachineProcessCategory_Machine
            FOREIGN KEY (MachineId) REFERENCES Masters_Machines(Id) ON DELETE CASCADE,
        CONSTRAINT FK_MachineProcessCategory_ProcessCategory
            FOREIGN KEY (ProcessCategoryId) REFERENCES Masters_ProcessCategories(Id) ON DELETE CASCADE,

        -- Unique constraint to prevent duplicates
        CONSTRAINT UQ_MachineProcessCategory
            UNIQUE (MachineId, ProcessCategoryId),

        -- Indexes for performance
        INDEX IX_MachineProcessCategory_MachineId (MachineId),
        INDEX IX_MachineProcessCategory_ProcessCategoryId (ProcessCategoryId)
    );

    PRINT 'Created table: Machine_ProcessCategories';
END
ELSE
BEGIN
    PRINT 'Table Machine_ProcessCategories already exists';
END
GO

-- Step 3: Remove old ProcessCapabilities column if it exists
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Machines') AND name = 'ProcessCapabilities')
BEGIN
    ALTER TABLE Masters_Machines
    DROP COLUMN ProcessCapabilities;

    PRINT 'Removed ProcessCapabilities column from Masters_Machines';
END
ELSE
BEGIN
    PRINT 'ProcessCapabilities column does not exist in Masters_Machines (already clean)';
END
GO

-- Step 4: Migrate existing machines to default categories (if needed)
-- For now, we'll leave this for manual assignment through the UI
PRINT 'Migration 067 completed successfully';
PRINT 'NOTE: Assign Process Categories to existing machines through the Machine Master UI';
GO
