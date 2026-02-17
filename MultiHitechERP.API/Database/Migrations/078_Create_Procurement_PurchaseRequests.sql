-- Migration 078: Create Purchase Request header table

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Procurement_PurchaseRequests')
BEGIN
    CREATE TABLE Procurement_PurchaseRequests (
        Id              INT PRIMARY KEY IDENTITY(1,1),
        PRNumber        NVARCHAR(20)  NOT NULL,
        ItemType        NVARCHAR(20)  NOT NULL,  -- 'Component' or 'RawMaterial'
        RequestedBy     NVARCHAR(100) NOT NULL DEFAULT 'Admin',
        RequestDate     DATETIME2     NOT NULL DEFAULT GETDATE(),
        Notes           NVARCHAR(500) NULL,
        Status          NVARCHAR(20)  NOT NULL DEFAULT 'Draft',
        RejectionReason NVARCHAR(500) NULL,
        ApprovedBy      NVARCHAR(100) NULL,
        ApprovedAt      DATETIME2     NULL,
        CreatedAt       DATETIME2     NOT NULL DEFAULT GETDATE(),
        CONSTRAINT UQ_PurchaseRequests_PRNumber UNIQUE (PRNumber),
        CONSTRAINT CHK_PR_Status CHECK (Status IN ('Draft','Submitted','UnderApproval','Approved','Rejected','POGenerated')),
        CONSTRAINT CHK_PR_ItemType CHECK (ItemType IN ('Component','RawMaterial'))
    );

    PRINT 'Procurement_PurchaseRequests table created successfully.';
END
ELSE
BEGIN
    PRINT 'Procurement_PurchaseRequests table already exists. Skipping.';
END
