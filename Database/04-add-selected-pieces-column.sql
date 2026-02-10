-- Add SelectedPieceIds column to Material Requisition Items table
-- This allows storing pre-selected material piece IDs for each requisition item

USE MultiHitechERP;
GO

-- Check if column already exists
IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[Stores_MaterialRequisitionItems]')
    AND name = 'SelectedPieceIds'
)
BEGIN
    -- Add the SelectedPieceIds column
    ALTER TABLE [Stores_MaterialRequisitionItems]
    ADD [SelectedPieceIds] NVARCHAR(500) NULL;

    PRINT 'Column SelectedPieceIds added successfully to Stores_MaterialRequisitionItems table';
END
ELSE
BEGIN
    PRINT 'Column SelectedPieceIds already exists in Stores_MaterialRequisitionItems table';
END
GO

PRINT 'Migration completed successfully!';
GO
