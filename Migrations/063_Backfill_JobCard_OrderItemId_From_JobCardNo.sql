-- ================================================================
-- Migration 063: Backfill JobCard OrderItemId and ItemSequence
-- ================================================================
-- Problem: Job cards have JobCardNo like "JC-ORD-202602-0002-A-ASSY-14"
--          but OrderItemId and ItemSequence are NULL
-- Solution: Parse the ItemSequence from JobCardNo and link to OrderItem
-- ================================================================

BEGIN TRANSACTION;

PRINT 'Starting Migration 063: Backfill JobCard OrderItemId and ItemSequence';

-- Step 1: Extract ItemSequence from JobCardNo and store in ItemSequence column
-- JobCardNo format: JC-ORD-202602-0002-A-ASSY-14
--                   Split: [JC][ORD][202602][0002][A][ASSY][14]
--                   ItemSequence is the 5th part (index 4, after 4 hyphens)
WITH JobCardParts AS (
    SELECT
        Id,
        JobCardNo,
        -- Find the 4th hyphen position
        CHARINDEX('-', JobCardNo) AS Pos1,
        CHARINDEX('-', JobCardNo, CHARINDEX('-', JobCardNo) + 1) AS Pos2,
        CHARINDEX('-', JobCardNo, CHARINDEX('-', JobCardNo, CHARINDEX('-', JobCardNo) + 1) + 1) AS Pos3,
        CHARINDEX('-', JobCardNo, CHARINDEX('-', JobCardNo, CHARINDEX('-', JobCardNo, CHARINDEX('-', JobCardNo) + 1) + 1) + 1) AS Pos4,
        CHARINDEX('-', JobCardNo, CHARINDEX('-', JobCardNo, CHARINDEX('-', JobCardNo, CHARINDEX('-', JobCardNo, CHARINDEX('-', JobCardNo) + 1) + 1) + 1) + 1) AS Pos5
    FROM Planning_JobCards
    WHERE OrderItemId IS NULL
      AND ItemSequence IS NULL
      AND JobCardNo LIKE 'JC-ORD-%-%-%-%'
)
UPDATE jc
SET ItemSequence = SUBSTRING(jcp.JobCardNo, jcp.Pos4 + 1, jcp.Pos5 - jcp.Pos4 - 1)
FROM Planning_JobCards jc
INNER JOIN JobCardParts jcp ON jc.Id = jcp.Id
WHERE jcp.Pos5 > 0;  -- Ensure we found all hyphens

PRINT 'Step 1: Extracted ItemSequence from JobCardNo';

-- Step 2: Update OrderItemId by joining with Orders_OrderItems table
UPDATE jc
SET jc.OrderItemId = oi.Id
FROM Planning_JobCards jc
INNER JOIN Orders_OrderItems oi ON oi.OrderId = jc.OrderId AND oi.ItemSequence = jc.ItemSequence
WHERE jc.OrderItemId IS NULL
  AND jc.ItemSequence IS NOT NULL;

PRINT 'Step 2: Linked job cards to OrderItems';

-- Step 3: Update OrderNo to include ItemSequence
UPDATE Planning_JobCards
SET OrderNo = OrderNo + '-' + ItemSequence
WHERE OrderItemId IS NOT NULL
  AND ItemSequence IS NOT NULL
  AND OrderNo NOT LIKE '%-' + ItemSequence;

PRINT 'Step 3: Updated OrderNo to include ItemSequence';

-- Verify the update
SELECT
    COUNT(*) AS TotalJobCards,
    COUNT(CASE WHEN OrderItemId IS NOT NULL THEN 1 END) AS JobCardsWithOrderItemId,
    COUNT(CASE WHEN ItemSequence IS NOT NULL THEN 1 END) AS JobCardsWithItemSequence,
    COUNT(CASE WHEN OrderNo LIKE '%-' + ItemSequence THEN 1 END) AS JobCardsWithCorrectOrderNo
FROM Planning_JobCards;

COMMIT TRANSACTION;

PRINT 'Migration 063 completed successfully';
