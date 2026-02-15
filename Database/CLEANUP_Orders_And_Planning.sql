-- ================================================
-- CLEANUP SCRIPT: Orders and Planning Data
-- ================================================
-- WARNING: This will DELETE all orders, order items, job cards,
-- and reset drawing reviews. Use with caution!
-- ================================================

USE MultiHitechERP;
GO

PRINT '========================================';
PRINT 'CLEANING UP ORDERS AND PLANNING DATA';
PRINT '========================================';
GO

-- Step 1: Delete Job Card Dependencies
PRINT 'Step 1: Deleting job card dependencies...';
DELETE FROM Planning_JobCardDependencies;
PRINT 'Deleted ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' job card dependencies';
GO

-- Step 2: Delete Job Card Material Requirements
PRINT 'Step 2: Deleting job card material requirements...';
DELETE FROM Planning_JobCardMaterialRequirements;
PRINT 'Deleted ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' material requirements';
GO

-- Step 3: Delete Production Entries
PRINT 'Step 3: Deleting production entries...';
DELETE FROM Production_Entries;
PRINT 'Deleted ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' production entries';
GO

-- Step 4: Delete Schedules
PRINT 'Step 4: Deleting schedules...';
DELETE FROM Planning_Schedules;
PRINT 'Deleted ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' schedules';
GO

-- Step 5: Delete Job Cards
PRINT 'Step 5: Deleting job cards...';
DELETE FROM Planning_JobCards;
PRINT 'Deleted ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' job cards';
GO

-- Step 6: Delete Material Requisitions
PRINT 'Step 6: Deleting material requisitions...';
DELETE FROM Stores_MaterialRequisitions;
PRINT 'Deleted ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' material requisitions';
GO

-- Step 7: Delete Order Items (multi-product orders)
PRINT 'Step 7: Deleting order items...';
DELETE FROM Orders_OrderItems;
PRINT 'Deleted ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' order items';
GO

-- Step 8: Delete Orders
PRINT 'Step 8: Deleting orders...';
DELETE FROM Orders_Orders;
PRINT 'Deleted ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' orders';
GO

-- Step 9: Reset Drawing Review Status (if needed)
PRINT 'Step 9: Resetting drawing reviews...';
UPDATE Masters_Drawings
SET Status = 'draft',
    ReviewedBy = NULL,
    ReviewedAt = NULL,
    ApprovedBy = NULL,
    ApprovedAt = NULL
WHERE Status IN ('under_review', 'approved', 'rejected');
PRINT 'Reset ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' drawing reviews';
GO

-- Step 10: Delete Delivery Challans (if any)
PRINT 'Step 10: Deleting delivery challans...';
DELETE FROM Dispatch_DeliveryChallans;
PRINT 'Deleted ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' delivery challans';
GO

-- Step 11: Delete Child Part Production Records (if any)
PRINT 'Step 11: Deleting child part production...';
DELETE FROM Production_ChildParts;
PRINT 'Deleted ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' child part production records';
GO

PRINT '';
PRINT '========================================';
PRINT 'CLEANUP COMPLETE!';
PRINT '========================================';
PRINT 'Database is now clean and ready for testing multi-product orders.';
PRINT '';

-- Verification: Show remaining counts
PRINT 'Verification - Remaining Counts:';
SELECT 'Orders' AS TableName, COUNT(*) AS RemainingCount FROM Orders_Orders
UNION ALL
SELECT 'OrderItems', COUNT(*) FROM Orders_OrderItems
UNION ALL
SELECT 'JobCards', COUNT(*) FROM Planning_JobCards
UNION ALL
SELECT 'JobCardDependencies', COUNT(*) FROM Planning_JobCardDependencies
UNION ALL
SELECT 'MaterialRequirements', COUNT(*) FROM Planning_JobCardMaterialRequirements
UNION ALL
SELECT 'Schedules', COUNT(*) FROM Planning_Schedules
UNION ALL
SELECT 'ProductionEntries', COUNT(*) FROM Production_Entries
UNION ALL
SELECT 'MaterialRequisitions', COUNT(*) FROM Stores_MaterialRequisitions
UNION ALL
SELECT 'DeliveryChallans', COUNT(*) FROM Dispatch_DeliveryChallans
UNION ALL
SELECT 'ChildPartProduction', COUNT(*) FROM Production_ChildParts;

PRINT '';
PRINT 'You can now create fresh multi-product orders for testing!';
GO
