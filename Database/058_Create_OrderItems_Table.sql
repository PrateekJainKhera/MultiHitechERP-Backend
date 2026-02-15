-- Migration: Create OrderItems Table for Multi-Product Orders
-- Description: Enable multiple products per order with backward compatibility
-- Date: 2026-02-14

USE MultiHitechERP;
GO

-- =============================================
-- 1. CREATE OrderItems Table
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Orders_OrderItems')
BEGIN
    CREATE TABLE Orders_OrderItems (
        -- Identity
        Id INT IDENTITY(1,1) PRIMARY KEY,
        OrderId INT NOT NULL,
        ItemSequence NVARCHAR(5) NOT NULL, -- A, B, C, D, etc.

        -- Product Reference
        ProductId INT NOT NULL,
        ProductName NVARCHAR(200) NULL, -- Denormalized for performance

        -- Quantities
        Quantity INT NOT NULL,
        QtyCompleted INT NOT NULL DEFAULT 0,
        QtyRejected INT NOT NULL DEFAULT 0,
        QtyInProgress INT NOT NULL DEFAULT 0,
        QtyScrap INT NOT NULL DEFAULT 0,

        -- Item-Specific Scheduling
        DueDate DATETIME NOT NULL,
        Priority NVARCHAR(20) NOT NULL DEFAULT 'Medium', -- Low, Medium, High, Urgent

        -- Item-Specific Status
        Status NVARCHAR(20) NOT NULL DEFAULT 'Pending', -- Pending, In Progress, Completed, Dispatched
        PlanningStatus NVARCHAR(20) NOT NULL DEFAULT 'Not Planned',

        -- Drawing Linkage (Item-specific)
        PrimaryDrawingId INT NULL,
        DrawingSource NVARCHAR(20) NULL, -- 'customer' or 'company'

        -- Template Linkage
        LinkedProductTemplateId INT NULL,

        -- Production Tracking
        CurrentProcess NVARCHAR(200) NULL,
        CurrentMachine NVARCHAR(100) NULL,
        CurrentOperator NVARCHAR(100) NULL,
        ProductionStartDate DATETIME NULL,
        ProductionEndDate DATETIME NULL,

        -- Material Approval (Item-specific)
        MaterialGradeApproved BIT NOT NULL DEFAULT 0,
        MaterialGradeApprovalDate DATETIME NULL,
        MaterialGradeApprovedBy NVARCHAR(100) NULL,
        MaterialGradeRemark NVARCHAR(500) NULL,

        -- Audit
        CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
        CreatedBy NVARCHAR(100) NOT NULL,
        UpdatedAt DATETIME NULL,
        UpdatedBy NVARCHAR(100) NULL,

        -- Constraints
        CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId)
            REFERENCES Orders(Id) ON DELETE CASCADE,
        CONSTRAINT FK_OrderItems_Products FOREIGN KEY (ProductId)
            REFERENCES Masters_Products(Id),
        CONSTRAINT FK_OrderItems_Drawings FOREIGN KEY (PrimaryDrawingId)
            REFERENCES Masters_Drawings(Id),
        CONSTRAINT FK_OrderItems_ProductTemplates FOREIGN KEY (LinkedProductTemplateId)
            REFERENCES Masters_ProductTemplates(Id),
        CONSTRAINT UQ_OrderItems_OrderId_ItemSequence UNIQUE (OrderId, ItemSequence),
        CONSTRAINT CHK_OrderItems_Quantity CHECK (Quantity > 0),
        CONSTRAINT CHK_OrderItems_ItemSequence CHECK (LEN(ItemSequence) > 0)
    );

    PRINT '✓ Created Orders_OrderItems table';
END
ELSE
BEGIN
    PRINT '⚠ Orders_OrderItems table already exists';
END
GO

-- =============================================
-- 2. CREATE Indexes for Performance
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OrderItems_OrderId')
BEGIN
    CREATE INDEX IX_OrderItems_OrderId ON Orders_OrderItems(OrderId);
    PRINT '✓ Created index on OrderId';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OrderItems_ProductId')
BEGIN
    CREATE INDEX IX_OrderItems_ProductId ON Orders_OrderItems(ProductId);
    PRINT '✓ Created index on ProductId';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OrderItems_Status')
BEGIN
    CREATE INDEX IX_OrderItems_Status ON Orders_OrderItems(Status);
    PRINT '✓ Created index on Status';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OrderItems_DueDate')
BEGIN
    CREATE INDEX IX_OrderItems_DueDate ON Orders_OrderItems(DueDate);
    PRINT '✓ Created index on DueDate';
END
GO

-- =============================================
-- 3. MIGRATE Existing Orders to OrderItems
-- =============================================

PRINT '📦 Starting data migration...';

-- Count existing orders
DECLARE @ExistingOrdersCount INT;
SELECT @ExistingOrdersCount = COUNT(*) FROM Orders;
PRINT 'Found ' + CAST(@ExistingOrdersCount AS NVARCHAR) + ' existing orders to migrate';

-- Migrate all existing orders → Create OrderItem with sequence 'A'
INSERT INTO Orders_OrderItems (
    OrderId,
    ItemSequence,
    ProductId,
    ProductName,
    Quantity,
    QtyCompleted,
    QtyRejected,
    QtyInProgress,
    QtyScrap,
    DueDate,
    Priority,
    Status,
    PlanningStatus,
    PrimaryDrawingId,
    DrawingSource,
    LinkedProductTemplateId,
    CurrentProcess,
    CurrentMachine,
    CurrentOperator,
    ProductionStartDate,
    ProductionEndDate,
    MaterialGradeApproved,
    MaterialGradeApprovalDate,
    MaterialGradeApprovedBy,
    MaterialGradeRemark,
    CreatedAt,
    CreatedBy,
    UpdatedAt,
    UpdatedBy
)
SELECT
    o.Id AS OrderId,
    'A' AS ItemSequence, -- All existing orders get sequence 'A'
    o.ProductId,
    o.ProductName,
    o.Quantity,
    o.QtyCompleted,
    o.QtyRejected,
    o.QtyInProgress,
    o.QtyScrap,
    o.DueDate,
    o.Priority,
    o.Status,
    o.PlanningStatus,
    o.PrimaryDrawingId,
    o.DrawingSource,
    o.LinkedProductTemplateId,
    o.CurrentProcess,
    o.CurrentMachine,
    o.CurrentOperator,
    o.ProductionStartDate,
    o.ProductionEndDate,
    o.MaterialGradeApproved,
    o.MaterialGradeApprovalDate,
    o.MaterialGradeApprovedBy,
    o.MaterialGradeRemark,
    o.CreatedAt,
    ISNULL(o.CreatedBy, 'System'),
    o.UpdatedAt,
    o.UpdatedBy
FROM Orders o
WHERE NOT EXISTS (
    -- Don't duplicate if already migrated
    SELECT 1 FROM Orders_OrderItems oi
    WHERE oi.OrderId = o.Id AND oi.ItemSequence = 'A'
);

-- Count migrated items
DECLARE @MigratedItemsCount INT;
SELECT @MigratedItemsCount = COUNT(*) FROM Orders_OrderItems WHERE ItemSequence = 'A';
PRINT '✓ Migrated ' + CAST(@MigratedItemsCount AS NVARCHAR) + ' existing orders to OrderItems (sequence A)';

GO

-- =============================================
-- 4. VERIFY Data Integrity
-- =============================================

PRINT '🔍 Verifying data integrity...';

-- Check if all orders have at least one item
DECLARE @OrdersWithoutItems INT;
SELECT @OrdersWithoutItems = COUNT(*)
FROM Orders o
WHERE NOT EXISTS (SELECT 1 FROM Orders_OrderItems oi WHERE oi.OrderId = o.Id);

IF @OrdersWithoutItems = 0
    PRINT '✓ All orders have OrderItems';
ELSE
    PRINT '⚠ WARNING: ' + CAST(@OrdersWithoutItems AS NVARCHAR) + ' orders have no items!';

-- Summary
DECLARE @TotalOrders INT, @TotalItems INT;
SELECT @TotalOrders = COUNT(*) FROM Orders;
SELECT @TotalItems = COUNT(*) FROM Orders_OrderItems;

PRINT '';
PRINT '========================================';
PRINT 'MIGRATION SUMMARY';
PRINT '========================================';
PRINT 'Total Orders: ' + CAST(@TotalOrders AS NVARCHAR);
PRINT 'Total Order Items: ' + CAST(@TotalItems AS NVARCHAR);
PRINT 'Average Items per Order: ' + CAST(@TotalItems / NULLIF(@TotalOrders, 0) AS NVARCHAR);
PRINT '========================================';
PRINT '';
PRINT '✅ Migration 058: OrderItems table created successfully!';
PRINT '';
PRINT 'NEXT STEPS:';
PRINT '1. Create OrderItem model in backend';
PRINT '2. Create OrderItem repository';
PRINT '3. Update Order APIs to support multiple items';
PRINT '4. Update frontend order creation form';
PRINT '';

GO
