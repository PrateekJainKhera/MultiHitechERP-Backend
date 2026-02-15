-- ================================================================
-- Migration 062: Fix JobCard OrderNo to include ItemSequence
-- ================================================================
-- Problem: Job cards have OrderNo like "ORD-202602-0002" but should have "ORD-202602-0002-A"
-- Solution: Update OrderNo to append ItemSequence for all job cards that have OrderItemId
-- ================================================================

BEGIN TRANSACTION;

PRINT 'Starting Migration 062: Fix JobCard OrderNo with ItemSequence';

-- Update all job cards that have an OrderItemId and ItemSequence
-- Append the ItemSequence to the OrderNo if it's not already there
UPDATE Planning_JobCards
SET OrderNo = OrderNo + '-' + ItemSequence
WHERE OrderItemId IS NOT NULL
  AND ItemSequence IS NOT NULL
  AND OrderNo NOT LIKE '%-%' + ItemSequence;

PRINT 'Updated job cards with ItemSequence appended to OrderNo';

-- Verify the update
SELECT
    COUNT(*) AS TotalJobCards,
    COUNT(CASE WHEN OrderItemId IS NOT NULL THEN 1 END) AS JobCardsWithOrderItem,
    COUNT(CASE WHEN OrderItemId IS NOT NULL AND OrderNo LIKE '%-' + ItemSequence THEN 1 END) AS JobCardsWithCorrectOrderNo
FROM Planning_JobCards;

COMMIT TRANSACTION;

PRINT 'Migration 062 completed successfully';
