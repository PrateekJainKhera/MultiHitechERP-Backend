-- Migration: Add OrderItemId and ItemSequence to JobCards for multi-product order support
-- Date: 2026-02-14
-- Purpose: Link job cards to specific order items and add item sequence for job card numbering

USE MultiHitechERP;
GO

-- Step 1: Add new columns to Planning_JobCards
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Planning_JobCards') AND name = 'OrderItemId')
BEGIN
    ALTER TABLE Planning_JobCards
    ADD OrderItemId INT NULL;

    PRINT 'Added OrderItemId column to Planning_JobCards';
END
ELSE
BEGIN
    PRINT 'OrderItemId column already exists in Planning_JobCards';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Planning_JobCards') AND name = 'ItemSequence')
BEGIN
    ALTER TABLE Planning_JobCards
    ADD ItemSequence NVARCHAR(10) NULL;

    PRINT 'Added ItemSequence column to Planning_JobCards';
END
ELSE
BEGIN
    PRINT 'ItemSequence column already exists in Planning_JobCards';
END
GO

-- Step 2: Backfill existing job cards with OrderItemId
-- For each existing job card, find the corresponding OrderItem with sequence 'A' (from legacy single-product orders)
UPDATE jc
SET
    jc.OrderItemId = oi.Id,
    jc.ItemSequence = oi.ItemSequence
FROM Planning_JobCards jc
INNER JOIN Orders_OrderItems oi ON jc.OrderId = oi.OrderId
WHERE jc.OrderItemId IS NULL
  AND oi.ItemSequence = 'A';  -- Legacy orders have a single item with sequence 'A'

PRINT 'Backfilled OrderItemId and ItemSequence for existing job cards';
GO

-- Step 3: Add foreign key constraint
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_JobCards_OrderItems')
BEGIN
    ALTER TABLE Planning_JobCards
    ADD CONSTRAINT FK_JobCards_OrderItems
    FOREIGN KEY (OrderItemId) REFERENCES Orders_OrderItems(Id);

    PRINT 'Added FK constraint FK_JobCards_OrderItems';
END
ELSE
BEGIN
    PRINT 'FK constraint FK_JobCards_OrderItems already exists';
END
GO

-- Step 4: Create index for better query performance
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_JobCards_OrderItemId')
BEGIN
    CREATE NONCLUSTERED INDEX IX_JobCards_OrderItemId
    ON Planning_JobCards(OrderItemId)
    INCLUDE (JobCardNo, Status, ProductionStatus);

    PRINT 'Created index IX_JobCards_OrderItemId';
END
ELSE
BEGIN
    PRINT 'Index IX_JobCards_OrderItemId already exists';
END
GO

-- Step 5: Verification
PRINT '=== Verification ===';
PRINT 'Total Job Cards: ' + CAST((SELECT COUNT(*) FROM Planning_JobCards) AS NVARCHAR);
PRINT 'Job Cards with OrderItemId: ' + CAST((SELECT COUNT(*) FROM Planning_JobCards WHERE OrderItemId IS NOT NULL) AS NVARCHAR);
PRINT 'Job Cards with ItemSequence: ' + CAST((SELECT COUNT(*) FROM Planning_JobCards WHERE ItemSequence IS NOT NULL) AS NVARCHAR);

-- Show sample of updated job cards
SELECT TOP 5
    jc.Id,
    jc.JobCardNo,
    jc.OrderId,
    jc.OrderNo,
    jc.OrderItemId,
    jc.ItemSequence,
    oi.ProductId,
    oi.Quantity
FROM Planning_JobCards jc
LEFT JOIN Orders_OrderItems oi ON jc.OrderItemId = oi.Id
ORDER BY jc.Id DESC;

PRINT 'Migration 059 completed successfully!';
GO
