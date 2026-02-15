-- ================================================
-- Migration 060: Add Drawing Review to OrderItems
-- ================================================
-- Purpose: Enable item-level drawing review for multi-product orders
-- Each order item can be independently reviewed and approved
-- ================================================

USE MultiHitechERP;
GO

PRINT '========================================';
PRINT 'Migration 060: Adding Drawing Review to OrderItems';
PRINT '========================================';
GO

-- Step 1: Add Drawing Review columns to OrderItems
PRINT 'Step 1: Adding Drawing Review columns to Orders_OrderItems...';

ALTER TABLE Orders_OrderItems
ADD DrawingReviewStatus NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    DrawingReviewedBy NVARCHAR(100) NULL,
    DrawingReviewedAt DATETIME NULL,
    DrawingReviewNotes NVARCHAR(1000) NULL;

PRINT 'Drawing Review columns added successfully';
GO

-- Step 2: Add check constraint for DrawingReviewStatus
PRINT 'Step 2: Adding check constraint for DrawingReviewStatus...';

ALTER TABLE Orders_OrderItems
ADD CONSTRAINT CHK_OrderItems_DrawingReviewStatus
    CHECK (DrawingReviewStatus IN ('Pending', 'UnderReview', 'Approved', 'Rejected', 'RevisionRequired'));

PRINT 'Check constraint added successfully';
GO

-- Step 3: Create index for DrawingReviewStatus
PRINT 'Step 3: Creating index for DrawingReviewStatus...';

CREATE INDEX IX_OrderItems_DrawingReviewStatus
ON Orders_OrderItems(DrawingReviewStatus);

PRINT 'Index created successfully';
GO

-- Step 4: Backfill existing OrderItems
-- Set all existing items to 'Approved' so they can proceed
PRINT 'Step 4: Backfilling existing order items...';

UPDATE Orders_OrderItems
SET DrawingReviewStatus = 'Approved',
    DrawingReviewedBy = 'System Migration',
    DrawingReviewedAt = GETUTCDATE(),
    DrawingReviewNotes = 'Auto-approved during migration to item-level drawing review'
WHERE DrawingReviewStatus = 'Pending';

PRINT 'Backfilled ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' existing order items as Approved';
GO

PRINT '';
PRINT '========================================';
PRINT 'Migration 060: COMPLETED';
PRINT '========================================';
PRINT 'OrderItems now support independent drawing review';
PRINT 'Each item can be approved separately for planning';
GO

-- Verification
PRINT 'Verification - Sample OrderItems with Drawing Review Status:';
SELECT TOP 10
    Id,
    OrderId,
    ItemSequence,
    ProductId,
    DrawingReviewStatus,
    DrawingReviewedBy,
    DrawingReviewedAt
FROM Orders_OrderItems
ORDER BY CreatedAt DESC;
GO
