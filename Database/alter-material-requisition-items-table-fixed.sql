-- Alter Stores_MaterialRequisitionItems table to match code expectations
USE MultiHitechERP;

-- Step 1: Add missing columns
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'LineNo')
    ALTER TABLE [Stores_MaterialRequisitionItems] ADD [LineNo] INT NULL;

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'MaterialGrade')
    ALTER TABLE [Stores_MaterialRequisitionItems] ADD [MaterialGrade] NVARCHAR(100) NULL;

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'QuantityRequired')
    ALTER TABLE [Stores_MaterialRequisitionItems] ADD [QuantityRequired] DECIMAL(18, 4) NULL;

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'LengthRequiredMM')
    ALTER TABLE [Stores_MaterialRequisitionItems] ADD [LengthRequiredMM] DECIMAL(18, 4) NULL;

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'DiameterMM')
    ALTER TABLE [Stores_MaterialRequisitionItems] ADD [DiameterMM] DECIMAL(18, 4) NULL;

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'NumberOfPieces')
    ALTER TABLE [Stores_MaterialRequisitionItems] ADD [NumberOfPieces] INT NULL;

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'QuantityAllocated')
    ALTER TABLE [Stores_MaterialRequisitionItems] ADD [QuantityAllocated] DECIMAL(18, 4) NULL;

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'QuantityIssued')
    ALTER TABLE [Stores_MaterialRequisitionItems] ADD [QuantityIssued] DECIMAL(18, 4) NULL;

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'QuantityPending')
    ALTER TABLE [Stores_MaterialRequisitionItems] ADD [QuantityPending] DECIMAL(18, 4) NULL;

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'Status')
    ALTER TABLE [Stores_MaterialRequisitionItems] ADD [Status] NVARCHAR(50) NULL DEFAULT 'Pending';

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'AllocatedAt')
    ALTER TABLE [Stores_MaterialRequisitionItems] ADD [AllocatedAt] DATETIME NULL;

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'IssuedAt')
    ALTER TABLE [Stores_MaterialRequisitionItems] ADD [IssuedAt] DATETIME NULL;

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'JobCardId')
    ALTER TABLE [Stores_MaterialRequisitionItems] ADD [JobCardId] INT NULL;

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'JobCardNo')
    ALTER TABLE [Stores_MaterialRequisitionItems] ADD [JobCardNo] NVARCHAR(50) NULL;

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'ProcessId')
    ALTER TABLE [Stores_MaterialRequisitionItems] ADD [ProcessId] INT NULL;

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'ProcessName')
    ALTER TABLE [Stores_MaterialRequisitionItems] ADD [ProcessName] NVARCHAR(200) NULL;

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'CreatedAt')
    ALTER TABLE [Stores_MaterialRequisitionItems] ADD [CreatedAt] DATETIME NULL DEFAULT GETUTCDATE();

-- Step 2: Copy data from old columns to new ones
UPDATE [Stores_MaterialRequisitionItems]
SET
    [QuantityRequired] = [RequestedQuantity],
    [QuantityIssued] = [IssuedQuantity],
    [QuantityPending] = [PendingQuantity],
    [LineNo] = 1,
    [Status] = 'Pending',
    [CreatedAt] = GETUTCDATE()
WHERE [QuantityRequired] IS NULL;

-- Step 3: Make new columns NOT NULL after data migration
ALTER TABLE [Stores_MaterialRequisitionItems]
ALTER COLUMN [QuantityRequired] DECIMAL(18, 4) NOT NULL;

ALTER TABLE [Stores_MaterialRequisitionItems]
ALTER COLUMN [LineNo] INT NOT NULL;

ALTER TABLE [Stores_MaterialRequisitionItems]
ALTER COLUMN [Status] NVARCHAR(50) NOT NULL;

ALTER TABLE [Stores_MaterialRequisitionItems]
ALTER COLUMN [CreatedAt] DATETIME NOT NULL;

-- Step 4: Create indexes for better performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MaterialRequisitionItems_RequisitionId')
    CREATE INDEX IX_MaterialRequisitionItems_RequisitionId ON [Stores_MaterialRequisitionItems]([RequisitionId]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MaterialRequisitionItems_MaterialId')
    CREATE INDEX IX_MaterialRequisitionItems_MaterialId ON [Stores_MaterialRequisitionItems]([MaterialId]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MaterialRequisitionItems_Status')
    CREATE INDEX IX_MaterialRequisitionItems_Status ON [Stores_MaterialRequisitionItems]([Status]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MaterialRequisitionItems_JobCardId')
    CREATE INDEX IX_MaterialRequisitionItems_JobCardId ON [Stores_MaterialRequisitionItems]([JobCardId]);

PRINT 'Table alteration completed successfully!';
