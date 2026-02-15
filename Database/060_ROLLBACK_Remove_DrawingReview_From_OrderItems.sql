-- ================================================
-- ROLLBACK Migration 060: Remove Drawing Review from OrderItems
-- ================================================
-- Purpose: Drawing review should be at PRODUCT level, not ORDER ITEM level
-- Moving to product-centric architecture where drawings are linked to products
-- ================================================

USE MultiHitechERP;
GO

PRINT '========================================';
PRINT 'ROLLBACK Migration 060: Removing Drawing Review from OrderItems';
PRINT '========================================';
GO

-- Step 1: Drop index
PRINT 'Step 1: Dropping index IX_OrderItems_DrawingReviewStatus...';
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OrderItems_DrawingReviewStatus' AND object_id = OBJECT_ID('Orders_OrderItems'))
BEGIN
    DROP INDEX IX_OrderItems_DrawingReviewStatus ON Orders_OrderItems;
    PRINT 'Index dropped successfully';
END
ELSE
BEGIN
    PRINT 'Index does not exist, skipping';
END
GO

-- Step 2: Drop check constraint
PRINT 'Step 2: Dropping check constraint CHK_OrderItems_DrawingReviewStatus...';
IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CHK_OrderItems_DrawingReviewStatus' AND parent_object_id = OBJECT_ID('Orders_OrderItems'))
BEGIN
    ALTER TABLE Orders_OrderItems DROP CONSTRAINT CHK_OrderItems_DrawingReviewStatus;
    PRINT 'Check constraint dropped successfully';
END
ELSE
BEGIN
    PRINT 'Check constraint does not exist, skipping';
END
GO

-- Step 3: Drop columns
PRINT 'Step 3: Dropping DrawingReview columns from Orders_OrderItems...';

-- Check and drop each column individually
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders_OrderItems') AND name = 'DrawingReviewNotes')
BEGIN
    ALTER TABLE Orders_OrderItems DROP COLUMN DrawingReviewNotes;
    PRINT 'Dropped DrawingReviewNotes';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders_OrderItems') AND name = 'DrawingReviewedAt')
BEGIN
    ALTER TABLE Orders_OrderItems DROP COLUMN DrawingReviewedAt;
    PRINT 'Dropped DrawingReviewedAt';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders_OrderItems') AND name = 'DrawingReviewedBy')
BEGIN
    ALTER TABLE Orders_OrderItems DROP COLUMN DrawingReviewedBy;
    PRINT 'Dropped DrawingReviewedBy';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders_OrderItems') AND name = 'DrawingReviewStatus')
BEGIN
    ALTER TABLE Orders_OrderItems DROP COLUMN DrawingReviewStatus;
    PRINT 'Dropped DrawingReviewStatus';
END

PRINT 'Drawing Review columns removed successfully';
GO

PRINT '';
PRINT '========================================';
PRINT 'ROLLBACK COMPLETED';
PRINT '========================================';
PRINT 'OrderItems no longer have drawing review fields';
PRINT 'Next: Implement product-level drawing architecture';
GO

-- Verification
PRINT 'Verification - Columns in Orders_OrderItems:';
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Orders_OrderItems'
ORDER BY ORDINAL_POSITION;
GO
