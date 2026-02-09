-- Alter Stores_MaterialRequisitionItems table to match code expectations
-- This adds missing columns and renames existing ones

USE MultiHitechERP
GO

-- Step 1: Add missing columns
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'LineNo')
BEGIN
    ALTER TABLE Stores_MaterialRequisitionItems
    ADD LineNo INT NULL
    PRINT 'Added LineNo column'
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'MaterialGrade')
BEGIN
    ALTER TABLE Stores_MaterialRequisitionItems
    ADD MaterialGrade NVARCHAR(100) NULL
    PRINT 'Added MaterialGrade column'
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'QuantityRequired')
BEGIN
    ALTER TABLE Stores_MaterialRequisitionItems
    ADD QuantityRequired DECIMAL(18, 4) NULL
    PRINT 'Added QuantityRequired column'
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'LengthRequiredMM')
BEGIN
    ALTER TABLE Stores_MaterialRequisitionItems
    ADD LengthRequiredMM DECIMAL(18, 4) NULL
    PRINT 'Added LengthRequiredMM column'
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'DiameterMM')
BEGIN
    ALTER TABLE Stores_MaterialRequisitionItems
    ADD DiameterMM DECIMAL(18, 4) NULL
    PRINT 'Added DiameterMM column'
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'NumberOfPieces')
BEGIN
    ALTER TABLE Stores_MaterialRequisitionItems
    ADD NumberOfPieces INT NULL
    PRINT 'Added NumberOfPieces column'
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'QuantityAllocated')
BEGIN
    ALTER TABLE Stores_MaterialRequisitionItems
    ADD QuantityAllocated DECIMAL(18, 4) NULL
    PRINT 'Added QuantityAllocated column'
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'QuantityIssued')
BEGIN
    ALTER TABLE Stores_MaterialRequisitionItems
    ADD QuantityIssued DECIMAL(18, 4) NULL
    PRINT 'Added QuantityIssued column'
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'QuantityPending')
BEGIN
    ALTER TABLE Stores_MaterialRequisitionItems
    ADD QuantityPending DECIMAL(18, 4) NULL
    PRINT 'Added QuantityPending column'
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'Status')
BEGIN
    ALTER TABLE Stores_MaterialRequisitionItems
    ADD Status NVARCHAR(50) NULL DEFAULT 'Pending'
    PRINT 'Added Status column'
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'AllocatedAt')
BEGIN
    ALTER TABLE Stores_MaterialRequisitionItems
    ADD AllocatedAt DATETIME NULL
    PRINT 'Added AllocatedAt column'
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'IssuedAt')
BEGIN
    ALTER TABLE Stores_MaterialRequisitionItems
    ADD IssuedAt DATETIME NULL
    PRINT 'Added IssuedAt column'
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'JobCardId')
BEGIN
    ALTER TABLE Stores_MaterialRequisitionItems
    ADD JobCardId INT NULL
    PRINT 'Added JobCardId column'
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'JobCardNo')
BEGIN
    ALTER TABLE Stores_MaterialRequisitionItems
    ADD JobCardNo NVARCHAR(50) NULL
    PRINT 'Added JobCardNo column'
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'ProcessId')
BEGIN
    ALTER TABLE Stores_MaterialRequisitionItems
    ADD ProcessId INT NULL
    PRINT 'Added ProcessId column'
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'ProcessName')
BEGIN
    ALTER TABLE Stores_MaterialRequisitionItems
    ADD ProcessName NVARCHAR(200) NULL
    PRINT 'Added ProcessName column'
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialRequisitionItems' AND COLUMN_NAME = 'CreatedAt')
BEGIN
    ALTER TABLE Stores_MaterialRequisitionItems
    ADD CreatedAt DATETIME NULL DEFAULT GETUTCDATE()
    PRINT 'Added CreatedAt column'
END

-- Step 2: Copy data from old columns to new ones (if there's existing data)
UPDATE Stores_MaterialRequisitionItems
SET
    QuantityRequired = RequestedQuantity,
    QuantityIssued = IssuedQuantity,
    QuantityPending = PendingQuantity,
    LineNo = 1,  -- Default line number
    Status = 'Pending'  -- Default status
WHERE QuantityRequired IS NULL

PRINT 'Data migrated from old columns to new columns'

-- Step 3: Make new columns NOT NULL after data migration
ALTER TABLE Stores_MaterialRequisitionItems
ALTER COLUMN QuantityRequired DECIMAL(18, 4) NOT NULL

ALTER TABLE Stores_MaterialRequisitionItems
ALTER COLUMN LineNo INT NOT NULL

ALTER TABLE Stores_MaterialRequisitionItems
ALTER COLUMN Status NVARCHAR(50) NOT NULL

ALTER TABLE Stores_MaterialRequisitionItems
ALTER COLUMN CreatedAt DATETIME NOT NULL

PRINT 'Made required columns NOT NULL'

-- Step 4: Create indexes for better performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MaterialRequisitionItems_RequisitionId')
BEGIN
    CREATE INDEX IX_MaterialRequisitionItems_RequisitionId
        ON Stores_MaterialRequisitionItems(RequisitionId)
    PRINT 'Created index on RequisitionId'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MaterialRequisitionItems_MaterialId')
BEGIN
    CREATE INDEX IX_MaterialRequisitionItems_MaterialId
        ON Stores_MaterialRequisitionItems(MaterialId)
    PRINT 'Created index on MaterialId'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MaterialRequisitionItems_Status')
BEGIN
    CREATE INDEX IX_MaterialRequisitionItems_Status
        ON Stores_MaterialRequisitionItems(Status)
    PRINT 'Created index on Status'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MaterialRequisitionItems_JobCardId')
BEGIN
    CREATE INDEX IX_MaterialRequisitionItems_JobCardId
        ON Stores_MaterialRequisitionItems(JobCardId)
    PRINT 'Created index on JobCardId'
END

PRINT 'Table alteration completed successfully!'

-- Optional: Drop old columns if you no longer need them
-- Uncomment these lines if you want to remove the old columns
/*
ALTER TABLE Stores_MaterialRequisitionItems DROP COLUMN RequestedQuantity
ALTER TABLE Stores_MaterialRequisitionItems DROP COLUMN IssuedQuantity
ALTER TABLE Stores_MaterialRequisitionItems DROP COLUMN PendingQuantity
ALTER TABLE Stores_MaterialRequisitionItems DROP COLUMN Purpose
PRINT 'Old columns dropped'
*/

GO
