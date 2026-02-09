-- Add missing columns to Stores_MaterialRequisitionItems
USE MultiHitechERP;

ALTER TABLE [Stores_MaterialRequisitionItems] ADD [LineNo] INT NULL;
ALTER TABLE [Stores_MaterialRequisitionItems] ADD [MaterialGrade] NVARCHAR(100) NULL;
ALTER TABLE [Stores_MaterialRequisitionItems] ADD [QuantityRequired] DECIMAL(18, 4) NULL;
ALTER TABLE [Stores_MaterialRequisitionItems] ADD [LengthRequiredMM] DECIMAL(18, 4) NULL;
ALTER TABLE [Stores_MaterialRequisitionItems] ADD [DiameterMM] DECIMAL(18, 4) NULL;
ALTER TABLE [Stores_MaterialRequisitionItems] ADD [NumberOfPieces] INT NULL;
ALTER TABLE [Stores_MaterialRequisitionItems] ADD [QuantityAllocated] DECIMAL(18, 4) NULL;
ALTER TABLE [Stores_MaterialRequisitionItems] ADD [QuantityIssued] DECIMAL(18, 4) NULL;
ALTER TABLE [Stores_MaterialRequisitionItems] ADD [QuantityPending] DECIMAL(18, 4) NULL;
ALTER TABLE [Stores_MaterialRequisitionItems] ADD [Status] NVARCHAR(50) NULL;
ALTER TABLE [Stores_MaterialRequisitionItems] ADD [AllocatedAt] DATETIME NULL;
ALTER TABLE [Stores_MaterialRequisitionItems] ADD [IssuedAt] DATETIME NULL;
ALTER TABLE [Stores_MaterialRequisitionItems] ADD [JobCardId] INT NULL;
ALTER TABLE [Stores_MaterialRequisitionItems] ADD [JobCardNo] NVARCHAR(50) NULL;
ALTER TABLE [Stores_MaterialRequisitionItems] ADD [ProcessId] INT NULL;
ALTER TABLE [Stores_MaterialRequisitionItems] ADD [ProcessName] NVARCHAR(200) NULL;
ALTER TABLE [Stores_MaterialRequisitionItems] ADD [CreatedAt] DATETIME NULL;

PRINT 'All columns added successfully';
