-- Migration 082: Create PR Item Cutting List table for Raw Material PRs
-- Raw material PRs specify total meters required, plus a cutting list
-- (how many pieces of each length to cut from the purchased material)

CREATE TABLE Procurement_PRItemCuttingList (
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    PRItemId      INT NOT NULL,
    LengthMeter   DECIMAL(10, 3) NOT NULL,
    Pieces        INT NOT NULL DEFAULT 1,
    Notes         NVARCHAR(200) NULL,
    CONSTRAINT FK_PRItemCuttingList_PRItem FOREIGN KEY (PRItemId)
        REFERENCES Procurement_PurchaseRequestItems(Id) ON DELETE CASCADE
);

CREATE INDEX IX_PRItemCuttingList_PRItemId ON Procurement_PRItemCuttingList(PRItemId);

PRINT 'Migration 082: Procurement_PRItemCuttingList table created successfully.';
