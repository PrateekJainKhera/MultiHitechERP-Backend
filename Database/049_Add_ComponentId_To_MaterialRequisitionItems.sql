-- Migration 049: Add ComponentId support to Stores_MaterialRequisitionItems
-- This allows requisition items to reference a purchased component instead of a raw material.
-- materialId and componentId are mutually exclusive (only one will be set per row).

-- Make MaterialId nullable (previously NOT NULL)
ALTER TABLE Stores_MaterialRequisitionItems
    ALTER COLUMN MaterialId INT NULL;
GO

-- Add ComponentId column (nullable, FK to Masters_Components)
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'ComponentId'
)
BEGIN
    ALTER TABLE Stores_MaterialRequisitionItems
        ADD ComponentId INT NULL;
    PRINT 'Added ComponentId column to Stores_MaterialRequisitionItems';
END
GO

-- Add ComponentCode and ComponentName for denormalized display
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'ComponentCode'
)
BEGIN
    ALTER TABLE Stores_MaterialRequisitionItems
        ADD ComponentCode NVARCHAR(50) NULL;
    PRINT 'Added ComponentCode column to Stores_MaterialRequisitionItems';
END
GO

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'ComponentName'
)
BEGIN
    ALTER TABLE Stores_MaterialRequisitionItems
        ADD ComponentName NVARCHAR(200) NULL;
    PRINT 'Added ComponentName column to Stores_MaterialRequisitionItems';
END
GO

PRINT 'Migration 049 completed successfully';
