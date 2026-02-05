/*
    Migration: Add LinkedOrderId to Masters_Drawings
    Purpose: Support multiple drawings per order
    Date: 2025-02-05

    This migration adds the ability to link multiple drawings to a single order.
    Each drawing can now be associated with an order, enabling support for:
    - Shaft drawings
    - Tikki drawings
    - Gear drawings
    - End cap drawings
    - Bearing drawings
    - Patti drawings
    - Assembly drawings
    - And more...
*/

-- Step 1: Add LinkedOrderId column to Masters_Drawings
IF NOT EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'Masters_Drawings'
    AND COLUMN_NAME = 'LinkedOrderId'
)
BEGIN
    PRINT 'Adding LinkedOrderId column to Masters_Drawings table...'

    ALTER TABLE Masters_Drawings
    ADD LinkedOrderId INT NULL;

    PRINT 'LinkedOrderId column added successfully.'
END
ELSE
BEGIN
    PRINT 'LinkedOrderId column already exists in Masters_Drawings table.'
END
GO

-- Step 2: Add foreign key constraint
IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_Masters_Drawings_Orders'
)
BEGIN
    PRINT 'Adding foreign key constraint FK_Masters_Drawings_Orders...'

    ALTER TABLE Masters_Drawings
    ADD CONSTRAINT FK_Masters_Drawings_Orders
    FOREIGN KEY (LinkedOrderId) REFERENCES Orders(Id)
    ON DELETE SET NULL;

    PRINT 'Foreign key constraint added successfully.'
END
ELSE
BEGIN
    PRINT 'Foreign key constraint FK_Masters_Drawings_Orders already exists.'
END
GO

-- Step 3: Add index for performance
IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_Masters_Drawings_LinkedOrderId'
    AND object_id = OBJECT_ID('Masters_Drawings')
)
BEGIN
    PRINT 'Creating index IX_Masters_Drawings_LinkedOrderId...'

    CREATE NONCLUSTERED INDEX IX_Masters_Drawings_LinkedOrderId
    ON Masters_Drawings(LinkedOrderId)
    WHERE LinkedOrderId IS NOT NULL;

    PRINT 'Index created successfully.'
END
ELSE
BEGIN
    PRINT 'Index IX_Masters_Drawings_LinkedOrderId already exists.'
END
GO

-- Step 4: Update DrawingType to support new types
-- Add comment documenting the expanded drawing types
EXEC sp_updateextendedproperty
    @name = N'MS_Description',
    @value = N'Type of drawing: shaft, pipe, final, gear, bushing, roller, tikki, ends, bearing, patti, assembly, other',
    @level0type = N'SCHEMA', @level0name = 'dbo',
    @level1type = N'TABLE',  @level1name = 'Masters_Drawings',
    @level2type = N'COLUMN', @level2name = 'DrawingType';
GO

PRINT ''
PRINT '========================================='
PRINT 'Migration completed successfully!'
PRINT '========================================='
PRINT 'Summary:'
PRINT '- LinkedOrderId column added'
PRINT '- Foreign key constraint created'
PRINT '- Performance index added'
PRINT '- DrawingType documentation updated'
PRINT ''
PRINT 'Next steps:'
PRINT '1. Run this script in SSMS'
PRINT '2. Backend API already supports GetByOrderId'
PRINT '3. Frontend Drawing Review page will display order drawings'
PRINT '========================================='
GO
