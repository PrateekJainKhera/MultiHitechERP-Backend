-- Migration 093: Recreate Inventory_Transactions table
-- This table was dropped in migration 042 but is still used by InventoryRepository
-- for audit trail of stock movements (receipts, issues, adjustments)

IF OBJECT_ID('dbo.Inventory_Transactions', 'U') IS NULL
BEGIN
    CREATE TABLE Inventory_Transactions (
        Id              INT IDENTITY(1,1) PRIMARY KEY,
        MaterialId      INT NOT NULL,           -- ComponentId or MaterialId depending on ItemType
        TransactionType NVARCHAR(50)  NOT NULL, -- ComponentReceipt, StockIn, StockOut, Adjustment, etc.
        TransactionNo   NVARCHAR(100) NOT NULL,
        TransactionDate DATETIME      NOT NULL,
        Quantity        DECIMAL(18,4) NOT NULL,
        UOM             NVARCHAR(20)  NOT NULL,
        ReferenceType   NVARCHAR(50)  NULL,
        ReferenceId     INT           NULL,
        ReferenceNo     NVARCHAR(100) NULL,
        FromLocation    NVARCHAR(200) NULL,
        ToLocation      NVARCHAR(200) NULL,
        UnitCost        DECIMAL(18,4) NULL,
        TotalCost       DECIMAL(18,4) NULL,
        BalanceQuantity DECIMAL(18,4) NOT NULL DEFAULT 0,
        Remarks         NVARCHAR(500) NULL,
        PerformedBy     NVARCHAR(100) NULL,
        JobCardId       INT           NULL,
        RequisitionId   INT           NULL,
        SupplierId      INT           NULL,
        GRNNo           NVARCHAR(100) NULL,
        CreatedAt       DATETIME      NOT NULL DEFAULT GETDATE(),
        CreatedBy       NVARCHAR(100) NULL
    );

    CREATE INDEX IX_Inventory_Transactions_MaterialId   ON Inventory_Transactions (MaterialId);
    CREATE INDEX IX_Inventory_Transactions_TransactionType ON Inventory_Transactions (TransactionType);
    CREATE INDEX IX_Inventory_Transactions_TransactionDate ON Inventory_Transactions (TransactionDate DESC);
    CREATE INDEX IX_Inventory_Transactions_JobCardId    ON Inventory_Transactions (JobCardId);

    PRINT 'Created Inventory_Transactions table';
END
ELSE
BEGIN
    PRINT 'Inventory_Transactions table already exists. Skipping.';
END
