-- Migration 087: Fix Dispatch_DeliveryChallans column names + add missing columns
-- Aligns the DB table with the C# DeliveryChallan model

-- 1. Rename mismatched column names
EXEC sp_rename 'Dispatch_DeliveryChallans.VehicleNo', 'VehicleNumber', 'COLUMN';
EXEC sp_rename 'Dispatch_DeliveryChallans.DispatchDate', 'DispatchedAt', 'COLUMN';

-- 2. Add missing columns
ALTER TABLE Dispatch_DeliveryChallans
    ADD ProductId          INT             NULL,
        ProductName        NVARCHAR(200)   NULL,
        QuantityDispatched INT             NOT NULL DEFAULT 0,
        NumberOfPackages   INT             NULL,
        PackagingType      NVARCHAR(100)   NULL,
        TotalWeight        DECIMAL(18,2)   NULL,
        InvoiceNo          NVARCHAR(100)   NULL,
        InvoiceDate        DATETIME2       NULL,
        DeliveredAt        DATETIME2       NULL,
        AcknowledgedAt     DATETIME2       NULL,
        DeliveryRemarks    NVARCHAR(500)   NULL;

PRINT 'Migration 087: Dispatch_DeliveryChallans columns fixed successfully.';
