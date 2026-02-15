-- Migration 066: Fix legacy job cards to link to order items
-- Purpose: Link job cards created before full multi-product support to their order items
-- Date: 2026-02-15

USE MultiHitechERP
GO

-- For job cards that have item sequence in JobCardNo but NULL OrderItemId,
-- extract the item sequence and link to the correct order item

-- Step 1: Update job cards with item sequence 'A' in the job card number
UPDATE jc
SET
    jc.OrderItemId = oi.Id,
    jc.ItemSequence = oi.ItemSequence
FROM Planning_JobCards jc
INNER JOIN Orders_OrderItems oi
    ON jc.OrderId = oi.OrderId
    AND oi.ItemSequence = 'A'
WHERE jc.OrderItemId IS NULL
  AND jc.JobCardNo LIKE '%-A-%';

PRINT 'Updated job cards with item sequence A';
GO

-- Step 2: Update job cards with item sequence 'B' in the job card number
UPDATE jc
SET
    jc.OrderItemId = oi.Id,
    jc.ItemSequence = oi.ItemSequence
FROM Planning_JobCards jc
INNER JOIN Orders_OrderItems oi
    ON jc.OrderId = oi.OrderId
    AND oi.ItemSequence = 'B'
WHERE jc.OrderItemId IS NULL
  AND jc.JobCardNo LIKE '%-B-%';

PRINT 'Updated job cards with item sequence B';
GO

-- Step 3: Update job cards with item sequence 'C' in the job card number
UPDATE jc
SET
    jc.OrderItemId = oi.Id,
    jc.ItemSequence = oi.ItemSequence
FROM Planning_JobCards jc
INNER JOIN Orders_OrderItems oi
    ON jc.OrderId = oi.OrderId
    AND oi.ItemSequence = 'C'
WHERE jc.OrderItemId IS NULL
  AND jc.JobCardNo LIKE '%-C-%';

PRINT 'Updated job cards with item sequence C';
GO

-- Step 4: Update job cards with item sequence 'D' in the job card number
UPDATE jc
SET
    jc.OrderItemId = oi.Id,
    jc.ItemSequence = oi.ItemSequence
FROM Planning_JobCards jc
INNER JOIN Orders_OrderItems oi
    ON jc.OrderId = oi.OrderId
    AND oi.ItemSequence = 'D'
WHERE jc.OrderItemId IS NULL
  AND jc.JobCardNo LIKE '%-D-%';

PRINT 'Updated job cards with item sequence D';
GO

-- Step 5: For orders that only have one order item and job cards with NULL OrderItemId,
-- link them to that single order item
UPDATE jc
SET
    jc.OrderItemId = oi.Id,
    jc.ItemSequence = oi.ItemSequence
FROM Planning_JobCards jc
INNER JOIN (
    -- Find orders that have exactly one order item
    SELECT OrderId, Id, ItemSequence
    FROM Orders_OrderItems oi1
    WHERE (SELECT COUNT(*) FROM Orders_OrderItems oi2 WHERE oi2.OrderId = oi1.OrderId) = 1
) oi ON jc.OrderId = oi.OrderId
WHERE jc.OrderItemId IS NULL;

PRINT 'Updated job cards for single-item orders';
GO

-- Summary: Show counts of job cards updated
SELECT
    'Job Cards with OrderItemId' as Status,
    COUNT(*) as Count
FROM Planning_JobCards
WHERE OrderItemId IS NOT NULL

UNION ALL

SELECT
    'Job Cards without OrderItemId' as Status,
    COUNT(*) as Count
FROM Planning_JobCards
WHERE OrderItemId IS NULL;
GO

PRINT 'Migration 066 completed successfully';
GO
