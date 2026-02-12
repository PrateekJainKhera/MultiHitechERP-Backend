-- =============================================================================
-- CLEANUP SCRIPT FOR TESTING
-- Run this to reset Planning, Material Requisitions, and Production data
-- This will NOT delete master data (materials, customers, products, etc.)
-- =============================================================================

BEGIN TRANSACTION;

-- 1. Delete Material Usage History
PRINT 'Deleting Material Usage History...';
DELETE FROM Stores_MaterialUsageHistory;

-- 2. Delete Material Requisition Items
PRINT 'Deleting Material Requisition Items...';
DELETE FROM Stores_MaterialRequisitionItems;

-- 3. Delete Material Requisitions
PRINT 'Deleting Material Requisitions...';
DELETE FROM Stores_MaterialRequisitions;

-- 4. Delete Material Allocations
PRINT 'Deleting Material Allocations...';
DELETE FROM Stores_MaterialAllocations;

-- 5. Delete Material Issues
PRINT 'Deleting Material Issues...';
DELETE FROM Stores_MaterialIssues;

-- 6. Reset Material Pieces to Available status
PRINT 'Resetting Material Pieces to Available status...';
UPDATE Stores_MaterialPieces
SET Status = 'Available',
    AllocatedToRequisitionId = NULL,
    IssuedToJobCardId = NULL,
    CurrentLengthMM = OriginalLengthMM,  -- Reset to original length
    CurrentWeightKG = OriginalWeightKG,  -- Reset to original weight
    IsWastage = 0,
    WastageReason = NULL,
    UpdatedAt = GETUTCDATE(),
    UpdatedBy = 'System_Cleanup'
WHERE Status IN ('Allocated', 'Issued', 'Consumed', 'In Use');

-- 7. Delete Job Card Material Requirements
PRINT 'Deleting Job Card Material Requirements...';
DELETE FROM Planning_JobCardMaterialRequirements;

-- 8. Delete Job Card Process Steps
PRINT 'Deleting Job Card Process Steps...';
DELETE FROM Planning_JobCardProcessSteps;

-- 9. Delete Job Cards
PRINT 'Deleting Job Cards...';
DELETE FROM Planning_JobCards;

-- 10. Reset Inventory Stock to match available pieces
PRINT 'Recalculating Inventory Stock...';
UPDATE s
SET s.CurrentStock = ISNULL(available.TotalAvailableMM, 0),
    s.AllocatedStock = 0,
    s.IssuedStock = 0,
    s.LastUpdated = GETUTCDATE(),
    s.UpdatedBy = 'System_Cleanup'
FROM Inventory_Stock s
LEFT JOIN (
    SELECT
        p.MaterialId,
        SUM(p.CurrentLengthMM) AS TotalAvailableMM
    FROM Stores_MaterialPieces p
    WHERE p.Status = 'Available'
    GROUP BY p.MaterialId
) available ON available.MaterialId = s.ItemId
WHERE s.ItemType = 'RawMaterial';

-- 11. Show summary of cleanup
PRINT '';
PRINT '=== CLEANUP SUMMARY ===';
SELECT
    'Material Pieces Reset to Available' AS Item,
    COUNT(*) AS Count
FROM Stores_MaterialPieces
WHERE Status = 'Available'

UNION ALL

SELECT
    'Total Material Pieces',
    COUNT(*)
FROM Stores_MaterialPieces

UNION ALL

SELECT
    'Material Requisitions Remaining',
    COUNT(*)
FROM Stores_MaterialRequisitions

UNION ALL

SELECT
    'Job Cards Remaining',
    COUNT(*)
FROM Planning_JobCards;

PRINT '';
PRINT '=== INVENTORY SUMMARY ===';
SELECT
    s.ItemCode,
    s.ItemName,
    s.CurrentStock AS AvailableStock_MM,
    s.UOM,
    (SELECT COUNT(*) FROM Stores_MaterialPieces p
     WHERE p.MaterialId = s.ItemId AND p.Status = 'Available') AS AvailablePieces
FROM Inventory_Stock s
WHERE s.ItemType = 'RawMaterial'
ORDER BY s.ItemName;

-- COMMIT or ROLLBACK
-- Uncomment one of the following:
COMMIT;
-- ROLLBACK;

PRINT '';
PRINT 'Cleanup completed successfully!';
PRINT 'All planning, requisition, and production data has been cleared.';
PRINT 'Material pieces have been reset to Available status.';
