-- Migration 086: Add dispatch tracking fields
-- Adds QtyDispatched to OrderItems for partial dispatch tracking
-- Adds OrderItemId + InvoiceDocument to DeliveryChallans

-- Track how much of each order item has been dispatched
ALTER TABLE Orders_OrderItems
    ADD QtyDispatched INT NOT NULL DEFAULT 0;

-- Link each challan to a specific order item (for multi-product support)
ALTER TABLE Dispatch_DeliveryChallans
    ADD OrderItemId INT NULL,
        InvoiceDocument NVARCHAR(500) NULL;

-- Create wwwroot upload folder path index (not a DB change - just a note)
-- Files stored at: wwwroot/uploads/dispatch/

PRINT 'Migration 086: Dispatch tracking fields added.';
