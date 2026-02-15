-- ================================================
-- CLEANUP SCRIPT: Fresh Start for Multi-Product Orders
-- ================================================
-- Deletes all orders, order items, job cards, and related data
-- ================================================

USE MultiHitechERP;
GO

PRINT '========================================';
PRINT 'STARTING FRESH CLEANUP';
PRINT '========================================';
GO

-- Step 1: Delete Machine Schedules (references JobCards)
PRINT 'Step 1: Deleting machine schedules...';
DELETE FROM Scheduling_MachineSchedules;
PRINT 'Deleted ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' machine schedules';
GO

-- Step 2: Delete Job Card Dependencies
PRINT 'Step 2: Deleting job card dependencies...';
DELETE FROM Planning_JobCardDependencies;
PRINT 'Deleted ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' job card dependencies';
GO

-- Step 3: Delete Job Card Material Requirements
PRINT 'Step 3: Deleting job card material requirements...';
DELETE FROM Planning_JobCardMaterialRequirements;
PRINT 'Deleted ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' material requirements';
GO

-- Step 4: Delete Job Cards (now safe, no FK dependencies)
PRINT 'Step 4: Deleting job cards...';
DELETE FROM Planning_JobCards;
PRINT 'Deleted ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' job cards';
GO

-- Step 5: Delete Material Requisitions (may reference orders)
PRINT 'Step 5: Deleting material requisitions...';
DELETE FROM Stores_MaterialRequisitions;
PRINT 'Deleted ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' material requisitions';
GO

-- Step 6: Delete Delivery Challans (reference orders)
PRINT 'Step 6: Deleting delivery challans...';
DELETE FROM Dispatch_DeliveryChallans;
PRINT 'Deleted ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' delivery challans';
GO

-- Step 7: Delete Order Items (now safe, JobCards deleted)
PRINT 'Step 7: Deleting order items...';
DELETE FROM Orders_OrderItems;
PRINT 'Deleted ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' order items';
GO

-- Step 8: Delete Orders (now safe, all dependencies gone)
PRINT 'Step 8: Deleting orders...';
DELETE FROM Orders;
PRINT 'Deleted ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' orders';
GO

PRINT '';
PRINT '========================================';
PRINT 'CLEANUP COMPLETE!';
PRINT '========================================';
GO

-- Verification
PRINT 'Verification - Remaining Counts:';
SELECT 'Orders' AS TableName, COUNT(*) AS Count FROM Orders
UNION ALL
SELECT 'OrderItems', COUNT(*) FROM Orders_OrderItems
UNION ALL
SELECT 'JobCards', COUNT(*) FROM Planning_JobCards
UNION ALL
SELECT 'MachineSchedules', COUNT(*) FROM Scheduling_MachineSchedules
UNION ALL
SELECT 'MaterialRequirements', COUNT(*) FROM Planning_JobCardMaterialRequirements
UNION ALL
SELECT 'MaterialRequisitions', COUNT(*) FROM Stores_MaterialRequisitions
UNION ALL
SELECT 'DeliveryChallans', COUNT(*) FROM Dispatch_DeliveryChallans;

PRINT '';
PRINT '✓ Database is clean!';
PRINT '✓ Ready to test multi-product orders!';
GO
