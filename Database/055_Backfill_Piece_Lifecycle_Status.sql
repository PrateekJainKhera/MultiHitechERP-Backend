-- Migration 055: Backfill piece lifecycle status for existing data
-- Run this ONCE after deploying the piece lifecycle backend changes.
-- Zero impact on the current application flow — only fixes historical records.
-- ==========================================================================

-- SECTION 1: Fix piece status for pieces that were issued but still show Available/Allocated
-- If a requisition is in Issued/Completed status and a piece was issued to a job card,
-- mark that piece as Issued (if the job card is still running) or Consumed (if completed).
-- --------------------------------------------------------------------------
UPDATE p
SET p.Status = CASE
    WHEN jc.ProductionStatus = 'Completed' THEN 'Consumed'
    ELSE 'Issued'
  END,
  p.UpdatedAt = GETUTCDATE()
FROM Stores_MaterialPieces p
JOIN Planning_JobCards jc ON jc.Id = p.IssuedToJobCardId
WHERE p.Status IN ('Available', 'Allocated')
  AND p.IssuedToJobCardId IS NOT NULL;
GO

-- SECTION 2: Fix piece status for pieces selected in requisition items
-- that belong to an Issued requisition but IssuedToJobCardId is NULL
-- (issued via the old flow before IssuePieceAsync was called).
-- --------------------------------------------------------------------------
UPDATE p
SET p.Status = 'Issued',
    p.IssuedToJobCardId = ri.JobCardId,
    p.UpdatedAt = GETUTCDATE()
FROM Stores_MaterialPieces p
JOIN (
    -- Parse comma-separated SelectedPieceIds to individual rows
    SELECT
        ri.JobCardId,
        CAST(TRIM(value) AS INT) AS PieceId
    FROM Stores_MaterialRequisitionItems ri
    CROSS APPLY STRING_SPLIT(ri.SelectedPieceIds, ',')
    WHERE ri.SelectedPieceIds IS NOT NULL
      AND ri.SelectedPieceIds != ''
) ri ON ri.PieceId = p.Id
JOIN Stores_MaterialRequisitions req ON req.Id = (
    SELECT TOP 1 RequisitionId FROM Stores_MaterialRequisitionItems WHERE Id = (
        SELECT TOP 1 Id FROM Stores_MaterialRequisitionItems
        WHERE JobCardId = ri.JobCardId
          AND SelectedPieceIds LIKE '%' + CAST(p.Id AS NVARCHAR) + '%'
    )
)
WHERE p.Status = 'Available'
  AND p.IssuedToJobCardId IS NULL
  AND req.Status IN ('Issued', 'Completed');
GO

-- SECTION 3: Recalculate Inventory_Stock.CurrentStock from actual available pieces
-- This is the most important fix — removes the gap between GRN-added stock
-- and what was actually issued. Sets CurrentStock = sum of Available piece lengths.
-- --------------------------------------------------------------------------
UPDATE s
SET s.CurrentStock = ISNULL(available.TotalAvailableMM, 0),
    s.LastUpdated   = GETUTCDATE(),
    s.UpdatedBy     = 'System_Backfill_055'
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
GO

-- SECTION 4: Show summary of changes made
-- --------------------------------------------------------------------------
SELECT
    'Pieces now Issued'   AS Category,
    COUNT(*)              AS Count
FROM Stores_MaterialPieces
WHERE Status = 'Issued'

UNION ALL

SELECT
    'Pieces now Consumed',
    COUNT(*)
FROM Stores_MaterialPieces
WHERE Status = 'Consumed'

UNION ALL

SELECT
    'Pieces still Available',
    COUNT(*)
FROM Stores_MaterialPieces
WHERE Status = 'Available';
GO

-- Show current Inventory_Stock for raw materials after recalc
SELECT
    s.ItemCode,
    s.ItemName,
    s.CurrentStock,
    s.UOM,
    s.LastUpdated
FROM Inventory_Stock s
WHERE s.ItemType = 'RawMaterial'
ORDER BY s.ItemName;
GO
