-- Migration 062: Add Drawing Request Tracking to Products
-- Author: System
-- Date: 2026-02-14
-- Description: Adds fields to track when drawing is requested for a product
--              Supports new workflow: Product Created → Request Drawing → Upload Drawings → Approve

USE MultiHitechERP
GO

-- =============================================
-- Step 1: Add Drawing Request Tracking Fields
-- =============================================

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('Masters_Products')
    AND name = 'DrawingRequestedAt'
)
BEGIN
    ALTER TABLE Masters_Products
    ADD DrawingRequestedAt DATETIME NULL,
        DrawingRequestedBy NVARCHAR(100) NULL;

    PRINT 'Added DrawingRequestedAt and DrawingRequestedBy columns to Masters_Products';
END
ELSE
BEGIN
    PRINT 'Drawing request columns already exist in Masters_Products';
END
GO

-- =============================================
-- Step 2: Create Index for Performance
-- =============================================

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_Products_DrawingRequestStatus'
    AND object_id = OBJECT_ID('Masters_Products')
)
BEGIN
    SET QUOTED_IDENTIFIER ON;

    CREATE NONCLUSTERED INDEX IX_Products_DrawingRequestStatus
    ON Masters_Products(DrawingReviewStatus, DrawingRequestedAt)
    WHERE DrawingRequestedAt IS NOT NULL;

    PRINT 'Created index IX_Products_DrawingRequestStatus on Masters_Products';
END
ELSE
BEGIN
    PRINT 'Index IX_Products_DrawingRequestStatus already exists';
END
GO

-- =============================================
-- Step 3: Update Existing Products
-- =============================================

-- For products that already have DrawingReviewStatus = 'UnderReview' or 'Approved',
-- set DrawingRequestedAt to their creation date (backward compatibility)
UPDATE Masters_Products
SET DrawingRequestedAt = CreatedAt,
    DrawingRequestedBy = CreatedBy
WHERE DrawingReviewStatus IN ('UnderReview', 'Approved', 'Rejected', 'RevisionRequired')
  AND DrawingRequestedAt IS NULL;

PRINT 'Updated existing products with DrawingRequestedAt for backward compatibility';
GO

-- =============================================
-- Verification
-- =============================================

SELECT
    COUNT(*) AS TotalProducts,
    SUM(CASE WHEN DrawingRequestedAt IS NOT NULL THEN 1 ELSE 0 END) AS ProductsWithDrawingRequests,
    SUM(CASE WHEN DrawingReviewStatus = 'Pending' THEN 1 ELSE 0 END) AS PendingDrawingRequest,
    SUM(CASE WHEN DrawingReviewStatus = 'UnderReview' THEN 1 ELSE 0 END) AS UnderReview,
    SUM(CASE WHEN DrawingReviewStatus = 'Approved' THEN 1 ELSE 0 END) AS Approved
FROM Masters_Products;

PRINT 'Migration 062 completed successfully!';
GO
