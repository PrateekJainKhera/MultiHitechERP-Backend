-- Migrate data and add constraints
USE MultiHitechERP;

-- Copy data from old columns to new ones
UPDATE [Stores_MaterialRequisitionItems]
SET
    [QuantityRequired] = [RequestedQuantity],
    [QuantityIssued] = ISNULL([IssuedQuantity], 0),
    [QuantityPending] = ISNULL([PendingQuantity], [RequestedQuantity]),
    [LineNo] = 1,
    [Status] = 'Pending',
    [CreatedAt] = GETUTCDATE()
WHERE [QuantityRequired] IS NULL;

PRINT 'Data migrated successfully';

-- Make required columns NOT NULL
ALTER TABLE [Stores_MaterialRequisitionItems]
ALTER COLUMN [QuantityRequired] DECIMAL(18, 4) NOT NULL;

ALTER TABLE [Stores_MaterialRequisitionItems]
ALTER COLUMN [LineNo] INT NOT NULL;

ALTER TABLE [Stores_MaterialRequisitionItems]
ALTER COLUMN [Status] NVARCHAR(50) NOT NULL;

ALTER TABLE [Stores_MaterialRequisitionItems]
ALTER COLUMN [CreatedAt] DATETIME NOT NULL;

PRINT 'Required columns set to NOT NULL';

-- Create indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MaterialRequisitionItems_RequisitionId')
    CREATE INDEX IX_MaterialRequisitionItems_RequisitionId ON [Stores_MaterialRequisitionItems]([RequisitionId]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MaterialRequisitionItems_MaterialId')
    CREATE INDEX IX_MaterialRequisitionItems_MaterialId ON [Stores_MaterialRequisitionItems]([MaterialId]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MaterialRequisitionItems_Status')
    CREATE INDEX IX_MaterialRequisitionItems_Status ON [Stores_MaterialRequisitionItems]([Status]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MaterialRequisitionItems_JobCardId')
    CREATE INDEX IX_MaterialRequisitionItems_JobCardId ON [Stores_MaterialRequisitionItems]([JobCardId]);

PRINT 'Indexes created successfully';
PRINT 'Table alteration completed!';
