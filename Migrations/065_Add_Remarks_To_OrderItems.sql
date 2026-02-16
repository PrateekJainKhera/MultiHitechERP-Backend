-- ================================================================
-- Migration 065: Add Remarks to OrderItems
-- ================================================================
-- Add optional Remarks field to Orders_OrderItems table
-- Each product in an order can have its own remarks/notes
-- ================================================================

BEGIN TRANSACTION;

PRINT 'Starting Migration 065: Add Remarks to OrderItems';

-- Add Remarks column to Orders_OrderItems
ALTER TABLE Orders_OrderItems
ADD Remarks NVARCHAR(500) NULL;

PRINT 'Added Remarks column to Orders_OrderItems';

-- Verify the change
SELECT
    TABLE_NAME,
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Orders_OrderItems'
  AND COLUMN_NAME = 'Remarks';

COMMIT TRANSACTION;

PRINT 'Migration 065 completed successfully';
PRINT 'OrderItems now support optional remarks for each product';
