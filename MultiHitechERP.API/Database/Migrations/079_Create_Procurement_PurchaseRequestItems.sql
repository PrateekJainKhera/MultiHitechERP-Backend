-- Migration 079: Create Purchase Request Items table

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Procurement_PurchaseRequestItems')
BEGIN
    CREATE TABLE Procurement_PurchaseRequestItems (
        Id                  INT PRIMARY KEY IDENTITY(1,1),
        PurchaseRequestId   INT           NOT NULL,
        ItemType            NVARCHAR(20)  NOT NULL,  -- 'Component' or 'RawMaterial'
        ItemId              INT           NOT NULL,
        ItemName            NVARCHAR(200) NOT NULL,
        ItemCode            NVARCHAR(50)  NULL,
        Unit                NVARCHAR(20)  NOT NULL,
        RequestedQty        DECIMAL(18,3) NOT NULL,
        ApprovedQty         DECIMAL(18,3) NULL,
        VendorId            INT           NULL,
        EstimatedUnitCost   DECIMAL(18,2) NULL,
        Status              NVARCHAR(20)  NOT NULL DEFAULT 'Pending',
        Notes               NVARCHAR(500) NULL,
        CONSTRAINT FK_PRItems_PR FOREIGN KEY (PurchaseRequestId)
            REFERENCES Procurement_PurchaseRequests(Id),
        CONSTRAINT FK_PRItems_Vendor FOREIGN KEY (VendorId)
            REFERENCES Masters_Vendors(Id),
        CONSTRAINT CHK_PRItem_Status CHECK (Status IN ('Pending','Approved','Rejected'))
    );

    CREATE INDEX IX_PRItems_PurchaseRequestId
        ON Procurement_PurchaseRequestItems (PurchaseRequestId);

    PRINT 'Procurement_PurchaseRequestItems table created successfully.';
END
ELSE
BEGIN
    PRINT 'Procurement_PurchaseRequestItems table already exists. Skipping.';
END
