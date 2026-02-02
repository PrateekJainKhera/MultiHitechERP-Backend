-- =============================================
-- MultiHitech ERP - Complete Database Cleanup
-- =============================================
-- This script deletes ALL test data from all tables
-- Use with caution - this is irreversible!
-- =============================================

USE MultiHitechERP;
GO

PRINT 'Starting database cleanup...';
GO

-- Disable foreign key constraints temporarily
EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';
GO

-- =============================================
-- 1. DELETE TRANSACTION DATA (Orders, Job Cards, etc.)
-- =============================================

PRINT 'Deleting transaction data...';

-- Delete Production related data
DELETE FROM Production_JobCardExecution;
DELETE FROM Production_JobCards;
DELETE FROM Production_ChildParts;

-- Delete Quality related data
DELETE FROM Quality_Rejections;
DELETE FROM Quality_ReworkOrders;

-- Delete Dispatch data
DELETE FROM Dispatch_DeliveryChallans;

-- Delete Stores data
DELETE FROM Stores_MaterialRequisitions;
DELETE FROM Stores_MaterialIssues;
DELETE FROM Stores_Inventory;

-- Delete Planning data
DELETE FROM Planning_JobCards;

-- Delete Orders
DELETE FROM Orders;

PRINT 'Transaction data deleted.';
GO

-- =============================================
-- 2. DELETE BOM AND TEMPLATE DATA
-- =============================================

PRINT 'Deleting BOM and template data...';

-- Delete BOM related
DELETE FROM Masters_BOM;
DELETE FROM Masters_ProductTemplates;
DELETE FROM Masters_ChildPartTemplates;
DELETE FROM Masters_ProcessTemplates;

PRINT 'BOM and template data deleted.';
GO

-- =============================================
-- 3. DELETE MASTER DATA
-- =============================================

PRINT 'Deleting master data...';

-- Delete Products
DELETE FROM Masters_Products;
PRINT 'Products deleted.';

-- Delete Components
DELETE FROM Masters_Components;
PRINT 'Components deleted.';

-- Delete Raw Materials
DELETE FROM Masters_RawMaterials;
PRINT 'Raw Materials deleted.';

-- Delete Material Categories
DELETE FROM Masters_MaterialCategories;
PRINT 'Material Categories deleted.';

-- Delete Processes
DELETE FROM Masters_Processes;
PRINT 'Processes deleted.';

-- Delete Customers (keep this last as it's referenced by many tables)
DELETE FROM Masters_Customers;
PRINT 'Customers deleted.';

-- Delete Machines
DELETE FROM Masters_Machines;
PRINT 'Machines deleted.';

-- Delete Drawings
DELETE FROM Masters_Drawings;
PRINT 'Drawings deleted.';

GO

-- =============================================
-- 4. RESET IDENTITY COLUMNS (Optional)
-- =============================================

PRINT 'Resetting identity columns...';

-- Reset all identity columns to start from 1
DBCC CHECKIDENT ('Masters_Customers', RESEED, 0);
DBCC CHECKIDENT ('Masters_Products', RESEED, 0);
DBCC CHECKIDENT ('Masters_RawMaterials', RESEED, 0);
DBCC CHECKIDENT ('Masters_Components', RESEED, 0);
DBCC CHECKIDENT ('Masters_Processes', RESEED, 0);
DBCC CHECKIDENT ('Masters_MaterialCategories', RESEED, 0);
DBCC CHECKIDENT ('Masters_Machines', RESEED, 0);
DBCC CHECKIDENT ('Masters_Drawings', RESEED, 0);
DBCC CHECKIDENT ('Masters_ProcessTemplates', RESEED, 0);
DBCC CHECKIDENT ('Masters_ChildPartTemplates', RESEED, 0);
DBCC CHECKIDENT ('Masters_ProductTemplates', RESEED, 0);
DBCC CHECKIDENT ('Masters_BOM', RESEED, 0);
DBCC CHECKIDENT ('Orders', RESEED, 0);
DBCC CHECKIDENT ('Production_JobCards', RESEED, 0);
DBCC CHECKIDENT ('Production_ChildParts', RESEED, 0);
DBCC CHECKIDENT ('Production_JobCardExecution', RESEED, 0);
DBCC CHECKIDENT ('Planning_JobCards', RESEED, 0);
DBCC CHECKIDENT ('Dispatch_DeliveryChallans', RESEED, 0);
DBCC CHECKIDENT ('Stores_Inventory', RESEED, 0);
DBCC CHECKIDENT ('Stores_MaterialRequisitions', RESEED, 0);
DBCC CHECKIDENT ('Stores_MaterialIssues', RESEED, 0);
DBCC CHECKIDENT ('Quality_Rejections', RESEED, 0);
DBCC CHECKIDENT ('Quality_ReworkOrders', RESEED, 0);

PRINT 'Identity columns reset.';
GO

-- =============================================
-- 5. RE-ENABLE FOREIGN KEY CONSTRAINTS
-- =============================================

PRINT 'Re-enabling foreign key constraints...';

EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL';

PRINT 'Foreign key constraints enabled.';
GO

-- =============================================
-- 6. VERIFICATION
-- =============================================

PRINT '';
PRINT '=============================================';
PRINT 'Database cleanup completed successfully!';
PRINT '=============================================';
PRINT '';
PRINT 'Row counts after cleanup:';
PRINT '';

SELECT 'Masters_Customers' AS TableName, COUNT(*) AS RowCount FROM Masters_Customers
UNION ALL
SELECT 'Masters_Products', COUNT(*) FROM Masters_Products
UNION ALL
SELECT 'Masters_RawMaterials', COUNT(*) FROM Masters_RawMaterials
UNION ALL
SELECT 'Masters_Components', COUNT(*) FROM Masters_Components
UNION ALL
SELECT 'Masters_Processes', COUNT(*) FROM Masters_Processes
UNION ALL
SELECT 'Masters_MaterialCategories', COUNT(*) FROM Masters_MaterialCategories
UNION ALL
SELECT 'Masters_ProcessTemplates', COUNT(*) FROM Masters_ProcessTemplates
UNION ALL
SELECT 'Masters_ChildPartTemplates', COUNT(*) FROM Masters_ChildPartTemplates
UNION ALL
SELECT 'Masters_ProductTemplates', COUNT(*) FROM Masters_ProductTemplates
UNION ALL
SELECT 'Masters_BOM', COUNT(*) FROM Masters_BOM
UNION ALL
SELECT 'Orders', COUNT(*) FROM Orders
UNION ALL
SELECT 'Production_JobCards', COUNT(*) FROM Production_JobCards;

PRINT '';
PRINT 'All data has been deleted. Database is ready for fresh testing!';
PRINT '';
GO
