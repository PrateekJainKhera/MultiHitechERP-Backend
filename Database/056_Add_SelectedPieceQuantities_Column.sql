-- Migration 056: Add SelectedPieceQuantities column to Stores_MaterialRequisitionItems
-- This enables storing cut quantities for each selected piece (Level 2 tracking)
-- ==================================================================================

-- Add the column if it doesn't exist
IF NOT EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems'
      AND COLUMN_NAME = 'SelectedPieceQuantities'
)
BEGIN
    ALTER TABLE Stores_MaterialRequisitionItems
    ADD SelectedPieceQuantities NVARCHAR(MAX) NULL;

    PRINT 'Added SelectedPieceQuantities column to Stores_MaterialRequisitionItems';
END
ELSE
BEGIN
    PRINT 'SelectedPieceQuantities column already exists';
END
GO

-- Verify the column was added
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems'
  AND COLUMN_NAME IN ('SelectedPieceIds', 'SelectedPieceQuantities')
ORDER BY ORDINAL_POSITION;
GO

PRINT '';
PRINT 'Migration 056 completed successfully!';
PRINT 'Column SelectedPieceQuantities is now available to store cut quantities for Level 2 tracking.';
