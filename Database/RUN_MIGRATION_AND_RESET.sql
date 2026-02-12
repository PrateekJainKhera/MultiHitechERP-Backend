-- =============================================================================
-- Combined Script: Run Migration 056 + Reset Piece Statuses
-- =============================================================================

PRINT '========================================';
PRINT 'Step 1: Adding SelectedPieceQuantities column';
PRINT '========================================';

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

    PRINT '✓ Added SelectedPieceQuantities column to Stores_MaterialRequisitionItems';
END
ELSE
BEGIN
    PRINT '✓ SelectedPieceQuantities column already exists';
END
GO

PRINT '';
PRINT '========================================';
PRINT 'Step 2: Resetting Material Pieces to Available';
PRINT '========================================';

-- Reset Issued pieces back to Available
UPDATE Stores_MaterialPieces
SET Status = 'Available',
    IssuedToJobCardId = NULL,
    AllocatedToRequisitionId = NULL,
    UpdatedAt = GETUTCDATE(),
    UpdatedBy = 'System_Reset'
WHERE Status IN ('Issued', 'Allocated', 'In Use');

PRINT '✓ Reset ' + CAST(@@ROWCOUNT AS VARCHAR) + ' pieces to Available status';
GO

PRINT '';
PRINT '========================================';
PRINT 'Step 3: Verification';
PRINT '========================================';

-- Verify column exists
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems'
  AND COLUMN_NAME IN ('SelectedPieceIds', 'SelectedPieceQuantities')
ORDER BY ORDINAL_POSITION;

-- Show piece status summary
SELECT
    Status,
    COUNT(*) AS PieceCount,
    SUM(CurrentLengthMM) AS TotalLengthMM
FROM Stores_MaterialPieces
GROUP BY Status
ORDER BY Status;

PRINT '';
PRINT '✓ Migration and reset completed successfully!';
