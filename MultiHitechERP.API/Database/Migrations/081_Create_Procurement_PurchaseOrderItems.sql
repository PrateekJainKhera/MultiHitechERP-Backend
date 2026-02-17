-- Migration 081: Create Purchase Order Items table

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Procurement_PurchaseOrderItems')
BEGIN
    CREATE TABLE Procurement_PurchaseOrderItems (
        Id                      INT PRIMARY KEY IDENTITY(1,1),
        PurchaseOrderId         INT           NOT NULL,
        PurchaseRequestItemId   INT           NULL,
        ItemType                NVARCHAR(20)  NOT NULL,  -- 'Component' or 'RawMaterial'
        ItemId                  INT           NOT NULL,
        ItemName                NVARCHAR(200) NOT NULL,
        ItemCode                NVARCHAR(50)  NULL,
        Unit                    NVARCHAR(20)  NOT NULL,
        OrderedQty              DECIMAL(18,3) NOT NULL,
        UnitCost                DECIMAL(18,2) NULL,
        TotalCost               DECIMAL(18,2) NULL,
        CONSTRAINT FK_POItems_PO FOREIGN KEY (PurchaseOrderId)
            REFERENCES Procurement_PurchaseOrders(Id),
        CONSTRAINT FK_POItems_PRItem FOREIGN KEY (PurchaseRequestItemId)
            REFERENCES Procurement_PurchaseRequestItems(Id)
    );

    CREATE INDEX IX_POItems_PurchaseOrderId
        ON Procurement_PurchaseOrderItems (PurchaseOrderId);

    PRINT 'Procurement_PurchaseOrderItems table created successfully.';
END
ELSE
BEGIN
    PRINT 'Procurement_PurchaseOrderItems table already exists. Skipping.';
END
