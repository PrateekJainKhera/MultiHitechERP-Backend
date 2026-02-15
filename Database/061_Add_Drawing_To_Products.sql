-- ================================================
-- Migration 061: Add Drawing Review to Product Master
-- ================================================
-- Purpose: Implement product-centric drawing architecture
-- Drawings are linked to PRODUCTS (not orders)
-- Same product used by multiple customers shares ONE set of approved drawings
-- ================================================

USE MultiHitechERP;
GO

PRINT '========================================';
PRINT 'Migration 061: Adding Drawing Review to Product Master';
PRINT '========================================';
GO

-- Step 1: Add Drawing fields to Masters_Products
PRINT 'Step 1: Adding Drawing columns to Masters_Products...';

ALTER TABLE Masters_Products
ADD
    -- Assembly Drawing (main product drawing)
    AssemblyDrawingId INT NULL,

    -- Optional customer-provided reference drawing
    CustomerProvidedDrawingId INT NULL,

    -- Drawing Review Status (PRODUCT-LEVEL GATE)
    DrawingReviewStatus NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    DrawingReviewedBy NVARCHAR(100) NULL,
    DrawingReviewedAt DATETIME NULL,
    DrawingReviewNotes NVARCHAR(1000) NULL;

PRINT 'Drawing columns added to Masters_Products successfully';
GO

-- Step 2: Add foreign key constraints
PRINT 'Step 2: Adding foreign key constraints...';

-- Note: Masters_Drawings table uses UNIQUEIDENTIFIER as PK, so we need to check compatibility
-- If your Products table uses INT and Drawings uses UNIQUEIDENTIFIER, we may need to adjust
-- For now, commenting out FK constraints - will add after verifying schema compatibility

-- ALTER TABLE Masters_Products
-- ADD CONSTRAINT FK_Products_AssemblyDrawing
--     FOREIGN KEY (AssemblyDrawingId) REFERENCES Masters_Drawings(Id);

-- ALTER TABLE Masters_Products
-- ADD CONSTRAINT FK_Products_CustomerDrawing
--     FOREIGN KEY (CustomerProvidedDrawingId) REFERENCES Masters_Drawings(Id);

PRINT 'Foreign key constraints skipped - will be added after schema verification';
GO

-- Step 3: Add check constraint for DrawingReviewStatus
PRINT 'Step 3: Adding check constraint for DrawingReviewStatus...';

ALTER TABLE Masters_Products
ADD CONSTRAINT CHK_Products_DrawingReviewStatus
    CHECK (DrawingReviewStatus IN ('Pending', 'UnderReview', 'Approved', 'Rejected', 'RevisionRequired'));

PRINT 'Check constraint added successfully';
GO

-- Step 4: Create index for DrawingReviewStatus
PRINT 'Step 4: Creating index for DrawingReviewStatus...';

CREATE INDEX IX_Products_DrawingReviewStatus
ON Masters_Products(DrawingReviewStatus);

PRINT 'Index created successfully';
GO

-- Step 5: Create Product_ChildPartDrawings table
PRINT 'Step 5: Creating Product_ChildPartDrawings table...';

CREATE TABLE Product_ChildPartDrawings (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProductId INT NOT NULL,
    ChildPartTemplateId INT NOT NULL,
    DrawingId INT NOT NULL,

    -- Tracking
    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100) NULL,
    UpdatedAt DATETIME NULL,
    UpdatedBy NVARCHAR(100) NULL,

    -- Constraints
    CONSTRAINT FK_ChildPartDrawings_Product
        FOREIGN KEY (ProductId) REFERENCES Masters_Products(Id) ON DELETE CASCADE,

    -- Note: FK to Masters_Drawings commented out - will add after schema verification
    -- CONSTRAINT FK_ChildPartDrawings_Drawing
    --     FOREIGN KEY (DrawingId) REFERENCES Masters_Drawings(Id),

    -- One drawing per child part per product
    CONSTRAINT UQ_Product_ChildPart_Drawing
        UNIQUE (ProductId, ChildPartTemplateId)
);

PRINT 'Product_ChildPartDrawings table created successfully';
GO

-- Step 6: Create indexes for Product_ChildPartDrawings
PRINT 'Step 6: Creating indexes for Product_ChildPartDrawings...';

CREATE INDEX IX_ChildPartDrawings_Product
ON Product_ChildPartDrawings(ProductId);

CREATE INDEX IX_ChildPartDrawings_ChildPartTemplate
ON Product_ChildPartDrawings(ChildPartTemplateId);

CREATE INDEX IX_ChildPartDrawings_Drawing
ON Product_ChildPartDrawings(DrawingId);

PRINT 'Indexes created successfully';
GO

-- Step 7: Backfill existing products
PRINT 'Step 7: Backfilling existing products...';

-- Set all existing products to 'Pending' (default already applied)
-- If you want to auto-approve existing products, uncomment below:
-- UPDATE Masters_Products
-- SET DrawingReviewStatus = 'Approved',
--     DrawingReviewedBy = 'System Migration',
--     DrawingReviewedAt = GETUTCDATE(),
--     DrawingReviewNotes = 'Auto-approved during migration to product-level drawing review'
-- WHERE DrawingReviewStatus = 'Pending';

PRINT 'Backfill completed - all products set to Pending (default)';
GO

PRINT '';
PRINT '========================================';
PRINT 'Migration 061: COMPLETED';
PRINT '========================================';
PRINT 'Products now support drawing review at product level';
PRINT 'Each product can have:';
PRINT '  - Assembly Drawing';
PRINT '  - Customer Provided Drawing (optional)';
PRINT '  - Child Part Drawings (via Product_ChildPartDrawings table)';
PRINT 'Drawing approval happens ONCE per product';
PRINT 'All orders for that product use the same approved drawings';
GO

-- Verification
PRINT 'Verification - Sample Products with Drawing Review Status:';
SELECT TOP 10
    Id,
    PartCode,
    ModelName,
    DrawingReviewStatus,
    AssemblyDrawingId,
    CustomerProvidedDrawingId,
    DrawingReviewedBy,
    DrawingReviewedAt
FROM Masters_Products
ORDER BY CreatedAt DESC;
GO

PRINT '';
PRINT 'Verification - Product_ChildPartDrawings table structure:';
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Product_ChildPartDrawings'
ORDER BY ORDINAL_POSITION;
GO
