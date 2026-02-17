-- Migration 080: Create Purchase Order header table

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Procurement_PurchaseOrders')
BEGIN
    CREATE TABLE Procurement_PurchaseOrders (
        Id                    INT PRIMARY KEY IDENTITY(1,1),
        PONumber              NVARCHAR(20)  NOT NULL,
        PurchaseRequestId     INT           NULL,
        VendorId              INT           NOT NULL,
        Status                NVARCHAR(20)  NOT NULL DEFAULT 'Draft',
        TotalAmount           DECIMAL(18,2) NULL,
        ExpectedDeliveryDate  DATE          NULL,
        Notes                 NVARCHAR(500) NULL,
        CreatedAt             DATETIME2     NOT NULL DEFAULT GETDATE(),
        CreatedBy             NVARCHAR(100) NULL,
        CONSTRAINT UQ_PurchaseOrders_PONumber UNIQUE (PONumber),
        CONSTRAINT FK_PO_PR FOREIGN KEY (PurchaseRequestId)
            REFERENCES Procurement_PurchaseRequests(Id),
        CONSTRAINT FK_PO_Vendor FOREIGN KEY (VendorId)
            REFERENCES Masters_Vendors(Id),
        CONSTRAINT CHK_PO_Status CHECK (Status IN ('Draft','Sent','Cancelled'))
    );

    CREATE INDEX IX_PO_VendorId ON Procurement_PurchaseOrders (VendorId);
    CREATE INDEX IX_PO_PRId     ON Procurement_PurchaseOrders (PurchaseRequestId);

    PRINT 'Procurement_PurchaseOrders table created successfully.';
END
ELSE
BEGIN
    PRINT 'Procurement_PurchaseOrders table already exists. Skipping.';
END
